using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ResearchCollector
{
    /// <summary>
    /// General class containg some fields and methods for classes that will do some work (i.e. filter/importer)
    /// </summary>
    abstract class Worker
    {
        /// <summary>
        /// Reference to the background worker this is being run on to report progress
        /// </summary>
        protected BackgroundWorker worker;
        public bool logActions;

        /// <summary>
        /// Current progress
        /// </summary>
        protected double progress;
        /// <summary>
        /// How much the progress is incremented with each publication/file that is parsed
        /// </summary>
        protected double progressIncrement;
        /// <summary>
        /// Previous progress 
        /// </summary>
        protected int prevProgress;

        /// <summary>
        /// Event that is raised when an action was completed (like a publication being parsed and added to the output)
        /// </summary>
        public event EventHandler<ActionCompletedEventArgs> ActionCompleted;
        /// <summary>
        /// Context used for posting messages to the UI (since <see cref="Run(string, string, BackgroundWorker)"/> will be run an a separate background thread)
        /// </summary>
        private readonly SynchronizationContext context;

        public Worker(SynchronizationContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Report to the UI a description of what is being done right now
        /// </summary>
        protected void ReportAction(string description)
        {
            if (logActions)
                context.Post(new SendOrPostCallback(RaiseActionEvent), description);
        }

        /// <summary>
        /// Event to be raised from within the context of the UI
        /// </summary>
        /// <param name="state"></param>
        private void RaiseActionEvent(object state)
        {
            ActionCompleted(this, new ActionCompletedEventArgs((string)state));
        }

        /// <summary>
        /// Increment progress and inform the UI if progress has been made beyond a few decimals
        /// </summary>
        protected void UpdateProgress()
        {
            progress = Math.Min(100.0, progress + progressIncrement);
            if ((int)progress > prevProgress)
            {
                prevProgress = (int)progress;
                worker.ReportProgress(prevProgress);
            }
        }

        /// <summary>
        /// Run the current worker
        /// </summary>
        /// <param name="worker"></param>
        public abstract void Run(BackgroundWorker worker);
    }

    public class ActionCompletedEventArgs : EventArgs
    {
        public string description;

        public ActionCompletedEventArgs(string description)
        {
            this.description = description;
        }
    }
}
