using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.Base;

namespace Stance.Models.LocalDB
{
    public class LocalDBChallenge : ChallengeBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBAccount))]
        public int AcccountId { get; set; }

        [ForeignKey(typeof(LocalDBProgram))]
        public int ProgramId { get; set; }

        public bool IsComplete { get; set; }
    }
}
