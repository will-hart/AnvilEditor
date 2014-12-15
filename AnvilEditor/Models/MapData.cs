namespace AnvilEditor.Models
{
    using System;
    using System.Collections.Generic;

    public class MapData
    {
        internal string ImageName;
        internal int MapXMin;
        internal int MapXMax;
        internal int MapYMin;
        internal int MapYMax;
        internal string Credits;
        internal string DownloadUrl;
        internal List<string> Addons;
        internal string MapAlias;

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
