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
    public class ContractPaymentValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, FinanceConstants.AccountPayableView, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult ContractPaymentValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ContractPaymentValidation> contractPaymentValidations = _db.ContractPaymentValidations.Where(x=>x.ContractPaymentId==id);
            DataSourceResult result = contractPaymentValidations.ToDataSourceResult(request, contractPaymentValidation => new {
                ContractPaymentValidationId = contractPaymentValidation.ContractPaymentValidationId,
                Status = contractPaymentValidation.Status,
                Remark = contractPaymentValidation.Remark,
                OrgBranchName = contractPaymentValidation.OrgBranchName,
                DateCreated = contractPaymentValidation.DateCreated,
                LastModified = contractPaymentValidation.LastModified,
                CreatedBy = contractPaymentValidation.CreatedBy,
                ModifiedBy = contractPaymentValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ContractPaymentValidation contractPaymentValidation)
        {
            var record = _db.ContractPayments.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (contractPaymentValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.Status = "Approved";
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.ContractPayments.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }

                    if (contractPaymentValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.ContractPayments.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                  
                var entity = new ContractPaymentValidation
                {
                    ContractPaymentId = id,
                    Status = contractPaymentValidation.Status,
                    Remark = contractPaymentValidation.Remark,
                    OrgBranchName = contractPaymentValidation.OrgBranchName,
                    DateCreated = contractPaymentValidation.DateCreated,
                    LastModified = contractPaymentValidation.LastModified,
                    CreatedBy = contractPaymentValidation.CreatedBy,
                    ModifiedBy = contractPaymentValidation.ModifiedBy
                };

                _db.ContractPaymentValidations.Add(entity);
                _db.SaveChanges();
                contractPaymentValidation.ContractPaymentValidationId = entity.ContractPaymentValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "contract payment approval", "Success", User.Identity.Name, $"approved payment {entity.ContractPaymentId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { contractPaymentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentValidations_Update([DataSourceRequest]DataSourceRequest request, ContractPaymentValidation contractPaymentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractPaymentValidation
                {
                    ContractPaymentValidationId = contractPaymentValidation.ContractPaymentValidationId,
                    Status = contractPaymentValidation.Status,
                    Remark = contractPaymentValidation.Remark,
                    OrgBranchName = contractPaymentValidation.OrgBranchName,
                    DateCreated = contractPaymentValidation.DateCreated,
                    LastModified = contractPaymentValidation.LastModified,
                    CreatedBy = contractPaymentValidation.CreatedBy,
                    ModifiedBy = contractPaymentValidation.ModifiedBy
                };

                _db.ContractPaymentValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { contractPaymentValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractPaymentValidations_Destroy([DataSourceRequest]DataSourceRequest request, ContractPaymentValidation contractPaymentValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractPaymentValidation
                {
                    ContractPaymentValidationId = contractPaymentValidation.ContractPaymentValidationId,
                    Status = contractPaymentValidation.Status,
                    Remark = contractPaymentValidation.Remark,
                    OrgBranchName = contractPaymentValidation.OrgBranchName,
                    DateCreated = contractPaymentValidation.DateCreated,
                    LastModified = contractPaymentValidation.LastModified,
                    CreatedBy = contractPaymentValidation.CreatedBy,
                    ModifiedBy = contractPaymentValidation.ModifiedBy
                };

                _db.ContractPaymentValidations.Attach(entity);
                _db.ContractPaymentValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { contractPaymentValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
