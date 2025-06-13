namespace EmissionReportService.DTO
{
    public class EmissionRecordDTO
    {
        public string Material_Hierarchy_Model_Node_Name { get; set; } 
        public string Material_Hierarchy_Parent_Hierarchy_Identifier { get; set; } 

        public string Material_Number { get; set; } 
        public string Iso_Country_Code { get; set; } 

        public int Category_ID { get; set; }
        public string Category_Name { get; set; } 

        public DateTime Source_Create_Timestamp { get; set; }
        public bool Logical_Deletion_Indicator { get; set; }
        public double Production_Rounded_KgCO2 { get; set; }
        public double Transport_Rounded_KgCO2 { get; set; }
        public double Energy_Use_Rounded_KgCO2 { get; set; }
        public double Eol_Rounded_KgCO2 { get; set; }
        public double Cartridges_Rounded_KgCO2 { get; set; }
        public double Consumables_Rounded_KgCO2 { get; set; }
        public double Paper_Rounded_KgCO2 { get; set; }
        public double Total_Rounded_KgCO2 { get; set; }
        public double Total_Lower_Bounds_KgCO2 { get; set; }
        public double Total_Upper_Bounds_KgCO2 { get; set; }
        public DateTime Source_System_Update_Timestamp { get; set; }
        public DateTime Insert_Gmt_Timestamp { get; set; }
    }
}
