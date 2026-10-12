using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.HR;
using System.Threading.Tasks;
using ExceedERP.ApplicationServices.HRM;
using ExceedERP.Core.Domain.Global;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class VacancyController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Recruitment_Admin = "HR_Recruitment_Admin";
        private const string HR_Recruitment_User = "HR_Recruitment_User";
        private const string HR_Admin = "HR_Admin";
        public static List<MainCompanyStructure> AllJobCodes { get; set; }
        string ErrorMessage { get; set; }
        public ActionResult Index()
        {
            GetRank();
            GetStep();
            PopulateViewData();
            return View();
        }
        public ActionResult VaccancyPrintOut(int id)
        {
            ViewBag.id = id;
            HRMIsoDocumentHelper.GetIsoDocumentInfo(Modules.HRM, HRMModuleReportType.Vacancy_For_Media, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;

            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            ViewData["jobTitle"] = db.Lookups.Where(l => l.LookupType == LookUpTypes.JOB_TITLE|| l.LookupType == LookUpTypes.OLD_JOB_TITLE)
                .Select(x=>new { x.LookupId, JobTitle=x.LookupId,x.Description}).ToList();
            ViewData["EducationLevel"] = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);

            ViewData["EducationField"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.MEDIA);

            ViewData["EduLevelList"] = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);

            ViewData["Media"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.MEDIA);
            GetOrganizations();
            var branches = db.Branch;
            ViewData["branches"] = branches;
            var vacancies = db.VaccancyPlans.ToList();
            ViewData["Vacancy"] = vacancies;
            ViewData["Employee"] = db.Person_Employee.ToList();
            var recruitmentPlans = db.RecruitmentPlans.ToList();

            ViewData["workunit"] = recruitmentPlans;


            var branchCostCenter = db.BranchCostCenter.ToList();

            foreach (var b in branchCostCenter)
            {
                var oneWorkUnit = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == b.OrgStructureId);
                if (oneWorkUnit != null)
                {
                    workUnits.Add(oneWorkUnit);
                }

            }
            ViewData["workunits"] = workUnits;
            ViewData["mainCompanyStructure"] = db.MainCompanyStructures.ToList();
            var branch = db.Branch.ToList();
            ViewData["Branches"] = branch;

            var recruits = db.RecruitmentPlans.ToList();
            List<RecruitPlan> planLists = new List<RecruitPlan>();
            foreach (var rr in recruits)
            {
                var plan = new RecruitPlan();

                plan.RecruitmentPlanId = rr.RecruitmentPlanId;
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == rr.LookupId);
                if (lookup != null)
                {
                    plan.JobTitle = lookup.Description;
                }

                planLists.Add(plan);
            }


            ViewData["Recruitments"] = planLists;
            ViewData["level"] = db.Lookups
             .Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL)
             .Select(x => new
             {
                 EducationLevelId = x.LookupId,
                 x.LookupId,
                 x.Description
             }).ToList();
        }
        public JsonResult GetRank()
        {
            db = new ExceedDbContext();
            var ranks = db.Ranks.AsQueryable();
            ViewData["RankList"] = ranks;
            return Json(ranks.Select(p => new { RankId = p.RankId, RankName = p.RankName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStep()
        {
            db = new ExceedDbContext();
            var stepss = db.Steps.AsQueryable();
            ViewData["StepList"] = stepss;
            return Json(stepss.Select(p => new { StepId = p.StepId, StepName = p.StepName }), JsonRequestBehavior.AllowGet);
        }
        private bool CheckVacancyValidation(string id)
        {
            ErrorMessage = "";


            #region ApplicantRegistration
            var ApplicantRegistration = db.ApplicantRegistration.Where(x => x.VacancyId.ToString() == id).FirstOrDefault();
            if (ApplicantRegistration != null)
            {

                ErrorMessage = "You cannot delete this Vacancy .Please delete Applicant Registration information first.";
                return false;


            }
            #endregion
            #region Recruitment Vacancy
            var vacancy = db.RecruitmentVacancy.Where(x => x.VacancyId.ToString() == id).FirstOrDefault();
            if (vacancy != null)
            {

                ErrorMessage = "You cannot delete this Vacancy .Please delete recruitment vacancy information first.";
                return false;


            }
            #endregion
            return true;
        }
      
        public JsonResult GetOrganizations()
        {

            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;
            return Json(structres.Select(s => new { OrgStructureId = s.OrgStructureId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
        public string GetBranch(int id)
        {
            var branch = db.Branch.FirstOrDefault(x => x.BranchId == id);
            if (branch != null)
            {
                return branch.Name;
            }
            return null;
        }

        public string GetJobtitle(int id)
        {
            var look = db.Lookups.FirstOrDefault(x => x.LookupId == id);
            if (look != null)
            {
                return look.Description;
            }
            return null;
        }
        public bool GetVancyStatus(int id)
        {
            var vacancy = db.VaccancyPlans.Find(id);
            if (vacancy != null)
            {

                if (vacancy.To<DateTime.Now)
                {
                    vacancy.IsActive = false;
                    vacancy.ModifiedBy = User.Identity.Name;
                    vacancy.LastModified = DateTime.Now;
                }
                else
                {
                    vacancy.IsActive = true;
                    vacancy.ModifiedBy = User.Identity.Name;
                    vacancy.LastModified = DateTime.Now;
                }
                db.Entry(vacancy).State = EntityState.Modified;
                db.SaveChanges();
                return vacancy.IsActive;
            }
            return false;

        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult VaccancyPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<Vacancy> vaccancyplans = db.VaccancyPlans.OrderByDescending(x => x.VacancyId);//.Where(x => mybraches.Contains(x.OrgBranchName));

            DataSourceResult result = vaccancyplans.ToList().ToDataSourceResult(request, vaccancyPlan => new
            {
                VacancyId = vaccancyPlan.VacancyId,
                StepId = vaccancyPlan.StepId,
                RankId = vaccancyPlan.RankId,
                VaccancyCode = vaccancyPlan.VaccancyCode,
                From = vaccancyPlan.From,
                To = vaccancyPlan.To,
                BranchId = vaccancyPlan.BranchId,
                Isposted = vaccancyPlan.Isposted,
                IsActive = GetVancyStatus(vaccancyPlan.VacancyId),
                Remark = vaccancyPlan.Remark,
                RecruSource = vaccancyPlan.RecruSource,
                DateCreated = vaccancyPlan.DateCreated,
                LastModified = vaccancyPlan.LastModified,
                CreatedBy = vaccancyPlan.CreatedBy,
                ModifiedBy = vaccancyPlan.ModifiedBy,
                FromVacAm = vaccancyPlan.FromVacAm,
                ToVacAm = vaccancyPlan.ToVacAm,
                VacancyType = vaccancyPlan.VacancyType,
                OrgBranchName = vaccancyPlan.OrgBranchName,
                RecruitmentPlanId = vaccancyPlan.RecruitmentPlanId,
                JobTitle = vaccancyPlan.JobTitle,
                Department = vaccancyPlan.Department,
                NoOfVacant = vaccancyPlan.NoOfVacant,
                Salary = vaccancyPlan.Salary,
                InterviewSchedule = vaccancyPlan.InterviewSchedule,
                WrittenExamSchedule = vaccancyPlan.WrittenExamSchedule,
                Location = vaccancyPlan.Location,
                ExperienceLevel = vaccancyPlan.ExperienceLevel,
                JobRequirement = vaccancyPlan.JobRequirement,
                Responsibility = vaccancyPlan.Responsibility,
                TotalYearOfExperience = vaccancyPlan.TotalYearOfExperience,
                Branch = GetBranch(vaccancyPlan.BranchId),
                JobtitleName = GetJobtitle(vaccancyPlan.JobTitle),
                EthioJobsCode = vaccancyPlan.EthioJobsCode,
                ReportsTo = vaccancyPlan.ReportsTo,
                Gender = vaccancyPlan.Gender


            });

            return Json(result);
        }

        public string GenerateVacancyCode(Vacancy vaccancyPlan)
        {
            string id = "";
            int nextID;

            var alllist = db.VaccancyPlans.ToList();

            var ExternalOrInternal = "";

            if (vaccancyPlan.RecruSource == RecruitmentSource.EXTERNAL)
            {
                ExternalOrInternal = "Ext";
            }
            else if (vaccancyPlan.RecruSource == RecruitmentSource.INTERNAL)
            {
                ExternalOrInternal = "Int";
            }

            if (vaccancyPlan.VacancyType == VacancyType.CONTRACT)
            {
                if (alllist.Any())
                {
                    var cont = alllist.Where(x => x.VacancyType == VacancyType.CONTRACT).OrderByDescending(x => x.VacancyId).FirstOrDefault();

                    if (cont != null)
                    {
                        if (cont.VaccancyCode.Contains('-'))
                        {
                            nextID = Convert.ToInt32(cont.VaccancyCode.Split('-')[2]) + 1;

                            id = ExternalOrInternal + "-CO-" + nextID.ToString() + "-" + DateTime.Now.Year.ToString();
                        }
                        else
                        {
                            // id = ExternalOrInternal + "-CO-" + nextID.ToString() + "-" + DateTime.Now.Year.ToString();
                        }
                    }
                    else
                    {
                        id = ExternalOrInternal + "-CO-" + "1" + "-" + DateTime.Now.Year.ToString();
                    }
                }
                else
                {
                    id = ExternalOrInternal + "-CO-" + "1" + "-" + DateTime.Now.Year.ToString();
                }

            }
            else
            {
                if (alllist.Any())
                {
                    var cont = alllist.Where(x => x.VacancyType == VacancyType.PERMANENT).OrderByDescending(x => x.VacancyId).FirstOrDefault();

                    if (cont != null)
                    {
                        if (cont.VaccancyCode.Contains('-'))
                        {
                            nextID = Convert.ToInt32(cont.VaccancyCode.Split('-')[2]) + 1;
                            id = ExternalOrInternal + "-PE-" + nextID.ToString() + "-" + DateTime.Now.Year.ToString();
                        }
                        else
                        {
                            // id = ExternalOrInternal + "-CO-" + nextID.ToString() + "-" + DateTime.Now.Year.ToString();
                        }

                    }
                    else
                    {
                        id = ExternalOrInternal + "-PE-" + "1" + "-" + DateTime.Now.Year.ToString();
                    }
                }
                else
                {
                    if (vaccancyPlan.VacancyType == VacancyType.CONTRACT)
                    {
                        id = ExternalOrInternal + "-CO-" + "1" + "-" + DateTime.Now.Year.ToString();
                    }
                    else
                    {
                        id = ExternalOrInternal + "-PE-" + "1" + "-" + DateTime.Now.Year.ToString();
                    }
                }
            }

            return id;
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult VaccancyPlans_Create([DataSourceRequest]DataSourceRequest request, Vacancy vaccancyPlan)
        {
            ModelState.Clear();

            #region Id Generate

            string id = HrHelper.GenerateVacancyCode(vaccancyPlan);

            #endregion


            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                if (string.IsNullOrEmpty(vaccancyPlan.FromVacAm)
                || string.IsNullOrEmpty(vaccancyPlan.ToVacAm))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory");
                    return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
                }
                vaccancyPlan.From = HrHelper.ParseEthiopicDate(vaccancyPlan.FromVacAm);
                vaccancyPlan.To = HrHelper.ParseEthiopicDate(vaccancyPlan.ToVacAm);
            }
            else
            {
                vaccancyPlan.FromVacAm = XAPI.EthiopicDateTime.GetEthiopicDate(vaccancyPlan.From.Day, vaccancyPlan.From.Month, vaccancyPlan.From.Year);
                vaccancyPlan.ToVacAm = XAPI.EthiopicDateTime.GetEthiopicDate(vaccancyPlan.To.Day, vaccancyPlan.To.Month, vaccancyPlan.To.Year);
            }
            var vacCheck = db.VaccancyPlans.FirstOrDefault(x => x.BranchId == vaccancyPlan.BranchId && x.To >= vaccancyPlan.From && x.IsActive && x.Department == vaccancyPlan.Department && x.JobTitle == vaccancyPlan.JobTitle);
            if (vacCheck != null)
            {
                ModelState.AddModelError("", "This vacancy is Posted Before If You Need To Post Again You should close Previos Post First");
                return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));

            }
            if (vaccancyPlan.From > vaccancyPlan.To)
            {
                ModelState.AddModelError("", "Please Check the date range, Start date must be less or equal to End date!!");
                return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));

            }

            var checkExistance = db.VaccancyPlans.FirstOrDefault(x => x.VaccancyCode == id && x.RecruSource == vaccancyPlan.RecruSource);

            if (checkExistance != null)
            {
                id = checkExistance.VaccancyCode + "" + DateTime.Now.Day.ToString();
            }

            if (ModelState.IsValid)
            {
                var entity = new Vacancy
                {
                    VaccancyCode = id,
                    StepId = vaccancyPlan.StepId,
                    RankId = vaccancyPlan.RankId,
                    RecruitmentPlanId = vaccancyPlan.RecruitmentPlanId,
                    From = vaccancyPlan.From,
                    BranchId = vaccancyPlan.BranchId,
                    To = vaccancyPlan.To,
                    IsActive = true,
                    //  IsComplete = false,
                    RecruSource = vaccancyPlan.RecruSource,
                    Remark = vaccancyPlan.Remark,
                    ReportsTo = vaccancyPlan.ReportsTo,
                    DateCreated = DateTime.Now,
                    LastModified = vaccancyPlan.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = vaccancyPlan.ModifiedBy,
                    FromVacAm = vaccancyPlan.FromVacAm,
                    ToVacAm = vaccancyPlan.ToVacAm,
                    VacancyType = vaccancyPlan.VacancyType,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    JobTitle = vaccancyPlan.JobTitle,
                    Department = vaccancyPlan.Department,
                    NoOfVacant = vaccancyPlan.NoOfVacant,
                    Salary = vaccancyPlan.Salary,
                    InterviewSchedule = vaccancyPlan.InterviewSchedule,
                    WrittenExamSchedule = vaccancyPlan.WrittenExamSchedule,
                    Location = vaccancyPlan.Location,
                    ExperienceLevel = vaccancyPlan.ExperienceLevel,
                    JobRequirement = vaccancyPlan.JobRequirement,
                    Responsibility = vaccancyPlan.Responsibility,
                    TotalYearOfExperience = vaccancyPlan.TotalYearOfExperience,
                    EthioJobsCode = vaccancyPlan.EthioJobsCode,
                    //Language = vaccancyPlan.Language,
                    // CurrentStatusTracking = vaccancyPlan.CurrentStatusTracking,
                    Gender = vaccancyPlan.Gender

                };

                db.VaccancyPlans.Add(entity);
                db.SaveChanges();
                vaccancyPlan.VacancyId = entity.VacancyId;

             
            }


            return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VaccancyPlans_Update([DataSourceRequest]DataSourceRequest request, Vacancy vaccancyPlan)
        {
            ModelState.Clear();


            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                if (string.IsNullOrEmpty(vaccancyPlan.FromVacAm)
                || string.IsNullOrEmpty(vaccancyPlan.ToVacAm))
                {
                    ModelState.AddModelError("", "Both From and To Date fields are mandatory");
                    return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
                }
                vaccancyPlan.From = HrHelper.ParseEthiopicDate(vaccancyPlan.FromVacAm);
                vaccancyPlan.To = HrHelper.ParseEthiopicDate(vaccancyPlan.ToVacAm);
            }
            else
            {
                vaccancyPlan.FromVacAm = XAPI.EthiopicDateTime.GetEthiopicDate(vaccancyPlan.From.Day, vaccancyPlan.From.Month, vaccancyPlan.From.Year);
                vaccancyPlan.ToVacAm = XAPI.EthiopicDateTime.GetEthiopicDate(vaccancyPlan.To.Day, vaccancyPlan.To.Month, vaccancyPlan.To.Year);
            }

            if (vaccancyPlan.From > vaccancyPlan.To)
            {
                ModelState.AddModelError("", "Please Check the date range, From must be less or equal to To!!");
                return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                if (vaccancyPlan.VaccancyCode == "" || vaccancyPlan.VaccancyCode == null)
                {
                    vaccancyPlan.VaccancyCode = HrHelper.GenerateVacancyCode(vaccancyPlan);
                }

                var entity = new Vacancy
                {
                    VacancyId = vaccancyPlan.VacancyId,
                    StepId = vaccancyPlan.StepId,
                    RankId = vaccancyPlan.RankId,
                    VaccancyCode = vaccancyPlan.VaccancyCode,
                    RecruitmentPlanId = vaccancyPlan.RecruitmentPlanId,
                    From = vaccancyPlan.From,
                    BranchId = vaccancyPlan.BranchId,
                    To = vaccancyPlan.To,
                    IsActive = vaccancyPlan.IsActive,
                    // IsComplete = vaccancyPlan.IsComplete,
                    Remark = vaccancyPlan.Remark,
                    RecruSource = vaccancyPlan.RecruSource,
                    DateCreated = vaccancyPlan.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = vaccancyPlan.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    FromVacAm = vaccancyPlan.FromVacAm,
                    ToVacAm = vaccancyPlan.ToVacAm,
                    VacancyType = vaccancyPlan.VacancyType,
                    OrgBranchName = vaccancyPlan.OrgBranchName,
                    ReportsTo = vaccancyPlan.ReportsTo,
                    JobTitle = vaccancyPlan.JobTitle,
                    Department = vaccancyPlan.Department,
                    NoOfVacant = vaccancyPlan.NoOfVacant,
                    Salary = vaccancyPlan.Salary,
                    InterviewSchedule = vaccancyPlan.InterviewSchedule,
                    WrittenExamSchedule = vaccancyPlan.WrittenExamSchedule,
                    Location = vaccancyPlan.Location,
                    ExperienceLevel = vaccancyPlan.ExperienceLevel,
                    JobRequirement = vaccancyPlan.JobRequirement,
                    Responsibility = vaccancyPlan.Responsibility,
                    TotalYearOfExperience = vaccancyPlan.TotalYearOfExperience,
                    EthioJobsCode = vaccancyPlan.EthioJobsCode,
                    Isposted = vaccancyPlan.Isposted,
                    Gender = vaccancyPlan.Gender
                };

                db.VaccancyPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VaccancyPlans_Destroy([DataSourceRequest]DataSourceRequest request, Vacancy vaccancyPlan)
        {
            ModelState.Clear();
            var checkVac = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == vaccancyPlan.VacancyId && !x.IsActive);
            if (checkVac != null)
            {
                ModelState.AddModelError("", "you can't delete closed vacancy");
            }
            if (ModelState.IsValid)
            {
                var entity = new Vacancy
                {
                    VacancyId = vaccancyPlan.VacancyId,
                    VaccancyCode = vaccancyPlan.VaccancyCode,
                    RecruitmentPlanId = vaccancyPlan.RecruitmentPlanId,
                    From = vaccancyPlan.From,
                    BranchId = vaccancyPlan.BranchId,
                    ReportsTo = vaccancyPlan.ReportsTo,
                    To = vaccancyPlan.To,
                    IsActive = vaccancyPlan.IsActive,
                    Remark = vaccancyPlan.Remark,
                    RecruSource = vaccancyPlan.RecruSource,
                    DateCreated = vaccancyPlan.DateCreated,
                    LastModified = vaccancyPlan.LastModified,
                    CreatedBy = vaccancyPlan.CreatedBy,
                    ModifiedBy = vaccancyPlan.ModifiedBy,
                    FromVacAm = vaccancyPlan.FromVacAm,
                    ToVacAm = vaccancyPlan.ToVacAm,
                    VacancyType = vaccancyPlan.VacancyType,
                    OrgBranchName = vaccancyPlan.OrgBranchName,
                    JobTitle = vaccancyPlan.JobTitle,
                    Department = vaccancyPlan.Department,
                    NoOfVacant = vaccancyPlan.NoOfVacant,
                    Salary = vaccancyPlan.Salary,
                    InterviewSchedule = vaccancyPlan.InterviewSchedule,
                    WrittenExamSchedule = vaccancyPlan.WrittenExamSchedule,
                    Location = vaccancyPlan.Location,
                    ExperienceLevel = vaccancyPlan.ExperienceLevel,
                    RankId = vaccancyPlan.RankId,
                    StepId = vaccancyPlan.StepId,
                    JobRequirement = vaccancyPlan.JobRequirement,
                    Responsibility = vaccancyPlan.Responsibility,
                    TotalYearOfExperience = vaccancyPlan.TotalYearOfExperience,
                    EthioJobsCode = vaccancyPlan.EthioJobsCode,
                    Gender = vaccancyPlan.Gender,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted vacancy of vacancy code " + vaccancyPlan.VaccancyCode);
                #endregion
                db.VaccancyPlans.Attach(entity);
                db.VaccancyPlans.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public string Close(int id)
        {
            var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == id);

            if (vacancy == null)
                return "Vacancy not found";

            if (!vacancy.IsActive)
                return "Already closed before";
            if (vacancy.To.Date > DateTime.Now.Date)
                return $"You can't close before: {vacancy.To:MMMM dd, yyyy}"; 
           
            vacancy.IsActive = false;
            db.Entry(vacancy).State = EntityState.Modified;
            db.SaveChanges();

            return "Successfully closed";
        }

        public string ForceOpen(int id)
        {
            var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == id);

            if (vacancy == null)
                return "Vacancy not found";

            if (vacancy.IsActive)
                return "Already opened before";

            var tomorrow = DateTime.Now.AddDays(1);

            vacancy.IsActive = true;
            vacancy.To = tomorrow;
            vacancy.ToVacAm = XAPI.EthiopicDateTime.GetEthiopicDate(
                tomorrow.Day, tomorrow.Month, tomorrow.Year
            );

            db.Entry(vacancy).State = EntityState.Modified;
            db.SaveChanges();

            return "Successfully opened";
        }
        public ActionResult GetRecruitmentSource()
        {
            var sources = Enum.GetValues(typeof(RecruitmentSource))
                       .Cast<RecruitmentSource>()
                       .Select(d => (d))
                       .ToList();

            return Json(sources.Select(p => new
            {
                Name = GetRecruitmentSourceDisplayName(p),
                Value = (int)p

            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetExperienceLevel()
        {
            var levels = Enum.GetValues(typeof(ExperienceLevel))
                       .Cast<ExperienceLevel>()
                       .Select(d => (d))
                       .ToList();

            return Json(levels.Select(p => new
            {
                Name = GetEmployeementLevelDisplayName(p),
                Value = (int)p

            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetVacancyType()
        {
            var levels = Enum.GetValues(typeof(VacancyType))
                .Cast<VacancyType>()
                .ToList();

            return Json(levels.Select(p => new
            {
                Name = GetDisplayName(p),

                Value = (int)p
            }), JsonRequestBehavior.AllowGet);
        }
        private string GetDisplayName(VacancyType vacancyType)
        {
            FieldInfo fieldInfo = vacancyType.GetType().GetField(vacancyType.ToString());
            if (fieldInfo != null)
            {
                DisplayAttribute displayAttribute = fieldInfo
                    .GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    return displayAttribute.GetName();
                }
            }

            return vacancyType.ToString(); // Fallback to raw enum name
        }
        private string GetEmployeementLevelDisplayName(ExperienceLevel experienceLevel)
        {
            FieldInfo fieldInfo = experienceLevel.GetType().GetField(experienceLevel.ToString());
            if (fieldInfo != null)
            {
                DisplayAttribute displayAttribute = fieldInfo
                    .GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    return displayAttribute.GetName();
                }
            }

            return experienceLevel.ToString(); // Fallback to raw enum name
        }
        private string GetRecruitmentSourceDisplayName(RecruitmentSource recruitmentSource)
        {
            FieldInfo fieldInfo = recruitmentSource.GetType().GetField(recruitmentSource.ToString());
            if (fieldInfo != null)
            {
                DisplayAttribute displayAttribute = fieldInfo
                    .GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    return displayAttribute.GetName();
                }
            }

            return recruitmentSource.ToString(); // Fallback to raw enum name
        }
        public ActionResult GetLanguages()
        {
            var levels = Enum.GetValues(typeof(Languages))
                       .Cast<Languages>()
                       .Select(d => (d))
                       .ToList();

            return Json(levels.Select(p => new
            {
                Name = p.ToString(),
                Value = (int)p

            }), JsonRequestBehavior.AllowGet);
        }
        public decimal GetSalaryAmount(int rank, int step)
        {
            var salary = db.SalariesJobTitles.FirstOrDefault(s => s.RankId == rank && s.StepId == step);

            if (salary != null)
            {
                return salary.Salary; //Json(salary.SalaryAmount, JsonRequestBehavior.AllowGet);
            }

            return 0; //Json(0, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetExperienceAmount(int lookUpId)
        {
            var amount = db.EducationLevelDetails
                .Where(x => x.ExperienceJob.LookupId == lookUpId)
                .Min(x => (decimal?)x.Experience) ?? 0;

            return Json(amount, JsonRequestBehavior.AllowGet);
        }
        
        public async Task<ActionResult> PostVacancyDetail(int id)
        {
            // Try detail + master Vacancy
            var detail = db.VacancyDetails
                           .Include(v => v.Vacancy)
                           .FirstOrDefault(x => x.VacancyId == id);

            VacancyRequest request;

            if (detail != null)
            {
                // Map from detail + master
                request = MapToVacancyRequest(detail);
            }
            else
            {
                // No detail → fallback to just master Vacancy
                var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == id);
                if (vacancy == null)
                    return HttpNotFound();
                var lookup = db.Lookups.Find(vacancy.JobTitle);
                request = new VacancyRequest
                {
                    title = lookup?.Description??"Job Vaccancy",
                    description = vacancy.Remark ?? "N/A",
                    published_date = vacancy.From.ToString("yyyy-MM-dd"),
                    deadline = vacancy.To.ToString("yyyy-MM-dd"),
                    employment_type = vacancy.VacancyType.ToString(),
                    location = "A.A",
                    salary = vacancy.Salary?.ToString(),
                    experience = vacancy.TotalYearOfExperience.ToString() ?? "0 years",
                    available_positions = vacancy.NoOfVacant.ToString(),
                    job_code = vacancy.VaccancyCode,
                    qualification = vacancy.JobtitleName ?? "Computer Science",
                    gpa = vacancy.Remark ?? ">3.0",
                    education_level = vacancy.Responsibility ?? "Bachelor's Degree",
                    grade = vacancy.RankId.ToString(),
                    skills = string.IsNullOrEmpty(vacancy.JobRequirement)? new List<string>()
                                                                     : vacancy.JobRequirement.Split(',')
                                                                               .Select(s => s.Trim())
                                                                               .ToList()
                };
            }

            var apiService = new VacancyApiService();

            try
            {
                var response = await apiService.PostVacancyAsync(request);
                var vacancy = db.VaccancyPlans.Find(id);
                if(vacancy != null)
                {
                    vacancy.Isposted = true;
                    db.Entry(vacancy).State = EntityState.Modified;
                    db.SaveChanges();
                }
                // Parse response if needed
                return Json(new { status = "success", response }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = "error", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        private VacancyRequest MapToVacancyRequest(VacancyDetail detail)
        {
            return new VacancyRequest
            {
                title = detail.Vacancy?.JobtitleName??"Job Vavancy",
                description = detail.Decription??"Job Vacancy",
                published_date = detail.Vacancy?.From.ToString("yyyy-MM-dd"),
                deadline = detail.Vacancy?.To.ToString("yyyy-MM-dd"),
                employment_type = detail.Vacancy?.VacancyType.ToString(),
                location = detail.Vacancy?.BranchName??"A.A",
                salary = detail.Vacancy?.Salary.ToString(),
                experience = $"{detail.Experience}+ years",
                available_positions = detail.Vacancy?.NoOfVacant.ToString(),
                job_code = detail.Vacancy?.VaccancyCode,
                qualification = detail.LookupId.ToString(),
                gpa = detail.Vacancy?.Remark??">3.0",
                education_level = detail.LookupId.ToString(),
                grade = detail.Vacancy?.RankId.ToString(),
                skills = new List<string>() // map if you have skills field
            };
        }

        public ActionResult SummaryReport()
        {

            return View();

        }

        public ActionResult SummaryReportCourt()
        {

            return View();

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class RecruitPlan
    {
        public int RecruitmentPlanId { get; set; }
        public string JobTitle { get; set; }
    }
    
}
