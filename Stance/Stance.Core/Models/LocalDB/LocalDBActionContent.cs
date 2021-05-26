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
    public class LocalDBActionContent : ActionContentBase
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBProgram))]
        public int ProgramId { get; set; }

        public string PhotoFilePath { get; set; }
        public string VideoFilePath { get; set; }
    }

    public class LocalDBActionContentV2 : ActionContentBase
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBProgram))]
        public int ProgramId { get; set; }

        public string PhotoFilePath { get; set; }
        public string VideoFilePath { get; set; }

        public bool IsPreview { get; set; }
        public DateTime? LastDownloadedOn { get; set; }
        public DateTime? LastDownloadAttempt { get; set; }

        [ForeignKey(typeof(LocalDBContactProgramDay))]
        public int ContactProgramDayId { get; set; } //this is for checking the download

    }
}
