using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.UI.Dispatching;
using AirportApp.Src.Converter;
namespace AirportApp.Tests.Unit.Src.Converter;

[TestClass]
public class NotHelpfulBackgroundConverterTests
{
    private NotHelpfulBackgroundConverter converter;

    [TestInitialize]
    public void Setup()
    {
        converter = new NotHelpfulBackgroundConverter();
    }

    [TestMethod()]
    public void ConvertBack_WhenCalled_ThrowsNotImplementedException()
    {
        Assert.ThrowsException<NotImplementedException>(() => converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }
}






