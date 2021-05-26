using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stance.Models.Optimized
{
    public class OptAccountV2 : BaseAttributes
    {
        public string Heading { get; set; }
        public string SubHeading { get; set; }

        public string PhotoUrl { get; set; }
        public string SecondaryPhotoUrl { get; set; }
        public string VideoUrl { get; set; }

        public int SequenceNumber { get; set; }

        public string IGProfileUrl { get; set; }
    }
}
