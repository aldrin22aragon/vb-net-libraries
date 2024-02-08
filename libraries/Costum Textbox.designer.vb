<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Costum_Textbox
   Inherits System.Windows.Forms.UserControl

   'UserControl overrides dispose to clean up the component list.
   <System.Diagnostics.DebuggerNonUserCode()> _
   Protected Overrides Sub Dispose(ByVal disposing As Boolean)
      Try
         If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
         End If
      Finally
         MyBase.Dispose(disposing)
      End Try
   End Sub

   'Required by the Windows Form Designer
   Private components As System.ComponentModel.IContainer

   'NOTE: The following procedure is required by the Windows Form Designer
   'It can be modified using the Windows Form Designer.  
   'Do not modify it using the code editor.
   <System.Diagnostics.DebuggerStepThrough()> _
   Private Sub InitializeComponent()
      Me.Label1 = New System.Windows.Forms.Label()
      Me.TxMain = New System.Windows.Forms.TextBox()
      Me.TxPlaceHolder = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Segoe UI Semibold", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.Label1.Location = New System.Drawing.Point(-2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(40, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Label1"
        Me.Label1.Visible = False
        '
        'TxMain
        '
        Me.TxMain.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.TxMain.Location = New System.Drawing.Point(0, 15)
        Me.TxMain.Name = "TxMain"
        Me.TxMain.Size = New System.Drawing.Size(257, 20)
        Me.TxMain.TabIndex = 2
        '
        'TxPlaceHolder
        '
        Me.TxPlaceHolder.BackColor = System.Drawing.SystemColors.Window
        Me.TxPlaceHolder.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.TxPlaceHolder.Location = New System.Drawing.Point(43, 12)
        Me.TxPlaceHolder.Name = "TxPlaceHolder"
        Me.TxPlaceHolder.Size = New System.Drawing.Size(100, 20)
        Me.TxPlaceHolder.TabIndex = 3
        Me.TxPlaceHolder.Text = "safsdf"
        '
        'Costum_Textbox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Turquoise
        Me.Controls.Add(Me.TxPlaceHolder)
        Me.Controls.Add(Me.TxMain)
        Me.Controls.Add(Me.Label1)
        Me.Name = "Costum_Textbox"
        Me.Size = New System.Drawing.Size(257, 35)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As Label
   Friend WithEvents TxMain As TextBox
    Friend WithEvents TxPlaceHolder As TextBox
End Class
