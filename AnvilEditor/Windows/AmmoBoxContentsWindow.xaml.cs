namespace AnvilEditor.Windows
{
    using MahApps.Metro.Controls;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;

    using Models;

    /// <summary>
    /// Interaction logic for AmmoBoxContentsWindow.xaml
    /// </summary>
    public partial class AmmoBoxContentsWindow : MetroWindow
    {
        private ObservableCollection<AmmoboxItem> contents;

        public AmmoBoxContentsWindow(IEnumerable<AmmoboxItem> items)
        {
            this.contents = new ObservableCollection<AmmoboxItem>(items);
            InitializeComponent();
        }

        /// <summary>
        /// Binds the list box display to the ammobox contents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            this.AmmoboxItemListBox.ItemsSource = this.contents;
        }

        /// <summary>
        /// Gets the resulting ammobox contents
        /// </summary>
        public IEnumerable<AmmoboxItem> Items
        {
            get
            {
                return this.contents;
            }
        }

        /// <summary>
        /// Handles a user adding a new item to the ammobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButtonClicked(object sender, RoutedEventArgs e)
        {
            this.contents.Add(new AmmoboxItem());
        }

        /// <summary>
        /// Removes an item from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveItemFromList(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            
            var ctx = btn.DataContext as AmmoboxItem;
            if (ctx == null) return;

            this.contents.Remove(ctx);
        }

        /// <summary>
        /// Handles the user clicking the save button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Handles the user clicking the close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
