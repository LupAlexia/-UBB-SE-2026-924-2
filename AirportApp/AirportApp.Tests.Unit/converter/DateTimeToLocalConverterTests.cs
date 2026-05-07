using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Converter;

namespace AirportApp.Tests.Unit.src.converter;

[TestClass]
public class DateTimeToLocalConverterTests
{
    private DateTimeToLocalConverter converter;

    [TestInitialize]
    public void Setup()
    {
        converter = new DateTimeToLocalConverter();
    }

    [TestMethod]
    public void Convert_ValidDateTimeOffset_ReturnsFormattedString()
    {
        var input = new DateTimeOffset(2024, 1, 10, 12, 30, 0, TimeSpan.Zero);

        var result = converter.Convert(input, typeof(string), null, null);

        Assert.IsInstanceOfType(result, typeof(string));
    }

    [TestMethod]
    public void Convert_ValidDateTimeOffset_FormatIsCorrect()
    {
        var input = new DateTimeOffset(2024, 1, 10, 12, 30, 0, TimeSpan.Zero);

        var result = converter.Convert(input, typeof(string), null, null) as string;

        Assert.IsTrue(result.Contains("Jan") || result.Contains("Feb") || result.Contains("Mar")
            || result.Contains("Apr") || result.Contains("May") || result.Contains("Jun")
            || result.Contains("Jul") || result.Contains("Aug") || result.Contains("Sep")
            || result.Contains("Oct") || result.Contains("Nov") || result.Contains("Dec"));
    }

    [TestMethod]
    public void Convert_InvalidType_ReturnsSameValue()
    {
        var input = "not a date";

        var result = converter.Convert(input, typeof(string), null, null);

        Assert.AreEqual(input, result);
    }

    [TestMethod]
    public void Convert_NullValue_ReturnsNull()
    {
        var result = converter.Convert(null, typeof(string), null, null);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ConvertBack_WhenCalled_ThrowsNotImplementedException()
    {
        Assert.ThrowsException<NotImplementedException>(() =>
            converter.ConvertBack("Jan 10, 12:30", typeof(DateTimeOffset), null, null));
    }
}




