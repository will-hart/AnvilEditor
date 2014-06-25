using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnvilEditor.Models
{
    public class GroupSizeItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection objectiveTypes = new ItemCollection();
            objectiveTypes.Add(0, "1");
            objectiveTypes.Add(1, "2-4");
            objectiveTypes.Add(2, "4-8");
            objectiveTypes.Add(3, "8-12");
            objectiveTypes.Add(4, "12-16");
            objectiveTypes.Add(5, "16-20");
            return objectiveTypes;
        }
    }
}
