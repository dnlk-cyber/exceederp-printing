using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    public class BudgetRequestDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Budget/BudgetRequestDetail
        public ActionResult BudgetRequestDetail_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<BudgetRequestDetail> budgetrequestlines = db.BudgetRequestDetails.Where(x => x.BudgetRequestId == Id).ToList();
            DataSourceResult result = budgetrequestlines.ToDataSourceResult(request, budgetRequestDetail => new
            {
                BudgetRequestDetailId = budgetRequestDetail.BudgetRequestDetailId,
                AccountCode = budgetRequestDetail.AccountCode,
                Description = budgetRequestDetail.Description,
                BudgetRequestId = budgetRequestDetail.BudgetRequestId,
                Status = budgetRequestDetail.Status,
                Remark = budgetRequestDetail.Remark,
                IsVoid = budgetRequestDetail.IsVoid,
                VoidBy = budgetRequestDetail.VoidBy,
                VoidTime = budgetRequestDetail.VoidTime,
                IsOnlineApproved = budgetRequestDetail.IsOnlineApproved,
                OnlineApprovedBy = budgetRequestDetail.OnlineApprovedBy,
                OnlineApprovedTime = budgetRequestDetail.OnlineApprovedTime,
                IsOnlineTransferred = budgetRequestDetail.IsOnlineTransferred,
                IsOnlineTransferredBy = budgetRequestDetail.IsOnlineTransferredBy,
                OnlineTransferredTime = budgetRequestDetail.OnlineTransferredTime,
                IsSendForApproval = budgetRequestDetail.IsSendForApproval,
                OrgBranchName = budgetRequestDetail.OrgBranchName,
                SendForApprovalBy = budgetRequestDetail.SendForApprovalBy,
                SendForApprovalTime = budgetRequestDetail.SendForApprovalTime,
                DateCreated = budgetRequestDetail.DateCreated,
                LastModified = budgetRequestDetail.LastModified,
                CreatedBy = budgetRequestDetail.CreatedBy,
                ModifiedBy = budgetRequestDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequestDetail_Create([DataSourceRequest]DataSourceRequest request, BudgetRequestDetail budgetRequestDetail, int Id)
        {
            if (ModelState.IsValid)
            {

                var entity = new BudgetRequestDetail
                {
                    BudgetRequestId = Id,
                    BudgetRequestDetailId = budgetRequestDetail.BudgetRequestDetailId,
                    Description = budgetRequestDetail.Description,
                    AccountCode = budgetRequestDetail.AccountCode,
                    Status = budgetRequestDetail.Status,
                    Remark = budgetRequestDetail.Remark,
                    IsVoid = budgetRequestDetail.IsVoid,
                    VoidBy = budgetRequestDetail.VoidBy,
                    VoidTime = budgetRequestDetail.VoidTime,
                    IsOnlineApproved = budgetRequestDetail.IsOnlineApproved,
                    OnlineApprovedBy = budgetRequestDetail.OnlineApprovedBy,
                    OnlineApprovedTime = budgetRequestDetail.OnlineApprovedTime,
                    IsOnlineTransferred = budgetRequestDetail.IsOnlineTransferred,
                    IsOnlineTransferredBy = budgetRequestDetail.IsOnlineTransferredBy,
                    OnlineTransferredTime = budgetRequestDetail.OnlineTransferredTime,
                    IsSendForApproval = budgetRequestDetail.IsSendForApproval,
                    OrgBranchName = budgetRequestDetail.OrgBranchName,
                    SendForApprovalBy = budgetRequestDetail.SendForApprovalBy,
                    SendForApprovalTime = budgetRequestDetail.SendForApprovalTime,
                    DateCreated = DateTime.Now,
                    LastModified = budgetRequestDetail.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = budgetRequestDetail.ModifiedBy
                };

                
                db.BudgetRequestDetails.Add(entity);
                db.SaveChanges();
                budgetRequestDetail.BudgetRequestDetailId = entity.BudgetRequestDetailId;
                
            }

            return Json(new[] { budgetRequestDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequestDetail_Update([DataSourceRequest]DataSourceRequest request, BudgetRequestDetail budgetRequestDetail)
        {
            if (ModelState.IsValid)
            {

                var entity = new BudgetRequestDetail
                {
                    
                    BudgetRequestDetailId = budgetRequestDetail.BudgetRequestDetailId,
                    BudgetRequestId = budgetRequestDetail.BudgetRequestId,
                    Description = budgetRequestDetail.Description,
                    AccountCode = budgetRequestDetail.AccountCode,
                    Status = budgetRequestDetail.Status,
                    Remark = budgetRequestDetail.Remark,
                    IsVoid = budgetRequestDetail.IsVoid,
                    VoidBy = budgetRequestDetail.VoidBy,
                    VoidTime = budgetRequestDetail.VoidTime,
                    IsOnlineApproved = budgetRequestDetail.IsOnlineApproved,
                    OnlineApprovedBy = budgetRequestDetail.OnlineApprovedBy,
                    OnlineApprovedTime = budgetRequestDetail.OnlineApprovedTime,
                    IsOnlineTransferred = budgetRequestDetail.IsOnlineTransferred,
                    IsOnlineTransferredBy = budgetRequestDetail.IsOnlineTransferredBy,
                    OnlineTransferredTime = budgetRequestDetail.OnlineTransferredTime,
                    IsSendForApproval = budgetRequestDetail.IsSendForApproval,
                    OrgBranchName = budgetRequestDetail.OrgBranchName,
                    SendForApprovalBy = budgetRequestDetail.SendForApprovalBy,
                    SendForApprovalTime = budgetRequestDetail.SendForApprovalTime,
                    DateCreated = DateTime.Now,
                    LastModified = budgetRequestDetail.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = budgetRequestDetail.ModifiedBy
                };


                db.BudgetRequestDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
               

            }

            return Json(new[] { budgetRequestDetail }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequestDetail_Delete([DataSourceRequest]DataSourceRequest request, BudgetRequestDetail budgetRequestDetail)
        {
            if (ModelState.IsValid)
            {

                var entity = new BudgetRequestDetail
                {
                    
                    BudgetRequestDetailId = budgetRequestDetail.BudgetRequestDetailId,
                    BudgetRequestId = budgetRequestDetail.BudgetRequestId,
                    Description = budgetRequestDetail.Description,
                    AccountCode = budgetRequestDetail.AccountCode,
                    Status = budgetRequestDetail.Status,
                    Remark = budgetRequestDetail.Remark,
                    IsVoid = budgetRequestDetail.IsVoid,
                    VoidBy = budgetRequestDetail.VoidBy,
                    VoidTime = budgetRequestDetail.VoidTime,
                    IsOnlineApproved = budgetRequestDetail.IsOnlineApproved,
                    OnlineApprovedBy = budgetRequestDetail.OnlineApprovedBy,
                    OnlineApprovedTime = budgetRequestDetail.OnlineApprovedTime,
                    IsOnlineTransferred = budgetRequestDetail.IsOnlineTransferred,
                    IsOnlineTransferredBy = budgetRequestDetail.IsOnlineTransferredBy,
                    OnlineTransferredTime = budgetRequestDetail.OnlineTransferredTime,
                    IsSendForApproval = budgetRequestDetail.IsSendForApproval,
                    OrgBranchName = budgetRequestDetail.OrgBranchName,
                    SendForApprovalBy = budgetRequestDetail.SendForApprovalBy,
                    SendForApprovalTime = budgetRequestDetail.SendForApprovalTime,
                    DateCreated = DateTime.Now,
                    LastModified = budgetRequestDetail.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = budgetRequestDetail.ModifiedBy
                };


                db.BudgetRequestDetails.Attach(entity);
                db.BudgetRequestDetails.Remove(entity);
                db.SaveChanges();
                budgetRequestDetail.BudgetRequestDetailId = entity.BudgetRequestDetailId;

            }

            return Json(new[] { budgetRequestDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}