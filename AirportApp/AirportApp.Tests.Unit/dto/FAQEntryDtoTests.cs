using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Tests.Unit.Dto;

[TestClass]
public class FAQEntryDtoTests
{
    [TestMethod]
    public void IsExpanded_Set_RaisesPropertyChanged()
    {
        var dataTransferObject = new FAQEntryDTO(1, "What type of cars can I park here", "Only audis", FAQCategoryEnum.All, 34, 23, 3);
        string? changedProperty = null;
        dataTransferObject.PropertyChanged += (sender, arguments) => changedProperty = arguments.PropertyName;

        dataTransferObject.IsExpanded = true;

        Assert.AreEqual("IsExpanded", changedProperty);
    }
}




