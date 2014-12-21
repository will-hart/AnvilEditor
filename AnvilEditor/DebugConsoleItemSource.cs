namespace AnvilEditor
{
    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

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
