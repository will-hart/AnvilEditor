using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnvilEditor.Models
{
    internal class MapData
    {
        internal string ImageName;
        internal int MapXMin;
        internal int MapXMax;
        internal int MapYMin;
        internal int MapYMax;
        internal string Credits;
        internal List<string> Addons;

        internal string ToString()
        {
            return this.Credits + Environment.NewLine + Environment.NewLine +
                "Map X Minimum: " + this.MapXMin + Environment.NewLine +
                "Map X Maximum: " + this.MapXMax + Environment.NewLine +
                "Map Y Minimum: " + this.MapYMin + Environment.NewLine +
                "Map Y Maximum: " + this.MapYMax + Environment.NewLine +
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
                    MapXMin=2000, 
                    MapXMax=30000, 
                    MapYMin=5000, 
                    MapYMax=26000,
                    Credits="Map created by 10T from Arma3 in game images. Released under the Arma Public License Share Alike (APL-SA). See http://forums.bistudio.com/showthread.php?178671-Tiled-maps-Google-maps-compatible-(WIP)",
                    Addons = new List<string>()
                }
            }, 
            { 
                "Stratis", 
                new MapData() 
                { 
                    ImageName="Stratis.png", 
                    MapXMin=1300, 
                    MapXMax=7700, 
                    MapYMin=200, 
                    MapYMax=8000,
                    Credits="Map created by 10T from Arma3 in game images. Released under the Arma Public License Share Alike (APL-SA). See http://forums.bistudio.com/showthread.php?178671-Tiled-maps-Google-maps-compatible-(WIP)",
                    Addons = new List<string>()
                }
            }, 
            { 
                "Takistan", 
                new MapData() 
                { 
                    ImageName="Takistan.png", 
                    MapXMin=1300, 
                    MapXMax=7700, 
                    MapYMin=200, 
                    MapYMax=8000,
                    Credits="Map created by 10T from Arma3 in game images. Released under the Arma Public License Share Alike (APL-SA). See http://forums.bistudio.com/showthread.php?178671-Tiled-maps-Google-maps-compatible-(WIP)",
                    Addons = new List<string>()
                }
            }, 
            { 
                "Zargabad", 
                new MapData() 
                { 
                    ImageName="Zargabad.png", 
                    MapXMin=1300, 
                    MapXMax=7700, 
                    MapYMin=200, 
                    MapYMax=8000,
                    Credits="Map created by 10T from Arma3 in game images. Released under the Arma Public License Share Alike (APL-SA). See http://forums.bistudio.com/showthread.php?178671-Tiled-maps-Google-maps-compatible-(WIP)",
                    Addons = new List<string>()
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
