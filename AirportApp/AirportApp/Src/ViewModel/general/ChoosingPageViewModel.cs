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
using AirportApp;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AirportApp.Src.ViewModel.General
{
	public class ChoosingPageViewModel
	{
        public void SetUserRole(string roleTag)
        {
            bool isEmployee = roleTag == "Employee";

            var application = (App)App.Current;
            application.IsEmployee = isEmployee;
        }
    }
}

