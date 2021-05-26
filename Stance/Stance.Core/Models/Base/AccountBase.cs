using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Base
{
    public abstract class AccountBase : BaseAttributes
    {
        public string Heading { get; set; }
        public string SubHeading { get; set; }

        public string PhotoUrl { get; set; }
        public string SecondaryPhotoUrl { get; set; }
        public string VideoUrl { get; set; }

        public int SequenceNumber { get; set; }
    }



}
