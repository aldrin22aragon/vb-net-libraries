Class MyLabelfader
   Friend WithEvents TimerCheck As New Timer
   Delegate Sub RunSub()
   Event Faded()
   Dim lbl As Label
   Dim fadeMlscnd As Integer
   Dim starts As DateTime
   '
   Sub New(l As Label, fadeMilliseconds As Integer)
      fadeMlscnd = fadeMilliseconds
      lbl = l
      lbl.Visible = False
   End Sub
   Sub showLabel(run As RunSub)
      run()
      starts = Now
      lbl.Visible = True
      TimerCheck.Enabled = True
   End Sub
   Private Sub TimerCheck_Tick(sender As Object, e As EventArgs) Handles TimerCheck.Tick
      Dim elapsed As TimeSpan = Now.Subtract(starts)
      If elapsed.TotalMilliseconds >= fadeMlscnd Then
         lbl.Visible = False
         TimerCheck.Enabled = False
         RaiseEvent Faded()
      End If
   End Sub
End Class