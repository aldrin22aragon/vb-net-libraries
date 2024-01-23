' please add WinSCP.exe startup Path 5.17.8.10803
' add reference WinSCPnet.dll 5.17.8.0


Imports WinSCP
Public Class UploadFile
   ReadOnly sesOptions As SessionOptions
   ReadOnly flPath As String
   ReadOnly destination As String
   ReadOnly dgvRow As DataGridViewRow
   Public status As New _STAT_INFO()
   Public th As System.Threading.Thread
   Public removeAfterDowload As Boolean = False

   Sub New(filePath As String, destFolder As String, sesOption As WinSCP.SessionOptions, dgvRow As DataGridViewRow)
      Me.flPath = filePath
      Me.sesOptions = sesOption
      Me.dgvRow = dgvRow
      Me.destination = destFolder
   End Sub
   Sub StartUpload()
      status = New _STAT_INFO() With {
         .isErr = False,
         .errMsg = "",
         .isRunning = True,
         .isUploaded = False,
         .isDoneRunning = False
      }
      th = New System.Threading.Thread(AddressOf RunThread)
      th.Start()
   End Sub
   Private Sub RunThread()
      Try
         Dim ses As New Session
         AddHandler ses.FileTransferProgress, Sub(sender As Object, e As FileTransferProgressEventArgs)
                                                 dgvRow.Cells(3).Value = String.Concat((e.FileProgress * 100).ToString, "%")
                                              End Sub
         ses.Open(Me.sesOptions)
         Try
            Dim transferOpt As New WinSCP.TransferOptions With {
               .TransferMode = TransferMode.Binary
            }
            Dim transferResult As TransferOperationResult
            Dim flNameWOext As String = IO.Path.GetFileNameWithoutExtension(Me.flPath)
            Dim extWithDot As String = IO.Path.GetExtension(Me.flPath)
            Dim tmpDest As String = RemotePath.Combine(Me.destination, String.Concat(flNameWOext, extWithDot, ".uploading"))
            Dim dest As String = RemotePath.Combine(Me.destination, String.Concat(flNameWOext, extWithDot))

            'Possible error FileExists
            If ses.FileExists(dest) Then
               status = New _STAT_INFO() With {
                  .isErr = False,
                  .errMsg = "File already uploaded.",
                  .isRunning = False,
                  .isUploaded = False,
                  .isDoneRunning = True
               }
               dgvRow.Cells(3).Value = "File already uploaded."
            Else
               'Possible error FileExists
               If ses.FileExists(tmpDest) Then
                  'Possible error RemoveFile
                  ses.RemoveFile(tmpDest)
               End If
               'Possible error PutFiles
               transferResult = ses.PutFiles(Me.flPath, tmpDest, False, transferOpt)
               transferResult.Check()
               While Not transferResult.IsSuccess
                  Application.DoEvents()
               End While
               'Possible error MoveFile
               ses.MoveFile(tmpDest, dest)
               status = New _STAT_INFO() With {
                  .isErr = False,
                  .errMsg = "Uploaded",
                  .isRunning = False,
                  .isUploaded = True,
                  .isDoneRunning = True
               }
               dgvRow.Cells(3).Value = "Completed"
            End If
         Catch ex As Exception
            dgvRow.Cells(3).Value = ex.Message
            status = New _STAT_INFO() With {
               .isErr = True,
               .errMsg = "Uploading error => " & ex.Message,
               .isRunning = False,
               .isUploaded = False,
               .isDoneRunning = True
            }
         End Try
      Catch ex As Exception
         dgvRow.Cells(3).Value = ex.Message
         status = New _STAT_INFO() With {
            .isErr = True,
            .errMsg = "Open session error => " & ex.Message,
            .isRunning = False,
            .isUploaded = False,
            .isDoneRunning = True
         }
      End Try
   End Sub
#Region "Utensils"
   Class _STAT_INFO
      Public isErr As Boolean = False
      Public errMsg As String = ""
      Public isRunning As Boolean = False
      Public isUploaded As Boolean = False
      Public isDoneRunning As Boolean = False
   End Class
#End Region
End Class
