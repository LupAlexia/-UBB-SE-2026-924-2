using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.UI.Dispatching;
using AirportApp.Src.Converter;

namespace AirportApp.Tests.Unit.src.converter;

[TestClass]
public class HelpfulBackgroundConverterTests
{
    private HelpfulBackgroundConverter converter;

    [TestInitialize]
    public void Setup()
    {
        converter = new HelpfulBackgroundConverter();
    }

    [TestMethod()]
    public void ConvertBack_WhenCalled_ThrowsNotImplementedException()
    {
        Assert.ThrowsException<NotImplementedException>(() => converter.ConvertBack(Color.FromArgb(255, 248, 249, 251), typeof(bool), null, null));
    }
}






