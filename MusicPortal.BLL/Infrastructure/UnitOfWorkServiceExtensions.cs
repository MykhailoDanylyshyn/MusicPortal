using Microsoft.Extensions.DependencyInjection;
using MusicPortal.DAL.Interfaces;
using MusicPortal.DAL.Repositories;

namespace MusicPortal.BLL.Infrastructure;

public static class UnitOfWorkServiceExtensions
{
    public static void AddUnitOfWorkService(this IServiceCollection services) =>
        services.AddScoped<IUnitOfWork, EFUnitOfWork>();
}