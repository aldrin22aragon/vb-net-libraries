<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.NFaceView1 = New Neurotec.Biometrics.Gui.NFaceView()
        Me.ClockPanel1 = New vb_net_libraries.ClockPanel()
        Me.SuspendLayout()
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1000
        '
        'NFaceView1
        '
        Me.NFaceView1.BackColor = System.Drawing.SystemColors.ControlDarkDark
        Me.NFaceView1.Face = Nothing
        Me.NFaceView1.FaceIds = Nothing
        Me.NFaceView1.IcaoArrowsColor = System.Drawing.Color.Red
        Me.NFaceView1.Location = New System.Drawing.Point(349, 111)
        Me.NFaceView1.Name = "NFaceView1"
        Me.NFaceView1.ShowIcaoArrows = True
        Me.NFaceView1.ShowTokenImageRectangle = True
        Me.NFaceView1.Size = New System.Drawing.Size(200, 162)
        Me.NFaceView1.TabIndex = 1
        Me.NFaceView1.TokenImageRectangleColor = System.Drawing.Color.White
        '
        'ClockPanel1
        '
        Me.ClockPanel1.Location = New System.Drawing.Point(52, 95)
        Me.ClockPanel1.Name = "ClockPanel1"
        Me.ClockPanel1.Size = New System.Drawing.Size(189, 178)
        Me.ClockPanel1.TabIndex = 0
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.NFaceView1)
        Me.Controls.Add(Me.ClockPanel1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Timer1 As Timer
    Friend WithEvents ClockPanel1 As ClockPanel
    Friend WithEvents NFaceView1 As Neurotec.Biometrics.Gui.NFaceView
End Class
