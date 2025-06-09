using System.Net;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using EventTrackingVer2.Data;
using EventTrackingVer2.Models;
using Radzen.Blazor;
using Radzen;

namespace EventTrackingVer2.Components.Account.Pages.Email
{
    public partial class EmailTemplate : ComponentBase
    {
        [Inject] private ApplicationDbContext Db { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public int? eventId { get; set; }

        private EmailModel model = new();
        private string? statusMessage;
        private Event eventDetail = new();
        private List<Guest> guestList = new();
        private RadzenDataGrid<Guest> guestGrid;
        private IRadzenFormComponent editor;
        private bool editorFocused;
        private List<Guest> guestsToUpdate = new();
        private string columnEditing;

        protected override async Task OnInitializedAsync()
        {
            if (eventId != null)
            {
                eventDetail = await Db.Events.FirstOrDefaultAsync(e => e.EventId == eventId);
                guestList = await Db.Guests.Where(g => g.EventId == eventId).ToListAsync();
            }
        }

        private async Task SendEmailsToGuests()
        {
            try
            {
                if (eventId == null)
                {
                    statusMessage = "❗ Event ID is missing.";
                    return;
                }

                var guests = await Db.Guests
                    .Where(g => !string.IsNullOrEmpty(g.Email) && g.EventId == eventId)
                    .ToListAsync();

                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential("event.tracking2025@gmail.com", "nmqu kwtc ynor vfvy")
                };

                foreach (var guest in guests)
                {
                    try
                    {
                        var personalizedBody = $@"
                    <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                        <p>Dear {guest.GuestName},</p>
                        <p>We are pleased to invite you to our upcoming event:</p>
                        <ul>
                            <li><strong>Event:</strong> {eventDetail.EventName}</li>
                            <li><strong>Organized by:</strong> {eventDetail.Organizer}</li>
                            <li><strong>Location:</strong> {eventDetail.Location}</li>
                            <li><strong>Start Time:</strong> {eventDetail.StartTime:dddd, MMMM dd, yyyy hh:mm tt}</li>
                        </ul>
                        <p>{model.Body}</p>
                        <p>We value your presence and look forward to welcoming you. Please confirm your attendance at your earliest convenience.</p>
                        <p>Sincerely,<br/>{eventDetail.Organizer}</p>
                    </div>"
                        .Replace("{{GuestName}}", guest.GuestName ?? "Guest");

                        var mail = new MailMessage
                        {
                            From = new MailAddress("event.tracking2025@gmail.com"),
                            Subject = model.Subject,
                            Body = personalizedBody,
                            IsBodyHtml = true
                        };
                        mail.To.Add(guest.Email);
                        await smtp.SendMailAsync(mail);

                        Db.SentEmails.Add(new SentEmail
                        {
                            ToEmail = guest.Email,
                            Subject = model.Subject,
                            Body = personalizedBody,
                            SentAt = DateTime.Now
                        });

                        await Task.Delay(300);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error sending to {guest.Email}: {ex.Message}");
                    }
                }

                await Db.SaveChangesAsync();
                NavigationManager.NavigateTo("/email-success");
            }
            catch (Exception ex)
            {
                statusMessage = $"❌ Failed to send emails: {ex.Message}";
            }
        }


        private async Task SaveAllChanges()
        {
            if (guestsToUpdate.Any())
            {
                Db.UpdateRange(guestsToUpdate);
                await Db.SaveChangesAsync();
                guestsToUpdate.Clear();
                statusMessage = $"✔ All changes have been saved.";
            }
            else
            {
                statusMessage = "No changes to update.";
            }
        }

        private async Task OnCellClick(DataGridCellMouseEventArgs<Guest> args)
        {
            if (!guestGrid.IsValid || guestsToUpdate.Contains(args.Data) && columnEditing == args.Column.Property)
                return;

            if (guestsToUpdate.Any())
            {
                await Update();
            }

            columnEditing = args.Column.Property;
            await EditRow(args.Data);
        }

        private async Task EditRow(Guest guest)
        {
            if (!guestsToUpdate.Contains(guest))
                guestsToUpdate.Add(guest);

            await guestGrid.EditRow(guest);
        }

        private async Task Update()
        {
            editorFocused = false;

            if (guestsToUpdate.Any())
            {
                await guestGrid.UpdateRow(guestsToUpdate.First());
            }
        }

        private bool IsEditing(string columnName, Guest guest)
        {
            return columnEditing == columnName && guestsToUpdate.Contains(guest);
        }

        private async Task OnUpdateRow(Guest guest)
        {
            guestsToUpdate.Remove(guest);
            Db.Update(guest);
            await Db.SaveChangesAsync();
        }

        public class EmailModel
        {
            [Required(ErrorMessage = "Subject is required.")]
            public string Subject { get; set; } = "";

            [Required(ErrorMessage = "Email body is required.")]
            public string Body { get; set; } = "";
        }
    }
}
