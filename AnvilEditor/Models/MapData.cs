namespace AnvilEditor.Models
{
    using System;
    using Newtonsoft.Json;

    public class MapData
    {
        public string ImageName { get; set; }
        public int MapXMin { get; set; }
        public int MapXMax { get; set; }
        public int MapYMin { get; set; }
        public int MapYMax { get; set; }
        public string Credits { get; set; }
        public virtual string DownloadUrl { get; set; }
        public string Addons { get; set; }
        public string MapAlias { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MapData()
        {
            this.IsDownloaded = false;
            this.DownloadUrl = string.Empty;
            this.MapAlias = string.Empty;
        }

        public override string ToString()
        {
            return this.Credits + Environment.NewLine + Environment.NewLine +
                "Map X Minimum: " + this.MapXMin + Environment.NewLine +
                "Map X Maximum: " + this.MapXMax + Environment.NewLine +
                "Map Y Minimum: " + this.MapYMin + Environment.NewLine +
                "Map Y Maximum: " + this.MapYMax + Environment.NewLine +
                "Download URL: " + this.DownloadUrl + Environment.NewLine +
                "Image Path: /data/maps/" + this.ImageName;
        }

        /// <summary>
        /// Gets a value indicating whether the map image can be downloaded
        /// </summary>
        [JsonIgnore]
        public bool IsDownloadable
        {
            get
            {
                return this.DownloadUrl != string.Empty && !this.IsDownloaded;
            }
        }

        [JsonIgnore]
        public bool IsDownloaded { get; set; }
    }
}
