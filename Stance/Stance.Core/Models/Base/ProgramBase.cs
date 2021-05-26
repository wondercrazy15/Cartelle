using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Base
{
    public abstract class ProgramBase : BaseAttributes
    {
        //public APIAccount Account { get; set; }
        //public Decimal Cost { get; set; } //removed
        public int TotalProgramDays { get; set; }
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

    }
}
