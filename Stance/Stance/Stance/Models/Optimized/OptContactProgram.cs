using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Optimized
{
    public class OptContactProgram : BaseAttributes
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDaysCreated { get; set; }
        public bool IsScheduleBuilt { get; set; }

        public OptProgram Program { get; set; }
    }
}
