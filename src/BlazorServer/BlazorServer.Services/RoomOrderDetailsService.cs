using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlazorServer.Common;
using BlazorServer.DataAccess;
using BlazorServer.Entities;
using BlazorServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorServer.Services;

public class RoomOrderDetailsService : IRoomOrderDetailsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IConfigurationProvider _mapperConfiguration;

    public RoomOrderDetailsService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mapperConfiguration = mapper.ConfigurationProvider;
    }

    public async Task<RoomOrderDetailsDto> CreateAsync(RoomOrderDetailsDto details)
    {
        var roomOrder = _mapper.Map<RoomOrderDetail>(details);
        roomOrder.Status = BookingStatus.Pending;
        var result = await _dbContext.RoomOrderDetails.AddAsync(roomOrder);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<RoomOrderDetailsDto>(result.Entity);
    }

    public Task<List<RoomOrderDetailsDto>> GetAllRoomOrderDetailsAsync()
    {
        return _dbContext.RoomOrderDetails
                         .Include(roomOrderDetail => roomOrderDetail.HotelRoom)
                         .ProjectTo<RoomOrderDetailsDto>(_mapperConfiguration)
                         .ToListAsync();
    }

    public async Task<RoomOrderDetailsDto?> GetRoomOrderDetailAsync(int roomOrderId)
    {
        var roomOrderDetailsDto = await _dbContext.RoomOrderDetails
                                                  .Include(u => u.HotelRoom)
                                                  .ThenInclude(x => x.HotelRoomImages)
                                                  .ProjectTo<RoomOrderDetailsDto>(_mapperConfiguration)
                                                  .FirstOrDefaultAsync(u => u.Id == roomOrderId);
        if (roomOrderDetailsDto is null)
        {
            return null;
        }

        var hotelRoomDto = roomOrderDetailsDto.HotelRoomDto;
        if (hotelRoomDto is null)
        {
            return null;
        }

        hotelRoomDto.TotalDays =
            roomOrderDetailsDto.CheckOutDate.Subtract(roomOrderDetailsDto.CheckInDate).Days;
        return roomOrderDetailsDto;
    }

    public async Task<RoomOrderDetailsDto?> GetOrderDetailByTrackingNumberAsync(long trackingNumber)
    {
        var roomOrderDetailsDto = await _dbContext.RoomOrderDetails
                                                  .Include(u => u.HotelRoom)
                                                  .ThenInclude(x => x.HotelRoomImages)
                                                  .ProjectTo<RoomOrderDetailsDto>(_mapperConfiguration)
                                                  .FirstOrDefaultAsync(u => u.ParbadTrackingNumber == trackingNumber);
        if (roomOrderDetailsDto is null)
        {
            return null;
        }

        var hotelRoomDto = roomOrderDetailsDto.HotelRoomDto;
        if (hotelRoomDto is null)
        {
            return null;
        }

        hotelRoomDto.TotalDays =
            roomOrderDetailsDto.CheckOutDate.Subtract(roomOrderDetailsDto.CheckInDate).Days;
        return roomOrderDetailsDto;
    }

    public Task<bool> IsRoomBookedAsync(int roomId, DateTime checkInDate, DateTime checkOutDate)
    {
        return _dbContext.RoomOrderDetails
                         .AnyAsync(
                                   roomOrderDetail =>
                                       roomOrderDetail.RoomId == roomId &&
                                       roomOrderDetail.IsPaymentSuccessful &&
                                       (
                                           (checkInDate < roomOrderDetail.CheckOutDate.Date &&
                                            checkInDate > roomOrderDetail.CheckInDate.Date) ||
                                           (checkOutDate > roomOrderDetail.CheckInDate.Date &&
                                            checkInDate < roomOrderDetail.CheckInDate.Date)
                                       )
                                  );
    }

    public async Task<RoomOrderDetailsDto?> MarkPaymentSuccessfulAsync(long trackingNumber, long amount)
    {
        var order = await _dbContext.RoomOrderDetails
                                    .FirstOrDefaultAsync(x => x.ParbadTrackingNumber == trackingNumber);
        if (order is null || order.IsPaymentSuccessful || order.TotalCost != amount)
        {
            return null;
        }

        order.IsPaymentSuccessful = true;
        order.Status = BookingStatus.Booked;
        var markPaymentSuccessful = _dbContext.RoomOrderDetails.Update(order);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<RoomOrderDetailsDto>(markPaymentSuccessful.Entity);
    }

    public async Task UpdateRoomOrderTrackingNumberAsync(int roomOrderId, long trackingNumber)
    {
        var order = await _dbContext.RoomOrderDetails.FindAsync(roomOrderId);
        if (order is null)
        {
            return;
        }

        order.ParbadTrackingNumber = trackingNumber;
        _dbContext.RoomOrderDetails.Update(order);
        await _dbContext.SaveChangesAsync();
    }
}