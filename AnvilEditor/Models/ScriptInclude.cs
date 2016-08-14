namespace AnvilEditor.Models
{
    /// <summary>
    /// Contains information about a script that should be included in the mission
    /// </summary>
    public class ScriptInclude
    {
        public ScriptInclude()
        {
            this.FriendlyName = "ERROR Script Missing Friendly Name";
            this.FolderName = "";
            this.DescriptionExtFunctions = "";
            this.DescriptionExtInit = "";
            this.Init = "";
            this.InitPlayerLocal = "";
            this.Url = "http://www.anvilproject.com";
        }

        /// <summary>
        /// Overrides base.ToString() and returns the friendly name when displaying this as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FriendlyName;
        }

        /// <summary>
        /// Gets or sets a value containing the human-friendly name for this script
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the name of the folder where this script is stored
        /// </summary>
        public string FolderName { get; set; }

        /// <summary>
        /// Gets or sets a value which is the lines included in the description.ext file in the init section
        /// </summary>
        public string DescriptionExtInit { get; set; }

        /// <summary>
        /// Gets or sets a value which is the lines included in the description.ext file in the cfgFunctions section
        /// </summary>
        public string DescriptionExtFunctions { get; set; }

        /// <summary>
        /// Gets or sets a string value containing the init code to be included in the mission's init.sqf
        /// </summary>
        public string Init { get; set; }

        /// <summary>
        /// Gets or sets a string value containing code that should be included in an initPlayerLocal.sqf file
        /// </summary>
        public string InitPlayerLocal { get; set; }

        /// <summary>
        /// Gets or sets a string representing the URL of this script, and where it can be found if
        /// the script is not found locally
        /// </summary>
        public string Url { get; set; }
    }
}
