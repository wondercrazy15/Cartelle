using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Test
{
    public partial class TestingAppMethods : ContentPage
    {
        private static string _methodFrom;

        public TestingAppMethods(string methodFrom = null)
        {
            InitializeComponent();            

            _methodFrom = methodFrom;

            if(_methodFrom != null)
            {
                MethodName.Text = _methodFrom;
            }
        }



    }
}
