' please add WinSCP.exe startup Path 5.17.8.10803
' add reference WinSCPnet.dll 5.17.8.0

Imports WinSCP
Imports System.Windows.Forms
Public Class UploadFile
   ReadOnly sesOptions As SessionOptions
   ReadOnly flPath As String
   ReadOnly ftpDestinationFolder As String
   '
   Public UP_Info As New UploadInfo
   Private th As System.Threading.Thread

   Sub New(filePath As String, ftpDestinationFolder As String, sesOption As WinSCP.SessionOptions)
      Me.flPath = filePath
      Me.sesOptions = sesOption
      Me.ftpDestinationFolder = ftpDestinationFolder
   End Sub
   Sub StartUpload()
      UP_Info.Status = UploadInfo.E_Status.Started
      th = New System.Threading.Thread(AddressOf RunThread)
      th.Start()
   End Sub

   Public Shared Sub CreateFtpDirectoryRecursive(ses As Session, ftpDirectory As String)
      If ses.Opened Then
         If Not ses.FileExists(ftpDirectory) Then
            Dim slice As String() = ftpDirectory.Split("/")
            Dim p As String = ""
            For Each i As String In slice
               If i = "" Then
                  p = ""
               Else
                  p = p & "/" & i
                  If Not ses.FileExists(p) Then
                     Try
                        ses.CreateDirectory(p)
                     Catch ex As Exception
                        If ses.FileExists(p) Then
                           Continue For
                        Else
                           Throw New Exception("Error on creating FTP directory :" & p)
                        End If
                     End Try
                  End If
               End If
            Next
         End If
      Else
         Throw New Exception("Session must be open  in Sub CreateDirectoryRecursive.")
      End If
   End Sub

   Private Sub RunThread()
      Try
         Dim ses As New Session
         AddHandler ses.FileTransferProgress, Sub(sender As Object, e As FileTransferProgressEventArgs)
                                                 UP_Info.DownloadPercentage = String.Concat((e.FileProgress * 100).ToString, "%")
                                              End Sub
         UP_Info.Status = UploadInfo.E_Status.OPENING_SESSION
         ses.Open(Me.sesOptions) ' possible error opening session
         Try
            Dim transferOpt As New WinSCP.TransferOptions With {
               .TransferMode = TransferMode.Binary
            }
            Dim transferResult As TransferOperationResult
            Dim flNameWOext As String = IO.Path.GetFileNameWithoutExtension(Me.flPath)
            Dim extWithDot As String = IO.Path.GetExtension(Me.flPath)
            Dim tmpDest As String = RemotePath.Combine(Me.ftpDestinationFolder, String.Concat(flNameWOext, extWithDot, ".uploading"))
            Dim dest As String = RemotePath.Combine(Me.ftpDestinationFolder, String.Concat(flNameWOext, extWithDot))
            CreateFtpDirectoryRecursive(ses, Me.ftpDestinationFolder)
            'Possible error FileExists
            If ses.FileExists(dest) Then
               UP_Info.Status = UploadInfo.E_Status.File_Already_Exist
            Else
               'Possible error FileExists
               If ses.FileExists(tmpDest) Then
                  'Possible error RemoveFile
                  ses.RemoveFile(tmpDest)
               End If
               'Possible error PutFiles
               UP_Info.Status = UploadInfo.E_Status.Uploading
               transferResult = ses.PutFiles(Me.flPath, tmpDest, False, transferOpt)
               transferResult.Check()
               While Not transferResult.IsSuccess
                  Application.DoEvents()
               End While
               'Possible error MoveFile
               UP_Info.Status = UploadInfo.E_Status.FINALIZING
               ses.MoveFile(tmpDest, dest)
               UP_Info.Status = UploadInfo.E_Status.Uploaded
            End If
         Catch ex As Exception
            UP_Info.Status = UploadInfo.E_Status.Error
            UP_Info.ErrorExeption = ex
            UP_Info.ErrorMessage = "Error from Uploading file."
         End Try
         ses.Close()
         ses.Dispose()
         GC.Collect()
         GC.WaitForPendingFinalizers()
      Catch ex As Exception
         UP_Info.Status = UploadInfo.E_Status.Error
         UP_Info.ErrorExeption = ex
         UP_Info.ErrorMessage = "Error from Opening session."
      End Try
   End Sub
#Region "Utensils"
   Class UploadInfo
      Enum E_Status As Integer
         NONE
         Started
         OPENING_SESSION
         File_Already_Exist
         Uploading
         FINALIZING
         Uploaded
         [Error]
      End Enum
      Public Status As E_Status = E_Status.NONE
      Public ErrorExeption As Exception = Nothing
      Public StatusMessage As String = ""
      Public DownloadPercentage As String = ""
      Public ErrorMessage As String = ""
   End Class
#End Region
End Class
