Imports System.Net.Mail
Public Class SendEmail
   Dim th As Threading.Thread
   ReadOnly EmailProperty As Props = Nothing
   ReadOnly MailMessageProperty As MailMessage = Nothing
   Property Status As EStatus = EStatus.NONE
   Property Exception As Exception = Nothing
   ''' <summary>
   ''' This automatically disposes all attachment tha have been used.
   ''' </summary>
   ''' <param name="EmailProp"></param>
   ''' <param name="Mail"></param>
   Sub New(EmailProp As Props, Mail As MailMessage)
      Me.EmailProperty = EmailProp
      Me.MailMessageProperty = Mail
   End Sub

   Sub StartSend()
      th = New System.Threading.Thread(AddressOf RunThread)
      th.Start()
      Status = EStatus.Running
   End Sub
   Private Sub RunThread()
      Try
         Status = EStatus.SettingUpCredentials
         Dim client As New SmtpClient() With {
            .Host = EmailProperty.Host,
            .Port = EmailProperty.Port,
            .DeliveryFormat = EmailProperty.DeliveryFormat, ' default SmtpDeliveryFormat.International
            .EnableSsl = EmailProperty.EnableSsl ' default  false
         }
         client.Credentials = New Net.NetworkCredential() With {
            .UserName = EmailProperty._UserName, 'email
            .Password = EmailProperty._Password,
            .Domain = EmailProperty._Domain 'default blank
         }
         Try
            Status = EStatus.Sending
            client.Send(MailMessageProperty)
            Status = EStatus.EmailSent
         Catch ex As Exception
            Exception = ex
            Status = EStatus.Error
         Finally
            MailMessageProperty.Attachments.Dispose()
            If MailMessageProperty.Attachments.Count > 0 Then MailMessageProperty.Attachments.Clear()
            MailMessageProperty.Dispose()
            client.Dispose()
            GC.Collect()
            GC.WaitForPendingFinalizers()
         End Try
      Catch ex As Exception
         Exception = ex
         Status = EStatus.Error
      End Try

   End Sub

   Class Props
      Public Host As String = ""
      Public Port As String = ""
      ''' <summary>
      ''' default is SevenBit
      ''' </summary>
      Public DeliveryFormat As SmtpDeliveryFormat = SmtpDeliveryFormat.SevenBit
      ''' <summary>
      ''' default is false
      ''' </summary>
      Public EnableSsl As Boolean = False '
      Public _UserName As String = ""
      Public _Password As String = ""
      ''' <summary>
      ''' Defaul Blank
      ''' </summary>
      Public _Domain As String = ""
   End Class

   Enum EStatus As Integer
      NONE = 0
      Running = 1
      Sending = 2
      EmailSent = 3
      [Error] = 4
      SettingUpCredentials = 5
   End Enum

End Class
