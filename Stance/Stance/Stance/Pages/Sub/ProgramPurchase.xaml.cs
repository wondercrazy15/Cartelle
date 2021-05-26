using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class ProgramPurchase : ContentPage
    {
        //This holders the Contacts Guid, Program Guid to be used for a transaction on the website;
        string UrlParamters = "";

        public ProgramPurchase()
        {
            InitializeComponent();

            //IF They have purchased this program....pushAsync to the the workoutOverview Page
            //how can we pass info from page to page... i think we use the constructor and pass info into it


            PurchasePageID.Source = "http://stanceathletes.com" + UrlParamters;
        }

        //async void Handle_Clicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new WorkoutOverview("1"));
        //}

    }
}
