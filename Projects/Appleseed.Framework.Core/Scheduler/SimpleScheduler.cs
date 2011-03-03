using System;
using System.Collections;
using System.Data;
using System.Threading;

namespace Appleseed.Framework.Scheduler
{
    //Author: Federico Dal Maso
    //e-mail: ifof@libero.it
    //date: 2003-06-17

    /// <summary>
    /// SimpleScheduler, perform a select every call to Scheduler
    /// Usefull only for long period timer.
    /// </summary>
    public class SimpleScheduler : IScheduler
    {
        /// <summary>
        /// 
        /// </summary>
        protected class TimerState
        {
            private int counter;

            /// <summary>
            /// 
            /// </summary>
            public int Counter
            {
                get { return counter; }
                set { counter = value; }
            }

            // = 0;

            private Timer timer;

            /// <summary>
            /// 
            /// </summary>
            public Timer Timer
            {
                get { return timer; }
                set { timer = value; }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static volatile SimpleScheduler _theScheduler = null;

        /// <summary>
        /// 
        /// </summary>
        internal SchedulerDB localSchDB;

        /// <summary>
        /// 
        /// </summary>
        protected TimerState localTimerState;

        /// <summary>
        /// 
        /// </summary>
        protected long localPeriod;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationMapPath"></param>
        /// <param name="connection"></param>
        /// <param name="period"></param>
        protected SimpleScheduler(string applicationMapPath, IDbConnection connection, long period)
        {
            localSchDB = new SchedulerDB(connection, applicationMapPath);
            localPeriod = period;

            localTimerState = new TimerState();

            Timer t = new Timer(
                new TimerCallback(Schedule),
                localTimerState,
                Timeout.Infinite,
                Timeout.Infinite);

            localTimerState.Timer = t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationMapPath"></param>
        /// <param name="connection"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static SimpleScheduler GetScheduler(string applicationMapPath, IDbConnection connection, long period)
        {
            //Sigleton
            if (_theScheduler == null)
            {
                lock (typeof (CachedScheduler))
                {
                    if (_theScheduler == null)
                        _theScheduler = new SimpleScheduler(applicationMapPath, connection, period);
                }
            }

            return _theScheduler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timerState"></param>
        protected virtual void Schedule(object timerState)
        {
            lock (this)
            {
                localTimerState.Counter++;

                SchedulerTask[] tsks = localSchDB.GetExpiredTask();

                Stop(); //Stop the timer while it works

                foreach (SchedulerTask tsk in tsks)
                {
                    try
                    {
                        ExecuteTask(tsk);
                    }
                    catch
                    {
                        //TODO: We have to apply some policy here...
                        //i.e. Move failed tasks on a log, call a Module feedback interface,....
                        //now task is removed always
                    }

                    RemoveTask(tsk);
                }

                Start(); //restart the timer
            }
        }

        /// <summary>
        /// Start the scheduler timer
        /// </summary>
        public virtual void Start()
        {
            localTimerState.Timer.Change(localPeriod, localPeriod);
        }

        /// <summary>
        /// Stop the scheduler timer
        /// </summary>
        public virtual void Stop()
        {
            localTimerState.Timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Get or set the timer period.
        /// </summary>
        public virtual long Period
        {
            get { return localPeriod; }
            set
            {
                localPeriod = value;
                localTimerState.Timer.Change(0L, localPeriod);
            }
        }

        /// <summary>
        /// Get an array of tasks of the specified module owner
        /// </summary>
        /// <param name="idModuleOwner"></param>
        /// <returns></returns>
        public virtual SchedulerTask[] GetTasksByOwner(int idModuleOwner)
        {
            IDataReader dr = localSchDB.GetTasksByOwner(idModuleOwner);
            ArrayList ary = new ArrayList();
            while (dr.Read())
                ary.Add(new SchedulerTask(dr));
            dr.Close();

            return (SchedulerTask[]) ary.ToArray(typeof (SchedulerTask));
        }

        /// <summary>
        /// Get an array of tasks of the specified module target
        /// </summary>
        /// <param name="idModuleTarget"></param>
        /// <returns></returns>
        public virtual SchedulerTask[] GetTasksByTarget(int idModuleTarget)
        {
            IDataReader dr = localSchDB.GetTasksByOwner(idModuleTarget);
            ArrayList ary = new ArrayList();
            while (dr.Read())
                ary.Add(new SchedulerTask(dr));
            dr.Close();

            return (SchedulerTask[]) ary.ToArray(typeof (SchedulerTask));
        }

        /// <summary>
        /// Insert a new task
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public virtual SchedulerTask InsertTask(SchedulerTask task)
        {
            if (task.IDTask != -1)
                throw new SchedulerException("Could not insert an inserted task");
            task.SetIDTask(localSchDB.InsertTask(task));
            return task;
        }

        /// <summary>
        /// Remove a task
        /// </summary>
        /// <param name="task"></param>
        public virtual void RemoveTask(SchedulerTask task)
        {
            if (task.IDTask == -1)
                return;

            localSchDB.RemoveTask(task.IDTask);
            return;
        }

        /// <summary>
        /// Call the correct ISchedulable methods of a target module assigned to the task.
        /// </summary>
        /// <param name="task"></param>
        protected void ExecuteTask(SchedulerTask task)
        {
            ISchedulable module;
            try
            {
                module = localSchDB.GetModuleInstance(task.IDModuleTarget);
            }
            catch
            {
                //TODO:
                return;
            }

            try
            {
                module.ScheduleDo(task);
            }
            catch (Exception ex)
            {
                try
                {
                    module.ScheduleRollback(task);
                }
                catch (Exception ex2)
                {
                    throw new SchedulerException("ScheduleDo fail. Rollback fails", ex2);
                }
                throw new SchedulerException("ScheduleDo fails. Rollback called successfully", ex);
            }

            try
            {
                module.ScheduleCommit(task);
            }
            catch (Exception ex)
            {
                throw new SchedulerException("ScheduleDo called successfully. Commit fails", ex);
            }
        }
    }
}