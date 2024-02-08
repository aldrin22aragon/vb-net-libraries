
Public Class Costum_Textbox
   ReadOnly Property _Label As Label
      Get
         Return Label1
      End Get
   End Property
   Property _TextboxFont As Font
      Get
         Return TxMain.Font
      End Get
      Set(value As Font)
         TxMain.Font = value
         Me.Height = TxMain.Height + 14
         FollowMainTextBox()
      End Set
   End Property
   Property _PanelColor As Color
      Get
         Return Me.BackColor
      End Get
      Set(value As Color)
         Me.BackColor = value
      End Set
   End Property

   ReadOnly Property _TextBox As TextBox
      Get
         Return TxMain
      End Get
   End Property

   Property _LabelText As String
      Get
         Return Label1.Text
      End Get
      Set(value As String)
         Label1.Text = value
         FollowMainTextBox()
      End Set
   End Property
   Property _TextBoxValue As String
      Get
         Return TxMain.Text
      End Get
      Set(value As String)
         TxMain.Text = value
         FollowMainTextBox()
      End Set
   End Property

   Private Sub Costum_Textbox_Load(sender As Object, e As EventArgs) Handles MyBase.Load
      Me.BackColor = Color.Transparent
      FollowMainTextBox()
   End Sub
   Sub FollowMainTextBox()
      TxPlaceHolder.Font = TxMain.Font
      TxPlaceHolder.Width = TxMain.Width
      TxPlaceHolder.Location = TxMain.Location
      If TxMain.Text <> "" Then
         TxPlaceHolder.Visible = False
         Label1.Visible = True
      Else
         TxPlaceHolder.Text = Label1.Text
         TxPlaceHolder.Visible = True
         Label1.Visible = False
      End If
   End Sub
   Dim dontTriggerLostFocuse As Boolean = False
   Private Sub TxPlaceHolder_GotFocus(sender As Object, e As EventArgs) Handles TxPlaceHolder.GotFocus
      TxPlaceHolder.Visible = False
      Label1.Visible = True
      TxMain.Select()
   End Sub

   Private Sub TxMain_LostFocus(sender As Object, e As EventArgs) Handles TxMain.LostFocus
      FollowMainTextBox()
   End Sub
   Public Event _KeyPress(sender As Object, e As KeyPressEventArgs)
   Private Sub TxMain_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxMain.KeyPress
      RaiseEvent _KeyPress(sender, e)
   End Sub
   Public Event _KeyKeyUp(sender As Object, e As KeyEventArgs)
   Private Sub TxMain_KeyUp(sender As Object, e As KeyEventArgs) Handles TxMain.KeyUp
      RaiseEvent _KeyKeyUp(sender, e)
   End Sub
   Public Event _KeyKeyDown(sender As Object, e As KeyEventArgs)
   Private Sub TxMain_KeyDown(sender As Object, e As KeyEventArgs) Handles TxMain.KeyDown
      RaiseEvent _KeyKeyDown(sender, e)
   End Sub
End Class
