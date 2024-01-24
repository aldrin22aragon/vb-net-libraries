' please add WinSCP.exe startup Path 5.17.8.10803
' add reference WinSCPnet.dll 5.17.8.0

Public Class FtpBrowser
   Dim sessionOptions As WinSCP.SessionOptions = Nothing
   Dim lastPath As String = ""

   Property DefPath As String = ""
   Property mode As Mode_ = Mode_.none
   Property multiSelect As Boolean = False
   'Property hasSelected As Boolean = False

   Private directoryWord As String = "Directory"
   Private fileWord As String = "File"

   Dim history As history_
   Public ReturnString As String = ""
   Public ReturnArray As String() = {}
   Public ReturnFileInfoCollection As New Dictionary(Of String, WinSCP.RemoteFileInfo)

   Friend WithEvents bg As System.ComponentModel.BackgroundWorker
   Friend WithEvents tmr As Timer

   Enum Mode_ As Integer
      none = 0
      file = 1
      folder = 2
   End Enum
   Sub New(sessionOptions_ As WinSCP.SessionOptions, md As Mode_, Optional defPath As String = "/", Optional multipleSelection As Boolean = False)
      ' This call is required by the designer.
      InitializeComponent()
      ' Add any initialization after the InitializeComponent() call.
      Me.DefPath = defPath
      Me.sessionOptions = sessionOptions_
      mode = md
      lastPath = ""
      ReturnString = ""
      ReturnArray = {}
      history = New history_
      Me.multiSelect = multipleSelection
      Me.DialogResult = Windows.Forms.DialogResult.Cancel
      bg = New System.ComponentModel.BackgroundWorker
      bg.WorkerReportsProgress = True
      tmr = New Timer
   End Sub
   Private Sub FtpBrowser_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
      If e.KeyCode = Keys.Escape Then
         Me.Close()

      End If
   End Sub
   Private Sub FtpBrowser_Load(sender As Object, e As EventArgs) Handles Me.Load
      lv.MultiSelect = multiSelect
      If mode = Mode_.folder Then
         Me.Text = "Please select " & directoryWord
         btnSelect.Text = String.Concat("Select ", directoryWord)
      ElseIf mode = Mode_.file Then
         Me.Text = "Please select " & fileWord
         btnSelect.Text = String.Concat("Select ", fileWord)
      End If
   End Sub

   Dim processing As Boolean = False

   Function CreateLabel(name As String, text As String, path As String, loc As Point) As LinkLabel
      Dim c As New LinkLabel
      c.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
      c.Location = loc
      c.Name = name
      c.TabStop = True
      c.Text = text
      c.Visible = True
      c.Tag = path
      c.AutoSize = True
      AddHandler c.LinkClicked, AddressOf linkClick
      Panel1.Controls.Add(c)
      Return c
   End Function

   Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnrefresh.Click
      RunBackgroungWorker(lastPath)
   End Sub
   Private Sub FtpBrowser_Shown(sender As Object, e As EventArgs) Handles Me.Shown
      If sessionOptions IsNot Nothing Then
         lastPath = DefPath
         RunBackgroungWorker(DefPath)
      Else
         MsgBox("Please make sure session options is initialized", MsgBoxStyle.Critical)
         Me.Close()
      End If
   End Sub
   Sub linkClick(sender As Object, e As LinkLabelLinkClickedEventArgs)
      Dim link As LinkLabel = CType(sender, LinkLabel)
      If link.Tag IsNot Nothing Then
         Dim pth As String = link.Tag
         RunBackgroungWorker(pth)
      End If
   End Sub
   Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
      If lv.SelectedItems.Count > 0 Then
         If getStrings(Mode_.folder).Length > 0 Then
            RunBackgroungWorker(getStrings(Mode_.folder)(0))
         End If
      End If
   End Sub
   Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnSelect.Click
      If lv.SelectedItems.Count > 0 Then
         If multiSelect Then
            ReturnArray = getStrings(mode)
         Else
            ReturnString = getStrings(mode)(0)
         End If
         For Each i As ListViewItem In lv.SelectedItems
            Dim type As String = i.SubItems(1).Text.ToString.ToUpper
            If (mode = Mode_.folder And type = directoryWord.ToUpper) Or (mode = Mode_.file And type = fileWord.ToUpper) Then
               Dim pth As String = lastPath.TrimEnd("/") & "/" & i.Text.ToString
               ReturnFileInfoCollection.Add(pth, i.Tag)
            End If
         Next
         Me.DialogResult = Windows.Forms.DialogResult.OK
         Me.Close()
      ElseIf mode = Mode_.folder Then
         ReturnString = lastPath
         Me.DialogResult = Windows.Forms.DialogResult.OK
         Me.Close()
      Else
         MsgBox("Please select item" & IIf(Me.multiSelect, "s", ""), MsgBoxStyle.Critical)
      End If
   End Sub
   Function getStrings(md As Mode_) As String()
      Dim res As New List(Of String)
      For Each i As ListViewItem In lv.SelectedItems
         Dim type As String = i.SubItems(1).Text.ToString.ToUpper
         Dim pth As String = lastPath.TrimEnd("/") & "/" & i.Text.ToString
         If (md = Mode_.folder And type = directoryWord.ToUpper) Or (md = Mode_.file And type = fileWord.ToUpper) Then
            res.Add(pth)
         End If
      Next
      Return res.ToArray
   End Function
   Private Sub lv_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lv.SelectedIndexChanged
      If lv.SelectedItems.Count > 0 Then
         Dim itm As ListViewItem = lv.SelectedItems(0)
         If itm.SubItems(1).Text.ToUpper = directoryWord.ToUpper Then
            btnOpen.Enabled = True
         Else
            btnOpen.Enabled = False
         End If
         itm = itm
      Else
         btnOpen.Enabled = False
      End If
   End Sub

   Private Sub lv_DoubleClick(sender As Object, e As EventArgs) Handles lv.DoubleClick
      If btnOpen.Enabled Then
         btnOpen.PerformClick()
      End If
   End Sub

   Public Class history_
      Dim curNdx As Integer = -1
      Dim lst As New List(Of String)
      Sub New()
      End Sub
      Function next_(Optional changeCurentHistory As Boolean = False) As String
         Dim res As String = ""
         Try
            Dim newNdx As Integer = curNdx + 1
            res = lst(newNdx)
            If changeCurentHistory Then
               curNdx = newNdx
            End If
         Catch ex As Exception
            res = ""
         End Try
         Return res
      End Function
      Function Prev_(Optional changeCurentHistory As Boolean = False) As String
         Dim res As String = ""
         Try
            Dim newNdx As Integer = curNdx - 1
            res = lst(newNdx)
            If changeCurentHistory Then
               curNdx = newNdx
            End If
         Catch ex As Exception
            res = ""
         End Try
         Return res
      End Function
      Sub addHistory(hist As String)
         Dim newNdx As Integer = curNdx + 1
         If curNdx = -1 Then
            lst.Add(hist)
            curNdx = 0
         Else
            Dim lastUlr As String = lst(curNdx)
            If lastUlr <> hist Then
               lst.Insert(newNdx, hist)
               curNdx = newNdx
            End If
         End If
      End Sub
   End Class
   Private Sub Button4_Click(sender As Object, e As EventArgs) Handles btnNext_.Click
      If history.next_ <> Nothing Then
         RunBackgroungWorker(history.next_(True))
      End If
   End Sub
   Private Sub BtnPrev__Click(sender As Object, e As EventArgs) Handles BtnPrev_.Click
      If history.Prev_ <> Nothing Then
         RunBackgroungWorker(history.Prev_(True))
      End If
   End Sub
   Dim bgRunThisPath As String = ""
   Sub RunBackgroungWorker(pth As String)
      If Not bg.IsBusy Then
         tmr.Enabled = True
         bgRunThisPath = pth
         bg.RunWorkerAsync()
      End If
   End Sub
   Private Sub bg_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bg.DoWork
      tmr.Enabled = True
      Dim path As String = bgRunThisPath
      If Not processing Then
         processing = True
         Dim fls As WinSCP.RemoteFileInfoCollection = Nothing
         Try
            Dim ses As New WinSCP.Session()
            ses.Open(sessionOptions)
            Try
               Dim directory As WinSCP.RemoteDirectoryInfo = ses.ListDirectory(path)
               fls = directory.Files
            Catch ex As Exception
               MsgBox(("Please try again.§§" & ex.Message).Replace("§", vbNewLine), MsgBoxStyle.Critical, "Error on listing directory.")
            End Try
            ses.Dispose()
            ses = Nothing
         Catch ex As Exception
            MsgBox(("Please try again.§§" & ex.Message).Replace("§", vbNewLine), MsgBoxStyle.Critical, "Error on connecting to ftp")
         End Try
         If fls IsNot Nothing Then
            bg.ReportProgress(1, {path, fls})
         End If
         processing = False
      End If
   End Sub
   Sub enables(bol As Boolean)
      Panel1.Enabled = bol
      lv.Enabled = bol
      btnSelect.Enabled = bol
      btnOpen.Enabled = bol
   End Sub
   Private Sub tmr_Tick(sender As Object, e As EventArgs) Handles tmr.Tick
      If bg.IsBusy Then
         If Not PictureBox1.Visible Then
            PictureBox1.Visible = True
            enables(False)
         End If
      Else
         PictureBox1.Visible = False
         enables(True)
         tmr.Enabled = False
      End If
   End Sub
   Private Sub bg_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles bg.ProgressChanged
      Dim path As String = e.UserState(0).ToString
      Dim fls As WinSCP.RemoteFileInfoCollection = e.UserState(1)
      lastPath = path
      lv.Items.Clear()
      Dim fileInfo As WinSCP.RemoteFileInfo
      Panel1.Controls.Clear()
      For Each fileInfo In fls
         If fileInfo.Name.Trim(".").Trim <> Nothing Then
            If (mode = Mode_.folder And fileInfo.IsDirectory) Or mode = Mode_.file Then
               Dim itm As ListViewItem = lv.Items.Add(fileInfo.Name)
               If fileInfo.IsDirectory Then
                  itm.SubItems.Add(directoryWord)
                  itm.ImageIndex = 1
               Else
                  itm.SubItems.Add(fileWord)
                  itm.ImageIndex = 0
               End If
               itm.Tag = fileInfo
               itm.EnsureVisible()
            End If
         End If
      Next
      Dim tmpName As String = ""
      Dim left_ As Integer = 5
      Dim top_ As Integer = 2
      Dim labelCounts As Integer = 1
      Dim pth As String = ""
      For i As Integer = 0 To path.Length - 1
         Dim vl As String = path(i)
         tmpName = tmpName & vl
         pth = pth & vl
         If vl = "/" Or i = path.Length - 1 Then
            Dim nm As String = "MyLabel" & labelCounts
            Dim loc As New Point(left_, top_)
            Dim lbl As LinkLabel = CreateLabel(nm, tmpName, pth, loc)
            left_ = left_ + lbl.Width
            labelCounts += 1
            tmpName = ""
         End If
      Next
      history.addHistory(path)
      If history.next_ <> Nothing Then
         btnNext_.Enabled = True
      Else
         btnNext_.Enabled = False
      End If
      If history.Prev_ <> Nothing Then
         BtnPrev_.Enabled = True
      Else
         BtnPrev_.Enabled = False
      End If
      lv_SelectedIndexChanged(New Object, New EventArgs)
   End Sub

End Class
