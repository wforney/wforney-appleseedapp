Survey - a vote module
Typical vote/survey module. Go to www.sysdatanet.com to see this module

Credits: www.sysdatanet.com

Another Appleseed desktop module - more to download on http://www.Appleseedportal.net


INSTALL
1. Go to Admin all and to add module definition. 
2. Point to install.xml install file
3. Add the module to a page
4. Edit module settings: See below
5. Create an survey - See instructions on www.sysdatanet.com
6. Use it! ;o) 
Note: The module is automatically installed when you install Appleseed.
The install procedure is only required if you deleted the module in Admin all


Module settings
---------------
VoteDayPeriod: Default value is 7. 
    This means that a user can not vote again until 7 days has passed. (we are using cookies)
Test: Default value is 0.
    Set to 1 to not display the percentage bar. Then you as webmaster/admin can vote as
    many times you like (disables the VoteDayPeriod). Please set back to 0 when the 
    percentage bar should be displayed


HISTORY
Ver. 1.0 - 22. jan 2003 - First realase by Jakob Hansen
Ver. 1.1 - 08. may 2003 - Updated to follow "Appleseed best practices"
Ver. 1.2 - 25. may 2003 - FAQs are now 100% ready for localization. 
           Localized strings will later be entered by Esperantus tool


Issues and Known problems:
- Tested with Appleseed version 1.2.8.1714 - 25/05/2003
- Version 1.2 only in english (the code is ready for localization! - Tanks to Marc B!)
- Nothing displyed for an option until the first vote has been issued


Note: (jes1111)
Code now modified to add Mod_Survey.css to page automatically
/* ===================================================
    CSS styles for module Survey in Appleseed portal
    Copy these classes into your .css in folder Themes
   ===================================================
*/   


.SurveyQuestion
{

	color: darkred;
}

.SurveyPanel
{
	color: silver;
	background-color: silver;
}

.SurveyOption
{
	color: black;
}

.SurveyButton
{
	color: darkred;
	font-weight: bold;
}
