namespace AnvilEditor.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class MapDefinitions
    {
        internal static readonly Dictionary<string, MapData> Maps = new Dictionary<string, MapData>()
        {
            
            { 
                "Aliabad", 
                new MapData() { 
                    ImageName="MCN_Aliabad.png", 
                    MapXMin =    0, 
                    MapXMax = 5120, 
                    MapYMin =    0, 
                    MapYMax = 5120,
                    Credits="Map by McNools. http://forums.bistudio.com/showthread.php?133996-Aliabad-Region-Version-1-1-Released!.  No images available due to lack of author permission, use Topography cheat to generate your own images (see help pages).",
                    Addons = new List<string>(),
                    MapAlias = "MCN_Aliabad"
                }
            },
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
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Altis.png",
                    MapAlias = "Altis"
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
                    Addons = new List<string>(),
                    MapAlias = "Altis"
                }
            }, 
            { 
                "Bukovina", 
                new MapData() 
                { 
                    ImageName="Bukovina.png", 
                    MapXMin = 0, 
                    MapXMax = 3840, 
                    MapYMin = 0, 
                    MapYMax = 3840,
                    Credits="Official BI map",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Bukovina.png",
                    MapAlias = "Bootcamp_ACR"
                }
            }, 
            { 
                "Bornholm", 
                new MapData() 
                { 
                    ImageName="Bornholm.png", 
                    MapXMin = 0, 
                    MapXMax = 22528, 
                    MapYMin = 0, 
                    MapYMax = 22528,
                    Credits="Map set in Denmark, created by Egil Sandfeld and exported by BI forums user Leolawndart. ",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Bornholm.png",
                    MapAlias = "Bornholm"
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
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Bystrica.png",
                    MapAlias = "Woodland_ACR"
                }
            },
            { 
                "Caribou Frontier", 
                new MapData() 
                { 
                    ImageName="Caribou.png", 
                    MapXMin =     0, 
                    MapXMax = 8192, 
                    MapYMin =     0, 
                    MapYMax = 8192,
                    Credits="Map created by Raunhofer and exported from ArmA 3 as EMF by BI Forums user Leolawndart. See http://forums.bistudio.com/showthread.php?161801-Caribou-Frontier",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Caribou.png",
                    MapAlias = "Caribou"
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
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Chernarus.png",
                    MapAlias = "Chernarus"
                }
            },
            { 
                "Desert", 
                new MapData() 
                { 
                    ImageName="Desert.png", 
                    MapXMin =     0, 
                    MapXMax = 2048, 
                    MapYMin =     0, 
                    MapYMax = 2048,
                    Credits="Official BI map exported from ArmA 3 as EMF by BI Forums user Leolawndart",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Desert.png",
                    MapAlias = "Desert_E"
                }
            },
            { 
                "Hazar Kot", 
                new MapData() { 
                    ImageName="MCN_HazarKot.png", 
                    MapXMin =    0, 
                    MapXMax = 5120, 
                    MapYMin =    0, 
                    MapYMax = 5120,
                    Credits="Map by McNools. http://forums.bistudio.com/showthread.php?129096-Hazar-Kot-Valley-Release-Thread.  No images available due to lack of author permission, use Topography cheat to generate your own images (see help pages).",
                    Addons = new List<string>(),
                    MapAlias = "MCN_HazarKot"
                }
            },
            { 
                "Porto", 
                new MapData() 
                { 
                    ImageName="Porto.png", 
                    MapXMin =     0, 
                    MapXMax = 5120, 
                    MapYMin =     0, 
                    MapYMax = 5120,
                    Credits="Official BI map exported from ArmA 3 as EMF by BI Forums user Leolawndart",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Porto.png",
                    MapAlias = "Porto"
                }
            },
            { 
                "Rahmadi", 
                new MapData() 
                { 
                    ImageName="Rahmadi.png", 
                    MapXMin =     0, 
                    MapXMax = 5120, 
                    MapYMin =     0, 
                    MapYMax = 5120,
                    Credits="Official BI map exported from ArmA 3 as EMF by BI Forums user Leolawndart",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Rahmadi.png",
                    MapAlias = "Intro"
                }
            },
            { 
                "Sahrani", 
                new MapData() 
                { 
                    ImageName="Sahrani.png", 
                    MapXMin = 0, 
                    MapXMax = 20480, 
                    MapYMin = 0, 
                    MapYMax = 20480,
                    Credits="ArmA 3 port of original BI ArmA 1 map Sahrani by NonovUrbizniz, M1lkm8n and Pliskin. Exported as EMB by BI forums user Leolawndart. Released under the Arma Public License Share Alike (APL-SA). See http://forums.bistudio.com/showthread.php?185924-SMD-Sahrani-A3-Bohemia-s-Sahrani-Terrain-as-ported-by-SMD",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Sahrani.png",
                    MapAlias = "Sahrani"
                }
            }, 
            { 
                "Shapur", 
                new MapData() 
                { 
                    ImageName="Shapur.png", 
                    MapXMin = 0, 
                    MapXMax = 2048, 
                    MapYMin = 0, 
                    MapYMax = 2048,
                    Credits="Official BI map",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Shapur.png",
                    MapAlias = "Shapur"
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
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Stratis.png",
                    MapAlias = "Stratis"
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
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Takistan.png",
                    MapAlias = "Takistan"
                }
            },  
            { 
                "Takistan Mountains", 
                new MapData() 
                { 
                    ImageName="TakistanMountains.png", 
                    MapXMin = 0, 
                    MapXMax = 6400, 
                    MapYMin = 27, 
                    MapYMax = 6400,
                    Credits="Official BI map exported from ArmA 3 as EMF by BI Forums user Leolawndart",
                    Addons = new List<string>(),
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/TakistanMountains.png",
                    MapAlias = "Mountains_ACR"
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
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Utes.png",
                    MapAlias = "Utes"
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
                    DownloadUrl = "http://www.anvilproject.com/downloads/files/Zargabad.png",
                    MapAlias = "Zargabad"
                }
            }
        };

        /// <summary>
        /// Gets a list of map aliases that are valid for folder names
        /// </summary>
        internal static List<string> MapAliases
        {
            get
            {
                return Maps.Values.Select(o => o.MapAlias).ToList();
            }
        }

        /// <summary>
        /// Gets a list of map names that can be displayed in the UI
        /// </summary>
        internal static List<string> MapNames
        {
            get
            {
                return Maps.Keys.ToList();
            }
        }
    }
}
