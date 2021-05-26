using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.API;
using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.LocalDB
{
    public class LocalDBContactProgram : ContactProgramBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBProgram))]
        public int ProgramId { get; set; }

        public bool IsComplete { get; set; }        

    }
}
