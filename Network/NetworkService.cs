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
        private readonly SmtpClient _client;
        private readonly string _emailFrom;

        public NetworkService(string smtpHost, int smtpPort, bool useSsl, string emailFrom, string password)
        {
            if (string.IsNullOrWhiteSpace(emailFrom))
                throw new ArgumentException("Email отправителя не может быть пустым.", nameof(emailFrom));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым.", nameof(password));

            _emailFrom = emailFrom;
            _client = new SmtpClient();

            try
            {
                _client.Connect(smtpHost, smtpPort, useSsl);
                _client.Authenticate(emailFrom, password);
            }
            catch (Exception ex)
            {
                _client.Dispose();
                throw new InvalidOperationException($"Не удалось подключиться к SMTP-серверу: {ex.Message}", ex);
            }
        }

        //  -= УДАЛЕНЫ =-
        // Connect() — подключение к SMTP-серверу
        // Disconnect() — отключение

        public async Task SendMessageAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Email получателя не может быть пустым.", nameof(toEmail));
            /*if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Тема письма не может быть пустой.", nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Тело письма не может быть пустым.", nameof(body));*/

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Marketplace", _emailFrom));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            await _client.SendAsync(message);
        }
    }
}