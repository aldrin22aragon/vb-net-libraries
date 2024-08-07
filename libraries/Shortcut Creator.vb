'Please add Refference => Windows Script Host Object Model
'windows + r / type shell:startup to open start up folder
Imports IWshRuntimeLibrary
Public Class Shortcut_Creator
   Public Shared Function CreateEXEShortcut(sourceEXE As String, TargetFolder As String, Optional IconPath As String = "") As ReturnInfo
      Dim res As New ReturnInfo
      Try
         Dim FullPathOfShortcutToCreate As String = IO.Path.Combine(TargetFolder, IO.Path.GetFileNameWithoutExtension(sourceEXE) & ".lnk")
         Dim TheExeOrUrlToExecute As String = sourceEXE
         Dim TheIconPath As String = IIf(IconPath <> "", IconPath, sourceEXE & ",0")
         Dim myShell As New WshShell()
         Dim myShortcut As WshShortcut = CType(myShell.CreateShortcut(FullPathOfShortcutToCreate), WshShortcut)
         myShortcut.TargetPath = sourceEXE
         myShortcut.IconLocation = TheIconPath
         myShortcut.Save()
         res.SUCCESS = True
         res.MESSAGE = "Shortcut is created"
         Runtime.InteropServices.Marshal.ReleaseComObject(myShortcut)
         Runtime.InteropServices.Marshal.ReleaseComObject(myShell)
      Catch ex As Exception
         res.SUCCESS = False
         res.MESSAGE = String.Concat(ex.Message, vbNewLine, vbNewLine, ex.StackTrace)
      End Try
      Return res
   End Function
   Class ReturnInfo
      Public SUCCESS As Boolean = False
      Public MESSAGE As String = ""
   End Class
   Public Shared Function GetStartupFolder() As String
      Dim res = Environment.GetFolderPath(Environment.SpecialFolder.Startup)
      Return res
   End Function
   Public Shared Function IsShortcutExists(Dir As String, fileNameWOExt As String) As Boolean
      Dim fl As String = IO.Path.Combine(Dir, fileNameWOExt & ".lnk")
      Return IO.File.Exists(fl)
   End Function
End Class
