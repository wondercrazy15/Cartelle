using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net;
using Stance.Utils;
using Xamarin.Forms;

[assembly: Dependency(typeof(Stance.Droid.DownloadTest))]
namespace Stance.Droid
{
    public class DownloadTest: IDownload
    {
        public void btnDownload_Click()
        {
            throw new NotImplementedException();
        }


    }
}