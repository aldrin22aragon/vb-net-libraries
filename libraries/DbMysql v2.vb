Imports MySql.Data.MySqlClient

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
      Public prggrammer As String = ""
      Public Function connectionString() As String
         If database = "" Then
            connectionString = String.Concat("Server=", server, ";Uid=", user, ";port=", port, ";Pwd=", password, ";AllowBatch=True;Convert Zero Datetime=True;")
         Else
            connectionString = String.Concat("server=", server, ";user=", user, ";database=", database, ";port=", port, ";password=", password, ";AllowBatch=True;Convert Zero Datetime=True;")
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
   End Class
   Structure ReturnStatement
      Dim message As String
      ''' <summary>
      ''' Inserted count or Updated count or Select result count.
      ''' </summary>
      Dim affectedCount As Integer
      Dim isSucces As Boolean
      Dim tbl As DataTable
      Dim tbl_firstRow As DataRow
      Dim tbl_lastRow As DataRow
      Dim tbl_rowCount As Integer
      Dim lastInsertedData As DataTable
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
      Me.connection = New MySqlConnection(connectionProp.connectionString)
   End Sub

   Function SelectQuery(columns As String, tableName As String, Optional whereCondition As String = "1", Optional bindParams As Dictionary(Of String, Object) = Nothing) As ReturnStatement
      Dim sql As String = String.Concat("SELECT ", columns, " FROM ", tableName, " WHERE ", whereCondition)
      TryOpenConnection()
      Dim cmd As New MySqlCommand(sql, Me.connection)
      If bindParams IsNot Nothing Then
         For Each dict As KeyValuePair(Of String, Object) In bindParams
            cmd.Parameters.AddWithValue(dict.Key, dict.Value)
         Next
      End If
      Try
         Dim adapter As New MySqlDataAdapter(cmd)
         Dim tbl As New DataTable
         adapter.Fill(tbl)
         TryCloseConncetion()
         Return New ReturnStatement("okay", True, tbl.Rows.Count, tbl)
      Catch ex As Exception
         Return New ReturnStatement(ex.Message)
      End Try
   End Function

   Iterator Function IterateSelectQuery(IterateCMD As MySqlCommand) As IEnumerable(Of MySqlDataReader)
      IterateCMD.Connection.Open()
      Dim rd = IterateCMD.ExecuteReader
      While rd.Read
         Yield rd
      End While
      rd.Close()
      IterateCMD.Connection.Close()
      GC.Collect()
      GC.WaitForPendingFinalizers()
   End Function
   Function IterateCMD(sql) As MySqlCommand
      Return New MySqlCommand(sql, New MySqlConnection(Me.connProp.connectionString))
   End Function
   'Dim sqlFileTbl = .myDb.IterateSelectQuery("SELECT * from file_tbl WHERE id=@id", {New String() {"id", fileID}})
   Iterator Function IterateSelectQuery(sql As String, ParamArray bindParams() As Array) As IEnumerable(Of MySqlDataReader)
      Dim IterateCMD = Me.IterateCMD(sql)
      IterateCMD.Connection.Open()
      If bindParams.Length > 0 Then
         For Each i As Array In bindParams
            IterateCMD.Parameters.AddWithValue(i(0), i(1))
         Next
      End If
      Dim rd = IterateCMD.ExecuteReader
      While rd.Read
         Yield rd
      End While
      rd.Close()
      IterateCMD.Connection.Close()
      GC.Collect()
      GC.WaitForPendingFinalizers()
   End Function

   'Dim sqlFileTbl = .myDb.SelectQuery("SELECT * from file_tbl WHERE id=@id", {New String() {"id", fileID}})
   Function SelectQuery(sql As String, ParamArray bindParams() As Array) As ReturnStatement
      TryOpenConnection()
      Dim cmd As New MySqlCommand(sql, Me.connection)
      If bindParams.Length > 0 Then
         For Each i As Array In bindParams
            cmd.Parameters.AddWithValue(i(0), i(1))
         Next
      End If
      Try
         Dim adapter As New MySqlDataAdapter(cmd)
         Dim tbl As New DataTable
         adapter.Fill(tbl)
         TryCloseConncetion()
         Return New ReturnStatement("okay", True, tbl.Rows.Count, tbl)
      Catch ex As Exception
         Return New ReturnStatement(ex.Message)
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
   'b = b
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

   Function NonQuery2(sql As String, ParamArray bindParams() As Array) As ReturnStatement
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
         Return New ReturnStatement(ex.Message)
      End Try
   End Function

   Function GetCommand(sql) As MySqlCommand
      Return New MySqlCommand(sql, Me.connection)
   End Function
End Class
