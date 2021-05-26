using Stance.Models.Optimized;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StanceWeb.Models.App.Optimized
{
    public class TaskResult
    {
        public string Type { get; set; }
        public object Obj { get; set; }
    }

    public class SignInResponseV5
    {
        public OptContactV3 Profile { get; set; }
        public List<OptSubscriptionV2> Subscriptions { get; set; }
        public List<OptContactProgramV2> ContactPrograms { get; set; }
        public ActiveContactProgramDays ContactProgramDays { get; set; }
        public List<ContactChallenge> ContactChallenges { get; set; }
    }

    public class ContactChallenge
    {
        public Guid ContactProgramGuid { get; set; }
        public OptChallenge Challenge { get; set; }
    }

    public class ContactProgramResponseV2
    {
        public List<OptContactProgramV2> ContactPrograms { get; set; }
        public ActiveContactProgramDays ContactProgramDays { get; set; }
    }

    public class ActiveContactProgramDays
    {
        public Guid ProgramGuid { get; set; }
        public Guid ContactProgramGuid { get; set; }

        public List<OptContactProgramDay> ContactProgramDays { get; set; }

        public ActiveContactProgramDays()
        {
            ContactProgramDays = new List<OptContactProgramDay>();
        }
    }

    public class DownloadResponseV2
    {
        public string Message { get; set; }
        public Guid ProgramDayGuid { get; set; }
        public Guid ContactProgramDayGuid { get; set; }
        public List<OptAudio> Audios { get; set; }
        public List<OptContactActionV2> ContactActions { get; set; }
    }


    public class TrialProgram
    {
        public Guid ProgramGuid { get; set; }
        public bool changedFilter { get; set; }
        public string WorkoutSetting { get; set; }
        public string WorkoutGoal { get; set; }
        public string WorkoutLevel { get; set; }
    }

}