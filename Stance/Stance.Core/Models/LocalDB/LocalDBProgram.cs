using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.Base;

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

        public int Type { get; set; }

        public string Tags { get; set; }
        public string AthleteName { get; set; }
        public string Code { get; set; }

    }


}
