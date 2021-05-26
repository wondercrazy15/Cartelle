using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Stance.Models.API;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Net;

namespace Stance.Pages.Sub
{
    public partial class ContactUs : BaseContentPage
    {
        private const string _PageName = "Contact Us/Feedback";
        private int _SubjectType;
        private string _Subject;
        private bool _SendBtnActive = false;
        private Guid _ProgramDayGuid = Guid.Empty;
        private string _type = "";

        public ContactUs(string programDayGuid = null, string type = null)
        {
            InitializeComponent();

            Spinner.IsVisible = false;
            MessageSentSL.IsVisible = false;
            MessageText.HeightRequest = 300;
            _type = type;

            if (programDayGuid != null && programDayGuid != "")
            {
                _ProgramDayGuid = Guid.Parse(programDayGuid);
            }

            if (type == "contactus")
            {
                Title.Text = "CONTACT US";
                ContactUsText.IsVisible = true;
                _SubjectType = 866660000;
            }
            else if(type == "feedback")
            {
                Title.Text = "FEEDBACK";
                SubjectInput.IsVisible = false;
                _SubjectType = 866660001;
                _Subject = "FEEDBACK";
            }
            else if(type == "workoutfeedback")
            {
                Title.Text = "WORKOUT FEEDBACK";
                SubjectInput.IsVisible = false;
                _SubjectType = 866660002;
                _Subject = "WORKOUT FEEDBACK";
            }
        }

        private async void Submit_Clicked(object sender, EventArgs e)
        {
            if (!_SendBtnActive)
            {
                _SendBtnActive = true;
                Spinner.IsVisible = true;
                FormValidationSpecialMessage.Text = "";
                SubjectInput.IsEnabled = false;
                MessageText.IsEnabled = false;

                if (MessageText.Text == null || MessageText.Text == "")
                {
                    FormValidationSpecialMessage.Text = "All fields are required";
                    UnlockForm();
                } else if (_SubjectType == 866660000 && (SubjectInput.Text == null || SubjectInput.Text == "") )
                {
                    FormValidationSpecialMessage.Text = "All fields are required";
                    UnlockForm();
                }
                else
                {
                    if (IsInternetConnected())
                    {
                        try
                        {
                            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Type", _type }, { "Action", "Sending"  } });
                            Submit.Text = "SENDING...";

                            var Message = new APIMessage
                            {
                                TypeCode = _SubjectType,
                                Message = MessageText.Text,
                                ProgramDayGuid = _ProgramDayGuid,
                            };

                            if(_SubjectType == 866660000)
                            {
                                Message.Subject = SubjectInput.Text;
                            } else
                            {
                                Message.Subject = _Subject;
                            }

                            var response = await WebAPIService.SendContactUsMessage(_client, Message);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Type", _type }, { "Action", "Sent" } });
                                FormSL.IsVisible = false;
                                MessageSentSL.IsVisible = true;
                            } else
                            {
                                Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Type", _type }, { "Action", "Error Sending" } });
                                await DisplayAlert("Error", "Something went wrong, try again later.", "OK");
                                UnlockForm();
                            }

                        } catch(Exception ex)
                        {
                            await DisplayAlert("Error", "Something went wrong.", "OK");
                            Crashes.TrackError(ex, new Dictionary<string, string>() { { "Page", _PageName }, { "Function", "Submit_Clicked()" } });
                            UnlockForm();
                        }

                    } else
                    {
                        await DisplayAlert("No Internet", "Please connect to the internet and try again.", "OK");
                        UnlockForm();
                    }
                }

                _SendBtnActive = false;
            }           


        }

        private void UnlockForm()
        {
            Submit.Text = "SEND";
            SubjectInput.IsEnabled = true;
            MessageText.IsEnabled = true;
            Spinner.IsVisible = false;
        }

        protected override void OnAppearing()
        {
            Analytics.TrackEvent(_PageName, new Dictionary<string, string>() { { "Type", _type }, { "Action", "OnAppearing" } });
            base.OnAppearing();
        }

    }
}
