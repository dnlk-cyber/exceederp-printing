using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{

    [DataObject]
    public class VacancyObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public VacancyViewModel GetVacancy(string vacancyId)
        {
            int.TryParse(vacancyId, out int id);
            var vacancy = db.VaccancyPlans.Find(id);
            if (vacancy == null)
                return new VacancyViewModel { Details = new List<VacancyDetailViewModel>() };

            var structure = db.MainCompanyStructures.ToDictionary(x => x.OrgStructureId);
            var lookups = db.Lookups.ToDictionary(x => x.LookupId);
            var branch = db.Branch.ToDictionary(x => x.BranchId);
            var mediavacancyCode = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == id)?.MediaVacancyCode;

            branch.TryGetValue(vacancy.BranchId, out var branchdic);
            structure.TryGetValue(vacancy?.Department??0, out var structuredic);
            lookups.TryGetValue(vacancy.JobTitle, out var lookupdic);

            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            var decodedJobRequirement = HttpUtility.HtmlDecode(vacancy.JobRequirement);
            var decodedResponsibility = HttpUtility.HtmlDecode(vacancy.Responsibility);

            var vacancyItem = new VacancyViewModel
            {
                BranchName = branchdic?.Name ?? branchdic?.AmharicName,
                DepartmentName = structuredic?.Name,
                JobtitleName = lookupdic?.Description ?? lookupdic?.DescriptionEng,
                VaccancyCode = vacancy.VaccancyCode,
                VacancyMediaCode = mediavacancyCode,
                From = datePreference ? vacancy.FromVacAm : vacancy.From.ToString("dd.MM.yyyy"),
                To = datePreference ? vacancy.ToVacAm : vacancy.To.ToString("dd.MM.yyyy"),
                TotalYearOfExperience = vacancy.TotalYearOfExperience,
                VacancyType = vacancy.VacancyType.ToString(),
                RecruSource = vacancy.RecruSource.ToString(),
                ExperienceLevel = vacancy.ExperienceLevel.ToString(),
                EthioJobsCode = vacancy.EthioJobsCode,
                LocationName = vacancy.Location,
                Salary = vacancy.Salary,
                NoOfVacant = vacancy.NoOfVacant,
                JobRequirement = decodedJobRequirement,
                Responsibility = decodedResponsibility,
                CGPA = vacancy.CGPA,
                IsActive = vacancy.IsActive,
                Isposted = vacancy.Isposted,
                Details = new List<VacancyDetailViewModel>()
            };

            //var vacancyDetails = db.VacancyDetails.Where(x => x.VacancyId == vacancy.VacancyId);
            //foreach (var detail in vacancyDetails)
            //{
            //    lookups.TryGetValue(detail.LookupId, out var jobTitleDic);

            //    vacancyItem.Details.Add(new VacancyDetailViewModel
            //    {
            //        JobTitle = jobTitleDic?.Description,
            //        Experience = detail.Experience,
            //        Description = detail.Decription
            //    });
            //}

            return vacancyItem;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
