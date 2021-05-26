using Xamarin.Forms;

namespace Stance.Utils
{
    public static class DeviceType
    {
        public static string GetDeviceType()
        {
            var isOfXFamily = DependencyService.Get<IDeviceInfo>().IsOfXFamily();
            if (isOfXFamily)
            {
                var isDeviceIphoneXorXS = DependencyService.Get<IDeviceInfo>().IsIphoneXorXSDevice();
                if (isDeviceIphoneXorXS)
                {
                    return "iPhone X or XS";
                }

                var isDeviceIphoneXR = DependencyService.Get<IDeviceInfo>().IsIphoneXRDevice();
                if (isDeviceIphoneXR)
                {
                    return "iPhone XR";
                }

                var isDeviceIphoneXSMax = DependencyService.Get<IDeviceInfo>().IsIphoneXSMaxDevice();
                if (isDeviceIphoneXSMax)
                {
                    return "iPhone XS Max";
                }
            }
            else if (Device.Idiom == TargetIdiom.Phone)
            {
                var isDeviceIphonePlus = DependencyService.Get<IDeviceInfo>().IsIphonePlus();
                if (isDeviceIphonePlus)
                {
                    return "iPhone Plus";
                }
                else
                {
                    return "iPhone";
                }
            }
            else if (Device.Idiom == TargetIdiom.Tablet)
            {
                var isDeviceLargeIPad = DependencyService.Get<IDeviceInfo>().IsLargerIPad();
                if (isDeviceLargeIPad)
                {
                    return "iPad 12.9-inch";
                }
                else
                {
                    return "iPad";
                }
            }
            return "";
        }

    }
}
