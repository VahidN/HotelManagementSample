namespace BlazorServer.Entities;

public class HotelAmenity
{
    public int Id { get; set; }

    [Required] public string Name { get; set; } = default!;

    [Required] public string Timming { get; set; } = default!;

    [Required] public string Description { get; set; } = default!;

    [Required] public string IconStyle { get; set; } = default!;

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }
}