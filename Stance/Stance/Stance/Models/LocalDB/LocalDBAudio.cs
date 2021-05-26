using SQLite;
using SQLiteNetExtensions.Attributes;
using StanceWeb.Models.App.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.LocalDB
{
    public class LocalDBAudio : AudioBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBProgramDay))]
        public int ProgramDayId { get; set; }

        [ForeignKey(typeof(LocalDBAction))]
        public int ActionId { get; set; }

        [ForeignKey(typeof(LocalDBAudioContentV2))]
        public int AudioContentId { get; set; }

    }
}
