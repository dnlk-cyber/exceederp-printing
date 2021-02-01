using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Contract
{
    public class ContractRenewalValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser, FinanceConstants.AccountPayableView, ProcurementConstants.ProcurementContractApprover)]
        public ActionResult ContractRenewalValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ContractRenewalValidation> contractrenewalvalidations = db.ContractRenewalValidations.Where(x=>x.ContractRenewalId==id);
            DataSourceResult result = contractrenewalvalidations.ToDataSourceResult(request, contractRenewalValidation => new {
                ContractRenewalValidationId = contractRenewalValidation.ContractRenewalValidationId,
                Status = contractRenewalValidation.Status,
                Remark = contractRenewalValidation.Remark,
                OrgBranchName = contractRenewalValidation.OrgBranchName,
                DateCreated = contractRenewalValidation.DateCreated,
                LastModified = contractRenewalValidation.LastModified,
                CreatedBy = contractRenewalValidation.CreatedBy,
                ModifiedBy = contractRenewalValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractRenewalValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ContractRenewalValidation contractRenewalValidation)
        {
            var record = db.ContractRenewals.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (contractRenewalValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.Status = "Approved";
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                    }

                    if (contractRenewalValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                    }
                    db.ContractRenewals.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                var entity = new ContractRenewalValidation
                {
                    ContractRenewalId = id,
                    Status = contractRenewalValidation.Status,
                    Remark = contractRenewalValidation.Remark,
                    OrgBranchName = contractRenewalValidation.OrgBranchName,
                    DateCreated = contractRenewalValidation.DateCreated,
                    LastModified = contractRenewalValidation.LastModified,
                    CreatedBy = contractRenewalValidation.CreatedBy,
                    ModifiedBy = contractRenewalValidation.ModifiedBy
                };

                db.ContractRenewalValidations.Add(entity);
                db.SaveChanges();
                contractRenewalValidation.ContractRenewalValidationId = entity.ContractRenewalValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "contract renewal approval", "Success", User.Identity.Name, $"approved renewal {entity.ContractRenewalId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { contractRenewalValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractRenewalValidations_Update([DataSourceRequest]DataSourceRequest request, ContractRenewalValidation contractRenewalValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractRenewalValidation
                {
                    ContractRenewalValidationId = contractRenewalValidation.ContractRenewalValidationId,
                    Status = contractRenewalValidation.Status,
                    Remark = contractRenewalValidation.Remark,
                    OrgBranchName = contractRenewalValidation.OrgBranchName,
                    DateCreated = contractRenewalValidation.DateCreated,
                    LastModified = contractRenewalValidation.LastModified,
                    CreatedBy = contractRenewalValidation.CreatedBy,
                    ModifiedBy = contractRenewalValidation.ModifiedBy
                };

                db.ContractRenewalValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { contractRenewalValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractRenewalValidations_Destroy([DataSourceRequest]DataSourceRequest request, ContractRenewalValidation contractRenewalValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractRenewalValidation
                {
                    ContractRenewalValidationId = contractRenewalValidation.ContractRenewalValidationId,
                    Status = contractRenewalValidation.Status,
                    Remark = contractRenewalValidation.Remark,
                    OrgBranchName = contractRenewalValidation.OrgBranchName,
                    DateCreated = contractRenewalValidation.DateCreated,
                    LastModified = contractRenewalValidation.LastModified,
                    CreatedBy = contractRenewalValidation.CreatedBy,
                    ModifiedBy = contractRenewalValidation.ModifiedBy
                };

                db.ContractRenewalValidations.Attach(entity);
                db.ContractRenewalValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { contractRenewalValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
