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
    public class ContractTerminationValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, FinanceConstants.AccountPayableView, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult ContractTerminationValidations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ContractTerminationValidation> contractTerminationValidations = _db.ContractTerminationValidations.Where(x=>x.ContractTerminationId==id);
            DataSourceResult result = contractTerminationValidations.ToDataSourceResult(request, contractTerminationValidation => new {
                ContractTerminationValidationId = contractTerminationValidation.ContractTerminationValidationId,
                Status = contractTerminationValidation.Status,
                Remark = contractTerminationValidation.Remark,
                OrgBranchName = contractTerminationValidation.OrgBranchName,
                DateCreated = contractTerminationValidation.DateCreated,
                LastModified = contractTerminationValidation.LastModified,
                CreatedBy = contractTerminationValidation.CreatedBy,
                ModifiedBy = contractTerminationValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractTerminationValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ContractTerminationValidation contractTerminationValidation)
        {
            var record = _db.ContractTerminations.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (contractTerminationValidation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.Status = "Approved";
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.ContractTerminations.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                        #region update contract status
                        var contract = _db.ContractManagements.Find(record.ContractManagementID);
                        if (contract != null)
                        {
                            contract.ContractStatus = ContractStatus.Terminated;
                            _db.ContractManagements.Attach(contract);
                        }
                        _db.Entry(contract).State = EntityState.Modified;
                        _db.SaveChanges();

                        #endregion

                        if (record.GiveAwardToNextSupplier)
                        {
                            #region update award status

                            var awardedSupplier = _db.BidAwardedSuppliers.Find(record.ContractManagementID);
                            if (awardedSupplier != null)
                            {
                                awardedSupplier.AwardGiven = false;
                                _db.BidAwardedSuppliers.Attach(awardedSupplier);
                            }

                            _db.Entry(awardedSupplier).State = EntityState.Modified;
                            _db.SaveChanges();

                            #endregion
                        }
                    }

                    if (contractTerminationValidation.Status == ApprovalStatuses.Reject)
                    {

                        record.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime = DateTime.Now.Date;
                        _db.ContractTerminations.Attach(record);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                  
                var entity = new ContractTerminationValidation
                {
                    ContractTerminationId = id,
                    Status = contractTerminationValidation.Status,
                    Remark = contractTerminationValidation.Remark,
                    OrgBranchName = contractTerminationValidation.OrgBranchName,
                    DateCreated = contractTerminationValidation.DateCreated,
                    LastModified = contractTerminationValidation.LastModified,
                    CreatedBy = contractTerminationValidation.CreatedBy,
                    ModifiedBy = contractTerminationValidation.ModifiedBy
                };

                _db.ContractTerminationValidations.Add(entity);
                _db.SaveChanges();
                contractTerminationValidation.ContractTerminationValidationId = entity.ContractTerminationValidationId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "contract termination approval", "Success", User.Identity.Name, $"approved termination {entity.ContractTerminationId} ", Modules.Procurement);
                #endregion
                }
            }

            return Json(new[] { contractTerminationValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractTerminationValidations_Update([DataSourceRequest]DataSourceRequest request, ContractTerminationValidation contractTerminationValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractTerminationValidation
                {
                    ContractTerminationValidationId = contractTerminationValidation.ContractTerminationValidationId,
                    Status = contractTerminationValidation.Status,
                    Remark = contractTerminationValidation.Remark,
                    OrgBranchName = contractTerminationValidation.OrgBranchName,
                    DateCreated = contractTerminationValidation.DateCreated,
                    LastModified = contractTerminationValidation.LastModified,
                    CreatedBy = contractTerminationValidation.CreatedBy,
                    ModifiedBy = contractTerminationValidation.ModifiedBy
                };

                _db.ContractTerminationValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { contractTerminationValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractTerminationValidations_Destroy([DataSourceRequest]DataSourceRequest request, ContractTerminationValidation contractTerminationValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractTerminationValidation
                {
                    ContractTerminationValidationId = contractTerminationValidation.ContractTerminationValidationId,
                    Status = contractTerminationValidation.Status,
                    Remark = contractTerminationValidation.Remark,
                    OrgBranchName = contractTerminationValidation.OrgBranchName,
                    DateCreated = contractTerminationValidation.DateCreated,
                    LastModified = contractTerminationValidation.LastModified,
                    CreatedBy = contractTerminationValidation.CreatedBy,
                    ModifiedBy = contractTerminationValidation.ModifiedBy
                };

                _db.ContractTerminationValidations.Attach(entity);
                _db.ContractTerminationValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { contractTerminationValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
