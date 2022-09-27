using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlazorServer.DataAccess;
using BlazorServer.Entities;
using BlazorServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorServer.Services;

public class HotelRoomService : IHotelRoomService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IConfigurationProvider _mapperConfiguration;
    private bool _isDisposed;

    public HotelRoomService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mapperConfiguration = mapper.ConfigurationProvider;
    }

    public async Task<HotelRoomDto> CreateHotelRoomAsync(HotelRoomDto hotelRoomDto)
    {
        var hotelRoom = _mapper.Map<HotelRoom>(hotelRoomDto);
        hotelRoom.CreatedDate = DateTime.UtcNow;
        hotelRoom.CreatedBy = "";
        var addedHotelRoom = await _dbContext.HotelRooms.AddAsync(hotelRoom);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<HotelRoomDto>(addedHotelRoom.Entity);
    }

    public async Task<int> DeleteHotelRoomAsync(int roomId)
    {
        var roomDetails = await _dbContext.HotelRooms.FindAsync(roomId);
        if (roomDetails == null)
        {
            return 0;
        }

        _dbContext.HotelRooms.Remove(roomDetails);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<List<HotelRoomDto>> GetAllHotelRoomsAsync(
        DateTime? checkInDate = null, DateTime? checkOutDate = null)
    {
        var hotelRooms = await _dbContext.HotelRooms
                                         .Include(x => x.HotelRoomImages)
                                         .Include(x => x.RoomOrderDetails)
                                         .ProjectTo<HotelRoomDto>(_mapperConfiguration)
                                         .ToListAsync();

        foreach (var hotelRoom in hotelRooms)
        {
            hotelRoom.IsBooked = IsRoomBooked(hotelRoom, checkInDate, checkOutDate);
        }

        return hotelRooms;
    }

    public async Task<HotelRoomDto?> GetHotelRoomAsync(
        int roomId, DateTime? checkInDate = null, DateTime? checkOutDate = null)
    {
        var hotelRoom = await _dbContext.HotelRooms
                                        .Include(x => x.HotelRoomImages)
                                        .Include(x => x.RoomOrderDetails)
                                        .ProjectTo<HotelRoomDto>(_mapperConfiguration)
                                        .FirstOrDefaultAsync(x => x.Id == roomId);
        if (hotelRoom is null)
        {
            return null;
        }

        hotelRoom.IsBooked = IsRoomBooked(hotelRoom, checkInDate, checkOutDate);
        return hotelRoom;
    }

    public async Task<bool> IsRoomUniqueAsync(string name, int roomId)
    {
        if (roomId == 0)
        {
            // Create Mode
            return !await _dbContext.HotelRooms.AnyAsync(x => x.Name == name);
        }

        // Edit Mode
        return !await _dbContext.HotelRooms.AnyAsync(x => x.Name == name && x.Id != roomId);
    }

    public async Task<HotelRoomDto?> UpdateHotelRoomAsync(HotelRoomDto hotelRoomDto)
    {
        // Note: Without this `Include`, HotelRoomImages will be in the detached state and won't be update here.
        var actualDbRoom = await _dbContext.HotelRooms.Include(x => x.HotelRoomImages)
                                           .FirstOrDefaultAsync(x => x.Id == hotelRoomDto.Id);
        if (actualDbRoom is null)
        {
            return null;
        }

        var updatedDbRoom = _mapper.Map(hotelRoomDto, actualDbRoom);
        updatedDbRoom.UpdatedBy = "";
        updatedDbRoom.UpdatedDate = DateTime.UtcNow;
        var updatedRoomEntityEntry = _dbContext.HotelRooms.Update(updatedDbRoom);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<HotelRoomDto>(updatedRoomEntityEntry.Entity);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private static bool IsRoomBooked(HotelRoomDto hotelRoom, DateTime? checkInDate, DateTime? checkOutDate)
    {
        if (checkInDate == null || checkOutDate == null)
        {
            return false;
        }

        return hotelRoom.RoomOrderDetails.Any(x => x.IsPaymentSuccessful &&
                                                   //check if checkin date that user wants does not fall in between any dates for room that is booked
                                                   ((checkInDate < x.CheckOutDate &&
                                                     checkInDate.Value.Date >= x.CheckInDate)
                                                    //check if checkout date that user wants does not fall in between any dates for room that is booked
                                                    || (checkOutDate.Value.Date > x.CheckInDate.Date &&
                                                        checkInDate.Value.Date <= x.CheckInDate.Date))
                                             );
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            try
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            finally
            {
                _isDisposed = true;
            }
        }
    }
}