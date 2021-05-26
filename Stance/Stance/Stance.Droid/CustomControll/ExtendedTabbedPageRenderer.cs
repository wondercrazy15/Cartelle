using System;
using Stance.Droid.CustomControll;
using Xamarin.Forms;
using Stance.Models;
using Xamarin.Forms.Platform.Android.AppCompat;
using Android.Support.Design.Widget;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Support.Design.Internal;
using Android.Widget;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Content;

[assembly: ExportRenderer(typeof(ExtendedTabbedPage), typeof(ExtendedTabbedPageRenderer))]
namespace Stance.Droid.CustomControll
{
    public class ExtendedTabbedPageRenderer : TabbedPageRenderer
    {
        Xamarin.Forms.TabbedPage tabbedPage;
        BottomNavigationView bottomNavigationView;
        Android.Views.IMenuItem lastItemSelected;
        private bool firstTime = true;
        private bool firstTimeTab = true;
        int lastItemId = -1;
        public ExtendedTabbedPageRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TabbedPage> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {

                tabbedPage = e.NewElement as ExtendedTabbedPage;
                tabbedPage.BackgroundColor=Xamarin.Forms.Color.White;
                tabbedPage.BarTextColor=Xamarin.Forms.Color.LightGray;
                tabbedPage.UnselectedTabColor=Xamarin.Forms.Color.LightGray;
                
                bottomNavigationView = (GetChildAt(0) as Android.Widget.RelativeLayout).GetChildAt(1) as BottomNavigationView;
                bottomNavigationView.NavigationItemSelected += BottomNavigationView_NavigationItemSelected;

                //Call to remove animation
                //SetShiftMode(bottomNavigationView, false, false);

                //Call to change the font
                //ChangeFont();
                var bottomNavMenuView = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;
                SetTabItemTextColor(bottomNavMenuView.GetChildAt(0) as BottomNavigationItemView, Android.Graphics.Color.ParseColor("#3E8DB2"));
               

            }

            if (e.OldElement != null)
            {
                bottomNavigationView.NavigationItemSelected -= BottomNavigationView_NavigationItemSelected;
            }

        }

        
        //Remove tint color
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (bottomNavigationView != null)
            {
                bottomNavigationView.ItemIconTintList = null;
            }

