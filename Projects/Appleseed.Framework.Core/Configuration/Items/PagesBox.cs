// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagesBox.cs" company="--">
//   Copyright © -- 2010. All Rights Reserved.
// </copyright>
// <summary>
//   Box tab
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Appleseed.Framework.Site.Configuration
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Pages Box is a collection of PageStripDetails.
    /// </summary>
    [History("jminond", "2005/03/10", "Tab to page conversion")]
    [Obsolete("Use Collection<PageStripDetails> instead.")]
    public class PagesBox : Collection<PageStripDetails>
    {
    }
}