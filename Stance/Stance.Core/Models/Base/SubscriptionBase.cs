using System;

namespace Stance.Models.Base
{
    public abstract class SubscriptionBase : BaseAttributes
    {
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
