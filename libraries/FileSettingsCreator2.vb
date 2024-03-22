
'Please install Newtonsoft.Json.13.0.1 
'in Manage NuGet Pacjages
Imports Newtonsoft.Json
Public Class FileSettingsCreator2(Of T)
   Public path As String = ""
   ReadOnly crpt As Crypt.String
   Dim instanceOfT As T
   Shared password As String = ""
   Public SetEnCrypted As Boolean = True
   Private Const EncryptedKey As String = "_DEJAVU_"

   Public Overrides Function ToString() As String
      Return IO.Path.GetFileNameWithoutExtension(Me.path)
   End Function

   Sub New(filePath As String, newInstance As T, Optional setPassword As String = "key_csgi_gi_search_key")
      Me.path = filePath
      password = setPassword
      crpt = New Crypt.String(password)
      instanceOfT = newInstance
   End Sub
   'Example
   Class SettingsInfo
      ' Pwede sa array pero hindi pwede sa List
      Public name As String = ""
   End Class
   Sub example()
      Dim a As New FileSettingsCreator2(Of SettingsInfo)("C:\Users\soft10\Documents\Face recognition\drihnz.txt", New SettingsInfo)
      Dim b = a.GetSettings() ' get the setting from file
      a.SetSettings(New SettingsInfo()) ' save the setings to file
   End Sub
   '/end of example
   Function SetSettings(classObj As T, Optional showError As Boolean = False) As Boolean
      Dim res As Boolean = False
      Dim tmpStr As String = JsonConvert.SerializeObject(classObj)
      Dim spl As Object = SplitInParts(tmpStr, 5000)
      Dim newSplt As New List(Of String)
      For Each i As String In spl
         If SetEnCrypted Then
            newSplt.Add(crpt.Encrypt(i))
         Else
            newSplt.Add(i)
         End If
      Next
      Try
         Dim wr As New IO.StreamWriter(path, False)
         If SetEnCrypted AndAlso newSplt.Count > 0 Then
            newSplt(0) = EncryptedKey & newSplt(0)
         End If
         For Each i As String In newSplt
            wr.WriteLine(i)
         Next
         wr.Close()
         res = True
      Catch ex As Exception
         'Throw New Exception("FileSettingsCreator > setSettings Error: " & ex.Message)
         If showError Then
            MsgBox(String.Concat("FileSettingsCreator Error > ", ex.Message))
         End If
      End Try
      GC.Collect()
      GC.WaitForPendingFinalizers()
      Return res
   End Function

   Function GetSettings(Optional showError As Boolean = False) As T
      Try
         If Not IO.File.Exists(path) Then
            'Using a As New IO.StreamWriter(path)
            'End Using
            Return Nothing
         End If
         Dim txtLines As String() = IO.File.ReadAllLines(path)
         Dim tx As String = ""
         Dim isEncrypted As Boolean = False
         If txtLines.Length > 0 Then
            If txtLines(0).StartsWith(EncryptedKey) Then
               isEncrypted = True
               txtLines(0) = txtLines(0).Replace(EncryptedKey, "")
            End If
         End If
         For Each i As String In txtLines
            If isEncrypted Then
               tx = String.Concat(tx, crpt.Decrypt(i))
            Else
               tx = String.Concat(tx, i)
            End If
         Next
         instanceOfT = JsonConvert.DeserializeObject(tx, instanceOfT.GetType)
         'JsonConvert.PopulateObject(tx, instanceOfT)
      Catch ex As Exception
         If showError Then
            MsgBox(String.Concat("FileSettingsCreator Error > ", ex.Message))
         End If
      End Try
      Return instanceOfT
   End Function

   Public Function SplitInParts(s As String, partLength As Integer) As IEnumerable(Of String)
      If String.IsNullOrEmpty(s) Then
         Throw New ArgumentNullException("String cannot be null or empty.")
      End If
      If partLength <= 0 Then
         Throw New ArgumentException("Split length has to be positive.")
      End If
      Return Enumerable.Range(0, Math.Ceiling(s.Length / partLength)).Select(Function(i) s.Substring(i * partLength, If(s.Length - (i * partLength) >= partLength, partLength, Math.Abs(s.Length - (i * partLength)))))
   End Function

   Enum EnryptType As Integer
      ENCRYPTED
      NOTSAFE
   End Enum

#Region "Encryptor and decryptor"
   Public Shared Function convertObject_To_StringEncrypted(classObj As Object) As String
      Dim tmpStr As String = JsonConvert.SerializeObject(classObj)
      Dim c As New Crypt.String(password)
      Dim res = c.Encrypt(tmpStr)
      Return res
   End Function
   Public Shared Sub convertStringEncryted_To_Object(str As String, ByRef obj As Object)
      Dim c As New Crypt.String(password)
      str = c.Decrypt(str)
      JsonConvert.PopulateObject(str, obj)
   End Sub
   Public Class Crypt

      Public Class [String]
         Private TripleDes As New Security.Cryptography.TripleDESCryptoServiceProvider

         Public Sub New(ByVal key As String)
            key = key & "DotNetCS-" & key & "-Key"
            TripleDes.Key = TruncateHash(key, TripleDes.KeySize \ 8)
            TripleDes.IV = TruncateHash("", TripleDes.BlockSize \ 8)
         End Sub

         Private Function TruncateHash(ByVal key As String, ByVal length As Integer) As Byte()
            Dim sha1 As New Security.Cryptography.SHA1CryptoServiceProvider
            Dim keyBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(key)
            Dim hash() As Byte = sha1.ComputeHash(keyBytes)
            ReDim Preserve hash(length - 1)
            Return hash
         End Function

         Public Function Encrypt(ByVal plaintext As String) As String
            If plaintext.Equals("") Then
               Return plaintext
            Else
               Dim plaintextBytes() As Byte = System.Text.Encoding.Unicode.GetBytes(plaintext)
               Dim ms As New System.IO.MemoryStream
               Dim encStream As New Security.Cryptography.CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write)
               encStream.Write(plaintextBytes, 0, plaintextBytes.Length)
               encStream.FlushFinalBlock()
               Return Convert.ToBase64String(ms.ToArray)
            End If
         End Function

         Public Function Decrypt(ByVal encryptedtext As String) As String
            Dim retStr As String = ""
            If encryptedtext.Trim.Equals("") Then
               retStr = encryptedtext
            Else
               Try
                  Dim encryptedBytes() As Byte = Convert.FromBase64String(encryptedtext)
                  Dim ms As New System.IO.MemoryStream
                  Dim decStream As New Security.Cryptography.CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write)
                  decStream.Write(encryptedBytes, 0, encryptedBytes.Length)
                  decStream.FlushFinalBlock()
                  retStr = System.Text.Encoding.Unicode.GetString(ms.ToArray)
               Catch ex As Exception
               End Try
            End If
            Return retStr
         End Function

         Public Shared Function GetMD5(ByVal value As String) As String
            Using md5 As Security.Cryptography.MD5 = Security.Cryptography.MD5.Create()
               Dim bytes As Byte() = md5.ComputeHash(Text.Encoding.UTF8.GetBytes(value))
               Dim builder As New Text.StringBuilder()
               For n As Integer = 0 To bytes.Length - 1
                  builder.Append(bytes(n).ToString("X2"))
               Next n
               Return builder.ToString()
            End Using
         End Function
      End Class
   End Class
#End Region
End Class

