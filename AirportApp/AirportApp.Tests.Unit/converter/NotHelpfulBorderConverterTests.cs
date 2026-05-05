using AirportApp.Src.Converter;
using Windows.UI;

namespace AirportApp.Tests.Unit.Src.Converter;

[TestClass]
public class NotHelpfulBorderConverterTests
{
    private NotHelpfulBorderConverter _converter;

    [TestInitialize]
    public void Setup()
    {
        _converter = new NotHelpfulBorderConverter();
    }

    [TestMethod()]
    public void ConvertBack_WhenCalled_ThrowsNotImplementedException()
    {

        Assert.ThrowsException<NotImplementedException>(() => _converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }
}





