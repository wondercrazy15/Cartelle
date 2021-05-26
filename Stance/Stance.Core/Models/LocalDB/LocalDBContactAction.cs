using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.Base;
using Stance.Models.LocalDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.LocalDB
{
    public class LocalDBContactAction : ContactActionBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBContactProgramDayV2))]
        public int ContactProgramDayId { get; set; }

        [ForeignKey(typeof(LocalDBAction))]
        public int ActionId { get; set; }

        [ForeignKey(typeof(LocalDBActionContentV2))]
        public int ActionContentId { get; set; }

        public bool IsComplete { get; set; }
        public DateTime? CompletedOn { get; set; }
    }
}
