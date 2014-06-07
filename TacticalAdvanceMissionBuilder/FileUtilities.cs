using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                        newLines.Add(line);
                        found = true;
                    }
                }
                else
                {
                    newLines.Add(line);

                    if (found == null && line.Contains(markerStart))
                    {
                        found = false;
                        newLines.Add(replaceWith);
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

            foreach (var line in lines)
            {
                if (line.StartsWith(lineStart))
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
                        // squash if it is an "already exists" exception
                        if (!e.Message.Contains("already exists")) throw;

                        // otherwise check if we want to overwrite the mission.sqm
                        var res = MessageBox.Show(
                            "Do you want to overwrite the mission.sqm file (Click YES) or manually paste in new markers (Click NO)? IF you select yes you will lose any changes you made in the editor. You can use the 'preview' functionality to see the marker text",
                            "Overwrite mission file?",
                            MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes) tempfile.CopyTo(path, true);
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
    }
}
