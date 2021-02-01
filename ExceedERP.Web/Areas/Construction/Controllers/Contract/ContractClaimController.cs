using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    public class ContractClaimController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        // GET: Construction/ContractClaim
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ContractClaim_Read([DataSourceRequest]DataSourceRequest request, int pctId)
        {
            IQueryable<ContractClaim> claimRequests = db.ContractClaim.Where(cc => cc.ProjectContractId == pctId);
            DataSourceResult result = claimRequests.ToDataSourceResult(request, claimRequest => new {
                ContractClaimId = claimRequest.ContractClaimId,
                //ContractClaimTypeId = claimRequest.ContractClaimTypeId,
                ProjectContractId = claimRequest.ProjectContractId,
                RefNumber = claimRequest.RefNumber,
                Description = claimRequest.Description,
                RequestedDate = claimRequest.RequestedDate,
                ReferanceLetter = claimRequest.ReferanceLetter,
                RespondedDate = claimRequest.RespondedDate,
                ResponseRefNumber = claimRequest.ResponseRefNumber,
                ResponseReferanceLatter = claimRequest.ResponseReferanceLatter,
                TimeConsumed = claimRequest.TimeConsumed,
                Remark = claimRequest.Remark,

                OrgBranchName = claimRequest.OrgBranchName,
                DateCreated = claimRequest.DateCreated,
                LastModified = claimRequest.LastModified,
                CreatedBy = claimRequest.CreatedBy,
                ModifiedBy = claimRequest.ModifiedBy ?? " ",

                Status = claimRequest.Status,
                IsSendForApproval = claimRequest.IsSendForApproval,
                SendForApprovalBy = claimRequest.SendForApprovalBy,
                SendForApprovalTime = claimRequest.SendForApprovalTime,
                IsOnlineApproved = claimRequest.IsOnlineApproved,
                OnlineApprovedBy = claimRequest.OnlineApprovedBy,
                OnlineApprovedTime = claimRequest.OnlineApprovedTime,
                IsOnlineTransferred = claimRequest.IsOnlineTransferred,
                IsOnlineTransferredBy = claimRequest.IsOnlineTransferredBy,
                OnlineTransferredTime = claimRequest.OnlineTransferredTime,
                IsVoid = claimRequest.IsVoid,
                VoidBy = claimRequest.VoidBy,
                VoidTime = claimRequest.VoidTime,
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractClaim_Create([DataSourceRequest]DataSourceRequest request, ContractClaim claimRequest, int pcrId)
        {
            if (ModelState.IsValid)
            {

                var entity = new ContractClaim
                {
                    ContractClaimId = claimRequest.ContractClaimId,
                    //ContractClaimTypeId = claimRequest.ContractClaimTypeId,
                    ProjectContractId = pcrId,
                    RefNumber = claimRequest.RefNumber,
                    Description = claimRequest.Description,
                    RequestedDate = claimRequest.RequestedDate,
                    ReferanceLetter = claimRequest.ReferanceLetter,
                    RespondedDate = claimRequest.RespondedDate,
                    ResponseRefNumber = claimRequest.ResponseRefNumber,
                    ResponseReferanceLatter = claimRequest.ResponseReferanceLatter,
                    TimeConsumed = claimRequest.TimeConsumed,
                    Remark = claimRequest.Remark,

                    OrgBranchName = claimRequest.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = claimRequest.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = claimRequest.ModifiedBy,

                    Status = OperationStatuses.New.ToString(),
                    IsSendForApproval = claimRequest.IsSendForApproval,
                    SendForApprovalBy = claimRequest.SendForApprovalBy,
                    SendForApprovalTime = claimRequest.SendForApprovalTime,
                    IsOnlineApproved = claimRequest.IsOnlineApproved,
                    OnlineApprovedBy = claimRequest.OnlineApprovedBy,
                    OnlineApprovedTime = claimRequest.OnlineApprovedTime,
                    IsOnlineTransferred = claimRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = claimRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = claimRequest.OnlineTransferredTime,
                    IsVoid = claimRequest.IsVoid,
                    VoidBy = claimRequest.VoidBy,
                    VoidTime = claimRequest.VoidTime,
                };

                db.ContractClaim.Add(entity);
                db.SaveChanges();
                claimRequest.ContractClaimId = entity.ContractClaimId;
            }

            return Json(new[] { claimRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractClaim_Update([DataSourceRequest]DataSourceRequest request, ContractClaim claimRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractClaim
                {
                    ContractClaimId = claimRequest.ContractClaimId,
                    //ContractClaimTypeId = claimRequest.ContractClaimTypeId,
                    ProjectContractId = claimRequest.ProjectContractId,
                    RefNumber = claimRequest.RefNumber,
                    Description = claimRequest.Description,
                    RequestedDate = claimRequest.RequestedDate,
                    ReferanceLetter = claimRequest.ReferanceLetter,
                    RespondedDate = claimRequest.RespondedDate,
                    ResponseRefNumber = claimRequest.ResponseRefNumber,
                    ResponseReferanceLatter = claimRequest.ResponseReferanceLatter,
                    TimeConsumed = claimRequest.TimeConsumed,
                    
                    //ClaimStatus = claimRequest.ClaimStatus,
                    Remark = claimRequest.Remark,

                    OrgBranchName = claimRequest.OrgBranchName,
                    DateCreated = claimRequest.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = claimRequest.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                    Status = claimRequest.Status,
                    IsSendForApproval = claimRequest.IsSendForApproval,
                    SendForApprovalBy = claimRequest.SendForApprovalBy,
                    SendForApprovalTime = claimRequest.SendForApprovalTime,
                    IsOnlineApproved = claimRequest.IsOnlineApproved,
                    OnlineApprovedBy = claimRequest.OnlineApprovedBy,
                    OnlineApprovedTime = claimRequest.OnlineApprovedTime,
                    IsOnlineTransferred = claimRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = claimRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = claimRequest.OnlineTransferredTime,
                    IsVoid = claimRequest.IsVoid,
                    VoidBy = claimRequest.VoidBy,
                    VoidTime = claimRequest.VoidTime,
                };

                db.ContractClaim.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { claimRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractClaim_Destroy([DataSourceRequest]DataSourceRequest request, ContractClaim claimRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractClaim
                {
                    ContractClaimId = claimRequest.ContractClaimId,
                    //ContractClaimTypeId = claimRequest.ContractClaimTypeId,
                    ProjectContractId = claimRequest.ProjectContractId,
                    RefNumber = claimRequest.RefNumber,
                    Description = claimRequest.Description,
                    RequestedDate = claimRequest.RequestedDate,
                    ReferanceLetter = claimRequest.ReferanceLetter,
                    RespondedDate = claimRequest.RespondedDate,
                    ResponseRefNumber = claimRequest.ResponseRefNumber,
                    ResponseReferanceLatter = claimRequest.ResponseReferanceLatter,
                    TimeConsumed = claimRequest.TimeConsumed,
                    Remark = claimRequest.Remark,

                    OrgBranchName = claimRequest.OrgBranchName,
                    DateCreated = claimRequest.DateCreated,
                    LastModified = claimRequest.LastModified,
                    CreatedBy = claimRequest.CreatedBy,
                    ModifiedBy = claimRequest.ModifiedBy,

                    Status = claimRequest.Status,
                    IsSendForApproval = claimRequest.IsSendForApproval,
                    SendForApprovalBy = claimRequest.SendForApprovalBy,
                    SendForApprovalTime = claimRequest.SendForApprovalTime,
                    IsOnlineApproved = claimRequest.IsOnlineApproved,
                    OnlineApprovedBy = claimRequest.OnlineApprovedBy,
                    OnlineApprovedTime = claimRequest.OnlineApprovedTime,
                    IsOnlineTransferred = claimRequest.IsOnlineTransferred,
                    IsOnlineTransferredBy = claimRequest.IsOnlineTransferredBy,
                    OnlineTransferredTime = claimRequest.OnlineTransferredTime,
                    IsVoid = claimRequest.IsVoid,
                    VoidBy = claimRequest.VoidBy,
                    VoidTime = claimRequest.VoidTime,
                };

                db.ContractClaim.Attach(entity);
                db.ContractClaim.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { claimRequest }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult SendForApproval(int id)
        {
            var record = db.ContractClaim.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved )
            {
                #region validate price and tax

                //foreach (var item in record.WorkInProgressItems)
                //{
                //    if (item.UnitPrice <= 0 || item.GLTaxRateID == 0)
                //    {
                //        var formattedData3 = new
                //        {
                //            Message = "Please fill unit price and tax information for all items.",
                //            Status = "NOT_OK"
                //        };
                //        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                //    }
                //}
                #endregion

                //record.OperationStatus = OperationStatuses.WaitingApproval;
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.ContractClaim.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            //if (record.WorkInProgressItems.Count == 0)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Please add line items first.",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
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