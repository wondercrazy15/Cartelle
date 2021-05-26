using Com.Allyants.Draggabletreeview;
using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Connectivity;
using SQLite;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.Utils;
using Stance.Utils.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stance.Pages.Sub
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditWorkoutSchedule : ContentPage
    {
        private static SQLiteAsyncConnection _connection;

        public class Cell1
        {
            public int id { get; set; }
            public int position { get; set; }
            public Label Label { get; set; }
        }

        public class Comp
        {
            public Cell1 cell { get; set; }
            public StackLayout SL { get; set; }
        }

        private List<Comp> _comps = new List<Comp>();
        private List<Comp> _compsInitial = new List<Comp>();

        public EditWorkoutSchedule(int weekNum, Guid ContactProgramGuid)
        {
            InitializeComponent();

            int i = 7;
            int j = 0;

            while (j < i)
            {
                j++;

                var daySL = new StackLayout()
                {
                    HeightRequest = 40,
                    //BackgroundColor = Color.Green,
                    HorizontalOptions = LayoutOptions.FillAndExpand,     
                    VerticalOptions = LayoutOptions.CenterAndExpand                                  
                };

                var lab = new Label
                {
                    Text = j.ToString(),
                    FontSize = 22,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };

                daySL.Children.Add(lab);

                var cell1 = new Cell1
                {
                    id = j,
                    position = j - 1,
                    Label = lab,
                };
                
                var comp = new Comp
                {
                    SL = daySL,
                    cell = cell1
                };

                _comps.Add(comp);

                var tapGestureRecognizer = new TapGestureRecognizer
                {
                    Command = new Command<Comp>(HandleCell1Click),
                    CommandParameter = comp,
                };
                daySL.GestureRecognizers.Add(tapGestureRecognizer);

                listOfContactProgramDays.Children.Add(daySL);
            }
            _compsInitial = _comps;

            Spinner.IsVisible = false;

        }

        private async void HandleCell1Click(Comp comp)
        {
            var currentPosition = comp.cell.position;

            if(currentPosition > 0) {
                //move up
                var tempLabelAbove = _comps[currentPosition - 1].cell.Label.Text;
                var tempLabelCurrent = _comps[currentPosition].cell.Label.Text;

                comp.cell.Label.Text = tempLabelAbove;
                _comps[currentPosition - 1].cell.Label.Text = tempLabelCurrent;

            }

        }

        private async void SaveProfile_Btn()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                SaveText.IsVisible = false;
                Spinner.IsVisible = true;

                try
                {
                   

                    APIContact editedContact = new APIContact
                    {

                    };

                  

                    //upload
                    HttpClient _client = new HttpClient(new NativeMessageHandler());
                    string json = JsonConvert.SerializeObject(editedContact);
                    var contentString = new StringContent(json, Encoding.UTF8, "application/json");
                    var newUri = new Uri(App._absoluteUri, App._profileUri);
                    _client.DefaultRequestHeaders.Add("Authorization", Auth.Token);
                    //_client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic " + Auth.Token);
                    //var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", Auth.Token);
                    //_client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

                    HttpResponseMessage response = await _client.PostAsync(newUri, contentString);


                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //Spinner.IsVisible = false;
                        //FormValidationSpecialMessage.Text = "SUCCESS";
                        //FormValidationSpecialMessage.TextColor = Color.White;
                        //FormValidationSpecialMessage.FontSize = 30;
                        //FormValidationSpecialMessage.FontAttributes = FontAttributes.Bold;

                        _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

                        var Profile = await _connection.Table<LocalDBContact>().FirstOrDefaultAsync();

                        if (Profile != null)
                        {

                           


                            await _connection.UpdateAsync(Profile);
                        }

                        MessagingCenter.Send(this, "UpdatedProfile");

                        await Navigation.PopModalAsync(false);

                    }
                    else
                    {
                        await DisplayAlert("CANNOT UPDATE", "Try again later.", "OK");
                        await Navigation.PopModalAsync();
                    }
                }
                catch (Exception ex)
                {
                    //await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
                    await Navigation.PopModalAsync();
                }
            }
            else
            {
                var result = await DisplayAlert("NO INTERNET", "Connect to the internet and try again.", "TRY AGAIN", "DON'T SAVE");
                if (!result)
                {
                    await Navigation.PopModalAsync();
                }
            }

            Spinner.IsVisible = false;
            SaveText.IsVisible = true;


        }

        private async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}