using System;
using Appleseed.Framework.Web.UI.WebControls;
using History=Appleseed.Framework.History;

namespace Appleseed.Content.Web.Modules
{
    /// <summary>
    /// PageKeyPhrase is a module that uses the PageKeyPhrase Control to allow users to position the menu keyphrase without the need to touch the layout.
    /// </summary>
    [History("John.Mandia@whitelightsolutions.com", "2003/10/25", "Created module and the control")]
    public partial class PageKeyPhraseModule : PortalModuleControl
    {

       
        #region General Implementation

        /// <summary>
        /// GuidID
        /// </summary>
        /// <value></value>
        public override Guid GuidID
        {
            get { return new Guid("{ED58A5FB-D041-4a4b-826E-654250B61E7C}"); }
        }

        #endregion

    }
}