namespace BlazorServer.Entities;

public class HotelRoom
{
    [Key] public int Id { get; set; }

    [Required] public string Name { get; set; } = default!;

    [Required] public int Occupancy { get; set; }

    [Required] public long RegularRate { get; set; }

    public string? Details { get; set; }

    public string? SqFt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public string? UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual ICollection<HotelRoomImage> HotelRoomImages { get; set; } = new List<HotelRoomImage>();

    public virtual ICollection<RoomOrderDetail> RoomOrderDetails { get; set; } = new List<RoomOrderDetail>();
}