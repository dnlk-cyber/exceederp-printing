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
    public class RecruitmentHeaderObjectDataSource : List<InternalVacancy>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalApplicantDetail> GetRecruitmentHeader(string jobTitle, string vacancyCod, string vacancyId, DateTime from, DateTime to)
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

            if (vacancyCod != null && vacancyId != null)
            {

                var recruitmentVacancy = db.RecruitmentVacancy.FirstOrDefault(x => x.MediaVacancyCode == vacancyCod && x.VacancyId.ToString() == vacancyId);
                if (recruitmentVacancy != null)
                {

                    var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitmentVacancy.VacancyId);
                    if (vacancy != null)
                    {
                        var recruitmentPlan = db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == vacancy.RecruitmentPlanId);
                        if (recruitmentPlan != null)
                        {
                            var recruitmentRequest = db.RecruitmentRequest.FirstOrDefault(x=>x.BranchId == recruitmentPlan.BranchId && 
                                                  x.OrgStructureId == recruitmentPlan.OrgStructureId && x.LookupId == recruitmentPlan.LookupId 
                                                  && x.GlFiscalYearId == recruitmentPlan.GlFiscalYearId);
                            if(recruitmentRequest != null)
                            {
                                applicantHeader.Quntity = recruitmentRequest.NoOfVaccantPostion;
                            }
                            applicantHeader.JobTitle = recruitmentPlan.JobtitleName;
                           
                            applicantHeader.EduandExp = recruitmentPlan.RelevantExp + ", " + recruitmentPlan.Experience;
                            applicantHeader.Salary = recruitmentPlan.PlannedSalary;
                            applicantHeader.Location = vacancy.Branch.Name;
                           // applicantHeader.RecruitmentType = vacancy.VacancyType;
                            if (vacancy.VacancyType == VacancyType.PERMANENT)
                            {
                                applicantHeader.RecruitmentType = "ቋሚ";
                            }
                            else
                            {
                                applicantHeader.RecruitmentType = "ጊዜያዊ";
                            }

                            var rank = db.Ranks.FirstOrDefault(x => x.RankId == recruitmentPlan.RankId);
                            if (rank != null)
                            {
                                applicantHeader.Rank = rank.RankName;
                            }
                            applicantHeaderList.Add(applicantHeader);
                        }
                    }

                }
            }
            return applicantHeaderList;
        }
    }

    [DataObject]
    public class RecruitmentInternalHeaderObjectDataSource : List<InternalVacancy>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalApplicantDetail> GetRecruitmentInternalHeader(string jobTitle, string vacancyCod, string vacancyId, DateTime from, DateTime to)
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
            if (vacancyCod != null && vacancyId != null)
            {

                var recruitmentVacancy = db.RecruitmentVacancy.FirstOrDefault(x => x.MediaVacancyCode == vacancyCod && x.VacancyId.ToString() == vacancyId);
                if (recruitmentVacancy != null)
                {

                    var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitmentVacancy.VacancyId);
                    if (vacancy != null)
                    {
                        var recruitmentPlan = db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == vacancy.RecruitmentPlanId);
                        if (recruitmentPlan != null)
                        {
                            applicantHeader.JobTitle = recruitmentPlan.JobtitleName;
                            applicantHeader.Quntity = recruitmentPlan.NoOfVacantPost;
                            applicantHeader.EduandExp = recruitmentPlan.RelevantExp + ", " + recruitmentPlan.Experience;
                            applicantHeader.Salary = recruitmentPlan.PlannedSalary;
                            applicantHeader.Location = vacancy.Branch.Name;
                            //applicantHeader.RecruitmentType = vacancy.VacancyType;
                            if (vacancy.VacancyType == VacancyType.PERMANENT)
                            {
                                applicantHeader.RecruitmentType = "ቋሚ";
                            }
                            else
                            {
                                applicantHeader.RecruitmentType = "ጊዜያዊ";
                            }

                            var rank = db.Ranks.FirstOrDefault(x => x.RankId == recruitmentPlan.RankId);
                            if (rank != null)
                            {
                                applicantHeader.Rank = rank.RankName;
                            }
                            applicantHeaderList.Add(applicantHeader);
                        }
                    }

                }
            }
            return applicantHeaderList;
        }
    }

}







