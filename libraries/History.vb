Public Class history_
    Dim curNdx As Integer = -1
    Dim lst As New List(Of String)
    Sub New()
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="changeCurentHistory"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
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