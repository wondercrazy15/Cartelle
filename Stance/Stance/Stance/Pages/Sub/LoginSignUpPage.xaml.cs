using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using SQLite;
using Stance.Models;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.Pages.Main;
using Stance.Utils;
using Stance.Utils.Auth;
using Stance.Utils.TimeZone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class LoginSignUpPage : ContentPage
    {
        public string _programId = String.Empty;
        public Guid _programGuid = Guid.Empty;
        private HttpClient _client = new HttpClient(new NativeMessageHandler());
        private static SQLiteAsyncConnection _connection;
        private const string _PageName = "login/sign-up";


        public LoginSignUpPage(string programId = null)
        {
            InitializeComponent();

            _programId = programId;
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();

            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;
            if (CrossConnectivity.Current.IsConnected)
            {
                NoNetwork.IsVisible = false;
            }
            else
            {
                NoNetwork.IsVisible = true;
            }

            if (programId != null && programId != "")
            {
                //If programId is not null, redirect to purchase page after setting up an account
                //Display message that they must first create an account or login before purchasing this program
                Guid.TryParse(programId, out _programGuid);

                if (_programGuid != Guid.Empty)
                {
                    ProgramMessage.Text = "First create an account or sign-in to associate your purchase with your account.";
                }
            }

            SignInSL.IsVisible = false;
            TandCMessage.Text = "By clicking JOIN NOW, you agree to The Cartelle's Terms of Use and Privacy Policy";
            Spinner.IsVisible = false;

            foreach (var tz in TimeZoneConverter.GetListOfTimeZones())
            {
                TimeZone.Items.Add(tz);
            }

        }

        private async void ExitBtn_Clicked()
        {
            await Navigation.PopModalAsync();
        }

        private async void JoinSubmitted()
        {
            Spinner.IsVisible = true;
            JoinNowBtn.IsEnabled = false;
            ShowSignIn_Btn.IsEnabled = false;
            FirstName.IsEnabled = false;
            LastName.IsEnabled = false;
            Email.IsEnabled = false;
            Password.IsEnabled = false;
            TimeZone.IsEnabled = false;

            FormValidationSpecialMessage.Text = "";
            string specialErrorMessage = "";
            bool IsValide = true;
            Regex nameReg = new Regex(@"^[a-zA-Z\s]*$", RegexOptions.IgnoreCase);
            Regex emailReg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);
            //one lower case, one upper case, one number, 8-15 characters, if you want one special character ^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$
            Regex passwordReg = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$");

            if (FirstName.Text == null || FirstName.Text == "")
            {
                FirstName.BackgroundColor = Color.FromHex("#ffb2b2");
                IsValide = false;
            }
            else if (!nameReg.Match(FirstName.Text).Success)
            {
                FirstName.BackgroundColor = Color.FromHex("#ffb2b2");
                specialErrorMessage = "Your name doesn't look real. ";
                IsValide = false;
            }
            else
            {
                FirstName.BackgroundColor = Color.FromHex("#99ccff");
            }

            if (LastName.Text == null || LastName.Text == "")
            {
                LastName.BackgroundColor = Color.FromHex("#ffb2b2");
                IsValide = false;
            }
            else if (!nameReg.Match(LastName.Text).Success)
            {
                if (!specialErrorMessage.Contains("name"))
                {
                    specialErrorMessage = "Your name doesn't look real. ";
                }
                LastName.BackgroundColor = Color.FromHex("#ffb2b2");
                IsValide = false;
            }
            else
            {
                LastName.BackgroundColor = Color.FromHex("#99ccff");
            }

            if (Email.Text == null || Email.Text == "")
            {
                Email.BackgroundColor = Color.FromHex("#ffb2b2");
                IsValide = false;
            }
            else if (!emailReg.Match(Email.Text).Success)
            {
                Email.BackgroundColor = Color.FromHex("#ffb2b2");
                specialErrorMessage += "Email doesn't look real. ";
                IsValide = false;
            }
            else
            {
                Email.BackgroundColor = Color.FromHex("#99ccff");
            }

            if (Password.Text == null || Password.Text == "")
            {
                Password.BackgroundColor = Color.FromHex("#ffb2b2");
                IsValide = false;
            }
            else if (!passwordReg.Match(Password.Text).Success)
            {
                Password.BackgroundColor = Color.FromHex("#ffb2b2");
                specialErrorMessage += "Password must contain 1 lowercase, uppercase, number and be 8-15 characters.";
                IsValide = false;
            }
            else
            {
                Password.BackgroundColor = Color.FromHex("#99ccff");
            }

            if (TimeZone.SelectedIndex == -1)
            {
                TimeZone.BackgroundColor = Color.FromHex("#ffb2b2");
                specialErrorMessage += "Time Zone is required.";
                IsValide = false;
            }

            if (!IsValide)
            {
                //Error
                FormValidationSpecialMessage.Text = specialErrorMessage;
                JoinNowBtn.IsEnabled = true;
                ShowSignIn_Btn.IsEnabled = true;
                FirstName.IsEnabled = true;
                LastName.IsEnabled = true;
                Email.IsEnabled = true;
                Password.IsEnabled = true;
                TimeZone.IsEnabled = true;
            }
            else
            {
                FormValidationSpecialMessage.Text = "";

                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {
                        JoinNowTitle.Text = "JOINING...";
                        //POST - Create a new Contact via API and wait to hear success back which Set Contact Created In App Field to Yes and Send welcome Email
                        //Send Fist and last name, email, and password
                        APIContact newContact = new APIContact
                        {
                            FirstName = FirstName.Text,
                            LastName = LastName.Text,
                            Email = Email.Text,
                            Password = Password.Text,
                            TimeZone = TimeZone.Items[TimeZone.SelectedIndex]
                        };

                        //upload
                        string json = JsonConvert.SerializeObject(newContact);
                        var contentString = new StringContent(json, Encoding.UTF8, "application/json");
                        var newUri = new Uri(App._absoluteUri, App._contactsUri);
                        _client.DefaultRequestHeaders.Add("Authorization", GeneralAuth.Token);

                        HttpResponseMessage response = await _client.PostAsync(newUri, contentString);
                        //var request = new HttpRequestMessage();
                        //request.RequestUri = new Uri(_absoluteUri, _contactsUri + "?firstname=" + FirstName.Text+ "&lastname=" + LastName.Text + "&email=" + Email.Text+ "&password=" + Password.Text);
                        //request.Method = HttpMethod.Post;
                        //request.Content = contentString;
                        //request.Headers.Add("Accept", "application/json");
                        //_client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");

                        //HttpResponseMessage response = await _client.SendAsync(request);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            JoinNowTitle.IsVisible = false;
                            Spinner.IsVisible = false;
                            FormValidationSpecialMessage.Text = "SUCCESS";
                            FormValidationSpecialMessage.TextColor = Color.White;
                            FormValidationSpecialMessage.FontSize = 30;
                            FormValidationSpecialMessage.FontAttributes = FontAttributes.Bold;

                            //Store user credentials in app 
                            Auth.SaveCredentials(Email.Text, Password.Text);

                            var profile = new LocalDBContact
                            {
                                FirstName = FirstName.Text,
                                LastName = LastName.Text,
                                Email = Email.Text,
                                Password = Password.Text,
                                TimeZone = TimeZone.Items[TimeZone.SelectedIndex]
                            };

                            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
                            await _connection.InsertAsync(profile);

                            await Task.Delay(1000);
                            if (_programGuid != Guid.Empty)
                            {
                               // await Navigation.PushModalAsync(new PurchaseOutcome(_programGuid), false);
                            }
                            else
                            {
                                App.Current.MainPage = new MainStartingPage();
                            }
                        }
                        else
                        {
                            JoinNowTitle.Text = "JOIN NOW";
                            FormValidationSpecialMessage.Text = "We have encountered a problem with signing you up, please try a different email address or make sure you are connected to the internet.";
                            //FormValidationSpecialMessage.Text = response.StatusCode + " - " + response.Headers;
                            JoinNowBtn.IsEnabled = true;
                            ShowSignIn_Btn.IsEnabled = true;
                            FirstName.IsEnabled = true;
                            LastName.IsEnabled = true;
                            Email.IsEnabled = true;
                            Password.IsEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
                        JoinNowTitle.Text = "JOIN NOW";
                    }
                }
                else
                {
                    await DisplayAlert("NO INTERNET", "Please connect to the internet and try again.", "OK");
                }


            }
            Spinner.IsVisible = false;

        }

        private async void SignInSubmitted()
        {
            Spinner.IsVisible = true;

            SignInBtn.IsEnabled = false;
            ShowJoinNow_Btn.IsEnabled = false;
            Email_SignIn.IsEnabled = false;
            Password_SignIn.IsEnabled = false;

            if (Email_SignIn.Text == null || Email_SignIn.Text == "" || Password_SignIn.Text == null || Password_SignIn.Text == "")
            {
                FormValidationSpecialMessage.Text = "Enter your Email and Password.";
                SignInBtn.IsEnabled = true;
                ShowJoinNow_Btn.IsEnabled = true;
                Email_SignIn.IsEnabled = true;
                Password_SignIn.IsEnabled = true;
            }
            else
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    try
                    {
                        SignInTitle.Text = "SIGNING IN...";
                        //Send Post request with the username and password to authenticate them
                        var request = new HttpRequestMessage();
                        request.RequestUri = new Uri(App._absoluteUri, App._contactsUri);
                        request.Method = HttpMethod.Get;
                        request.Headers.Add("Accept", "application/json");

                        string _auth = string.Format("{0}:{1}", Email_SignIn.Text, Password_SignIn.Text);
                        string _enc = Convert.ToBase64String(Encoding.UTF8.GetBytes(_auth));
                        string _cred = string.Format("{0} {1}", "Basic", _enc);
                        request.Headers.Add("Authorization", _cred);

                        HttpResponseMessage response = await _client.SendAsync(request);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var request2 = new HttpRequestMessage();
                            request2.RequestUri = new Uri(App._absoluteUri, App._profileUri);
                            request2.Method = HttpMethod.Get;
                            request2.Headers.Add("Accept", "application/json");
                            request2.Headers.Add("Authorization", _cred);

                            HttpResponseMessage response2 = await _client.SendAsync(request2);

                            if (response2.StatusCode == HttpStatusCode.OK)
                            {
                                HttpContent content = response2.Content;
                                var json = await content.ReadAsStringAsync();
                                var contactProfile = JsonConvert.DeserializeObject<APIContact>(json);

                                if (contactProfile != null)
                                {
                                    var profile = new LocalDBContact();
                                    profile.GuidCRM = contactProfile.GuidCRM;
                                    profile.StateCodeValue = contactProfile.StateCodeValue;
                                    profile.StatusCodeValue = contactProfile.StatusCodeValue;
                                    profile.FirstName = contactProfile.FirstName;
                                    profile.LastName = contactProfile.LastName;
                                    profile.Email = contactProfile.Email;
                                    profile.TimeZone = contactProfile.TimeZone;
                                    profile.Birthday = contactProfile.Birthday;
                                    profile.GenderTypeCode = contactProfile.GenderTypeCode;
                                    profile.Gender = contactProfile.Gender;
                                    profile.HeightCm = contactProfile.HeightCm;
                                    profile.WeightLbs = contactProfile.WeightLbs;
                                    profile.TrainingGoalTypeCode = contactProfile.TrainingGoalTypeCode;
                                    profile.TrainingGoal = contactProfile.TrainingGoal;
                                    profile.RegionTypeCode = contactProfile.RegionTypeCode;
                                    profile.Region = contactProfile.Region;

                                    await _connection.InsertAsync(profile);
                                }
                            }

                            //If good
                            SignInTitle.IsVisible = false;
                            Spinner.IsVisible = false;
                            FormValidationSpecialMessage.Text = "SUCCESS";
                            FormValidationSpecialMessage.TextColor = Color.White;
                            FormValidationSpecialMessage.FontSize = 30;
                            FormValidationSpecialMessage.FontAttributes = FontAttributes.Bold;
                            //Store user credentials in app 
                            Auth.SaveCredentials(Email_SignIn.Text, Password_SignIn.Text);

                            //await Task.Delay(1000);

                            if (_programGuid != Guid.Empty)
                            {
                               // await Navigation.PushModalAsync(new PurchaseOutcome(_programGuid), false);
                            }
                            else
                            {
                                App.Current.MainPage = new MainStartingPage();
                            }

                        }
                        else
                        {
                            //If Bad
                            SignInTitle.Text = "SIGN IN";
                            FormValidationSpecialMessage.Text = "Email or Password are invalid";
                            SignInBtn.IsEnabled = true;
                            ShowJoinNow_Btn.IsEnabled = true;
                            Email_SignIn.IsEnabled = true;
                            Password_SignIn.IsEnabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        // await DisplayAlert("ERROR", "Report issue to app developer: " + _PageName, "OK");
                        SignInTitle.Text = "SIGN IN";
                    }
                }
                else
                {
                    await DisplayAlert("NO INTERNET", "Please connect to the internet and try again.", "OK");
                }


            }
            Spinner.IsVisible = false;

        }

        private void ShowSignIn()
        {
            FormValidationSpecialMessage.Text = "";
            ShowSignIn_Btn.BackgroundColor = Color.Black;
            JoinNowSL.IsVisible = false;
            SignInSL.IsVisible = true;
            ShowSignIn_Btn.BackgroundColor = Color.FromHex("#00bac6");
            ShowJoinNow_Btn.BackgroundColor = Color.Transparent;

        }

        private void ShowJoinNow()
        {
            FormValidationSpecialMessage.Text = "";
            ShowJoinNow_Btn.BackgroundColor = Color.Black;
            SignInSL.IsVisible = false;
            JoinNowSL.IsVisible = true;
            ShowJoinNow_Btn.BackgroundColor = Color.FromHex("#00bac6");
            ShowSignIn_Btn.BackgroundColor = Color.Transparent;

        }

        private void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                NoNetwork.IsVisible = false;
            }
            else if (!e.IsConnected)
            {
                NoNetwork.IsVisible = true;
            }
        }


    }
}
