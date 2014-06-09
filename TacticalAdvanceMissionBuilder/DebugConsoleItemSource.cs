using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnvilEditor
{
    public class DebugConsoleItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection objectiveTypes = new ItemCollection();
            objectiveTypes.Add(0, "Disabled");
            objectiveTypes.Add(1, "Admins");
            return objectiveTypes;
        }
    }
}
