Quote
Display a random text string, e.g.: "Not all who wander are lost -- Tolkien".
The text is read from a file or entered by the user. It is possible to select other 
files than the provided demo file. You can control the size of the text and if the
the text should be in italic and/or bold. A set of userdefined start and end html tags 
are provided for customizing.


Credits: Community Starter Kit.


Another Appleseed desktop module - more to download on http://www.Appleseedportal.net


INSTALL
1. Go to Admin all and to add module definition. 
2. Point to install.xml install file
3. Add the module to a page
4. Edit module settings: See below
5. Use it! ;o) 


HISTORY
Ver. 1.0 - 23. feb 2005 - First realase by Jakob Hansen


Issues and Known problems:
- Tested with Appleseed version 1.5.0.1789h - 22/01/2005
- Module settings are not localized.


Module settings
---------------
Quote source?: Default value is "File". Get quotes from a file or display the text from field My Quote
Quote file:    Default value is "demo.quote". The name of the file containing quotes. The file extension must be ".quote".
My Quote:      Enter any quote here and set Quote source to My Quote

Text size:          Text size of the quote text. The 6 build-in heading sizes (HTML tag <H1>,<H2>,etc)
Display in italic?: Display all the quote text in italic style (HTML tag <i>)
Display in bold?:   Display all the quote text in bold/fat letters (HTML tag <b>)

Start tag: Enter any special customizing HTML start tag here, e.g. a marquee tag make the text scroll.
End tag:   Must correspond to the Start tag


QuoteFileFolder
---------------
Quote files are default in the same folder as the module Quote.ascx file. But this can be changed:

All quote files (*.quote) placed in the folder "QuoteFileFolder" specified in the web.config 
will show up in the dropdown list. Add <add key="QuoteFileFolder" value="~/Portals/_Appleseed/YourFolder/" />
in section <appSettings file="Appleseed.config">. 
