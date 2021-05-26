using Stance.Models.Base;
using System;

namespace Stance.Models.Optimized
{
    public class OptChallenge : BaseAttributes
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool Activated { get; set; }
        public bool ScheduleSet { get; set; }

        public Guid AccountGuid { get; set; }
        public Guid ProgramGuid { get; set; }
    }
}