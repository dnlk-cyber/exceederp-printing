using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Need;
using ExceedERP.Core.Domain.Procurement.Plan;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.NeedAndPlan
{
    
    public class ProcurementPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var periods = db.GLPeriods;
            var branches = db.Branch;
            var costCenter = db.BranchCostCenter;
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
                    Name = i.Branch.Name,
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


        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult Plans_Read([DataSourceRequest]DataSourceRequest request)
        {
            var branch = UserHelper.GetUserDefaultBranch(User.Identity.Name);
            IQueryable<ProcurementPlan> plans = db.ProcurementPlans;//.Where(x => x.OrgBranchName == branch);
            DataSourceResult result = plans.ToDataSourceResult(request, plan => new
            {
                ProcurementPlanId = plan.ProcurementPlanId,
                Name = plan.Name,
                BranchId = plan.BranchId,
                GlFiscalYearId = plan.GlFiscalYearId,
                Remark = plan.Remark,
                Status = plan.Status,

                IsSendForApproval = plan.IsSendForApproval,
                SendForApprovalBy = plan.SendForApprovalBy,
                SendForApprovalTime = plan.SendForApprovalTime,

                IsOnlineApproved = plan.IsOnlineApproved,
                OnlineApprovedBy = plan.OnlineApprovedBy,
                OnlineApprovedTime = plan.OnlineApprovedTime,
                isVoid = plan.isVoid,
                VoidBy = plan.VoidBy,
                VoidTime = plan.VoidTime,

                OrgBranchName = plan.OrgBranchName,
                DateCreated = plan.DateCreated,
                LastModified = plan.LastModified,
                CreatedBy = plan.CreatedBy,
                ModifiedBy = plan.ModifiedBy
            });

            return Json(result);
        }

        
        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlanItems_Create(int id, ProcurementPlanItem planItem)
        {
            var branch = UserHelper.GetUserDefaultBranch(User.Identity.Name);
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Plan First");
            }

            if (ModelState.IsValid)
            {
                var entity = new ProcurementPlanItem
                {
                    ProcurementPlanId = id,
                    CatCode = planItem.CatCode,
                    CategoryName = planItem.CategoryName,
                    ItmCode = planItem.ItmCode,
                    ItemName = planItem.ItemName,
                    PartNo = planItem.PartNo,
                    Quantity = planItem.Quantity,
                    ItemSpecification = planItem.ItemSpecification,
                    UoM = planItem.UoM,
                    ItemType = planItem.ItemType,
                    BudgetSource = planItem.BudgetSource,
                    ProcurementCategory = planItem.ProcurementCategory,
                    ProcurementMethod = planItem.ProcurementMethod,
                    TenderFloatingTime = planItem.TenderFloatingTime,
                    PurchaseDeliveryTime = planItem.PurchaseDeliveryTime,
                    Remark = planItem.Remark,
                    OrgBranchName = branch,
                    DateCreated = planItem.DateCreated,
                    LastModified = planItem.LastModified,
                    CreatedBy = planItem.CreatedBy,
                    ModifiedBy = planItem.ModifiedBy
                };


                db.ProcurementPlanItems.Add(entity);
                db.SaveChanges();
                planItem.ProcurementPlanItemId = entity.ProcurementPlanItemId;
            }

            return Json(new[] { planItem }.ToDataSourceResult(new DataSourceRequest(), ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Plans_Create([DataSourceRequest]DataSourceRequest request, ProcurementPlan plan)
        {
            #region validation

            var exists = db.ProcurementPlans
                .Where(x => x.BranchId == plan.BranchId)
                .Where(x => x.GlFiscalYearId == plan.GlFiscalYearId).ToList();
            if (exists.Any())
            {
                ModelState.AddModelError("exists", "The plan for the selected fiscal year and branch/project is already available.");
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ProcurementPlan
                {
                    BranchId = plan.BranchId,
                    GlFiscalYearId = plan.GlFiscalYearId,
                    Remark = plan.Remark,
                    Status = plan.Status,
                    IsOnlineApproved = plan.IsOnlineApproved,
                    OnlineApprovedBy = plan.OnlineApprovedBy,
                    OnlineApprovedTime = plan.OnlineApprovedTime,
                    isVoid = plan.isVoid,
                    VoidBy = plan.VoidBy,
                    VoidTime = plan.VoidTime,
                    OrgBranchName = plan.OrgBranchName,
                    DateCreated = plan.DateCreated,
                    LastModified = plan.LastModified,
                    CreatedBy = plan.CreatedBy,
                    ModifiedBy = plan.ModifiedBy
                };

                db.ProcurementPlans.Add(entity);
                db.SaveChanges();
                plan.ProcurementPlanId = entity.ProcurementPlanId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "procurement plan management", "Success", User.Identity.Name, $"created plan {entity.ProcurementPlanId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        

        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Plans_Update([DataSourceRequest]DataSourceRequest request, ProcurementPlan plan)
        {
            #region validation

            var exists = db.ProcurementPlans
                .Where(x => x.ProcurementPlanId != plan.ProcurementPlanId)
                .Where(x => x.BranchId == plan.BranchId)
                .Where(x => x.GlFiscalYearId == plan.GlFiscalYearId).ToList();
            if (exists.Any())
            {
                ModelState.AddModelError("exists", "The plan for the selected fiscal year and branch/project is already available.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ProcurementPlan
                {
                    ProcurementPlanId = plan.ProcurementPlanId,
                    Name = plan.Name,
                    GlFiscalYearId = plan.GlFiscalYearId,
                    BranchId = plan.BranchId,
                    Remark = plan.Remark,
                    Status = plan.Status,
                    IsOnlineApproved = plan.IsOnlineApproved,
                    OnlineApprovedBy = plan.OnlineApprovedBy,
                    OnlineApprovedTime = plan.OnlineApprovedTime,
                    isVoid = plan.isVoid,
                    VoidBy = plan.VoidBy,
                    VoidTime = plan.VoidTime,
                    OrgBranchName = plan.OrgBranchName,
                    DateCreated = plan.DateCreated,
                    LastModified = plan.LastModified,
                    CreatedBy = plan.CreatedBy,
                    ModifiedBy = plan.ModifiedBy
                };

                db.ProcurementPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "procurement plan management", "Success", User.Identity.Name, $"updated plan {entity.ProcurementPlanId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Plans_Destroy([DataSourceRequest]DataSourceRequest request, ProcurementPlan plan)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcurementPlan
                {
                    ProcurementPlanId = plan.ProcurementPlanId,
                    Name = plan.Name,
                    GlFiscalYearId = plan.GlFiscalYearId,

                    Remark = plan.Remark,
                    Status = plan.Status,
                    IsOnlineApproved = plan.IsOnlineApproved,
                    OnlineApprovedBy = plan.OnlineApprovedBy,
                    OnlineApprovedTime = plan.OnlineApprovedTime,
                    isVoid = plan.isVoid,
                    VoidBy = plan.VoidBy,
                    VoidTime = plan.VoidTime,
                    OrgBranchName = plan.OrgBranchName,
                    DateCreated = plan.DateCreated,
                    LastModified = plan.LastModified,
                    CreatedBy = plan.CreatedBy,
                    ModifiedBy = plan.ModifiedBy
                };

                db.ProcurementPlans.Attach(entity);
                db.ProcurementPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = db.ProcurementPlans.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.PlanItems.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.ProcurementPlans.Attach(record);
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

            else if (record.PlanItems.Count == 0)
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




       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
