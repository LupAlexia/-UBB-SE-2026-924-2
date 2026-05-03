namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class AddOn
    {
        public int AddOnId { get; set; }
        public string Name { get; set; } = string.Empty;
        public float BasePrice { get; set; }

        public AddOn()
        {
        }

        public AddOn(string name, float basePrice)
        {
            Name = name;
            BasePrice = basePrice;
        }

        public AddOn(int addOnId, string name, float basePrice)
        {
            AddOnId = addOnId;
            Name = name;
            BasePrice = basePrice;
        }
    }
}
