using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Appleseed.Framework.Scheduler
{
    //Author: Federico Dal Maso
    //e-mail: ifof@libero.it
    //date: 2003-06-17

    /// <summary>
    /// Describe a Task
    /// </summary>
    public class SchedulerTask
    {
        private int _idOwner;
        private int _idTarget;
        private object _arg;
        private string _description;
        private DateTime _dueTime;
        private int _idTask;

        private const int MAX_DESCRIPTION_LENGTH = 150; //see db

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idModuleOwner"></param>
        /// <param name="idModuleTarget"></param>
        /// <param name="dueTime"></param>
        public SchedulerTask(int idModuleOwner, int idModuleTarget, DateTime dueTime)
        {
            _idOwner = idModuleOwner;
            _idTarget = idModuleTarget;
            _dueTime = dueTime;
            _idTask = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idModuleOwner"></param>
        /// <param name="idModuleTarget"></param>
        /// <param name="dueTime"></param>
        /// <param name="description"></param>
        public SchedulerTask(int idModuleOwner, int idModuleTarget, DateTime dueTime, string description)
            : this(idModuleOwner, idModuleTarget, dueTime)
        {
            _description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idModuleOwner"></param>
        /// <param name="idModuleTarget"></param>
        /// <param name="dueTime"></param>
        /// <param name="argument"></param>
        public SchedulerTask(int idModuleOwner, int idModuleTarget, DateTime dueTime, object argument)
            : this(idModuleOwner, idModuleTarget, dueTime)
        {
            if (!argument.GetType().IsSerializable)
                throw new ApplicationException("argument parameter must be a serializable type");
            _arg = argument;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idModuleOwner"></param>
        /// <param name="idModuleTarget"></param>
        /// <param name="dueTime"></param>
        /// <param name="description"></param>
        /// <param name="argument"></param>
        public SchedulerTask(int idModuleOwner, int idModuleTarget, DateTime dueTime, string description,
                             object argument) : this(idModuleOwner, idModuleTarget, dueTime, description)
        {
            if (!argument.GetType().IsSerializable)
                throw new ApplicationException("argument parameter must be a serializable type");
            _arg = argument;
        }

        internal SchedulerTask(int iDTask, int idModuleOwner, int idModuleTarget, DateTime dueTime, string description,
                               object argument) : this(idModuleOwner, idModuleTarget, dueTime, description, argument)
        {
            _idTask = iDTask;
        }

        internal SchedulerTask(IDataReader dr)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ss = new MemoryStream((byte[]) dr["Argument"]);
            _arg = bf.Deserialize(ss);
            ss.Close();
            _idTask = (int) dr["IDTask"];
            _idOwner = (int) dr["IDModuleOwner"];
            _idTarget = (int) dr["IDModuleTarget"];
            _dueTime = (DateTime) dr["DueTime"];
            _description = (string) dr["Description"];
        }


        /// <summary>
        /// 
        /// </summary>
        public object Argument
        {
            get { return _arg; }
            set
            {
                if (!value.GetType().IsSerializable)
                    throw new ApplicationException("argument parameter must be a serializable type");
                _arg = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DueTime
        {
            get { return _dueTime; }
            set { _dueTime = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int IDModuleOwner
        {
            get { return _idOwner; }
            set { _idOwner = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int IDModuleTarget
        {
            get { return _idTarget; }
            set { _idTarget = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                if (_description.Length > MAX_DESCRIPTION_LENGTH)
                    _description = _description.Substring(0, MAX_DESCRIPTION_LENGTH);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int IDTask
        {
            get { return _idTask; }
        }

        internal void SetIDTask(int id)
        {
            _idTask = id;
        }
    }
}