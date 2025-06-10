using EventTrackingVer2.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Radzen.Blazor;

namespace EventTrackingVer2.Components.Pages.Events
{
    public partial class EventDetail
    {
        [Parameter] public int EventId { get; set; }

        private Event eventDetail = new();
        private List<GuestList> guestList = new();

        private IRadzenFormComponent editor;
        private bool editorFocused;

      
        private string? message;

       
        protected override async Task OnInitializedAsync()
        {
            eventDetail = await DbContext.Events
                .FirstOrDefaultAsync(e => e.EventId == EventId);

            guestList = await DbContext.Guests
                .Where(g => g.EventId == EventId)
                .ToListAsync();
        }

        private async Task HandleValidSubmit()
        {
            // Save event info
            DbContext.Events.Update(eventDetail);
            await DbContext.SaveChangesAsync();

            // Save modified guests
            if (guestsToUpdate.Any())
            {
                DbContext.UpdateRange(guestsToUpdate);
                await DbContext.SaveChangesAsync();

                guestsToUpdate.Clear();
                columnEditing = "";

                message = $"✅ The event and all guest changes have been successfully saved.";
            }
        }

       
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!editorFocused && editor != null)
            {
                editorFocused = true;

                try
                {
                    await editor.FocusAsync();
                }
                catch
                {
                    // Ignore focus errors
                }
            }
        }
    }
}
