using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.LocalDB
{
    public class LocalDBProgramDay : ProgramDayBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBProgram))]
        public int ProgramId { get; set; }

        public string PhotoFilePath { get; set; }
        public string VideoFilePath { get; set; }

    }
}
