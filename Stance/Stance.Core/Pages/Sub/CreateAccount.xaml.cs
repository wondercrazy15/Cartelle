using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Stance.Models.LocalDB;
using Stance.Models.Optimized;
using Stance.Pages.Main;
using Stance.Utils;
using Stance.Utils.Auth;
using Stance.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using StanceWeb.Models.App.Optimized;
using BranchXamarinSDK;

namespace Stance.Pages.Sub
{


    public partial class CreateAccount : BaseContentPage
    {
        private const string _PageName = "Create Account";
        private bool _IsCreatingAccount = false;
        private ContactSignUpV2 _ContactSignUp = new ContactSignUpV2();
        //#00BBCB button ready color, Background coolor red if error

        public CreateAccount(ContactSignUpV2 contactSignUp)
        {
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetBackButtonTitle(this, "");
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            _ContactSignUp = contactSignUp;

            if(_ContactSignUp.AccountGuid == Guid.Empty)//cartelle
            {
                string goalText = "for achieving your goal";
                if(_ContactSignUp.WorkoutGoal == "tone up")
                {
                    goalText = "for TONING UP your body";

                }else if (_ContactSignUp.WorkoutGoal == "fat loss")
                {
                    goalText = "for maximum WEIGHT LOSS";

                }else if (_ContactSignUp.WorkoutGoal == "strengthen")
                {
                    goalText = "for BUILDING MUSCLE your body";
                }              
                ProgramMessage.Text = "You'll be setup on our most popular " + _ContactSignUp.WorkoutLevel.ToUpper() + " " + _ContactSignUp.WorkoutSetting.ToUpper() + " program " + goalText + ". You can switch between programs at anytime.";
            } else
            {
                var Account = _connection.Table<LocalDBAccount>().Where(x => x.GuidCRM == _ContactSignUp.AccountGuid).FirstOrDefaultAsync().Result;
                if (Account != null)
                {
                    var Program = _connection.Table<LocalDBProgram>().Where(x => x.GuidCRM == _ContactSignUp.ProgramGuid).FirstOrDefaultAsync().Result;
                    if(Program != null)
                    {
                        ProgramMessage.Text = "You'll be setup on " + Account.Heading + "'s " + Program.Heading + " program. You can switch between programs at anytime.";
                    }
                }
            }

            if (Device.Idiom == TargetIdiom.Phone)
            {
                SignInBox.Padding = new Thickness(40, 60, 40, 60);
                //cartelleLogo.WidthRequest = 180;
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                SignInBox.Padding = new Thickness(200, 160, 200, 60);
                //cartelleLogo.WidthRequest = 240;
            }
            else
            {
                SignInBox.Padding = new Thickness(40, 60, 40, 60);
                //cartelleLogo.WidthRequest = 180;
            }

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                // handle the tap
                TermsBtn_Clicked();
            };
            tapGestureRecognizer.NumberOfTapsRequired = 1;
            TermsSL.GestureRecognizers.Add(tapGestureRecognizer);

            var tapGestureRecognizer2 = new TapGestureRecognizer();
            tapGestureRecognizer2.Tapped += (s, e) =>
            {
                // handle the tap
                PrivacyBtn_Clicked();
            };
            tapGestureRecognizer2.NumberOfTapsRequired = 1;
            PoliciesSL.GestureRecognizers.Add(tapGestureRecognizer2);
        }

