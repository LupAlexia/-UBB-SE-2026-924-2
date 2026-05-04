using CloudSpritzers1.Src.View.Faq;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using CloudSpritzers1.Src.Converter;

namespace CloudSpritzers1Tests.Src.Converter;

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

        Assert.ThrowsExactly<NotImplementedException>(() => _converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }
}
