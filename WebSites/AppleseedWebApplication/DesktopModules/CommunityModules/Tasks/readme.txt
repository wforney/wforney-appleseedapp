Task Module - Task list tool

This module renders a list of tasks. Individual tasks can be viewed by clicking the
task title. Tasks includes an edit page, which allows authorized users to edit the
Tasks data stored in the SQL database


Credits: ??? (the guy did not write his name in the original code)


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
Ver. 1.0 - 21. dec 2002 - First realase by Jakob Hansen
Ver. 1.1 - 22. dec 2002 - Renamed files and fixed problem in Tasks_DBPatch.sql (Jakob)
Ver. 1.2 - 29. april 2003 - Updated to follow "Appleseed best practices"
Ver. 1.3 - 18. may 2004 - Added last changed column
Ver. 1.4 - 27. may 2004 - Chris Farrell, chris@cftechconsulting:
  -Fixed "Improper Identity Seed for ItemID in table rb_Tasks".
   Updated SQL install file and provided update.sql code.
  -TasksView.aspx was not wrapping Description field text.  Fixed,
   converted HtmlControls.HtmlGenericControl to a ASP label.
  -Added support for WYSIWYG HTML text editors instead of asp:textbox
   for the "descriptionfield".
  -TasksView.aspx.  Removed the "if(ItemID != 0)" block.  There was an
   improper coding of the Identity seed for ItemID so valid items might have a
   ItemID = 0. There is no reason that the TasksView.aspx page should not display 
   where ItemID = 0 so this helps offset the hassle of this bug for users who already 
   have data present.
Ver. 1.5 - 19 jan 2005 - by Mike Stone 
  - Changed Description field to ntext which removes the 3000 char limit
  - To Apply these changes you must re install the Tasks module. DO NOT
    uninstall it first or you will loose all our tasks entries. Just go to
    the add module and put in DesktopModules/Tasks/Install.xml and your done.
    You DO NOT have to reinstall if you just did a clean install. 
Ver. 1.6 - 19 June 2005 - by Jes1111
  - modified to add Mod_Tasks.css file to the page automatically
  
Issues and Known problems:
- Tested with Appleseed version 1.4.0.1764h - 06/06/04 by Jakob Hansen
- Version 1.0 only in english (the code is only partly localized)



Background

This module was inspired by the Tasks list in the Microsoft SharePoint Team Services.
While much of the code is based on the Events module that comes with the IBuySpy Portal,
there are some additional features that could be extended to other modules.

- The Tasks list can be sorted by clicking on the column header
- Clicking a column header second time reverses the sortorder
- The Tasks edit page utilizes a pop-up calendar to allow dates to be selected
