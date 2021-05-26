using Newtonsoft.Json;
using Plugin.Connectivity;
using SQLite;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.Pages.Main;
using Stance.Utils;
using Stance.Utils.Auth;
using Stance.Utils.LocalDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class PurchaseOutcome : ContentPage
    {
        private Guid _programId = Guid.Empty;
        private HttpClient _client = new HttpClient();
        private const string _PageName = "purchase outcome";

        public PurchaseOutcome(Guid programGuid)
        {
            InitializeComponent();
            //Do the download and purchase here

            _programId = programGuid;
            SeeProgramBtn.IsVisible = false;

            if (Auth.IsAuthenticated())
            {
                purchaseOutcome.Text = "SETTING UP YOUR PROGRAM...";
                SetupProgramForContact();
            }
            else
            {
                purchaseOutcome.Text = "YOU MUST LOGGIN!";
                purchaseMessage.Text = "You cannot purchase a program without being logged in first.";
            }

        }

        private async void SetupProgramForContact()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(App._absoluteUri, App._transactionsUri + _programId);
                    request.Method = HttpMethod.Post;
                    request.Headers.Add("Accept", "application/json");

                    string _auth = string.Format("{0}:{1}", Auth.Username, Auth.Password);
                    string _enc = Convert.ToBase64String(Encoding.UTF8.GetBytes(_auth));
                    string _cred = string.Format("{0} {1}", "Basic", _enc);
                    request.Headers.Add("Authorization", _cred);

                    HttpResponseMessage response = await _client.SendAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        HttpContent content = response.Content;
                        var json = await content.ReadAsStringAsync();
                        var contactProgram = JsonConvert.DeserializeObject<APIContactProgram>(json);

                        if (contactProgram != null)
                        {
                            Database.SaveContactProgram(contactProgram);

                            purchaseOutcome.Text = "SUCCESS!";
                            purchaseMessage.Text = "Your Program is setup, and is available in the Workout Tab.";
                            SeeProgramBtn.IsVisible = true;
                        }

                    }
                    else if (response.StatusCode == HttpStatusCode.Forbidden)
                    {

                        purchaseOutcome.Text = "YOU ALREADY PURCHASED A PROGRAM.";
                        purchaseMessage.Text = "At this time you can only have a single program.";
                        SeeProgramBtn.IsVisible = true;


                    }
                    else
                    {
                        purchaseOutcome.Text = "OPPS SOMETHING WENT WRONG.";
                        purchaseMessage.Text = "Sorry we are working on it. Please try again later.";
                        SeeProgramBtn.IsVisible = true;

                    }
                }
                catch (Exception ex)
                {
                    purchaseOutcome.Text = "ERROR!";
                    purchaseMessage.Text = "We could not setup your program right now. Please try again later.";
                    SeeProgramBtn.IsVisible = true;
                    await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");

                }
            } else
            {
                await DisplayAlert("NO INTERNET", "Please connect to the internet and try again.", "OK");
                App.Current.MainPage = new MainStartingPage();
            }

        }

        private void SeeProgramBtn_Clicked()
        {
            App.Current.MainPage = new MainStartingPage();
        }

    }
}
