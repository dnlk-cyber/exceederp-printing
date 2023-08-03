using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Contract
{
    public class ContractManagementValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractUser, ProcurementRoles.ProcurementContractApprover)]
        public ActionResult ContractManagementValidations_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<ContractManagementValidation> contractmanagementvalidations = db.ContractManagementValidations.Where(x=>x.ContractManagementID==id);
            DataSourceResult result = contractmanagementvalidations.ToDataSourceResult(request, contractManagementValidation => new {
                ContractManagementValidationId = contractManagementValidation.ContractManagementValidationId,
                ContractManagementID = contractManagementValidation.ContractManagementID,
                Status = contractManagementValidation.Status,
                Remark = contractManagementValidation.Remark,
                OrgBranchName = contractManagementValidation.OrgBranchName,
                DateCreated = contractManagementValidation.DateCreated,
                LastModified = contractManagementValidation.LastModified,
                CreatedBy = contractManagementValidation.CreatedBy,
                ModifiedBy = contractManagementValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractManagementValidations_Create(int id,[DataSourceRequest]DataSourceRequest request, ContractManagementValidation contractManagementValidation)
        {
            var record = db.ContractManagements.Find(id);

            if (record != null && record.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate This Record: It is Already Approved");
            }
            if (ModelState.IsValid)
            {
                if (record != null)
                {

                    if (contractManagementValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsFinalApproval)
                        {
                            record.Status = "Approved";
                            record.IsVoid = false;
                            record.IsOnlineApproved = true;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            record.ContractStatus =ContractStatus.Active;
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();

                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                Message =$"Contract management document {record.ContractManagementID} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
                            };

                            Notify.NotifyUser(notif);

                            #endregion
                        }
                        else
                        {
                            record.Status = "Partially Approved";
                            record.IsOnlineApproved = false;
                            record.IsVoid = false;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();

                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                Message =
                                    $"Contract management document {record.ContractManagementID} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            };

                            Notify.NotifyUser(notif);

                            #endregion

                            #region forward to next approver

                            decimal docamount = record.ContractManagementItem.Sum(x => x.Total);
                            DocApprovalWorkflowHelper.ForwardToApprover(record.ContractManagementID, docamount, "",
                                record.ForwardedTo, ApprovalDocumentType.ContractManagement, false);

                            #endregion

                        }


                    }

                    if (contractManagementValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        //notification
                    }

                    var entity = new ContractManagementValidation
                    {
                        ContractManagementID = id,
                        Status = contractManagementValidation.Status,
                        Remark = contractManagementValidation.Remark,
                        OrgBranchName = contractManagementValidation.OrgBranchName,
                        DateCreated = contractManagementValidation.DateCreated,
                        LastModified = contractManagementValidation.LastModified,
                        CreatedBy = contractManagementValidation.CreatedBy,
                        ModifiedBy = contractManagementValidation.ModifiedBy
                    };

                    db.ContractManagementValidations.Add(entity);
                    db.SaveChanges();
                    contractManagementValidation.ContractManagementValidationId = entity.ContractManagementValidationId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "contract approval", "Success", User.Identity.Name, $"approved contract {entity.ContractManagementID} ", Modules.Procurement);
                    #endregion
                }

            }

            return Json(new[] { contractManagementValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractManagementValidations_Update(int id,[DataSourceRequest]DataSourceRequest request, ContractManagementValidation contractManagementValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractManagementValidation
                {
                    ContractManagementID=id,
                    ContractManagementValidationId = contractManagementValidation.ContractManagementValidationId,
                    Status = contractManagementValidation.Status,
                    Remark = contractManagementValidation.Remark,
                    OrgBranchName = contractManagementValidation.OrgBranchName,
                    DateCreated = contractManagementValidation.DateCreated,
                    LastModified = contractManagementValidation.LastModified,
                    CreatedBy = contractManagementValidation.CreatedBy,
                    ModifiedBy = contractManagementValidation.ModifiedBy
                };

                db.ContractManagementValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { contractManagementValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementContractApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractManagementValidations_Destroy([DataSourceRequest]DataSourceRequest request, ContractManagementValidation contractManagementValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractManagementValidation
                {
                    ContractManagementValidationId = contractManagementValidation.ContractManagementValidationId,
                    Status = contractManagementValidation.Status,
                    Remark = contractManagementValidation.Remark,
                    OrgBranchName = contractManagementValidation.OrgBranchName,
                    DateCreated = contractManagementValidation.DateCreated,
                    LastModified = contractManagementValidation.LastModified,
                    CreatedBy = contractManagementValidation.CreatedBy,
                    ModifiedBy = contractManagementValidation.ModifiedBy
                };

                db.ContractManagementValidations.Attach(entity);
                db.ContractManagementValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { contractManagementValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
