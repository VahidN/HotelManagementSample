namespace BlazorServer.Models;

public class HotelAmenityDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter the amenity name")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Please enter the amenity timming")]
    public string Timming { get; set; } = default!;

    [Required(ErrorMessage = "Please enter the amenity description")]
    public string Description { get; set; } = default!;

    [Required(ErrorMessage = "Please enter the amenity icon from a web-font")]
    public string IconStyle { get; set; } = default!;

    public string? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }
}