using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Base
{
    public class MessageBase : BaseAttributes
    {
        public string Subject { get; set; }
        public int TypeCode { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }

        public Guid ProgramDayGuid { get; set; }

        public DateTime? SentOn { get; set; }
        public DateTime? ReceivedOn { get; set; }
    }
}
