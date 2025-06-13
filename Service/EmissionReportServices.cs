using CsvHelper;
using EmissionReportService.DTO;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace EmissionReportService.Service
{
    public class EmissionReportServices : IEmissionReportService
    {
        private readonly IEmissionDataClient _emissionDataClient;
        private readonly ILogger<EmissionReportServices> _logger;

        public EmissionReportServices(IEmissionDataClient emissionDataClient, ILogger<EmissionReportServices> logger)
        {
            _emissionDataClient = emissionDataClient;
            _logger = logger;
        }

        public async Task<List<EmissionRecordDTO>> GetReportsByMaterialNo(string? materialNo)
        {                 
            _logger.LogInformation("Fetching emissions by Material No: {MaterialNo}", materialNo);
            if (string.IsNullOrWhiteSpace(materialNo))
            {
                _logger.LogWarning("Material number is required, cannot be null.");
                throw new ArgumentException("Material number cannot be null or empty.");
            }
            return await _emissionDataClient.GetByMaterialNoAsync(materialNo);
        }

        public async Task<List<EmissionRecordDTO>> GetReportsByCountryCode(string? isoCode)
        {
            _logger.LogInformation("Fetching emissions by Country Code: {IsoCode}", isoCode);
            if (string.IsNullOrWhiteSpace(isoCode))
            {
                _logger.LogWarning("Country ISO code is required, cannot be null.");
                throw new ArgumentException("Country ISO code cannot be null or empty.");
            }

            return await _emissionDataClient.GetByCountryCodeAsync(isoCode);
        }

        public async Task<List<EmissionRecordDTO>> GetReportsByCategoryId(int categoryId)
        {
            _logger.LogInformation("Fetching emissions by Category ID: {CategoryId}", categoryId);
            var allData = await _emissionDataClient.GetAllEmissionDataRecordsAsync();
            return allData.Where(r => r.Category_ID == categoryId).ToList();
        }

        public async Task<List<EmissionRecordDTO>> GetReportsByRange(DateTime? startDate, DateTime? endDate)
        {
            _logger.LogInformation("Fetching emissions from {StartDate} to {EndDate}", startDate, endDate);
            if (startDate == null || endDate == null)
            {
                _logger.LogWarning("Both start and end dates are required.");
                throw new ArgumentException("Both start and end dates are required.");

            }
            if (startDate > endDate)
            {
                _logger.LogWarning("Start date must be before end date.");
                throw new ArgumentException("Start date must be before end date.");
            }
            
            var allRecords = await _emissionDataClient.GetAllEmissionDataRecordsAsync();
            return allRecords
                .Where(r => r.Source_Create_Timestamp >= startDate && r.Source_Create_Timestamp <= endDate)
                .ToList();
        }

        public async Task<List<EmissionRecordDTO>> GetReportOutliers()
        {
            _logger.LogInformation("Exporting all emission records to CSV");

            var allRecords = await _emissionDataClient.GetAllEmissionDataRecordsAsync();

            if (allRecords == null || !allRecords.Any())
            {
                _logger.LogWarning("No emission records available to export.");
                throw new InvalidOperationException("No emission records available to export.");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "AllEmissionRecords.csv");

            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(allRecords);

            return allRecords;
        }

        public async Task<ReportSummaryDTO> GetReportsSummary()
        {
            _logger.LogInformation("Generating summary report");

            var all = await _emissionDataClient.GetAllEmissionDataRecordsAsync();

            if (all == null || !all.Any())
                throw new InvalidOperationException("No data available to generate summary.");

            var modelData = all
                .GroupBy(r => r.Material_Hierarchy_Model_Node_Name)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Total_Rounded_KgCO2));

            var modelMaterialData = all
                .GroupBy(r => $"{r.Material_Hierarchy_Model_Node_Name} - {r.Material_Number}")
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Total_Rounded_KgCO2));

            return new ReportSummaryDTO
            {
                TotalRecords = all.Count,
                EmissionsByModel = new Dictionary<string, object>
                {
                    { "title", "Total Emissions by Model" },
                    { "unit", "kgCO2" },
                    { "data", modelData }
                },
                EmissionsByModelAndMaterialNumber = new Dictionary<string, object>
                {
                    { "title", "Total Emissions by Model and Material Number" },
                    { "unit", "kgCO2" },
                    { "data", modelMaterialData }
                }
            };
        }
    }
}
