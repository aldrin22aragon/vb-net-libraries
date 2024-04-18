
Imports vb_net_libraries.DataTables
Public Class Form1
   Friend WithEvents DT As Table
   Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
      DT = New Table(New TableProperties() With {
         .TxtSearch = TextBox1
      })
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

   Private Sub DT_TablePropsChange(obj As Table, e As String) Handles DT.TablePropsChange
      e = e
   End Sub
End Class
