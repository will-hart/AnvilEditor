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
            objectiveTypes.Add(1, "INTEL");
            objectiveTypes.Add(2, "ASSASSINATE");
            objectiveTypes.Add(3, "DESTROY");
            objectiveTypes.Add(4, "DESTROY AA");
            return objectiveTypes;
        }
    }
}
