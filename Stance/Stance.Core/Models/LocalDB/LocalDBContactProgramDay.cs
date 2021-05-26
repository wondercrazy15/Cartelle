using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.Base;
using System;

namespace Stance.Models.LocalDB
{
    public class LocalDBContactProgramDay : ContactProgramDayBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBProgramDay))]
        public int ProgramDayId { get; set; }

        [ForeignKey(typeof(LocalDBContactProgram))]
        public int ContactProgramId { get; set; }

        public bool IsComplete { get; set; }
        public bool IsDownloaded { get; set; }
        public DateTime? DownloadedOn { get; set; }
        public DateTime? ReceivedOn { get; set; }

    }

    public class LocalDBContactProgramDayV2 : ContactProgramDayBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get;
            set; }


        [ForeignKey(typeof(LocalDBProgramDay))]
        public int ProgramDayId { get; set; }

        [ForeignKey(typeof(LocalDBContactProgram))]
        public int ContactProgramId { get; set; }

        public bool IsComplete { get; set; }
        public bool IsDownloaded { get; set; }
        public DateTime? DownloadedOn { get; set; }
        public DateTime? ReceivedOn { get; set; }

        public bool IsRepeat { get; set; }
        public bool IsScheduleDeviation { get; set; }

        [ForeignKey(typeof(LocalDBContactProgramDayV2))]
        public int ContactProgramDayParentId { get; set; }

    }
}
