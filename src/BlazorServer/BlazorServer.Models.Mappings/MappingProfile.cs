using AutoMapper;
using BlazorServer.Entities;

namespace BlazorServer.Models.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<HotelRoomDto, HotelRoom>().ReverseMap(); // two-way mapping
        CreateMap<HotelRoomImageDto, HotelRoomImage>().ReverseMap(); // two-way mapping
        CreateMap<HotelAmenity, HotelAmenityDto>().ReverseMap(); // two-way mapping
        CreateMap<RoomOrderDetail, RoomOrderDetailsDto>().ReverseMap(); // two-way mapping
    }
}