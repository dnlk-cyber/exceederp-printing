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
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Core.Domain.Health;

namespace ExceedERP.Web.Areas.Health.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin, HrmConstants.HRDepartmentHead)]
    public class MedicalRequestStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private string ReferredDateAm { get; set; }
        private DateTime ReferredDate { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            GetDepartments();
            GetStatusType();
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);

            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
            ViewData["Department"] = db.MainCompanyStructures.ToList();
        }
        public void SetDate(string name, string value)
        {
            if (name == "ReferredDate")
            {
                ReferredDateAm = value;
                var splitDate = ReferredDateAm.Split('/');
                ReferredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            if (name == "ReferredDate")
            {
                ReferredDateAm = value;
                var splitDate = ReferredDateAm.Split('/');
                ReferredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

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
            ViewData["OrgStructureId"] = allDepartment;
        }
        public JsonResult GetEmployees(int OrgStructureId)
        {
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var palcment = db.Placements.Where(x => x.Department == OrgStructureId).ToList();
            if (palcment.Any())
            {
                foreach (var p in palcment)
                {
                    var employeeList = db.Person_Employee.Where(x => x.EmployeeId == p.EmployeeId == x.IsActive).AsQueryable();




                    foreach (var emp in employeeList)
                    {
                        EmployeeIdView = new EmployeeIdView();
                        EmployeeIdView.PersonId = emp.EmployeeId;
                        EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                        EmployeeIdViews.Add(EmployeeIdView);
                    }
                }
            }
            ViewData["EmployeeList"] = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return Json(EmployeeIdViews.Select(p => new { PersonId = p.PersonId, FullName = p.FullName }), JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult MedicalRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MedicalRequest> medicalrequest = db.MedicalRequest;

            List<MedicalRequest> medicals = new List<MedicalRequest>();
            foreach (var m in medicalrequest)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => x.Status == RequestStatus.WAITING_FOR_APPROVAL && x.RequestId == m.MedicalRequestId && x.ReferenceType == "Medical").FirstOrDefault();
                if (status != null)
                {
                    medicals.Add(m);
                }

            }

            medicalrequest = medicals.AsQueryable().OrderBy(x => x.Date);
            DataSourceResult result = medicalrequest.ToDataSourceResult(request, medicalRequest => new
            {
                MedicalRequestId = medicalRequest.MedicalRequestId,
                EmployeeId = medicalRequest.EmployeeId,
                Date = medicalRequest.Date,
                OrgStructureId = medicalRequest.OrgStructureId,
                Remark = medicalRequest.Remark,
                DateAmharic = medicalRequest.DateAmharic,
                EmployeeCode = medicalRequest.EmployeeCode,
                DepartmentSendForApprovalBy = medicalRequest.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = medicalRequest.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = medicalRequest.IsDepartmentSendForApproval,
                DepartmentOnlineApprovedBy = medicalRequest.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = medicalRequest.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = medicalRequest.IsDepartmentOnlineApproved,
                OnlineApprovedBy = medicalRequest.OnlineApprovedBy,
                OnlineApprovedTime = medicalRequest.OnlineApprovedTime,
                IsOnlineApproved = medicalRequest.IsOnlineApproved,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalRequest_Create([DataSourceRequest]DataSourceRequest request, MedicalRequest medicalRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new MedicalRequest
                {
                    EmployeeId = medicalRequest.EmployeeId,
                    Date = medicalRequest.Date,
                    OrgStructureId = medicalRequest.OrgStructureId,
                    Remark = medicalRequest.Remark
                };

                db.MedicalRequest.Add(entity);
                db.SaveChanges();
                medicalRequest.MedicalRequestId = entity.MedicalRequestId;
            }

            return Json(new[] { medicalRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalRequest_Update([DataSourceRequest]DataSourceRequest request, MedicalRequest medicalRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new MedicalRequest
                {
                    MedicalRequestId = medicalRequest.MedicalRequestId,
                    EmployeeId = medicalRequest.EmployeeId,
                    Date = medicalRequest.Date,
                    OrgStructureId = medicalRequest.OrgStructureId,
                    Remark = medicalRequest.Remark
                };

                db.MedicalRequest.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { medicalRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MedicalRequest_Destroy([DataSourceRequest]DataSourceRequest request, MedicalRequest medicalRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new MedicalRequest
                {
                    MedicalRequestId = medicalRequest.MedicalRequestId,
                    EmployeeId = medicalRequest.EmployeeId,
                    Date = medicalRequest.Date,
                    OrgStructureId = medicalRequest.OrgStructureId,
                    Remark = medicalRequest.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted medical request of employee code  " + medicalRequest.EmployeeCode);
                #endregion
                db.MedicalRequest.Attach(entity);
                db.MedicalRequest.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { medicalRequest }.ToDataSourceResult(request, ModelState));
        }
        public string CheckStatus(int masterId)
        {
            var reques = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Medical").FirstOrDefault();
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Medical" && x.Status == RequestStatus.APPROVE).FirstOrDefault();
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "Medical" && x.Status == RequestStatus.DEPARTMENT_APPROVAL).FirstOrDefault();
                if (deptApproval != null)
                {
                    return "Dapproved";
                }
                return "true";
            }
            return "false";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
