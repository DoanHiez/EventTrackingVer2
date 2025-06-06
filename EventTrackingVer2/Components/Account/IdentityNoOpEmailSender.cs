
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

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) =>
        SendEmailAsync(email, "Confirm email", $"Please <a href='{confirmationLink}'>click here to confirm</a>.");

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        SendEmailAsync(email, "Reset password", $"Please <a href='{resetLink}'>click here to reset password</a>.");

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        SendEmailAsync(email, "Password reset code", $"Your password reset code is: <strong>{resetCode}</strong>");
    }
}