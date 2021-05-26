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
    public class LocalDBAudioContent : AudioContentBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string AudioFilePath { get; set; }
    }

    public class LocalDBAudioContentV2 : AudioContentBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string AudioFilePath { get; set; }

        public DateTime? LastDownloadedOn { get; set; }
        public DateTime? LastDownloadAttempt { get; set; }

        [ForeignKey(typeof(LocalDBContactProgramDay))]
        public int ContactProgramDayId { get; set; } //this is for checking the download

    }

}