        private async void CreateAccountBtn(object sender, EventArgs e)
        {
            if (_IsCreatingAccount)
                return;

            DisableForm();

            var formValidation = AreFormFieldsValid();
            if (!formValidation.Item1)
            {
                //Error
                FormValidationSpecialMessage.Text = formValidation.Item2;
                EnableForm();
                return;
            }

            if (!IsInternetConnected())
            {
                FormValidationSpecialMessage.Text = "";
                await DisplayAlert("No Internet", "Please connect to the internet and try again.", "OK");
                EnableForm();
                return;
            }

            FormValidationSpecialMessage.Text = "";

            try
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Create Account Attempt" } });
                JoinNowBtn.Text = "SENDING...";

                //POST - Create a new Contact via API and wait to hear success back which Set Contact Created In App Field to Yes and Send welcome Email
                _ContactSignUp.FirstName = FirstName.Text;
                _ContactSignUp.Email = Email.Text.ToLower();
                _ContactSignUp.Password = Password.Text;

                _ContactSignUp.DeviceOS = 866660001;//Apple iOS
                _ContactSignUp.AppVersion = App.AppVersion;
                _ContactSignUp.DeviceType = DeviceInfo.Model;

                if (Device.Idiom == TargetIdiom.Phone)
                {
                    _ContactSignUp.PlatformSource = 866660001;
                }
                else if (Device.Idiom == TargetIdiom.Tablet)
                {
                    _ContactSignUp.PlatformSource = 866660003;
                }

                //upload
                _client.DefaultRequestHeaders.Clear();
                string json = JsonConvert.SerializeObject(_ContactSignUp);
                var contentString = new StringContent(json, Encoding.UTF8, "application/json");
                var newUri = new Uri(App._absoluteUri, App._contactsUri);
                _client.DefaultRequestHeaders.Add("Authorization", GeneralAuth.Token);

                Task<HttpResponseMessage> t1 = Task.Run(async () => { return await _client.PostAsync(newUri, contentString); });
                await Task.Delay(500);
                JoinNowBtn.Text = "RECEIVED...";
                await Task.Delay(500);
                JoinNowBtn.Text = "PROCESSING...";
                await Task.Delay(500);
                //HttpResponseMessage response = await _client.PostAsync(newUri, contentString);
                HttpResponseMessage response = t1.Result;
                //the response should tell us about the error details

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Spinner.IsVisible = false;
                    JoinNowBtn.Text = "LOADING...";         

                    HttpContent content = response.Content;
                    var json2 = await content.ReadAsStringAsync();
                    var signInResponse = JsonConvert.DeserializeObject<SignInResponseV5>(json2);
                    await WebAPIService.SaveInitialLoadData(signInResponse);

                    Task tSub = Task.Run(() => WebAPIService.UpdateLocalIAPs());
                    Auth.SaveCredentials(Email.Text.ToLower(), Password.Text);
                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Create Account Success" } });

                    //DependencyService.Get<IFacebookEvent>().CompletedRegistration();
                    await tSub;

                    //if (signInResponse.Profile.RP != 0)
                    //{
                    //    DependencyService.Get<IFacebookEvent>().SendRevenuePool(signInResponse.Profile.RP);
                    //}

                    JoinNowBtn.Text = "SETTING YOU UP...";
                    await Task.Delay(50);
                    App.Current.MainPage = new MainStartingPage("register");
                    return;
                }
                else
                {
                    HttpContent content = response.Content;
                    var json2 = await content.ReadAsStringAsync();
                    var createAccountResponse = JsonConvert.DeserializeObject<ApiResponseMessage>(json2);

                    Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Create Account Error" } });
                    FormValidationSpecialMessage.Text = createAccountResponse.Message;
                    EnableForm();
                    return;

                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "CreateAccountBtn" } });
                await DisplayAlert("Error", "Something went wrong.", "OK");
                EnableForm();
                return;
            }
        }

        private async void Close_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void Entry_Changed(object sender, EventArgs e)
        {
            try
            {
                if (FirstName.Text.Length >= 1 && Email.Text.Length >= 7 && Password.Text.Length >= 5)
                {
                    JoinNowBtn.BackgroundColor = Color.FromHex("#00BBCB");
                    JoinNowBtn.Opacity = 1;
                    JoinNowBtn.IsEnabled = true;
                }
                else
                {
                    JoinNowBtn.BackgroundColor = Color.FromHex("#4F6572");
                    JoinNowBtn.Opacity = 0.5;
                    JoinNowBtn.IsEnabled = false;
                }
            }
            catch(Exception ex)
            {
                var err = ex.ToString();
            }

        }
        
        public class CreateAccountResponse
        {
            public Guid CRMGuid { get; set; }
            public bool ConfirmedEmail { get; set; }
            public int RP { get; set; }
            public List<OptAccountV2> Athletes { get; set; }

            public CreateAccountResponse()
            {
                Athletes = new List<OptAccountV2>();
            }
        }

        public class ApiResponseMessage
        {
            public string Message { get; set; }
        }

        private void EnableForm()
        {
            JoinNowBtn.Text = "CREATE ACCOUNT";
            JoinNowBtn.IsEnabled = true;
            FirstName.IsEnabled = true;
            //LastName.IsEnabled = true;
            Email.IsEnabled = true;
            Password.IsEnabled = true;
            Spinner.IsVisible = false;
            _IsCreatingAccount = false;
        }

        private void DisableForm()
        {
            _IsCreatingAccount = true;
            Spinner.IsVisible = true;
            JoinNowBtn.IsEnabled = false;
            FirstName.IsEnabled = false;
            // LastName.IsEnabled = false;
            Email.IsEnabled = false;
            Password.IsEnabled = false;
            FormValidationSpecialMessage.Text = "";
        }

        private Tuple<bool, string> AreFormFieldsValid()
        {
            string specialErrorMessage = "";
            bool IsValide = true;

            Regex nameReg = new Regex(@"^[a-zA-Z\s]*$", RegexOptions.IgnoreCase);
            Regex emailReg = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);
            //one lower case, one upper case, one number, 8-15 characters, if you want one special character ^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$
            //Regex passwordReg = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$");
            Regex passwordReg = new Regex(@"^.{5,30}$");

            if (FirstName.Text == null || FirstName.Text == "")
            {
                FirstNameError.BackgroundColor = Color.Red;//Color.FromHex("#ffb2b2");
                IsValide = false;
            }
            else if (!nameReg.Match(FirstName.Text).Success)
            {
                FirstNameError.BackgroundColor = Color.Red;// Color.FromHex("#ffb2b2");
                specialErrorMessage = "Your name doesn't look real. ";
                IsValide = false;
            }
            else
            {
                FirstNameError.BackgroundColor = Color.Transparent;
            }

            //if (LastName.Text == null || LastName.Text == "")
            //{
            //    LastName.BackgroundColor = Color.FromHex("#ffb2b2");
            //    IsValide = false;
            //}
            //else if (!nameReg.Match(LastName.Text).Success)
            //{
            //    if (!specialErrorMessage.Contains("name"))
            //    {
            //        specialErrorMessage = "Your name doesn't look real. ";
            //    }
            //    LastName.BackgroundColor = Color.FromHex("#ffb2b2");
            //    IsValide = false;
            //}
            //else
            //{
            //    LastName.BackgroundColor = Color.FromHex("#99ccff");
            //}

            if (Email.Text == null || Email.Text == "")
            {
                EmailError.BackgroundColor = Color.Red;//.FromHex("#ffb2b2");
                IsValide = false;
            }
            else if (!emailReg.Match(Email.Text).Success)
            {
                EmailError.BackgroundColor = Color.Red;//.FromHex("#ffb2b2");
                specialErrorMessage += "Email doesn't look real. ";
                IsValide = false;
            }
            else
            {
                EmailError.BackgroundColor = Color.Transparent;
            }

            if (Password.Text == null || Password.Text == "")
            {
                PasswordError.BackgroundColor = Color.Red;//.FromHex("#ffb2b2");
                IsValide = false;
            }
            else if (!passwordReg.Match(Password.Text).Success)
            {
                PasswordError.BackgroundColor = Color.Red;//.FromHex("#ffb2b2");
                //specialErrorMessage += "Password must contain 1 lowercase, uppercase, number and be 8-15 characters.";
                specialErrorMessage += "Password must be 5-30 characters.";
                IsValide = false;
            }
            else
            {
                PasswordError.BackgroundColor = Color.Transparent;
            }
            return new Tuple<bool, string>(IsValide, specialErrorMessage);
        }


        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "OnAppearing" } });
            //DependencyService.Get<IFacebookEvent>().CompletedOnBoardingProcess();
            MessagingCenter.Send(this, "OnSignUp");
            base.OnAppearing();
        }

        private async void TermsBtn_Clicked()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Terms" } });

            if (IsInternetConnected())
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Opening Terms Page" } });
                Device.OpenUri(new Uri("https://thecartelle.com/home/terms"));
            }
            else
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
            }
        }

        private async void PrivacyBtn_Clicked()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Privacy" } });

            if (IsInternetConnected())
            {
                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Action", "Opening Privacy Page" } });
                Device.OpenUri(new Uri("https://thecartelle.com/home/privacy"));
            }
            else
            {
                await DisplayAlert("No Internet", "Connect to the internet and try again", "OK");
            }
        }


    }
}
