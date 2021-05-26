using SQLite;
using System;

namespace Stance.Models.LocalDB
{
    public class LocalDBIAP
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string PurchaseId { get; set; }
        public string Token { get; set; }
        public int State { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public bool IsSynced { get; set; }
        public bool Confirmed { get; set; }
        public Guid ProgramGuid { get; set; }
        //public bool IsInTestingMode { get; set; }
    }
}
