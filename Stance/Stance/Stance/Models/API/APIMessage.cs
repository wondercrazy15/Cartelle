using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.API
{
    public class APIMessage : MessageBase
    {
        public APIContactV2 Sender { get; set; }
        public APIContactV2 Receiver { get; set; }
    }
}
