namespace BlazorServer.Models;

public class HotelRoomDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter the room's name")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Please enter the occupancy")]
    public int Occupancy { get; set; }

    [Range(1, 30000, ErrorMessage = "Regular rate must be between 1 and 30000")]
    public long RegularRate { get; set; }

    public string? Details { get; set; }

    public string? SqFt { get; set; }

    public int TotalDays { get; set; }
    public long TotalAmount { get; set; }

    public bool IsBooked { get; set; }

    public virtual ICollection<HotelRoomImageDto> HotelRoomImages { get; set; } = new List<HotelRoomImageDto>();

    public virtual ICollection<RoomOrderDetailsDto> RoomOrderDetails { get; set; } = new List<RoomOrderDetailsDto>();
}