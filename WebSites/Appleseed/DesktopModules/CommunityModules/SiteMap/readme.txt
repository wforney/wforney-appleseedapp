Sitemap module
Credits and copyrights: Michel Barneveld ( Appleseed@MichelBarneveld.Com )

Another Appleseed desktop module - more to download on http://www.Appleseedportal.net


INSTALL
1. Go to Admin all and to add module definition. 
2. Point to install.xml install file
3. Add the module to a page
4. Edit module settings: See below
5. Use it! ;o) 
Note: The module is automatically installed when you install Appleseed.
The install procedure is only required if you deleted the module in Admin all


HISTORY
Ver 1.0 - 9. may 2003 - initial release (Added by Jakob Hansen)
Ver 1.1 - 23. june 2004 - Added setting "ShowTab"

Issues and Known problems:
- Tested with Appleseed version 1.4.0.1764j - 23/06/2004


Module settings
---------------
BindToTab: if checked the sitemap will be shown from the current tab, 
otherwise it shows the complete site (well, only the pages you have access to)

ShowTabID: Only used when BindToTab is checked. Default value is 0 meaning the tab where the module is placed.
You set the ShowTabID value to an existing Tab id: Use the number after "tabID__" in the url string, 
e.g. 3326 in "http://Yourdomain/Appleseed/portal/alias__Appleseed/lang__en-US/tabID__3326/DesktopDefault.aspx".
