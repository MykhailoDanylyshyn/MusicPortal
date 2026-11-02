using AutoMapper;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;
using MusicPortal.DAL.Entities;
using MusicPortal.DAL.Interfaces;

namespace MusicPortal.BLL.Services;

public class PlaylistService : IPlaylistService
{
    private readonly IUnitOfWork database;
    private readonly IMapper mapper;

    public PlaylistService(IUnitOfWork database, IMapper mapper)
    {
        this.database = database;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<PlaylistDTO>> GetAllPlaylists()
    {
        var playlists = await database.Playlists.GetAll();
        return mapper.Map<IEnumerable<PlaylistDTO>>(playlists);
    }

    public async Task<PlaylistDTO?> GetPlaylist(int id)
    {
        var playlist = await database.Playlists.Get(id);
        return mapper.Map<PlaylistDTO?>(playlist);
    }

    public async Task CreatePlaylist(PlaylistDTO playlist)
    {
        var newPlaylist = new Playlist
        {
            Name = playlist.Name,
            Description = playlist.Description,
            UserId = playlist.UserId
        };

        await database.Playlists.Create(newPlaylist);
        await database.Save();
    }

    public async Task UpdatePlaylist(PlaylistDTO playlist)
    {
        var existingPlaylist = await database.Playlists.Get(playlist.Id);

        if (existingPlaylist == null)
            return;

        existingPlaylist.Name = playlist.Name;
        existingPlaylist.Description = playlist.Description;

        database.Playlists.Update(existingPlaylist);
        await database.Save();
    }

    public async Task DeletePlaylist(int id)
    {
        await database.Playlists.Delete(id);
        await database.Save();
    }
}
