using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using NLog;

using SharpRaven;

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
            this.CaptureException(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal(e.ExceptionObject);
            this.CaptureException(e.ExceptionObject as Exception);
        }

        private void CaptureException(Exception e)
        {
            var result = MessageBox.Show("The application has experienced an error and will probably have to close. Would you like to send an anonymous error report to the developer? " +
                "If not, click no and check the /logs directory and feel free to bring the contents to the attention of the developer", "Error! Do you want to report it?",
                MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                var ravenClient = new RavenClient("https://4510c8a1199545abb190d5e63d15885f:f0ca1031909b49899ee4b0898a397826@anvilsentry.herokuapp.com/2");
                ravenClient.CaptureException(e);
            }
        }
    }
}
