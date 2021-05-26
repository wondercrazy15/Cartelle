using SQLite;
using System;

namespace Stance.Models.LocalDB
{
    public class LocalDBSync
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime? SyncedOn { get; set; }
        public DateTime? SubscriptionSyncedOn { get; set; }

    }
}
