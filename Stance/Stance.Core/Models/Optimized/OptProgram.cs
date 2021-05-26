using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Optimized
{
    public class OptProgramV2 : BaseAttributes
    {
        public int SequenceNumber { get; set; }
        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public string PhotoUrl { get; set; }
        public string SecondaryPhotoUrl { get; set; }
        public string VideoUrl { get; set; }
        public int TotalWeeks { get; set; }
        public int GoalValue { get; set; }
        public string Goal { get; set; }
        public int LevelValue { get; set; }
        public string Level { get; set; }
        public int Type { get; set; }

        public Guid AccountGuid { get; set; }
    }

    public class OptProgramV3 : BaseAttributes
    {
        public int SequenceNumber { get; set; }
        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public string PhotoUrl { get; set; }
        public string SecondaryPhotoUrl { get; set; }
        public string VideoUrl { get; set; }
        public int TotalWeeks { get; set; }
        public int GoalValue { get; set; }
        public string Goal { get; set; }
        public int LevelValue { get; set; }
        public string Level { get; set; }
        public int Type { get; set; }
        public string Tags { get; set; }
        public string AthleteName { get; set; }
    }
}
