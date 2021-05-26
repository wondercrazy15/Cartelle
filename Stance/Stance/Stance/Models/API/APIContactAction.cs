using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.API
{
    public class APIContactAction : ContactActionBase
    {
        public APIContactV2 Contact { get; set; }
        public APIContactProgramDay ContactProgramDay { get; set; }

        public APIAction Action { get; set; }
        public APIActionContent ActionContent { get; set; }       

    }
}
