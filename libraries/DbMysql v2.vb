Imports MySql.Data.MySqlClient
Imports System.Windows.Forms
''' <summary>
''' Nagkakaroon ng error sa Select query using MySqlDataAdapter or DataReader. Kaya Dapat Seperate lang yung Connection ng pag select Query.
''' </summary>
Public Class DbMysql
   Public connection As MySqlConnection
   Public tryOpenConnectionAttempCount As Integer = 10
   Public tryCloseConnectionAttempCount As Integer = 10
   Public connProp As ConnectionProp
   Public retainOpenConnection As Boolean = False
   Sub New(connectionProp As ConnectionProp)
      Me.connProp = connectionProp
      Me.connection = New MySqlConnection(connectionProp.ConnectionString)
   End Sub

   Enum OpenStat As Integer
      success = 1
      FAILED = 2
   End Enum

   Enum CloseStat As Integer
      success = 3
      RETAINED_open = 4
      FAILED = 5
   End Enum
   Shared Sub AddLog(str As String)
      Dim appPath As String = IO.Path.Combine(Application.StartupPath, "MySqlLogs")
      Dim fileName As String = Format(Now, "yyyyMMdd") & ".txt"
      Dim filePath As String = IO.Path.Combine(appPath, fileName)
      Dim createHeader As Boolean = False
      If Not IO.Directory.Exists(appPath) Then MkDir(appPath)
      If Not IO.File.Exists(filePath) Then createHeader = True
      Using wr As New IO.StreamWriter(filePath, True)
         If createHeader Then
            wr.WriteLine("========This file will be deleted after 20 days from the time its created========")
         End If
         wr.WriteLine(str)
      End Using
      Dim dateForDelete As Date = Now.AddDays(-20)
      dateForDelete = dateForDelete.AddMinutes(-3)
      Dim keepTheseFiles As New List(Of String)
      While dateForDelete < Now
         Dim toKeepFileName As String = Format(dateForDelete, "yyyyMMdd") & ".txt"
         Dim toKeepFilePath As String = IO.Path.Combine(appPath, toKeepFileName)
         If IO.File.Exists(toKeepFilePath) Then
            keepTheseFiles.Add(toKeepFilePath)
         End If
         dateForDelete = dateForDelete.AddDays(1)
      End While
      For Each file As String In IO.Directory.GetFiles(appPath)
         If Not keepTheseFiles.Contains(file) Then
            Try
               IO.File.Delete(file)
            Catch ex As Exception
            End Try
         End If
      Next
   End Sub

   Shared Sub AddLog(ex As Exception)
      Dim appPath As String = IO.Path.Combine(Application.StartupPath, "MySqlLogs")
      Dim fileName As String = Format(Now, "yyyyMMdd") & ".txt"
      Dim filePath As String = IO.Path.Combine(appPath, fileName)
      Dim createHeader As Boolean = False
      If Not IO.Directory.Exists(appPath) Then MkDir(appPath)
      If Not IO.File.Exists(filePath) Then createHeader = True
      Using wr As New IO.StreamWriter(filePath, True)
         If createHeader Then
            wr.WriteLine("========This file will be deleted after 20 days from the time its created========")
         End If
         wr.WriteLine("<message>")
         wr.WriteLine(ex.Message)
         wr.WriteLine("----TRACE----")
         wr.WriteLine(ex.StackTrace)
         wr.WriteLine("</message>")
      End Using
      Dim dateForDelete As Date = Now.AddDays(-20)
      dateForDelete = dateForDelete.AddMinutes(-3)
      Dim keepTheseFiles As New List(Of String)
      While dateForDelete < Now
         Dim toKeepFileName As String = Format(dateForDelete, "yyyyMMdd") & ".txt"
         Dim toKeepFilePath As String = IO.Path.Combine(appPath, toKeepFileName)
         If IO.File.Exists(toKeepFilePath) Then
            keepTheseFiles.Add(toKeepFilePath)
         End If
         dateForDelete = dateForDelete.AddDays(1)
      End While
      For Each file As String In IO.Directory.GetFiles(appPath)
         If Not keepTheseFiles.Contains(file) Then
            Try
               IO.File.Delete(file)
            Catch exexp As Exception
            End Try
         End If
      Next
   End Sub

   Function TryOpenConnection(Optional specifyConnection As MySqlConnection = Nothing) As OpenStat
      If specifyConnection Is Nothing Then
         specifyConnection = connection
      End If
      Dim tryCount As Integer = 0
      While Not specifyConnection.State = ConnectionState.Open
         tryCount += 1
         Try
            specifyConnection.Open()
            Return OpenStat.success
         Catch ex As Exception
            If tryCount >= tryOpenConnectionAttempCount Then
               AddLog("Reached the maximum retry for opening connection")
               AddLog(ex)
               Exit While
            End If
         End Try
         Application.DoEvents()
      End While
      Return OpenStat.FAILED
   End Function

   Function TryCloseConncetion(Optional specifyConnection As MySqlConnection = Nothing) As CloseStat
      If specifyConnection Is Nothing Then
         If Not retainOpenConnection Then
            Dim tryCount As Integer = 0
            While connection.State = ConnectionState.Open
               Try
                  tryCount += 1
                  connection.Close()
                  connection.Dispose()
                  GC.Collect()
                  GC.WaitForPendingFinalizers()
                  Return CloseStat.success
               Catch ex As Exception
                  If tryCount >= tryCloseConnectionAttempCount Then
                     AddLog("Reached the maximum retry for closing connection")
                     AddLog(ex)
                     Exit While
                  End If
               End Try
               Application.DoEvents()
            End While
         Else
            Return CloseStat.RETAINED_open
         End If
      Else
         Dim tryCount As Integer = 0
         While specifyConnection.State = ConnectionState.Open
            Try
               tryCount += 1
               specifyConnection.Close()
               specifyConnection.Dispose()
               GC.Collect()
               GC.WaitForPendingFinalizers()
               Return CloseStat.success
            Catch ex As Exception
               If tryCount >= tryCloseConnectionAttempCount Then
                  AddLog("Reached the maximum retry for closing connection")
                  AddLog(ex)
                  Exit While
               End If
            End Try
            Application.DoEvents()
         End While
      End If
      Return CloseStat.FAILED
   End Function

   Function ShowDatabasses() As List(Of String)
      Dim res As New List(Of String)
      If TryOpenConnection() Then
         Using command As New MySqlCommand
            command.Connection = Me.connection
            command.CommandText = "SHOW DATABASES;"
            Using reader As MySqlDataReader = command.ExecuteReader
               If reader.HasRows Then
                  While reader.Read
                     res.Add(reader.GetString(0))
                  End While
               End If
            End Using
         End Using
         Me.connection.Close()
         GC.Collect()
         GC.WaitForPendingFinalizers()
      End If
      Return res
   End Function
   Function ShowTables() As List(Of String)
      Dim res As New List(Of String)
      If TryOpenConnection() Then
         Using command As New MySqlCommand
            command.Connection = Me.connection
            command.CommandText = "SHOW TABLES;"
            Using reader As MySqlDataReader = command.ExecuteReader
               If reader.HasRows Then
                  While reader.Read
                     res.Add(reader.GetString(0))
                  End While
               End If
            End Using
         End Using
         Me.connection.Close()
         GC.Collect()
         GC.WaitForPendingFinalizers()
      End If
      Return res
   End Function



   ''' <summary>
   ''' 'Dim sqlFileTbl = .myDb.SelectQuery("SELECT * from file_tbl WHERE id=@id", {New String() {"id", fileID}})
   ''' </summary>
   ''' <param name="sql"></param>
   ''' <param name="bindParams"></param>
   ''' <returns></returns>
   Function SelectQuery(sql As String, ParamArray bindParams() As Array) As ReturnStatement
      Dim newCon As New MySqlConnection(connProp.ConnectionString)
      TryOpenConnection(newCon)
      Using cmd As New MySqlCommand(sql, newCon)
         If bindParams.Length > 0 Then
            For Each i As Array In bindParams
               cmd.Parameters.AddWithValue(i(0), i(1))
            Next
         End If
         Try
            Using adapter As New MySqlDataAdapter(cmd)
               Dim tbl As New DataTable
               adapter.Fill(tbl)
               TryCloseConncetion(newCon)
               Return New ReturnStatement("okay", True, tbl.Rows.Count, tbl)
            End Using
         Catch ex As Exception
            AddLog(ex)
            Dim res As New ReturnStatement(ex.Message) With {
               .Exception = ex
            }
            Return res
         Finally
            TryCloseConncetion(newCon)
         End Try
      End Using
   End Function

   Function SelectQuery(sql As String, Optional param As Dictionary(Of String, Object) = Nothing) As ReturnStatement
      Dim newCon As New MySqlConnection(connProp.ConnectionString)
      TryOpenConnection(newCon)
      Using cmd As New MySqlCommand(sql, newCon)
         If param IsNot Nothing Then
            For Each i As KeyValuePair(Of String, Object) In param
               cmd.Parameters.AddWithValue(i.Key, i.Value)
            Next
         End If
         Try
            Using adapter As New MySqlDataAdapter(cmd)
               Dim tbl As New DataTable
               adapter.Fill(tbl)
               TryCloseConncetion(newCon)
               Return New ReturnStatement("okay", True, tbl.Rows.Count, tbl)
            End Using
         Catch ex As Exception
            AddLog(ex)
            Dim res As New ReturnStatement(ex.Message) With {
               .Exception = ex
            }
            Return res
         Finally
            TryCloseConncetion(newCon)
         End Try
      End Using
   End Function

   'example
   'Dim a As New DbMysql(con)
   'Dim par As New List(Of Array)
   'par.Add({"fileName", "aldrin.xls"})
   'par.Add({"fileNameFullPath", "D://aldrin.xls"})
   'par.Add({"projID", 1})
   'par.Add({"fileDate", "2022-03-02 12:34:00"})
   'par.Add({"totalRecords", 21})
   'par.Add({"totalUploaded", 21})
   'par.Add({"completed", "YES"})
   'Dim b = a.Insert("files", par.ToArray)
   Function InsertToTable(tableNme As String, ParamArray bindParams() As Array) As ReturnStatement
      Dim extColumns As String = ""
      Dim extValues As String = ""
      If bindParams.Length > 0 Then
         For Each i As Array In bindParams
            extColumns = String.Concat(extColumns, ",", i(0))
            extValues = String.Concat(extValues, ",@", i(0))
         Next
      End If
      extColumns = extColumns.Trim(",")
      extValues = extValues.Trim(",")
      Dim sql As String = String.Concat("INSERT INTO ", tableNme, " (", extColumns, ") values (", extValues, "); select last_insert_id() as id;")
      Dim res = Me.SelectQuery(sql, bindParams)
      If res.isSucces And res.tbl_rowCount > 0 Then
         Return New ReturnStatement("okay", True, 1, Nothing, res.tbl)
      Else
         Return New ReturnStatement(res.message)
      End If
   End Function

   Function InsertToTable2(tableNme As String, Optional param As Dictionary(Of String, Object) = Nothing) As ReturnStatement
      Dim extColumns As String = ""
      Dim extValues As String = ""
      If param IsNot Nothing Then
         For Each i As KeyValuePair(Of String, Object) In param
            extColumns = String.Concat(extColumns, ",", i.Key)
            extValues = String.Concat(extValues, ",@", i.Key)
         Next
      End If
      extColumns = extColumns.Trim(",")
      extValues = extValues.Trim(",")
      Dim sql As String = String.Concat("INSERT INTO ", tableNme, " (", extColumns, ") values (", extValues, "); select last_insert_id() as id;")
      Dim res = Me.SelectQuery(sql, param)
      If res.isSucces And res.tbl_rowCount > 0 Then
         Return New ReturnStatement("okay", True, 1, Nothing, res.tbl)
      Else
         Return New ReturnStatement(res.message)
      End If
   End Function

   'res = .myDb.NonQuery(String.Concat("Update investigate_tbl set allocate_user_id=@allocate_user_id, upd_process_id=@upd_process_id, state=@state WHERE id=@id"),
   '{New String() {"id", .getOrSet_Record},
   'New String() {"allocate_user_id", .USER_ID},
   'New String() {"state", 2},
   'New String() {"upd_process_id", process_tbl_inserted_id}})
   Function NonQuery(sql As String, ParamArray bindParams() As Array) As ReturnStatement
      TryOpenConnection()
      Using cmd As New MySqlCommand(sql, Me.connection)
         If bindParams.Length > 0 Then
            For Each i As Array In bindParams
               cmd.Parameters.AddWithValue(i(0), i(1))
            Next
         End If
         Try
            Dim count As Integer = cmd.ExecuteNonQuery()
            TryCloseConncetion()
            Return New ReturnStatement("okay", True, count)
         Catch ex As Exception
            AddLog(ex)
            Dim res As New ReturnStatement(ex.Message) With {
               .Exception = ex
            }
            Return res
         End Try
      End Using
   End Function

   Function NonQuery2(sql As String, Optional params As Dictionary(Of String, Object) = Nothing) As ReturnStatement
      TryOpenConnection()
      Using cmd As New MySqlCommand(sql, Me.connection)
         If params IsNot Nothing Then
            For Each kv As KeyValuePair(Of String, Object) In params
               cmd.Parameters.AddWithValue(kv.Key, kv.Value)
            Next
         End If
         Try
            Dim count As Integer = cmd.ExecuteNonQuery()
            TryCloseConncetion()
            Return New ReturnStatement("okay", True, count)
         Catch ex As Exception
            AddLog(ex)
            Return New ReturnStatement(ex.Message)
         End Try
      End Using
   End Function

   Function ExecuteNonQueryCommand(cmd As MySqlCommand) As ReturnStatement
      Try
         TryOpenConnection(cmd.Connection)
         Dim count As Integer = cmd.ExecuteNonQuery()
         TryCloseConncetion(cmd.Connection)
         Return New ReturnStatement("okay", True, count)
      Catch ex As Exception
         AddLog(ex)
         Return New ReturnStatement() With {
            .message = ex.Message,
            .Exception = ex
         }
      End Try
   End Function
   ''' <summary>
   ''' Gumagawa sya ng separate connection. Nag automatic Open and Close yung connection
   ''' </summary>
   ''' <param name="sb"></param>
   Sub GetCommand(sb As Action(Of MySqlCommand))
      Dim con As New MySqlConnection(connProp.ConnectionString)
      TryOpenConnection(con)
      Using cmd As New MySqlCommand("", con)
         sb(cmd)
      End Using
      TryCloseConncetion(con)
   End Sub

   Sub GetCommand(sb As Action(Of MySqlCommand, MySqlTransaction))
      Dim con As New MySqlConnection(connProp.ConnectionString)
      TryOpenConnection(con)
      Using cmd As New MySqlCommand("", con)
         Dim transaction As MySqlTransaction = con.BeginTransaction
         sb(cmd, transaction)
      End Using
      TryCloseConncetion(con)
   End Sub

   Class ConnectionProp
      Public server As String = ""
      Public database As String = ""
      Public port As String = "0"
      Public user As String = ""
      Public password As String = ""
      Public Function ConnectionString() As String
         If database = "" Then
            ConnectionString = String.Concat("Server=", server, ";Uid=", user, ";port=", port, ";Pwd=", password, ";AllowBatch=True;Convert Zero Datetime=True;")
         Else
            ConnectionString = String.Concat("server=", server, ";user=", user, ";database=", database, ";port=", port, ";password=", password, ";AllowBatch=True;Convert Zero Datetime=True;")
         End If
      End Function
      Sub New()
      End Sub
      Sub New(server As String, user As String, database As String, port As String, password As String)
         Me.server = server
         Me.user = user
         Me.database = database
         Me.port = port
         Me.password = password
      End Sub
      Public Function GetNewConnection() As DbMysql
         Dim res As DbMysql = New DbMysql(Me) With {
            .retainOpenConnection = False
         }
         Return res
      End Function
   End Class
   Structure ReturnStatement
      Dim message As String
      Dim affectedCount As Integer
      Dim isSucces As Boolean
      Dim tbl As DataTable
      Dim tbl_firstRow As DataRow
      Dim tbl_lastRow As DataRow
      Dim tbl_rowCount As Integer
      Dim lastInsertedData As DataTable
      Dim Exception As Exception
      Sub New(message As String, Optional isSuccess As Boolean = False, Optional affectedCount As Integer = 0, Optional tbl As DataTable = Nothing, Optional insertedData As DataTable = Nothing)
         Me.message = message
         Me.isSucces = isSuccess
         Me.affectedCount = affectedCount
         Me.tbl = tbl
         Me.lastInsertedData = insertedData
         If tbl IsNot Nothing Then
            Me.tbl_rowCount = tbl.Rows.Count
            If tbl.Rows.Count > 0 Then
               Me.tbl_firstRow = tbl.Rows(0)
               Me.tbl_lastRow = tbl.Rows(tbl.Rows.Count - 1)
            End If
         End If
      End Sub
   End Structure
End Class
