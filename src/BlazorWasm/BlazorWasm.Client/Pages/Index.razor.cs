using Blazored.LocalStorage;
using BlazorServer.Common;
using BlazorServer.Models;
using BlazorWasm.Client.Models.ViewModels;
using BlazorWasm.Client.Services;
using BlazorWasm.Client.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWasm.Client.Pages;

public partial class Index
{
    private HomeVM HomeModel { get; } = new();
    private IEnumerable<HotelAmenityDto>? HotelAmenities { set; get; }
    private bool IsProcessing { set; get; }

    [Inject] public ILocalStorageService LocalStorage { set; get; } = default!;
    [Inject] public IJSRuntime JsRuntime { set; get; } = default!;
    [Inject] public NavigationManager NavigationManager { set; get; } = default!;
    [Inject] public IClientHotelAmenityService HotelAmenityService { set; get; } = default!;

    protected override async Task OnInitializedAsync()
    {
        IsProcessing = true;
        try
        {
            HotelAmenities = await HotelAmenityService.GetHotelAmenities();
        }
        catch (Exception e)
        {
            await JsRuntime.ToastrError(e.Message);
        }

        IsProcessing = false;
    }

    private async Task SaveInitialData()
    {
        try
        {
            HomeModel.EndDate = HomeModel.StartDate.AddDays(HomeModel.NoOfNights);
            await LocalStorage.SetItemAsync(ConstantKeys.LocalInitialBooking, HomeModel);
            NavigationManager.NavigateTo("hotel-rooms");
        }
        catch (Exception e)
        {
            await JsRuntime.ToastrError(e.Message);
        }
    }
}