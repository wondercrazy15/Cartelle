using Stance.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class Testing_Page : ContentPage
    {
        public Testing_Page()
        {
            InitializeComponent();
        }

        async void ViewImageBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new ViewImage_Page(ImageUrl.Text));
        }

        async void WatchVideoBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new WatchVideoPage(VideoUrl.Text));
        }

        void HearAudioBtn_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushModalAsync(new HearAudio_Page(AudioUrl.Text));
        }

        void DownloadBtn_Clicked(object sender, EventArgs e)
        {

            var t = new DownloadResources();
            t.InitiateVideoDownload();

            //works
            //var dT = DependencyService.Get<IDownload>();
            //dT.btnDownload_Click();

        }


    }
}
