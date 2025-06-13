using EmissionReportService.DTO;    

namespace EmissionReportService.Service
{
    public interface IEmissionReportService
    {
        public Task<List<EmissionRecordDTO>> GetReportsByMaterialNo(string? materialNo);
        public Task<List<EmissionRecordDTO>> GetReportsByCountryCode(string? isoCode);
        public Task<List<EmissionRecordDTO>> GetReportsByCategoryId(int categoryId);
        public Task<List<EmissionRecordDTO>> GetReportsByRange(DateTime? startDate, DateTime? endDate);
        public Task<List<EmissionRecordDTO>> GetReportOutliers();
        public Task<ReportSummaryDTO> GetReportsSummary();
    }
}
