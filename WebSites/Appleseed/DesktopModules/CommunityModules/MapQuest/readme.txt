MapQuest - Map module
The following module leverages the MapQuest LinkFree web service. 
You can read a description of the service by browsing to 
http://www.mapquest.com/solutions/product.adp?page=linkfree . 
This solution goes beyond the "smart hyperlinking" concept described by 
allowing you to show the actual map on your own site. This is accomplished 
by scraping the URL of the Response to extract the map image link which can 
then be used to display the map image on your site. The solution also builds
a URL which leverages the "Get Directions" functionality of MapQuest (US only -
you must edit file MapQuest.ascx: delete the comments <!-- -->). 


Credits: Shaun Walker


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
Ver. 1.0 - 21. jan 2003 - Moved from original IBS and VB into Appleseed and C# by Jakob Hansen
Ver. 1.1 - 26. april 2003 - Updated to follow "Appleseed best practices"


Issues and Known problems:
- Tested with Appleseed version 1.2.8.1627 - 25/04/2003
- Problems when behind a firewall. See bug reported on source:
  http://sourceforge.net/tracker/?func=detail&aid=738196&group_id=66837&atid=515929


Module settings (example):
Location: "Jakob Hansen lives here"
City	: Helsingoer
PostalCode : 3000
Country	: DK
Region	: (put state here if it's an US address)
ShowAddres : Yes/No to show address as text above the map
ShowMap	: If no only a link to the map is displayed
Target	: blank   (select between: blank, parent, self, top)
