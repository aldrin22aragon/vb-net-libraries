'' install newtonsoft Json in nugget
Imports System.Net
Imports System.Net.Sockets
Imports Newtonsoft.Json

Public Class AOATCPIP
   Private listener As TcpListener
   Public myPort As Integer
   Public myIPaddress As String
   Public recieverIPaddress As String
   Public recieverPort As String
   Friend WithEvents Timer As Timer
   Event Recieved As EventHandler(Of SendReceInfo)
   ''' <summary>
   ''' Para ito sa mga mag coconect sa host.
   ''' </summary>
   ''' <param name="recieverPort">Yung host na yung mag bibigay kung anong port</param>
   ''' <param name="recieverIPaddress">Yung host na yung mag bibigay kung anong IP Address</param>
   Sub New(recieverPort As String, recieverIPaddress As String)
      Dim ipAddress_ As String = GetAvailableIPAddress()
      Dim ss As Integer = FreeTcpPort()
      If ipAddress_ <> "" Then
         myIPaddress = ipAddress_
         Me.recieverIPaddress = recieverIPaddress
         Me.recieverPort = recieverPort
         Timer = New Timer()
         Timer.Enabled = False
      Else
         Throw New Exception("Cant get ip address")
      End If
   End Sub
   ''' <summary>
   ''' Para lang ito sa maghohost. Kelangan tawagin ang function na startListen() para maka receive ng message.
   ''' Ganito ang tamag gamit kapag host. | 
   ''' Friend WithEvents Chat As AOATCPIP ===> sa global to | 
   ''' Chat = New AOATCPIP(_settings.port) ===> sa Form load naman to. |
   ''' Merong event ang AOATCPIP. | 
   ''' Private Sub Chat_Recieved(sender As Object, e As AOATCPIP.SendReceInfo) Handles Chat.Recieved
   ''' </summary>
   ''' <param name="ifHost_supplyPort">Mag supply ka ng port. Kung walang port, kunin mo dito AOATCPIP.FreeTcpPort()". Matik na yung IP mag generate.</param>
   Sub New(ifHost_supplyPort As String)
      Dim ipAddress_ As String = GetAvailableIPAddress()
      If ipAddress_ <> "" Then
         Dim ep As IPEndPoint = New IPEndPoint(IPAddress.Parse(ipAddress_), ifHost_supplyPort)
         listener = New TcpListener(ep)
         myIPaddress = ipAddress_
         myPort = ifHost_supplyPort
         Timer = New Timer()
         Timer.Enabled = False
      Else
         Throw New Exception("Cant get ip address")
      End If
   End Sub
   Public Sub StopListen()
      Timer.Stop()
      listener.Stop()
   End Sub
   Public Sub StartListen()
      Dim ep As IPEndPoint = New IPEndPoint(IPAddress.Parse(myIPaddress), myPort)
      listener = New TcpListener(ep)
      listener.Start()
      Timer.Enabled = True
   End Sub
   Public Shared Function FreeTcpPort() As Integer
      Dim l As TcpListener = New TcpListener(IPAddress.Loopback, 0)
      l.Start()
      Dim port As Integer = (CType(l.LocalEndpoint, IPEndPoint)).Port
      l.[Stop]()
      Return port
   End Function
   Public Async Function SendMessage(sendingInfo As SendReceInfo) As Task(Of SendReceInfo)
      Dim res As New SendReceInfo
      Try
         Dim messageBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(sendingInfo))
         Dim client As System.Net.Sockets.TcpClient = New System.Net.Sockets.TcpClient(recieverIPaddress, recieverPort)
         Dim stream As NetworkStream = client.GetStream()
         stream.Write(messageBytes, 0, messageBytes.Length)
         messageBytes = New Byte(1048575) {}
         Await stream.ReadAsync(messageBytes, 0, messageBytes.Length)
         stream.Dispose()
         client.Close()

         Dim msg As String = CleanMessage(messageBytes)
         JsonConvert.PopulateObject(msg, res)
      Catch e As Exception
         res.isError = True
         res.errorMsg = e.Message
      End Try
      Return res
   End Function
   Public Shared Async Function TrySendMessage(sendingInfo As SendReceInfo, recieverIPaddress_ As String, recieverPort_ As Integer) As Task(Of SendReceInfo)
      Dim res As New SendReceInfo
      Try
         Dim messageBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(sendingInfo))
         Dim client As System.Net.Sockets.TcpClient = New System.Net.Sockets.TcpClient(recieverIPaddress_, recieverPort_)
         Dim stream As NetworkStream = client.GetStream()
         stream.Write(messageBytes, 0, messageBytes.Length)
         messageBytes = New Byte(1048575) {}
         Await stream.ReadAsync(messageBytes, 0, messageBytes.Length)
         stream.Dispose()
         client.Close()
         Dim msg As String = CleanMessage(messageBytes)
         Dim receivedInfo As New SendReceInfo
         JsonConvert.PopulateObject(msg, receivedInfo)
         res = receivedInfo
      Catch e As Exception
         res.isError = True
         res.errorMsg = e.Message
      End Try
      Return res
   End Function

   Dim ticking As Boolean = False
   Private Sub Timer_Tick(sender_ As Object, e As EventArgs) Handles Timer.Tick
      If Not ticking Then
         ticking = True
         If listener.Pending Then
            Const bytesize As Integer = 1024 * 1024
            Dim message As String = Nothing
            Dim buffer As Byte() = New Byte(1048575) {}
            Dim sender = listener.AcceptTcpClient()
            sender.GetStream().Read(buffer, 0, bytesize)
            message = CleanMessage(buffer)
            'message = System.Text.Encoding.Unicode.GetString(buffer)
            Dim recievedInfo As New SendReceInfo
            JsonConvert.PopulateObject(message, recievedInfo)
            recievedInfo.GetStream = sender.GetStream()
            RaiseEvent Recieved(Me, recievedInfo)
         End If
         ticking = False
      End If
   End Sub
#Region "Functions"
   Public Shared Function CleanMessage(ByVal bytes As Byte()) As String
      Dim message As String = System.Text.Encoding.Unicode.GetString(bytes)
      Dim messageToPrint As String = Nothing
      For Each nullChar In message
         If nullChar <> vbNullChar Then
            messageToPrint += nullChar
         End If
      Next
      Return messageToPrint
   End Function
   Shared Function GetAvailableIPAddress() As String
      Dim res As String = ""
      Try
         Dim aaa = System.Net.Dns.GetHostAddresses(getComputerId)
         res = aaa(aaa.Length - 1).ToString
      Catch ex As Exception
      End Try
      Return res
   End Function
   Shared Function getComputerId() As String
      Return System.Net.Dns.GetHostName()
   End Function

#End Region
#Region "Classes"
   Class SendReceInfo
      Public actionType As String = ""
      Public dataReturn As String = ""
      Public exeType As String = ""
      Public success As Boolean = False
      Public userId As String = ""
      Public message As String = ""
      Public isError As Boolean = False
      Public errorMsg As String = ""
      Public _object As Object = Nothing
      Public GetStream As NetworkStream = Nothing
      Sub Reply(rep As SendReceInfo)
         If GetStream Is Nothing Then
            MsgBox("Cant reply because GetStream is nothing.")
            Exit Sub
         End If
         Dim byt As Byte() = System.Text.Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(rep))
         Me.GetStream.Write(byt, 0, byt.Length)
         Me.GetStream.Flush()
         Me.GetStream.Close()
         Me.GetStream.Dispose()
      End Sub
   End Class
#End Region
End Class