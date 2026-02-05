using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;   
using Microsoft.Extensions.Logging;


namespace StajerManager.Services
{
    public class EmailService : IEmailService   
    {
        private readonly IConfiguration _configuration;  
        private readonly ILogger<EmailService> _logger;


        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)   
        {
            _configuration = configuration;  
            _logger = logger;
        }


        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                // Yeni email mesajı oluştur
                var emailMessage = new MimeMessage();

                // Gönderen bilgilerini ayarla (appsettings.json'dan al)
                emailMessage.From.Add(new MailboxAddress("StajerManager", _configuration["EmailSettings:FromEmail"]));

                // Alıcı bilgilerini ayarla
                emailMessage.To.Add(new MailboxAddress("", email));

                // Email konusunu ayarla
                emailMessage.Subject = subject;

                // Email içeriğini HTML formatında ayarla
                emailMessage.Body = new TextPart("html") { Text = message };

                // SMTP client oluştur ve bağlan
                using var client = new SmtpClient();

                // SMTP sunucusuna bağlan
                var smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                
                // Port'a göre SSL seçeneği belirle
                MailKit.Security.SecureSocketOptions sslOption;
                if (smtpPort == 465)
                {
                    // Port 465 için SSL kullan
                    sslOption = MailKit.Security.SecureSocketOptions.SslOnConnect;
                }
                else
                {
                    // Port 587 için STARTTLS kullan
                    sslOption = MailKit.Security.SecureSocketOptions.StartTls;
                }
                
                await client.ConnectAsync(smtpServer, smtpPort, sslOption);

                // SMTP sunucusunda kimlik doğrulama yap
                await client.AuthenticateAsync(
                    _configuration["EmailSettings:Username"], // Email kullanıcı adı
                    _configuration["EmailSettings:Password"] // Email şifresi (App Password)
                );

                // Email'i gönder
                await client.SendAsync(emailMessage);

                // Bağlantıyı kapat
                await client.DisconnectAsync(true);

                // Başarılı gönderim logla
                _logger.LogInformation("Email başarıyla gönderildi: {Email}", email);
            }
            catch (Exception ex)
            {
                // Hata durumunu logla
                _logger.LogError(ex, "Email gönderilirken hata oluştu: {Email}", email);
                throw; // Hatayı üst seviyeye fırlat
            }
        }


        /// <summary>
        /// Email doğrulama linki gönderme
        /// Kullanıcı kayıt olduktan sonra email adresini doğrulaması için
        /// </summary>
        /// <param name="email">Kullanıcının email adresi</param>
        /// <param name="callbackUrl">Doğrulama linki (AccountController'dan gelir)</param>
        public async Task SendEmailConfirmationAsync(string email, string callbackUrl)
        {
            // Email konusu
            var subject = "E-posta Adresinizi Doğrulayın";

            // HTML formatında email içeriği
            var message = $@"
                <h2>E-posta Doğrulama</h2>
                <p>Merhaba,</p>
                <p>StajerManager hesabınızı oluşturmak için aşağıdaki bağlantıya tıklayın:</p>
                <p><a href='{callbackUrl}'>E-posta Adresimi Doğrula</a></p>
                <p>Bu bağlantı 24 saat geçerlidir.</p>
                <p>Eğer bu işlemi siz yapmadıysanız, bu e-postayı görmezden gelebilirsiniz.</p>
                <br>
                <p>StajerManager Ekibi</p>
            ";

            // Genel email gönderme metodunu çağır
            await SendEmailAsync(email, subject, message);
        }

        /// <summary>
        /// Şifre sıfırlama linki gönderme
        /// Kullanıcı şifresini unuttuğunda yeni şifre belirlemesi için
        /// </summary>
        /// <param name="email">Kullanıcının email adresi</param>
        /// <param name="callbackUrl">Şifre sıfırlama linki (AccountController'dan gelir)</param>
        public async Task SendPasswordResetAsync(string email, string callbackUrl)
        {
            // Email konusu
            var subject = "Şifre Sıfırlama";

            // HTML formatında email içeriği
            var message = $@"
                <h2>Şifre Sıfırlama</h2>
                <p>Merhaba,</p>
                <p>Şifrenizi sıfırlamak için aşağıdaki bağlantıya tıklayın:</p>
                <p><a href='{callbackUrl}'>Şifremi Sıfırla</a></p>
                <p>Bu bağlantı 1 saat geçerlidir.</p>
                <p>Eğer bu işlemi siz yapmadıysanız, bu e-postayı görmezden gelebilirsiniz.</p>
                <br>
                <p>StajerManager Ekibi</p>
            ";

            // Genel email gönderme metodunu çağır
            await SendEmailAsync(email, subject, message);
        }

    }
}
