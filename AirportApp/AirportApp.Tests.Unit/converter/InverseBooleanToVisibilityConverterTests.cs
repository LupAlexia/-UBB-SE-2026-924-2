using AirportApp.Src.Converter;
using Microsoft.UI.Xaml;
namespace AirportApp.Tests.Unit.src.converter;

[TestClass]
public class InverseBooleanToVisibilityConverterTests
{
    private InverseBooleanToVisibilityConverter converter;

    [TestInitialize]
    public void Setup()
    {
        converter = new InverseBooleanToVisibilityConverter();
    }

    [TestMethod]
    public void Convert_FalseToVisible_Succeeds()
    {
        var result = converter.Convert(false, typeof(Visibility), null, null);
        Assert.AreEqual(Visibility.Visible, result);
    }

    [TestMethod]
    public void Convert_TrueToCollapsede_Succeeds()
    {
        var result = converter.Convert(true, typeof(Visibility), null, null);
        Assert.AreEqual(Visibility.Collapsed, result);
    }

    [TestMethod()]
    public void ConvertBack_WhenCalled_ThrowsNotImplementedException()
    {
        Assert.ThrowsException<NotImplementedException>(() => converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, null));
    }
}





