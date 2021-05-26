using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Optimized
{
    public class OptContactV3 : BaseAttributes
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public bool IsAdmin { get; set; }
        //public string TimeZone { get; set; }

        public DateTime? Birthday { get; set; }

        public int GenderTypeCode { get; set; }
        public string Gender { get; set; }

        public float HeightCm { get; set; }
        public float WeightLbs { get; set; }

        public int TrainingGoalTypeCode { get; set; }
        public string TrainingGoal { get; set; }

        public int RegionTypeCode { get; set; }
        public string Region { get; set; }

        public string InstagramHandle { get; set; }

        public bool ConfirmedEmail { get; set; }
        public int RP { get; set; }
    }
}
