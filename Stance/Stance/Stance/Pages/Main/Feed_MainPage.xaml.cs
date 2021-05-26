using Stance.Pages.Sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Main
{
    public partial class Feed_MainPage : ContentPage
    {
        public Feed_MainPage()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "");

            var toolItem1 = new ToolbarItem
            {
                Icon = "user_26.png",
                Order = ToolbarItemOrder.Primary,
                Priority = 1
            };

            toolItem1.Clicked += (s, e) => {
                ProfileBtn_Clicked(s, e);
            };

            var toolItem2 = new ToolbarItem
            {
                Icon = "Message_26.png",
                Order = ToolbarItemOrder.Primary,
                Priority = 0
            };

            toolItem2.Clicked += (s, e) => {
                //InboxBtn_Clicked(s, e);
            };

            ToolbarItems.Add(toolItem1);
            ToolbarItems.Add(toolItem2);


        }

        async void ProfileBtn_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new PersonalProfile());
        }

 
    }


}
