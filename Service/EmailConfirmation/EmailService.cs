using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Service.EmailConfirmation
{
    public class EmailService
    {

        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendConfirmationEmail(string email, string token)
        {

            if (string.IsNullOrWhiteSpace(email) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
            {
                throw new ArgumentException("Địa chỉ email không hợp lệ.", nameof(email));
            }
            // Tạo nội dung email với mã xác nhận (token) thay vì đường link
            var emailSubject = "[EVRental] Mã xác nhận email của bạn";
            var emailBody = $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6;'>
    <h2 style='color: #0066cc;'>Xác minh địa chỉ email</h2>
    <p>Xin chào!</p>
    <p>Dưới đây là mã xác nhận tài khoản của bạn:</p>
    <div style='background-color: #f5f5f5; padding: 10px; border-radius: 5px; font-size: 18px; font-weight: bold;'>
        {token}
    </div>
    <p>Vui lòng nhập mã này vào trang xác nhận để hoàn tất quá trình.</p>
    <p>Mã có hiệu lực trong 15 phút.</p>
    <p>Trân trọng,<br/>Đội ngũ hỗ trợ EVRental!</p>
</div>";

            using var smtpClient = new SmtpClient(_smtpSettings.Server)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = emailSubject,
                Body = emailBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"Lỗi khi gửi email: {ex.Message}");
            }
        }
        public async Task SendResetPasswordEmailAsync(string email, string fullName, string otp)
        {
            if (string.IsNullOrWhiteSpace(email) || !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
            {
                throw new ArgumentException("Địa chỉ email không hợp lệ.", nameof(email));
            }

            var emailSubject = "[EVRental] Mã reset mật khẩu của bạn";
            var emailBody = $@"
<div style='font-family: Arial, sans-serif; line-height: 1.6;'>
    <h2 style='color: #0066cc;'>Reset mật khẩu</h2>
    <p>Xin chào {fullName},</p>
    <p>Bạn đã yêu cầu reset mật khẩu. Dưới đây là mã xác nhận:</p>
    <div style='background-color: #f5f5f5; padding: 10px; border-radius: 5px; font-size: 18px; font-weight: bold;'>
        {otp}
    </div>
    <p>Vui lòng nhập mã này vào form reset để đặt mật khẩu mới. Mã hết hạn sau 15 phút.</p>
    <p>Nếu bạn không yêu cầu, hãy bỏ qua email này.</p>
    <p>Trân trọng,<br/>Đội ngũ hỗ trợ EVRental!</p>
</div>";

            using var smtpClient = new SmtpClient(_smtpSettings.Server)
            {
                Port = _smtpSettings.Port,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = emailSubject,
                Body = emailBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"Lỗi khi gửi email reset: {ex.Message}");
                throw;
            }
        }
    }
}
