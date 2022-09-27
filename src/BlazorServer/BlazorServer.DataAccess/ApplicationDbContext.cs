using BlazorServer.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorServer.DataAccess;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<HotelRoom> HotelRooms { get; set; } = default!;

    public DbSet<HotelRoomImage> HotelRoomImages { get; set; } = default!;

    public DbSet<HotelAmenity> HotelAmenities { get; set; } = default!;

    public DbSet<RoomOrderDetail> RoomOrderDetails { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        base.OnModelCreating(builder); // Calling this line is necessary for the IdentityDbContext

        SeedData(builder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HotelAmenity>().HasData(
                                                    new HotelAmenity
                                                    {
                                                        Id = 1,
                                                        Name = "Gym",
                                                        Timming = "Timming 1",
                                                        Description = "24x7 gym room is available.",
                                                        IconStyle = "gym",
                                                        CreatedBy = "",
                                                        CreatedDate =
                                                            DateTime.Parse("2012-07-10 11:30:00",
                                                                           CultureInfo.InvariantCulture),
                                                        UpdatedBy = "",
                                                        UpdatedDate =
                                                            DateTime.Parse("2012-07-10 11:30:00",
                                                                           CultureInfo.InvariantCulture),
                                                    },
                                                    new HotelAmenity
                                                    {
                                                        Id = 2,
                                                        Name = "Free Breakfast",
                                                        Timming = "Timming 2",
                                                        Description = "Enjoy free breakfast at out hotel.",
                                                        IconStyle = "heart",
                                                        CreatedBy = "",
                                                        CreatedDate =
                                                            DateTime.Parse("2012-07-10 11:30:00",
                                                                           CultureInfo.InvariantCulture),
                                                        UpdatedBy = "",
                                                        UpdatedDate =
                                                            DateTime.Parse("2012-07-10 11:30:00",
                                                                           CultureInfo.InvariantCulture),
                                                    }
                                                   );

        modelBuilder.Entity<HotelRoom>().HasData(
                                                 new HotelRoom
                                                 {
                                                     Id = 1,
                                                     Name = "Room 1",
                                                     Occupancy = 2,
                                                     RegularRate = 1000,
                                                     Details = "Test Test!",
                                                     SqFt = "30",
                                                     CreatedBy = "",
                                                     CreatedDate =
                                                         DateTime.Parse("2012-07-10 11:30:00",
                                                                        CultureInfo.InvariantCulture),
                                                     UpdatedBy = "",
                                                     UpdatedDate =
                                                         DateTime.Parse("2012-07-10 11:30:00",
                                                                        CultureInfo.InvariantCulture),
                                                 },
                                                 new HotelRoom
                                                 {
                                                     Id = 2,
                                                     Name = "Room 3",
                                                     Occupancy = 3,
                                                     RegularRate = 1500,
                                                     Details = "Test Test!",
                                                     SqFt = "50",
                                                     CreatedBy = "",
                                                     CreatedDate =
                                                         DateTime.Parse("2012-07-10 11:30:00",
                                                                        CultureInfo.InvariantCulture),
                                                     UpdatedBy = "",
                                                     UpdatedDate =
                                                         DateTime.Parse("2012-07-10 11:30:00",
                                                                        CultureInfo.InvariantCulture),
                                                 });
    }
}