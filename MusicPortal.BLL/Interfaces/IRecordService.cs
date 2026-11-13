using MusicPortal.BLL.DTO;

namespace MusicPortal.BLL.Interfaces;

public interface IRecordService
{
    Task<IEnumerable<RecordDTO>> GetAllRecords();

    Task<RecordDTO?> GetRecord(int id);

    Task CreateRecord(RecordDTO record);

    Task UpdateRecord(RecordDTO record);

    Task DeleteRecord(int id);
}
