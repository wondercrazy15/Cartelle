using Stance.Models.LocalDB;
using Stance.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.Models.Local
{
    public class ExerciseVideoPlayerCell
    {
        //content types 
        //exercise reps = 585860000
        //exercise time = 585860001
        //rest time = 585860002
        //seperator = 585860004
        //stretch time = 585860003
        public int num { get; set; }
        public int actionId { get; set; }
        public string videoUrl { get; set; }
        public string videoFilePath { get; set; }
        public StackLayout parentSL { get; set; }
        public StackLayout videoSL { get; set; }
        public StackLayout overlaySL { get; set; }
        public RelativeLayout videoRL { get; set; }
        public StackLayout cellSL { get; set; }
        public StackLayout restSL { get; set; }
        public int CurrentTimeSeconds { get; set; }
        public int RequiredTimeSeconds { get; set; }
        public bool IsComplete { get; set; }
        public bool IsFirstPlay { get; set; }
        public Label repsLabel { get; set; }
        public Label weightLabel { get; set; }
        public Button DoneBtn { get; set; }
        public StackLayout DoneBtnSL { get; set; }
        public int ContentTypeValue { get; set; }
        public bool TimerInProgress { get; set; }
        public Label TimerLabel { get; set; }
        public ExerciseVideoPlayerCell PreviousExerciseCell { get; set; }
        public ExerciseVideoPlayerCell NextExerciseCell { get; set; }
        public Guid ContactActionGuid { get; set; }
        public int ActionId { get; set; }
        public Guid ActionGuid { get; set; }
        public int ActionContentId { get; set; }
        //public int Type { get; set; } // 1 - seperator, 2- time based, 3 - rest, 4 - rep based
        public bool IsAudioCompletePlaying { get; set; }
        public int AudioClipToPlaySequenceNumber { get; set; } // Set to the first in the list, when switching to another set to it, and when done set to Guid Empty
        public List<LocalDBAudio> AudioFiles { get; set; }
        public CancellationTokenSource cts { get; set; }
        public CustomProgressBar ProgressBar { get; set; }
        public StackLayout ProgressBarSL { get; set; }
        public bool IsFirstSecond { get; set; }
        public Button SkipRest { get; set; }
        public bool IsSkipped { get; set; }
        public CustomProgressBarForRest ProgressBarForRest { get; set; }
        public StackLayout ProgressBarForRestSL { get; set; }
    }


}
