using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Base
{
    public abstract class ProgramDayBase : BaseAttributes
    {
        public int TimeSeconds { get; set; }

        public int SequenceNumber { get; set; }
        public string DayType { get; set; }
        public int DayTypeValue { get; set; }
        public int TotalActions { get; set; }

        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public string PhotoUrl { get; set; }
        public int TotalExercises { get; set; }
        public int TimeMinutes { get; set; }

        public int LevelValue { get; set; }
        public string Level { get; set; }

    }
}
