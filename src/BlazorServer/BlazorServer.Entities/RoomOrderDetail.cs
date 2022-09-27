namespace BlazorServer.Entities;

public class RoomOrderDetail
{
    public int Id { get; set; }

    [Required] public string UserId { get; set; } = default!;

    [Required] public long ParbadTrackingNumber { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public DateTime ActualCheckInDate { get; set; }

    public DateTime ActualCheckOutDate { get; set; }

    public long TotalCost { get; set; }

    public int RoomId { get; set; }

    public bool IsPaymentSuccessful { get; set; }

    [Required] public string Name { get; set; } = default!;

    [Required] public string Email { get; set; } = default!;

    public string? Phone { get; set; }

    [ForeignKey("RoomId")] public HotelRoom HotelRoom { get; set; } = default!;

    public string? Status { get; set; }
}