using SQLite;
using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.LocalDB
{
    public class LocalDBAccount : AccountBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string PhotoFilePath { get; set; }
        public string VideoFilePath { get; set; }

        public string IGProfileUrl { get; set; }
        public string Code { get; set; }


    }
}
