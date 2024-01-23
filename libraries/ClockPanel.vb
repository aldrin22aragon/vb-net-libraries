
'How to use
'Private clock As ClockPanel
'' Every second, refresh the clock
'Private Sub clockTimer_Tick(sender As Object, e As EventArgs) Handles clockTimer.Tick
'    clock.Refresh()
'End Sub
'Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
'    clock = New ClockPanel()
'    clock.Name = "newpanel"
'    clock.Width = 200
'    clock.Height = 200
'    clock.Left = 50
'    clock.Top = 280
'    Me.Controls.Add(clock)
'    clockTimer.Start()
'End Sub

' add resource image from  name it as "clockface"
'https://www.coderslexicon.com/wp-content/uploads/2013/04/clockface.gif

Public Class ClockPanel : Inherits Panel

   Friend WithEvents Timer As New Timer()

   Public Sub New()
      Me.DoubleBuffered = True
      Timer.Interval = 1000
      Timer.Start()
   End Sub

   ' Find a point on a circle's circumference given the circle's origin, radius and degrees.
   Private Function FindPointOnCircle(originPoint As Point, radius As Double, angleDegrees As Double) As Point
        Dim x As Double = radius * Math.Cos(Math.PI * angleDegrees / 180.0) + originPoint.X
        Dim y As Double = radius * Math.Sin(Math.PI * angleDegrees / 180.0) + originPoint.Y

        Return New Point(x, y)
    End Function


    ' Draw an individual hand on the clock given the origin and the point on the clock.
    Private Sub DrawHand(originPoint As Point, endPoint As Point, g As Graphics, Optional aPen As Pen = Nothing)
        If aPen Is Nothing Then
            Using BlackPen = New Pen(Brushes.Black)
                BlackPen.Width = 8
                g.DrawLine(BlackPen, originPoint, endPoint)
            End Using
        Else
            g.DrawLine(aPen, originPoint, endPoint)
        End If
    End Sub


    Private Function DrawClock() As Image
        Dim dt As DateTime = DateTime.Now

        Dim clockImage As Image = ConvertImageToRGBFormat(My.Resources.clockface)
        Dim clockGraphicsObj As Graphics = Graphics.FromImage(clockImage)

        ' Radius of minute hand 70% of half the width of the panel
        Dim radius As Double = (clockImage.Width / 2) * 0.7

        ' Origin half of width and height of panel
        Dim origin As New Point(clockImage.Width / 2, clockImage.Height / 2)

        ' Calculate degrees for each tick of the hand. 6 degrees for minutes and seconds (360 / 60)
        ' And 30 degrees for each hour tick (360 / 12)
        ' Subtract 90 to start hand from Noon/Midnight

        Dim degreesMinutes As Double = (dt.Minute * 6) - 90.0
        Dim degreesHours As Double = (dt.Hour * 30) - 90.0
        Dim degreesSeconds As Double = (dt.Second * 6) - 90.0


        ' Find the point on the circle the hand needs to point to
        ' Hour hand is half the length of the other two hands.
        Dim minutesPoint As Point = FindPointOnCircle(origin, radius, degreesMinutes)
        Dim hoursPoint As Point = FindPointOnCircle(origin, radius / 2, degreesHours)
        Dim secondsPoint As Point = FindPointOnCircle(origin, radius, degreesSeconds)


        ' Draw minutes and hours with normal default black pen
        DrawHand(origin, minutesPoint, clockGraphicsObj)
        DrawHand(origin, hoursPoint, clockGraphicsObj)

        ' Seconds hand is drawn with a red pen of width 4
        Using p As New Pen(Brushes.Red)
            p.Width = 4
            DrawHand(origin, secondsPoint, clockGraphicsObj, p)
        End Using

        Return clockImage
    End Function


    ' Function handles converting images to an 32 bit RGB pixel format
    Private Function ConvertImageToRGBFormat(img As Image) As Image
        If Not img.PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb Then
            Dim temp As Bitmap = New Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb)
            Dim g As Graphics = Graphics.FromImage(temp)
            g.DrawImage(img, New Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel)
            g.Dispose()

            Return temp
        End If

        Return img
    End Function


    ' Override the Panel's paint method, draw the clock and then call the base paint event
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)
        e.Graphics.DrawImage(DrawClock(), 0, 0, Me.Width, Me.Height)
    End Sub

   Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick
      Me.Refresh()
   End Sub
End Class
