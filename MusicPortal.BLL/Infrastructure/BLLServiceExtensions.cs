using Microsoft.Extensions.DependencyInjection;
using MusicPortal.BLL.Interfaces;
using MusicPortal.BLL.Services;

namespace MusicPortal.BLL.Infrastructure;

public static class BLLServiceExtensions
{
    public static void AddMusicPortalBLL(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IArtistService, ArtistService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IRecordService, RecordService>();
        services.AddScoped<IPlaylistService, PlaylistService>();
    }
}