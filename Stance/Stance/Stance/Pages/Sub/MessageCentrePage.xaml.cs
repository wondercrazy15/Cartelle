using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class MessageCentrePage : ContentPage
    {
        public MessageCentrePage()
        {
            InitializeComponent();
        }

        async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

    }
}
