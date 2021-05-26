using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Base
{
    public abstract class ActionBase : BaseAttributes
    {
        public int SequenceNumber { get; set; }

        public string ContentType { get; set; }
        public int ContentTypeValue { get; set; }
        public bool IsTrainingRound { get; set; }

        public string Heading { get; set; }
        public int NumberOfReps { get; set; }
        public float WeightLbs { get; set; }
        public int TimeSeconds { get; set; }
        public string Intensity { get; set; }
        public int IntensityValue { get; set; }

    }
}
