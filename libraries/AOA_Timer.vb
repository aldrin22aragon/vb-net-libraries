Public Class AOA_Timer
   Friend WithEvents Timer As New Timer
   Event Tick(e As TickEventInfo)
   Dim maximum_ As Integer
   Dim SW As New Stopwatch
   Sub New(seconds As Integer)
      maximum_ = seconds
   End Sub
   Sub StartTimer(Optional overWriteSeconds As Integer = 0)
      If overWriteSeconds > 0 Then maximum_ = overWriteSeconds
      SW = New Stopwatch
      SW.Start()
      Timer.Start()
   End Sub
   Sub RestartTimer(Optional overWriteSeconds As Integer = 0)
      If overWriteSeconds > 0 Then maximum_ = overWriteSeconds
      SW = New Stopwatch
      SW.Start()
      Timer.Start()
   End Sub
   Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
      If IsTimeReached() Then
         SW.Stop()
         Timer.Stop()
      End If
      RaiseEvent Tick(New TickEventInfo() With {.IsTimeReached = IsTimeReached(), .RemainingSeconds = RemainingSeconds()})
   End Sub

   Public Function IsTimeReached() As Boolean
      Return SW.Elapsed.Seconds >= maximum_
   End Function
   Public Function RemainingSeconds() As Integer
      Return maximum_ - SW.Elapsed.Seconds
   End Function

   Class TickEventInfo
      Public IsTimeReached As Boolean
      Public RemainingSeconds As Integer
   End Class
End Class
