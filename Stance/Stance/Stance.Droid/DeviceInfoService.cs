using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Stance.Droid;
using Stance.Utils;
using Xamarin.Essentials;
using Xamarin.Forms;


[assembly: Dependency(typeof(DeviceInfoService))]
namespace Stance.Droid
{
    public class DeviceInfoService: IDeviceInfo
    {
        public DeviceInfoService() { }

        const int smallWightResolution = 751;
        const int smallHeightResolution = 1335;

        public bool IsOfXFamily()
        {
            if (IsIphoneXorXSDevice())
            {
                
                    return true;
            }
            else if (IsIphoneXRDevice())
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

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            // Width (in pixels)
            var width = mainDisplayInfo.Width;
            // Height (in pixels)
            var height = mainDisplayInfo.Height;
            if (width <= smallWightResolution && height <= smallHeightResolution)
            {
                return true;
            }
            else
                return false;
        }

        public bool IsIphoneXSMaxDevice()
        {
            
            return false;
        }

        public bool IsIphoneXRDevice()
        {

            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            // Width (in pixels)
            var width = mainDisplayInfo.Width;
            // Height (in pixels)
            var height = mainDisplayInfo.Height;
            if (width <= smallWightResolution && height <= smallHeightResolution)
            {
                return false;
            }
            else
                return true;
        }

        public bool IsIphonePlus()
        {
            
                return false;
            
        }


        public bool IsLargerIPad()
        {
            
            return false;
        }
    }
}