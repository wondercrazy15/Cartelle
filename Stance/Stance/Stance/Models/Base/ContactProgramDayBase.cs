using Stance.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Base
{
    public class ContactProgramDayBase : BaseAttributes
    {
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }

        public int PercentComplete { get; set; }
        public int NumberOfDownloads { get; set; }
        public int Rating { get; set; }

        public bool Synced { get; set; }
        public int SequenceNumber { get; set; }
        public int DayTypeValue { get; set; }
    }
}
