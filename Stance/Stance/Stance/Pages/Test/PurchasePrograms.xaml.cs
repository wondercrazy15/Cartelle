using Plugin.Connectivity;
using Stance.Models.API;
using Stance.Models.Base;
using Stance.Models.LocalDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Plugin.Connectivity.Abstractions;

namespace Stance.Pages.Test
{
    public partial class PurchasePrograms : ContentPage
    {
        private const string _PageName = "purchase programs";


        public PurchasePrograms()
        {
            InitializeComponent();

            //Get a list of all published programs in CRM and add to a list, click BTN to Purchase, confirmation box or say that they have already purchased it
            //List of purchased programs, click a program and go to the next page
            //use fake api data at first, and put into same list OR Data in the constructor

            //Add a PURCHASE BTN on the Programs that are not Downloaded


            List<ProgramBase> listOfPrograms = new List<ProgramBase>();
            //use polymorphisum to combine LocalDbProgram and Program
            //listOfPrograms.Add(new LocalDBProgram());
            listOfPrograms.Add(new APIProgram());

            foreach(var item in listOfPrograms)
            {
                
            }

            var api = new APIProgram();
            var local = new LocalDBProgram();


            //page indicator to show downloading in progress
            this.IsBusy = true;

            try
            {

            }catch(Exception ex)
            {

            }
            finally
            {
                this.IsBusy = false;
            }

            //Test is connected to the internet
            if (CrossConnectivity.Current.IsConnected)
            {

            }

            //notified when connectivity changes
            CrossConnectivity.Current.ConnectivityChanged += OnConnectivityChanged;

            //Look at all availbale connections
            IEnumerable<ConnectionType> connectionTypes;
            connectionTypes = CrossConnectivity.Current.ConnectionTypes;

            foreach(var connection in connectionTypes)
            {

                switch (connection)
                {
                    case ConnectionType.Cellular:
                        break;
                    case ConnectionType.Desktop:
                        break;
                    case ConnectionType.WiFi:
                        break;
                    case ConnectionType.Wimax:
                        break;
                    case ConnectionType.Other:
                        break;
                }

            }


        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
            {

            }
        }
    }
}
