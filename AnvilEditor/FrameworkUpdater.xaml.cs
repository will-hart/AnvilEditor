namespace AnvilEditor
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Windows;

    using NLog;

    internal struct VersionInformation
    {
        internal int version;
        internal string url;
        internal bool partial;
        internal bool breaking;
    };

    /// <summary>
    /// Interaction logic for FrameworkUpdater.xaml
    /// </summary>
    public partial class FrameworkUpdater
    {
        /// <summary>
        /// A debug logger
        /// </summary>
        private static Logger Log = LogManager.GetLogger("FrameworkUpdater");
            
        /// <summary>
        /// The URL from which to request the latest framework version
        /// </summary>
        private string requestUrl = "http://www.anvilproject.com/downloads/files/version.json";
        
        /// <summary>
        /// Holds version information downloaded from the web server
        /// </summary>
        private VersionInformation versionInfo;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public FrameworkUpdater()
        {
            InitializeComponent();

            // start the request
            Log.Debug("Checking for framework updates");
            this.CheckForUpdates();

            this.Downloaded = false;

            this.versionInfo.breaking = false;
            this.versionInfo.partial = false;
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
                Log.Error("  - Unable to connect to update server.  The error message was: " + ex.Message);
                this.StatusLabel.Content = "Unable to connect to update server.  The error message was " + Environment.NewLine + Environment.NewLine + ex.Message;
                return;
            }

            Log.Debug("  - Found update data, now processing to find version number");
            
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                var json = sr.ReadToEnd();
                var o = JObject.Parse(json);

                foreach (var prop in o.Properties())
                {
                    if (prop.Name == "version")
                    {
                        this.versionInfo.version = prop.Value.ToObject<int>();
                    }
                    else if (prop.Name == "url")
                    {
                        this.versionInfo.url = prop.Value.ToObject<string>();
                    }
                    else if (prop.Name == "breaking")
                    {
                        this.versionInfo.breaking = prop.Value.ToObject<bool>();
                    }
                    else if (prop.Name == "partial")
                    {
                        this.versionInfo.partial = prop.Value.ToObject<bool>();
                    }
                }
            }

            var currentVersion = AnvilEditor.Properties.Settings.Default.FrameworkVersion;

            Log.Debug("  - Current version {0}, latest version {1}", currentVersion, this.versionInfo.version);
            Log.Debug("  - New FW version has " + (this.versionInfo.partial ? "partial" : "full") + " support with this editor version");
            Log.Debug("  - New FW version has is " + (this.versionInfo.breaking ? "" : "not ") + " broken with this editor version");

            if (this.versionInfo.version <= currentVersion)
            {
                this.StatusLabel.Content = "You are already on the latest framework version - v" + this.versionInfo.version.ToString();
                return;
            }

            if (this.versionInfo.version > currentVersion && !this.versionInfo.breaking)
            {
                this.StatusLabel.Content = "A newer version is available. Click 'Download' to upgrade the framework from v" + currentVersion.ToString() +
                    " to v" + this.versionInfo.version.ToString() + 
                    (this.versionInfo.partial ? ". Support for this version is incomplete with your editor version - you may want to upgrade the editor" : "");
                this.DownloadUpdateButton.IsEnabled = true;
                return;
            }
            this.StatusLabel.Content = "Unfortunately the framework v" + this.versionInfo.version.ToString() + " is incompatible with your editor version. " + Environment.NewLine + 
                " To continue you will need to update to the latest version of the editor which can be found at www.anvilproject.com/downloads";
            
        }

        /// <summary>
        /// Downloads a zipped mission from the given URL and extracts it to the mission_raw folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadFrameworkUpdate(object sender, RoutedEventArgs e)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "AnvilFramework_v" + this.versionInfo.version.ToString() + ".zip");
            Log.Debug("  - User requested framework download");
            Log.Debug("  - Saving downloaded zip file to {0}", tempPath);

            // download the file
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(this.versionInfo.url, tempPath);
                }
            }
            catch (WebException ex)
            {
                Log.Error("  - Unable to download from {0}", this.versionInfo.url);
                Log.Error("  - The error message was: {0}", ex.Message);
                this.StatusLabel.Content = "Unable to connect to update server.  The error message was " + Environment.NewLine + Environment.NewLine + ex.Message;
                return;
            }

            // unzip to mission_raw folder
            var frameworkPath = Path.Combine(
                Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "mission_raw");
            Log.Debug("  - Starting framework download to {0}", frameworkPath);

            FileUtilities.EmptyMissionDirectory(frameworkPath, false);
            Log.Debug("  - Removed old framework");

            using (FileStream zipToOpen = new FileStream(tempPath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    archive.ExtractToDirectory(frameworkPath);
                }
            }

            // delete the temp zip archive
            Log.Debug("  - Removed temporary zip file");
            File.Delete(tempPath);

            // update the version number
            AnvilEditor.Properties.Settings.Default.FrameworkVersion = this.versionInfo.version;
            AnvilEditor.Properties.Settings.Default.Save();
            Log.Debug("  - Finished updating to version {0}", this.versionInfo.version);

            this.Downloaded = true;
        }

        /// <summary>
        /// Closes the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Log.Debug("  - Update dialog closed");
            this.Close();
        }

        /// <summary>
        /// Gets a flag indicating whether the dialog installed a new framework fersion
        /// </summary>
        internal bool Downloaded { get; private set; }
    }
}
