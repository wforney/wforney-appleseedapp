FAQs - Frequently Asked Questions module
Credits and copyrights: Christopher S Judd, CDP & Horizons, LLC

NOTE: Does not work together with SendThoughts on same tabpage!


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
Ver 1.0 - 5. dec 2002 - Moved from original IBS and VB into Appleseed and C# by Jakob Hansen
Ver 1.1 - 25. april 2003 - Updated to follow "Appleseed best practices"
Ver 1.2 - 25. may 2003 - FAQs are now 100% ready for localization. 
          Localized strings will later be entered by Esperantus tool
Ver 1.3 - 19. Jan 2005 - Mike Stone [mstone@kaskaskia.edu] Changed the Answer field
          so that it is unlimited amount of text. To apply new changes simply 
          reinstall the Module - DesktopModules/FAQs/Install.xml  Make sure you 
          DO NOT Uninstall it first or you will loose all data. 
          You DO NOT have to reinstall if you just did a clean install.


Issues and Known problems:
- Tested with Appleseed version 1.2.8.1714 - 25/05/2003
- In the top of file DBPatch.sql is list of DB changes this patch introduces
- Version 1.2 only in english (the code is ready for localization! - Tanks to Marc B!)
- Does not work if module SendThoughts is on same tabpage
- Scrolling problem: When you click for view the answer the page refresh to the 
  top and you have to scroll down to see your text. (same problem exists in original code)


Module settings
---------------
None!

*****************************************************************************
10/27/03-Chris Farrell, chris@cftechconsulting.com

1:  Added support for multi-editors with the FreeTextBox as default

Notes: no required field validator for Answer because the requiredFieldValidator does
not work with the FreeTextBox.
