using BlazorServer.App.Utils;
using BlazorServer.Common;
using BlazorServer.Models;
using BlazorServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorServer.App.Pages.HotelAmenity;

[Authorize(Roles = ConstantRoles.Admin)]
public partial class HotelAmenityList
{
    [Inject] public IJSRuntime JsRuntime { set; get; } = default!;

    private IAmenityService AmenityService => Service;
    private IEnumerable<HotelAmenityDto> HotelAmenities { get; set; } = new List<HotelAmenityDto>();
    private int? DeleteAmenityId { get; set; }
    private bool IsProcessing { get; set; }

    protected override async Task OnInitializedAsync()
    {
        HotelAmenities = await AmenityService.GetAllHotelAmenityAsync();
    }

    private async Task HandleDelete(int amenityId)
    {
        DeleteAmenityId = amenityId;
        await JsRuntime.InvokeVoidAsync("ShowDeleteConfirmationModal");
    }

    private async Task ConfirmDelete_Click(bool isConfirmed)
    {
        IsProcessing = true;
        if (isConfirmed && DeleteAmenityId != null)
        {
            try
            {
                await AmenityService.DeleteHotelAmenityAsync(DeleteAmenityId.Value, "");
                await JsRuntime.ToastrSuccess("Amenity Deleted successfully");
            }
            catch (Exception e)
            {
                await JsRuntime.ToastrError(e.Message);
            }

            HotelAmenities = await AmenityService.GetAllHotelAmenityAsync();
        }

        await JsRuntime.InvokeVoidAsync("HideDeleteConfirmationModal");
        IsProcessing = false;
    }
}