using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.Base
{
    public abstract class BaseAttributes
    {
        public Guid GuidCRM { get; set; }
        public int StateCodeValue { get; set; }
        public int StatusCodeValue { get; set; }
    }
}
