using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class Video_Test_Page : ContentPage
    {
        const string _video1 = "t1.m2ts";
        //const string _video1 = "snow.mp4";

        const string _video2 = "t2.mp4";
        const string _video3 = "t3.mp4";
        
        const string _video4 = "1080p_h264_2mbps.mp4";
        const string _video5 = "1080p_h264_25mbps.mp4";
        const string _video6 = "1080p_h265_20mbps.mp4";
        const string _video7 = "1080p_h265_25mbps.mp4";


        public Video_Test_Page()
        {
            InitializeComponent();

            var btn1 = new Button
            {
                Text = "720p 1.8 m2ts",
            };

            btn1.Clicked += HandleClick1;

            var btn2 = new Button
            {
                Text = "720p 2.0 h264",
            };
            btn2.Clicked += HandleClick2;

            var btn3 = new Button
            {
                Text = "720p 2.5 h264",
            };
            btn3.Clicked += HandleClick3;


            var btn4 = new Button
            {
                Text = "1080p 2.0 h264",
            };
            btn4.Clicked += HandleClick4;

            var btn5 = new Button
            {
                Text = "1080p 2.5 h264",
            };
            btn5.Clicked += HandleClick5;

            var btn6 = new Button
            {
                Text = "1080p 2.0 h265",
            };
            btn6.Clicked += HandleClick6;

            var btn7 = new Button
            {
                Text = "1080p 2.5 h265",
            };
            btn7.Clicked += HandleClick7;




            var SL = new StackLayout
            {
                BackgroundColor = Color.Yellow,
            };

            SL.Children.Add(btn1);
            SL.Children.Add(btn2);
            SL.Children.Add(btn3);

            SL.Children.Add(btn4);
            SL.Children.Add(btn5);
            SL.Children.Add(btn6);
            SL.Children.Add(btn7);

            Content = SL;
        }



        public async void HandleClick1(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Watch_More_Videos(_video1));
        }

        public async void HandleClick2(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Watch_More_Videos(_video2));
        }

        public async void HandleClick3(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Watch_More_Videos(_video3));
        }

        public async void HandleClick4(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Watch_More_Videos(_video4));
        }

        public async void HandleClick5(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Watch_More_Videos(_video5));
        }

        public async void HandleClick6(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Watch_More_Videos(_video6));
        }

        public async void HandleClick7(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Watch_More_Videos(_video7));
        }





    }
}
