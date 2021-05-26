using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;


namespace StanceWeb.Models.App.Base
{
    public class AudioContentBase : BaseAttributes
    {

        //public string ContentType { get; set; }
        //public int ContentTypeValue { get; set; }
        public int LengthMilliseconds { get; set; }
        public string AudioUrl { get; set; }

    }
}