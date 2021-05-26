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

    public class SignInResponseV2
    {
        public OptContactV2 Profile { get; set; }
        public List<OptContactProgram> ContactPrograms { get; set; }
        public ActiveContactProgramDays ContactProgramDays { get; set; }
    }

    public class ContactProgramResponse
    {
        public List<OptContactProgram> ContactPrograms { get; set; }
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

    public class DownloadResponse
    {
        public string Message { get; set; }
        public Guid ProgramDayGuid { get; set; }
        public Guid ContactProgramDayGuid { get; set; }
        public List<OptAudio> Audios { get; set; }
        public List<OptContactAction> ContactActions { get; set; }
    }

    public class DownloadResponseV2
    {
        public string Message { get; set; }
        public Guid ProgramDayGuid { get; set; }
        public Guid ContactProgramDayGuid { get; set; }
        public List<OptAudio> Audios { get; set; }
        public List<OptContactActionV2> ContactActions { get; set; }
    }

}