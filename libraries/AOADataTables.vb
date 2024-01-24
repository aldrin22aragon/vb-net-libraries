Public Class AOADataTables
   Friend WithEvents _showCount As ComboBox
   Friend WithEvents _search As TextBox
   Friend WithEvents _1 As Button
   Friend WithEvents _2 As Button
   Friend WithEvents _3 As Button
   Friend WithEvents _4 As Button
   Friend WithEvents _5 As Button
   Friend WithEvents _next As Button
   Friend WithEvents _prev As Button
   Friend WithEvents _dgv As DataGridView
   Friend WithEvents _info As Label
   Public Event TablePropertiesChanged(TableProperties As TableProperties)

   Sub New(showCount As ComboBox,
           search As TextBox,
           __1 As Button,
           __2 As Button,
           __3 As Button,
           __4 As Button,
           __5 As Button,
           __next As Button,
           __prev As Button,
           __dgv As DataGridView,
           __info As Label)
      _showCount = showCount
      _search = search
      _1 = __1
      _2 = __2
      _3 = __3
      _4 = __4
      _5 = __5
      _next = __next
      _prev = __prev
      _dgv = __dgv
      _info = __info
      If _showCount.Items.Count < 1 Then
         _showCount.DropDownStyle = ComboBoxStyle.DropDownList
         _showCount.FlatStyle = FlatStyle.Standard
         _showCount.Items.AddRange({10, 25, 50, 100})
      End If
      _showCount.SelectedIndex = 0
   End Sub
   Public Sub setDataTable(res As Response)
      Dim displayCOunt As Integer = Val(_showCount.SelectedItem.ToString)
      Dim pageNumber As Integer = 0
      _dgv.Rows.Clear()
      For Each i As String() In res.aaData
         _dgv.Rows.Add(i)
      Next
      Dim start As Integer = (pageNumber * displayCOunt) - (displayCOunt - 1)
      Dim _end As Integer = (pageNumber * displayCOunt)
      If _end > res.iTotalDisplayRecords Then
         _end = res.iTotalDisplayRecords
      End If
      _info.Text = String.Concat("Showing ", start, " to ", _end, " of ", res.iTotalDisplayRecords.ToString("N0"), " entries")
   End Sub

   Class TableProperties
      Public showCount As Integer = 10
      Public search As String = ""
      Public page As Integer = 1

   End Class


   Class Response
      Public aaData As List(Of Object())
      Public draw As Integer
      Public iTotalDisplayRecords As Integer
      Public iTotalRecords As Integer
      'Public sDraw As String = ""
      'Public sEcho As Integer = 0
   End Class

   Property TableProp As TableProperties
      Get
         Dim res As New TableProperties
         res.showCount = Val(_showCount.SelectedItem.ToString)
         res.search = _search.Text.ToString

         Return res
      End Get
      Set(value As TableProperties)

      End Set
   End Property

#Region "Event Raiser"
   Public Sub Refresh()
      RaiseEvent TablePropertiesChanged(New TableProperties() With {.showCount = 600})
   End Sub
   Private Sub _1_Click(sender As Object, e As EventArgs) Handles _1.Click, _2.Click, _3.Click, _4.Click, _next.Click, _prev.Click
      RaiseEvent TablePropertiesChanged(New TableProperties() With {.showCount = 600})
   End Sub
   Private Sub _showCount_SelectedValueChanged(sender As Object, e As EventArgs) Handles _showCount.SelectedValueChanged
      RaiseEvent TablePropertiesChanged(New TableProperties() With {.showCount = 600})
   End Sub
   Private Sub _search_KeyUp(sender As Object, e As KeyEventArgs) Handles _search.KeyUp
      RaiseEvent TablePropertiesChanged(New TableProperties() With {.showCount = 600})
   End Sub

   Private Sub _dgv_Sorted(sender As Object, e As EventArgs) Handles _dgv.Sorted
      RaiseEvent TablePropertiesChanged(New TableProperties() With {.showCount = 600})
   End Sub
#End Region
End Class
