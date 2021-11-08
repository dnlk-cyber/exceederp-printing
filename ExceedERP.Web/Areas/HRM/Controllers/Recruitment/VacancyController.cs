using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
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

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    //[Authorize(Roles = HR_Recruitment_Admin + "," + HR_Recruitment_User + "," + HR_Admin)]
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
    public class VacancyController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Recruitment_Admin = "HR_Recruitment_Admin";
        private const string HR_Recruitment_User = "HR_Recruitment_User";
        private const string HR_Admin = "HR_Admin";
        public static List<MainCompanyStructure> AllJobCodes { get; set; }
        public static int JobCode { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public static DateTime StartDate { get; set; }
        public static DateTime EndDate { get; set; }
        public static string AmharicStartDate { get; set; }
        public static string AmharicEndDate { get; set; }
        string ErrorMessage { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

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
        public void SetDate(string name, string value)
        {
            if (name == "FromVacAm")
            {
                AmharicStartDate = value;
                var splitDate = AmharicStartDate.Split('/');
                StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToVacAm")
            {
                AmharicEndDate = value;
                var splitDate = AmharicEndDate.Split('/');
                EndDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
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

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult VaccancyPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<Vacancy> vaccancyplans = db.VaccancyPlans.OrderByDescending(x => x.VacancyId);//.Where(x => mybraches.Contains(x.OrgBranchName));

            DataSourceResult result = vaccancyplans.ToList().ToDataSourceResult(request, vaccancyPlan => new
            {
                VacancyId = vaccancyPlan.VacancyId,
                VaccancyCode = vaccancyPlan.VaccancyCode,
                From = vaccancyPlan.From,
                To = vaccancyPlan.To,
                BranchId = vaccancyPlan.BranchId,
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
                Language = vaccancyPlan.Language,
                // CurrentStatusTracking = vaccancyPlan.CurrentStatusTracking,
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


            if (vaccancyPlan.FromVacAm != null)
            {
                var splitDateFrom = vaccancyPlan.FromVacAm.Split('/');
                vaccancyPlan.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateFrom[0]), Convert.ToInt32(splitDateFrom[1]), Convert.ToInt32(splitDateFrom[2]));
            }
            else
            {
                ModelState.AddModelError("", "Start date is required");
            }
            if (vaccancyPlan.ToVacAm != null)
            {
                var splitDateTo = vaccancyPlan.ToVacAm.Split('/');
                vaccancyPlan.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));
            }
            else
            {
                ModelState.AddModelError("", "End date is required");
            }

            var vacCheck = db.VaccancyPlans.FirstOrDefault(x => x.BranchId == vaccancyPlan.BranchId && x.To >= vaccancyPlan.From && x.IsActive && x.Department == vaccancyPlan.Department && x.JobTitle == vaccancyPlan.JobTitle);
            if (vacCheck != null)
            {
                ModelState.AddModelError("", "This vacancy is Posted Before If You Need To Post Again You should close Previos Post First");
            }
            if (vaccancyPlan.From > vaccancyPlan.To)
            {
                ModelState.AddModelError("", "Please Check the date range, Start date must be less or equal to End date!!");
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
                    RecruitmentPlanId = vaccancyPlan.RecruitmentPlanId,
                    From = vaccancyPlan.From,
                    BranchId = vaccancyPlan.BranchId,
                    To = vaccancyPlan.To,
                    IsActive = true,
                    //  IsComplete = false,
                    RecruSource = vaccancyPlan.RecruSource,
                    Remark = vaccancyPlan.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = vaccancyPlan.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = vaccancyPlan.ModifiedBy,
                    FromVacAm = AmharicStartDate,
                    ToVacAm = AmharicEndDate,
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
                    Language = vaccancyPlan.Language,
                    // CurrentStatusTracking = vaccancyPlan.CurrentStatusTracking,
                    Gender = vaccancyPlan.Gender

                };

                db.VaccancyPlans.Add(entity);
                db.SaveChanges();
                vaccancyPlan.VacancyId = entity.VacancyId;

                //var vacancyExistdb = db.VaccancyPlans.Where(x => x.BranchId == vaccancyPlan.BranchId && x.IsActive && x.From != vaccancyPlan.From && x.To != vaccancyPlan.To).ToList();
                //if (vacancyExistdb.Any())
                //{
                //    foreach (var vacancy in vacancyExistdb)
                //    {
                //        vacancy.IsActive = false;
                //        db.Entry(vacancy).State = EntityState.Modified;
                //        db.SaveChanges();
                //    }
                //}
            }


            return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VaccancyPlans_Update([DataSourceRequest]DataSourceRequest request, Vacancy vaccancyPlan)
        {
            ModelState.Clear();

            if (vaccancyPlan.FromVacAm != null)
            {
                var splitDateFrom = vaccancyPlan.FromVacAm.Split('/');
                vaccancyPlan.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateFrom[0]), Convert.ToInt32(splitDateFrom[1]), Convert.ToInt32(splitDateFrom[2]));
            }
            else
            {
                ModelState.AddModelError("", "Start date is required");
            }
            if (vaccancyPlan.ToVacAm != null)
            {
                var splitDateTo = vaccancyPlan.ToVacAm.Split('/');
                vaccancyPlan.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]), Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));
            }
            else
            {
                ModelState.AddModelError("", "End date is required");
            }

            if (vaccancyPlan.From > vaccancyPlan.To)
            {
                ModelState.AddModelError("", "Please Check the date range, From must be less or equal to To!!");
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
                    FromVacAm = AmharicStartDate,
                    ToVacAm = AmharicEndDate,
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
                    JobRequirement = vaccancyPlan.JobRequirement,
                    Responsibility = vaccancyPlan.Responsibility,
                    TotalYearOfExperience = vaccancyPlan.TotalYearOfExperience,
                    EthioJobsCode = vaccancyPlan.EthioJobsCode,
                    Language = vaccancyPlan.Language,
                    // CurrentStatusTracking = vaccancyPlan.CurrentStatusTracking,
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
                    JobRequirement = vaccancyPlan.JobRequirement,
                    Responsibility = vaccancyPlan.Responsibility,
                    TotalYearOfExperience = vaccancyPlan.TotalYearOfExperience,
                    EthioJobsCode = vaccancyPlan.EthioJobsCode,
                    Language = vaccancyPlan.Language,
                    // CurrentStatusTracking = vaccancyPlan.CurrentStatusTracking,
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
        [AcceptVerbs(HttpVerbs.Post)]
        public string Close(int id)
        {
            var status = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == id);
            if (status != null)
            {
                if (status.IsActive)
                {
                    status.IsActive = false;
                }
                else
                {
                    return "already closed before";
                }


                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
            }
            return "successfully closed";
        }
        public string ForceOpen(int id)
        {
            var status = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == id);
            if (status != null)
            {
                if (!status.IsActive)
                {
                    status.IsActive = true;
                }
                else
                {
                    return "already opened before";
                }


                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
            }
            return "successfully opened";
        }
        public ActionResult GetRecruitmentSource()
        {
            var sources = Enum.GetValues(typeof(RecruitmentSource))
                       .Cast<RecruitmentSource>()
                       .Select(d => (d))
                       .ToList();

            return Json(sources.Select(p => new
            {
                Name = p.ToString(),
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
                Name = p.ToString(),
                Value = (int)p

            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVacancyType()
        {
            var levels = Enum.GetValues(typeof(VacancyType))
                       .Cast<VacancyType>()
                       .Select(d => (d))
                       .ToList();

            return Json(levels.Select(p => new
            {
                Name = p.ToString(),
                Value = (int)p

            }), JsonRequestBehavior.AllowGet);
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult SummaryReport()
        {
            return View();
        }

        public ActionResult SummaryReportCourt()
        {
            return View();
        }
    }

    public class RecruitPlan
    {
        public int RecruitmentPlanId { get; set; }
        public string JobTitle { get; set; }
    }
}
