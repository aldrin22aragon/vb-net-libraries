Namespace DataTables
   Public Class Table
      Event TablePropsChange(obj As Table, e As String)
      Friend WithEvents TxtSearch As TextBox
      ReadOnly Props As TableProperties
      Sub New(props_ As TableProperties)
         Props = props_
         TxtSearch = Props.TxtSearch
      End Sub

      Private Sub Tb_TextChanged(sender As Object, e As EventArgs) Handles TxtSearch.TextChanged
         RaiseEvent TablePropsChange(Me, "changed")
      End Sub
   End Class

   Public Class TableProperties
      Public TxtSearch As TextBox
   End Class
End Namespace
