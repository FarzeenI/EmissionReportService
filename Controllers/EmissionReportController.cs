using CsvHelper;
using CsvHelper.Configuration;
using EmissionReportService.DTO;
using EmissionReportService.Service;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace EmissionReportService.Controllers
{
    [ApiExplorerSettings(GroupName = "Reports")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmissionReportController : ControllerBase
    {
        private readonly IEmissionReportService _reportService;
        private readonly ILogger<EmissionReportController> _logger;

        public EmissionReportController(IEmissionReportService reportService, ILogger<EmissionReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet("/api/emissions/material")]
        public async Task<ActionResult<List<EmissionRecordDTO>>> GetByMaterialNoAsync([FromQuery] string? materialNo)
        {
            _logger.LogInformation("Fetching emissions for Material No: {MaterialNo}", materialNo);
            if (string.IsNullOrWhiteSpace(materialNo))
            {
                _logger.LogWarning("Material number is required, cannot be null.");
                throw new ArgumentException("Material number is required.");                

            }           
            var result = await _reportService.GetReportsByMaterialNo(materialNo);
            return Ok(result);
        }

        [HttpGet("/api/emissions/country")]
        public async Task<ActionResult<List<EmissionRecordDTO>>> GetByCountryCodeAsync([FromQuery] string? isoCode)
        {
            _logger.LogInformation("Fetching emissions for Country Code: {IsoCode}", isoCode);
            if (string.IsNullOrWhiteSpace(isoCode))
            {
                _logger.LogWarning("Country ISO code is required, cannot be null.");
                throw new ArgumentException("Country ISO code is required.");
            }
            var result = await _reportService.GetReportsByCountryCode(isoCode);
            return Ok(result);
        }

        [HttpGet("/api/emissions/category")]
        public async Task<ActionResult<List<EmissionRecordDTO>>> GetByCategoryId([FromQuery]int categoryId)
        {
            _logger.LogInformation("Fetching emissions for Category ID: {CategoryId}", categoryId);
            var result = await _reportService.GetReportsByCategoryId(categoryId);
            return Ok(result);
        }

        [HttpGet("/api/emissions/range")]
        public async Task<ActionResult<List<EmissionRecordDTO>>> GetByRange([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
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
            var result = await _reportService.GetReportsByRange(startDate, endDate);
            return Ok(result);
        }

        [HttpGet("/api/emissions/outliers")]
        public async Task<ActionResult> GetOutliers()
        {
            _logger.LogInformation("Fetching outlier emission records");
            var outliers = await _reportService.GetReportOutliers();

            if (outliers == null || !outliers.Any())
            {
                _logger.LogWarning("No emission records available to export.");
                throw new InvalidOperationException("No emission records available to export.");
            }

            _logger.LogInformation("Found {Count} outlier records", outliers.Count);
            var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "|"
            }))
            {
                csv.WriteRecords(outliers);
                writer.Flush();
            }
            memoryStream.Position = 0;

            return File(memoryStream, "text/csv", "OutlierEmissionRecords.csv");
        }

        [HttpGet("/api/emissions/summary")]
        public async Task<ActionResult<ReportSummaryDTO>> GetSummary()
        {
            _logger.LogInformation("Generating summary report for emissions");
            var result = await _reportService.GetReportsSummary();

            if (result == null || result.TotalRecords == 0)
            {
                _logger.LogWarning("No data available to generate summary.");
                throw new InvalidOperationException("No data available to generate summary.");
            }

            return Ok(result);
        }
    }
}
