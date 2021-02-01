using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HRSelfServiceManager, HrmConstants.HRTrainingApprover)]
    public class TrainingApplicantStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {

            GetCourse();
            GetDepartments();
            GetStatusType();
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            var depo = db.MainCompanyStructures.ToList();
            ViewData["Depo"] = depo;

            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(RequestStatus))
                .Cast<RequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
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
        public ActionResult TrainingApplicants_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TrainingApplicant> trainingapplicants = db.TrainingApplicants.OrderBy(x => x.Date);

            List<TrainingApplicant> requests = new List<TrainingApplicant>();
            foreach (var m in trainingapplicants)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => x.Status == RequestStatus.WAITING_FOR_APPROVAL && x.RequestId == m.TrainingApplicantId && x.ReferenceType == "Training").FirstOrDefault();
                if (status != null)
                {
                    requests.Add(m);
                }

            }

            trainingapplicants = requests.AsQueryable().OrderBy(x => x.Date);
            DataSourceResult result = trainingapplicants.ToDataSourceResult(request, trainingApplicant => new
            {
                TrainingApplicantId = trainingApplicant.TrainingApplicantId,
                OrgStructureId = trainingApplicant.OrgStructureId,
                LookupId = trainingApplicant.LookupId,
                Remark = trainingApplicant.Remark,
                Date = trainingApplicant.Date,
                DepartmentSendForApprovalBy = trainingApplicant.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = trainingApplicant.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = trainingApplicant.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = trainingApplicant.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = trainingApplicant.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = trainingApplicant.IsDepartmentOnlineApproved,
                OnlineApprovedBy = trainingApplicant.OnlineApprovedBy,
                OnlineApprovedTime = trainingApplicant.OnlineApprovedTime,
                IsOnlineApproved = trainingApplicant.IsOnlineApproved,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicants_Create([DataSourceRequest]DataSourceRequest request, TrainingApplicant trainingApplicant)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingApplicant
                {
                    OrgStructureId = trainingApplicant.OrgStructureId,
                    LookupId = trainingApplicant.LookupId,
                    Remark = trainingApplicant.Remark,
                    Date = trainingApplicant.Date,

                };

                db.TrainingApplicants.Add(entity);
                db.SaveChanges();
                trainingApplicant.TrainingApplicantId = entity.TrainingApplicantId;
            }

            return Json(new[] { trainingApplicant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicants_Update([DataSourceRequest]DataSourceRequest request, TrainingApplicant trainingApplicant)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingApplicant
                {
                    TrainingApplicantId = trainingApplicant.TrainingApplicantId,
                    OrgStructureId = trainingApplicant.OrgStructureId,
                    LookupId = trainingApplicant.LookupId,
                    Remark = trainingApplicant.Remark,
                    Date = trainingApplicant.Date,

                };

                db.TrainingApplicants.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { trainingApplicant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingApplicants_Destroy([DataSourceRequest]DataSourceRequest request, TrainingApplicant trainingApplicant)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingApplicant
                {
                    TrainingApplicantId = trainingApplicant.TrainingApplicantId,
                    OrgStructureId = trainingApplicant.OrgStructureId,
                    LookupId = trainingApplicant.LookupId,
                    Remark = trainingApplicant.Remark,
                    Date = trainingApplicant.Date,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted training applicant information of work unit " + trainingApplicant.OrgStructureId);
                #endregion
                db.TrainingApplicants.Attach(entity);
                db.TrainingApplicants.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainingApplicant }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
