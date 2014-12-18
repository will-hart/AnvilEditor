namespace AnvilEditor.Models
{
    using System;
    using System.Collections.Generic;

    public class MapData
    {
        public string ImageName { get; internal set; }
        public int MapXMin { get; internal set; }
        public int MapXMax { get; internal set; }
        public int MapYMin { get; internal set; }
        public int MapYMax { get; internal set; }
        public string Credits { get; internal set; }
        public virtual string DownloadUrl { get; internal set; }
        public List<string> Addons { get; internal set; }
        public string MapAlias { get; internal set; }

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
        public bool IsDownloadable
        {
            get
            {
                return this.DownloadUrl != string.Empty && !this.IsDownloaded;
            }
        }

        public bool IsDownloaded { get; set; }
    }
}
