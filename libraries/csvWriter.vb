Imports System.IO
Public Class CsvWriter : Implements IDisposable
   Public Shared wrtr As StreamWriter
   Public filePath As String = ""
   Public fileName As String = ""
   Public liness As New List(Of String)
   '

   Private ReadOnly separator As String = ""
   Private ReadOnly addSurroundDoubleQuotesEachValue As Boolean
   Private tmpLine As String = ""

   ''' <summary>
   ''' 
   ''' </summary>
   ''' <param name="pth"></param>
   ''' <param name="append"></param>
   ''' <remarks></remarks>
   Public Sub New(pth As String, append As Boolean, encoding As System.Text.Encoding, Optional separator As String = ",", Optional _addSurroundDoubleQuotesEachValue As Boolean = False)
      wrtr = New StreamWriter(pth, append, encoding)
      filePath = pth
      Me.separator = separator
      Me.addSurroundDoubleQuotesEachValue = _addSurroundDoubleQuotesEachValue
      fileName = IO.Path.GetFileName(filePath)
      liness.Clear()
   End Sub
   ''' <summary>
   ''' closes the writer.
   ''' Using GC.Collect()  
   ''' and GC.WaitForPendingFinalizers() 
   ''' </summary>
   ''' <remarks></remarks>
   Public Sub Close()
      wrtr.Close()
      GC.Collect()
      GC.WaitForPendingFinalizers()
   End Sub


   Private Sub AddLines(ByVal str As String)
      If tmpLine <> Nothing Then
         liness(liness.Count - 1) = liness(liness.Count - 1) & str
      Else
         liness.Add(str)
      End If
      tmpLine = Nothing
   End Sub

   Public Sub WritLine(ByVal ParamArray values() As String)
      Dim line As String = ""
      For i As Integer = 0 To values.Length - 1
         Dim ln As String = values(i)
         Dim vl As String = AdjustValueCheckCommas(ln)
         If i = 0 Then
            line = vl
         Else
            line = line & separator & vl
         End If
      Next
      wrtr.WriteLine(line)
      AddLines(line)
   End Sub

   ''' <summary>
   ''' 
   ''' </summary>
   ''' <param name="n"></param>
   ''' <returns></returns>
   ''' <remarks></remarks>
   Private Function AdjustValueCheckCommas(ByVal n As String) As String
      Dim sor As Boolean = False
      If n.Contains(",") Or n.Contains("""") Then
         sor = True
      End If
      n = n.Replace("""", """""")
      If sor Or addSurroundDoubleQuotesEachValue Then
         n = """" & n & """"
      End If
      Return n
   End Function

   Public Sub Dispose() Implements IDisposable.Dispose
      Try
         Close()
      Catch ex As Exception
      End Try
   End Sub
End Class
