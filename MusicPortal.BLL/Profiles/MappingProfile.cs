using AutoMapper;
using MusicPortal.DAL.Entities;
using MusicPortal.BLL.DTO;

namespace MusicPortal.BLL.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDTO>();

        CreateMap<Artist, ArtistDTO>()
            .ForMember(dest => dest.RecordCount, opt => opt.MapFrom(src => src.MusicalRecords.Count));

        CreateMap<Genre, GenreDTO>()
            .ForMember(dest => dest.RecordCount, opt => opt.MapFrom(src => src.Records.Count));

        CreateMap<Record, RecordDTO>()
            .ForMember(dest => dest.ArtistName,
                opt => opt.MapFrom(src =>
                    src.Artist != null ? src.Artist.Name : null))
            .ForMember(dest => dest.GenreName,
                opt => opt.MapFrom(src => src.Genre.Name));

        CreateMap<Playlist, PlaylistDTO>();

        CreateMap<PlaylistRecord, PlaylistRecordDTO>();
    }
}

