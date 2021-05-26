using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;


namespace StanceWeb.Models.App.Base
{
    public class AudioBase : BaseAttributes
    {

        public int SequenceNumber { get; set; }
        public int PreDelay { get; set; }
        public bool IsRepeat { get; set; }
        public int NumberOfRepeats { get; set; }
        public int RepeatCycleSeconds { get; set; }
        
    }
}