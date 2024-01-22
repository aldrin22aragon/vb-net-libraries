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