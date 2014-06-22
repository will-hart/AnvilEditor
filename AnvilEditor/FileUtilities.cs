using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using AnvilEditor.Templates;

using AnvilParser;
using AnvilParser.Grammar;
using AnvilParser.Tokens;

using Sprache;

namespace AnvilEditor
{
    internal class FileUtilities
    {

        /// <summary>
        /// Opens and edits the given file and replaces the MARKER with the text of REPLACEWITH
        /// </summary>
        /// <param name="path">The path of the file to edit</param>
        /// <param name="markerStart">The marker to replace from</param>
        /// <param name="markerEnd">The marker to replace until</param>
        /// <param name="replaceWith">The text to replace the marker with</param>
        internal static void ReplaceSection(string path, string markerStart, string markerEnd, string replaceWith)
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
        internal static void ReplaceLines(string path, string lineStart, string replaceWith)
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
        /// <param name="dest">The destination root directory</param>
        internal static void SafeDirectoryCopy(string src, string dest)
        {
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            if (Path.GetFileName(dest) == "fw_scripts") return;

            var dirInfo = new DirectoryInfo(src);
            var files = dirInfo.GetFiles();

            foreach (var tempfile in files)
            {
                var path = System.IO.Path.Combine(dest, tempfile.Name);
                if (tempfile.Name == "mission.sqm")
                {
                    try
                    {
                        tempfile.CopyTo(path);
                    }
                    catch (IOException e)
                    {
                        // squash if it is an "already exists" exception, otherwise don't overwrite the existing file
                        if (!e.Message.Contains("already exists")) throw;
                    }
                }
                else
                {
                    tempfile.CopyTo(path, true);
                }
            }

            var dirs = dirInfo.GetDirectories();
            foreach (var tempdir in dirs)
            {
                SafeDirectoryCopy(
                    System.IO.Path.Combine(src, tempdir.Name), System.IO.Path.Combine(dest, tempdir.Name));
            }
        }

        /// <summary>
        /// Reads in the contents of the file at the given path and parses them using the AnvilParser
        /// </summary>
        /// <param name="path">The file path of the mission.sqm file</param>
        /// <returns>A MissionBase object populated from the mission.sqm</returns>
        internal static ParserClass BuildSqmTreeFromFile(string path)
        {
            if (!File.Exists(path)) return new MissionBase("root");

            using (var f = new StreamReader(path))
            {
                var sqm = f.ReadToEnd();
                return SQMGrammar.SQMParser.Parse(sqm);
            }
        }
    }
}
