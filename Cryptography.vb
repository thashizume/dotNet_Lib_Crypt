Public Class Cryptography

    ' 128bit(16byte)のIV（初期ベクタ）とKey（暗号キー）
    Private Const AesIV As String = "!QAZ2WSX#EDC4RFV"
    Private Const AesKey As String = "5TGB&YHN7UJM(IK<"

    Private _fingerPrint As String = String.Empty

    Public Sub New()
        Me.FingerPrint = String.Empty
    End Sub

    Public Sub New(fingerPrint As String)

        Me.FingerPrint = fingerPrint
    End Sub

    Public Property FingerPrint As String
        Get
            Return Me._fingerPrint
        End Get
        Set(value As String)
            If value <>  String.Empty Then
                If value.Length <> 16 Then Throw New Exception("Public Key is Invalid Strings")

                Me._fingerPrint = value
            Else

                Me._fingerPrint = String.Empty

            End If
        End Set
    End Property

    Public Function getFingerPrint() As String

        Const keychars As String = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!#$%()+><*~{}"

        Dim result As String = String.Empty
        Dim sb As New System.Text.StringBuilder(16)
        Dim r As New System.Random

        For i As Integer = 0 To 15
            Dim p As Integer = r.Next(keychars.Length)
            sb.Append(keychars(p))
        Next

        result = sb.ToString
        Return result

    End Function

    ''' <summary>
    ''' 文字列をAESで暗号化
    ''' </summary>
    Public Function Encrypt(text As String, Optional fingerPrint As String = Nothing) As String

        If IsNothing(fingerPrint) Then Me.FingerPrint = fingerPrint

        ' AES暗号化サービスプロバイダ
        Dim aes As New System.Security.Cryptography.AesCryptoServiceProvider()
        aes.BlockSize = 128
        aes.KeySize = 128
        aes.IV = System.Text.Encoding.UTF8.GetBytes(IIf(Me.FingerPrint = String.Empty, AesIV, Me.FingerPrint))
        aes.Key = System.Text.Encoding.UTF8.GetBytes(AesKey)
        aes.Mode = System.Security.Cryptography.CipherMode.CBC
        aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7

        ' 文字列をバイト型配列に変換
        Dim src As Byte() = System.Text.Encoding.Unicode.GetBytes(text)

        ' 暗号化する
        Using enc As System.Security.Cryptography.ICryptoTransform = aes.CreateEncryptor()
            Dim dest As Byte() = enc.TransformFinalBlock(src, 0, src.Length)

            ' バイト型配列からBase64形式の文字列に変換
            Return Convert.ToBase64String(dest)
        End Using
    End Function

    ''' <summary>
    ''' 文字列をAESで復号化
    ''' </summary>
    Public Function Decrypt(text As String, Optional fingerPrint As String = Nothing) As String

        If IsNothing(fingerPrint) Then Me.FingerPrint = fingerPrint

        ' AES暗号化サービスプロバイダ
        
        Dim aes As New System.Security.Cryptography.AesCryptoServiceProvider()
        aes.BlockSize = 128
        aes.KeySize = 128
        aes.IV = System.Text.Encoding.UTF8.GetBytes(IIf(Me.FingerPrint = String.Empty, AesIV, Me.FingerPrint))
        aes.Key = System.Text.Encoding.UTF8.GetBytes(AesKey)
        aes.Mode = System.Security.Cryptography.CipherMode.CBC
        aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7

        ' Base64形式の文字列からバイト型配列に変換
        Dim src As Byte() = System.Convert.FromBase64String(text)

        ' 複号化する
        Using dec As System.Security.Cryptography.ICryptoTransform = aes.CreateDecryptor()
            Dim dest As Byte() = dec.TransformFinalBlock(src, 0, src.Length)
            Return System.Text.Encoding.Unicode.GetString(dest)
        End Using
    End Function


End Class
