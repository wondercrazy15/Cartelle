﻿using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.API
{
    public class APIProgramDay : ProgramDayBase
    {
        public APIProgram Program { get; set; }

    }
}
