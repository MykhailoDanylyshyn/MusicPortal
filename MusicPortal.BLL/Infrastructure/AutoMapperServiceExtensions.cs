using Microsoft.Extensions.DependencyInjection;
using MusicPortal.BLL.Profiles;

namespace MusicPortal.BLL.Infrastructure;

public static class AutoMapperServiceExtensions
{
    public static void AddMusicPortalAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => 
        {
            cfg.AddProfile<MappingProfile>();
        });
    }
}