using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Need;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.SelfCare
{
    public class ProcurementNeedSelfServiceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover, ProcurementRoles.ProcurementApprover, ProcurementRoles.ProcurementUser)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var periods = db.GLFiscalYears;
            var categories = db.ItemCategorys;
          
            var branches = db.Branch;
            var costCenter = db.MainCompanyStructures;
            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstName + " " + i.MiddleName + " " + i.LastName,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;

            ViewData["Periods"] = periods;
            ViewData["Branches"] = branches;

            var items = db.AnnualNeedAssesmentItems;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItmCode + "-" + i.ItemName,
                    ItmCode = i.ItmCode
                }).AsQueryable();

            ViewData["Items"] = selecteditems;

            var selectedcatedories = items.Select(
                i => new
                {
                    Name = i.CatCode + "-" + i.CategoryName,
                    CatCode = i.CatCode
                }).AsQueryable();
            ViewData["Category"] = selectedcatedories;
            ViewData["defaultCategory"] = selectedcatedories.FirstOrDefault();
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover, ProcurementRoles.ProcurementApprover, ProcurementRoles.ProcurementUser)]
        public ActionResult AnnualNeedAssesments_Read([DataSourceRequest]DataSourceRequest request)
        {
            var branch = UserHelper.GetUserDefaultBranch(User.Identity.Name);
            int userworkunit = UserInformationHelper.GetUserReportsTo(User.Identity.Name);
            

            var annualneedassesments = db.AnnualNeedAssesments.AsQueryable();

            if (System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.UserDepartment)
                && System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.PropertyRequistion))
            {
                annualneedassesments = annualneedassesments.Where(x => x.OrgStructureId == userworkunit && userworkunit != 0
                                                                     || x.CreatedBy == User.Identity.Name).AsQueryable();
            }

            else if (System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.UserDepartment))
            {
                annualneedassesments = annualneedassesments.Where(x => x.OrgStructureId == userworkunit && userworkunit != 0
                                                                     || x.CreatedBy == User.Identity.Name).AsQueryable();
            }
            else if (System.Web.HttpContext.Current.User.IsInRole(InventoryConstants.PropertyRequistion))
            {
                annualneedassesments = annualneedassesments.Where(x => x.CreatedBy == User.Identity.Name).AsQueryable();
            }

            //IQueryable<AnnualNeedAssesment> annualneedassesments = db.AnnualNeedAssesments;//.Where(x => x.Branch == branch);
            DataSourceResult result = annualneedassesments.ToDataSourceResult(request, annualNeedAssesment => new
            {
                AnnualNeedAssesmentID = annualNeedAssesment.AnnualNeedAssesmentID,
                OrgStructureId = annualNeedAssesment.OrgStructureId,
                BranchId = annualNeedAssesment.BranchId,
                EmployeeId = annualNeedAssesment.EmployeeId,
                Remark = annualNeedAssesment.Remark,
                GlFiscalYearId = annualNeedAssesment.GlFiscalYearId,

                Status = annualNeedAssesment.Status,
                IsOnlineApproved = annualNeedAssesment.IsOnlineApproved,
                OnlineApprovedBy = annualNeedAssesment.OnlineApprovedBy,
                OnlineApprovedTime = annualNeedAssesment.OnlineApprovedTime,
                IsSendForApproval = annualNeedAssesment.IsSendForApproval,
                SendForApprovalBy = annualNeedAssesment.SendForApprovalBy,
                SendForApprovalTime = annualNeedAssesment.SendForApprovalTime,
                isVoid = annualNeedAssesment.isVoid,
                VoidBy = annualNeedAssesment.VoidBy,
                VoidTime = annualNeedAssesment.VoidTime,
                OrgBranchName = annualNeedAssesment.OrgBranchName,
                DateCreated = annualNeedAssesment.DateCreated,
                LastModified = annualNeedAssesment.LastModified,
                CreatedBy = annualNeedAssesment.CreatedBy,
                ModifiedBy = annualNeedAssesment.ModifiedBy
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesments_Create([DataSourceRequest]DataSourceRequest request, AnnualNeedAssesment annualNeedAssesment)
        {
            #region validation

            var exists = db.AnnualNeedAssesments
                .Where(x => x.OrgStructureId == annualNeedAssesment.OrgStructureId)
                .Where(x => x.BranchId == annualNeedAssesment.BranchId)
                .Where(x => x.GlFiscalYearId == annualNeedAssesment.GlFiscalYearId).ToList();
            if (exists.Any())
            {
                ModelState.AddModelError("exists", "The need for this branch/project,work unit and work unit is already available.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesment
                {
                    OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(annualNeedAssesment.EmployeeId),
                    BranchId =EmployeeHelper.GetEmployeePlacementBranch(annualNeedAssesment.EmployeeId),
                    EmployeeId = annualNeedAssesment.EmployeeId,
                    GlFiscalYearId = annualNeedAssesment.GlFiscalYearId,
                    Remark = annualNeedAssesment.Remark,
                    Status = "Editing",
                    IsOnlineApproved = annualNeedAssesment.IsOnlineApproved,
                    OnlineApprovedBy = annualNeedAssesment.OnlineApprovedBy,
                    OnlineApprovedTime = annualNeedAssesment.OnlineApprovedTime,
                    isVoid = annualNeedAssesment.isVoid,
                    VoidBy = annualNeedAssesment.VoidBy,
                    VoidTime = annualNeedAssesment.VoidTime,
                    OrgBranchName = EmployeeHelper.GetEmployeePlacementBranch(annualNeedAssesment.EmployeeId).ToString(),
                    DateCreated = DateTime.Now,
                    LastModified = annualNeedAssesment.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = annualNeedAssesment.ModifiedBy
                };

                db.AnnualNeedAssesments.Add(entity);
                db.SaveChanges();
                annualNeedAssesment.AnnualNeedAssesmentID = entity.AnnualNeedAssesmentID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "procurement need self care", "Success", User.Identity.Name, $"created need {entity.AnnualNeedAssesmentID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { annualNeedAssesment }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesments_Update([DataSourceRequest]DataSourceRequest request, AnnualNeedAssesment annualNeedAssesment)
        {
            #region validation

            var exists = db.AnnualNeedAssesments.Where(x => x.AnnualNeedAssesmentID != annualNeedAssesment.AnnualNeedAssesmentID)
                                                .Where(x => x.OrgStructureId == annualNeedAssesment.OrgStructureId)
                                                .Where(x => x.BranchId == annualNeedAssesment.BranchId)
                                                .Where(x => x.GlFiscalYearId == annualNeedAssesment.GlFiscalYearId).ToList();
            if (exists.Any())
            {
                ModelState.AddModelError("exists", "The need for this branch/project,work unit and work unit is already available.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesment
                {
                    AnnualNeedAssesmentID = annualNeedAssesment.AnnualNeedAssesmentID,
                    OrgStructureId = UserInformationHelper.GetEmployeeWorkUnitById(annualNeedAssesment.EmployeeId),
                    BranchId = EmployeeHelper.GetEmployeePlacementBranch(annualNeedAssesment.EmployeeId),
                    EmployeeId = annualNeedAssesment.EmployeeId,
                    GlFiscalYearId = annualNeedAssesment.GlFiscalYearId,
                    Remark = annualNeedAssesment.Remark,
                    Status = annualNeedAssesment.Status,
                    IsOnlineApproved = annualNeedAssesment.IsOnlineApproved,
                    OnlineApprovedBy = annualNeedAssesment.OnlineApprovedBy,
                    OnlineApprovedTime = annualNeedAssesment.OnlineApprovedTime,
                    isVoid = annualNeedAssesment.isVoid,
                    VoidBy = annualNeedAssesment.VoidBy,
                    VoidTime = annualNeedAssesment.VoidTime,
                    OrgBranchName = EmployeeHelper.GetEmployeePlacementBranch(annualNeedAssesment.EmployeeId).ToString(),

                    DateCreated = annualNeedAssesment.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = annualNeedAssesment.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.AnnualNeedAssesments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "procurement need self care", "Success", User.Identity.Name, $"updated need {entity.AnnualNeedAssesmentID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { annualNeedAssesment }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualNeedAssesments_Destroy([DataSourceRequest]DataSourceRequest request, AnnualNeedAssesment annualNeedAssesment)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnnualNeedAssesment
                {
                    AnnualNeedAssesmentID = annualNeedAssesment.AnnualNeedAssesmentID,
                    OrgStructureId = annualNeedAssesment.OrgStructureId,
                    BranchId = annualNeedAssesment.BranchId,
                    EmployeeId = annualNeedAssesment.EmployeeId,
                    GlFiscalYearId = annualNeedAssesment.GlFiscalYearId,
                    Remark = annualNeedAssesment.Remark,
                    Status = annualNeedAssesment.Status,
                    IsOnlineApproved = annualNeedAssesment.IsOnlineApproved,
                    OnlineApprovedBy = annualNeedAssesment.OnlineApprovedBy,
                    OnlineApprovedTime = annualNeedAssesment.OnlineApprovedTime,
                    isVoid = annualNeedAssesment.isVoid,
                    VoidBy = annualNeedAssesment.VoidBy,
                    VoidTime = annualNeedAssesment.VoidTime,


                    OrgBranchName = annualNeedAssesment.OrgBranchName,
                    DateCreated = annualNeedAssesment.DateCreated,
                    LastModified = annualNeedAssesment.LastModified,
                    CreatedBy = annualNeedAssesment.CreatedBy,
                    ModifiedBy = annualNeedAssesment.ModifiedBy
                };

                db.AnnualNeedAssesments.Attach(entity);
                db.AnnualNeedAssesments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { annualNeedAssesment }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetPeriods()
        {
            var periods = db.GLPeriods.ToList();
            return Json(periods.Select(x => new { Name = x.Name + "_" + x.GlFiscalYear.Name, Glper = x.GlFiscalYearId }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCostCenter()
        {
            var costCenter = db.DepartmentCostCenters;
            return Json(costCenter.Select(i => new { Name = i.LocationCode + "-" + i.DepartmentName, code = i.LocationCode }),
             JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.AnnualNeedAssesments.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.AnnualNeedAssesmentItem.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.AnnualNeedAssesments.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.AnnualNeedAssesmentItem.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
