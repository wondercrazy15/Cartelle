using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.API;
using Stance.Models.LocalDB;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Stance.Pages.Sub
{
    public partial class DBTestPage : ContentPage
    {

        //public class Testing
        //{
        //    [PrimaryKey, AutoIncrement]
        //    public int Id { get; set; }

        //    public string Text { get; set; }

        //    public string Name { get; set; }

        //    [ForeignKey(typeof(TestingParent))]
        //    public int ParentId { get; set; }

        //}

        //public class TestingParent
        //{
        //    [PrimaryKey, AutoIncrement]
        //    public int Id { get; set; }

        //    public string Text { get; set; }

        //    public string Name { get; set; }   
            
        //}

        //public class TestingReference
        //{
        //    [PrimaryKey, AutoIncrement]
        //    public int Id { get; set; }

        //    public string Text { get; set; }

        //    public string Name { get; set; }

        //}


        public DBTestPage()
        {
            InitializeComponent();

            Task.Factory.StartNew(() => speak("hello")).Wait();

            Task<int> t = Task.Factory.StartNew(() => Add(1, 3));

            int value = t.Result;//4
        }


        private static void speak(string s)
        {

        }

        private static int Add (int x, int y)
        {
            return x + y;
        }
             


        private bool TimerElapsed()
        {
            if (_TimerSeconds >= 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    //put here your code which updates the view
                    TimerLabel.Text = _TimerSeconds.ToString();
                    _TimerSeconds--;
                });
                return true;
            } else
            {
                return false;
            }
            //return true to keep timer reccuring
            //return false to stop timer
        }

        private int _TimerSeconds = 10;

        private async void TestingC()
        {
            TimerLabel.Text = _TimerSeconds.ToString();

            Device.StartTimer(TimeSpan.FromSeconds(1), TimerElapsed);

            await Task.Delay(2);
            TimerLabel.Text = "DONE";


            //var t = new List<APIContactAction>();

            //var ca = new APIContactAction
            //{
            //    GuidCRM = Guid.Empty,
            //};

            //t.Add(ca);

            //var indx = t.ElementAt(0);

            //var gd = indx.GuidCRM;

            //var _connection = DependencyService.Get<ISQLiteDb>().GetConnection();


            //await _connection.CreateTableAsync<LocalDBContactProgramDay>();

            //var ContactProgramDay = await _connection.Table<LocalDBContactProgramDay>().Where(x => x.GuidCRM == indx.GuidCRM).FirstOrDefaultAsync();

            //var cpd = new LocalDBContactProgramDay
            //{
            //    GuidCRM = Guid.Empty,
            //};

            //await _connection.InsertAsync(cpd);

            //var result = await _connection.Table<LocalDBContactProgramDay>().ToListAsync();

            //var i = 0;
            //await connection.CreateTableAsync<LocalDBContactProgram>();
            //await connection.CreateTableAsync<Testing>();

            //var p = new TestingParent
            //{
            //    Text = "PAPA",
            //    Name = "DAD"
            //};

            //var contactAction = new LocalDBContactProgram
            //{
            //    GuidCRM = Guid.Empty,
            //    StartDate = DateTime.UtcNow,
            //};

            //await connection.InsertAsync(contactAction);

            //var g = new List<Testing>
            //{
            //    new Testing { Text="Balls", Name="mine", ParentId = p.Id },
            //    new Testing { Text = "Number 2", Name="mine", ParentId = p.Id },
            //    new Testing { Text = "three", Name="mine", ParentId = p.Id }
            // };

            //await connection.InsertAllAsync(g);

            //var result = await connection.Table<LocalDBContactProgram>().ToListAsync();

            //listView.ItemsSource = result.Select(x => x.StartDate);
        }





    }
}
