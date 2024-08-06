'aldrin
Public Class AOA_Timer
   Friend WithEvents Timer As New Timer()
    Event Tick(sender As AOA_Timer, e As TickEventInfo)
    Public maximumSeconds As Integer
   Dim SW As Stopwatch = Nothing
   Dim backup As Integer = -1
   Public firstRunStartsAt_seconds As Integer = 0
   'ReadOnly mode As Mode_enum
   'Enum Mode_enum As Integer
   '   RunOnce = 1
   '   PabalikBalik = 2
   'End Enum
   Sub New(seconds As Integer)
      maximumSeconds = seconds
   End Sub

   Sub Stop_()
      If SW IsNot Nothing AndAlso SW.IsRunning Then SW.Stop()
      Timer.Stop()
   End Sub

   Sub ExtendMaximum(seconds As Integer)
      backup = maximumSeconds
      maximumSeconds += seconds
   End Sub

   Sub StartRunning(Optional overWriteSeconds As Integer = 0)
      If SW Is Nothing Then
         If overWriteSeconds > 0 Then maximumSeconds = overWriteSeconds
         SW = New Stopwatch
         SW.Start()
         Timer.Start()
      Else
         If SW.IsRunning Then
            Throw New Exception("Timer is already running")
         Else
            RestartTimer()
         End If
      End If
   End Sub
   Sub RestartTimer(Optional overWriteSeconds As Integer = 0)
      If SW IsNot Nothing Then
         If overWriteSeconds > 0 Then maximumSeconds = overWriteSeconds
         SW = New Stopwatch
         SW.Start()
         Timer.Start()
      Else
         Throw New Exception("Timer must be start first")
      End If
   End Sub
   Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
      If IsTimeReached() Then
         SW.Stop()
         Timer.Stop()
         If backup <> -1 Then
            maximumSeconds = backup
            backup = -1
         End If
         RaiseEvent Tick(Me, New TickEventInfo() With {
                    .IsTimeReached = True,
                    .Span = RemainingTime(),
                    .secondsRemaining = RemainingSeconds()
         })
      Else
         RaiseEvent Tick(Me, New TickEventInfo() With {
                    .IsTimeReached = False,
                    .Span = RemainingTime(),
                    .secondsRemaining = RemainingSeconds()
         })
      End If
   End Sub

   Public Function IsRunning() As Boolean
      If SW IsNot Nothing Then
         Return SW.IsRunning
      Else
         Return False
      End If
   End Function

   Private Function IsTimeReached() As Boolean
      If firstRunStartsAt_seconds > 0 Then
         If SW.Elapsed.TotalSeconds >= firstRunStartsAt_seconds Then
            firstRunStartsAt_seconds = 0
            Return True
         End If
      End If
      Return SW.Elapsed.TotalSeconds >= maximumSeconds
   End Function
   Function RemainingTime() As TimeSpan
      Dim res As Integer = maximumSeconds - SW.Elapsed.TotalSeconds
      Dim span As TimeSpan = TimeSpan.FromSeconds(res)
      Return span
   End Function
   Function RemainingSeconds() As Integer
      Return maximumSeconds - SW.Elapsed.TotalSeconds
   End Function

   Class TickEventInfo
      Public IsTimeReached As Boolean
      Public secondsRemaining As Integer
      Public Span As TimeSpan
      Public debug As String
   End Class
End Class
