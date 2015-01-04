namespace AnvilEditor.Helpers
{
    using NLog;
    using System.Collections.Generic;

    using AnvilEditor.Models;
    
    public class DataHelper
    {
        /// <summary>
        /// Create a logger
        /// </summary>
        private static Logger Log = LogManager.GetLogger("DataHelper");

        /// <summary>
        /// A private DataHelper instance "singleton"
        /// </summary>
        private static DataHelper instance;

        /// <summary>
        /// The default contents for ammoboxes for new missions, initially loaded from JSON files
        /// </summary>
        private List<AmmoboxItem> defaultAmmoboxContents;

        /// <summary>
        /// A list of spawn configurations used to configure EOS
        /// </summary>
        private Dictionary<string, EosSpawnConfiguration> eosSpawnConfigurations;

        private DataHelper()
        {
            Log.Debug("Loading default data");
            this.defaultAmmoboxContents = FileHelper.GetDataFile<List<AmmoboxItem>>("default_ammobox.json");
            this.eosSpawnConfigurations = FileHelper.GetDataFile<Dictionary<string, EosSpawnConfiguration>>("eos_spawn_configurations.json");
            Log.Debug("Done loading default data");
        }
        
        /// <summary>
        /// Gets a reference to the static data helper instance
        /// </summary>
        public static DataHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataHelper();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets or sets a list of Ammobox Items which represent the default mission ammo box contents
        /// </summary>
        public List<AmmoboxItem> DefaultAmmoboxContents
        {
            get
            {
                return this.defaultAmmoboxContents;
            }
            set
            {
                this.defaultAmmoboxContents = value;
                FileHelper.WriteDataFile("default_ammobox.json", this.defaultAmmoboxContents);
            }
        }

        /// <summary>
        /// Gets or sets a dictionary of EOS spawn configurations indexed by confiugration name
        /// </summary>
        public Dictionary<string, EosSpawnConfiguration> EosSpawnConfigurations
        {
            get
            {
                return this.eosSpawnConfigurations;
            }
            set
            {
                this.eosSpawnConfigurations = value;
                FileHelper.WriteDataFile("eos_spawn_configurations.json", this.eosSpawnConfigurations);
            }
        }
    }
}
