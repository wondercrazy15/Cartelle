

namespace Stance.Models.Base
{
    public abstract class ActionContentBase : BaseAttributes
    {
        public string ContentType { get; set; }
        public int ContentTypeValue { get; set; }


        public string Heading { get; set; }
        public string SubHeading { get; set; }

        public int NumberOfReps { get; set; }
        public float WeightLbs { get; set; }
        public int TimeSeconds { get; set; }
        public string Intensity { get; set; }
        public int IntensityValue { get; set; }

        public string PhotoUrl { get; set; }
        public string VideoUrl { get; set; }

    }
}
