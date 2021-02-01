using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
    public class RecruitmentPlanController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int current { get; set; }
        public static int tobe { get; set; }
        public static int vacant { get; set; }
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        public static string AmharicFrom { get; set; }
        public static string AmharicTo { get; set; }
        public static DateTime RecruitTime { get; set; }
        public static string RecruitTimeAmharic { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            GetOrganizations();
            //PrepareRecruitmentPlans();
            return View();
        }
        private void PopulateViewData()
        {
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();


            var levels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EduLevelList"] = levels;
            var ranks = db.Ranks.ToList();
            ViewData["RankList"] = ranks;
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
            var branch = db.Branch.ToList();
            ViewData["Branches"] = branch;

            ViewData["Jobtitles"] = db.Lookups.Where(x=>x.LookupType == LookUpTypes.JOB_TITLE);
    }
        public void SetDate(string name, string value)
        {
            if (name == "RecruiTimeAmharic")
            {
                RecruitTimeAmharic = value;
                var splitDate = RecruitTimeAmharic.Split('/');
                RecruitTime = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToAmharic")
            {
                AmharicTo = value;
                var splitDate = AmharicTo.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "FromAmharic")
            {
                AmharicFrom = value;
                var splitDate = AmharicFrom.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }
        public JsonResult GetJobTitle(int OrgStructureId)
        {
            db = new ExceedDbContext();
            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == OrgStructureId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    if (lookup != null)
                    {
                        if (!titleList.Contains(lookup))
                        {

                            titleList.Add(lookup);
                        }
                    }

                }
            }
            return Json(titleList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        public string GetDetail(int OrgId, int title)
        {
            string detail = "";
            string expriance = "";
            string relevantExp = "";
            string specialSkill = "";
            string plandSalary = "";
            string level = "";
            string vacantPostion = "";

            var lookups = db.Lookups;
            var placment = db.Placements.Where(x => x.OrgStructureId == OrgId && x.LookupId == title && x.isCurrent).ToList();
            var mapping = db.JobTitleMapping.Where(x => x.OrgStructureId == OrgId && x.LookupId == title).FirstOrDefault();
            if (mapping != null)
            {
                vacantPostion = (mapping.Quntity - placment.Count).ToString() + " ";
                var mappingDetails = db.JobTitleMappingDetail.Where(X => X.JobTitleMappingId == mapping.JobTitleMappingId).ToList();
                foreach (var m in mappingDetails)
                {
                    expriance = m.Experience + " ";
                    relevantExp = m.RelevantExp + " ";
                    specialSkill = m.SpecialSkill + " ";
                    plandSalary = m.PlannedSalary.ToString() + " ";

                    var edulevel = lookups.Where(x => x.LookupId == m.LookupId).FirstOrDefault();
                    if (edulevel != null)
                    {
                        level = edulevel.Description + " ";
                    }
                }
            }
            return plandSalary + "-" + relevantExp + "-" + specialSkill + "-" + expriance + "-" + level + "-" + vacantPostion;
        }
        private string GetTitleName(int id)
        {

            try
            {
                db = new ExceedDbContext();
                var name = db.Lookups.Where(x => x.LookupId == id).FirstOrDefault().Description;
                return name;
            }
            catch (Exception)
            {

                return "";
            }
        }
        public int CheckPostions(int OrgId, int title, int branch)
        {
            tobe = 0;
            current = 0;
            vacant = 0;
            var toBepostions = db.JobTitleMapping.FirstOrDefault(x => x.OrgStructureId == OrgId && x.LookupId == title && x.BranchId == branch);
            var currentStaff = db.Placements.Where(x => x.OrgStructureId == OrgId && x.LookupId == title && x.BranchId == branch && x.isCurrent).ToList();
            if (currentStaff.Any())
            {
                current = currentStaff.Count;
            }
            int quntity = 0;
            if (toBepostions != null)
            {
                tobe = toBepostions.Quntity;

            }

            vacant = tobe - current;
            return vacant;//tobe - currentStaff;


        }
        public JsonResult PrepareRecruitmentPlans(string branch)
        {
            var mapping = db.JobTitleMapping.ToList();
            if (branch != null)
            {
                mapping = mapping.Where(x => x.BranchId.ToString() == branch).ToList();
            }
            db = new ExceedDbContext();
            var lookups = db.Lookups;
            
            db = new ExceedDbContext();
            if (mapping.Any())
            {
                foreach (var m in mapping)
                {
                    db = new ExceedDbContext();
                    var plan = db.RecruitmentPlans.FirstOrDefault(x => x.LookupId == m.LookupId && x.OrgStructureId == m.OrgStructureId);
                    if (plan == null)
                    {
                        var detail = db.JobTitleMappingDetail.FirstOrDefault(x => x.JobTitleMappingId == m.JobTitleMappingId);
                        if (detail != null)
                        {
                            var level = lookups.FirstOrDefault(x => x.LookupId == detail.LookupId);
                            var entity = new RecruitmentPlan
                            {
                                OrgStructureId = m.OrgStructureId,
                                JobtitleName = GetTitleName(m.LookupId),
                                LookupId = m.LookupId,
                                NoOfVacantPost = CheckPostions(m.OrgStructureId, m.LookupId, m.BranchId),
                                EduLevel = "",
                                RelevantExp = detail.RelevantExp,
                                SpecialSkill = detail.SpecialSkill,
                                Experience = detail.Experience,
                                PlannedSalary = detail.PlannedSalary,
                                BranchId = m.BranchId,
                                WorkUnitId = m.WorkUnitId,
                                //RecruiTime = null,
                                RecruitmentType = VacancyType.PERMANENT,
                                From = null,
                                To = null,
                                RankId = null,
                                Remark = null,
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = null,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

                                FromAmharic = null,
                                ToAmharic = null,
                            };
                            db = new ExceedDbContext();
                            if (entity.NoOfVacantPost >= 1)
                            {
                                if (level != null)
                                {
                                    entity.EduLevel = level.Description;
                                }
                                db.RecruitmentPlans.Add(entity);
                                db.SaveChanges();
                                
                            }
                            // recruitmentPlan.RecruitmentPlanId = entity.RecruitmentPlanId;
                           
                        }
                    }
                    else
                    {
                        db = new ExceedDbContext();
                        var detaill = db.JobTitleMappingDetail.FirstOrDefault(x => x.JobTitleMappingId == m.JobTitleMappingId);
                        if (detaill != null)
                        {
                            var level = lookups.FirstOrDefault(x => x.LookupId == detaill.LookupId);

                            var entity = new RecruitmentPlan
                            {
                                RecruitmentPlanId = plan.RecruitmentPlanId,
                                OrgStructureId = plan.OrgStructureId,
                                JobtitleName = plan.JobtitleName,
                                LookupId = plan.LookupId,
                                NoOfVacantPost = CheckPostions(plan.OrgStructureId, plan.LookupId, m.BranchId),
                                EduLevel = "",
                                RelevantExp = detaill.RelevantExp,
                                SpecialSkill = detaill.SpecialSkill,
                                Experience = detaill.Experience,
                                PlannedSalary = detaill.PlannedSalary,
                                BranchId = m.BranchId,
                                WorkUnitId = m.WorkUnitId,
                                RecruitmentType = plan.RecruitmentType,
                                From = plan.From,
                                To = plan.To,
                                RankId = plan.RankId,
                                Remark = plan.Remark,
                                DateCreated = plan.DateCreated,
                                LastModified = DateTime.Now,
                                CreatedBy = plan.CreatedBy,
                                ModifiedBy = User.Identity.Name,
                                //RecruiTimeAmharic = plan.RecruiTimeAmharic,
                                FromAmharic = plan.FromAmharic,
                                GlFiscalYearId = plan.GlFiscalYearId,
                                ToAmharic = plan.ToAmharic,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                            };
                            if (level != null)
                            {
                                entity.EduLevel = level.Description;
                            }
                            db.RecruitmentPlans.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                           
                        }
                        
                    }
                }

                var formattedData = new
                {
                    Message = "Successfully Refresh.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1= new
            {
                Message = "m.",
                Status = "NOT_OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrganizations()
        {

            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;
            return Json(structres.Select(s => new
            {
                OrgStructureId = s.OrgStructureId,
                Name = s.Name
            }), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult RecruitmentPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<RecruitmentPlan> recruitmentplans = db.RecruitmentPlans.Where(x => mybraches.Contains(x.OrgBranchName) && x.NoOfVacantPost > 0);
            DataSourceResult result = recruitmentplans.ToDataSourceResult(request, recruitmentPlan => new
            {
                RecruitmentPlanId = recruitmentPlan.RecruitmentPlanId,
                LookupId = recruitmentPlan.LookupId,
                OrgStructureId = recruitmentPlan.OrgStructureId,
                JobtitleName = GetTitleName(recruitmentPlan.LookupId),
                NoOfVacantPost = recruitmentPlan.NoOfVacantPost,
                EduLevel = recruitmentPlan.EduLevel,
                GlFiscalYearId = recruitmentPlan.GlFiscalYearId,
                RelevantExp = recruitmentPlan.RelevantExp,
                SpecialSkill = recruitmentPlan.SpecialSkill,
                PlannedSalary = recruitmentPlan.PlannedSalary,
                //RecruiTime = recruitmentPlan.RecruiTime,
                RecruitmentType = recruitmentPlan.RecruitmentType,
                From = recruitmentPlan.From,
                To = recruitmentPlan.To,
                Experience = recruitmentPlan.Experience,
                RankId = recruitmentPlan.RankId,
                Remark = recruitmentPlan.Remark,
                Status = PlanStatus(recruitmentPlan.OrgStructureId, recruitmentPlan.LookupId, recruitmentPlan.BranchId),
                DateCreated = recruitmentPlan.DateCreated,
                LastModified = recruitmentPlan.LastModified,
                CreatedBy = recruitmentPlan.CreatedBy,
                ModifiedBy = recruitmentPlan.ModifiedBy,
                OrgBranchName = recruitmentPlan.OrgBranchName,
                FromAmharic = recruitmentPlan.FromAmharic,
                ToAmharic = recruitmentPlan.ToAmharic,
                BranchId = recruitmentPlan.BranchId,
                WorkUnitId = recruitmentPlan.WorkUnitId,
                //GlFiscalYearId = recruitmentPlan.GlFiscalYearId

            });

            return Json(result);
        }
        private int PlanStatus(int orgid, int id, int branchid)
        {
            var toBepostions = db.JobTitleMapping.FirstOrDefault(x => x.OrgStructureId == orgid && x.LookupId == id && x.BranchId == branchid);
            var currentStaff = db.Placements.Where(x => x.OrgStructureId == orgid && x.LookupId == id && x.BranchId == branchid).ToList().Count;
            int quntity = 0;
            if (toBepostions != null)
            {
                quntity = toBepostions.Quntity;


            }
            return quntity - currentStaff;
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentPlans_Create([DataSourceRequest]DataSourceRequest request, RecruitmentPlan recruitmentPlan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (From.Date > To.Date)
                {
                    ModelState.AddModelError("", "Please check the date range.");
                }
                else
                {
                    var entity = new RecruitmentPlan
                    {
                        OrgStructureId = recruitmentPlan.OrgStructureId,
                        JobtitleName = GetTitleName(recruitmentPlan.LookupId),
                        LookupId = recruitmentPlan.LookupId,
                        NoOfVacantPost = recruitmentPlan.NoOfVacantPost,
                        EduLevel = recruitmentPlan.EduLevel,
                        RelevantExp = recruitmentPlan.RelevantExp,
                        SpecialSkill = recruitmentPlan.SpecialSkill,
                        Experience = recruitmentPlan.Experience,
                        PlannedSalary = recruitmentPlan.PlannedSalary,
                        GlFiscalYearId = recruitmentPlan.GlFiscalYearId,
                        //RecruiTime = RecruitTime,
                        RecruitmentType = recruitmentPlan.RecruitmentType,
                        From = From,
                        To = To,
                        RankId = recruitmentPlan.RankId,
                        Remark = recruitmentPlan.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = recruitmentPlan.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = recruitmentPlan.ModifiedBy,
                        //RecruiTimeAmharic =RecruitTimeAmharic,
                        FromAmharic = AmharicFrom,
                        ToAmharic = AmharicTo,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        BranchId = recruitmentPlan.BranchId,
                        WorkUnitId = recruitmentPlan.WorkUnitId,
                    };

                    db.RecruitmentPlans.Add(entity);
                    db.SaveChanges();
                    recruitmentPlan.RecruitmentPlanId = entity.RecruitmentPlanId;
                }
            }

            return Json(new[] { recruitmentPlan }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentPlans_Update([DataSourceRequest]DataSourceRequest request, RecruitmentPlan recruitmentPlan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AmharicFrom == null)
                {
                    if (recruitmentPlan.FromAmharic != null)
                    {
                        AmharicFrom = recruitmentPlan.FromAmharic;
                        var splitDate = AmharicFrom.Split('/');
                        From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]),
                            Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    }
                }
                if (AmharicTo == null)
                {
                    if (recruitmentPlan.ToAmharic != null)
                    {
                        AmharicTo = recruitmentPlan.ToAmharic;
                        var splitDate = AmharicTo.Split('/');
                        To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]),
                            Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    }
                }
                if (From.Date > To.Date)
                {
                    ModelState.AddModelError("", "Please check the date range.");
                }
                else
                {
                    var entity = new RecruitmentPlan
                    {
                        RecruitmentPlanId = recruitmentPlan.RecruitmentPlanId,
                        OrgStructureId = recruitmentPlan.OrgStructureId,
                        JobtitleName = GetTitleName(recruitmentPlan.LookupId),
                        LookupId = recruitmentPlan.LookupId,
                        NoOfVacantPost = recruitmentPlan.NoOfVacantPost,
                        EduLevel = recruitmentPlan.EduLevel,
                        Experience = recruitmentPlan.Experience,
                        RecruitmentType = recruitmentPlan.RecruitmentType,
                        RelevantExp = recruitmentPlan.RelevantExp,
                        SpecialSkill = recruitmentPlan.SpecialSkill,
                        PlannedSalary = recruitmentPlan.PlannedSalary,
                        GlFiscalYearId = recruitmentPlan.GlFiscalYearId,
                        //RecruiTime = RecruitTime,                  
                        From = From,
                        To = To,
                        RankId = recruitmentPlan.RankId,
                        Remark = recruitmentPlan.Remark,
                        DateCreated = recruitmentPlan.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = recruitmentPlan.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        //RecruiTimeAmharic = RecruitTimeAmharic,
                        FromAmharic = AmharicFrom,
                        ToAmharic = AmharicTo,
                        OrgBranchName = recruitmentPlan.OrgBranchName,
                        BranchId = recruitmentPlan.BranchId,
                        WorkUnitId = recruitmentPlan.WorkUnitId,
                    };

                    db.RecruitmentPlans.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    var branch = db.Branch.FirstOrDefault(x=>x.BranchId == entity.BranchId);
                    if(branch != null)
                    {
                        var org = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == entity.OrgStructureId);
                        if(org != null)
                        {
                            var job = db.Lookups.FirstOrDefault(x=>x.LookupId == entity.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                            if(job !=null)
                            {
                                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                                var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                                if (name != null)
                                {
                                    UserHelper.OperationLog(Request.UserHostAddress, "Recruitment Plan ", "Success", User.Identity.Name, name.EmployeeCode + " update Recruitment Plan of branch "+branch.Name+" workunit "+org.Name+" job title "+job.Description + " date " + DateTime.Today.ToShortDateString(),Core.Domain.Global.Modules.HRM);
                                }

                            }

                        }
                    }
                   
                    #endregion

                }
            }

            return Json(new[] { recruitmentPlan }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecruitmentPlans_Destroy([DataSourceRequest]DataSourceRequest request, RecruitmentPlan recruitmentPlan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new RecruitmentPlan
                {
                    RecruitmentPlanId = recruitmentPlan.RecruitmentPlanId,
                    LookupId = recruitmentPlan.LookupId,
                    NoOfVacantPost = recruitmentPlan.NoOfVacantPost,
                    EduLevel = recruitmentPlan.EduLevel,
                    RelevantExp = recruitmentPlan.RelevantExp,
                    SpecialSkill = recruitmentPlan.SpecialSkill,
                    PlannedSalary = recruitmentPlan.PlannedSalary,
                    GlFiscalYearId = recruitmentPlan.GlFiscalYearId,
                    //RecruiTime = recruitmentPlan.RecruiTime,
                    RecruitmentType = recruitmentPlan.RecruitmentType,
                    Experience = recruitmentPlan.Experience,
                    From = recruitmentPlan.From,
                    To = recruitmentPlan.To,
                    RankId = recruitmentPlan.RankId,
                    OrgStructureId = recruitmentPlan.OrgStructureId,
                    Remark = recruitmentPlan.Remark,
                    DateCreated = recruitmentPlan.DateCreated,
                    LastModified = recruitmentPlan.LastModified,
                    CreatedBy = recruitmentPlan.CreatedBy,
                    ModifiedBy = recruitmentPlan.ModifiedBy,
                    //RecruiTimeAmharic = recruitmentPlan.RecruiTimeAmharic,
                    FromAmharic = recruitmentPlan.FromAmharic,
                    ToAmharic = recruitmentPlan.ToAmharic,
                    OrgBranchName = recruitmentPlan.OrgBranchName,
                    BranchId = recruitmentPlan.BranchId,
                    WorkUnitId = recruitmentPlan.WorkUnitId,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted recruitment plan of structure  " + recruitmentPlan.MainCompanyStructure.Name + " branch" + recruitmentPlan.BranchId);
                #endregion
                db.RecruitmentPlans.Attach(entity);
                db.RecruitmentPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { recruitmentPlan }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Recruitment plan ");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Recruitment plan ");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
