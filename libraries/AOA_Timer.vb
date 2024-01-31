'aldrin
Public Class AOA_Timer
   Friend WithEvents Timer As New Timer
   Event Tick(e As TickEventInfo)
   Public maximumSeconds As Integer
   Dim SW As New Stopwatch
   'ReadOnly mode As Mode_enum
   'Enum Mode_enum As Integer
   '   RunOnce = 1
   '   PabalikBalik = 2
   'End Enum
   Sub New(seconds As Integer)
      maximumSeconds = seconds
   End Sub
   Sub StartTimer(Optional overWriteSeconds As Integer = 0)
      If overWriteSeconds > 0 Then maximumSeconds = overWriteSeconds
      SW = New Stopwatch
      SW.Start()
      Timer.Start()
   End Sub
   Sub RestartTimer(Optional overWriteSeconds As Integer = 0)
      If overWriteSeconds > 0 Then maximumSeconds = overWriteSeconds
      SW = New Stopwatch
      SW.Start()
      Timer.Start()
   End Sub
   Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
      If IsTimeReached() Then
         SW.Stop()
         Timer.Stop()
      End If
      RaiseEvent Tick(New TickEventInfo() With {
                      .IsTimeReached = IsTimeReached(),
                      .Span = RemainingTime(),
                      .secondsRemaining = RemainingSeconds()
      })
   End Sub

   Public Function IsTimeReached() As Boolean
      Return SW.Elapsed.Seconds >= maximumSeconds
   End Function
   Function RemainingTime() As TimeSpan
      Dim res As Integer = maximumSeconds - SW.Elapsed.Seconds
      Dim span As TimeSpan = TimeSpan.FromSeconds(res)
      Return span
   End Function
   Function RemainingSeconds() As Integer
      Return maximumSeconds - SW.Elapsed.Seconds
   End Function

   Class TickEventInfo
      Public IsTimeReached As Boolean
      Public secondsRemaining As Integer
      Public Span As TimeSpan
   End Class
End Class
