namespace AnvilEditor.Models
{
    /// <summary>
    /// Contains information about items in an ammobox
    /// </summary>
    public class AmmoboxItem
    {
        public string Category { get; set; }
        public string ClassName { get; set; }
        public int Quantity { get; set; }

        public AmmoboxItem()
        {
            this.Category = "Weapon";
            this.ClassName = "ClassName";
            this.Quantity = 10;
        }
    }
}
