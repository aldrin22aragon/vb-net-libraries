'aldrin
Public Class AOA_Timer
   Friend WithEvents Timer As New Timer
   Event Tick(e As TickEventInfo)
   Public maximumSeconds As Integer
   Dim SW As New Stopwatch
   ReadOnly mode As Mode_enum
   Enum Mode_enum As Integer
      RunOnce = 1
      PabalikBalik = 2
   End Enum
   Sub New(seconds As Integer, Optional _mode As Mode_enum = Mode_enum.RunOnce)
      maximumSeconds = seconds
      mode = _mode
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
      RaiseEvent Tick(New TickEventInfo() With {.IsTimeReached = IsTimeReached(), .RemainingSeconds = RemainingSeconds()})
      If IsTimeReached() Then
         If mode = Mode_enum.RunOnce Then
            SW.Stop()
            Timer.Stop()
         Else
            RestartTimer()
         End If
      End If
   End Sub

   Public Function IsTimeReached() As Boolean
      Return SW.Elapsed.Seconds >= maximumSeconds
   End Function
   Public Function RemainingSeconds() As Integer
      Return maximumSeconds - SW.Elapsed.Seconds
   End Function

   Class TickEventInfo
      Public IsTimeReached As Boolean
      Public RemainingSeconds As Integer
   End Class
End Class
