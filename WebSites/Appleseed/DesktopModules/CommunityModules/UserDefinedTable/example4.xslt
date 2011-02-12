<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<!-- 

Good to know:

 * How to insert/use Espanantus Localization in your output:  
   Use    : @@localize.ESPARANTUSKEY@@  (case sensitive !)
   Example: @@localize.BACK@@

 * How to insert href for sorting link:  
   Use    : @@sort.FIELDNAME@@   (case sensitive !)
   Example: @@sort.title@@

 * How to make an A link: see http://www.4guysfromrolla.com/webtech/060700-2.3.shtml

-->
<!-- START - DON'T REMOVE OR EDIT THIS -->
	<xsl:output indent="no" method="html"/>
	<xsl:key name="preg" match="Row" use="@ID"/>

	<xsl:template match="/UserDefinedTable">
		<xsl:if test="./@ShowDetail = 0 ">
			<xsl:call-template name="List">
			</xsl:call-template>
		</xsl:if>
		<xsl:if test="./@ShowDetail > 0 ">
			 <xsl:for-each select="key('preg',./@ShowDetail)">
				 <xsl:call-template name="Detail">
				 </xsl:call-template>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
<!-- END - DON'T REMOVE OR EDIT THIS -->

	<xsl:template name="List">
	<!-- START - LIST VIEW TEMPATE -->
		<h2>My CD Collection</h2>
		<table border="1">
			<tr bgcolor="#9acd32">
				<th align="left">
					<A href="@@sort.title@@">Title</A>
				</th>
				<th align="left">
					<A href="@@sort.artist@@">Artist</A>
				</th>
				<th align="left"> </th>
				<th align="left"> </th>
			</tr>
			<xsl:for-each select="Row">
				<tr>
					<td>
						<xsl:value-of select="title"/>
					</td>
					<td>
						<xsl:value-of select="artist"/>
					</td>
					<td>
						<A><xsl:attribute name="href"><xsl:value-of select="EditURL"/></xsl:attribute>EDIT</A>
					</td>
					<td>
						<A><xsl:attribute name="href"><xsl:value-of select="ShowDetailURL"/></xsl:attribute>SHOW DETAIL</A>
					</td>
				</tr>
			</xsl:for-each>
		</table>
	<!-- END - LIST VIEW TEMPATE -->
	</xsl:template>

	<xsl:template name="Detail">
	<!-- START - DETAIL VIEW TEMPATE -->
	<!-- Place your template for Detail view of a record here -->
			<h2>Title: <xsl:value-of select="title" /></h2>
			<strong>Artist:</strong> <xsl:value-of select="artist" />
			<p><a href="javascript:history.go(-1)">@@localize.back@@</a></p>
	<!-- END - DETAIL VIEW TEMPATE -->
	</xsl:template>	

</xsl:stylesheet>
