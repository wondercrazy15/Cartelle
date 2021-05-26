using System;
using Octane.Xamarin.Forms.VideoPlayer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Stance.MarkupExtensions
{
    [ContentProperty("Resource")]
    public class VideoResourceExtension : IMarkupExtension
    {
        public string Resource { get; set; }
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Resource == null)
                return null;

            var videoSource = VideoSource.FromResource(Resource);

            return videoSource;
        }
    }
}
