using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Stance.iOS;
using UIKit;
using Stance.Utils;
using Stance.Pages.Main;
using Stance.Pages.Sub;

[assembly: ExportRenderer(typeof(Athletes_MainPage), typeof(CustomContentPageRenderer))]
[assembly: ExportRenderer(typeof(Workout_MainPage), typeof(CustomContentPageRenderer))]
[assembly: ExportRenderer(typeof(Progress_MainPage), typeof(CustomContentPageRenderer))]
[assembly: ExportRenderer(typeof(ProgramSearch), typeof(CustomContentPageRenderer))]

namespace Stance.iOS
{

    public class CustomContentPageRenderer : PageRenderer
    {
        public new ContentPage Element
        {
            get { return (ContentPage)base.Element; }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var LeftNavList = new List<UIBarButtonItem>();
            var rightNavList = new List<UIBarButtonItem>();

            UINavigationItem navigationItem = null;

            try
            {
                navigationItem = this.NavigationController.TopViewController.NavigationItem;
            }
            catch (Exception ex)
            {
                return;
            }

            var navPrimaryItemCount = Element.ToolbarItems.Where(c => c.Order == ToolbarItemOrder.Primary).Count();

            if (Element.ToolbarItems.Count() > 0)
            {
                if (navigationItem != null)
                {

                    if (navigationItem.LeftBarButtonItems == null)
                    {
                        for (var i = 0; i < Element.ToolbarItems.Count; i++)
                        {
                            //var reorder = (Element.ToolbarItems.Count - 1);
                            //var ItemPriority = Element.ToolbarItems[reorder - i].Priority;
                            var ItemPriority = Element.ToolbarItems[i].Priority;
                            var ItemOrder = Element.ToolbarItems[i].Order;

                            if (ItemOrder == ToolbarItemOrder.Primary)
                            {
                                if (ItemPriority == 1)
                                {
                                    if (i < navPrimaryItemCount)
                                    {
                                        UIBarButtonItem LeftNavItems = navigationItem.RightBarButtonItems[i];
                                        LeftNavList.Add(LeftNavItems);
                                    }
                                }
                                else if (ItemPriority == 0)
                                {
                                    if (i < navPrimaryItemCount)
                                    {
                                        UIBarButtonItem RightNavItems = navigationItem.RightBarButtonItems[i];
                                        rightNavList.Add(RightNavItems);
                                    }
                                }

                            }
                            //if (ItemOrder == ToolbarItemOrder.Secondary)
                            //{
                            //    Element.ToolbarItems[i].Clicked += (s, e) =>
                            //    {
                                                                       

                            //    };
                            //}
                        }
                    }


                }
            }


            if (LeftNavList.Count() > 0)
            {
                navigationItem.SetLeftBarButtonItems(LeftNavList.ToArray(), false);
            }

            if (rightNavList.Count() > 0)
            {
                navigationItem.SetRightBarButtonItems(rightNavList.ToArray(), false);
            }

        }
    }
}