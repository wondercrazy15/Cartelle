using Stance.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Base
{
    public class ContactProgramBase : BaseAttributes
    {

        public DateTime? StartDate { get; set; } // changed
        public DateTime? EndDate { get; set; } // changed
        public DateTime? ExpiryDate { get; set; }// changed
        public bool IsDaysCreated { get; set; }
        public bool IsScheduleBuilt { get; set; }

        public int TotalProgramDays { get; set; }
        public int TotalProgramDaysComplete { get; set; }
        public decimal PercentComplete { get; set; }

        public int TotalExercisedDays { get; set; }
        public int TotalMissedDays { get; set; }
        public int TotalDaysPast { get; set; }


    }
}
