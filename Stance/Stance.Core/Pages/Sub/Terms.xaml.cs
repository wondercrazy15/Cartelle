using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class Terms : ContentPage
    {
        private const string _PageName = "Terms";

        public Terms()
        {
            InitializeComponent();

            var browser = new WebView
            {
                Source = "https://thecartelle.com/terms.html"
            };
            Content = browser;
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
