﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.LocalDB
{
    public class LocalDBAction : ActionBase
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(LocalDBProgramDay))]
        public int ProgramDayId { get; set; }

        [ForeignKey(typeof(LocalDBActionContentV2))]
        public int ActionContentId { get; set; }


    }
}
