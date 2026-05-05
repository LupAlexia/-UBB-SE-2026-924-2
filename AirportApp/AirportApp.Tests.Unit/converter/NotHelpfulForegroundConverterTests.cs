using Microsoft.UI.Xaml.Media;
using Windows.UI;
using AirportApp.Src.Converter;

namespace AirportApp.Tests.Unit.Src.Converter;

[TestClass]
public class NotHelpfulForegroundConverterTests
{
    private NotHelpfulForegroundConverter _converter;

    [TestInitialize]
    public void Setup()
    {
        _converter = new NotHelpfulForegroundConverter();
    }

    [TestMethod()]
    public void ConvertBack_WhenCalled_ThrowsNotImplementedException()
    {

        Assert.ThrowsException<NotImplementedException>(() => _converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }
}






