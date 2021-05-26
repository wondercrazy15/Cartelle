using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Stance.ViewModels
{

    public class DownloadResult
    {
        public string photoFilePath { get; set; }
        public string videoFilePath { get; set; }
        public Guid ActionContentGuid { get; set; }
    }

    public class DownloadResultAudio
    {
        public string audioFilePath { get; set; }
        public Guid AudioContentGuid { get; set; }
    }

    public class RescheduledCPD3
    {
        public Guid ContactProgramGuid { get; set; }
        public List<CPD3> cpds { get; set; }
    }

    public class CPD3
    {
        public Guid guid { get; set; }
        public int SequenceNumber { get; set; }
        public int StateCode { get; set; }
        public int StatusCode { get; set; }
    }

    public class holdingCell
    {
        public CPDCell cell { get; set; }
    }

    public class CPDCell
    {
        public Guid ContactProgramDayGuid { get; set; }
        public Guid ProgramDayGuid { get; set; }
        public int dayNum { get; set; }
        public int position { get; set; }
        public DateTime? scheduledDate { get; set; }
        public int SequenceNumber { get; set; }
        public int stateCode { get; set; }
        public int statusCode { get; set; }
        public int dayTypeCode { get; set; }
        public Label Label { get; set; } //Reference to Label on SL
        public string LabelText { get; set; }
        public StackLayout btnSL { get; set; } // to hide and show
        public StackLayout bottomSL { get; set; } // to hide
        public StackLayout mainSL { get; set; } // to adjust the height
        public StackLayout ArrowSL { get; set; }
        public Button upBtn { get; set; }
        public Button downBtn { get; set; }
    }

    public class AudioModel
    {
        public Guid AudioGuid { get; set; }
        public int SequenceNumber { get; set; }
        public string FilePath { get; set; }
        public int LengthMilliseconds { get; set; }
        public int DelayMilliseconds { get; set; }
        public bool IsRepeat { get; set; }
        public int NumberOfRepeats { get; set; }
        public int RepeatCycleSeconds { get; set; }
    }

    public class SyncContactProgramDay
    {
        public Guid GuidCRM { get; set; }
        public bool Synced { get; set; }
        public bool IsComplete { get; set; }
        public int Rating { get; set; }
        public DateTime ActualStartDate { get; set; }
    }

    public class SyncCPD
    {
        public List<SyncContactProgramDay> cpd { get; set; }
    }

    public class SyncCA
    {
        public List<SyncContactAction> ca { get; set; }
    }

    public class SyncContactAction
    {
        public Guid GuidCRM { get; set; }
        public bool Synced { get; set; }
        public bool IsComplete { get; set; }

        public int ActualNumberOfReps { get; set; }
        public float ActualWeightLbs { get; set; }
        public int ActualTimeSeconds { get; set; }
        public int ActualRestTimeSeconds { get; set; }
    }

}
