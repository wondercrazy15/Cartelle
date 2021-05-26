using BranchXamarinSDK;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace Stance.Utils
{
    public class TestXamarinFormsAppBUO : Application, IBranchBUOSessionInterface
    {

        public TestXamarinFormsAppBUO()
        {
        }

        #region IBranchBUOSessionInterface implementation

        public void InitSessionComplete(BranchUniversalObject buo, BranchLinkProperties blp)
        {
            //System.Diagnostics.Debug.WriteLine(buo);
            //System.Diagnostics.Debug.WriteLine(blp);

        }

        public void SessionRequestError(BranchError error)
        {
        }

        #endregion
    }
}
