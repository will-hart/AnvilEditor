namespace AnvilEditor.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class MapDefinitions
    {
        /// <summary>
        /// Gets a map name index dictionary of map data
        /// </summary>
        internal static Dictionary<string, MapData> Maps
        {
            get
            {
                // load the maps from JSON file if they don't already exist
                if (maps == null)
                {
                    var json = string.Empty;
                    var path = Path.Combine(FileUtilities.GetDataFolder, "map_definitions.json");
                    using (var sr = new StreamReader(path))
                    {
                        json = sr.ReadToEnd();
                    }
                    maps = JsonConvert.DeserializeObject<Dictionary<string, MapData>>(json);
                }

                return maps;
            }
        }

        /// <summary>
        /// The static dictionary of maps that are accessed by the editor
        /// </summary>
        private static Dictionary<string, MapData> maps; 

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
