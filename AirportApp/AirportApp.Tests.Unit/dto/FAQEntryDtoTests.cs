using CloudSpritzers1.Src.Dto;
using CloudSpritzers1.Src.Model.Faq;

namespace CloudSpritzers1Tests.Dto;

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
