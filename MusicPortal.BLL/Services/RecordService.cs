using AutoMapper;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;
using MusicPortal.DAL.Entities;
using MusicPortal.DAL.Interfaces;

namespace MusicPortal.BLL.Services;

public class RecordService : IRecordService
{
    private readonly IUnitOfWork database;
    private readonly IMapper mapper;

    public RecordService(IUnitOfWork database, IMapper mapper)
    {
        this.database = database;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<RecordDTO>> GetAllRecords()
    {
        var records = await database.Records.GetAll();
        return mapper.Map<IEnumerable<RecordDTO>>(records);
    }

    public async Task<RecordDTO?> GetRecord(int id)
    {
        var record = await database.Records.Get(id);
        return mapper.Map<RecordDTO?>(record);
    }

    public async Task CreateRecord(RecordDTO record)
    {
        var newRecord = new Record
        {
            Title = record.Title,            
            AudioUrl = record.AudioUrl,
            ArtistId = record.ArtistId,
            GenreId = record.GenreId
        };

        await database.Records.Create(newRecord);
        await database.Save();
    }

    public async Task UpdateRecord(RecordDTO record)
    {
        var existingRecord = await database.Records.Get(record.Id);

        if (existingRecord == null)
            return;

        existingRecord.Title = record.Title;
        existingRecord.AudioUrl = record.AudioUrl;
        existingRecord.ArtistId = record.ArtistId;
        existingRecord.GenreId = record.GenreId;

        database.Records.Update(existingRecord);
        await database.Save();
    }

    public async Task DeleteRecord(int id)
    {
        await database.Records.Delete(id);
        await database.Save();
    }
}
