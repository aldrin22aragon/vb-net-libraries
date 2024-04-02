' install in in nugget package
'ICSharpCode.SharpZipLib.dll

Imports ICSharpCode.SharpZipLib.Zip
Imports ICSharpCode.SharpZipLib.Core
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

   Shared Sub ExtractZipFile(archiveFilenameIn As String, password As String, outFolder As String)
      Dim zf As ZipFile = Nothing
      Try
         Dim fs As FileStream = File.OpenRead(archiveFilenameIn)
         zf = New ZipFile(fs)
         If Not [String].IsNullOrEmpty(password) Then    ' AES encrypted entries are handled automatically
            zf.Password = password
         End If
         For Each zipEntry As ZipEntry In zf
            If Not zipEntry.IsFile Then     ' Ignore directories
               Continue For
            End If
            Dim entryFileName As [String] = zipEntry.Name
            ' to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
            ' Optionally match entrynames against a selection list here to skip as desired.
            ' The unpacked length is available in the zipEntry.Size property.

            Dim buffer As Byte() = New Byte(4095) {}    ' 4K is optimum
            Dim zipStream As Stream = zf.GetInputStream(zipEntry)

            ' Manipulate the output filename here as desired.
            Dim fullZipToPath As [String] = Path.Combine(outFolder, entryFileName)
            Dim directoryName As String = Path.GetDirectoryName(fullZipToPath)
            If directoryName.Length > 0 Then
               Directory.CreateDirectory(directoryName)
            End If

            ' Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
            ' of the file, but does not waste memory.
            ' The "Using" will close the stream even if an exception occurs.
            Using streamWriter As FileStream = File.Create(fullZipToPath)
               StreamUtils.Copy(zipStream, streamWriter, buffer)
            End Using
         Next
      Finally
         If zf IsNot Nothing Then
            zf.IsStreamOwner = True     ' Makes close also shut the underlying stream
            ' Ensure we release resources
            zf.Close()
         End If
      End Try
   End Sub

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
