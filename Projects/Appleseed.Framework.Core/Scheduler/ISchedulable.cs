namespace Appleseed.Framework.Scheduler
{
    //Author: Federico Dal Maso
    //e-mail: ifof@libero.it
    //date: 2003-06-17

    /// <summary>
    /// Must be implemented by module who want use scheduler callback feature.
    /// </summary>
    public interface ISchedulable
    {
        /// <summary>
        /// Called when a task occurs
        /// </summary>
        /// <param name="task"></param>
        void ScheduleDo(SchedulerTask task);

        /// <summary>
        /// Called after ScheduleDo if it doesn't throw any exception
        /// </summary>
        /// <param name="task"></param>
        void ScheduleCommit(SchedulerTask task);

        /// <summary>
        /// Called after ScheduleDo if it throws an exception
        /// </summary>
        /// <param name="task"></param>
        void ScheduleRollback(SchedulerTask task);
    }
}