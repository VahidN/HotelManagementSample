namespace BlazorServer.Models;

public class RoomOrderDetailsDto
{
    public int Id { get; set; }

    [Required] public string UserId { get; set; } = default!;

    [Required] public long ParbadTrackingNumber { get; set; }

    [Required] public DateTime CheckInDate { get; set; }

    [Required] public DateTime CheckOutDate { get; set; }

    public DateTime ActualCheckInDate { get; set; }

    public DateTime ActualCheckOutDate { get; set; }

    [Required] public long TotalCost { get; set; }

    [Required] public int RoomId { get; set; }

    public bool IsPaymentSuccessful { get; set; }

    [Required] public string Name { get; set; } = default!;

    [Required] public string Email { get; set; } = default!;

    public string? Phone { get; set; }

    public HotelRoomDto? HotelRoomDto { get; set; }

    public string? Status { get; set; }
}