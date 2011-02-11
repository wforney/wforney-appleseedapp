<?xml version="1.0" encoding="ISO-8859-1"?>
<?xmlspysamplexml C:\Documents and Settings\rsiera\Bureaublad\Appleseed css\test.xml?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- 

Good to know:

 * How to insert/use Esperantus Localization in your output:  
   Use    : @@localize.ESPERANTUSKEY@@  (case sensitive !)
   Example: @@localize.BACK@@

 * How to insert href for sorting link:  
   Use    : @@sort.FIELDNAME@@   (case sensitive !)
   Example: @@sort.title@@

 * How to make an A link: see http://www.4guysfromrolla.com/webtech/060700-2.3.shtml

* How to implement multilingual XLS:
  Here is a site with several methods (not all of them appropriat in Appleseed environement):
  http://www.topxml.com/xsltStylesheets/xslt_multilingual.asp
  In this example we use method 4

-->
	<!-- START - DON'T REMOVE OR EDIT THIS -->
	<xsl:output indent="no" method="html"/>
	<xsl:key name="preg" match="Row" use="@ID"/>
	<xsl:template match="/UserDefinedTable">
		<xsl:if test="./@ShowDetail = 0 ">
			<xsl:call-template name="List" />
		</xsl:if>
		<xsl:if test="./@ShowDetail > 0 ">
			<xsl:for-each select="key('preg',./@ShowDetail)">
				<xsl:call-template name="Detail" />
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	<!-- END - DON'T REMOVE OR EDIT THIS -->
	
	<xsl:template name="List">
		<!-- START - LIST VIEW TEMPATE -->
		<h2>My CD Collection</h2>
		<table border="1">
			<tr class="Grid_Header">
				<th align="left">
					<A href="@@sort.title@@">
				<xsl:choose>
				<xsl:when test="lang('nl')">Titel</xsl:when>
				<xsl:when test="lang('fr')">Titre</xsl:when>
				<xsl:otherwise>Title</xsl:otherwise>
			</xsl:choose>
			</A> @@imgsortorder.title@@
				</th>
				<th align="left">
					<A href="@@sort.artist@@"><xsl:choose>
				<xsl:when test="lang('nl')">Uitvoerder</xsl:when>
				<xsl:when test="lang('fr')">Artiste</xsl:when>
				<xsl:otherwise>Performer</xsl:otherwise>
			</xsl:choose></A> @@imgsortorder.artist@@
				</th>
				<th align="left"> </th>
				<th align="left"> </th>
			</tr>
			<xsl:for-each select="Row">
				<tr class="Grid_Item Normal">
					<td>
						<xsl:value-of select="title"/>
					</td>
					<td>
						<xsl:value-of select="artist"/>
					</td>
					<td>
						<A>
							<xsl:attribute name="href"><xsl:value-of select="EditURL"/></xsl:attribute>EDIT</A>
					</td>
					<td>
						<A>
							<xsl:attribute name="href"><xsl:value-of select="ShowDetailURL"/></xsl:attribute>SHOW DETAIL</A>
					</td>
				</tr>
			</xsl:for-each>
		</table>
		<!-- END - LIST VIEW TEMPATE -->
	</xsl:template>
	<xsl:template name="Detail-EN">
		<!-- START - DETAIL VIEW TEMPATE -->
		<!-- Place your template for Detail view of a record here -->
		<h2>Title: <xsl:value-of select="title"/>
		</h2>
		<strong>Artist:</strong>
		<xsl:value-of select="artist"/>
		<p>
			<a href="javascript:history.go(-1)">@@localize.back@@</a>
		</p>
		<!-- END - DETAIL VIEW TEMPATE -->
	</xsl:template>
</xsl:stylesheet>
