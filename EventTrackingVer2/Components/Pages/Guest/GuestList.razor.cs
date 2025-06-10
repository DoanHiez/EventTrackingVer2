using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Radzen.Blazor;
using EventTrackingVer2.Models;

namespace EventTrackingVer2.Components.Pages.Guest
{
    public partial class GuestList
    {
        [Parameter] public List<GuestList>? guestLists { get; set; }
        [Parameter] public EventCallback<GuestList> OnUpdateGuest { get; set; }
        private List<GuestList> guestsToUpdate = new();
        private RadzenDataGrid<GuestList>? guestGrid;
        private List<string> statusOptions = new() { "Unconfirm", "Confirmed" };
        private List<string> resultOptions = new() { "Absent", "Present" };
        private string columnEditing;
        private bool editorFocused;
        private List<KeyValuePair<int, string>> editedFields = new();

        private async Task OnCellClick(DataGridCellMouseEventArgs<GuestList> args)
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

        private async Task EditRow(GuestList guest)
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

        private void Reset(GuestList guest = null)
        {
            editorFocused = false;

            if (guest != null)
                guestsToUpdate.Remove(guest);
            else
                guestsToUpdate.Clear();
        }

        private bool IsEditing(string columnName, GuestList guest)
        {
            return columnEditing == columnName && guestsToUpdate.Contains(guest);
        }

        private async Task OnUpdateRow(GuestList guest)
        {
            Reset(guest);
            DbContext.Update(guest);
            await DbContext.SaveChangesAsync();
        }

    }
}
