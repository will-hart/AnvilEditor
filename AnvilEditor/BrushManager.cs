namespace AnvilEditor
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Maintains a brushes that can be used in the application
    /// </summary>
    public static class BrushManager
    {

        /// <summary>
        /// A brush for drawing in objective ellipses
        /// </summary>
        public static readonly ImageBrush Objective = new ImageBrush(new BitmapImage(new Uri(@"data\icons\occupied_objective.png", UriKind.Relative)));

        /// <summary>
        /// A brush for highlighting the selected objective
        /// </summary>
        public static readonly ImageBrush Selection = new ImageBrush(new BitmapImage(new Uri(@"data\icons\selected.png", UriKind.Relative)));

        /// <summary>
        /// A brush for unoccupied regions
        /// </summary>
        public static readonly ImageBrush UnoccupiedObjective = new ImageBrush(new BitmapImage(new Uri(@"data\icons\unoccupied_objective.png", UriKind.Relative)));

        /// <summary>
        /// A brush for drawing the respawn point
        /// </summary>
        public static readonly ImageBrush Respawn = new ImageBrush(new BitmapImage(new Uri(@"data\icons\respawn_point.png", UriKind.Relative)));

        /// <summary>
        /// A brush for drawing the respawn point
        /// </summary>
        public static readonly ImageBrush Ambient = new ImageBrush(new BitmapImage(new Uri(@"data\icons\occupied_ambient.png", UriKind.Relative)));

        /// <summary>
        /// A brush for drawing the respawn point
        /// </summary>
        public static readonly ImageBrush UnoccupiedAmbient = new ImageBrush(new BitmapImage(new Uri(@"data\icons\unoccupied_ambient.png", UriKind.Relative)));
		
		/// <summary>
		/// A brush for an objective with a spawn point reward
		/// </summary>
        public static readonly ImageBrush NewSpawn = new ImageBrush(new BitmapImage(new Uri(@"data\icons\spawn.png", UriKind.Relative)));

        /// <summary>
        /// A brush for an objective which has an ammobox reward
        /// </summary>
        public static readonly ImageBrush NewAmmo = new ImageBrush(new BitmapImage(new Uri(@"data\icons\ammo.png", UriKind.Relative)));

        /// <summary>
        /// A brush for an objective which has a special ammobox reward
        /// </summary>
        public static readonly ImageBrush NewSpecial = new ImageBrush(new BitmapImage(new Uri(@"data\icons\special_weapons.png", UriKind.Relative)));
    }
}
