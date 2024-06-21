' please add WinSCP.exe startup Path 5.17.8.10803
' add reference WinSCPnet.dll 5.17.8.0

Imports WinSCP
Imports System.Windows.Forms
Public Class DownloadFile
   ReadOnly sesOptions As SessionOptions
   ReadOnly ftpFilePath As String
   ReadOnly destination As String
   '
   Public DL_info As New DownloadInfo
   Public th As System.Threading.Thread
   Public removeAfterDowload As Boolean = False
   '
   Property IsProcessDone As Boolean = False
   '
   Sub New(ftpFilePath As String, destFolder As String, sesOption As WinSCP.SessionOptions)
      Me.ftpFilePath = ftpFilePath
      Me.sesOptions = sesOption
      Me.destination = destFolder
   End Sub
   Sub StartDownload()
      IsProcessDone = False
      DL_info.Status = DownloadInfo.E_Status.Started
      th = New System.Threading.Thread(AddressOf RunThread)
      th.Start()
   End Sub
   Private Sub RunThread()
      Try
         Dim ses As New Session
         AddHandler ses.FileTransferProgress, Sub(sender As Object, e As FileTransferProgressEventArgs)
                                                 DL_info.DownloadPercentage = String.Concat((e.FileProgress * 100).ToString, "%")
                                              End Sub
         DL_info.Status = DownloadInfo.E_Status.OPENING_SESSIONG
         ses.Open(Me.sesOptions)
         Try
            Dim transferOpt As New WinSCP.TransferOptions With {
               .TransferMode = TransferMode.Binary
            }
            Dim tmpDest As String = IO.Path.Combine(Me.destination, IO.Path.GetFileName(Me.ftpFilePath))
            Dim tmpDownloadingPath As String = tmpDest & ".downloading"
            Dim cnt As Integer = 0
            While IO.File.Exists(tmpDownloadingPath)
               tmpDownloadingPath = String.Concat(tmpDest, ".downloading", cnt)
               cnt += 1
            End While
            '            '
            DL_info.Status = DownloadInfo.E_Status.Downloading

            Dim transferResult As WinSCP.TransferOperationResult = ses.GetFiles(Me.ftpFilePath, tmpDownloadingPath, removeAfterDowload, transferOpt)
            transferResult.Check()
            Dim i As Integer = 0
            While Not transferResult.IsSuccess
               Application.DoEvents()
            End While
            Dim downloadedFile As String = IO.Path.Combine(Me.destination, IO.Path.GetFileName(Me.ftpFilePath))
            If IO.File.Exists(downloadedFile) Then
               IO.File.Delete(downloadedFile)
            End If
            cnt = 0
            While IO.File.Exists(downloadedFile)
               cnt += 1
               Dim tmpFlNm As String = IO.Path.GetFileNameWithoutExtension(Me.ftpFilePath)
               Dim xt As String = IO.Path.GetExtension(Me.ftpFilePath)
               downloadedFile = IO.Path.Combine(Me.destination, String.Concat(tmpFlNm, cnt, xt))
            End While
            DL_info.Status = DownloadInfo.E_Status.FINALIZING
            My.Computer.FileSystem.RenameFile(tmpDownloadingPath, IO.Path.GetFileName(downloadedFile))
            DL_info.Status = DownloadInfo.E_Status.Downloaded
         Catch ex As Exception
            DL_info.Status = DownloadInfo.E_Status.Error
            DL_info.ErrorExeption = ex
            DL_info.ErrorMessage = "Error from downloading file."
         End Try
         ses.Close()
         ses.Dispose()
         GC.Collect()
         GC.WaitForPendingFinalizers()
      Catch ex As Exception
         DL_info.Status = DownloadInfo.E_Status.Error
         DL_info.ErrorExeption = ex
         DL_info.ErrorMessage = "Error from opening session."
      End Try
      IsProcessDone = True
   End Sub

#Region "Utensils"
   Class DownloadInfo
      Enum E_Status As Integer
         NONE
         Started
         OPENING_SESSIONG
         Downloading
         FINALIZING
         Downloaded
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
