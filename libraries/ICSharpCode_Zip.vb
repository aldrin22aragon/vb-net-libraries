' install in in nugget package
'ICSharpCode.SharpZipLib.dll

Imports ICSharpCode.SharpZipLib.Zip
Imports System.IO

Public Class ICSharpCode_Zip
   Class CompressInfo
      Public Succes As Boolean = False
      Public ErrorInfo As Exception = Nothing
      Public CompressedPath As String = ""
   End Class
   Public Shared Function CompressFolder(sourceFolderPath As String) As CompressInfo
      Dim gzipFilePath As String = sourceFolderPath & ".zip"
      Dim res As New CompressInfo
      Try
         Using zipStream As FileStream = New FileStream(gzipFilePath, FileMode.Create)
            Using zipOutput As ZipOutputStream = New ZipOutputStream(zipStream)
               zipOutput.SetLevel(5) ' Compression level (0-9)
               CompressFolder(sourceFolderPath, "", zipOutput)
            End Using
         End Using
         res.Succes = True
         res.CompressedPath = gzipFilePath
      Catch ex As Exception
         res.Succes = False
         res.ErrorInfo = ex
      End Try
      Return res
   End Function

   Public Shared Function CompressFile(ByVal fil As String) As CompressInfo
      Dim res As New CompressInfo
      Try
         Dim f As New FileInfo(fil)
         Dim newName As String = Mid$(f.FullName, 1, Len(f.FullName) - Len(f.Extension)) & ".zip"
         If File.Exists(newName) Then
            File.Delete(newName)
         End If
         Dim fsZipFile As IO.FileStream
         Dim zfsZipFile As ICSharpCode.SharpZipLib.Zip.ZipOutputStream
         fsZipFile = New FileStream(newName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)
         zfsZipFile = New ICSharpCode.SharpZipLib.Zip.ZipOutputStream(fsZipFile)
         zfsZipFile.SetLevel(9)
         Dim zeZipEntry As ICSharpCode.SharpZipLib.Zip.ZipEntry

         Dim strFile As String = fil
         Dim baInputBuffer() As Byte, iBytesRead As Integer
         Dim fsInputFile As FileStream


         ' Allocate the input buffer byte array, we'll use a 1024K buffer for this
         ReDim baInputBuffer(1048576)

         ' Open the file to be added to the archive for reading.
         fsInputFile = New FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.Read)

         ' Create a zip file entry for the new file
         zeZipEntry = New ICSharpCode.SharpZipLib.Zip.ZipEntry("" & IO.Path.GetFileName(strFile))
         zeZipEntry.DateTime = DateTime.Now
         zeZipEntry.Size = fsInputFile.Length

         ' Add the zip file entry to the zip file
         zfsZipFile.PutNextEntry(zeZipEntry)

         While fsInputFile.Position < fsInputFile.Length
            ' Try to read a 1MB chunk of file...
            iBytesRead = fsInputFile.Read(baInputBuffer, 0, 1048576)

            ' Add the chunk of data read into the Zip File
            zfsZipFile.Write(baInputBuffer, 0, iBytesRead)
         End While

         ' Close the file just added to the archive
         fsInputFile.Close()
         zfsZipFile.Finish()
         zfsZipFile.Close()
         zfsZipFile.Dispose()
         res.Succes = True
         res.CompressedPath = newName
      Catch ex As Exception
         res.ErrorInfo = ex
      End Try
      Return res
   End Function

   Private Shared Sub CompressFolder(sourceFolderPath As String, relativePath As String, zipOutput As ZipOutputStream)
      Dim files As String() = Directory.GetFiles(sourceFolderPath)
      Dim subFolders As String() = Directory.GetDirectories(sourceFolderPath)

      ' Compress files
      For Each filePath As String In files
         Dim entryName As String = Path.Combine(relativePath, Path.GetFileName(filePath))
         Dim newEntry As ZipEntry = New ZipEntry(entryName)
         newEntry.DateTime = DateTime.Now
         newEntry.Size = New FileInfo(filePath).Length

         zipOutput.PutNextEntry(newEntry)

         Using fileStream As FileStream = New FileStream(filePath, FileMode.Open, FileAccess.Read)
            Dim buffer As Byte() = New Byte(4096) {}
            Dim bytesRead As Integer

            Do
               bytesRead = fileStream.Read(buffer, 0, buffer.Length)
               zipOutput.Write(buffer, 0, bytesRead)
            Loop While bytesRead > 0
         End Using

         zipOutput.CloseEntry()
      Next

      ' Recursively compress sub-folders
      For Each subFolder As String In subFolders
         Dim entryName As String = Path.Combine(relativePath, Path.GetFileName(subFolder) + Path.DirectorySeparatorChar)
         zipOutput.PutNextEntry(New ZipEntry(entryName))
         CompressFolder(subFolder, entryName, zipOutput)
      Next
   End Sub
End Class
