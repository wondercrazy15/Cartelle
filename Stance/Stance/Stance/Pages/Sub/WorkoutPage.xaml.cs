using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class WorkoutPage : ContentPage
    {
        public WorkoutPage()
        {
            InitializeComponent();

            //var imageSource = (UriImageSource)ImageSource.FromUri(new Uri("http://lorempixel.com/1920/1080/sports/7/"));

            var imgSource = new UriImageSource { Uri = new Uri("http://lorempixel.com/1920/1080/sports/7/") };
            imgSource.CachingEnabled = true;
            imgSource.CacheValidity = TimeSpan.FromHours(8);

            image.Source = imgSource;
            image.Aspect = Aspect.AspectFill;
            
        }
    }
}
