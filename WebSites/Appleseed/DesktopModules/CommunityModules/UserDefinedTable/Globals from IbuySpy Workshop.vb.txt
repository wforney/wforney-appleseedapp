Imports System.Web
Imports System.Web.Mail
Imports System.Data.SqlClient

Namespace WorkshopHelper

    '*********************************************************************
    '
    ' Globals Module
    ' This module contains global utility functions, constants, and enumerations.
    '
    '*********************************************************************

    Module Globals

        ' returns a string representing the fully qualified url of the page calling this function (not including the page name).
            Public Function GetAbsoluteURL(ByVal RequestObj As HttpRequest, ByVal CallingPageName As String) As String

                Dim strReturnVal As String = LCase(RequestObj.Url.ToString())
                strReturnVal = Left(strReturnVal, InStr(1, strReturnVal, LCase(CallingPageName)) - 1)
                Return strReturnVal

            End Function

            ' returns the relative server path to the root ( ie. /directory/ or / )
            Public Function GetRelativeServerPath(ByVal Request As HttpRequest) As String
                Dim ServerPath As String

                ServerPath = Request.ApplicationPath
                If Mid(ServerPath, Len(ServerPath), 1) <> "/" Then
                    ServerPath = ServerPath & "/"
                End If

                Return ServerPath
            End Function

            ' returns the absolute server path to the root ( ie. D:\Inetpub\wwwroot\directory\ )
            Public Function GetAbsoluteServerPath(ByVal Request As HttpRequest) As String
                Dim strServerPath As String

                strServerPath = Request.MapPath(Request.ApplicationPath)
                If Mid(strServerPath, Len(strServerPath), 1) <> "\" Then
                    strServerPath = strServerPath & "\"
                End If

                GetAbsoluteServerPath = strServerPath
            End Function

            ' returns the domain name of the current request ( ie. www.domain.com or 207.132.12.123 or www.domain.com/directory if subhost )
            Public Function GetURLDomain(ByVal Request As HttpRequest) As String
                Dim DomainName As String
                Dim URL() As String
                Dim intURL As Integer

                URL = Split(Request.Url.ToString(), "/")
                For intURL = 2 To URL.GetUpperBound(0)
                    Select Case URL(intURL).ToLower
                        Case "localhost" ' ignore
                        Case "admin", "desktopmodules", "mobilemodules"
                            Exit For
                        Case Else
                            ' check if filename
                            If InStr(1, URL(intURL), ".aspx") = 0 And InStr(1, URL(intURL), ".ascx") = 0 Then
                                DomainName = DomainName & IIf(DomainName <> "", "/", "") & URL(intURL)
                            Else
                                Exit For
                            End If
                    End Select
                Next intURL

                GetURLDomain = DomainName
            End Function

            ' sends a simple email
            Public Sub SendNotification(ByVal strFrom As String, ByVal strTo As String, ByVal strBcc As String, ByVal strSubject As String, ByVal strBody As String)
                Dim mail As New MailMessage()

                mail.From = strFrom
                mail.To = strTo
                If strBcc <> "" Then
                    mail.Bcc = strBcc
                End If
                mail.Subject = strSubject
                mail.Body = strBody

                Try
                    SmtpMail.Send(mail)
                Catch
                    ' mail configuration problem
                End Try
            End Sub

            ' encodes a URL for posting to an external site
            Public Function HTTPPOSTEncode(ByVal strPost As String) As String
                strPost = Replace(strPost, "\", "")
                strPost = System.Web.HttpUtility.UrlEncode(strPost)
                strPost = Replace(strPost, "%2f", "/")
                HTTPPOSTEncode = strPost
            End Function

            ' retrieves the domain name of the portal ( ie. http://www.domain.com " )
            Public Function GetPortalDomainName(ByVal strPortalAlias As String) As String
                If InStr(1, strPortalAlias, ".") = 0 Then
                    strPortalAlias = "localhost/" & strPortalAlias
                End If
                If InStr(1, strPortalAlias, ",") <> 0 Then
                    strPortalAlias = Left(strPortalAlias, InStr(1, strPortalAlias, ",") - 1)
                End If
                If InStr(1, LCase(strPortalAlias), "http://") = 0 Then
                    strPortalAlias = "http://" & strPortalAlias
                End If
                GetPortalDomainName = strPortalAlias
            End Function

            ' convert datareader to dataset
            Public Function ConvertDataReaderToDataSet(ByVal reader As SqlDataReader) As DataSet

                Dim dataSet As DataSet = New DataSet()

                Dim schemaTable As DataTable = reader.GetSchemaTable()

                Dim dataTable As DataTable = New DataTable()

                Dim intCounter As Integer

                For intCounter = 0 To schemaTable.Rows.Count - 1
                    Dim dataRow As DataRow = schemaTable.Rows(intCounter)
                    Dim columnName As String = CType(dataRow("ColumnName"), String)
                    Dim column As DataColumn = New DataColumn(columnName, CType(dataRow("DataType"), Type))
                    dataTable.Columns.Add(column)
                Next

                dataSet.Tables.Add(dataTable)

                While reader.Read()
                    Dim dataRow As DataRow = dataTable.NewRow()

                    For intCounter = 0 To reader.FieldCount - 1
                        dataRow(intCounter) = reader.GetValue(intCounter)
                    Next

                    dataTable.Rows.Add(dataRow)
                End While

                Return dataSet

            End Function

            ' convert datareader to crosstab dataset
            Public Function BuildCrossTabDataSet(ByVal DataSetName As String, ByVal result As SqlDataReader, ByVal FixedColumns As String, ByVal VariableColumns As String, ByVal KeyColumn As String, ByVal FieldColumn As String, ByVal FieldTypeColumn As String, ByVal StringValueColumn As String, ByVal NumericValueColumn As String) As DataSet

                Dim arrFixedColumns As String()
                Dim arrVariableColumns As String()
                Dim arrField As String()
                Dim FieldName As String
                Dim FieldType As String
                Dim intColumn As Integer
                Dim intKeyColumn As Integer

                ' create dataset
                Dim crosstab As New DataSet(DataSetName)
                crosstab.Namespace = "NetFrameWork"

                ' create table
                Dim tab As New DataTable(DataSetName)

                ' split fixed columns
                arrFixedColumns = FixedColumns.Split(",".ToCharArray())

                ' add fixed columns to table
                For intColumn = LBound(arrFixedColumns) To UBound(arrFixedColumns)
                    arrField = arrFixedColumns(intColumn).Split("|".ToCharArray())
                    Dim col As New DataColumn(arrField(0), System.Type.GetType("System." & arrField(1)))
                    tab.Columns.Add(col)
                Next intColumn

                ' split variable columns
                If VariableColumns <> "" Then
                    arrVariableColumns = VariableColumns.Split(",".ToCharArray())

                    ' add varible columns to table
                    For intColumn = LBound(arrVariableColumns) To UBound(arrVariableColumns)
                        arrField = arrVariableColumns(intColumn).Split("|".ToCharArray())
                        Dim col As New DataColumn(arrField(0), System.Type.GetType("System." & arrField(1)))
                        col.AllowDBNull = True
                        tab.Columns.Add(col)
                    Next intColumn
                End If

                ' add table to dataset
                crosstab.Tables.Add(tab)

                ' add rows to table
                intKeyColumn = -1
                Dim row As DataRow
                While result.Read()
                    ' loop using KeyColumn as control break
                    If result(KeyColumn) <> intKeyColumn Then
                        ' add row
                        If intKeyColumn <> -1 Then
                            tab.Rows.Add(row)
                        End If

                        ' create new row
                        row = tab.NewRow()

                        ' assign fixed column values
                        For intColumn = LBound(arrFixedColumns) To UBound(arrFixedColumns)
                            arrField = arrFixedColumns(intColumn).Split("|".ToCharArray())
                            row(arrField(0)) = result(arrField(0))
                        Next intColumn

                        ' initialize variable column values
                        If VariableColumns <> "" Then
                            For intColumn = LBound(arrVariableColumns) To UBound(arrVariableColumns)
                                arrField = arrVariableColumns(intColumn).Split("|".ToCharArray())
                                Select Case arrField(1)
                                    Case "Decimal"
                                        row(arrField(0)) = 0
                                    Case "String"
                                        row(arrField(0)) = ""
                                End Select
                            Next intColumn
                        End If

                        intKeyColumn = result(KeyColumn)
                    End If

                    ' assign pivot column value
                    If FieldTypeColumn <> "" Then
                        FieldType = result(FieldTypeColumn)
                    Else
                        FieldType = "String"
                    End If
                    Select Case FieldType
                        Case "Decimal" ' decimal
                            row(result(FieldColumn)) = result(NumericValueColumn)
                        Case "String" ' string
                            row(result(FieldColumn)) = result(StringValueColumn)
                    End Select
                End While

                result.Close()

                ' add row
                If intKeyColumn <> -1 Then
                    tab.Rows.Add(row)
                End If

                ' finalize dataset
                crosstab.AcceptChanges()

                ' return the dataset
                Return crosstab

            End Function

            Public Class FileItem
                Private _Value As String
                Private _Text As String

                Public Sub New(ByVal Value As String, ByVal Text As String)
                    _Value = Value
                    _Text = Text
                End Sub

                Public ReadOnly Property Value() As String
                    Get
                        Return _Value
                    End Get
                End Property

                Public ReadOnly Property Text() As String
                    Get
                        Return _Text
                    End Get
                End Property

            End Class

            ' format an address on a single line ( ie. Unit, Street, City, Region, Country, PostalCode )
            Public Function FormatAddress(ByVal Unit As Object, ByVal Street As Object, ByVal City As Object, ByVal Region As Object, ByVal Country As Object, ByVal PostalCode As Object) As String

                Dim strAddress As String = ""

                If Not IsDBNull(Unit) Then
                    If Trim(Unit.ToString()) <> "" Then
                        strAddress += ", " & Unit.ToString
                    End If
                End If
                If Not IsDBNull(Street) Then
                    If Trim(Street.ToString()) <> "" Then
                        strAddress += ", " & Street.ToString
                    End If
                End If
                If Not IsDBNull(City) Then
                    If Trim(City.ToString()) <> "" Then
                        strAddress += ", " & City.ToString
                    End If
                End If
                If Not IsDBNull(Region) Then
                    If Trim(Region.ToString()) <> "" Then
                        strAddress += ", " & Region.ToString
                    End If
                End If
                If Not IsDBNull(Country) Then
                    If Trim(Country.ToString()) <> "" Then
                        strAddress += ", " & Country.ToString
                    End If
                End If
                If Not IsDBNull(PostalCode) Then
                    If Trim(PostalCode.ToString()) <> "" Then
                        strAddress += ", " & PostalCode.ToString
                    End If
                End If
                If Trim(strAddress) <> "" Then
                    strAddress = Mid(strAddress, 3)
                End If

                FormatAddress = strAddress

            End Function

            ' format an email address including link
            Public Function FormatEmail(ByVal Email As Object) As String

                If Not IsDBNull(Email) Then
                    If Trim(Email.ToString()) <> "" Then
                        If InStr(1, Email.ToString(), "@") Then
                            FormatEmail = "<a href=""mailto:" & Email.ToString() & """>" & Email.ToString() & "</a>"
                        Else
                            FormatEmail = Email.ToString()
                        End If
                    End If
                End If

            End Function

            ' format a domain name including link
            Public Function FormatWebsite(ByVal Website As Object) As String

                If Not IsDBNull(Website) Then
                    If Trim(Website.ToString()) <> "" Then
                        If InStr(1, Website.ToString(), ".") Then
                            FormatWebsite = "<a href=""" & IIf(InStr(1, LCase(Website.ToString()), "http://"), "", "http://") & Website.ToString() & """>" & Website.ToString() & "</a>"
                        Else
                            FormatWebsite = Website.ToString()
                        End If
                    End If
                End If

            End Function

    End Module

End Namespace