using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilEditor.Models
{
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

        public override string ToString()
        {
            return this.Credits + Environment.NewLine + Environment.NewLine +
                "Map X Minimum: " + this.MapXMin + Environment.NewLine +
                "Map X Maximum: " + this.MapXMax + Environment.NewLine +
                "Map Y Minimum: " + this.MapYMin + Environment.NewLine +
                "Map Y Maximum: " + this.MapYMax + Environment.NewLine +
                "Download URL: " + (this.DownloadUrl == null ? "Not available" : this.DownloadUrl) + Environment.NewLine +
                "Image Path: /data/maps/" + this.ImageName;
        }
    }

    internal class MapDefinitions
    {
        internal static readonly Dictionary<string, MapData> Maps = new Dictionary<string, MapData>()
        {
            { 
                "Altis", 
                new MapData() { 
                    ImageName="Altis.png", 
                    MapXMin =     0, 
                    MapXMax = 30769, 
                    MapYMin =   247, 
                    MapYMax = 30956,
                    Credits="Map created by 10T from Arma3 in game images. Released under the Arma Public License Share Alike (APL-SA). See http://forums.bistudio.com/showthread.php?178671-Tiled-maps-Google-maps-compatible-(WIP)",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Altis.png"
                }
            },
            { 
                "Altis Enhanced", 
                new MapData() { 
                    ImageName="AltisEnhanced.png", 
                    MapXMin =  2000, 
                    MapXMax = 30000, 
                    MapYMin =  5000, 
                    MapYMax = 33000,
                    Credits="The official Steam supporters edition map, only available to users who have purchased the Digital Deluxe versions of ArmA 3 on Steam. The map image for this will never be provided due to copyright reasons.",
                    Addons = new List<string>()
                }
            }, 
            { 
                "Bystrica", 
                new MapData() 
                { 
                    ImageName="Bystrica.png", 
                    MapXMin = 0, 
                    MapXMax = 7680, 
                    MapYMin = 0, 
                    MapYMax = 7680,
                    Credits="Official BI map exported from ArmA 3 as EMF by BI Forums user Leolawndart",
                    Addons = new List<string>()
                }
            },
            { 
                "Chernarus", 
                new MapData() 
                { 
                    ImageName="Chernarus.png", 
                    MapXMin =     0, 
                    MapXMax = 15254, 
                    MapYMin =     0, 
                    MapYMax = 15260,
                    Credits="Map created by 10T from Arma3 in game images. Released under the Arma Public License Share Alike (APL-SA). See http://forums.bistudio.com/showthread.php?178671-Tiled-maps-Google-maps-compatible-(WIP)",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Chernarus.png"
                }
            }, 
            { 
                "Stratis", 
                new MapData() 
                { 
                    ImageName="Stratis.png", 
                    MapXMin = 0, 
                    MapXMax = 8378, 
                    MapYMin = 0, 
                    MapYMax = 8388,
                    Credits="Map created by 10T from Arma3 in game images. Released under the Arma Public License Share Alike (APL-SA). See http://forums.bistudio.com/showthread.php?178671-Tiled-maps-Google-maps-compatible-(WIP)",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Stratis.png"
                }
            }, 
            { 
                "Takistan", 
                new MapData() 
                { 
                    ImageName="Takistan.png", 
                    MapXMin = 0, 
                    MapXMax = 12929, 
                    MapYMin = 27, 
                    MapYMax = 12929,
                    Credits="Map created by 10T from Arma3 in game images. Released under the Arma Public License Share Alike (APL-SA). See http://forums.bistudio.com/showthread.php?178671-Tiled-maps-Google-maps-compatible-(WIP)",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Takistan.png"
                }
            }, 
            { 
                "Utes", 
                new MapData() 
                { 
                    ImageName="Utes.png", 
                    MapXMin = 0, 
                    MapXMax = 5120, 
                    MapYMin = 0, 
                    MapYMax = 5120,
                    Credits="Official BI map exported from ArmA 3 as EMF by BI Forums user Leolawndart",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Utes.png"
                }
            },
            { 
                "Zargabad", 
                new MapData() 
                { 
                    ImageName="Zargabad.png", 
                    MapXMin =    0, 
                    MapXMax = 8150, 
                    MapYMin =  230, 
                    MapYMax = 8390,
                    Credits="Map created by 10T from Arma3 in game images. Released under the Arma Public License Share Alike (APL-SA). See http://forums.bistudio.com/showthread.php?178671-Tiled-maps-Google-maps-compatible-(WIP)",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Zargabad.png"
                }
            }
        };

        internal static List<string> MapNames
        {
            get
            {
                return Maps.Keys.ToList();
            }
        }
    }
}
