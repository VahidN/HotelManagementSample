using BlazorServer.Models;

namespace BlazorWasm.Client.Services;

public interface IClientRoomOrderDetailsService
{
    Task<RoomOrderDetailsDto?> SaveRoomOrderDetailsAsync(RoomOrderDetailsDto details);
    Task<RoomOrderDetailsDto?> GetOrderDetailAsync(long trackingNumber);
}