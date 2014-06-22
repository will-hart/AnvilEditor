using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows;

namespace AnvilEditor
{
    internal struct VersionInformation
    {
        internal int version;
        internal string url;
    };

    /// <summary>
    /// Interaction logic for FrameworkUpdater.xaml
    /// </summary>
    public partial class FrameworkUpdater : Window
    {
        /// <summary>
        /// The URL from which to request the latest framework version
        /// </summary>
        private string requestUrl = "http://www.anvilproject.com/downloads/files/version.json";

        /// <summary>
        /// Holds version information downloaded from the web server
        /// </summary>
        private VersionInformation versionInfo;

        public FrameworkUpdater()
        {
            InitializeComponent();

            // start the request
            this.CheckForUpdates();
        }

        /// <summary>
        /// Checks the remote URL for the latest version and checks if an update is required
        /// </summary>
        private void CheckForUpdates()
        {
            var request = WebRequest.Create(this.requestUrl);
            request.ContentType = "application/json; charset=utf-8";
            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                this.StatusLabel.Content = "Unable to connect to update server.  The error message was " + Environment.NewLine + Environment.NewLine + ex.Message;
                return;
            }
            
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var json = sr.ReadToEnd();
                this.versionInfo = JsonConvert.DeserializeObject<VersionInformation>(json);
            }

            var currentVersion = AnvilEditor.Properties.Settings.Default.FrameworkVersion;

            if (this.versionInfo.version > currentVersion)
            {
                this.StatusLabel.Content = "A newer version is available. Click 'Download' to upgrade the framework from v" + currentVersion.ToString() +
                    " to v" + this.versionInfo.version.ToString();
                this.DownloadUpdateButton.IsEnabled = true;
            }
            else
            {
                this.StatusLabel.Content = "You are already on the latest framework version";
            }
        }

        /// <summary>
        /// Downloads a zipped mission from the given URL and extracts it to the mission_raw folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadFrameworkUpdate(object sender, RoutedEventArgs e)
        {

            var tempPath = Path.Combine(Path.GetTempPath(), "AnvilFramework_v" + this.versionInfo.version.ToString() + ".zip");

            // download the file
            using (var client = new WebClient())
            {
                client.DownloadFile(this.versionInfo.url, tempPath);
            }

            // unzip to mission_raw folder
            var frameworkPath = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "mission_raw");
            using (FileStream zipToOpen = new FileStream(tempPath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(frameworkPath);
                }
            }

            // delete the temp zip archive
            File.Delete(tempPath);

            // update the version number
            AnvilEditor.Properties.Settings.Default.FrameworkVersion = this.versionInfo.version;
            AnvilEditor.Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Closes the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
