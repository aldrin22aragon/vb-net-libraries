Imports System.IO
Public Class CsvWriter
   Public Shared wrtr As StreamWriter
   Public path As String = ""
   Public name As String = ""
   Public separator As String = ""
   Public liness As New List(Of String)
   Private addSurroundDoubleQuotesEachValue As Boolean
   Private tmpLine As String = ""
   ''' <summary>
   ''' 
   ''' </summary>
   ''' <param name="pth"></param>
   ''' <param name="encoding"></param>
   ''' <param name="append"></param>
   ''' <remarks></remarks>
   Public Sub New(ByVal pth As String, ByVal encoding As System.Text.Encoding, ByVal append As Boolean, Optional separator As String = ",", Optional _addSurroundDoubleQuotesEachValue As Boolean = False)
      wrtr = New StreamWriter(pth, append, encoding)
      path = pth
      Me.separator = separator
      Me.addSurroundDoubleQuotesEachValue = _addSurroundDoubleQuotesEachValue
      name = IO.Path.GetFileName(path)
      liness.Clear()
   End Sub
   ''' <summary>
   ''' closes the writer.
   ''' Using GC.Collect()  
   ''' and GC.WaitForPendingFinalizers() 
   ''' </summary>
   ''' <remarks></remarks>
   Public Sub close()
      wrtr.Close()
      GC.Collect()
      GC.WaitForPendingFinalizers()
   End Sub


   Private Function addLines(ByVal str As String) As Boolean
      If tmpLine <> Nothing Then
         liness(liness.Count - 1) = liness(liness.Count - 1) & str
      Else
         liness.Add(str)
      End If
      tmpLine = Nothing
   End Function

   Public Function writLine(ByVal ParamArray values() As String) As Boolean
      Dim line As String = ""
      For i As Integer = 0 To values.Length - 1
         Dim ln As String = values(i)
         Dim vl As String = adjustValueCheckCommas(ln)
         If i = 0 Then
            line = vl
         Else
            line = line & separator & vl
         End If
      Next
      wrtr.WriteLine(line)
      addLines(line)
   End Function
   ' ''' <summary>
   ' ''' 
   ' ''' </summary>
   ' ''' <param name="n"></param>
   ' ''' <returns></returns>
   ' ''' <remarks></remarks>
   'Public Function write(ByVal n As String) As Boolean
   '   wrtr.Write(n)
   '   If tmpLine = Nothing Then
   '      liness.Add(n)
   '   Else
   '      liness(liness.Count - 1) = liness(liness.Count - 1) & n
   '   End If
   '   tmpLine = tmpLine & n
   'End Function


   ''' <summary>
   ''' 
   ''' </summary>
   ''' <param name="n"></param>
   ''' <returns></returns>
   ''' <remarks></remarks>
   Function adjustValueCheckCommas(ByVal n As String) As String
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

End Class
