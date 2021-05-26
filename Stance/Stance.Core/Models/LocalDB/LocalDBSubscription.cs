using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.Base;

namespace Stance.Models.LocalDB
{
    public class LocalDBSubscription : SubscriptionBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBAccount))]
        public int AccountId { get; set; }

        [ForeignKey(typeof(LocalDBProgram))]
        public int ProgramId { get; set; }

        [ForeignKey(typeof(LocalDBChallenge))]
        public int ChallengeId { get; set; }
    }
}
