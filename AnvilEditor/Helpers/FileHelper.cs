namespace AnvilEditor.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using AnvilEditor.Templates;

    using AnvilParser;
    using AnvilParser.Grammar;

    using Sprache;
    using System.Reflection;
    using AnvilEditor.Models;
    using Newtonsoft.Json;
    using NLog;

    public static class FileHelper
    {
        /// <summary>
        /// Holds the script folder name globally
        /// </summary>
        internal static readonly string ScriptFolderName = "anvil";

        /// <summary>
        /// Create a logger
        /// </summary>
        private static Logger Log = LogManager.GetLogger("FileHelper");

        /// <summary>
        /// Opens and edits the given file and replaces the MARKER with the text of REPLACEWITH
        /// </summary>
        /// <param name="path">The path of the file to edit</param>
        /// <param name="markerStart">The marker to replace from</param>
        /// <param name="markerEnd">The marker to replace until</param>
        /// <param name="replaceWith">The text to replace the marker with</param>
        public static void ReplaceSection(string path, string markerStart, string markerEnd, string replaceWith)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var newLines = new List<string>();
            bool? found = null;

            // a regex.replace would probably be better but then ... regex

            foreach (var line in lines)
            {
                // if we are within the markers, don't append the line
                if (found == false)
                {
                    if (line.Contains(markerEnd))
                    {
                        found = true;
                    }
                }
                else
                {
                    if (found == null && line.Contains(markerStart))
                    {
                        found = false;
                        newLines.Add(replaceWith);
                    }
                    else
                    {
                        newLines.Add(line);
                    }
                }
            }

            System.IO.File.WriteAllLines(path, newLines);
        }

        /// <summary>
        /// Replaces all lines in the file starting with `lineStart` with the value in `replaceWith`
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <param name="lineStart">The start of the line to match</param>
        /// <param name="replaceWith">The value to replace with</param>
        public static void ReplaceLines(string path, string lineStart, string replaceWith)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var newLines = new List<string>();
            lineStart = lineStart.ToLower();

            foreach (var line in lines)
            {
                if (line.ToLower().StartsWith(lineStart))
                {
                    newLines.Add(replaceWith);
                }
                else
                {
                    newLines.Add(line);
                }
            }

            System.IO.File.WriteAllLines(path, newLines);
        }

        /// <summary>
        /// Copy the raw mission files to the given directory and edit the
        /// framework_init and mission SQM files to add in the generated content
        /// 
        /// Borrowed some code from http://stackoverflow.com/a/12283793/233608
        /// 
        /// This is called "safe" as it does not overwrite the mission.sqm file,
        /// only updates the contents between the markers
        /// </summary>
        /// <param name="destDir">The destination root directory</param>
        public static void SafeDirectoryCopy(string srcDir, string destDir)
        {
            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
            if (Path.GetFileName(destDir) == "fw_scripts") return;

            var srcDirInfo = new DirectoryInfo(srcDir);
            var srcFiles = srcDirInfo.GetFiles();

            var ignoredFiles = new List<string> { "mission.sqm", "briefing.sqf" };

            foreach (var destFile in srcFiles)
            {
                var destPath = System.IO.Path.Combine(destDir, destFile.Name);
                if (ignoredFiles.Contains(destFile.Name))
                {
                    // only create ignored files if they don't exist
                    if (!File.Exists(destPath))
                    {
                        destFile.CopyTo(destPath);
                    }
                }
                else
                {
                    destFile.CopyTo(destPath, true);
                }
            }

            var dirs = srcDirInfo.GetDirectories();
            foreach (var tempdir in dirs)
            {
                SafeDirectoryCopy(
                    System.IO.Path.Combine(srcDir, tempdir.Name), System.IO.Path.Combine(destDir, tempdir.Name));
            }
        }

        /// <summary>
        /// Reads in the contents of the file at the given path and parses them using the AnvilParser
        /// </summary>
        /// <param name="path">The file path of the mission.sqm file</param>
        /// <returns>A MissionBase object populated from the mission.sqm</returns>
        public static ParserClass BuildSqmTreeFromFile(string path)
        {
            if (!File.Exists(path)) return new MissionBase("root");

            using (var f = new StreamReader(path))
            {
                var sqm = f.ReadToEnd();
                var grammar = SQMGrammar.SQMParser.Parse(sqm);
                return grammar;
            }
        }

        /// <summary>
        /// Empties out the loaded path directory, with a flag to optionally preserve the mission_data.json file
        /// </summary>
        /// <param name="saveMissionDataJson"></param>
        public static bool EmptyMissionDirectory(string folder, bool saveMissionDataJson = true)
        {
            var dir = new DirectoryInfo(folder);
            var allDeleted = true;

            foreach (var fi in dir.GetFiles())
            {
                if (!(saveMissionDataJson && fi.FullName.EndsWith("mission_data.json")))
                {
                    fi.Delete();
                }
                else
                {
                    allDeleted = false;
                }
            }

            foreach (var di in dir.GetDirectories())
            {
                var allGone = EmptyMissionDirectory(di.FullName, saveMissionDataJson);
                if (allGone)
                {
                    di.Delete();
                }
                else
                {
                    allDeleted = false;
                }
            }

            return allDeleted;
        }

        /// <summary>
        /// Gets a list of ScriptIncludes that do not have the folder downloaded to the anvil included scripts repository. 
        /// These will need to be downloaded before the mission can be correctly built.
        /// </summary>
        /// <param name="scripts"></param>
        /// <param name="availableScripts"></param>
        /// <returns></returns>
        public static List<ScriptInclude> GetMissingIncludedScriptFolders(List<string> scripts, List<ScriptInclude> availableScripts)
        {
            var missing = new List<ScriptInclude>();

            foreach (var script in scripts)
            {
                ScriptInclude scriptObject;
                try
                {
                    scriptObject = availableScripts.Where(o => o.FriendlyName == script).First();
                }
                catch (ArgumentNullException)
                {
                    // ignore the script
                    continue;
                }

                if (scriptObject.FolderName != "") {
                    var path = System.IO.Path.Combine(FileHelper.GetFrameworkSourceFolder, "fw_scripts", scriptObject.FolderName);
                    if (!Directory.Exists(path))
                    {
                        missing.Add(scriptObject);
                    }
                }
            }

            return missing;
        }

        /// <summary>
        /// Finds a parent directory - either the last loaded directory or the parent of the currently loaded map
        /// </summary>
        /// <returns>A string path pointing to a suitable directory to start browsing files from</returns>
        public static string GetUsefulParentDirectory(string loadedPath)
        {
            string topPath;
            if (loadedPath.Length == 0)
            {
                if (AnvilEditor.Properties.Settings.Default.RecentItems.Count > 0)
                {
                    topPath = AnvilEditor.Properties.Settings.Default.RecentItems[0];
                }
                else
                {
                    topPath = "";
                }
            }
            else
            {
                topPath = loadedPath;
            }
            return topPath;
        }

        /// <summary>
        /// Loads a JSON data file into an object, returning a new default object if no json data file is found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T GetDataFile<T>(string fileName) where T : new()
        {
            var filePath = System.IO.Path.Combine(GetDataFolder, fileName);
            T returnItem;

            try
            {
                using (var sw = new StreamReader(filePath))
                {
                    returnItem = JsonConvert.DeserializeObject<T>(sw.ReadToEnd());
                }
            }
            catch (FileNotFoundException) 
            {
                Log.Warn(string.Format("Unable to find data file '{0}', returning an empty object (GetDataFile<T>)", fileName));
                return new T();
            }

            // handle the case of an empty JSON file before returning
            return returnItem == null ? new T() : returnItem;
        }

        /// <summary>
        /// Writes the given object as JSON to a data file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        public static void WriteDataFile(string fileName, object data)
        {
            var filePath = Path.Combine(GetDataFolder, fileName);
            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (var sw = new StreamWriter(filePath))
            {
                using (var writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, data);
                }
            }
        }

        /// <summary>
        /// Gets a value containing the path to the source folder for the Anvil Framework
        /// </summary>
        public static string GetFrameworkSourceFolder
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mission_raw");
            }
        }

        /// <summary>
        /// Gets a link to the data folder for map images 
        /// </summary>
        public static string GetDataFolder
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "data");
            }
        }
    }
}
