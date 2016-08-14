namespace AnvilEditor.Models.Sources
{
    using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

    using Helpers;

    public class SpawnConfigurationItemSource : IItemsSource
    {
        public ItemCollection GetValues()
        {
            ItemCollection configurationSource = new ItemCollection();
            configurationSource.Add("Default for Side");
            foreach (var conf in DataHelper.Instance.EosSpawnConfigurations.Keys)
            {
                configurationSource.Add(conf);
            }

            return configurationSource;
        }
    }
}
