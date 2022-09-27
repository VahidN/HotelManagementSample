namespace BlazorServer.Models;

public class HotelRoomImageDto
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public string? RoomImageUrl { get; set; }
}