namespace AnvilEditor
{
    using System.Windows;

    using AnvilEditor.Models;
    using MahApps.Metro.Controls.Dialogs;

    /// <summary>
    /// Interaction logic for BriefingWindow.xaml
    /// </summary>
    public partial class BriefingWindow
    {
        /// <summary>
        /// The mission whose briefing is being edited
        /// </summary>
        private Mission mission;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="mission"></param>
        public BriefingWindow(Mission mission)
        {
            this.mission = mission;
            this.DataContext = this.mission;
            this.SelectedSection = "";
            InitializeComponent();
            this.UpdateSections();
            this.UpdateDetailView(this.SelectedSection);
        }

        /// <summary>
        /// Shows a prompt for a new section name and adds it to the end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddBriefingItemButtonClick(object sender, RoutedEventArgs e)
        {
            var title = await this.ShowInputAsync("Specify section name", "Enter a briefing section name");
            if (title.Length == 0) return;

            if (!this.mission.MissionBriefing.BriefingSections.Contains(title))
            {
                this.mission.MissionBriefing.BriefingSections.Add(title);
            }

            this.UpdateSections();
        }

        /// <summary>
        /// Deletes a briefing item from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteBriefingItemButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.SelectedSection == "") return;

            this.mission.MissionBriefing.Delete(this.SelectedSection);

            this.SelectedSection = "";
            this.UpdateSections();
            this.UpdateDetailView("");
        }

        /// <summary>
        /// Updates the sections in the list box
        /// </summary>
        private void UpdateSections()
        {
            this.SectionListBox.Items.Clear();
            foreach (var s in this.mission.MissionBriefing.BriefingSections)
            {
                this.SectionListBox.Items.Add(s);
            }

            if (this.SelectedSection != "")
            {
                this.SectionListBox.SelectedItem = this.SelectedSection;
                this.UpdateDetailView(this.SelectedSection);
            }
        }

        /// <summary>
        /// Handle selecting a new item in the list box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SectionListBoxSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.SectionListBox.SelectedIndex == -1)
            {
                this.SelectedSection = "";
            }
            else
            {
                this.SelectedSection = (string)this.SectionListBox.SelectedItem;
            }

            this.UpdateDetailView(this.SelectedSection);
        }

        /// <summary>
        /// Sets the detail view to match the selected section
        /// </summary>
        /// <param name="p"></param>
        private void UpdateDetailView(string p)
        {
            this.SectionTextBox.Text = this.mission.MissionBriefing.Get(p);
        }

        /// <summary>
        /// Poor man's data binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SectionTextBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.SelectedSection != "")
            {
                this.mission.MissionBriefing.Set(this.SelectedSection, this.SectionTextBox.Text);
            }
        }
      
        /// <summary>
        /// The string section heading currently selected in the LH box
        /// </summary>
        private string SelectedSection { get; set; }
    }
}
