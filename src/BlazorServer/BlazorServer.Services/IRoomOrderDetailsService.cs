using BlazorServer.Models;

namespace BlazorServer.Services;

public interface IRoomOrderDetailsService
{
    Task<RoomOrderDetailsDto> CreateAsync(RoomOrderDetailsDto details);

    Task<List<RoomOrderDetailsDto>> GetAllRoomOrderDetailsAsync();

    Task<RoomOrderDetailsDto?> GetRoomOrderDetailAsync(int roomOrderId);

    Task<bool> IsRoomBookedAsync(int roomId, DateTime checkInDate, DateTime checkOutDate);

    Task<RoomOrderDetailsDto?> MarkPaymentSuccessfulAsync(long trackingNumber, long amount);

    Task<RoomOrderDetailsDto?> GetOrderDetailByTrackingNumberAsync(long trackingNumber);

    Task UpdateRoomOrderTrackingNumberAsync(int roomOrderId, long trackingNumber);
}