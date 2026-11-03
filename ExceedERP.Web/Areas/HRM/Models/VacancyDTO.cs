using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.HRM.Models
{
    public class VacancyResponseDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("vacancy")]
        public VacancyDto Vacancy { get; set; }
        public class VacancyDataDto

        {
            public VacancyDto Vacancy { get; set; }


        }
    }
    public class VacancyDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("job_code")]
        public string JobCode { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("deadline")]
        public DateTime? Deadline { get; set; }

        [JsonProperty("employment_type")]
        public string EmploymentType { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("available_positions")]
        public string AvailablePositions { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("published_date")]
        public DateTime? PublishedDate { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }

        [JsonProperty("job_applications")]
        public List<JobApplicationDto> JobApplications { get; set; }
    }


    public class JobApplicationDto
    {
        [JsonProperty("id")]
        public int Application_Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("applicant")]
        public ApplicantDto Applicant { get; set; }

        [JsonProperty("cover_letter")]
        public string CoverLetter { get; set; }

        [JsonProperty("resume")]
        public string Resume { get; set; }
    }
    public class ApplicantDto
    {
        [JsonProperty("id")]
        public int ApplicantId { get; set; }

        [JsonProperty("first_name")]
        public string First_Name { get; set; }

        [JsonProperty("middle_name")]
        public string Middle_Name { get; set; }

        [JsonProperty("last_name")]
        public string Last_Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("age")]
        public string Age { get; set; }

        [JsonProperty("gender")]
        public string Sex { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("bio")]
        public string Description { get; set; }

        [JsonProperty("education")]
        public List<EducationDto> Education { get; set; }

        [JsonProperty("experiences")]
        public List<ExperienceDto> Experiences { get; set; }
    }


    public class EducationDto
    {
        [JsonProperty("institution")]
        public string Institution { get; set; }

        [JsonProperty("field_of_study")]
        public string Field_Of_Study { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("start_year")]
        public string Start_Year { get; set; }

        [JsonProperty("end_year")]
        public string End_Year { get; set; }

        [JsonProperty("gpa")]
        public string GPA { get; set; }
    }

    public class ExperienceDto
    {
        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("employment_type")]
        public string Employment_Type { get; set; }

        [JsonProperty("start_date")]
        public DateTime? Start_Date { get; set; }

        [JsonProperty("end_date")]
        public DateTime? End_Date { get; set; }

        [JsonProperty("organization_type")]
        public string OrganizationType { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }
    }
    



}