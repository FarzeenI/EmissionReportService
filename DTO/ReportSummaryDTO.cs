namespace EmissionReportService.DTO
{
    public class ReportSummaryDTO
    {
        public int TotalRecords { get; set; }     

        public required Dictionary<string, object> EmissionsByModel { get; set; }

        public required Dictionary<string, object> EmissionsByModelAndMaterialNumber { get; set; }
    }
}
