using CloudSpritzers1.Src.View.Faq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.UI.Dispatching;
using System;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Converter;
namespace CloudSpritzers1Tests.Src.Converter;

[TestClass]
public class NotHelpfulBackgroundConverterTests
{
    private NotHelpfulBackgroundConverter _converter;

    [TestInitialize]
    public void Setup()
    {
        _converter = new NotHelpfulBackgroundConverter();
    }

    [TestMethod()]
    public void ConvertBack_WhenCalled_ThrowsNotImplementedException()
    {

        Assert.ThrowsExactly<NotImplementedException>(() => _converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }

}
