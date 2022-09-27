using Blazored.LocalStorage;
using BlazorServer.Common;
using BlazorServer.Models;
using BlazorWasm.Client.Services;
using BlazorWasm.Client.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWasm.Client.Pages.HotelRooms;

[Authorize(Policy = PolicyTypes.RequireEmployeeOrCustomer)]
public partial class PaymentResult
{
    private bool IsLoading { set; get; }
    private bool IsPaymentSuccessful { set; get; }

    [Inject] public ILocalStorageService LocalStorage { set; get; } = default!;
    [Inject] public IClientRoomOrderDetailsService RoomOrderDetailService { set; get; } = default!;
    [Inject] public IJSRuntime JsRuntime { set; get; } = default!;
    [Inject] public NavigationManager NavigationManager { set; get; } = default!;

    [Parameter] public int OrderId { set; get; }

    [Parameter] public long TrackingNumber { set; get; }

    [Parameter] public string? Message { set; get; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        try
        {
            var finalOrderDetail = await RoomOrderDetailService.GetOrderDetailAsync(TrackingNumber);
            var localOrderDetail =
                await LocalStorage.GetItemAsync<RoomOrderDetailsDto>(ConstantKeys.LocalRoomOrderDetails);
            if (finalOrderDetail is not null &&
                finalOrderDetail.IsPaymentSuccessful &&
                string.Equals(finalOrderDetail.Status, BookingStatus.Booked, StringComparison.Ordinal) &&
                localOrderDetail is not null &&
                localOrderDetail.TotalCost == finalOrderDetail.TotalCost)
            {
                IsPaymentSuccessful = true;
                await LocalStorage.RemoveItemAsync(ConstantKeys.LocalRoomOrderDetails);
                await LocalStorage.RemoveItemAsync(ConstantKeys.LocalInitialBooking);
            }
            else
            {
                IsPaymentSuccessful = false;
            }
        }
        catch (Exception ex)
        {
            await JsRuntime.ToastrError(ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }
}