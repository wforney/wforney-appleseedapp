ServiceItemList
This module uses the Appleseed community WebService. This module list data from 
another Appleseed based portal or the same portal where the module resides.


Credits: Jakob hansen


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
Ver. 1.0 - 21. may 2003 - First realase (BETA) by Jakob Hansen
Ver. 1.1 - 6.  aug 2004 - First production version (No code changes since ver 1.0)


Issues and Known problems:
- Topic serch not supported
- Tested with Appleseed version 1.4.0.1778d - 06/10/2004 - RC4
- Version 1.1 only in english (the code is only partly localized)


Module settings
---------------
This module list data from another Appleseed based portal in a table that looks
similar to the PortalSearch output. Data from the same portal where the module resides
can also be displyed. Various kind of data-filtering, searching and ordering is supported.

To get a quick understanding of how this modules works please add the module to a page
and edit the Module setting (lets pretend you are interrested in localization stuff): 

ServiceItemList Module settings - simple example
------------------------------------------------ 
URL : http://www.Appleseedportal.net/
PortalAlias: <Leave this field empty>
LocalMode : Uncheked
Searchstring : localization 
NOTE: just use the default settings of the rest of the fields!
Press OK and see the resulting list of items in a table. The table should list approx. 
9 items: 4 Announcements, 3 articles and 2 HTML Documents.

The layout of the table can be controlled by settings that start with "Show". 
E.g.: Showcreateddate controls if column Date shold be displyed. Ordering is controlled using 
fields Sortfield and SortDirection.


Description of all settings
---------------------------
URL			: The url of the Appleseed portal that supplies the data.
Portalalias		: Represents the Alias of the site. Must be set for a multiportal Appleseed installation.
Localmode		: When checked data from the local Appleseed database is used. Please still fill out URL.
Moduletype		: All;Announcements;Contacts;Discussion;Events;HtmlModule;Documents;Pictures;Articles;
				  Tasks;FAQs;ComponentModule.
				  Controls which module data is comming from. Default value is "All" which is all modules.
Maxhits			: The maximum numbers of items to display in the table.
Searchstring		: All listed items must include this string. THIS SETTING CAN NOT BE EMPTY! 
				  See search tips below.
Searchfield		: Only this field is searched. E.g.: "Title". Be carefull - you must have db knowledge
			      to set this field.
Sortfield		: ModuleName;Title;CreatedByUser;CreatedDate;TabName. Sorting is always done on the selected field.
Sortdirection	: ASC;DESC. Sort Ascending or Descending.
Mobileonly		: If checked only items for mobiledevices are listed.
Idlist			: Comma separeted list of id's. Here you can enter a list of e.g. modules you want to get 
				  date from, e.g.: 3327,3328,3329. The type of id is controlled by setting Idlisttype
Idlisttype		: Item;Module;Tab. Controls the type of id's in setting Idlist. 
Tag			: Not used (leave the 0).
Target			: Blank;parent;self;top. Default value is "Blank" which will open a new browser.
Showid			: If true ID's are displayed in the lists
Showmodulefriendlyname : If true column X are displayed in the list
Showsearchtitle		: If true column Title are displayed in the list
Showdescription		: If true column Description are displayed in the list
Showcreatedbyuser	: If true column User are displayed in the list
Showcreateddate		: If true column Date are displayed in the list
Showlink		: If true column Link are displayed in the list
Showtabname		: If true column Tab are displayed in the list
Showmoduletitle		: If true column Module are displayed in the list


Search tips for field Searchstring:
- Use AND and NOT
- It is not necessary to type AND between words
- Use '-' in front of words. Example:
  "word1 word2 -word3" and "word1 word2 NOT word3" returns same result
- Use ? as singlecharacter wildcart
- OR is not supported (it is ignored)
- Phrase searching supported using quotes: "this is a phrase"
NOTE: The search code PortalSearch uses are called by module ServiceItemList.
