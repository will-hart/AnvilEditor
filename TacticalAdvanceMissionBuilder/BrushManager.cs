using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TacticalAdvanceMissionBuilder
{
    /// <summary>
    /// Maintains a brushes that can be used in the application
    /// </summary>
    public static class BrushManager
    {

        /// <summary>
        /// A brush for drawing in objective ellipses
        /// </summary>
        public static readonly SolidColorBrush ObjectiveBrush = new SolidColorBrush(Color.FromArgb(155, 0, 0, 255));

        /// <summary>
        /// A brush for highlighting the selected objective
        /// </summary>
        public static readonly SolidColorBrush SelectionBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

        /// <summary>
        /// A brush for unoccupied regions
        /// </summary>
        public static readonly SolidColorBrush UnoccupiedBrush = new SolidColorBrush(Color.FromArgb(155, 0, 255, 0));
    }
}
