Imports System.IO
Public Class FileManager
   Public Shared Function IsFileOpen(ByVal file As String)
      Dim res As Boolean = False

      Try
         If IO.File.Exists(file) Then
            Dim flInfo As New IO.FileInfo(file)
            Dim stream As FileStream = flInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)
            stream.Close()
            stream.Dispose()
            res = False
         End If
      Catch ex As Exception
         res = True
      End Try
      Return res
   End Function
End Class
