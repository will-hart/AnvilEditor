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
        public AmbientZone(int id, Point location)
            : base(id, location)
        {
            this.Id = id;
            this.screenX = location.X;
            this.screenY = location.Y;

            // set some defaults
            this.Radius = 50;
            this.Infantry = 0;
            this.Motorised = 0;
            this.Armour = 0;
            this.Air = 0;
            this.TroopStrength = 0;
        }
    }
}
