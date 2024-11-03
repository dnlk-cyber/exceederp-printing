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

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    public class TrainingAssessmentController : BaseController
    {
        // GET: HRM/TrainingAssessment
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            GetCourse();
            GetProvider();
            PopulateViewData();
            TrainingType();
            return View();
        }
        private void PopulateViewData()
        {
            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;
            GetDepartments();
            var branch = db.Branch.ToList();
            ViewData["Branches"] = branch;
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList;
            var depo = db.MainCompanyStructures.ToList();
            ViewData["Depo"] = depo;
            ViewData["TargetGroup"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.TARGET_GROUP);
        }
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.Where(x => x.Parent == 0).FirstOrDefault();
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
            ViewData["ProviderList"] = providerlist != null ? providerlist.ToList() : new List<MainCompany>();
            return Json(providerlist.Select(p => new { MainCompanyId = p.MainCompanyId, TradeName = p.TradeName }), JsonRequestBehavior.AllowGet);
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
      
        public ActionResult TrainingAssessments_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<NeedAssessment> needassessments = db.NeedAssessments.Where(x=>x.IsSendForApproval &&x.IsTransfered);//.Where(x => mybraches.Contains(x.OrgBranchName)&&x.IsTransfered==false);
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
                IsSecondSendForApproval = needAssessment.IsSecondSendForApproval,
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingAssessments_Create([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new NeedAssessment
                {
                    OrgStructureId = needAssessment.OrgStructureId,


                    NoOfStaff = needAssessment.NoOfStaff,
                    RequiredTraining = needAssessment.RequiredTraining,
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
                //var needAssessmentSummary = needAssessments.Where(x => x.Check)
                //    .GroupBy(x => new
                //    {
                //        x.GlFiscalYearId,
                //        x.LookupId,
                //    }).Select(g => new NeedAssessmentSummary
                //    {
                //        GlFiscalYearId = g.Key.GlFiscalYearId,
                //        LookupId = g.Key.LookupId,
                //        NoOfStaff = g.Sum(s => s.NoOfStaff),
                //        RequiredTraining = 0,
                //        CostPerPerson = 0,
                //        IsTransfered = true,
                //        IsSendForApproval = true,
                //        IsSecondSendForApproval = true,
                //        Check = true,
                //        DateCreated = DateTime.Now.Date,
                //        CreatedBy = User.Identity.Name,
                //        LastModified=null,
                //        ModifiedBy = null

                //    }).ToList();
                //db.NeedAssessmentSummary.AddRange(needAssessmentSummary);
                //db.SaveChanges();
               
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
                            assessment.NoOfStaff = check.NoOfStaff;
                            assessment.IsSendForApproval = true;

                            db.Entry(assessment).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                    }

                }
                
            }

            return Json(planList.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingAssessments_Update([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new NeedAssessment
                {
                    AssesmentId = needAssessment.AssesmentId,
                    OrgStructureId = needAssessment.OrgStructureId,
                    NoOfStaff = needAssessment.NoOfStaff,
                    RequiredTraining = needAssessment.RequiredTraining,
                    GlFiscalYearId=needAssessment.GlFiscalYearId,
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
            }

            return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
        }
    
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingAssessments_Destroy([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new NeedAssessment
                {
                    AssesmentId = needAssessment.AssesmentId,
                    OrgStructureId = needAssessment.OrgStructureId,
                    GlFiscalYearId=needAssessment.GlFiscalYearId,
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
        public JsonResult RejectRequest(int id)
        {            
            var assessment = db.NeedAssessments.FirstOrDefault(x => x.AssesmentId == id);
            
            if (assessment != null)
            {
                assessment.IsSendForApproval = false;

                db.Entry(assessment).State = EntityState.Modified;
                db.SaveChanges();

                var messageStatus1 = new
                {
                    Message = "Successfully Rejected",
                    Status = "Ok"
                };

                return Json(messageStatus1, JsonRequestBehavior.AllowGet);
            }

            var messageStatus2 = new
            {
                Message = "Can't reject",
                Status = "Fail"
            };

            return Json(messageStatus2);

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
