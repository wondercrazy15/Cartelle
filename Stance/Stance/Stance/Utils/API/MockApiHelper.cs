using ModernHttpClient;
using Stance.Models;
using Stance.Models.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stance.Utils
{
    public class MockApiHelper
    {

        const string url = "http://stanceathletes.com/api/athletes";
        private HttpClient _client = new HttpClient(new NativeMessageHandler());

        public class ProgramWeek
        {
            public int Id { get; set; }
            public Guid guid { get; set; }
            public int DayNumber { get; set; }
            public string Heading { get; set; }
            public int DayType { get; set; }
            public int NumberOfExercises { get; set; }
            public int TotalTime { get; set; }
            public string Level { get; set; }
            public bool DetailsVisible { get; set; }
        }

        public IEnumerable<ProgramWeek> GetProgramWeek(int WeekNumber)
        {
            var programWeek = new List<ProgramWeek>
            {
                new ProgramWeek { Id=1,guid=Guid.Empty,DayNumber=1,Heading="GETTING STARTED",NumberOfExercises=15,TotalTime=45,Level="MODERATE", DetailsVisible=true, DayType=1  },
                new ProgramWeek { Id=2,guid=Guid.Empty,DayNumber=2,Heading="GETTIGN STARTED AGAIN",NumberOfExercises=15,TotalTime=45,Level="MODERATE", DetailsVisible=true, DayType=1   },
                new ProgramWeek { Id=3,guid=Guid.Empty,DayNumber=3,Heading="DOWN TO BUSINESS",NumberOfExercises=15,TotalTime=45,Level="MODERATE", DetailsVisible=true, DayType=1   },
                new ProgramWeek { Id=4,guid=Guid.Empty,DayNumber=4,Heading="REST DAY", DetailsVisible=false, DayType=0  },
                new ProgramWeek { Id=5,guid=Guid.Empty,DayNumber=5,Heading="HEADS UP SMASH",NumberOfExercises=15,TotalTime=45,Level="MODERATE", DetailsVisible=true , DayType=1  },
                new ProgramWeek { Id=6,guid=Guid.Empty,DayNumber=6,Heading="LOWER BODY BLAST",NumberOfExercises=15,TotalTime=45,Level="MODERATE", DetailsVisible=true , DayType=1  },
                new ProgramWeek { Id=7,guid=Guid.Empty,DayNumber=7,Heading="REST DAY", DetailsVisible=false, DayType=0   },
            };

            return programWeek;
        }




        public APIProgram GetAthleteProgram(string id)
        {
            var program = new APIProgram
            {

                Heading = "SWING SHIFT",
                PhotoUrl = "girl_bentover_kettlebell.jpg",
                VideoUrl = "http://vjs.zencdn.net/v/oceans.mp4",
                TotalWeeks = 8,
                Goal = "FAT LOSS",
                Level = "MODERATE",
                Cost = 0.00M

            };

            return program;
        }



        public IEnumerable<APIProgram> GetAthletePrograms(Guid Id)
        {
            var programs = new List<APIProgram>
            {
                new APIProgram {  Heading="SWING SHIFT", SubHeading="Join me on this 8 week program to get ripped fast. This is your chance to get in great shape and have fun doing it!", PhotoUrl="girl_with_kettlebell.jpg",  },
                new APIProgram {  Heading="KETTLE BELT FIT", SubHeading="Join me on this 8 week program to get ripped fast. This is your chance to get in great shape and have fun doing it!", PhotoUrl="main_girl_bar.jpg",  },
                new APIProgram {  Heading="PUSH UP NUTS", SubHeading="Join me on this 8 week program to get ripped fast. This is your chance to get in great shape and have fun doing it!", PhotoUrl="main_girl_pushup.jpg",  },
            };

            return programs;

        }

        public IEnumerable<APIProgram> GetMyPrograms()
        {
            var programs = new List<APIProgram>
            {
                new APIProgram {  Heading="SWING SHIFT", SubHeading="Join me on this 8 week program to get ripped fast. This is your chance to get in great shape and have fun doing it!", PhotoUrl="girl_with_kettlebell.jpg", },
                new APIProgram {  Heading="KETTLE BELT FIT", SubHeading="Join me on this 8 week program to get ripped fast. This is your chance to get in great shape and have fun doing it!", PhotoUrl="main_girl_bar.jpg",  },
                new APIProgram {  Heading="PUSH UP NUTS", SubHeading="Join me on this 8 week program to get ripped fast. This is your chance to get in great shape and have fun doing it!", PhotoUrl="main_girl_pushup.jpg", },
            };

            return programs;

        }

        public IEnumerable<APIAction> GetProgramDayActions(Guid ProgramDayGuid)
        {
            var actions = new List<APIAction>
            {
                //content types 
                //exercise reps = 0
                //exercise time = 1
                //res time = 2
                //seperator = 3
                //stretch time, handled same as exercise time              

                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid }, ActionContent = new APIActionContent {Heading="WARM-UP" }, ContentTypeValue=3, SequenceNumber=1 },
                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP", VideoUrl="http://vjs.zencdn.net/v/oceans.mp4",PhotoUrl="rsz_pete_abdominal_tuck.jpg", }, NumberOfReps=555,WeightLbs=200.0F,TimeSeconds=180, ContentTypeValue=1, SequenceNumber=2, },
                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP" }, TimeSeconds=30,ContentTypeValue=2, SequenceNumber=3 },
                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP", VideoUrl="http://vjs.zencdn.net/v/oceans.mp4",PhotoUrl="rsz_pete_abdominal_tuck.jpg", }, NumberOfReps=15,WeightLbs=200.0F,TimeSeconds=180,  ContentTypeValue=1, SequenceNumber=4,  },
                new APIAction {ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},   ActionContent = new APIActionContent{Heading="WARM-UP" }, TimeSeconds=30,ContentTypeValue=2, SequenceNumber=5 },
                new APIAction {ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},   ActionContent = new APIActionContent{Heading="WARM-UP" }, ContentTypeValue=3, SequenceNumber=6 },
                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP", VideoUrl="http://vjs.zencdn.net/v/oceans.mp4",PhotoUrl="rsz_pete_abdominal_tuck.jpg", },NumberOfReps=55,WeightLbs=10.0F,TimeSeconds=180, ContentTypeValue=0, SequenceNumber=7,  },
                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP", VideoUrl="http://vjs.zencdn.net/v/oceans.mp4",PhotoUrl="rsz_pete_abdominal_tuck.jpg", }, NumberOfReps=1,WeightLbs=1.0F,TimeSeconds=180, ContentTypeValue=0, SequenceNumber=8,  },
                new APIAction { ProgramDay= new APIProgramDay{GuidCRM = ProgramDayGuid },  ActionContent = new APIActionContent{Heading="WARM-UP" }, TimeSeconds=30,ContentTypeValue=2, SequenceNumber=9 },
                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP", VideoUrl="http://vjs.zencdn.net/v/oceans.mp4",PhotoUrl="rsz_pete_abdominal_tuck.jpg", }, NumberOfReps=55,WeightLbs=2.0F,TimeSeconds=180,  ContentTypeValue=0, SequenceNumber=10,  },
                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP", VideoUrl="http://vjs.zencdn.net/v/oceans.mp4",PhotoUrl="rsz_pete_abdominal_tuck.jpg", }, NumberOfReps=15,WeightLbs=10.0F,TimeSeconds=180,  ContentTypeValue=0, SequenceNumber=11,  },
                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP", VideoUrl="http://vjs.zencdn.net/v/oceans.mp4",PhotoUrl="rsz_pete_abdominal_tuck.jpg", }, NumberOfReps=105,WeightLbs=5.0F,TimeSeconds=180,ContentTypeValue=0, SequenceNumber=12,  },
                new APIAction {ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP" }, TimeSeconds=30,ContentTypeValue=2, SequenceNumber=13 },
                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP" }, ContentTypeValue=3, SequenceNumber=14 },
                new APIAction { ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP", VideoUrl="http://vjs.zencdn.net/v/oceans.mp4",PhotoUrl="rsz_pete_abdominal_tuck.jpg", },NumberOfReps=5,WeightLbs=80.0F,TimeSeconds=180, ContentTypeValue=0, SequenceNumber=15,  },
                new APIAction { ProgramDay= new APIProgramDay{GuidCRM = ProgramDayGuid },  ActionContent = new APIActionContent{Heading="WARM-UP", VideoUrl="http://vjs.zencdn.net/v/oceans.mp4",PhotoUrl="rsz_pete_abdominal_tuck.jpg", }, NumberOfReps=12,WeightLbs=12.0F,TimeSeconds=180,  ContentTypeValue=0, SequenceNumber=16,  },
                new APIAction {ProgramDay= new APIProgramDay{GuidCRM = ProgramDayGuid },   ActionContent = new APIActionContent{Heading="WARM-UP" }, TimeSeconds=30,ContentTypeValue=2, SequenceNumber=17 },
                new APIAction {ProgramDay= new APIProgramDay{GuidCRM = ProgramDayGuid },  ActionContent = new APIActionContent{Heading="WARM-UP" }, ContentTypeValue=3, SequenceNumber=18 },
                new APIAction {ProgramDay= new APIProgramDay{ GuidCRM = ProgramDayGuid},  ActionContent = new APIActionContent{Heading="WARM-UP", VideoUrl="http://vjs.zencdn.net/v/oceans.mp4",PhotoUrl="rsz_pete_abdominal_tuck.jpg", }, NumberOfReps=9,WeightLbs=200.0F,TimeSeconds=180, ContentTypeValue=0, SequenceNumber=19,  },

            };

            return actions.OrderBy(x => x.SequenceNumber);

            //img.Source = "rsz_pete_abdominal_tuck.jpg";
            //headingText.Text = "Jumping Jacks";
            //repsNumText.Text = "55";
            //weightNumText.Text = "200";
        }

        public IEnumerable<APIContactAction> GetContactActions(Guid ProgramDayGuid)
        {
            var actions = new List<APIContactAction>
            {
                //content types 
                //exercise reps = 0
                //exercise time = 1
                //res time = 2
                //seperator = 3
                //stretch time, handled same as exercise time              

                new APIContactAction {  Action = { ContentTypeValue=3, SequenceNumber=1},  },
                new APIContactAction { Action = {NumberOfReps=555,WeightLbs=200.0F,TimeSeconds=180, ContentTypeValue=1, SequenceNumber=2, },  },
                new APIContactAction { Action = {  TimeSeconds=30,ContentTypeValue=2, SequenceNumber=3  },},
                new APIContactAction { Action = { NumberOfReps=15,WeightLbs=200.0F,TimeSeconds=180, ActionContent = { VideoUrl = "http://vjs.zencdn.net/v/oceans.mp4", PhotoUrl = "rsz_pete_abdominal_tuck.jpg", }, ContentTypeValue = 1, SequenceNumber=4,},   },
                new APIContactAction { Action = { TimeSeconds=30,ContentTypeValue=2, SequenceNumber=5},  },
                new APIContactAction { Action = {  ContentTypeValue=3, SequenceNumber=6 },},
                new APIContactAction { Action = { NumberOfReps=55,WeightLbs=10.0F,TimeSeconds=180, ActionContent = { VideoUrl = "http://vjs.zencdn.net/v/oceans.mp4", PhotoUrl = "rsz_pete_abdominal_tuck.jpg", }, ContentTypeValue =0, SequenceNumber=7,},   },
                new APIContactAction { Action = { NumberOfReps=1,WeightLbs=1.0F,TimeSeconds=180, ActionContent = { VideoUrl = "http://vjs.zencdn.net/v/oceans.mp4", PhotoUrl = "rsz_pete_abdominal_tuck.jpg", }, ContentTypeValue =0, SequenceNumber=8,},   },
                new APIContactAction { Action = { TimeSeconds=30,ContentTypeValue=2, SequenceNumber=9},  },
                new APIContactAction { Action = { NumberOfReps=55,WeightLbs=2.0F,TimeSeconds=180, ActionContent = { VideoUrl = "http://vjs.zencdn.net/v/oceans.mp4", PhotoUrl = "rsz_pete_abdominal_tuck.jpg", }, ContentTypeValue = 0, SequenceNumber=10,},   },
                new APIContactAction { Action = {  NumberOfReps=15,WeightLbs=10.0F,TimeSeconds=180, ActionContent = { VideoUrl = "http://vjs.zencdn.net/v/oceans.mp4", PhotoUrl = "rsz_pete_abdominal_tuck.jpg", }, ContentTypeValue = 0, SequenceNumber=11,},  },
                new APIContactAction { Action = { NumberOfReps=105,WeightLbs=5.0F,TimeSeconds=180, ActionContent = { VideoUrl = "http://vjs.zencdn.net/v/oceans.mp4", PhotoUrl = "rsz_pete_abdominal_tuck.jpg", }, ContentTypeValue = 0, SequenceNumber=12, },  },
                new APIContactAction {Action = { TimeSeconds=30,ContentTypeValue=2, SequenceNumber=13 }, },
                new APIContactAction { Action = { ContentTypeValue=3, SequenceNumber=14},  },
                new APIContactAction { Action = {NumberOfReps=5,WeightLbs=80.0F,TimeSeconds=180, ActionContent = { VideoUrl = "http://vjs.zencdn.net/v/oceans.mp4", PhotoUrl = "rsz_pete_abdominal_tuck.jpg", }, ContentTypeValue =0, SequenceNumber=15, },   },
                new APIContactAction { Action = { NumberOfReps=12,WeightLbs=12.0F,TimeSeconds=180, ActionContent = { VideoUrl = "http://vjs.zencdn.net/v/oceans.mp4", PhotoUrl = "rsz_pete_abdominal_tuck.jpg", }, ContentTypeValue =0, SequenceNumber=16,},   },
                new APIContactAction { Action = { TimeSeconds=30,ContentTypeValue=2, SequenceNumber=17},  },
                new APIContactAction {Action = { ContentTypeValue=3, SequenceNumber=18 }, },
                new APIContactAction {Action = { NumberOfReps=9,WeightLbs=200.0F,TimeSeconds=180, ActionContent = { VideoUrl = "http://vjs.zencdn.net/v/oceans.mp4", PhotoUrl = "rsz_pete_abdominal_tuck.jpg", }, ContentTypeValue = 0, SequenceNumber=19, },  },

            };

            return actions.OrderBy(x => x.Action.SequenceNumber);

            //img.Source = "rsz_pete_abdominal_tuck.jpg";
            //headingText.Text = "Jumping Jacks";
            //repsNumText.Text = "55";
            //weightNumText.Text = "200";
        }


    }
}
