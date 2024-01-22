Imports System.IO
Imports System.Data.OleDb

Public Class MyDb
    Class QueryInfo
        Public table As DataTable = Nothing
        Public rowsCount As Integer = 0
        Public firstRow As DataRow = Nothing
        Public lastRow As DataRow = Nothing
        Public err As String = ""
        Function rows(index) As DataRow
            Return table.Rows(index)
        End Function
        Sub New(tbl As DataTable)
            If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                table = tbl
                rowsCount = table.Rows.Count
                firstRow = table.Rows(0)
                lastRow = table.Rows(table.Rows.Count - 1)
            End If
        End Sub
    End Class

    Public con As OleDbConnection = Nothing
    Public providerAndEngine As String = "Provider=Microsoft.Jet.OLEDB.4.0;Jet OLEDB:Database Password = 'drihnz';Data Source="
    Sub New(Optional ByVal dbPath As String = "")
        If dbPath <> "" Then
            Me.con = New OleDbConnection(providerAndEngine & dbPath)
            Me.con.Open()
        End If
    End Sub
    Public Function createAccessDatabase(ByVal dbPath As String, Optional deletIfExit As Boolean = False) As Boolean
        Dim bAns As Boolean
        Dim cat As New ADOX.Catalog()
        If deletIfExit Then
            Try
                IO.File.Delete(dbPath)
            Catch ex As Exception
                Return False
            End Try
        End If
        Try
            Dim sCreateString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & dbPath & ";Jet OLEDB:Engine Type=5"
            cat.Create(sCreateString)
            Me.con = New OleDbConnection(sCreateString)
            Me.con.Open()
            bAns = True
        Catch Excep As System.Runtime.InteropServices.COMException
            bAns = False
        Finally
            cat = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
        Return bAns
    End Function
    Public Function select_(ByVal Sql As String, Optional ByVal path As String = "") As QueryInfo
        Dim adapter As OleDbDataAdapter
        Dim data As New DataTable("Data")
        If Me.con IsNot Nothing Then
            adapter = New OleDbDataAdapter(Sql, con)
        Else
            Dim tmpCon As New OleDbConnection(providerAndEngine & path)
            adapter = New OleDbDataAdapter(Sql, tmpCon)
        End If
        Try
            adapter.Fill(data)
        Catch ex As Exception
            Dim ret As New QueryInfo(data)
            ret.err = ex.Message
            adapter.Dispose()
            Return ret
        End Try
        adapter.Dispose()
        Return New QueryInfo(data)
    End Function
    Function query(ByVal sql As String) As String
        Try
            Dim res As Integer
            Dim cmd As OleDbCommand
            cmd = New OleDbCommand(sql, Me.con)
            res = cmd.ExecuteNonQuery
            Return res
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
    Public Function closeConnections() As Boolean
        Dim res As Boolean = False
        Try
            If Me.con.State = ConnectionState.Open Then
                con.Close()
                GC.Collect()
                GC.WaitForPendingFinalizers()
                res = True
            End If
        Catch ex As Exception
        End Try
        Return res
    End Function

    Public Function _FileInUsed(ByVal FilePath As String, ByVal displayMsg As Boolean) As Boolean
        Try
            Dim FS As IO.FileStream = IO.File.Open(FilePath, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.None)
            FS.Close()
            FS.Dispose()
            FS = Nothing
            Return False
        Catch ioEX As IOException
            If displayMsg Then
                MsgBox("File " & vbNewLine & FilePath & vbNewLine & " is In Use!", MsgBoxStyle.Critical, "")
            End If
            Return True
        Catch ex As Exception
            If displayMsg Then
                MsgBox("Unknown error occured!", MsgBoxStyle.Critical, "")
            End If
            Return True
        End Try
    End Function
End Class
