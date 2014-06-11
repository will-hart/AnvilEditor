using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnvilParser.Tokens;

namespace AnvilParser
{
    public class IntelBase : ParserClass
    {
        /// <summary>
        /// Handles random seeds
        /// </summary>
        private static readonly Random Rand = new Random();

        /// <summary>
        /// Creates a new mission base, completely unpopulated
        /// </summary>
        /// <param name="name"></param>
        public IntelBase()
            : base("Intel")
        {
            // weather
            this.Add("timeOfChanges", 1800.0d);
            this.Add("startWeather", 0.3d);
            this.Add("startWind", 0.1d);
            this.Add("startWaves", 0.1d);
            this.Add("forecastWeather", 0.3d);
            this.Add("forecastWind", 0.1d);
            this.Add("forecastWaves", 0.1d);
            this.Add("forecastLightnings", 0.1d);
            this.Add("startFogDecay", 0.013d);
            this.Add("forecastFogDecay", 0.013d);

            // date
            this.Add("year", 2035);
            this.Add("month", 6);
            this.Add("day", 24);
            this.Add("hour", 12);
            this.Add("minute", 0);
        }

    }
}
