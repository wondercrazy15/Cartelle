using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stance.Models.Optimized
{

    public class OptSubscriptionV2 : BaseAttributes
    {
        public Guid AccountGuid { get; set; }
        public Guid ProgramGuid { get; set; }
        public Guid ChallengeGuid { get; set; }

        public int Type { get; set; }
        public bool AutoRenew { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? EndedOn { get; set; }

        public bool StartedAsTrial { get; set; }
        public DateTime? TrialEndedOn { get; set; }
        public DateTime? TrialEndDate { get; set; }
    }
}
