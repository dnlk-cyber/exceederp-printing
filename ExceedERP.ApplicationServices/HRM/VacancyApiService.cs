using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.HRM
{
    public class VacancyApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://jobs.occ-oro.com/api/occ-vacancies";
        private const string Token = "bFAH1ZJmU6oGTr4EXBywPGft73LaYVxWIgQOzjAp5nMbkdqRLEu8vctFhK2sXeMliDTgNJr7V9CWoYZUBpxA3qS0nf4MYhwKE";

        public VacancyApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
        }

        public async Task<string> PostVacancyAsync(VacancyRequest vacancy)
        {
            var json = JsonConvert.SerializeObject(vacancy);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error posting vacancy: {response.StatusCode} - {error}");
            }
            
            return await response.Content.ReadAsStringAsync();
        }
    }

    public class VacancyRequest
    {
        public string title { get; set; }
        public string description { get; set; }
        public string published_date { get; set; }
        public string deadline { get; set; }
        public string employment_type { get; set; }
        public string location { get; set; }
        public string salary { get; set; }
        public string experience { get; set; }
        public string available_positions { get; set; }
        public string job_code { get; set; }
        public string qualification { get; set; }
        public string gpa { get; set; }
        public string education_level { get; set; }
        public string grade { get; set; }
        public List<string> skills { get; set; }
    }


}
