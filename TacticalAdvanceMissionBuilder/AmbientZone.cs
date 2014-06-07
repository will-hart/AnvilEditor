using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace TacticalAdvanceMissionBuilder
{
    /// <summary>
    /// Holds objective data that can be written to file
    /// </summary>
    public class AmbientZone : ObjectiveBase {
        public AmbientZone(Point location) : base(location)
        {

        }
    }
}
