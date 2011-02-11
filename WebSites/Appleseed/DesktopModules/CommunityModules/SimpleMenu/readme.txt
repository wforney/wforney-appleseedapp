SimpleMenu - module by mario@hartmann.net

Another Appleseed desktop module - more to download on http://www.Appleseedportal.net

INSTALL 
1. Go to Admin all and to add module definition. 
2. Point to install.xml install file
3. Add the module to a page
4. Edit module settings: 
5. Use it! 8) 


HISTORY
SimpleMenu module ver. 1.5	 - 16/06/2005 - 'Tab change to Page' naming conventions applied
SimpleMenu module Ver. 1.5       - 19/01/2005 - SimpleMenu upgraded to Appleseed 1.5 standards
SimpleMenu module Ver. 1.0.1     - 07/06/2003 - SimpleMenu now is restored in Appleseed.dll to avoid conflicts 
SimpleMenu module Ver. 1.0.0     - 04/06/2003 - SimpleMenu now is in its own project 
SimpleMenu module Ver. 0.11 beta - 14/10/2003 - added dynamic loading of MenuTypes
SimpleMenu module Ver. 0.10 beta - 02/10/2003 - added SolpartNavigation menucontrol
SimpleMenu module Ver. 0.9 beta  - 16/07/2003 - added horizontal alignment capability
SimpleMenu module Ver. 0.8 beta  - 16/07/2003 - added ItemMenu, modified HWMenu for using css-StyleSheets.
SimpleMenu module Ver. 0.5 beta  - 27/05/2003 - initial release by Mario Hartmann

ISSUES AND KNOWN PROBLEMS:
- there are only one HWMenu dhtml-menu allowed per page
- the static menu is just showing one level
- in case you use the solpart menu, the default style definition 
  for the 'TD' element in every pane is responsible for false rendering 
  of the the toplevel of menu styles
- Tested with Appleseed version 1.5.0.1789


MODULE SETTINGS
---------------
sm_MenuBindingType		- the 'BindingType' of the menu.
sm_ParentPageID			- the 'TabID' of the parent menu tab.
sm_Menu_RepeatDirection		- the direction of the menu (horizonta/vertical).
sm_MenuType			- describes which type of  menu is used. 
				--StaticMenu (one-level)
				--StaticItemMenu (one-level with ShopMenu behavior)
				--DHTMLMenu (multilevel) 
				--DHTMLItemMenu (multilevel with ShopMenu behavior)
				--SolPartMenu uses the DHTML Menu from  Solution Partners 'www.solpart.com'


STYLE SETTINGS
--------------
the module (StaticMenu,StaticItemMenu) uses following styles.
- .sm_SimpleMenu
- .sm_SelectedTab
- .sm_OtherSubTabs
- .sm_OtherSubTabsAlt
- .sm_Header
- .sm_Footer

the module (dhtml 'HWMenuTopStyle' menu) uses following styles.
- .HWMenuItem
- .HWMenuHiItem
- .HWMenuSubItem
- .HWMenuHiSubItem

the module (dhtml 'HWMenuLeftStyle' menu) uses following styles.
- .MenuItem
- .MenuHiItem
- .MenuSubItem
- .MenuHiSubItem

the module (dhtml 'SolpartMenu' menu) uses folowing styles.
MenuCSS-MenuDefaultItemHighLight="spm_DefaultItemHighlight"
MenuCSS-MenuDefaultItem="spm_DefaultItem"
MenuCSS-MenuArrow="spm_MenuArrow"
MenuCSS-MenuIcon="spm_MenuIcon"
MenuCSS-RootMenuArrow="spm_RootMenuArrow"
MenuCSS-MenuBreak="spm_MenuBreak"
MenuCSS-SubMenu="spm_SubMenu"
MenuCSS-MenuContainer="spm_MenuContainer"
MenuCSS-MenuItem="spm_MenuItem"
MenuCSS-MenuItemSel="spm_MenuItemSel"
MenuCSS-MenuBar="spm_MenuBar"

the module (dhtml 'Tabs' menu) uses folowing styles.
- Tabs
- SubTabs
- SelectedTab

EXAMPLE FOR THE STYLESHEET (included in >>~/Design/Themes/Default/default.css<<)

(simple menu are not in default.css)

/* =================================
   SimpleMenu Module - static styles
   ================================= */
.sm_SimpleMenu			{FONT-SIZE: 12px;COLOR:#ffcc00;background-color:#ffcc00;BORDER-RIGHT: black 1px solid; BORDER-TOP: black 1px solid;BORDER-LEFT: black 1px solid; WIDTH: 100%; BORDER-BOTTOM: black 1px solid }
.sm_SimpleMenu A:hover	{FONT-SIZE: 12px;COLOR:#ffcc00;TEXT-DECORATION: none }
.sm_SelectedTab			{FONT-SIZE: 12px;COLOR:#ffcc00;FONT-WEIGHT: bold; background-color: dodgerblue;}
.sm_OtherSubTabs		{FONT-SIZE: 12px;COLOR:#ffcc00;background-color: dodgerblue }
.sm_OtherSubTabsAlt		{FONT-SIZE: 12px;COLOR:#ffcc00;background-color: dodgerblue }
.sm_Header				{FONT-SIZE: 12px;COLOR:#000000;BORDER-BOTTOM: black 2px solid }
.sm_Header A:link, .sm_Header A:visited	{FONT-SIZE: 12px;COLOR:#000000;}
.sm_Header A:hover		{FONT-SIZE: 12px;COLOR:#000000;FONT-WEIGHT: bold;}
.sm_Footer				{FONT-SIZE: 12px;COLOR:#000000;BORDER-TOP: black 2px solid;}
/* ================================ */

