using Microsoft.UI.Xaml;
using AirportApp.Src.Converter;
namespace Airport.Tests.Unit.converter;

[TestClass]
public class BooleanToVisibilityConverterTests
{
    private BooleanToVisibilityConverter converter;

    [TestInitialize]
    public void Setup()
    {
        converter = new BooleanToVisibilityConverter();
    }

    [TestMethod]
    public void Convert_TrueToVisible_Succeeds()
    {
        var result = converter.Convert(true, typeof(Visibility), null, null);
        Assert.AreEqual(Visibility.Visible, result);
    }

    [TestMethod]
    public void Convert_FalseToCollapsed_Succeeds()
    {
        var result = converter.Convert(false, typeof(Visibility), null, null);
        Assert.AreEqual(Visibility.Collapsed, result);
    }

    [TestMethod]
    public void ConvertBack_VisibleToTrue_Succeeds()
    {
        var result = converter.ConvertBack(Visibility.Visible, typeof(bool), null, null);
        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void ConvertBack_CollapsedToFalse_Succeeds()
    {
        var result = converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, null);
        Assert.AreEqual(false, result);
    }
}




