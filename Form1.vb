Public Class Form1

   Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

   End Sub

   Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
   End Sub

   Private Sub Button1_Click(sender As Object, e As EventArgs) 

   End Sub

   Private Sub Button1_Click_1(sender As Object, e As EventArgs) Handles Button1.Click
      Dim aa As New FtpSettings(Nothing)
      Dim s As DialogResult = aa.ShowDialog()
      s = s
   End Sub
End Class
