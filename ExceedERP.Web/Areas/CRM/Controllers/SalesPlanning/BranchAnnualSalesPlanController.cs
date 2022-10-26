using System;
//using System.Data
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers.SalesPlanning
{
    [AuthorizeRoles(CRMRoles.SalesPlanApprover, CRMRoles.SalesPlanUser)]
    public class BranchAnnualSalesPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BranchAnnualSalesPlans_Read([DataSourceRequest]DataSourceRequest request, int id = 0)
        {
            IQueryable<BranchAnnualSalesPlan> branchannualsalesplans = db.BranchAnnualSalesPlans.Where(a => a.AnnualSalesPlanId == id);
            DataSourceResult result = branchannualsalesplans.ToDataSourceResult(request, branchAnnualSalesPlan => new {
                BranchAnnualSalesPlanId = branchAnnualSalesPlan.BranchAnnualSalesPlanId,
                AnnualSalesPlanId = branchAnnualSalesPlan.AnnualSalesPlanId,
                BranchId = branchAnnualSalesPlan.BranchId,
                Ratio = branchAnnualSalesPlan.Ratio,
                Quantity = branchAnnualSalesPlan.Quantity,
                //Profit = branchAnnualSalesPlan.Profit,
                Value = branchAnnualSalesPlan.Value,
                Status = branchAnnualSalesPlan.Status,
                Remark = branchAnnualSalesPlan.Remark,
                IsVoid = branchAnnualSalesPlan.IsVoid,
                VoidBy = branchAnnualSalesPlan.VoidBy,
                VoidTime = branchAnnualSalesPlan.VoidTime,
                //IsOnlineApproved = branchAnnualSalesPlan.IsOnlineApproved,
                IsOnlineApproved = branchAnnualSalesPlan.AnnualSalesPlan.IsOnlineApproved,
                OnlineApprovedBy = branchAnnualSalesPlan.OnlineApprovedBy,
                OnlineApprovedTime = branchAnnualSalesPlan.OnlineApprovedTime,
                IsOnlineTransferred = branchAnnualSalesPlan.IsOnlineTransferred,
                IsOnlineTransferredBy = branchAnnualSalesPlan.IsOnlineTransferredBy,
                OnlineTransferredTime = branchAnnualSalesPlan.OnlineTransferredTime,
                IsSendForApproval = branchAnnualSalesPlan.IsSendForApproval,
                OrgBranchName = branchAnnualSalesPlan.OrgBranchName,
                SendForApprovalBy = branchAnnualSalesPlan.SendForApprovalBy,
                SendForApprovalTime = branchAnnualSalesPlan.SendForApprovalTime,
                DateCreated = branchAnnualSalesPlan.DateCreated,
                LastModified = branchAnnualSalesPlan.LastModified,
                CreatedBy = branchAnnualSalesPlan.CreatedBy,
                ModifiedBy = branchAnnualSalesPlan.ModifiedBy
            });

            return Json(result);
        }

        private bool DuplicateBranchExists(BranchAnnualSalesPlan branchAnnualSalesPlan, int headerId)
        {
            return db.BranchAnnualSalesPlans.Any(b => b.AnnualSalesPlanId == headerId && b.BranchId == branchAnnualSalesPlan.BranchId && b.BranchAnnualSalesPlanId != branchAnnualSalesPlan.BranchAnnualSalesPlanId);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BranchAnnualSalesPlans_Create([DataSourceRequest]DataSourceRequest request, BranchAnnualSalesPlan branchAnnualSalesPlan, int id)
        {
            if(DuplicateBranchExists(branchAnnualSalesPlan, id))
            {
                ModelState.AddModelError("", "Plan for this branch exists");
            }
            if (ModelState.IsValid)
            {
                var entity = new BranchAnnualSalesPlan
                {
                    AnnualSalesPlanId = id,
                    BranchId = branchAnnualSalesPlan.BranchId,
                    Ratio = branchAnnualSalesPlan.Ratio,
                    Quantity = branchAnnualSalesPlan.Quantity,
                    //Profit = branchAnnualSalesPlan.Profit,
                    Value = branchAnnualSalesPlan.Value,
                    Status = branchAnnualSalesPlan.Status,
                    Remark = branchAnnualSalesPlan.Remark,
                    IsVoid = branchAnnualSalesPlan.IsVoid,
                    VoidBy = branchAnnualSalesPlan.VoidBy,
                    VoidTime = branchAnnualSalesPlan.VoidTime,
                    IsOnlineApproved = branchAnnualSalesPlan.IsOnlineApproved,
                    OnlineApprovedBy = branchAnnualSalesPlan.OnlineApprovedBy,
                    OnlineApprovedTime = branchAnnualSalesPlan.OnlineApprovedTime,
                    IsOnlineTransferred = branchAnnualSalesPlan.IsOnlineTransferred,
                    IsOnlineTransferredBy = branchAnnualSalesPlan.IsOnlineTransferredBy,
                    OnlineTransferredTime = branchAnnualSalesPlan.OnlineTransferredTime,
                    IsSendForApproval = branchAnnualSalesPlan.IsSendForApproval,
                    OrgBranchName = branchAnnualSalesPlan.OrgBranchName,
                    SendForApprovalBy = branchAnnualSalesPlan.SendForApprovalBy,
                    SendForApprovalTime = branchAnnualSalesPlan.SendForApprovalTime,

                    DateCreated = DateTime.Now,
                    LastModified = branchAnnualSalesPlan.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = branchAnnualSalesPlan.ModifiedBy
                };
                var headerRecord = db.AnnualSalesPlans.Find(entity.AnnualSalesPlanId);
                if (headerRecord != null)
                {
                    var branchQty = entity.Ratio / 100 * headerRecord.TotalAnnualPlanQuantity;
                    var bValueWihoutProfit = branchQty * headerRecord.AveragePrice;
                    var bProfit = bValueWihoutProfit * headerRecord.Profit / 100;

                    entity.Quantity = branchQty;
                    entity.Value = bValueWihoutProfit + bProfit;
                }

                db.BranchAnnualSalesPlans.Add(entity);
                db.SaveChanges();
                branchAnnualSalesPlan.BranchAnnualSalesPlanId = entity.BranchAnnualSalesPlanId;
            }

            return Json(new[] { branchAnnualSalesPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BranchAnnualSalesPlans_Update([DataSourceRequest]DataSourceRequest request, BranchAnnualSalesPlan branchAnnualSalesPlan)
        {
            if (DuplicateBranchExists(branchAnnualSalesPlan, branchAnnualSalesPlan.AnnualSalesPlanId))
            {
                ModelState.AddModelError("", "Plan for this branch exists");
            }
            if (ModelState.IsValid)
            {
                var entity = new BranchAnnualSalesPlan
                {
                    BranchAnnualSalesPlanId = branchAnnualSalesPlan.BranchAnnualSalesPlanId,
                    AnnualSalesPlanId = branchAnnualSalesPlan.AnnualSalesPlanId,
                    BranchId = branchAnnualSalesPlan.BranchId,
                    Ratio = branchAnnualSalesPlan.Ratio,
                    Quantity = branchAnnualSalesPlan.Quantity,
                    //Profit = branchAnnualSalesPlan.Profit,
                    Value = branchAnnualSalesPlan.Value,
                    Status = branchAnnualSalesPlan.Status,
                    Remark = branchAnnualSalesPlan.Remark,
                    IsVoid = branchAnnualSalesPlan.IsVoid,
                    VoidBy = branchAnnualSalesPlan.VoidBy,
                    VoidTime = branchAnnualSalesPlan.VoidTime,
                    IsOnlineApproved = branchAnnualSalesPlan.IsOnlineApproved,
                    OnlineApprovedBy = branchAnnualSalesPlan.OnlineApprovedBy,
                    OnlineApprovedTime = branchAnnualSalesPlan.OnlineApprovedTime,
                    IsOnlineTransferred = branchAnnualSalesPlan.IsOnlineTransferred,
                    IsOnlineTransferredBy = branchAnnualSalesPlan.IsOnlineTransferredBy,
                    OnlineTransferredTime = branchAnnualSalesPlan.OnlineTransferredTime,
                    IsSendForApproval = branchAnnualSalesPlan.IsSendForApproval,
                    OrgBranchName = branchAnnualSalesPlan.OrgBranchName,
                    SendForApprovalBy = branchAnnualSalesPlan.SendForApprovalBy,
                    SendForApprovalTime = branchAnnualSalesPlan.SendForApprovalTime,

                    DateCreated = branchAnnualSalesPlan.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = branchAnnualSalesPlan.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                var headerRecord = db.AnnualSalesPlans.Find(entity.AnnualSalesPlanId);
                if(headerRecord != null)
                {
                    var branchQty = entity.Ratio /100 * headerRecord.TotalAnnualPlanQuantity;
                    var bValueWihoutProfit = branchQty * headerRecord.AveragePrice;
                    var bProfit = bValueWihoutProfit * headerRecord.Profit / 100;

                    entity.Quantity = branchQty;
                    entity.Value = bValueWihoutProfit + bProfit;
                }

                db.BranchAnnualSalesPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                new CRMSalesPlanHelper().UpdateStoreAnnualPlansOnBranchAnnualUpdate(entity);
            }

            return Json(new[] { branchAnnualSalesPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BranchAnnualSalesPlans_Destroy([DataSourceRequest]DataSourceRequest request, BranchAnnualSalesPlan branchAnnualSalesPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new BranchAnnualSalesPlan
                {
                    BranchAnnualSalesPlanId = branchAnnualSalesPlan.BranchAnnualSalesPlanId,
                    AnnualSalesPlanId = branchAnnualSalesPlan.AnnualSalesPlanId,
                    BranchId = branchAnnualSalesPlan.BranchId,
                    Ratio = branchAnnualSalesPlan.Ratio,
                    Quantity = branchAnnualSalesPlan.Quantity,
                    //Profit = branchAnnualSalesPlan.Profit,
                    Value = branchAnnualSalesPlan.Value,
                    Status = branchAnnualSalesPlan.Status,
                    Remark = branchAnnualSalesPlan.Remark,
                    IsVoid = branchAnnualSalesPlan.IsVoid,
                    VoidBy = branchAnnualSalesPlan.VoidBy,
                    VoidTime = branchAnnualSalesPlan.VoidTime,
                    IsOnlineApproved = branchAnnualSalesPlan.IsOnlineApproved,
                    OnlineApprovedBy = branchAnnualSalesPlan.OnlineApprovedBy,
                    OnlineApprovedTime = branchAnnualSalesPlan.OnlineApprovedTime,
                    IsOnlineTransferred = branchAnnualSalesPlan.IsOnlineTransferred,
                    IsOnlineTransferredBy = branchAnnualSalesPlan.IsOnlineTransferredBy,
                    OnlineTransferredTime = branchAnnualSalesPlan.OnlineTransferredTime,
                    IsSendForApproval = branchAnnualSalesPlan.IsSendForApproval,
                    OrgBranchName = branchAnnualSalesPlan.OrgBranchName,
                    SendForApprovalBy = branchAnnualSalesPlan.SendForApprovalBy,
                    SendForApprovalTime = branchAnnualSalesPlan.SendForApprovalTime,
                    DateCreated = branchAnnualSalesPlan.DateCreated,
                    LastModified = branchAnnualSalesPlan.LastModified,
                    CreatedBy = branchAnnualSalesPlan.CreatedBy,
                    ModifiedBy = branchAnnualSalesPlan.ModifiedBy
                };

                db.BranchAnnualSalesPlans.Attach(entity);
                db.BranchAnnualSalesPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { branchAnnualSalesPlan }.ToDataSourceResult(request, ModelState));
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

        
        private void CalculateDependentValuesOfBranchPlan(BranchAnnualSalesPlan branchAnnualSalesPlan)
        {
            var headerRecord = db.AnnualSalesPlans.Find(branchAnnualSalesPlan.AnnualSalesPlanId);
            if(headerRecord != null)
            {
                var qty = headerRecord.TotalAnnualPlanQuantity * branchAnnualSalesPlan.Ratio;
                var value = qty * headerRecord.AveragePrice * (headerRecord.Profit / 100);
            }
        }
        //private void UpdateAnnualPlanRecordOnBranchDataModification(BranchAnnualSalesPlan branchAnnualSalesPlan)
        //{
        //    var headerRecord = db.AnnualSalesPlans.Find(branchAnnualSalesPlan.AnnualSalesPlanId);
        //    if(headerRecord != null)
        //    {

        //    }
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
