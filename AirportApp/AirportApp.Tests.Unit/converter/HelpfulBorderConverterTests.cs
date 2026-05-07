using AirportApp.Src.Converter;
using Windows.UI;

namespace AirportApp.Tests.Unit.Src.Converter;

[TestClass]
public class HelpfulBorderConverterTests
{
    private HelpfulBorderConverter converter;

    [TestInitialize]
    public void Setup()
    {
        converter = new HelpfulBorderConverter();
    }

    [TestMethod()]
    public void ConvertBack_WhenCalled_ThrowsNotImplementedException()
    {
        Assert.ThrowsException<NotImplementedException>(() => converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }
}