            if (firstTime && bottomNavigationView != null)
            {
                for (int i = 0; i < Element.Children.Count; i++)
                {
                    var item = bottomNavigationView.Menu.GetItem(i);

                    if (bottomNavigationView.SelectedItemId == item.ItemId)
                    {
                        item.Icon.SetColorFilter(Android.Graphics.Color.ParseColor("#3E8DB2"), PorterDuff.Mode.SrcIn);
                        
                        // lastItemSelected.Icon.SetColorFilter(Android.Graphics.Color.ParseColor("#3E8DB2"), PorterDuff.Mode.SrcIn);

                        //BottomNavigationView_NavigationItemSelected(default(Object), default(BottomNavigationView.NavigationItemSelectedEventArgs));
                        // SetTabItemTextColor(bottomNavigationView, Android.Graphics.Color.ParseColor("#3E8DB2"));
                        // bottomNavigationView.ItemIconTintList[1] .Item.Icon.SetColorFilter(selectedColor, PorterDuff.Mode.SrcIn);
                        //SetupBottomNavigationView(item);
                        break;
                    }
                }
                firstTime = false;
            }

        }

        void BottomNavigationView_NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            var bottomNavMenuView = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;
            var normalColor = Android.Graphics.Color.ParseColor("#D3D3D3"); ;//tabbedPage.On<Xamarin.Forms.PlatformConfiguration.Android>().GetBarItemColor().ToAndroid();
            var selectedColor = Android.Graphics.Color.ParseColor("#3E8DB2");// tabbedPage.On<Xamarin.Forms.PlatformConfiguration.Android>().GetBarSelectedItemColor().ToAndroid();

            if (firstTimeTab && bottomNavigationView != null)
            {
                for (int i = 0; i < Element.Children.Count; i++)
                {
                    var item = bottomNavigationView.Menu.GetItem(i);

                    if (true)
                    {
                        item.Icon.SetColorFilter(normalColor, PorterDuff.Mode.SrcIn);
                        var bottomNavMenuViews = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;
                        SetTabItemTextColor(bottomNavMenuViews.GetChildAt(0) as BottomNavigationItemView, normalColor);
                        // lastItemSelected.Icon.SetColorFilter(Android.Graphics.Color.ParseColor("#3E8DB2"), PorterDuff.Mode.SrcIn);

                        //BottomNavigationView_NavigationItemSelected(default(Object), default(BottomNavigationView.NavigationItemSelectedEventArgs));
                        // SetTabItemTextColor(bottomNavigationView, Android.Graphics.Color.ParseColor("#3E8DB2"));
                        // bottomNavigationView.ItemIconTintList[1] .Item.Icon.SetColorFilter(selectedColor, PorterDuff.Mode.SrcIn);
                        //SetupBottomNavigationView(item);
                        break;
                    }
                }
                firstTimeTab = false;
            }

           
            
            if (lastItemSelected != null)
            {
                lastItemSelected.Icon.SetColorFilter(normalColor, PorterDuff.Mode.SrcIn);
                
            }

            if ($"{e.Item}" != "App")
            {
                e.Item.Icon.SetColorFilter(selectedColor, PorterDuff.Mode.SrcIn);
                lastItemSelected = e.Item;
            }

            if (lastItemId != -1)
            {
                SetTabItemTextColor(bottomNavMenuView.GetChildAt(lastItemId) as BottomNavigationItemView, normalColor);
            }

            SetTabItemTextColor(bottomNavMenuView.GetChildAt(e.Item.ItemId) as BottomNavigationItemView, selectedColor);


            //SetupBottomNavigationView(e.Item);
            this.OnNavigationItemSelected(e.Item);

            lastItemId = e.Item.ItemId;

        }


        void SetTabItemTextColor(BottomNavigationItemView bottomNavigationItemView, Android.Graphics.Color textColor)
        {
            var itemTitle = bottomNavigationItemView.GetChildAt(1);
            var smallTextView = ((TextView)((BaselineLayout)itemTitle).GetChildAt(0));
            var largeTextView = ((TextView)((BaselineLayout)itemTitle).GetChildAt(1));

            smallTextView.SetTextColor(textColor);
            largeTextView.SetTextColor(textColor);
        }


        //Adding line view
        void SetupBottomNavigationView(IMenuItem item)
        {
            int lineBottomOffset = 8;
            int lineWidth = 4;
            int itemHeight = bottomNavigationView.Height - lineBottomOffset;
            int itemWidth = (bottomNavigationView.Width / Element.Children.Count);
            int leftOffset = item.ItemId * itemWidth;
            int rightOffset = itemWidth * (Element.Children.Count - (item.ItemId + 1));
            GradientDrawable bottomLine = new GradientDrawable();
           
            bottomLine.SetStroke(lineWidth, Xamarin.Forms.Color.DarkGray.ToAndroid());

            var layerDrawable = new LayerDrawable(new Drawable[] { bottomLine });
            layerDrawable.SetLayerInset(0, leftOffset, itemHeight, rightOffset, 0);

            bottomNavigationView.SetBackground(layerDrawable);
        }


        //Remove animation
        public void SetShiftMode(BottomNavigationView bottomNavigationView, bool enableShiftMode, bool enableItemShiftMode)
        {
            try
            {
                var menuView = bottomNavigationView.GetChildAt(0) as BottomNavigationMenuView;
               
                if (menuView == null)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to find BottomNavigationMenuView");
                    return;
                }
               
                var shiftMode = menuView.Class.GetDeclaredField("mShiftingMode");
                shiftMode.Accessible = true;
                shiftMode.SetBoolean(menuView, enableShiftMode);
                shiftMode.Accessible = false;
                shiftMode.Dispose();
                for (int i = 0; i < menuView.ChildCount; i++)
                {
                    var item = menuView.GetChildAt(i) as BottomNavigationItemView;
                    if (item == null)
                        continue;
                    item.SetShifting(false);
                    item.SetChecked(item.ItemData.IsChecked);
                }
                menuView.UpdateMenuView();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to set shift mode: {ex}");
            }
        }
    }
}