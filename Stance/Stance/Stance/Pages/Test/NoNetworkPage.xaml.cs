using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Test
{
    public partial class NoNetworkPage : ContentPage
    {
        public NoNetworkPage()
        {
            InitializeComponent();

            BackgroundColor = Color.FromRgb(0xf0, 0xf0, 0xf0);
            Content = new Label()
            {
                Text = "No Network Connection Available",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.FromRgb(0x40, 0x40, 0x40),
            };
        }
    }
}
