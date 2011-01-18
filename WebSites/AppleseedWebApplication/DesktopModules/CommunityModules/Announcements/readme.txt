Announcements Module

This module renders a list of Announcements.


Credits: ??? 


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
Ver. 1.0 - 
Ver. 1.1 - moved module to a seperate folder
Ver. 1.2 - Added Paging.  Chris Farrell, chris@cftechconsulting.com.  05/09/2005(5. september)


Issues and Known problems:
- Tested with Appleseed version 1.2.8.1634 - 09/11/03 (11. september)
- If you Uninstall the 'Annoucements' module the 'UnusedModule' entry in DB have to be deleted too.
	(delete [rb_modules] where ModuleID=0 and ModuleDefID=1 and ModuleTitle='Unused Module')
	I am not sure where tis entry is used for, so i decided to MOT delete it while updating the db.
	



