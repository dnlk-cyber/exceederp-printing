using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{

    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class NeedAssessmentController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            HRMInputs inputs = new HRMInputs();
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
            GetCourse();
            GetProvider();
            GetProvider1();
            PopulateViewData();
            GetTargetGroup();
            TrainingType();
            return View();
        }
        private void PopulateViewData()
        {
            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;
            GetDepartments();
            var branch = db.Branch.ToList();
            ViewData["Branches"] = branch;
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList;
            ViewData["allDepartments"] = db.MainCompanyStructures.ToList();

            ViewData["TargetGroup"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.TARGET_GROUP);

        }
        public ActionResult FiscalYearTrainingPlanActionPlan()
        {
            return View();
        }
        public ActionResult TrainingNeedRequestReport()
        {
            return View();
        }
        public JsonResult GetTargetGroup()
        {
            var targetGroupList = db.Lookups.AsQueryable().ToList();
            if (targetGroupList.Count > 0)
            {
                targetGroupList = targetGroupList.Where(l => l.LookupType == LookUpTypes.TARGET_GROUP && l.IsActive).ToList();
            }
            ViewData["TargetGroupList"] = targetGroupList.Count > 0 ? targetGroupList.ToList() : new List<Lookup>();
            return Json(targetGroupList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.FirstOrDefault(x => x.Parent == 0);
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo =
                            db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            ViewData["Departments"] = allDepartment;
        }
        public JsonResult GetCourse()
        {
            var courseList = db.Lookups.AsQueryable();
            if (courseList.Count() > 0)
            {
                courseList = courseList.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive).AsQueryable();
            }
            ViewData["CourseList"] = courseList != null ? courseList.ToList() : new List<Lookup>();
            return Json(courseList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult TrainingType()
        {
            var traTypeList = db.Lookups.AsQueryable();
            if (traTypeList.Count() > 0)
            {
                traTypeList = traTypeList.Where(l => l.LookupType == LookUpTypes.TRAINING_TYPE).OrderBy(o => o.LookupType);

            }
            ViewData["TraTypeList"] = traTypeList != null ? traTypeList.ToList() : new List<Lookup>();
            return Json(traTypeList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProvider()
        {
            var providerlist = db.Companies.AsQueryable();
            if (providerlist.Count() > 0)
            {
                providerlist = providerlist.Where(c => c.Category == "1" && c.IsActive).AsQueryable();
            }
           // ViewData["Companies"] = providerlist.Count > 0 ? providerlist.ToList() : new List<MainCompany>();
            ViewData["ProviderList"] = providerlist != null ? providerlist.ToList() : new List<MainCompany>();
            return Json(providerlist.Select(p => new { MainCompanyId = p.MainCompanyId, TradeName = p.TradeName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProvider1()
        {
            var providerlist = db.Companies.AsQueryable().ToList();
            if (providerlist.Any())
            {
                providerlist = providerlist.Where(c => c.Category == "1" && c.IsActive).ToList();
            }
            ViewData["Companies"] = providerlist.Count > 0 ? providerlist.ToList() : new List<MainCompany>();
            return Json(providerlist.Select(p => new
            {
                MainCompanyId = p.MainCompanyId,
                TradeName = p.TradeName
            }), JsonRequestBehavior.AllowGet);
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
        public ActionResult SummarizeNeedSummary(int[] assessmentIds)
        {
            var needAssessments = db.NeedAssessments.Where(x => assessmentIds.Contains(x.AssesmentId)).ToList();
            if (needAssessments.Count() > 0)
            {
                var needAssessmentSummaries = needAssessments
                    .GroupBy(x => new
                    {
                        x.GlFiscalYearId,
                        x.LookupId,
                    }).Select(g => new NeedAssessmentSummary
                    {
                        GlFiscalYearId = g.Key.GlFiscalYearId,
                        LookupId = g.Key.LookupId,
                        NoOfStaff = g.Sum(s => s.NoOfStaff),
                        RequiredTraining = 0,
                        CostPerPerson = 0,
                        Check = true,
                        DateCreated = DateTime.Now.Date,
                        CreatedBy = User.Identity.Name,
                        LastModified = null,
                        ModifiedBy = null

                    }).ToList();
                foreach (var needAssessmentSummary in needAssessmentSummaries)
                {
                    var entity = new NeedAssessmentSummary
                    {
                        GlFiscalYearId = needAssessmentSummary.GlFiscalYearId,
                        LookupId = needAssessmentSummary.LookupId,
                        NoOfStaff = needAssessmentSummary.NoOfStaff,
                        RequiredTraining = 0,
                        CostPerPerson = 0,
                        IsTransfered = false,
                        IsSendForApproval = false,
                        IsSecondSendForApproval = false,
                        Check = false,
                        DateCreated = DateTime.Now.Date,
                        CreatedBy = User.Identity.Name,
                        LastModified = null,
                        ModifiedBy = null

                    };
                    db.NeedAssessmentSummary.Add(entity);
                    db.SaveChanges();
                    var needAssessmentIds = needAssessments.Where(n => n.GlFiscalYearId == needAssessmentSummary.GlFiscalYearId && n.LookupId == needAssessmentSummary.LookupId).Select(n => n.AssesmentId);
                    foreach(var assessmentId in needAssessmentIds)
                    {
                        var entity1 = new NeedAssessmentSummaryDetail
                        {
                            AssesmentId = assessmentId,
                            AssesmentSummaryId = entity.NeedAssessmentSummaryId
                        };
                        db.NeedAssessmentSummaryDetails.Add(entity1);
                        db.SaveChanges();
                    }
                }
                foreach(var assessmentId in assessmentIds)
                {
                    var needAssessmentUpdate = db.NeedAssessments.Find(assessmentId);
                    if(needAssessmentUpdate != null)
                    {
                        needAssessmentUpdate.IsSummarized = true;
                        db.NeedAssessments.Attach(needAssessmentUpdate);
                        db.Entry(needAssessmentUpdate).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }

            }
            return Json("Summarized");
        }
        public ActionResult NeedAssessments_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<NeedAssessment> needassessments = db.NeedAssessments.Where(x=>x.IsSendForApproval && !x.IsSummarized);//.Where(x => mybraches.Contains(x.OrgBranchName));

            DataSourceResult result = needassessments.ToDataSourceResult(request, needAssessment => new
            {
                AssesmentId = needAssessment.AssesmentId,
                OrgStructureId = needAssessment.OrgStructureId,
                GlFiscalYearId = needAssessment.GlFiscalYearId,
                NoOfStaff = needAssessment.NoOfStaff,
                RequiredTraining = needAssessment.RequiredTraining,
                LookupId = needAssessment.LookupId,
                Description = needAssessment.Description,
                Type = needAssessment.Type,
                MainCompanyId = needAssessment.MainCompanyId,
                DevelopmentRequired = needAssessment.DevelopmentRequired,
                DevelopmentBy = needAssessment.DevelopmentBy,
                Remark = needAssessment.Remark,
                DateCreated = needAssessment.DateCreated,
                LastModified = needAssessment.LastModified,
                CreatedBy = needAssessment.CreatedBy,
                ModifiedBy = needAssessment.ModifiedBy,
                OrgBranchName = needAssessment.OrgBranchName,
                IsTransfered = needAssessment.IsTransfered,
                IsSendForApproval = needAssessment.IsSendForApproval,
            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessments_Create([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId == employeeId /*&& x.isCurrent*/);
            if(placement != null)
            {
                needAssessment.OrgStructureId = placement.OrgStructureId;
            }
            if(needAssessment.NoOfStaff <= 0)
            {
                ModelState.AddModelError("","No of staff should be greater than zero");
            }
            if (ModelState.IsValid)
            {
                var entity = new NeedAssessment
                {
                    OrgStructureId = needAssessment.OrgStructureId,
                    NoOfStaff = needAssessment.NoOfStaff,
                    RequiredTraining = needAssessment.RequiredTraining,
                    IsTransfered = false,
                    IsSendForApproval = true,
                    GlFiscalYearId = needAssessment.GlFiscalYearId,
                    LookupId = needAssessment.LookupId,
                    Description = needAssessment.Description,
                    Type = needAssessment.Type,
                    MainCompanyId = needAssessment.MainCompanyId,
                    DevelopmentRequired = needAssessment.DevelopmentRequired,
                    DevelopmentBy = needAssessment.DevelopmentBy,
                    Remark = needAssessment.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = needAssessment.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = needAssessment.ModifiedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)

                };

                db.NeedAssessments.Add(entity);
                db.SaveChanges();
                needAssessment.AssesmentId = entity.AssesmentId;

                #region operation log
                              
                var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
                if (name != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Need Assessment ", "Success", User.Identity.Name, name.EmployeeCode + " add new Need Assessment adding date " + DateTime.Today.ToShortDateString(),Modules.HRM);

                }

                #endregion
            }

            return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessments_Update([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();

            if (needAssessment.NoOfStaff <= 0)
            {
                ModelState.AddModelError("", "No of staff should be greater than zero");
            }

            if (ModelState.IsValid)
            {
                var entity = new NeedAssessment
                {
                    AssesmentId = needAssessment.AssesmentId,
                    OrgStructureId = needAssessment.OrgStructureId,

                    GlFiscalYearId = needAssessment.GlFiscalYearId,
                    NoOfStaff = needAssessment.NoOfStaff,
                    RequiredTraining = needAssessment.RequiredTraining,
                    IsTransfered = false,
                    LookupId = needAssessment.LookupId,
                    Description = needAssessment.Description,
                    Type = needAssessment.Type,
                    MainCompanyId = needAssessment.MainCompanyId,
                    DevelopmentRequired = needAssessment.DevelopmentRequired,
                    DevelopmentBy = needAssessment.DevelopmentBy,
                    Remark = needAssessment.Remark,
                    DateCreated = needAssessment.DateCreated,
                    LastModified = needAssessment.LastModified,
                    CreatedBy = needAssessment.CreatedBy,
                    ModifiedBy = needAssessment.ModifiedBy,
                    OrgBranchName = needAssessment.OrgBranchName,
                };

                db.NeedAssessments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                if (name != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Need Assessment ", "Success", User.Identity.Name, name.EmployeeCode + " update Need Assessment Info updated date " + DateTime.Today.ToShortDateString(),Modules.HRM);

                }

                #endregion
            }

            return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult NeedAssessments_Destroy([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new NeedAssessment
                {
                    AssesmentId = needAssessment.AssesmentId,
                    OrgStructureId = needAssessment.OrgStructureId,
                    NoOfStaff = needAssessment.NoOfStaff,
                    GlFiscalYearId = needAssessment.GlFiscalYearId,
                    RequiredTraining = needAssessment.RequiredTraining,
                    IsTransfered = false,
                    LookupId = needAssessment.LookupId,
                    Description = needAssessment.Description,
                    Type = needAssessment.Type,
                    MainCompanyId = needAssessment.MainCompanyId,
                    DevelopmentRequired = needAssessment.DevelopmentRequired,
                    DevelopmentBy = needAssessment.DevelopmentBy,
                    Remark = needAssessment.Remark,
                    DateCreated = needAssessment.DateCreated,
                    LastModified = needAssessment.LastModified,
                    CreatedBy = needAssessment.CreatedBy,
                    ModifiedBy = needAssessment.ModifiedBy,
                    OrgBranchName = needAssessment.OrgBranchName,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted need assessment information of course " + needAssessment.LookupId);
                #endregion
                db.NeedAssessments.Attach(entity);
                db.NeedAssessments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Transfer_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<NeedAssessment> needAssessments)
        {
            ModelState.Clear();
            var results = new List<NeedAssessment>();


            TrainingPlan trainingPlan = new TrainingPlan();
            List<TrainingPlan> planList = new List<TrainingPlan>();

            ExceedDbContext dbs = new ExceedDbContext();

            List<NeedAssessment> needAssessment = db.NeedAssessments.ToList();

            if (needAssessments.Any() && ModelState.IsValid)
            {
                foreach (var check in needAssessments)
                {
                    if (check.Check)
                    {
                        trainingPlan = new TrainingPlan();

                        //trainingPlan.GlFiscalYearId = product.MainCompanyId;
                        //trainingPlan.GlFiscalYearId = 5;//check.GlFiscalYearId,  
                        trainingPlan.WorkUnit = check.OrgStructureId;
                        trainingPlan.LookupId = check.LookupId;
                        trainingPlan.MainCompanyId = check.MainCompanyId;
                        trainingPlan.GlFiscalYearId = check.GlFiscalYearId;
                        //trainingPlan.WorkUnit = check.WorkUnit,  
                        //trainingPlan.CostPerPerson = check.CostPerPerson,
                        //trainingPlan.TotalCost = check.TotalCost,                       
                        trainingPlan.Course = check.LookupId;
                        trainingPlan.NoofTrainee = check.NoOfStaff;
                        trainingPlan.OrgBranchName = check.OrgBranchName;
                        //trainingPlan.LookupId = check.LookupId;
                        trainingPlan.CreatedBy = User.Identity.Name;
                        trainingPlan.DateCreated = DateTime.Now;
                        trainingPlan.ModifiedBy = null;
                        UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        trainingPlan.LastModified = null;
                        planList.Add(trainingPlan);

                        db.TrainingPlans.Add(trainingPlan);
                        db.SaveChanges();

                        #region remove transfered assessment
                        TrainingAssessmentViewModel train = new TrainingAssessmentViewModel();
                        // train.TrainingAssessmentViewModelId=check
                        var assessment = db.NeedAssessments.Find(check.AssesmentId);
                        if (assessment != null)
                        {
                            assessment.IsTransfered = true;

                            db.Entry(assessment).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                    }


                }
             
            }

            return Json(planList.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetJobTitle(int OrganizationId)
        {
            db = new ExceedDbContext();
            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == OrganizationId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    titleList.Add(lookup);

                }
            }
            return Json(titleList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
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
