using BlazorServer.App.Utils;
using BlazorServer.Common;
using BlazorServer.Models;
using BlazorServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorServer.App.Pages.HotelAmenity;

[Authorize(Roles = ConstantRoles.Admin)]
public partial class HotelAmenityUpsert
{
    [Inject] public NavigationManager NavigationManager { set; get; } = default!;
    [Inject] public IJSRuntime JsRuntime { set; get; } = default!;

    [Parameter] public int? Id { get; set; }

    private IAmenityService AmenityService => Service;
    private HotelAmenityDto? HotelAmenityModel { get; set; }

    private string Title { get; set; } = "Create";

    private bool IsProcessingStart { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Id != null)
        {
            Title = "Update";
            HotelAmenityModel = await AmenityService.GetHotelAmenityAsync(Id.Value);
        }
        else
        {
            HotelAmenityModel = new HotelAmenityDto();
        }
    }

    private async Task HandleHotelAmenityCreate()
    {
        try
        {
            IsProcessingStart = true;
            if (HotelAmenityModel is null)
            {
                await JsRuntime.ToastrError("HotelAmenityModel is null.");
                return;
            }

            if (HotelAmenityModel.Id != 0 &&
                string.Equals(Title, "Update", StringComparison.OrdinalIgnoreCase))
            {
                var amenityDetailsByName =
                    await AmenityService.IsSameNameAmenityAlreadyExistsAsync(HotelAmenityModel.Name);
                if (amenityDetailsByName != null && amenityDetailsByName.Id != HotelAmenityModel.Id)
                {
                    await JsRuntime.ToastrError("Hotel Amenity already exists.");
                    return;
                }

                //Update the hotel amenity here
                await AmenityService.UpdateHotelAmenityAsync(HotelAmenityModel.Id, HotelAmenityModel);

                await JsRuntime.ToastrSuccess("Hotel Amenity updated successfully.");
            }
            else
            {
                var amenityDetailsByName =
                    await AmenityService.IsSameNameAmenityAlreadyExistsAsync(HotelAmenityModel.Name);
                if (amenityDetailsByName != null)
                {
                    await JsRuntime.ToastrError("Hotel Amenity name is already exists.");
                }

                //Create new Hotel Amenity here
                await AmenityService.CreateHotelAmenityAsync(HotelAmenityModel);
                HotelAmenityModel = new HotelAmenityDto();
                await JsRuntime.ToastrSuccess("Hotel amenity created successfully.");
            }

            NavigationManager.NavigateTo("hotel-amenity");
            IsProcessingStart = false;
        }
        catch (Exception e)
        {
            await JsRuntime.ToastrError(e.Message);
            IsProcessingStart = false;
        }
    }
}