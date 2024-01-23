Public Class ControlFader
   Friend WithEvents timer As New Timer
   Public control As Control
   Public timeStarts As DateTime
   Public timeOutMiliseconcd As Integer
   Event ControlHidded()
   Sub New(ctlr As Control, timeOutMilisecond As Integer)
      Me.timer.Interval = 400
      Me.timeStarts = Now
      Me.control = ctlr
      Me.timeOutMiliseconcd = timeOutMilisecond

      Me.control.Visible = False
   End Sub
   Sub ShowControl()
      timeStarts = Now
      control.Visible = True
      timer.Enabled = True
   End Sub
   Sub HideControl()
      control.Visible = False
      timer.Enabled = False
      RaiseEvent ControlHidded()
   End Sub
   Private Sub timer_Tick(sender As Object, e As EventArgs) Handles timer.Tick
      Dim elaps As TimeSpan = Now.Subtract(timeStarts)
      If elaps.TotalMilliseconds >= timeOutMiliseconcd Then
         HideControl()
      End If
   End Sub
End Class
