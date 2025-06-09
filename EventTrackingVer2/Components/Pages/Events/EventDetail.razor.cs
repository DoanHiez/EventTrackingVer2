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
        private List<Guest> guestList = new();
        private RadzenDataGrid<Guest> guestGrid;

        private IRadzenFormComponent editor;
        private bool editorFocused;

        private List<Guest> guestsToUpdate = new();
        private List<KeyValuePair<int, string>> editedFields = new();

        private string columnEditing;
        private string? message;

        private List<string> statusOptions = new() { "Unconfirm", "Confirmed" };
        private List<string> resultOptions = new() { "Absent", "Present" };

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

        private async Task OnCellClick(DataGridCellMouseEventArgs<Guest> args)
        {
            if (!guestGrid.IsValid ||
                guestsToUpdate.Contains(args.Data) && columnEditing == args.Column.Property) return;

            if (guestsToUpdate.Any())
            {
                editedFields.Add(new(guestsToUpdate.First().Id, columnEditing));
                await Update();
            }

            columnEditing = args.Column.Property;
            await EditRow(args.Data);
        }

        private async Task EditRow(Guest guest)
        {
            Reset();
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

        private void Reset(Guest guest = null)
        {
            editorFocused = false;

            if (guest != null)
                guestsToUpdate.Remove(guest);
            else
                guestsToUpdate.Clear();
        }

        private bool IsEditing(string columnName, Guest guest)
        {
            return columnEditing == columnName && guestsToUpdate.Contains(guest);
        }

        private async Task OnUpdateRow(Guest guest)
        {
            Reset(guest);
            DbContext.Update(guest);
            await DbContext.SaveChangesAsync();
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
