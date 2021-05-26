using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.LocalDB
{
    public class LocalDBProgram : ProgramBase
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBAccount))]
        public int AccountId { get; set; }

        public string PhotoFilePath { get; set; }
        public string VideoFilePath { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //private string _name;

        //public string Name
        //{
        //    get { return _name; }
        //    set
        //    {
        //        if(_name == value)
        //        {
        //            return;
        //        } else
        //        {
        //            _name = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));            
        //}



    }
}
