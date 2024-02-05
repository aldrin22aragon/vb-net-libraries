Imports System.IO
Imports WinSCP
Public Class FtpSettings
   'new comment
   Public ReturnSessionOptions As SessionOptions = Nothing

   Private Sub BtnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnCancel.Click
      Me.Close()
   End Sub

   Sub New(sesOptions As SessionOptions)

      ' This call is required by the designer.
      InitializeComponent()
      ReturnSessionOptions = sesOptions
      ' Add any initialization after the InitializeComponent() call.

   End Sub

   Private Sub CmbConnectionType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbConnectionType.SelectedIndexChanged
      Dim type = CmbConnectionType.SelectedItem.ToString
      If LCase(type) = "sftp" Then
         TxtHostkey.Visible = True
      ElseIf LCase(type) = "ftp" Then
         TxtHostkey.Visible = False
      End If
   End Sub

   Property FormValue As SessionOptions
      Get
         Dim res As New SessionOptions With {
            .HostName = TxtHost.Text,
            .PortNumber = TxtPort.Value,
            .UserName = TxtUsername.Text,
            .Password = TxtPassword.Text,
            .Protocol = IIf(CmbConnectionType.SelectedItem = "FTP", Protocol.Ftp, Protocol.Sftp)
         }
         If res.Protocol = Protocol.Sftp Then
            res.SshHostKeyFingerprint = TxtHostkey.Text
         End If
         Return res
      End Get
      Set(value As SessionOptions)
         TxtHost.Text = ""
         TxtPort.Value = 21
         TxtUsername.Text = ""
         TxtPassword.Text = ""
         CmbConnectionType.SelectedIndex = 0
         TxtHostkey.Text = ""
         If value IsNot Nothing Then
            TxtHost.Text = value.HostName
            TxtPort.Value = value.PortNumber
            TxtUsername.Text = value.UserName
            TxtPassword.Text = value.Password
            CmbConnectionType.SelectedItem = IIf(value.Protocol = Protocol.Ftp, "FTP", "SFTP")
            TxtHostkey.Text = value.SshHostKeyFingerprint
         End If
      End Set
   End Property

   Private Sub FrmSettings_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      FormValue = ReturnSessionOptions
   End Sub

   Private Sub BtnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnSave.Click
      ReturnSessionOptions = FormValue
      DialogResult = DialogResult.OK
   End Sub

   Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnTestConnect.Click
      BtnTestConnect.Text = "Connecting..."
      BtnTestConnect.Enabled = False
      Try
         Dim session As Session = New Session
         session.Open(FormValue)
         MessageBox.Show("Successfully Connected", "FTP Test", MessageBoxButtons.OK, MessageBoxIcon.Information)
         session.Dispose()
         session = Nothing
         BtnTestConnect.Text = "Test Connection"
         BtnTestConnect.Enabled = True
      Catch ex As Exception
         MessageBox.Show("Error Connecting to FTP Protocol!" & vbNewLine & ex.Message, "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error)
         BtnTestConnect.Text = "Test Connection"
         BtnTestConnect.Enabled = True
         Exit Sub
      End Try
   End Sub


   Private Sub TxtPort_ValueChanged(sender As Object, e As EventArgs) Handles TxtPort.ValueChanged
      If TxtPort.Value = 22 Then
         CmbConnectionType.SelectedItem = "SFTP"
      Else
         CmbConnectionType.SelectedItem = "FTP"
      End If
   End Sub
End Class