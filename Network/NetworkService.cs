using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Marketplace_Group_Project.Network
{
    public class NetworkService
    {
        private SmtpClient? _client;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly bool _useSsl;
        private readonly string _emailFrom;
        private readonly string _password;

        public NetworkService(string smtpHost, int smtpPort, bool useSsl, string emailFrom, string password)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _useSsl = useSsl;
            _emailFrom = emailFrom;
            _password = password;
        }

        // Connect() — подключение к SMTP-серверу
        public async Task ConnectAsync()
        {
            if (_client != null && _client.IsConnected)
                return;

            _client = new SmtpClient();
            await _client.ConnectAsync(_smtpHost, _smtpPort, _useSsl);
            await _client.AuthenticateAsync(_emailFrom, _password);
        }

        // Disconnect() — отключение
        public async Task DisconnectAsync()
        {
            if (_client == null) return;
            if (_client.IsConnected)
                await _client.DisconnectAsync(true);
            _client.Dispose();
            _client = null;
        }

        // SendMessage() — отправка письма
        public async Task SendMessageAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("", _emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            if (_client == null || !_client.IsConnected)
            {
                throw new InvalidOperationException("Сначала вызовите ConnectAsync().");
            }

            await _client.SendAsync(message);
        }

        // ReceiveMessage() и StartListening() Для получения POP3/IMAP (отдельные клиенты)
    }
}