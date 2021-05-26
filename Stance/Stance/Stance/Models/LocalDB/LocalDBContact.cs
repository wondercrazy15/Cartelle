using SQLite;
using Stance.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Models.LocalDB
{
    class LocalDBContact : ContactBaseV2
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string ProfilePhotoFilePath { get; set; }
        public string BeforePhotoFilePath { get; set; }
        public string AfterPhotoFilePath { get; set; }

        public string ProfileVideoFilePath { get; set; }

    }

    class LocalDBContactV2 : ContactBaseV2
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string ProfilePhotoFilePath { get; set; }
        public string BeforePhotoFilePath { get; set; }
        public string AfterPhotoFilePath { get; set; }

        public string ProfileVideoFilePath { get; set; }

    }
}
