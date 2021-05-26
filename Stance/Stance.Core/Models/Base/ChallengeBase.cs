using System;
using System.Collections.Generic;
using System.Text;

namespace Stance.Models.Base
{
    public class ChallengeBase : BaseAttributes
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 

        public bool Activated { get; set; }
        public bool ScheduleSet { get; set; }
    }
}
