using BranchXamarinSDK;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace Stance.Utils
{
    public class TestXamarinFormsApp : Application, IBranchSessionInterface
    {

        public TestXamarinFormsApp()
        {
        }

        #region IBranchSessionInterface implementation

        public void InitSessionComplete(Dictionary<string, object> data)
        {
        }

        public void CloseSessionComplete()
        {
        }

        public void SessionRequestError(BranchError error)
        {
        }

        #endregion
    }
}
