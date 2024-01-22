Public Class TextboxPlaceholder
    Dim lbl As Label
    Dim controls() As Control
    Dim PlaceholderForeColor As Color = Color.FromArgb(64, 64, 64)
    ''' <summary>
    ''' Set tag attribute of textbox control to change label text
    ''' Reminder Label text may depend on tag attribute of textbox
    ''' </summary>
    ''' <param name="label"></param>
    ''' <param name="controls"></param>
    Sub New(label As Label, ParamArray controls() As Control)
        Me.lbl = label
        Me.lbl.Visible = False
        Me.lbl.BackColor = Color.FromArgb(0, 255, 255, 255)
        Me.controls = controls
        For Each i As Control In Me.controls
            Try
                If i.Text = Nothing Or i.Text = "" Then
                    i.ForeColor = PlaceholderForeColor
                End If
            Catch ex As Exception
            End Try
            AddHandler i.GotFocus, AddressOf TextBox1_GotFocus
            AddHandler i.LostFocus, AddressOf TextBox1_LostFocus
        Next
    End Sub
    Sub TextBox1_GotFocus(sender As Object, e As EventArgs)
        Dim c As Control = CType(sender, Control)
        lbl.Parent = c.Parent
        lbl.Text = IIf(c.Tag IsNot Nothing, c.Tag, "")
        lbl.Font = c.Font
        Dim tp As Integer = (c.Top - lbl.Height) - 3
        lbl.Top = tp
        lbl.Left = c.Left
        lbl.Visible = True
        If c.ForeColor = PlaceholderForeColor Then
            c.ForeColor = Color.Black
        End If
        For i As Integer = (lbl.Left + 40) To lbl.Left Step -1
            lbl.Left = i
            Application.DoEvents()
        Next
    End Sub
    Sub TextBox1_LostFocus(sender As Object, e As EventArgs)
        Dim c As Control = CType(sender, Control)
        If c.Text = Nothing Or c.Text = "" Then
            c.ForeColor = PlaceholderForeColor
        End If
        lbl.Visible = False
    End Sub
End Class
