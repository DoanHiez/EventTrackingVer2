
using EventTrackingVer2.Data;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Mail;

namespace EventTrackingVer2.Components.Account
{
    internal sealed class IdentityNoOpEmailSender : IEmailSender<ApplicationUser>
    {
        // Gán trực tiếp email và mật khẩu ứng dụng ở đây
        private readonly string fromEmail = "event.tracking2025@gmail.com";
        private readonly string appPassword = "nmqu kwtc ynor vfvy";

        private async Task SendAsync(string toEmail, string subject, string htmlMessage)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mail.To.Add(toEmail);

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, appPassword),
                EnableSsl = true
            };

            try
            {
                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                // Log or throw an error as needed
                throw new InvalidOperationException("Failed to send email", ex);
            }
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage) =>
        SendAsync(email, subject, htmlMessage);

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            string subject = "Xác nhận địa chỉ email của bạn";

            string body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; background-color: #ffffff; border: 1px solid #e0e0e0;'>
                <h2 style='color: #333;'>Chào {user.UserName ?? "bạn"},</h2>
                <p style='font-size: 16px; color: #555;'>Cảm ơn bạn đã đăng ký. Để hoàn tất quá trình, vui lòng xác nhận địa chỉ email của bạn bằng cách nhấn vào nút bên dưới:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{confirmationLink}' style='background-color: #1a73e8; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-size: 16px;'>Xác nhận email</a>
                </div>
                <p style='font-size: 14px; color: #888;'>Nếu bạn không tạo tài khoản, bạn có thể bỏ qua email này.</p>
                <p style='font-size: 14px; color: #888;'>Trân trọng,<br/>Đội ngũ hỗ trợ</p>
            </div>";

            return SendEmailAsync(email, subject, body);
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            string subject = "Yêu cầu đặt lại mật khẩu";

            string body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; background-color: #ffffff; border: 1px solid #e0e0e0;'>
            <h2 style='color: #333;'>Chào {user.UserName ?? "bạn"},</h2>
            <p style='font-size: 16px; color: #555;'>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn. Để tiếp tục, vui lòng nhấn vào nút bên dưới:</p>
            <div style='text-align: center; margin: 30px 0;'>
                <a href='{resetLink}' style='background-color: #d93025; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-size: 16px;'>Đặt lại mật khẩu</a>
            </div>
            <p style='font-size: 14px; color: #888;'>Nếu bạn không yêu cầu điều này, vui lòng bỏ qua email này. Mật khẩu của bạn sẽ không bị thay đổi.</p>
            <p style='font-size: 14px; color: #888;'>Trân trọng,<br/>Đội ngũ hỗ trợ</p>
        </div>";

            return SendEmailAsync(email, subject, body);
        }


        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendEmailAsync(email, "Password reset code", $"Your password reset code is: <strong>{resetCode}</strong>");
    }
}