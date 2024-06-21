Imports MySql.Data.MySqlClient
Imports System.Windows.Forms
Public Class DbMysql
   Public connection As MySqlConnection
   Public tryOpenConnectionAttempCount As Integer = 10
   Public tryCloseConnectionAttempCount As Integer = 10
   Public connProp As ConnectionProp
   Public retainOpenConnection As Boolean = False
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
   Function TryOpenConnection(specifyConnection As MySqlConnection) As Boolean
      Dim tryCount As Integer = 0
      While Not specifyConnection.State = ConnectionState.Open
         tryCount += 1
         Try
            specifyConnection.Open()
            Return True
         Catch ex As Exception
            If tryCount >= tryOpenConnectionAttempCount Then
               Exit While
            End If
         End Try
         Application.DoEvents()
      End While
      Return False
   End Function
   Function TryOpenConnection() As Boolean
      Dim tryCount As Integer = 0
      While Not Me.connection.State = ConnectionState.Open
         tryCount += 1
         Try
            Me.connection.Open()
            Return True
         Catch ex As Exception
            If tryCount >= tryOpenConnectionAttempCount Then
               Exit While
            End If
         End Try
         Application.DoEvents()
      End While
      Return False
   End Function
   Function TestOpenConnection() As Boolean
      Dim tryCount As Integer = 0
      While Not Me.connection.State = ConnectionState.Open
         tryCount += 1
         Try
            Me.connection.Open()
            Return True
         Catch ex As Exception
            If tryCount >= tryOpenConnectionAttempCount Then
               Exit While
            End If
         Finally
            Try
               Me.connection.Close()
               GC.Collect()
               GC.WaitForPendingFinalizers()
            Catch ex As Exception
            End Try
         End Try
         Application.DoEvents()
      End While
      Return False
   End Function
   Function TestOpenConnection(specifyConnection As MySqlConnection) As Boolean
      Dim tryCount As Integer = 0
      While Not specifyConnection.State = ConnectionState.Open
         tryCount += 1
         Try
            specifyConnection.Open()
            Return True
         Catch ex As Exception
            If tryCount >= tryOpenConnectionAttempCount Then
               Exit While
            End If
         Finally
            Try
               specifyConnection.Close()
               specifyConnection.Dispose()
               GC.Collect()
               GC.WaitForPendingFinalizers()
            Catch ex As Exception
            End Try
         End Try
         Application.DoEvents()
      End While
      Return False
   End Function
   Sub TryCloseConncetion()
      If Not retainOpenConnection Then
         Dim tryCount As Integer = 0
         While Me.connection.State = ConnectionState.Open
            Try
               tryCount += 1
               Me.connection.Close()
               GC.Collect()
               GC.WaitForPendingFinalizers()
            Catch ex As Exception
               If tryCount >= tryCloseConnectionAttempCount Then
                  Exit While
               End If
            End Try
            Application.DoEvents()
         End While
      End If
   End Sub

   Sub TryCloseConncetion(specifyConnection As MySqlConnection)
      If Not retainOpenConnection Then
         Dim tryCount As Integer = 0
         While specifyConnection.State = ConnectionState.Open
            Try
               tryCount += 1
               specifyConnection.Close()
               specifyConnection.Dispose()
               GC.Collect()
               GC.WaitForPendingFinalizers()
            Catch ex As Exception
               If tryCount >= tryCloseConnectionAttempCount Then
                  Exit While
               End If
            End Try
            Application.DoEvents()
         End While
      End If
   End Sub

   Function ShowDatabasses() As List(Of String)
      Dim res As New List(Of String)
      If TryOpenConnection() Then
         Using command As New MySql.Data.MySqlClient.MySqlCommand
            command.Connection = Me.connection
            command.CommandText = "SHOW DATABASES;"
            Using reader As MySql.Data.MySqlClient.MySqlDataReader = command.ExecuteReader
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
         Using command As New MySql.Data.MySqlClient.MySqlCommand
            command.Connection = Me.connection
            command.CommandText = "SHOW TABLES;"
            Using reader As MySql.Data.MySqlClient.MySqlDataReader = command.ExecuteReader
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
   Sub New(connectionProp As ConnectionProp)
      Me.connProp = connectionProp
      Me.connection = New MySqlConnection(connectionProp.ConnectionString)
   End Sub
   ''' <summary>
   ''' 'Dim sqlFileTbl = .myDb.SelectQuery("SELECT * from file_tbl WHERE id=@id", {New String() {"id", fileID}})
   ''' </summary>
   ''' <param name="sql"></param>
   ''' <param name="bindParams"></param>
   ''' <returns></returns>
   Function SelectQuery(sql As String, ParamArray bindParams() As Array) As ReturnStatement
      Dim con As New MySqlConnection(connProp.ConnectionString)
      TryOpenConnection(con)
      Dim cmd As New MySqlCommand(sql, con)
      If bindParams.Length > 0 Then
         For Each i As Array In bindParams
            cmd.Parameters.AddWithValue(i(0), i(1))
         Next
      End If
      Try
         Dim adapter As New MySqlDataAdapter(cmd)
         Dim tbl As New DataTable
         adapter.Fill(tbl)
         Return New ReturnStatement("okay", True, tbl.Rows.Count, tbl)
      Catch ex As Exception
         Return New ReturnStatement(ex.Message)
      Finally
         TryCloseConncetion(con)
      End Try
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
   Function Insert(tableNme As String, ParamArray bindParams() As Array) As ReturnStatement
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
      TryOpenConnection()
      Try
         Dim res = Me.SelectQuery(sql, bindParams)
         If res.isSucces And res.tbl_rowCount > 0 Then
            Return New ReturnStatement("okay", True, 1, Nothing, res.tbl)
         Else
            Return New ReturnStatement(res.message)
         End If
      Catch ex As Exception
         Return New ReturnStatement(ex.Message)
      End Try
   End Function
   'res = .myDb.NonQuery(String.Concat("Update investigate_tbl set allocate_user_id=@allocate_user_id, upd_process_id=@upd_process_id, state=@state WHERE id=@id"),
   '{New String() {"id", .getOrSet_Record},
   'New String() {"allocate_user_id", .USER_ID},
   'New String() {"state", 2},
   'New String() {"upd_process_id", process_tbl_inserted_id}})
   Function NonQuery(sql As String, ParamArray bindParams() As Array) As ReturnStatement
      Dim extColumns As String = ""
      TryOpenConnection()
      Dim cmd As New MySqlCommand(sql, Me.connection)
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
         Return New ReturnStatement(ex.Message)
      End Try
   End Function

   Function ExecuteNonQueryCommand(cmd As MySqlCommand) As ReturnStatement
      Try
         TryOpenConnection()
         Dim count As Integer = cmd.ExecuteNonQuery()
         TryCloseConncetion()
         Return New ReturnStatement("okay", True, count)
      Catch ex As Exception
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
      Dim cmd As New MySqlCommand("", con)
      sb(cmd)
      TryCloseConncetion(con)
   End Sub



End Class
