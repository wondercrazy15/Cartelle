using System;
using System.Collections.Generic;
using System.Text;

namespace Stance.Utils
{
    public interface IDeviceInfo
    {
        bool IsOfXFamily();

        bool IsIphoneXorXSDevice();

        bool IsIphoneXSMaxDevice();

        bool IsIphoneXRDevice();

        bool IsIphonePlus();

        bool IsLargerIPad();
    }
}
