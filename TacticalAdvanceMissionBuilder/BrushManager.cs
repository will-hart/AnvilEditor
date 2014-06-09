using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AnvilEditor
{
    /// <summary>
    /// Maintains a brushes that can be used in the application
    /// </summary>
    public static class BrushManager
    {

        /// <summary>
        /// A brush for drawing in objective ellipses
        /// </summary>
        public static readonly SolidColorBrush Objective = new SolidColorBrush(Color.FromArgb(155, 0, 0, 255));

        /// <summary>
        /// A brush for highlighting the selected objective
        /// </summary>
        public static readonly SolidColorBrush Selection = new SolidColorBrush(Color.FromArgb(155, 255, 0, 0));

        /// <summary>
        /// A brush for unoccupied regions
        /// </summary>
        public static readonly SolidColorBrush Unoccupied = new SolidColorBrush(Color.FromArgb(155, 0, 255, 0));

        /// <summary>
        /// A brush for drawing the respawn point
        /// </summary>
        public static readonly SolidColorBrush Respawn = new SolidColorBrush(Color.FromArgb(155, 170, 30, 240));

        /// <summary>
        /// A brush for drawing the respawn point
        /// </summary>
        public static readonly SolidColorBrush Ambient = new SolidColorBrush(Color.FromArgb(190, 70, 70, 70));

        /// <summary>
        /// A brush for drawing the respawn point
        /// </summary>
        public static readonly SolidColorBrush UnoccupiedAmbient = new SolidColorBrush(Color.FromArgb(130, 70, 70, 70));
		
		/// <summary>
		/// A brush for the stroke of objectives with a spawn point reward
		/// </summary>
		public static readonly SolidColorBrush NewSpawn = new SolidColorBrush(Color.FromArgb(155, 250, 240, 90));
    }
}
