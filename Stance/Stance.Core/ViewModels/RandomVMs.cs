using Stance.Models.Base;
using Stance.Models.LocalDB;
using Stance.Models.Optimized;
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

    public class SyncContactProgramDayV2
    {
        public Guid GuidCRM { get; set; }
        public bool Synced { get; set; }
        public bool IsComplete { get; set; }
        public int Rating { get; set; }
        public DateTime ActualStartDate { get; set; }
        public Guid ProgramGuid { get; set; }
    }

    public class SyncCPDV2
    {
        public List<SyncContactProgramDayV2> cpd { get; set; }
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

    public class SubscriptionsV2
    {
        public List<OptSubscriptionV2> subscriptions { get; set; }
    }

    public class WeekBtn
    {
        public Button btn { get; set; }
        public int WeekNum { get; set; }
        public bool isEditing { get; set; }
    }

    public class ContactProgramList
    {
        public LocalDBProgram Program { get; set; }
        public int ContactProgramStatus { get; set; }
        public int ContactProgramState { get; set; }
        public bool IsActive { get; set; }
    }

    public class ContactProgramBtn
    {
        public Button Button { get; set; }
        public LocalDBContactProgram ContactProgram { get; set; }
    }

    public class CreateAccountContact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public Guid ProgramGuid { get; set; }
        //public bool changedFilter { get; set; }
        //public string WorkoutSetting { get; set; }
        //public string WorkoutGoal { get; set; }
        //public string WorkoutLevel { get; set; }
        public int PlatformSource { get; set; }
        public string AppVersion { get; set; }
        public string DeviceType { get; set; }
        public int DeviceOS { get; set; }
    }

    public class ContactSignUpV2
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int PlatformSource { get; set; }
        public string AppVersion { get; set; }
        public string DeviceType { get; set; }
        public int DeviceOS { get; set; }

        public int RevenuePool { get; set; } //Athlete or Cartelle
        public string WorkoutSetting { get; set; }
        public string WorkoutGoal { get; set; }
        public string WorkoutLevel { get; set; }

        public string ReferralPool { get; set; }//cartelle or athlete - used in app for revenue pool
        public string ReferrerCode { get; set; }//athlete code or cartelle - identifies athlete code
        public string LeadSource { get; set; }//lead source in crm/where link was placed - used to describe where link was placed

        public Guid ProgramGuid { get; set; }
        public Guid AccountGuid { get; set; }
        public string ProgramCode { get; set; }
        public string AccountCode { get; set; }
    }

    public class SyncIAP
    {
        public string Token { get; set; }
        public string DeviceModel { get; set; }
        public int PaymentProvider { get; set; }
        public string AppVersion { get; set; }
        public int PlatformSource { get; set; }
        public int DeviceOS { get; set; }
        public Guid ProgramGuid { get; set; }
    }


    public class SignUpData
    {
        public List<AthleteData> Athletes { get; set; }
        public SignUpData()
        {
            Athletes = new List<AthleteData>();
        }
    }

    public class AthleteOverviewData : BaseAttributes
    {
        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public string SecondaryPhotoUrl { get; set; }
        public string Code { get; set; }
        public List<ProgramData> Programs { get; set; }
        public AthleteOverviewData()
        {
            Programs = new List<ProgramData>();
        }
    }

    public class AthleteData : BaseAttributes
    {
        public string Heading { get; set; }
        public int SequenceNumber { get; set; }
        public string IGProfileUrl { get; set; }
        public string Code { get; set; }
        public List<ProgramData> Programs { get; set; }
        public AthleteData()
        {
            Programs = new List<ProgramData>();
        }
    }

    public class ProgramData : BaseAttributes
    {
        public int SequenceNumber { get; set; }
        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public string PhotoUrl { get; set; }
        public int Type { get; set; }
        public string Code { get; set; }
    }

}
