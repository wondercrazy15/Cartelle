using PCLStorage;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Xamarin.Forms;

[assembly: Dependency(typeof(Stance.iOS.DownloadTest))]
namespace Stance.iOS
{
    public class DownloadTest : IDownload
    {
        public void btnDownload_Click()
        {
            //Method 2 - works
            String fileLocation = "http://vjs.zencdn.net/v/oceans.mp4";
            //String fileLocation = "http://stanceathletes.com/images/api/1_Account/1_Program/1_Workout/3_Exercise/jumping_jacks_mini_loop.mp4";
            var fileName = "oceans3.mp4";

            var erwt = DependencyService.Get<IFileSystemCustom>();
            var path = erwt.GetRootFolder(fileName);
            String diskLocation = path;

            WebClient webClient = new WebClient();
            webClient.OpenRead(fileLocation);
            Int64 bytes_total = Convert.ToInt64(webClient.ResponseHeaders["Content-Length"]);

            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(new Uri(fileLocation), diskLocation);
        }

        private static void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var progress = e.ProgressPercentage;
            //Console.WriteLine("Progress: " + progress);
        }

        private static void Completed(object sender, AsyncCompletedEventArgs e)
        {
            //Console.WriteLine("Download completed!");
        }


    }
}