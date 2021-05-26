using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.Base;

namespace Stance.Models.LocalDB
{
    public class LocalDBContactProgram : ContactProgramBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBProgram))]
        public int ProgramId { get; set; }

        public bool IsComplete { get; set; }

        [ForeignKey(typeof(LocalDBChallenge))]
        public int ChallengeId { get; set; }

    }
}
