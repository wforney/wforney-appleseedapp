Blog Module

This module renders a list of Blog Entries and provides RSS and other basic blog features.
NOTE: See list of keys and style classes at the end of this file!

Credits: 

Joe Audette joe@joeaudette.com  http://www.joeaudette.com

Another Appleseed desktop module - more to download on http://www.Appleseedportal.net

INSTALL
1. Go to Admin all and to add module definition. 
2. Point to install.xml install file
3. Add the module to a page
4. Edit module settings:
5. Use it! ;o) 

Note: The module is automatically installed when you install Appleseed.
The install procedure is only required if you deleted the module in Admin all


HISTORY
Ver. 1.0 - 8. february 2004. Released by Joe Audette
Ver. 1.1 - 18. february 2004. Added to core Appleseed by Jakob Hansen.
Ver. 1.2 - 3. march 2004. Fixed PortalSearch problem caused by this module.


Issues and Known problems:
- Tested with Appleseed version 1.3.0.1759 - 03/03/2004


Keys used by this module (for file Resources\Appleseed.??.resx):

    <data name="BLOG_ENTRIES">
        <value>Entries</value>
    </data>
    <data name="BLOG_COMMENTS">
        <value>Comments</value>
    </data>
    <data name="BLOG_SYNDICATION">
        <value>Syndication</value>
    </data>
    <data name="BLOG_STATISTICS">
        <value>Statistics</value>
    </data>
    <data name="BLOG_ARCHIVES">
        <value>Archives</value>
    </data>
    <data name="BLOG_ENTRY">
        <value>Blog Entry</value>
    </data>
    <data name="BLOG_POSTSFROM">
        <value>Posts From</value>
    </data>



Style Class examples for use in themes:

/* Blog */
/* style for Blog titles */
.BlogHead
{
    font-family: Verdana, Helvetica, sans-serif;
    font-size: 15px;
    font-weight: bold;
    color: #000000;
}
.BlogTitle
{
    font-family: Verdana, Helvetica, sans-serif;
    font-size: 15px;
    font-weight: bold;
    color: #000000;
}
.BlogFooter
{
    
}
.BlogCommentName
{

}

.BlogItemDate
{

}

