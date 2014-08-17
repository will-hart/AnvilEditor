using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnvilEditor.Models
{
    public class MissionTypeItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection objectiveTypes = new ItemCollection();
            objectiveTypes.Add(0, "CAPTURE");
            objectiveTypes.Add(5, "CLEAR");
            objectiveTypes.Add(2, "ASSASSINATE");
            objectiveTypes.Add(3, "DESTROY");
            objectiveTypes.Add(4, "DESTROY AA");
            objectiveTypes.Add(6, "DESTROY AMMO");
            objectiveTypes.Add(9, "DESTROY HELO");
            objectiveTypes.Add(7, "DESTROY UAV");
            objectiveTypes.Add(1, "INTEL");
            objectiveTypes.Add(8, "INTEL FROM HELI CRASH");
            objectiveTypes.Add(11, "MOVE TO");
            objectiveTypes.Add(10, "MOVE THROUGH");

            return objectiveTypes;
        }
    }
}
