using AutoMapper;
using AutoMapper.QueryableExtensions;
using BlazorServer.DataAccess;
using BlazorServer.Entities;
using BlazorServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorServer.Services;

public class AmenityService : IAmenityService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IConfigurationProvider _mapperConfiguration;
    private bool _isDisposed;

    public AmenityService(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mapperConfiguration = mapper.ConfigurationProvider;
    }

    public async Task<HotelAmenityDto> CreateHotelAmenityAsync(HotelAmenityDto hotelAmenity)
    {
        var amenity = _mapper.Map<HotelAmenity>(hotelAmenity);
        amenity.CreatedBy = "";
        amenity.CreatedDate = DateTime.UtcNow;
        var addedHotelAmenity = await _dbContext.HotelAmenities.AddAsync(amenity);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<HotelAmenityDto>(addedHotelAmenity.Entity);
    }

    public async Task<HotelAmenityDto> UpdateHotelAmenityAsync(int amenityId, HotelAmenityDto hotelAmenity)
    {
        var amenityDetails = await _dbContext.HotelAmenities.FindAsync(amenityId);
        var amenity = _mapper.Map(hotelAmenity, amenityDetails);
        if (amenity is null)
        {
            throw new InvalidOperationException("amenity is null");
        }

        amenity.UpdatedBy = "";
        amenity.UpdatedDate = DateTime.UtcNow;
        var updatedAmenity = _dbContext.HotelAmenities.Update(amenity);
        await _dbContext.SaveChangesAsync();
        return _mapper.Map<HotelAmenityDto>(updatedAmenity.Entity);
    }

    public async Task<int> DeleteHotelAmenityAsync(int amenityId, string userId)
    {
        var amenityDetails = await _dbContext.HotelAmenities.FindAsync(amenityId);
        if (amenityDetails == null)
        {
            return 0;
        }

        _dbContext.HotelAmenities.Remove(amenityDetails);
        return await _dbContext.SaveChangesAsync();
    }

    public Task<List<HotelAmenityDto>> GetAllHotelAmenityAsync() =>
        _dbContext.HotelAmenities
                  .ProjectTo<HotelAmenityDto>(_mapperConfiguration)
                  .ToListAsync();

    public Task<HotelAmenityDto?> GetHotelAmenityAsync(int amenityId)
    {
        return _dbContext.HotelAmenities
                         .ProjectTo<HotelAmenityDto>(_mapperConfiguration)
                         .FirstOrDefaultAsync(x => x.Id == amenityId);
    }

    public Task<HotelAmenityDto?> IsSameNameAmenityAlreadyExistsAsync(string name)
    {
        return _dbContext.HotelAmenities
                         .ProjectTo<HotelAmenityDto>(_mapperConfiguration)
                         .FirstOrDefaultAsync(x => x.Name == name);
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