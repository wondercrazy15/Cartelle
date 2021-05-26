using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Base
{
    public class ContactActionBase : BaseAttributes
    {
        public int ActualNumberOfReps { get; set; }
        public float ActualWeightLbs { get; set; }
        public int ActualTimeSeconds { get; set; }
        public int ActualRestTimeSeconds { get; set; }
        public bool Synced { get; set; }

    }
}
