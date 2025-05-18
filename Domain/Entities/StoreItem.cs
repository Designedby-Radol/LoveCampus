using System.Globalization;

namespace CampusLove.Domain.Entities
{
    public class StoreItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CapcoinPrice { get; set; }
        public string Type { get; set; } // "token" or "like"
        public int Quantity { get; set; }

        public string FormattedPrice => CapcoinPrice.ToString("N0", CultureInfo.CurrentCulture);

        public override string ToString()
        {
            return $"{Name} - {FormattedPrice} Capcoins - {Description}";
        }
    }
}