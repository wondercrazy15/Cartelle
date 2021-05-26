using PCLStorage;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class DownloadPage : ContentPage
    {
        const string _video1 = "https://res.cloudinary.com/stance/video/upload/v1485929725/720p_2mbps.mp4";

        const string _video2 = "http://vjs.zencdn.net/v/oceans.mp4";

        Slider slider = new Slider
        {
            Value = 0,
            Minimum = 0,
            Maximum = 100,
        };

        public DownloadPage()
        {
            InitializeComponent();

            var btn = new Button
            {
                Text = "Download",
            };

            btn.Clicked += (s, e) => btnClicked();



            var sl = new StackLayout
            {

            };

            sl.Children.Add(btn);
            sl.Children.Add(slider);


            Content = sl;

        }

        private void btnClicked()
        {
            this.DownloadValueChanged += OnDowloadProgressChange;
            var ts = new Task(ReadVideoAsyncTwo);
            ts.Start();
            ts.Wait();

        }

        private void OnDowloadProgressChange(object sender, int progress)
        {
            slider.Value = progress;
        }

        public event EventHandler<int> DownloadValueChanged;

        private async void ReadVideoAsyncTwo()
        {
            int offset = 0;
            long? streamLength = 0;
            var result = new List<byte>();
            var responseBuffer = new byte[1500];

            using (HttpClient client = new HttpClient())
            {
                const string url = "http://vjs.zencdn.net/v/oceans.mp4";
                using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    streamLength = response.Content.Headers.ContentLength;
                    var progress = 0;

                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {


                        int read;

                        do
                        {
                            read = await streamToReadFrom.ReadAsync(responseBuffer, offset, responseBuffer.Length);
                            result.AddRange(responseBuffer);
                            offset += read;

                            if (streamLength != null && streamLength != 0)
                            {
                                progress = (offset / (int)streamLength) * 100;

                                Xamarin.Forms.Device.BeginInvokeOnMainThread(
                                    delegate
                                    {
                                        slider.Value = (double)progress;
                                    }

                                    );

                                //await Task.Delay(50);

                            }

                            //MessagingCenter.Send(this, "DownloadValueChanged", progress);
                            // here I want to send percents of downloaded data
                            // offset / (totalSize / 100)

                        } while (read != 0);

                        IFolder rootFolder = FileSystem.Current.LocalStorage;
                        IFile file = await rootFolder.CreateFileAsync("oceans.mp4", CreationCollisionOption.ReplaceExisting);

                        var res = result.ToArray();

                        using (Stream streamToWriteTo = await file.OpenAsync(FileAccess.ReadAndWrite))
                        {
                            for (int i = 0; i < res.Length; i++)
                            {
                                streamToWriteTo.WriteByte(res[i]);
                            }

                            streamToWriteTo.Seek(0, SeekOrigin.Begin);

                            var _file = file;

                        }
                    }
                }
            }
        }


        private void btnClicked2()
        {
            // MessagingCenter.Subscribe<DownloadResources, int>(this, "DownloadValueChanged", OnDowloadProgressChange);
            //download and store videos in file system
            var it = new DownloadResources();
            it.InitiateVideoDownload();

            // MessagingCenter.Unsubscribe<DownloadPage>(this, "DownloadValueChanged");
        }




    }
}
