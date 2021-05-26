using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class PrivacyPolicy : ContentPage
    {
        private const string _PageName = "Privacy Policy";

        public PrivacyPolicy()
        {
            InitializeComponent();

            var browser = new WebView
            {
                Source = "https://thecartelle.com/privacy.html"
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
