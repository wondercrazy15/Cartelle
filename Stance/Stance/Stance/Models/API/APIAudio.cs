using Stance.Models.API;
using StanceWeb.Models.App.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StanceWeb.Models.App.API
{
    public class APIAudio : AudioBase
    {
        public APIAction Action { get; set; }
        public APIAudioContent AudioContent { get; set; }

        public APIProgramDay ProgramDay { get; set; }

    }
}