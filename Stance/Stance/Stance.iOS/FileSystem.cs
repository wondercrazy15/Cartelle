using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Stance.Utils;
using Xamarin.Forms;
using System.IO;
using System.Net;

[assembly: Dependency(typeof(Stance.iOS.FileSystem))]

namespace Stance.iOS
{
    public class FileSystem : IFileSystemCustom
    {

        public string GetRootFolder(string fileName)
        {
            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);//Environment.SpecialFolder.MyDocuments
            var path = Path.Combine(docsPath, fileName);

            return path;

        }

        public async Task WriteTextAsync(string fileName, string text)
        {
            var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);//Environment.SpecialFolder.MyDocuments
            var path = Path.Combine(docsPath, fileName);
            using(var writer = File.CreateText(path))
            {
                await writer.WriteAsync(text);
            }

        }
    }
}