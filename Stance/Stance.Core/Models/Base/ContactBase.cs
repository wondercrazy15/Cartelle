using System;

namespace Stance.Models.Base
{
    public abstract class ContactBaseV2 : BaseAttributes
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        //public string TimeZone { get; set; }

        public DateTime? Birthday { get; set; }
        //public int Age { get; set; }
        public int GenderTypeCode { get; set; }
        public string Gender { get; set; }

        public float HeightCm { get; set; }
        public float WeightLbs { get; set; }

        public int TrainingGoalTypeCode { get; set; }
        public string TrainingGoal { get; set; }

        public int RegionTypeCode { get; set; }
        public string Region { get; set; }

        public string InstagramHandle { get; set; }


    }

    public abstract class ContactBaseV3 : BaseAttributes
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        //public string TimeZone { get; set; }

        public DateTime? Birthday { get; set; }
        //public int Age { get; set; }
        public int GenderTypeCode { get; set; }
        public string Gender { get; set; }

        public float HeightCm { get; set; }
        public float WeightLbs { get; set; }

        public int TrainingGoalTypeCode { get; set; }
        public string TrainingGoal { get; set; }

        public int RegionTypeCode { get; set; }
        public string Region { get; set; }

        public string InstagramHandle { get; set; }

        public string AppVersion { get; set; }
        public string DeviceType { get; set; }
        public int DeviceOS { get; set; }

    }
}
