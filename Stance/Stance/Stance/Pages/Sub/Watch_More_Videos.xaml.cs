using Octane.Xam.VideoPlayer;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class Watch_More_Videos : ContentPage
    {
        String _FilePathToVideo = String.Empty;


        public Watch_More_Videos(string fileName = null)
        {
            InitializeComponent();
           
            VideoPlayID.Source = VideoSource.FromResource(fileName);           

        }

        async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
