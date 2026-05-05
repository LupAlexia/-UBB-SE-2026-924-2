using AirportApp.Src.ViewModel;
using AirportApp.ClassLibrary.Entity.Domain;

namespace Tests.Unit.Fixtures;

public static class AddOnFixture
{
    public static AddOn CreateValidAddOn(int id = 1, string name = "Bagaj de cala", float price = 25.0f)
    {
        return new AddOn
        {
            Name = name,
            BasePrice = price
        };
    }

    public static List<AddOn> CreateAddOnList()
    {
        return new List<AddOn>
        {
            CreateValidAddOn(1, "Bagaj de mana extra", 15.0f),
            CreateValidAddOn(2, "Bagaj de cala (20kg)", 35.0f),
            CreateValidAddOn(3, "Prioritate", 10.0f)
        };
    }
}




