using Stance.Models.Base;

namespace Stance.Models.Optimized
{
    public class OptAction : BaseAttributes
    {
        public int SequenceNumber { get; set; }
        public int ContentTypeValue { get; set; }
        public string Heading { get; set; }
        public int NumberOfReps { get; set; }
        public float WeightLbs { get; set; }
        public int TimeSeconds { get; set; }
        public int IntensityValue { get; set; }
    }

    public class OptActionV2 : BaseAttributes
    {
        public int SequenceNumber { get; set; }
        public int ContentTypeValue { get; set; }
        public bool IsTrainingRound { get; set; }
        public string Heading { get; set; }
        public int NumberOfReps { get; set; }
        public float WeightLbs { get; set; }
        public int TimeSeconds { get; set; }
        public int IntensityValue { get; set; }
    }

}
