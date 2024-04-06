using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.HRMPlan;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    public class TrainingNeedRequestsController : BaseController
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
      
        public ActionResult TrainingNeedRequests_Read([DataSourceRequest]DataSourceRequest request,int aId)
        {
            IQueryable<TrainingNeedRequest> trainingNeedRequests = db.TrainingNeedRequests.Where(x=>x.AssesmentId == aId);//.Where(x => mybraches.Contains(x.OrgBranchName)&&x.IsTransfered==false);
            DataSourceResult result = trainingNeedRequests.ToDataSourceResult(request, trainingNeedRequest => new
            {
                TrainingNeedRequestId = trainingNeedRequest.TrainingNeedRequestId,
                AssesmentId = trainingNeedRequest.AssesmentId,
                EmployeeId = trainingNeedRequest.EmployeeId,
                OrgStructureId = trainingNeedRequest.OrgStructureId,
                GlFiscalYearId = trainingNeedRequest.GlFiscalYearId,
                LookupId = trainingNeedRequest.LookupId,
                Prerequest = trainingNeedRequest.Prerequest,
                EmployeePerformanceGap = trainingNeedRequest.EmployeePerformanceGap,
                TrainingNeedForEmployeePerformanceGap = trainingNeedRequest.TrainingNeedForEmployeePerformanceGap,
                TheNeedForTrainingRelatedToInistitution = trainingNeedRequest.TheNeedForTrainingRelatedToInistitution,
                ExpectedResultFromTraining = trainingNeedRequest.ExpectedResultFromTraining,
               
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingNeedRequests_Create([DataSourceRequest]DataSourceRequest request, TrainingNeedRequest trainingNeedRequest, int assesId)
        {
            ModelState.Clear();
            var needassessment = db.NeedAssessments.Find(assesId);
            if(needassessment != null)
            {
                trainingNeedRequest.OrgStructureId = needassessment.OrgStructureId;
                trainingNeedRequest.RequiredTraining = needassessment.RequiredTraining;
                trainingNeedRequest.LookupId = needassessment.LookupId;
                trainingNeedRequest.GlFiscalYearId = needassessment.GlFiscalYearId;
            }
            var exists = db.TrainingNeedRequests.Where(x => x.AssesmentId == assesId && x.EmployeeId == trainingNeedRequest.EmployeeId).Any();
            if (exists)
            {
                ModelState.AddModelError("exist", "This Employee is Registered. ");

            }
            var assessment = db.NeedAssessments.Find(assesId);
            if(assessment != null)
            {
                var trainee = db.TrainingNeedRequests.Where(x => x.AssesmentId == assesId).Select(x => x.EmployeeId).Count();
                if(trainee>=assessment.NoOfStaff )
                {
                    ModelState.AddModelError("exist", "the number of Employee can't exceed " + assessment.NoOfStaff);
                }
            }
            if (ModelState.IsValid)
            {
                trainingNeedRequest.AssesmentId = assesId;
                var entity = GetMappedObject(trainingNeedRequest);
                db.TrainingNeedRequests.Add(entity);
                db.SaveChanges();
                trainingNeedRequest.TrainingNeedRequestId = entity.TrainingNeedRequestId;
            }

            return Json(new[] { trainingNeedRequest }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingNeedRequests_Update([DataSourceRequest]DataSourceRequest request, TrainingNeedRequest trainingNeedRequest)
        {
            ModelState.Clear();
            var needassessment = db.NeedAssessments.Find(trainingNeedRequest.AssesmentId);
            if (needassessment != null)
            {
                trainingNeedRequest.OrgStructureId = needassessment.OrgStructureId;
                trainingNeedRequest.RequiredTraining = needassessment.RequiredTraining;
                trainingNeedRequest.LookupId = needassessment.LookupId;
                trainingNeedRequest.GlFiscalYearId = needassessment.GlFiscalYearId;
            }
            var exists = db.TrainingNeedRequests.Where(x => x.AssesmentId == trainingNeedRequest.AssesmentId && x.EmployeeId == trainingNeedRequest.EmployeeId&&trainingNeedRequest.TrainingNeedRequestId !=trainingNeedRequest.TrainingNeedRequestId).Any();
            if (exists)
            {
                ModelState.AddModelError("exist", "This Employee is registered. ");

            }
            var assessment = db.NeedAssessments.Find(trainingNeedRequest.AssesmentId);
            if (assessment != null)
            {
                var trainee = db.TrainingNeedRequests.Where(x => x.AssesmentId == trainingNeedRequest.AssesmentId).Select(x => x.EmployeeId).Count();
                if (trainee >= assessment.NoOfStaff)
                {
                    ModelState.AddModelError("exist", "The number of Employee can't Exceed " + assessment.NoOfStaff);
                }
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(trainingNeedRequest);
                db.TrainingNeedRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { trainingNeedRequest }.ToDataSourceResult(request, ModelState));
        }
    
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingNeedRequests_Destroy([DataSourceRequest]DataSourceRequest request, TrainingNeedRequest trainingNeedRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(trainingNeedRequest);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted need assessment information of course " + trainingNeedRequest.EmployeeId);
                #endregion
                db.TrainingNeedRequests.Attach(entity);
                db.TrainingNeedRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainingNeedRequest }.ToDataSourceResult(request, ModelState));
        }
        private TrainingNeedRequest GetMappedObject(TrainingNeedRequest trainingNeedRequest)
        {
            var entity = new TrainingNeedRequest
            {
                TrainingNeedRequestId = trainingNeedRequest.TrainingNeedRequestId,
                AssesmentId = trainingNeedRequest.AssesmentId,
                EmployeeId = trainingNeedRequest.EmployeeId,
                OrgStructureId = trainingNeedRequest.OrgStructureId,
                GlFiscalYearId = trainingNeedRequest.GlFiscalYearId,
                LookupId = trainingNeedRequest.LookupId,
                Prerequest = trainingNeedRequest.Prerequest,
                EmployeePerformanceGap = trainingNeedRequest.EmployeePerformanceGap,
                TrainingNeedForEmployeePerformanceGap = trainingNeedRequest.TrainingNeedForEmployeePerformanceGap,
                TheNeedForTrainingRelatedToInistitution = trainingNeedRequest.TheNeedForTrainingRelatedToInistitution,
                ExpectedResultFromTraining = trainingNeedRequest.ExpectedResultFromTraining,

            };
            return entity;
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
