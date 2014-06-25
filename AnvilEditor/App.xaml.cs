using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using NLog;

namespace AnvilEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Create a global logger
        /// </summary>
        private static Logger Log = LogManager.GetLogger("MainWindow");

        public App()
            : base()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("The application has experienced an error and will probably have to close. Check the /logs directory" +
                " and feel free to bring the contents to the attention of the developer");
            Log.Fatal(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("The application has experienced an error and will probably have to close. Check the /logs directory" +
                " and feel free to bring the contents to the attention of the developer");
            Log.Fatal(e.ExceptionObject);
        }
    }
}
