using EmissionReportService.DTO;

namespace EmissionReportService.Service
{
    public interface IEmissionDataClient
    {
        Task<List<EmissionRecordDTO>> GetAllEmissionDataRecordsAsync();
        Task<List<EmissionRecordDTO>> GetByMaterialNoAsync(string? materialNo);
        Task<List<EmissionRecordDTO>> GetByCountryCodeAsync(string? isoCode);
        Task<EmissionRecordDTO?> AddNewEmissionAsync(EmissionRecordDTO emissionDataRecord);
        Task<EmissionRecordDTO?> UpdateByEmissionAsync(EmissionRecordDTO emissionDataRecord);
    }
}
