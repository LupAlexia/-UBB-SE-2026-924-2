using Microsoft.UI.Xaml.Media;
using Windows.UI;
using AirportApp.Src.Converter;

namespace AirportApp.Tests.Unit.Src.Converter;

[TestClass]
public class NotHelpfulForegroundConverterTests
{
    private NotHelpfulForegroundConverter converter;

    [TestInitialize]
    public void Setup()
    {
        converter = new NotHelpfulForegroundConverter();
    }

    [TestMethod()]
    public void ConvertBack_WhenCalled_ThrowsNotImplementedException()
    {
        Assert.ThrowsException<NotImplementedException>(() => converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }
}






