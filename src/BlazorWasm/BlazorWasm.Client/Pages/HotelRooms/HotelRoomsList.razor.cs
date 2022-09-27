using Blazored.LocalStorage;
using BlazorServer.Common;
using BlazorServer.Models;
using BlazorWasm.Client.Models.ViewModels;
using BlazorWasm.Client.Services;
using BlazorWasm.Client.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWasm.Client.Pages.HotelRooms;

public partial class HotelRoomsList
{
    private readonly string ImagesBaseAddress = "https://localhost:5006";
    private HomeVM HomeModel { set; get; } = new();
    private bool IsProcessing { set; get; }
    private IEnumerable<HotelRoomDto>? Rooms { set; get; }

    [Inject] public ILocalStorageService LocalStorage { set; get; } = default!;
    [Inject] public IJSRuntime JsRuntime { set; get; } = default!;
    [Inject] public IClientHotelRoomService HotelRoomService { set; get; } = default!;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var model = await LocalStorage.GetItemAsync<HomeVM>(ConstantKeys.LocalInitialBooking);
            if (model is not null)
            {
                HomeModel = model;
            }
            else
            {
                HomeModel.NoOfNights = 1;
            }

            await LoadRoomsAsync();
        }
        catch (Exception e)
        {
            await JsRuntime.ToastrError(e.Message);
        }
    }

    private async Task LoadRoomsAsync()
    {
        Rooms = await HotelRoomService.GetHotelRoomsAsync(HomeModel.StartDate, HomeModel.EndDate);
        if (Rooms is null)
        {
            await JsRuntime.ToastrError("Rooms is null");
            return;
        }

        foreach (var room in Rooms)
        {
            room.TotalAmount = room.RegularRate * HomeModel.NoOfNights;
            room.TotalDays = HomeModel.NoOfNights;
        }
    }

    private async Task SaveBookingInfo()
    {
        IsProcessing = true;
        HomeModel.EndDate = HomeModel.StartDate.AddDays(HomeModel.NoOfNights);
        await LocalStorage.SetItemAsync(ConstantKeys.LocalInitialBooking, HomeModel);
        await LoadRoomsAsync();
        IsProcessing = false;
    }
}