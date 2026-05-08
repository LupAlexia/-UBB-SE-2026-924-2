using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AirportApp.Src.Converter;
using Microsoft.UI.Xaml;

namespace AirportApp.Tests.Unit.src.converter;

[TestClass()]
public class BooleanToGlyphConverterTests
{
    private BooleanToGlyphConverter converter;

    [TestInitialize]
    public void Setup()
    {
        converter = new BooleanToGlyphConverter();
    }

    [TestMethod()]
    public void Convert_TrueValue_ReturnsCorrespondingString()
    {
        var result = converter.Convert(true, typeof(string), null, null);
        Assert.AreEqual("\uE70D", result);
    }

    [TestMethod()]
    public void Convert_FalseValue_ReturnsCorrespondingString()
    {
        var result = converter.Convert(false, typeof(string), null, null);
        Assert.AreEqual("\uE76C", result);
    }

    [TestMethod()]
    public void ConvertBack_WhenCalled_ThrowsNotImplementedException()
    {
        Assert.ThrowsException<NotImplementedException>(() => converter.ConvertBack(HorizontalAlignment.Left, typeof(bool), null, null));
    }
}



