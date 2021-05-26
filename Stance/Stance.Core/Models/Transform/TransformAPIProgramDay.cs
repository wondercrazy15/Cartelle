using Stance.Models.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Transform
{
    public class TransformAPIProgramDay : APIProgramDay
    {
        public bool DetailsVisible { get; set; }

        public TransformAPIProgramDay()
        {
            Program = new APIProgram();
        }
    }
}
