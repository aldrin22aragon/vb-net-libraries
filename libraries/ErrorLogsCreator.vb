Public Class ErrorLogsCreator
   Shared logFolder As String = IO.Path.Combine(Application.StartupPath, "Log Files")

   Enum LogFor As Integer
      SendTestToSelf
   End Enum

   Shared Sub CreateLog(logFor As LogFor, exeption As Exception, Optional message As String = "")
      Dim mainfolder As String = ""
      Dim dt As String = Format(Now, "yyyyMMdd")
      Select Case logFor
         Case LogFor.SendTestToSelf
            mainfolder = IO.Path.Combine(logFolder, "Test send email to self")
      End Select
      If mainfolder <> "" Then
         If Not IO.Directory.Exists(mainfolder) Then IO.Directory.CreateDirectory(mainfolder)
         Dim fl As String = IO.Path.Combine(mainfolder, dt & ".log")
         Using wr As New IO.StreamWriter(fl, True)
            If message <> "" Then
               wr.WriteLine("Message: " & message)
            End If
            wr.WriteLine("Exception Message: " & exeption.Message)
            wr.WriteLine("Exception stacktrace: " & exeption.StackTrace)
            If exeption.InnerException IsNot Nothing Then
               wr.WriteLine("InnerException message: " & exeption.InnerException.Message)
               wr.WriteLine("InnerException Stacktrace: " & exeption.InnerException.StackTrace)
            Else
               wr.WriteLine("InnerException message: NONE ")
               wr.WriteLine("InnerException Stacktrace: NONE")
            End If
            wr.WriteLine("_______________________________________________________________")
         End Using
      End If
   End Sub

End Class
