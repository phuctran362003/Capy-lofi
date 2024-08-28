using AutoMapper;
using Domain.DTOs.BackgroundDTOs;
using Domain.DTOs.MessageDTOs;
using Domain.DTOs.MusicsDTOs;
using Domain.Entities;


namespace Service.Mappers
{
    public class MapperConfigProfile : Profile
    {
        public MapperConfigProfile()
        {
            CreateMap<Background, BackgroundDTO>()
                .ReverseMap();

            CreateMap<Background, CreateBackgroundDTO>()
                .ReverseMap();

            CreateMap<Music, MusicDTO>()
                .ReverseMap();

            CreateMap<Music, CreateMusicDTO>()
                .ReverseMap();

            CreateMap<Message, MessageDTO>()
                .ReverseMap();
        }
    }
}
