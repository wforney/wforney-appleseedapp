namespace Appleseed.Framework.Scheduler
{
    //Author: Federico Dal Maso
    //e-mail: ifof@libero.it
    //date: 2003-06-17
    /// <summary>
    /// Standard interface for a scheduler
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Start the scheduler
        /// </summary>
        void Start();

        /// <summary>
        /// Stop scheduler activities
        /// </summary>
        void Stop();

        /// <summary>
        /// Get or set the scheduler timer period
        /// </summary>
        long Period { get; set; }

        /// <summary>
        /// Get an array of tasks of the specified module owner
        /// </summary>
        SchedulerTask[] GetTasksByTarget(int moduleTargetId);

        /// <summary>
        /// Get an array of tasks of the specified module target
        /// </summary>
        SchedulerTask[] GetTasksByOwner(int moduleOwnerId);

        /// <summary>
        /// Insert a new task
        /// </summary>
        SchedulerTask InsertTask(SchedulerTask task);

        /// <summary>
        /// Remove a task
        /// </summary>
        /// <param name="task"></param>
        void RemoveTask(SchedulerTask task);
    }
}