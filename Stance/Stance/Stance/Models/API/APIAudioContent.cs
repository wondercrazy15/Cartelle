using Stance.Models.API;
using StanceWeb.Models.App.Base;
using System;
using System.Collections.Generic;
using System.Linq;


namespace StanceWeb.Models.App.API
{
    public class APIAudioContent : AudioContentBase
    {
        public APIProgram Program { get; set; }

    }
}