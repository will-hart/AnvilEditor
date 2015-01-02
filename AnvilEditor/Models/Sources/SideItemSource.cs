namespace AnvilEditor.Models.Sources
{
    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    public class SideItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection objectiveTypes = new ItemCollection();
            objectiveTypes.Add("WEST", "WEST");
            objectiveTypes.Add("EAST", "EAST");
            objectiveTypes.Add("INDEPENDENT", "INDEPENDENT");
            return objectiveTypes;
        }
    }
}
