using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlazorServer.DataAccess;
using BlazorServer.Entities;
using BlazorServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorServer.Services;

public class HotelRoomImageService : IHotelRoomImageService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IConfigurationProvider _mapperConfiguration;
    private bool _isDisposed;

    public HotelRoomImageService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mapperConfiguration = mapper.ConfigurationProvider;
    }

    public async Task<int> CreateHotelRoomImageAsync(HotelRoomImageDto imageDto)
    {
        var image = _mapper.Map<HotelRoomImage>(imageDto);
        await _dbContext.HotelRoomImages.AddAsync(image);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteHotelRoomImageByImageIdAsync(int imageId)
    {
        var image = await _dbContext.HotelRoomImages.FindAsync(imageId);
        if (image is null)
        {
            return -1;
        }

        _dbContext.HotelRoomImages.Remove(image);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteHotelRoomImageByRoomIdAsync(int roomId)
    {
        var imageList = await _dbContext.HotelRoomImages.Where(x => x.RoomId == roomId).ToListAsync();
        _dbContext.HotelRoomImages.RemoveRange(imageList);
        return await _dbContext.SaveChangesAsync();
    }

    public Task<List<HotelRoomImageDto>> GetHotelRoomImagesAsync(int roomId)
    {
        return _dbContext.HotelRoomImages
                         .Where(x => x.RoomId == roomId)
                         .ProjectTo<HotelRoomImageDto>(_mapperConfiguration)
                         .ToListAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
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