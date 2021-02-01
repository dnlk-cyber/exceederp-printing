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
using ExceedERP.Core.Domain.Global;

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

            var levels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EduLevelList"] = levels;
            var media = db.Lookups.Where(x => x.LookupType == LookUpTypes.MEDIA);
            ViewData["Media"] = media;
            GetOrganizations();
            var branches = db.Branch;
            ViewData["branches"] = branches;
            var vacancies = db.VaccancyPlans.ToList();
            ViewData["Vacancy"] = vacancies;

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

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult VaccancyPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<Vacancy> vaccancyplans = db.VaccancyPlans.OrderByDescending(x => x.VacancyId).Where(x => mybraches.Contains(x.OrgBranchName));

            DataSourceResult result = vaccancyplans.ToList().ToDataSourceResult(request, vaccancyPlan => new
            {
                VacancyId = vaccancyPlan.VacancyId,
                VaccancyCode = vaccancyPlan.VaccancyCode,
                GlFiscalYearId = vaccancyPlan.GlFiscalYearId,
                //RecruitmentPlanId = vaccancyPlan.RecruitmentPlanId,
                From = vaccancyPlan.From,
                To = vaccancyPlan.To,
                BranchId = vaccancyPlan.BranchId,
                LookupId = vaccancyPlan.LookupId,
                IsActive = vaccancyPlan.IsActive,
                IsComplete = vaccancyPlan.IsComplete,
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
                WorkUnitId = vaccancyPlan.WorkUnitId,
                OrgStructureId = vaccancyPlan.OrgStructureId


            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VaccancyPlans_Create([DataSourceRequest]DataSourceRequest request, Vacancy vaccancyPlan)
        {
            ModelState.Clear();

            string id = "";
            int nextID;
            var alllist = db.VaccancyPlans;
            if (vaccancyPlan.VacancyType == VacancyType.CONTRACT)
            {
                if (alllist.Any())
                {

                    var cont = alllist.Where(x => x.VacancyType == VacancyType.CONTRACT).OrderByDescending(x => x.VacancyId).FirstOrDefault();

                    if (cont != null)
                    {
                        nextID = Convert.ToInt32(cont.VaccancyCode.Split('-')[1]) + 1;

                        id = "CO-" + nextID.ToString() + "-" + DateTime.Now.Year.ToString();
                    }
                    else
                    {
                        id = "CO-" + "1" + "-" + DateTime.Now.Year.ToString();
                    }
                }
                else
                {
                    id = "CO-" + "1" + "-" + DateTime.Now.Year.ToString();
                }

            }
            else
            {
                if (alllist.Any())
                {

                    var cont = alllist.Where(x => x.VacancyType == VacancyType.PERMANENT).OrderByDescending(x => x.VacancyId).FirstOrDefault();

                    if (cont != null)
                    {
                        nextID = Convert.ToInt32(cont.VaccancyCode.Split('-')[1]) + 1;
                        id = "PE-" + nextID.ToString() + "-" + DateTime.Now.Year.ToString();
                    }
                    else
                    {
                        id = "PE-" + "1" + "-" + DateTime.Now.Year.ToString();
                    }
                }
                else
                {
                    if (vaccancyPlan.VacancyType == VacancyType.CONTRACT)
                    {
                        id = "CO-" + "1" + "-" + DateTime.Now.Year.ToString();
                    }
                    else
                    {
                        id = "PE-" + "1" + "-" + DateTime.Now.Year.ToString();
                    }
                }
            }
            var recruitmentPlan = db.RecruitmentPlans.Find(vaccancyPlan.RecruitmentPlanId);
            if (recruitmentPlan != null)
            {
                var recruitmentReq = db.RecruitmentRequest.FirstOrDefault(x => x.OrgStructureId == recruitmentPlan.OrgStructureId &&
                                                                              x.LookupId == recruitmentPlan.LookupId &&
                                                                              x.GlFiscalYearId == vaccancyPlan.GlFiscalYearId &&
                                                                              x.BranchId == vaccancyPlan.BranchId);
                if (recruitmentReq == null)
                {
                    ModelState.AddModelError("", "No such Vacancy Request, cannot post vacancy without request!!!");
                }
            }

            startDate = vaccancyPlan.FromVacAm;
            var splitDateFrom = startDate.Split('/');
            StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateFrom[0]),
                Convert.ToInt32(splitDateFrom[1]), Convert.ToInt32(splitDateFrom[2]));
            endDate = vaccancyPlan.ToVacAm;
            var splitDateTo = endDate.Split('/');
            EndDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateTo[0]),
                Convert.ToInt32(splitDateTo[1]), Convert.ToInt32(splitDateTo[2]));



            var vacCheck = db.VaccancyPlans.FirstOrDefault(x =>
                x.BranchId == vaccancyPlan.BranchId && x.OrgStructureId == vaccancyPlan.OrgStructureId &&
                x.RecruitmentPlanId == vaccancyPlan.RecruitmentPlanId &&
                x.GlFiscalYearId == vaccancyPlan.GlFiscalYearId && x.To >= StartDate && !x.IsComplete);
            if (vacCheck != null)
            {
                ModelState.AddModelError("", "This vacancy is Posted Before If You Need To Post Again You should close Previos Post First");
            }

            if (ModelState.IsValid)
            {
                var checkExistance =
                  db.VaccancyPlans.FirstOrDefault(x => x.VaccancyCode == id && x.RecruSource == vaccancyPlan.RecruSource);

                if (checkExistance != null)
                {
                    //ModelState.AddModelError("", "This Vacancy code is already registered!!");
                    id = checkExistance.VaccancyCode + "" + DateTime.Now.Day.ToString();
                }
                else if (StartDate > EndDate)
                {
                    ModelState.AddModelError("", "Please Check the date range, Start date must be less or equal to End date!!");
                }
                else if (AmharicStartDate == null || AmharicEndDate == null)
                {
                    ModelState.AddModelError("", "Start date and end date values are mandatory!!");
                }
                else
                {



                    var entity = new Vacancy
                    {
                        //VacancyId = vaccancyPlan.VacancyId,
                        VaccancyCode = id,
                        RecruitmentPlanId = vaccancyPlan.RecruitmentPlanId,
                        OrgStructureId = vaccancyPlan.OrgStructureId,
                        From = StartDate,
                        BranchId = vaccancyPlan.BranchId,
                        GlFiscalYearId = vaccancyPlan.GlFiscalYearId,
                        To = EndDate,
                        IsActive = true,
                        IsComplete = false,
                        LookupId = vaccancyPlan.LookupId,
                        RecruSource = vaccancyPlan.RecruSource,
                        Remark = vaccancyPlan.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = vaccancyPlan.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = vaccancyPlan.ModifiedBy,
                        FromVacAm = AmharicStartDate,
                        ToVacAm = AmharicEndDate,
                        VacancyType = vaccancyPlan.VacancyType,
                        WorkUnitId = vaccancyPlan.WorkUnitId,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)

                    };

                    db.VaccancyPlans.Add(entity);
                    db.SaveChanges();
                    vaccancyPlan.VacancyId = entity.VacancyId;


                    var vacancyExistdb = db.VaccancyPlans.Where(x =>
                        x.BranchId == vaccancyPlan.BranchId && x.OrgStructureId == vaccancyPlan.OrgStructureId &&
                        x.RecruitmentPlanId == vaccancyPlan.RecruitmentPlanId & x.IsActive && x.From != StartDate &&
                        x.To != EndDate).ToList();
                    if (vacancyExistdb.Any())
                    {
                        foreach (var vacancy in vacancyExistdb)
                        {
                            vacancy.IsActive = false;
                            db.Entry(vacancy).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    #region operation log
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == entity.BranchId);
                    if (branch != null)
                    {
                        var org = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == entity.OrgStructureId);
                        if (org != null)
                        {
                            var recruitment = db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == entity.RecruitmentPlanId);
                            if (recruitment != null)
                            {
                                var job = db.Lookups.FirstOrDefault(x=>x.LookupId == recruitment.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                                if(job != null)
                                {
                                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                                    var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                                    if (name != null)
                                    {
                                        UserHelper.OperationLog(Request.UserHostAddress, "Vacancy ", "Success", User.Identity.Name, name.EmployeeCode + " add new Vacancy for branch " + branch.Name + " workunit " + org.Name + " job title " + job.Description + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                                    }

                                }

                            }

                        }
                    }

                    #endregion

                }
            }


            return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VaccancyPlans_Update([DataSourceRequest]DataSourceRequest request, Vacancy vaccancyPlan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AmharicStartDate == null)
                {
                    AmharicStartDate = vaccancyPlan.FromVacAm;
                    var splitDate = AmharicStartDate.Split('/');
                    StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                else if (AmharicEndDate == null)
                {
                    AmharicEndDate = vaccancyPlan.ToVacAm;
                    var splitDate = AmharicEndDate.Split('/');
                    EndDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (StartDate > EndDate)
                {
                    ModelState.AddModelError("", "Please Check the date range, From must be less or equal to To!!");
                }
                else
                {
                    var entity = new Vacancy
                    {
                        VacancyId = vaccancyPlan.VacancyId,
                        VaccancyCode = vaccancyPlan.VaccancyCode,
                        RecruitmentPlanId = vaccancyPlan.RecruitmentPlanId,
                        OrgStructureId = vaccancyPlan.OrgStructureId,
                        From = StartDate,
                        BranchId = vaccancyPlan.BranchId,
                        GlFiscalYearId = vaccancyPlan.GlFiscalYearId,
                        To = EndDate,
                        LookupId = vaccancyPlan.LookupId,
                        IsActive = vaccancyPlan.IsActive,
                        IsComplete = vaccancyPlan.IsComplete,
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
                    };

                    db.VaccancyPlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == entity.BranchId);
                    if (branch != null)
                    {
                        var org = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == entity.OrgStructureId);
                        if (org != null)
                        {
                            var recruitment = db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == entity.RecruitmentPlanId);
                            if (recruitment != null)
                            {
                                var job = db.Lookups.FirstOrDefault(x => x.LookupId == recruitment.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                                if (job != null)
                                {
                                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                                    var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                                    if (name != null)
                                    {
                                        UserHelper.OperationLog(Request.UserHostAddress, "Vacancy ", "Success", User.Identity.Name, name.EmployeeCode + " update Vacancy of branch " + branch.Name + " workunit " + org.Name + " job title " + job.Description + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                                    }

                                }

                            }

                        }
                    }

                    #endregion

                }
            }

            return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VaccancyPlans_Destroy([DataSourceRequest]DataSourceRequest request, Vacancy vaccancyPlan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckVacancyValidation(vaccancyPlan.VaccancyCode))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new Vacancy
                    {
                        VacancyId = vaccancyPlan.VacancyId,
                        VaccancyCode = vaccancyPlan.VaccancyCode,
                        RecruitmentPlanId = vaccancyPlan.RecruitmentPlanId,
                        OrgStructureId = vaccancyPlan.OrgStructureId,
                        GlFiscalYearId = vaccancyPlan.GlFiscalYearId,
                        From = vaccancyPlan.From,
                        BranchId = vaccancyPlan.BranchId,
                        To = vaccancyPlan.To,
                        LookupId = vaccancyPlan.LookupId,
                        IsActive = vaccancyPlan.IsActive,
                        IsComplete = vaccancyPlan.IsComplete,
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
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted vacancy of vacancy code " + vaccancyPlan.VaccancyCode);
                    #endregion
                    db.VaccancyPlans.Attach(entity);
                    db.VaccancyPlans.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { vaccancyPlan }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public string Close(int id)
        {
            var status = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == id);
            if (status != null)
            {
                status.IsComplete = true;
                if (status.IsActive)
                {
                    status.IsActive = false;
                }
                db.VaccancyPlans.Attach(status);
                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
            }
            return status?.ToString();
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
