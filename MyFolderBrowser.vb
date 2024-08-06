Public Class MyFolderBrowser
    Property InitialPath As String = ""
    Property SelectedPath As String = ""
    Sub New(initialPath As String)

        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        Me.InitialPath = initialPath
    End Sub
    Private Sub MyFolderBrowser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            DirListBox1.Path = InitialPath
        Catch ex As Exception
        End Try
    End Sub
    Private Sub MyFolderBrowser_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub DriveListBox1_SelectedValueChanged(sender As Object, e As EventArgs) Handles DriveListBox1.SelectedValueChanged
        DirListBox1.Path = DriveListBox1.Drive
    End Sub

    Private Sub DirListBox1_DoubleClick(sender As Object, e As EventArgs) Handles DirListBox1.DoubleClick
        TextBox1.Text = DirListBox1.DirList(-1)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text <> "" Then
            SelectedPath = TextBox1.Text
            Me.DialogResult = DialogResult.OK
        Else
            MsgBox("Please select a folder first.")
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        MyFolderBrowser_KeyDown(New Object, New KeyEventArgs(Keys.Escape))
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        SelectedPath = ""
        DialogResult = DialogResult.OK
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim i As String = InputBox("Please paste directory.", "Open folder")
        If IO.Directory.Exists(i) Then
            DirListBox1.Path = i
        Else
            MsgBox("Directory not found")
        End If
    End Sub
End Class