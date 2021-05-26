using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Optimized
{
    public class OptProgramDay : BaseAttributes
    {
        public int SequenceNumber { get; set; }
        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public string PhotoUrl { get; set; }
        public string DayType { get; set; }
        public int DayTypeValue { get; set; }
        public int TotalExercises { get; set; }
        public int TimeMinutes { get; set; }
        public int LevelValue { get; set; }
        public string Level { get; set; }

       // public OptProgram Program { get; set; }
       // public Guid ProgramGuid { get; set; }
    }
}
