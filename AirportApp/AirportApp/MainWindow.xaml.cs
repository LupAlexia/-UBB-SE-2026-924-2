using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AirportApp.Src.Helpers.WindowHelper.MaximizeWindow(this);
        }
    }
}