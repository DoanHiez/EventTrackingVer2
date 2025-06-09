using CsvHelper;
using CsvHelper.Configuration;
using EventTrackingVer2.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;

namespace EventTrackingVer2.Components.Pages.Events
{
    public partial class EventRegister
    {
        [SupplyParameterFromForm]
        private Event eventModel { get; set; } = new();

        private IBrowserFile? selectedFile;
        private string? message;

        private void HandleFileSelected(InputFileChangeEventArgs e)
        {
            selectedFile = e.File;
        }

        private async Task HandleValidSubmit()
        {
            if (selectedFile == null)
            {
                message = "❗ Please select a CSV file.";
                return;
            }

            try
            {
                // Step 1: Save event
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var userId = authState.User.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    eventModel.UserId = userId;
                    DbContext.Events.Add(eventModel);
                    await DbContext.SaveChangesAsync(); // EventId is now available
                }

                // Step 2: Read CSV using CsvHelper
                using var stream = selectedFile.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    BadDataFound = null,
                    MissingFieldFound = null,
                    HeaderValidated = null
                });

                var guests = new List<Guest>();

                await foreach (var record in csv.GetRecordsAsync<GuestCsvRow>())
                {
                    guests.Add(new Guest
                    {
                        GuestCode = record.GuestCode.Trim(),
                        GuestName = record.GuestName.Trim(),
                        PhoneNumber = record.PhoneNumber.Trim(),
                        Email = record.Email.Trim(),
                        Status = "Unconfirm",
                        Result = "Absent",
                        EventId = eventModel.EventId
                    });
                }

                // Step 3: Save guests
                if (guests.Any())
                {
                    DbContext.Guests.AddRange(guests);
                    await DbContext.SaveChangesAsync();
                    Console.WriteLine("📥 Event and guest list saved successfully.");
                    message = $"✅ Event registered & {guests.Count} guests saved.";
                    NavigationManager.NavigateTo($"/email-template?eventId={eventModel.EventId}");
                }
                else
                {
                    message = "⚠️ The CSV file does not contain valid guest data.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error processing CSV file:");
                Console.WriteLine(ex);
                message = $"❌ An error occurred while processing the CSV file: {ex.Message}";
            }
        }
    }
}
