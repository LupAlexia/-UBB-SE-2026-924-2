using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AirportApp.Src.ViewModel
{
	public class ChoosingPageViewModel
	{
        public void SetUserRole(string roleTag)
        {
            bool isEmployee = roleTag == "Employee";

            var application = (App)Microsoft.UI.Xaml.Application.Current;
            application.IsEmployee = isEmployee;
        }
    }
}

