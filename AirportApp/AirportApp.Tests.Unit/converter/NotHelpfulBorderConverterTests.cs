using CloudSpritzers1.Src.Converter;
using Windows.UI;

namespace CloudSpritzers1Tests.Src.Converter;

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

        Assert.ThrowsExactly<NotImplementedException>(() => _converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }
}
