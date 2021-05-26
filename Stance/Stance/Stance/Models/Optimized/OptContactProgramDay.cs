using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Optimized
{
    public class OptContactProgramDay : BaseAttributes
    {
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public bool Synced { get; set; }
        public int SequenceNumber { get; set; }
        public int DayTypeValue { get; set; }

        public OptProgramDay ProgramDay { get; set; }
       // public OptContactProgram ContactProgram { get; set; } //Should be ContactProgramGuid only

        //public Guid ContactProgramGuid { get; set; }
        //public Guid ProgramGuid { get; set; }

    }
}
