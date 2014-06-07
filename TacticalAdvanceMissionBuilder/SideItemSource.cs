using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnvilEditor
{
    public class FriendlySideItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection objectiveTypes = new ItemCollection();
            objectiveTypes.Add("WEST", "WEST");
            //objectiveTypes.Add("EAST", "EAST");
            //objectiveTypes.Add("INDEPENDENT", "INDEPENDENT");
            return objectiveTypes;
        }
    }

    public class EnemySideItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection objectiveTypes = new ItemCollection();
            //objectiveTypes.Add("WEST", "WEST");
            objectiveTypes.Add("EAST", "EAST");
            //objectiveTypes.Add("INDEPENDENT", "INDEPENDENT");
            return objectiveTypes;
        }
    }
}
