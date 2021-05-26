using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class GenericWebviewPage : ContentPage
    {
        public GenericWebviewPage(string url = null)
        {
            InitializeComponent();
            PurchasePageID.Source = url;
        }

        async void Handle_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
