
Public Class MyExcel
   ' Public xlWorkBook As Excel.Workbook
   ' Public xlWorkSheet As Excel.Worksheet
   ' Public xlRange As Excel.Range
   ' Public misValue As Object = System.Reflection.Missing.Value
   ' Public xlApp As Excel.Application
   ' Public xlPath As String
   ' Public xlFileName As String
   ' Public xlFileNameWoExtension As String

   'Public Function isExcelInstalled() As Boolean
   '   If xlApp Is Nothing Then
   '      MessageBox.Show("Excel is not properly installed!!")
   '      Return False
   '   Else : Return True
   '   End If
   'End Function

   'Public Sub New(ByVal path As String, ByVal overwrite As Boolean)
   '   Try
   '      If overwrite And IO.File.Exists(path) Then
   '         IO.File.Delete(path)
   '      End If
   '      xlApp = New Excel.Application
   '      xlWorkBook = xlApp.Workbooks.Add()
   '      xlPath = path
   '      xlFileName = IO.Path.GetFileName(path)
   '      xlFileNameWoExtension = IO.Path.GetFileNameWithoutExtension(path)
   '   Catch ex As Exception
   '      MsgBox("Error in setting xls file\\\".Replace("\", vbNewLine) & ex.Message, MsgBoxStyle.Critical)
   '   End Try
   'End Sub

   'Public Function cells(ByVal sheet As Integer, ByVal row As Integer, ByVal col As Integer, ByVal otherCelRow As Integer, ByVal otherCelCol As Integer) As Excel.Range
   '   Return Me.sheet(sheet).Range(ColumnIndexToColumnLetter(col) & row, ColumnIndexToColumnLetter(otherCelCol) & otherCelRow)
   'End Function

   'Public Function cell(ByVal sheet As Integer, ByVal row As Integer, ByVal col As Integer) As Excel.Range
   '   Return Me.sheet(sheet).Range(ColumnIndexToColumnLetter(col) & row, ColumnIndexToColumnLetter(col) & row)
   'End Function

   'Function sheet(ByVal sheeet As Integer) As Excel.Worksheet
   '   Return xlWorkBook.Sheets("Sheet" & sheeet)
   'End Function

   'Function save() As Boolean
   '   Try
   '      xlWorkBook.SaveAs(xlPath, _
   '                        Excel.XlFileFormat.xlWorkbookNormal, _
   '                        misValue, _
   '                        misValue, _
   '                        misValue, _
   '                        misValue, _
   '                        Excel.XlSaveAsAccessMode.xlExclusive, _
   '                        misValue, _
   '                        misValue, _
   '                        misValue, _
   '                        misValue)
   '      xlWorkBook.Close(True, misValue, misValue)
   '      releaseObject(xlWorkSheet)
   '      releaseObject(xlWorkBook)
   '      releaseObject(xlApp)
   '      xlWorkSheet = Nothing
   '      xlWorkBook = Nothing
   '      xlApp.Quit()
   '      xlApp = Nothing
   '      Return True
   '   Catch ex As Exception
   '      MsgBox("Unable trying to save Excel file\\\".Replace("\", vbNewLine) & ex.Message, MsgBoxStyle.Critical)
   '      Return False
   '   End Try
   'End Function

   'Public Function ColumnIndexToColumnLetter(ByVal colIndex As Integer) As String
   '   Dim div As Integer = colIndex
   '   Dim colLetter As String = String.Empty
   '   Dim modnum As Integer = 0

   '   While div > 0
   '      modnum = (div - 1) Mod 26
   '      colLetter = Chr(65 + modnum) & colLetter
   '      div = CInt((div - modnum) \ 26)
   '   End While

   '   Return colLetter
   'End Function

   'Private Sub releaseObject(ByVal obj As Object)
   '   Try
   '      System.Runtime.InteropServices.Marshal.ReleaseComObject(obj)
   '      obj = Nothing
   '   Catch ex As Exception
   '      obj = Nothing
   '   Finally
   '      GC.Collect()
   '   End Try
   'End Sub
End Class
