using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Utils.OptionSets
{

    public class OptionSetModel
    {
        public int IndexVal { get; set; }
        public int TypeCode { get; set; }
        public string Name { get; set; }
    }

    public static class CRMOptionSets
    {
        public static List<OptionSetModel> GenderOptionSet()
        {
            var genderList = new List<OptionSetModel>()
            {
                new OptionSetModel
                {
                    IndexVal = 0,
                    TypeCode = 2,
                    Name = "Female"
                },
                new OptionSetModel
                {
                    IndexVal = 1,
                    TypeCode = 1,
                    Name = "Male"
                },
                new OptionSetModel
                {
                    IndexVal = 2,
                    TypeCode = 866660000,
                    Name = "Other"
                },
            };

            return genderList; 
        }

        public static List<OptionSetModel> TrainingGoalOptionSet()
        {
            var TrainingGoalList = new List<OptionSetModel>()
            {
                new OptionSetModel
                {
                    IndexVal = 0,
                    TypeCode = 866660000,
                    Name = "Strength"
                },
                new OptionSetModel
                {
                    IndexVal = 1,
                    TypeCode = 866660001,
                    Name = "Endurance"
                },
                new OptionSetModel
                {
                    IndexVal = 2,
                    TypeCode = 866660002,
                    Name = "Mobility"
                },
                new OptionSetModel
                {
                    IndexVal = 3,
                    TypeCode = 866660003,
                    Name = "Weight Loss"
                },
                new OptionSetModel
                {
                    IndexVal = 4,
                    TypeCode = 866660004,
                    Name = "Get Ripped"
                },
                new OptionSetModel
                {
                    IndexVal = 5,
                    TypeCode = 866660005,
                    Name = "Skills"
                },
                new OptionSetModel
                {
                    IndexVal = 6,
                    TypeCode = 866660007,
                    Name = "Health"
                },
                new OptionSetModel
                {
                    IndexVal = 7,
                    TypeCode = 866660006,
                    Name = "Other"
                },
            };

            return TrainingGoalList;
        }

        public static List<OptionSetModel> RegionOptionSet()
        {
            var RegionGoalList = new List<OptionSetModel>()
            {
                new OptionSetModel
                {
                    IndexVal = 0,
                    TypeCode = 866660011,
                    Name = "North America - Canada"
                },
                new OptionSetModel
                {
                    IndexVal = 1,
                    TypeCode = 866660006,
                    Name = "North America - USA"
                },
                new OptionSetModel
                {
                    IndexVal = 2,
                    TypeCode = 866660003,
                    Name = "Eastern Europe"
                },
                new OptionSetModel
                {
                    IndexVal = 3,
                    TypeCode = 866660004,
                    Name = "European Union"
                },
                new OptionSetModel
                {
                    IndexVal = 4,
                    TypeCode = 866660001,
                    Name = "Asia"
                },
                new OptionSetModel
                {
                    IndexVal = 5,
                    TypeCode = 866660002,
                    Name = "Central America"
                },
                new OptionSetModel
                {
                    IndexVal = 6,
                    TypeCode = 866660005,
                    Name = "Middle East"
                },
                new OptionSetModel
                {
                    IndexVal = 7,
                    TypeCode = 866660007,
                    Name = "Oceania - Australia"
                },
                new OptionSetModel
                {
                    IndexVal = 8,
                    TypeCode = 866660008,
                    Name = "South America"
                },
                new OptionSetModel
                {
                    IndexVal = 9,
                    TypeCode = 866660009,
                    Name = "The Caribbean"
                },
                new OptionSetModel
                {
                    IndexVal = 10,
                    TypeCode = 866660000,
                    Name = "Africa"
                },
                //new OptionSetModel
                //{
                //    IndexVal = 11,
                //    TypeCode = 866660010,
                //    Name = "Polar"
                //},

            };

            return RegionGoalList;

        }



    }
}
