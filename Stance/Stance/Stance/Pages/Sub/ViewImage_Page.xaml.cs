using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class ViewImage_Page : ContentPage
    {
        private string _imgUrl = String.Empty;

        public ViewImage_Page(string imgUrl = null)
        {
            InitializeComponent();

            var btnCenter = new Button
            {
                Text = "Center",
                HeightRequest = 50,
                BackgroundColor = Color.Black,
                TextColor = Color.White,

            };

            btnCenter.Clicked += (s, e) =>
            {
                CenterImg();
            };

            var btnCenterAndExpand = new Button
            {
                Text = "CenterAndExpand",
                HeightRequest = 50,
                BackgroundColor = Color.Aqua,
                TextColor = Color.White,
            };

            btnCenterAndExpand.Clicked += (s, e) =>
            {
                CenterAndExpand();
            };

            var btnFillAndExpand = new Button
            {
                Text = "FillAndExpand",
                HeightRequest = 50,
                BackgroundColor = Color.Black,
                TextColor = Color.White,

            };

            btnFillAndExpand.Clicked += (s, e) =>
            {
                FillAndExpand();
            };

            var btnExit = new Button
            {
                Text = "Exit",
                HeightRequest = 50,
                BackgroundColor = Color.Aqua,
                TextColor = Color.White,

            };

            btnExit.Clicked += (s, e) =>
            {
                Exit();
            };


            if (imgUrl != null)
            {
                imgToModify.Source = imgUrl;
                imgToModify.Aspect = Aspect.AspectFill;
                imgToModify.VerticalOptions = LayoutOptions.FillAndExpand;
                _imgUrl = imgUrl;
            }


            //btnSL.Children.Add(btnCenter);
            //btnSL.Children.Add(btnCenterAndExpand);
            //btnSL.Children.Add(btnFillAndExpand);
            btnSL.Children.Add(btnExit);

        }

        async private void Exit()
        {
            await Navigation.PopModalAsync();
        }

        private void FillAndExpand()
        {
            if(_imgUrl != String.Empty)
            {
                imgToModify.VerticalOptions = LayoutOptions.FillAndExpand;
            }
        }

        private void CenterAndExpand()
        {
            if (_imgUrl != String.Empty)
            {
                imgToModify.VerticalOptions = LayoutOptions.CenterAndExpand;
            }
        }

        private void CenterImg()
        {
            if (_imgUrl != String.Empty)
            {
                imgToModify.VerticalOptions = LayoutOptions.Center;
            }
        }
    }
}
