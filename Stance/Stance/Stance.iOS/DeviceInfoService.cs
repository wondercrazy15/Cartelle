using Stance.iOS;
using Stance.Utils;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceInfoService))]
namespace Stance.iOS
{
    public class DeviceInfoService : IDeviceInfo
    {
        public DeviceInfoService() { }

        public bool IsOfXFamily()
        {
            if (IsIphoneXorXSDevice())
            {
                return true;
            } else if (IsIphoneXRDevice())
            {
                return true;
            }
            else if (IsIphoneXSMaxDevice())
            {
                return true;
            }
            return false;
        }

        public bool IsIphoneXorXSDevice()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                if ((UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale) == 2436) //iphoneX
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsIphoneXSMaxDevice()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                if ((UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale) == 2688) //iphoneXS Max
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsIphoneXRDevice()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                if ((UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale) == 1792) //iphoneXR
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsIphonePlus()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                if ((UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale) == 2208)//iPhone 8,7,6s Plus
                {
                    return true;
                }
            }
            return false;
        }


        public bool IsLargerIPad()
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
            {
                if ((UIScreen.MainScreen.Bounds.Height * UIScreen.MainScreen.Scale) > 2224)//iPad 12.9"
                {
                    return true;
                }
            }
            return false;
        }
    }
}