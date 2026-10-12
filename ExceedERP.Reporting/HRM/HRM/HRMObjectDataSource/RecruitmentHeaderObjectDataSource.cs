using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{

    [DataObject]
    public class RecruitmentHeaderObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalApplicantDetail> GetRecruitmentHeader(string vacancyId)
        {
            List<InternalApplicantDetail> applicantHeaderList = new List<InternalApplicantDetail>();
            InternalApplicantDetail applicantHeader = new InternalApplicantDetail();

            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
            var address =
                  db.Addresses.Where(
                      a =>
                          a.Reference == Organization.MainCompanyId.ToString() &&
                          a.ReferenceType.ToLower() == "company").ToList();
            if (address.Any())
            {
                foreach (var a in address)
                {
                    if (a.Category == AddressType.KIFLEKETEMA)
                    {
                        applicantHeader.Address = "Subcity: " + a.Value + " ";
                    }
                    if (a.Category == AddressType.KEBELE)
                    {
                        applicantHeader.Address += "Kebele: " + a.Value + " ";
                    }
                    if (a.Category == AddressType.CITYPROVINCE)
                    {
                        if (applicantHeader.Address == null)
                        {
                            applicantHeader.Address += a.Value + " -Ethiopia";
                        }
                    }
                    else if (a.Category == AddressType.POBOX)
                    {

                        applicantHeader.POBox = "P.O.Box: " + a.Value;

                    }
                    else if (a.Category == AddressType.OFFICE_EMAIL)
                    {

                        applicantHeader.Email = "Email: " + a.Value;

                    }
                    else if (a.Category == AddressType.WEBSITE)
                    {

                        applicantHeader.WebSite = "Website: " + a.Value;

                    }
                    else if (a.Category == AddressType.FAX)
                    {

                        applicantHeader.Fax = "Fax:  " + a.Value;

                    }
                    else if (a.Category == AddressType.OFFICE_PHONE)
                    {

                        applicantHeader.Tel = "Tel:  " + a.Value;

                    }
                }

            }

            if (vacancyId != null)
            {
                int.TryParse(vacancyId, out int vacId);

                var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == vacId);
                if (vacancy != null)
                {
                    applicantHeader.Quntity = vacancy.NoOfVacant;
                    applicantHeader.From = vacancy.From;
                    applicantHeader.To = vacancy.To;

                    var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == vacancy.JobTitle);
                    if (lookup != null)
                    {
                        applicantHeader.JobTitle = lookup.Description;
                    }

                    var manPowerPlan = db.JobTitleMapping.FirstOrDefault(x => x.BranchId == vacancy.BranchId && x.OrgStructureId == vacancy.Department && x.LookupId == vacancy.JobTitle);
                    if (manPowerPlan != null)
                    {
                        var manPowerDetail = db.JobTitleMappingDetail.FirstOrDefault(x => x.JobTitleMappingId == manPowerPlan.JobTitleMappingId);
                        if (manPowerDetail != null)
                        {
                            applicantHeader.EduandExp = manPowerDetail.RelevantExp + ", " + manPowerDetail.Experience;
                        }
                    }

                    applicantHeader.Salary = vacancy.Salary.ToString();

                    if (applicantHeader.Salary == "0")
                    {
                        //applicantHeader.Salary = "በድርጅቱ ስኬል ";
                        applicantHeader.Salary = "By The company's scale ";
                    }

                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == vacancy.BranchId);
                    if (branch != null)
                    {
                        applicantHeader.Location = branch.Name;
                    }
                    // applicantHeader.RecruitmentType = vacancy.VacancyType;
                    if (vacancy.VacancyType == VacancyType.PERMANENT)
                    {
                        //applicantHeader.RecruitmentType = "ቋሚ";
                        applicantHeader.RecruitmentType = VacancyType.PERMANENT.ToString().ToLower();
                    }
                    else
                    {
                        //applicantHeader.RecruitmentType = "ጊዜያዊ";
                        applicantHeader.RecruitmentType = VacancyType.CONTRACT.ToString().ToLower();

                    }

                    //var rank = db.Ranks.FirstOrDefault(x => x.RankId == vacancy.RankId);
                    //if (rank != null)
                    //{
                    //    applicantHeader.Rank = rank.RankName;
                    //}
                    applicantHeaderList.Add(applicantHeader);
                }

            }
            return applicantHeaderList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

    [DataObject]
    public class RecruitmentInternalHeaderObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalApplicantDetail> GetRecruitmentInternalHeader(string vacancyId)
        {
            List<InternalApplicantDetail> applicantHeaderList = new List<InternalApplicantDetail>();
            InternalApplicantDetail applicantHeader = new InternalApplicantDetail();

            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
            var address =
                  db.Addresses.Where(
                      a =>
                          a.Reference == Organization.MainCompanyId.ToString() &&
                          a.ReferenceType.ToLower() == "company").ToList();
            if (address.Any())
            {
                foreach (var a in address)
                {
                    if (a.Category == AddressType.KIFLEKETEMA)
                    {
                        applicantHeader.Address = "Subcity: " + a.Value + " ";
                    }
                    if (a.Category == AddressType.KEBELE)
                    {
                        applicantHeader.Address += "Kebele: " + a.Value + " ";
                    }
                    if (a.Category == AddressType.CITYPROVINCE)
                    {
                        if (applicantHeader.Address == null)
                        {
                            applicantHeader.Address += a.Value + " -Ethiopia";
                        }
                    }
                    else if (a.Category == AddressType.POBOX)
                    {

                        applicantHeader.POBox = "P.O.Box: " + a.Value;

                    }
                    else if (a.Category == AddressType.OFFICE_EMAIL)
                    {

                        applicantHeader.Email = "Email: " + a.Value;

                    }
                    else if (a.Category == AddressType.WEBSITE)
                    {

                        applicantHeader.WebSite = "Website: " + a.Value;

                    }
                    else if (a.Category == AddressType.FAX)
                    {

                        applicantHeader.Fax = "Fax:  " + a.Value;

                    }
                    else if (a.Category == AddressType.OFFICE_PHONE)
                    {

                        applicantHeader.Tel = "Tel:  " + a.Value;

                    }
                }

            }
            if (vacancyId != null)
            {
                int.TryParse(vacancyId, out int vacId);
                var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == vacId);
                if (vacancy != null)
                {
                    var jobtitle = db.Lookups.FirstOrDefault(x => x.LookupId == vacancy.JobTitle);
                    if (jobtitle != null)
                    {
                        applicantHeader.JobTitle = jobtitle.Description;
                    }

                    applicantHeader.Quntity = vacancy.NoOfVacant;

                    applicantHeader.EduandExp = vacancy.TotalYearOfExperience.ToString();
                    applicantHeader.ExperienceLevel = vacancy.ExperienceLevel.ToString();
                    applicantHeader.Salary = vacancy.Salary.ToString();

                    if (applicantHeader.Salary == "0")
                    {
                        //applicantHeader.Salary = "በድርጅቱ ስኬል";
                        applicantHeader.Salary = "By The company's scale ";

                    }

                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == vacancy.BranchId);
                    if (branch != null)
                    {
                        applicantHeader.Location = branch.Name;
                    }
                    //applicantHeader.RecruitmentType = vacancy.VacancyType;
                    if (vacancy.VacancyType == VacancyType.PERMANENT)
                    {
                       // applicantHeader.RecruitmentType = "ቋሚ";
                        applicantHeader.RecruitmentType = VacancyType.PERMANENT.ToString().ToLower();


                    }
                    else
                    {
                        // applicantHeader.RecruitmentType = "ጊዜያዊ";
                        applicantHeader.RecruitmentType = VacancyType.CONTRACT.ToString().ToLower();

                    }

                    //var rank = db.Ranks.FirstOrDefault(x => x.RankId == vacancy.RankId);
                    //if (rank != null)
                    //{
                    //    applicantHeader.Rank = rank.RankName;
                    //}
                    applicantHeaderList.Add(applicantHeader);
                }
            }

            return applicantHeaderList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }

}







