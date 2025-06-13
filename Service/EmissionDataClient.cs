using Azure;
using EmissionReportService.DTO;
using System.Net.Http.Json;
using System.Reflection.Metadata;

namespace EmissionReportService.Service
{
    public class EmissionDataClient : IEmissionDataClient
    {
        private readonly HttpClient _httpClient;

        public EmissionDataClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EmissionRecordDTO>> GetAllEmissionDataRecordsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<EmissionRecordDTO>>("/api/emissions");
            if (result == null)
            {
                return [];
            }
            return result;
        }

        public async Task<List<EmissionRecordDTO>> GetByCountryCodeAsync(string? isoCode)
        {
            var result = await _httpClient.GetFromJsonAsync<List<EmissionRecordDTO>>($"/api/emissions/country?isoCode={isoCode}");
            if (result == null)
            {
                return [];
            }
            return result;
        }

        public async Task<List<EmissionRecordDTO>> GetByMaterialNoAsync(string? materialNo)
        {
            var response = await _httpClient.GetFromJsonAsync<List<EmissionRecordDTO>>($"/api/emissions/material?materialNo={materialNo}");
            if (response == null)
            {                
                throw new ArgumentException("No emission data found for the provided Material Number.");
            }
            return response;

        }

        public async Task<EmissionRecordDTO?> AddNewEmissionAsync(EmissionRecordDTO emissionDataRecord)
        {
            var result = await _httpClient.PostAsJsonAsync("/api/emissions", emissionDataRecord); 
            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadFromJsonAsync<EmissionRecordDTO>();
            }
            return null;
        }
       
        public async Task<EmissionRecordDTO?> UpdateByEmissionAsync(EmissionRecordDTO emissionDataRecord)
        {
            if (string.IsNullOrWhiteSpace(emissionDataRecord.Material_Number))
                throw new ArgumentException("Material_Number is required for update.");

            var response = await _httpClient.PutAsJsonAsync($"/api/emissions/material", emissionDataRecord);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<EmissionRecordDTO>();
            }
            return null;
        
        }
    }
}
