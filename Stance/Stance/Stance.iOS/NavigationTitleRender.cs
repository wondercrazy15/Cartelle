
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationTitleRender))]

namespace Stance.iOS
{
    public class NavigationTitleRender : NavigationRenderer
    {

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (this.NavigationBar == null) return;

            //SetNavBarStyle();
            //SetNavBarTitle();
            SetNavBarItems();
        }

        private void SetNavBarStyle()
        {
            NavigationBar.ShadowImage = new UIImage();
            NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            UINavigationBar.Appearance.ShadowImage = new UIImage();
            UINavigationBar.Appearance.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
        }

        private void SetNavBarItems()
        {
            var navPage = this.Element as NavigationPage;

            if (navPage == null) return;

            var textAttributes = new UITextAttributes()
            {
                Font = UIFont.FromName("AvenirNextCondensed-Medium", 16),
                TextColor = Color.White.ToUIColor(),                
            };

            var textAttributesHighlighted = new UITextAttributes()
            {
                TextColor = Color.White.ToUIColor(),
                Font = UIFont.FromName("AvenirNextCondensed-Medium", 16)
            };

            UIBarButtonItem.Appearance.SetTitleTextAttributes(textAttributes,
                UIControlState.Normal);
            UIBarButtonItem.Appearance.SetTitleTextAttributes(textAttributesHighlighted,
                UIControlState.Highlighted);
        }

        private void SetNavBarTitle()
        {
            var navPage = this.Element as NavigationPage;

            if (navPage == null) return;

            //this.NavigationBar.TitleTextAttributes = new UIStringAttributes
            //{
            //    Font = UIFont.FromName("AvenirNextCondensed-Medium", 16),
            //    UnderlineColor = Color.White.ToUIColor(),

            //};

            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes
            {
                TextColor = UIColor.White,
                Font = UIFont.FromName("AvenirNextCondensed-Medium", 16)
            });
        }


    }
}