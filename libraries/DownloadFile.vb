' please add WinSCP.exe startup Path 5.17.8.10803
' add reference WinSCPnet.dll 5.17.8.0

Imports WinSCP
Public Class DownloadFile
   ReadOnly sesOptions As SessionOptions
   ReadOnly ftpFilePath As String
   ReadOnly destination As String
   '
   Public status As New _STAT_INFO()
   Public th As System.Threading.Thread
   Public removeAfterDowload As Boolean = False
   '
   Sub New(ftpFilePath As String, destFolder As String, sesOption As WinSCP.SessionOptions)
      Me.ftpFilePath = ftpFilePath
      Me.sesOptions = sesOption
      Me.destination = destFolder
   End Sub
   Sub StartDownload()
      status = New _STAT_INFO() With {.isRunning = True}
      th = New System.Threading.Thread(AddressOf RunThread)
      th.Start()
   End Sub
   Private Sub RunThread()
      Try
         Dim ses As New Session
         AddHandler ses.FileTransferProgress, Sub(sender As Object, e As FileTransferProgressEventArgs)
                                                 status.DownloadPercentage = String.Concat((e.FileProgress * 100).ToString, "%")
                                              End Sub
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
            Dim transferResult As WinSCP.TransferOperationResult = ses.GetFiles(Me.ftpFilePath, tmpDownloadingPath, removeAfterDowload, transferOpt)
            transferResult.Check()
            Dim i As Integer = 0
            While Not transferResult.IsSuccess
               Application.DoEvents()
            End While
            Dim downloadedFile As String = IO.Path.Combine(Me.destination, IO.Path.GetFileName(Me.ftpFilePath))
            cnt = 0
            While IO.File.Exists(downloadedFile)
               cnt += 1
               Dim tmpFlNm As String = IO.Path.GetFileNameWithoutExtension(Me.ftpFilePath)
               Dim xt As String = IO.Path.GetExtension(Me.ftpFilePath)
               downloadedFile = IO.Path.Combine(Me.destination, String.Concat(tmpFlNm, cnt, xt))
            End While
            My.Computer.FileSystem.RenameFile(tmpDownloadingPath, IO.Path.GetFileName(downloadedFile))
            status = New _STAT_INFO() With {
               .isErr = False,
               .errMsg = "Downloaded",
               .isRunning = False,
               .isDownloaded = True,
               .isDoneRunning = True
            }
         Catch ex As Exception
            status = New _STAT_INFO() With {
               .isErr = True,
               .errMsg = "Downloading error => " & ex.Message,
               .isRunning = False,
               .isDownloaded = False,
               .isDoneRunning = True
            }
         End Try
         ses.Close()
         ses.Dispose()
         GC.Collect()
         GC.WaitForPendingFinalizers()

      Catch ex As Exception
         status = New _STAT_INFO() With {
               .isErr = True,
               .errMsg = "Open session error => " & ex.Message,
               .isRunning = False,
               .isDownloaded = False,
               .isDoneRunning = True
            }
      End Try
   End Sub
#Region "Utensils"
   Class _STAT_INFO
      Public isErr As Boolean = False
      Public errMsg As String = ""
      Public isRunning As Boolean = False
      Public isDownloaded As Boolean = False
      Public isDoneRunning As Boolean = False
      Public DownloadPercentage As String = ""
   End Class
#End Region
End Class
