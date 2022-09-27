using Blazored.LocalStorage;
using BlazorServer.Common;
using BlazorServer.Models;
using BlazorWasm.Client.Models.ViewModels;
using BlazorWasm.Client.Services;
using BlazorWasm.Client.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorWasm.Client.Pages.HotelRooms;

[Authorize(Policy = PolicyTypes.RequireEmployeeOrCustomer)]
public partial class RoomDetails
{
    private HotelRoomBookingVM HotelBooking { get; } = new();
    private int NoOfNights { set; get; } = 1;

    [Inject] public IJSRuntime JsRuntime { set; get; } = default!;
    [Inject] public ILocalStorageService LocalStorage { set; get; } = default!;
    [Inject] public IClientHotelRoomService HotelRoomService { set; get; } = default!;
    [Inject] public IClientRoomOrderDetailsService RoomOrderDetailsService { set; get; } = default!;
    [Inject] public NavigationManager NavigationManager { set; get; } = default!;
    [Inject] public HttpClient HttpClient { set; get; } = default!;

    [Parameter] public int? Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            HotelBooking.OrderDetails = new RoomOrderDetailsDto();
            if (Id != null)
            {
                if (await LocalStorage.GetItemAsync<HomeVM>(ConstantKeys.LocalInitialBooking) != null)
                {
                    var roomInitialInfo = await LocalStorage.GetItemAsync<HomeVM>(ConstantKeys.LocalInitialBooking);
                    var hotelRoomDetails = await HotelRoomService.GetHotelRoomDetailsAsync(
                                            Id.Value, roomInitialInfo.StartDate,
                                            roomInitialInfo.EndDate);
                    if (hotelRoomDetails is null)
                    {
                        await JsRuntime.ToastrError("hotelRoomDetails is null");
                        return;
                    }

                    HotelBooking.OrderDetails.HotelRoomDto = hotelRoomDetails;
                    NoOfNights = roomInitialInfo.NoOfNights;
                    HotelBooking.OrderDetails.CheckInDate = roomInitialInfo.StartDate;
                    HotelBooking.OrderDetails.CheckOutDate = roomInitialInfo.EndDate;
                    HotelBooking.OrderDetails.HotelRoomDto.TotalDays = roomInitialInfo.NoOfNights;
                    HotelBooking.OrderDetails.HotelRoomDto.TotalAmount =
                        roomInitialInfo.NoOfNights * HotelBooking.OrderDetails.HotelRoomDto.RegularRate;
                }
                else
                {
                    var hotelRoomDetails = await HotelRoomService.GetHotelRoomDetailsAsync(
                                            Id.Value, DateTime.UtcNow,
                                            DateTime.UtcNow.AddDays(1));
                    if (hotelRoomDetails is null)
                    {
                        await JsRuntime.ToastrError("hotelRoomDetails is null");
                        return;
                    }

                    HotelBooking.OrderDetails.HotelRoomDto = hotelRoomDetails;
                    NoOfNights = 1;
                    HotelBooking.OrderDetails.CheckInDate = DateTime.UtcNow;
                    HotelBooking.OrderDetails.CheckOutDate = DateTime.UtcNow.AddDays(1);
                    HotelBooking.OrderDetails.HotelRoomDto.TotalDays = 1;
                    HotelBooking.OrderDetails.HotelRoomDto.TotalAmount =
                        HotelBooking.OrderDetails.HotelRoomDto.RegularRate;
                }

                if (await LocalStorage.GetItemAsync<UserDto>(ConstantKeys.LocalUserDetails) != null)
                {
                    var userInfo = await LocalStorage.GetItemAsync<UserDto>(ConstantKeys.LocalUserDetails);
                    if (userInfo is null)
                    {
                        await JsRuntime.ToastrError("Couldn't get the userInfo from LocalStorage.");
                        return;
                    }

                    HotelBooking.OrderDetails.UserId = userInfo.Id ?? "";
                    HotelBooking.OrderDetails.Name = userInfo.Name ?? "";
                    HotelBooking.OrderDetails.Email = userInfo.Email ?? "";
                    HotelBooking.OrderDetails.Phone = userInfo.PhoneNo;
                }
            }
        }
        catch (Exception e)
        {
            await JsRuntime.ToastrError(e.Message);
        }
    }

    private async Task HandleNoOfNightsChange(ChangeEventArgs e)
    {
        if (e.Value is null)
        {
            await JsRuntime.ToastrError("Please select a value");
            return;
        }

        if (Id is null)
        {
            await JsRuntime.ToastrError("The room-id is null");
            return;
        }

        NoOfNights = Convert.ToInt32(e.Value.ToString(), CultureInfo.InvariantCulture);
        var hotelBookingOrderDetails = HotelBooking.OrderDetails;
        if (hotelBookingOrderDetails is null)
        {
            await JsRuntime.ToastrError("hotelBookingOrderDetails is null");
            return;
        }

        var checkOutDate = hotelBookingOrderDetails.CheckInDate
                                                   .AddDays(NoOfNights);
        var hotelRoomDetails = await HotelRoomService.GetHotelRoomDetailsAsync(Id.Value,
                                                                               hotelBookingOrderDetails.CheckInDate,
                                                                               checkOutDate);
        if (hotelRoomDetails is null)
        {
            await JsRuntime.ToastrError("hotelRoomDetails is null");
            return;
        }

        hotelBookingOrderDetails.HotelRoomDto = hotelRoomDetails;
        hotelBookingOrderDetails.CheckOutDate = checkOutDate;
        hotelBookingOrderDetails.HotelRoomDto.TotalDays = NoOfNights;
        hotelBookingOrderDetails.HotelRoomDto.TotalAmount =
            NoOfNights * hotelBookingOrderDetails.HotelRoomDto.RegularRate;
    }

    private async Task HandleCheckout()
    {
        if (!await HandleValidation())
        {
            return;
        }

        try
        {
            var hotelBookingOrderDetails = HotelBooking.OrderDetails;
            if (hotelBookingOrderDetails is null)
            {
                await JsRuntime.ToastrError("hotelBookingOrderDetails is null");
                return;
            }

            hotelBookingOrderDetails.ParbadTrackingNumber = -1;
            var hotelRoomDto = hotelBookingOrderDetails.HotelRoomDto;
            if (hotelRoomDto is null)
            {
                await JsRuntime.ToastrError("hotelRoomDto is null");
                return;
            }

            hotelBookingOrderDetails.RoomId = hotelRoomDto.Id;
            hotelBookingOrderDetails.TotalCost = hotelRoomDto.TotalAmount;
            var roomOrderDetailsSaved =
                await RoomOrderDetailsService.SaveRoomOrderDetailsAsync(hotelBookingOrderDetails);
            if (roomOrderDetailsSaved is null)
            {
                await JsRuntime.ToastrError("roomOrderDetailsSaved is null");
                return;
            }

            await LocalStorage.SetItemAsync(ConstantKeys.LocalRoomOrderDetails, roomOrderDetailsSaved);

            var httpClientBaseAddress = HttpClient.GetBaseAddressUri();
            var paymentUri = new UriBuilderExt(new Uri(httpClientBaseAddress, "/api/ParbadPayment/PayRoomOrder"))
                             .AddParameter("orderId", roomOrderDetailsSaved.Id.ToString(CultureInfo.InvariantCulture))
                             .AddParameter("amount",
                                           roomOrderDetailsSaved.TotalCost.ToString(CultureInfo.InvariantCulture))
                             .Uri;
            NavigationManager.NavigateTo(paymentUri.ToString(), true);
        }
        catch (Exception e)
        {
            await JsRuntime.ToastrError(e.Message);
        }
    }

    private async Task<bool> HandleValidation()
    {
        var hotelBookingOrderDetails = HotelBooking.OrderDetails;
        if (hotelBookingOrderDetails is null)
        {
            await JsRuntime.ToastrError("hotelBookingOrderDetails is null");
            return false;
        }

        if (string.IsNullOrEmpty(hotelBookingOrderDetails.Name))
        {
            await JsRuntime.ToastrError("Name cannot be empty");
            return false;
        }

        if (string.IsNullOrEmpty(hotelBookingOrderDetails.Phone))
        {
            await JsRuntime.ToastrError("Phone cannot be empty");
            return false;
        }

        if (string.IsNullOrEmpty(hotelBookingOrderDetails.Email))
        {
            await JsRuntime.ToastrError("Email cannot be empty");
            return false;
        }

        return true;
    }
}