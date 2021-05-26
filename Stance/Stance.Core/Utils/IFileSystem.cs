using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Utils
{
    public interface IFileSystemCustom
    {
        Task WriteTextAsync(string fileName, string text);
        // Task WriteBytesAsync(string fileName, byte[] byteArray);
        string GetRootFolder(string fileName);

    }
}
