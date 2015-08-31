namespace AnvilEditor.Windows
{ 
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for LongRunningTaskProgressDialog.xaml
    /// </summary>
    public partial class LongRunningTask : Window
    {
        private IProgress<int> progress;

        public LongRunningTask()
        {
            InitializeComponent();


            progress = new Progress<int>((b) =>
            {
                TaskProgressLabel.Content = b.ToString() + "%";
                TaskProgressBar.Value = b;
            });
        }

        /// <summary>
        /// Show a indeterminate dialog but don't perform any tasks
        /// </summary>
        /// <param name="message"></param>
        public void StartEmpty(string message)
        {
            TaskProgressLabel.Content = message;
            TaskProgressBar.IsIndeterminate = true;
        }

        /// <summary>
        /// Start an indeterminate long running task
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="process"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<T> Start<T>(Func<T> process, string message)
        {
            TaskProgressBar.IsIndeterminate = true;
            TaskProgressLabel.Content = message;

            return Task.Run(() =>
            {
                return process();
            });
        }

        /// <summary>
        /// Start a long running task which runs over a list of items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="itemHandler"></param>
        /// <returns></returns>
        public Task<int> Start<T>(List<T> items, Func<T, int> itemHandler)
        {
            var max = items.Count;
            int processed = 0;

            // perform the task
            return Task.Run(() =>
            {
                foreach (var item in items)
                {
                    itemHandler(item);

                    if (progress != null)
                    {
                        progress.Report(processed * 100 / max);
                    }

                    ++processed;
                }

                return processed;
            });
        }

    }
}
