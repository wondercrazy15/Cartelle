using System;
using Android.Content;
using Stance.Droid.CustomControll;
using Xamarin.Forms;
using Stance.Models;
[assembly: Dependency(typeof(FileSystemImplementation))]
namespace Stance.Droid.CustomControll
{
    public class FileSystemImplementation : IFileSystem
    {
        public string GetExternalStorage()
        {
            Context context = Android.App.Application.Context;
            var filePath = context.GetExternalFilesDir("");
            return filePath.Path;
        }
    }
}