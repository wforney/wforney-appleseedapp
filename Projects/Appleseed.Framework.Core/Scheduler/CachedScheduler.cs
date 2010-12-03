using System;
using System.Collections;
using System.Data;

namespace Appleseed.Framework.Scheduler
{
    //Author: Federico Dal Maso
    //e-mail: ifof@libero.it
    //date: 2003-06-17

    /// <summary>
    /// Implementation of IScheduler with in-memory cache.
    /// CacheScheduler uses an internal SortedList fill with a queue of nearest to occur tasks (ordered by dueTime)
    /// Timer can check tasks list very often because it has to check only first element of sortedList and not send any request to database.
    /// When a task occurs (because of DueTime expires) task is executed and removed from db and from cache.
    /// When a module add a task, it's added to db and to cache.
    /// When and only when cache is empty, scheduler performs a SELECT in db and refill the cache.
    /// </summary>
    public class CachedScheduler : SimpleScheduler
    {
        private class TaskComparer : IComparer
        {
            /// <summary>
            /// Compare two tasks first order by dueTime. If dueTimes are equal they are ordered by IDTask.
            /// Used in sortedlist.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(object x, object y)
            {
                SchedulerTask xtsk = x as SchedulerTask;
                SchedulerTask ytsk = y as SchedulerTask;
                if (x == null || y == null)
                    throw new ArgumentException("object is not a SchedulerTask");

                if (xtsk.DueTime < ytsk.DueTime)
                    return -1;
                if (xtsk.DueTime > ytsk.DueTime)
                    return 1;
                if (xtsk.DueTime == ytsk.DueTime)
                {
                    if (xtsk.IDTask < ytsk.IDTask)
                        return -1;
                    if (xtsk.IDTask > ytsk.IDTask)
                        return 1;
                    if (xtsk.IDTask == ytsk.IDTask)
                        return 0;
                }

                throw new ArgumentException("Impossible exception"); //... to compile.
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected SortedList _cache;

        private static volatile CachedScheduler _theScheduler = null; //the scheduler instance

        /// <summary>
        /// Get the scheduler, using a singleton pattern.
        /// </summary>
        /// <param name="applicationMapPath">usually HttpContext.Current.Server.MapPath(PortalSettings.ApplicationPath)</param>
        /// <param name="connection">db connection</param>
        /// <param name="period">scheduler timer milliseconds</param>
        /// <param name="cacheSize">max number of in-memory tasks</param>
        /// <returns></returns>
        public static CachedScheduler GetScheduler(string applicationMapPath, IDbConnection connection, long period,
                                                   int cacheSize)
        {
            //Sigleton
            if (_theScheduler == null)
            {
                lock (typeof (CachedScheduler))
                {
                    if (_theScheduler == null)
                        _theScheduler = new CachedScheduler(applicationMapPath, connection, period, cacheSize);
                }
            }

            return _theScheduler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="applicationMapPath"></param>
        /// <param name="connection"></param>
        /// <param name="period"></param>
        /// <param name="cacheSize"></param>
        protected CachedScheduler(string applicationMapPath, IDbConnection connection, long period, int cacheSize)
            : base(applicationMapPath, connection, period)
        {
            _cache = new SortedList(new TaskComparer(), cacheSize);
            FillCache();
        }

        /// <summary>
        /// Fill internal tasks cache
        /// </summary>
        public void FillCache()
        {
            using (IDataReader dr = localSchDB.GetOrderedTask())
            {
                SchedulerTask tsk;

                while (dr.Read() && _cache.Count < _cache.Capacity)
                {
                    tsk = new SchedulerTask(dr);

                    lock (_cache.SyncRoot)
                    {
                        _cache.Add(tsk, tsk);
                    }
                }
                dr.Close();
            }
        }

        /// <summary>
        /// Insert a new task
        /// </summary>
        /// <param name="task"></param>
        /// <remarks>After a new task is inserted it obtains a IDTask. Before it's -1.</remarks>
        /// <returns></returns>
        public override SchedulerTask InsertTask(SchedulerTask task)
        {
            if (task.IDTask != -1)
                throw new SchedulerException("Could not insert an inserted task");

            task.SetIDTask(localSchDB.InsertTask(task));
            if (_cache.Count != 0 && task.DueTime < ((SchedulerTask) _cache.GetKey(_cache.Count - 1)).DueTime)
            {
                lock (_cache.SyncRoot)
                {
                    _cache.RemoveAt(_cache.Count - 1);
                    _cache.Add(task, task);
                }
            }

            return task;
        }

        /// <summary>
        /// Remove tasks
        /// </summary>
        /// <param name="task"></param>
        public override void RemoveTask(SchedulerTask task)
        {
            if (task.IDTask == -1)
                return;

            //remoce from DB
            base.RemoveTask(task);

            //remove from cache
            lock (_cache.SyncRoot)
            {
                _cache.Remove(task);
            }
        }

        /// <summary>
        /// Performs a schedulation. Called by timer.
        /// </summary>
        /// <param name="timerState"></param>
        protected override void Schedule(object timerState)
        {
            lock (this)
            {
                localTimerState.Counter++;
                Stop(); //Stop timer while scheduler works
                while (_cache.Count != 0)
                {
                    SchedulerTask task = (SchedulerTask) _cache.GetKey(0);
                    if (task.DueTime > DateTime.Now)
                        break;
                    try
                    {
                        ExecuteTask(task);
                    }
                    catch
                    {
                        //TODO: We have to apply some policy here...
                        //i.e. Move failed tasks on a log, call a Module feedback interface,....
                        //now task is removed always
                    }
                    RemoveTask(task);
                }
                if (_cache.Count == 0)
                {
                    FillCache();
                    if (_cache.Count == 0)
                        return; //avoid loop in case of empty tasks-queue in db.
                    Schedule(timerState);
                }
                Start(); //restart timer
            }
        }
    }
}