'Please see link below for more info about Ghost Script
'https://www.ghostscript.com/doc/current/Use.htm
Imports System.IO
Public Class PdfToTiffCOnverter
    Dim appPath As String = Application.StartupPath
    Dim ghostScriptLibraryDirectory As String = IO.Path.Combine(appPath, "GS")
    Dim ghostScriptEXEPath As String = IO.Path.Combine(ghostScriptLibraryDirectory, "gswin32c.exe")
    Dim ghostScriptDLLPath As String = IO.Path.Combine(ghostScriptLibraryDirectory, "gsdll32.dll")
    Dim isReadyForConvert As Boolean = True
    Sub informUser()
        InputBox("Please include Library 'gswin32c.exe' and 'gsdll32.dll'  to folder ", "!!!!", ghostScriptLibraryDirectory)
    End Sub
    Sub New()
        If Not IO.Directory.Exists(ghostScriptLibraryDirectory) Then MkDir(ghostScriptLibraryDirectory)
        If Not IO.File.Exists(ghostScriptDLLPath) Or Not IO.File.Exists(ghostScriptEXEPath) Then
            isReadyForConvert = False
        End If
    End Sub
    Function convertPdfToTiff(pdfSource As String, Optional directoryOutput As String = Nothing, Optional quality As Integer = 150) As Boolean
        If Not isReadyForConvert Then
            informUser()
            Return False
        End If
        Dim res As Boolean = True
        ''creating Command file
        If directoryOutput = Nothing Then
            directoryOutput = IO.Path.GetDirectoryName(pdfSource)
        End If
        Dim file As IO.FileInfo
        Dim cmdFilePath As String = IO.Path.Combine(ghostScriptLibraryDirectory, IO.Path.GetFileNameWithoutExtension(pdfSource) & ".cmd")
        Dim newTifFilePath As String = IO.Path.Combine(directoryOutput, IO.Path.GetFileNameWithoutExtension(pdfSource) & ".tif")
        file = New FileInfo(cmdFilePath)
        If file.Exists Then file.Delete()
        Dim wrt As StreamWriter
        wrt = New StreamWriter(cmdFilePath, False)
        wrt.WriteLine("@echo off")
        wrt.WriteLine("cd """ & ghostScriptLibraryDirectory & """")
        wrt.WriteLine(concat("""", IO.Path.Combine(ghostScriptLibraryDirectory, "gswin32c"), """ ^"))
        wrt.WriteLine("   -dSAFER ^")
        wrt.WriteLine("   -dBATCH ^")
        wrt.WriteLine("   -dNOPAUSE ^")
        wrt.WriteLine("   -q ^")
        wrt.WriteLine("   -sDEVICE=tiff24nc ^")
        wrt.WriteLine("   -r" & quality & " ^")
        wrt.WriteLine("   -sCompression=lzw ^")
        wrt.WriteLine(concat("   -sOutputFile=""", newTifFilePath, """ ^"))
        wrt.WriteLine(concat("    """, pdfSource, """"))
        wrt.Close()
        '' Run command file for convertion
        Dim ProcessProperties As New ProcessStartInfo
        ProcessProperties.FileName = cmdFilePath
        ProcessProperties.Arguments = pdfSource
        ProcessProperties.WindowStyle = ProcessWindowStyle.Hidden
        Dim myProcess As Process = Process.Start(ProcessProperties)
        myProcess.WaitForExit()
        Dim errExt As Integer = myProcess.ExitCode
        If (errExt > 0) And (Not myProcess.HasExited) Then
            res = False
            myProcess.Kill()
        End If
        If IO.File.Exists(cmdFilePath) Then
            Try
                IO.File.Delete(cmdFilePath)
            Catch ex As Exception
            End Try
        End If
        Return res
    End Function
    Function concat(ParamArray str As String()) As String
        Dim res As String = ""
        For Each i As String In str
            res = res & i
        Next
        Return res
    End Function
End Class
