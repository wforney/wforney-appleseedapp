<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output indent="no" method="html"/>
<xsl:template match="/">
    <h2>My private CD Collection</h2>
    <table border="1">
    <tr bgcolor="#FFFd32">
      <th align="left">Title</th>
      <th align="left">Artist</th>
      <th align="left"> </th>
    </tr>
    <xsl:for-each select="UserDefinedTable/Row">
    <tr>
      <td><xsl:value-of select="title"/></td>
      <td><xsl:value-of select="artist"/></td>
      <td><A><xsl:attribute name="href"><xsl:value-of select="EditURL"/></xsl:attribute>EDIT</A></td> <!-- This is the way to make an A link (see http://www.4guysfromrolla.com/webtech/060700-2.3.shtml) -->
    </tr>
    </xsl:for-each>
    </table>
</xsl:template>
</xsl:stylesheet>