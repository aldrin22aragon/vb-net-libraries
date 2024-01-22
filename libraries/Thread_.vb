Public Class Thread_

   Public wrkr As New System.ComponentModel.BackgroundWorker
   Public stat As Integer
   Public isWorking As Boolean = False
   Public isDone As Boolean = False
   Public max As Integer

   Public Sub doThis()
      Multi_Thread.onProcess += 1
      isWorking = True
      Me.wrkr.RunWorkerAsync()
   End Sub

   Sub New()
      Randomize()
      max = CInt(Int((40 * Rnd()) + 1))
      AddHandler wrkr.DoWork, AddressOf doWork
      AddHandler wrkr.RunWorkerCompleted, AddressOf wrkrDone
      AddHandler wrkr.ProgressChanged, AddressOf wrkrChanged
   End Sub

   Private Sub doWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
      For i As Integer = 1 To max
         stat = i
         System.Threading.Thread.Sleep(400)
      Next
   End Sub

   Private Sub wrkrChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs)

   End Sub

   Private Sub wrkrDone(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)
      Multi_Thread.onProcess -= 1
      isDone = True
      isWorking = False
      wrkr.Dispose()
   End Sub

End Class
