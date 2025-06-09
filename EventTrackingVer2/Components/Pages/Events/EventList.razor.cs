using EventTrackingVer2.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Radzen.Blazor;

namespace EventTrackingVer2.Components.Pages.Events
{
    public partial class EventList 
    {
        private List<Event> eventList = new();
        private RadzenDataGrid<Event> eventGrid;
        private bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userId = authState.User.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                eventList = await DbContext.Events
                    .Where(e => e.UserId == userId)
                    .ToListAsync();
            }

            isLoading = false;
        }


    }
}
