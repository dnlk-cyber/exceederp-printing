using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
    public class DepartmentNeedAssessmentController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            HRMInputs inputs = new HRMInputs();
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
            GetCourse();
            GetProvider();
            PopulateViewData();
            GetTargetGroup();
            TrainingType();
            return View();
        }

        public JsonResult GetTrainingRequestEmployees()
        {
            var employees = db.Person_Employee
                .Where(employee => employee.IsActive)
                .OrderBy(employee => employee.EmployeeCode)
                .ThenBy(employee => employee.EmployeeId)
                .Select(employee => new
                {
                    employee.EmployeeId,
                    employee.EmployeeCode,
                    employee.FirstName,
                    employee.MiddleName,
                    employee.LastName,
                    employee.FirstNameEnglish,
                    employee.MiddleNameEnglish,
                    employee.LastNameEnglish
                })
                .ToList();

            return Json(employees.Select(employee =>
            {
                var name = $"{employee.FirstName} {employee.MiddleName} {employee.LastName}".Trim();
                if (string.IsNullOrEmpty(name))
                    name = $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}".Trim();

                return new
                {
                    employee.EmployeeId,
                    FullNameAmharic = string.IsNullOrWhiteSpace(employee.EmployeeCode) ? name : employee.EmployeeCode + " - " + name
                };
            }).ToList(), JsonRequestBehavior.AllowGet);
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

            ViewData["TargetGroup"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.TARGET_GROUP).ToList();
            ViewData["Jobtitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;

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
        private bool GetRejectedStatus(int id)
        {
            var summaryId = db.NeedAssessmentSummaryDetails
                              .Where(x => x.AssesmentId == id)
                              .Select(x => x.AssesmentSummaryId)
                              .FirstOrDefault();

            if (summaryId == 0) // No matching detail record found
            {
                return false;
            }

            return db.NeedAssessmentSummary
                     .Where(x => x.NeedAssessmentSummaryId == summaryId)
                     .Select(x => x.IsRejected)
                     .FirstOrDefault();
        }

        //private bool GetRejectedStatus(int id)
        //{
        //    var detail = db.NeedAssessmentSummaryDetails.FirstOrDefault(x => x.AssesmentId == id);
        //    if(detail == null)
        //    {
        //        return false;
        //    }
        //    var summeried = db.NeedAssessmentSummary.Find(detail.AssesmentSummaryId);
        //    if(summeried == null)
        //    {
        //        return false;
        //    }
        //    return summeried.IsRejected;
        //}
        public ActionResult DepartmentNeedAssessment_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            List<NeedAssessment> needAssessments = new List<NeedAssessment>();
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId == empId && x.isCurrent);
            if(placement != null)
            {
                var childDeps = db.MainCompanyStructures.Where(x=>x.Parent == placement.OrgStructureId).Select(x=>x.OrgStructureId);

                needAssessments = db.NeedAssessments.Where(x=>x.OrgStructureId == placement.OrgStructureId || childDeps.Contains(x.OrgStructureId??0)).ToList();
            }
           // IQueryable<NeedAssessment> needassessments = db.NeedAssessments;//.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = needAssessments.ToDataSourceResult(request, needAssessment => new
            {
                AssesmentId = needAssessment.AssesmentId,
                OrgStructureId = needAssessment.OrgStructureId,
                BusinessUnitId = needAssessment.BusinessUnitId,
                NoOfStaff = needAssessment.NoOfStaff,
                TotalDays = needAssessment.TotalDays,
                GlFiscalYearId = needAssessment.GlFiscalYearId,
                RequiredTraining = needAssessment.RequiredTraining,
                IsTransfered = needAssessment.IsTransfered,
                LookupId = needAssessment.LookupId,
                Description = needAssessment.Description,
                Type = needAssessment.Type,
                From = needAssessment.From,
                FromAmharic = needAssessment.FromAmharic,
                To = needAssessment.To,
                ToAmharic = needAssessment.ToAmharic,
                IsSecondSendForApproval = needAssessment.IsSecondSendForApproval,
                IsSendForApproval = needAssessment.IsSendForApproval,
                IsSummarized = needAssessment.IsSummarized,
                IsThirdSendForApproval = needAssessment.IsThirdSendForApproval,
                Check = needAssessment.Check,
                MainCompanyId = needAssessment.MainCompanyId,
                DevelopmentRequired = needAssessment.DevelopmentRequired,
                DevelopmentBy = needAssessment.DevelopmentBy,
                Remark = needAssessment.Remark,
                DateCreated = needAssessment.DateCreated,
                LastModified = needAssessment.LastModified,
                CreatedBy = needAssessment.CreatedBy,
                ModifiedBy = needAssessment.ModifiedBy,
                OrgBranchName = needAssessment.OrgBranchName,
                IsRejected = GetRejectedStatus(needAssessment.AssesmentId),
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentNeedAssessment_Create([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId && x.isCurrent);
            if (placement == null)
            {
                ModelState.AddModelError("", "Please add current placement for this employee");
                return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
            }
            needAssessment.OrgStructureId = placement.OrgStructureId;
            needAssessment.BusinessUnitId = placement.BusinessUnitId;
            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                // Ethiopic date input
                if (!string.IsNullOrWhiteSpace(needAssessment.FromAmharic))
                {
                    needAssessment.From = HrHelper.ParseEthiopicDate(needAssessment.FromAmharic);
                }

                if (!string.IsNullOrWhiteSpace(needAssessment.ToAmharic))
                {
                    needAssessment.To = HrHelper.ParseEthiopicDate(needAssessment.ToAmharic);
                }
            }
            else
            {
                // Gregorian date input, convert to Amharic
                if (needAssessment.From.HasValue)
                {
                    var from = needAssessment.From.Value;
                    needAssessment.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(from.Day, from.Month, from.Year);
                }

                if (needAssessment.To.HasValue)
                {
                    var to = needAssessment.To.Value;
                    needAssessment.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(to.Day, to.Month, to.Year);
                }
            }


            if (needAssessment.NoOfStaff <= 0)
            {
                ModelState.AddModelError("", "No of staff should be greater than zero");
                return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));

            }
           
            needAssessment.DateCreated = DateTime.Now;
            needAssessment.CreatedBy = User.Identity.Name;
            needAssessment.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(needAssessment);
                db.NeedAssessments.Add(entity);
                db.SaveChanges();
                needAssessment.AssesmentId = entity.AssesmentId;

                #region operation log

                var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
                if (name != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Need Assessment ", "Success", User.Identity.Name, name.EmployeeCode + " add new Need Assessment adding date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                }

                #endregion
            }

            return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentNeedAssessment_Update([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId && x.isCurrent);
            if (placement == null)
            {
                ModelState.AddModelError("", "Please add current placement for this employee");
                return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
            }
            needAssessment.OrgStructureId = placement.OrgStructureId;
            needAssessment.BusinessUnitId = placement.BusinessUnitId;
            bool datePreference = HrHelper.GetDatePreference();
            if (datePreference)
            {
                // Ethiopic date input
                if (!string.IsNullOrWhiteSpace(needAssessment.FromAmharic))
                {
                    needAssessment.From = HrHelper.ParseEthiopicDate(needAssessment.FromAmharic);
                }

                if (!string.IsNullOrWhiteSpace(needAssessment.ToAmharic))
                {
                    needAssessment.To = HrHelper.ParseEthiopicDate(needAssessment.ToAmharic);
                }
            }
            else
            {
                // Gregorian date input, convert to Amharic
                if (needAssessment.From.HasValue)
                {
                    var from = needAssessment.From.Value;
                    needAssessment.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(from.Day, from.Month, from.Year);
                }

                if (needAssessment.To.HasValue)
                {
                    var to = needAssessment.To.Value;
                    needAssessment.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(to.Day, to.Month, to.Year);
                }
            }


            if (needAssessment.NoOfStaff <= 0)
            {
                ModelState.AddModelError("", "No of staff should be greater than zero");
                return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));

            }
           
            needAssessment.LastModified=DateTime.UtcNow;
            needAssessment.ModifiedBy = User.Identity.Name;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(needAssessment);
                db.NeedAssessments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                if (name != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Need Assessment ", "Success", User.Identity.Name, name.EmployeeCode + " update Need Assessment Info updated date " + DateTime.Today.ToShortDateString(), Modules.HRM);

                }

                #endregion
            }

            return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentNeedAssessment_Destroy([DataSourceRequest]DataSourceRequest request, NeedAssessment needAssessment)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity =GetMappedObject(needAssessment);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted need assessment information of course " + needAssessment.LookupId);
                #endregion
                db.NeedAssessments.Attach(entity);
                db.NeedAssessments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { needAssessment }.ToDataSourceResult(request, ModelState));
        }

        private NeedAssessment GetMappedObject(NeedAssessment needAssessment)
        {
            var entity = new NeedAssessment
            {
                AssesmentId = needAssessment.AssesmentId,
                OrgStructureId = needAssessment.OrgStructureId,
                BusinessUnitId = needAssessment.BusinessUnitId,
                NoOfStaff = needAssessment.NoOfStaff,
                TotalDays = needAssessment.TotalDays,
                GlFiscalYearId = needAssessment.GlFiscalYearId,
                RequiredTraining = needAssessment.RequiredTraining,
                IsTransfered = needAssessment.IsTransfered,
                LookupId = needAssessment.LookupId,
                Description = needAssessment.Description,
                Type = needAssessment.Type,
                From = needAssessment.From,
                FromAmharic = needAssessment.FromAmharic,
                To = needAssessment.To,
                ToAmharic = needAssessment.ToAmharic,
                IsSecondSendForApproval = needAssessment.IsSecondSendForApproval,
                IsSendForApproval = needAssessment.IsSendForApproval,
                IsSummarized = needAssessment.IsSummarized,
                IsThirdSendForApproval = needAssessment.IsThirdSendForApproval,
                Check = needAssessment.Check,
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
            return entity;
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
                        trainingPlan.WorkUnit = check.OrgStructureId??0;
                        //trainingPlan.WorkUnit = check.BusinessUnitId;
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
        public JsonResult SendAssessmentForApproval(int id)
        {
            var assessment = db.NeedAssessments.FirstOrDefault(x => x.AssesmentId == id);

            if (assessment == null)
            {
                return Json(new
                {
                    Message = "The specified assessment record was not found.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }

            if (assessment.IsSendForApproval)
            {
                return Json(new
                {
                    Message = "This assessment has already been submitted for approval.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }

            var trainingNeedRequests = db.TrainingNeedRequests.Where(x => x.AssesmentId == id);

            //if (!trainingNeedRequests.Any())
            //{
            //    return Json(new
            //    {
            //        Message = "There are no pending training requests associated with this assessment.",
            //        Status = "Not-Ok"
            //    }, JsonRequestBehavior.AllowGet);
            //}

            //if (trainingNeedRequests.Count() < assessment.NoOfStaff)
            //{
            //    return Json(new
            //    {
            //        Message = $"The number of employees added is insufficient. You must add exactly {assessment.NoOfStaff} employees.",
            //        Status = "Not-Ok"
            //    }, JsonRequestBehavior.AllowGet);
            //}

            if (trainingNeedRequests.Count() > assessment.NoOfStaff)
            {
                return Json(new
                {
                    Message = $"The number of employees added exceeds the required amount. Please ensure the total is exactly {assessment.NoOfStaff}.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }

            assessment.IsSendForApproval = true;
            db.Entry(assessment).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new
            {
                Message = "The assessment has been successfully submitted for approval.",
                Status = "Ok"
            }, JsonRequestBehavior.AllowGet);
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
