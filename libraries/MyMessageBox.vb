Public Class MyMessageBox
   Shared Sub ErrMsg(msg As String)
      MessageBox.Show(msg, "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error)
   End Sub
   Shared Sub SuccessMsg(msg As String)
      MessageBox.Show(msg, "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information)
   End Sub
   Public Shared Function YesNo(msg As String, Optional header As String = "????", Optional icon As Windows.Forms.MessageBoxIcon = MessageBoxIcon.Question) As DialogResult
      Return MessageBox.Show(msg, header, MessageBoxButtons.YesNo, icon, MessageBoxDefaultButton.Button2)
   End Function
   Public Shared Function YesNoCancel(msg As String, Optional header As String = "????", Optional icon As Windows.Forms.MessageBoxIcon = MessageBoxIcon.Question) As DialogResult
      Dim res As DialogResult = DialogResult.Cancel
      res = MessageBox.Show(msg, header, MessageBoxButtons.YesNoCancel, icon, MessageBoxDefaultButton.Button2)
      Return res
   End Function
   Public Shared Function okCancel(msg As String) As DialogResult
      Return MessageBox.Show(msg, "!!!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
   End Function
End Class
