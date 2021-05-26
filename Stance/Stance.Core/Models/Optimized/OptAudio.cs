using System;

namespace Stance.Models.Optimized
{
    public class OptAudio
    {
        public Guid GuidCRM { get; set; }
        public int SequenceNumber { get; set; }
        public int PreDelay { get; set; }
        public bool IsRepeat { get; set; }
        public int NumberOfRepeats { get; set; }
        public int RepeatCycleSeconds { get; set; }
        public Guid ActionGuid { get; set; }

        public OptAudioContent AudioContent { get; set; }
    }
}