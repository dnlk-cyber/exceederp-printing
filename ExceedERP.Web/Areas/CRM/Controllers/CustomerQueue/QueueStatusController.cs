using System;
using System.Collections.Generic;
////using System.Data
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMQueueApprover, CRMRoles.CRMQueueUser)]
    public class QueueStatusController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["QueueTypes"] = db.QueueTypes.Select(q => new { q.QueueTypeId, q.QueueName }).ToList();
            ViewData["InvestmentTypes"] = db.InvestmentTypes.ToList();
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["LocationMaps"] = db.LocationMap.Select(l => new { l.LocationMapId, l.LocationName }).ToList();

            #region temp code to readjust aproval status and record status columns
            var incorrectRec = db.QueueStatus.Where(q => q.QueueStatusStatus == CustomerQueueStatus.Waiting).ToList();
            if (incorrectRec.Any())
            {
                foreach(QueueStatus queueStatus in incorrectRec)
                {
                    queueStatus.QueueStatusStatus = (CustomerQueueStatus)int.Parse(queueStatus.Status);

                    queueStatus.Status = null;

                    db.QueueStatus.Attach(queueStatus);
                    db.Entry(queueStatus).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            #endregion

            return View();
        }

        public ActionResult Queue_Read([DataSourceRequest]DataSourceRequest request)
        {
            var locIds = new HashSet<int>(db.LocationUserAssignment.Where(uas => uas.UserName == User.Identity.Name).ToList().Select(m => m.LocationMapId));

            IQueryable<QueueStatus> queues = db.QueueStatus.Where(qs => locIds.Contains(qs.Queue.LocationMapId));
            DataSourceResult result = queues.ToDataSourceResult(request, queueStatus => new QueueStatus
            {
                QueueStatusId = queueStatus.QueueStatusId,
                QueueId = queueStatus.QueueId,

                QueueNumber = queueStatus.Queue.QueueNumber,
                CustomerName = queueStatus.Queue.QueueCustomer.CustomerName,
                TinNumber = queueStatus.Queue.QueueCustomer.TinNumber,
                Phone = queueStatus.Queue.QueueCustomer.Phone,
                InvestmentType = queueStatus.Queue.QueueCustomer.InvestmentType.InvestmentName,

                TotalRequestedAmount = queueStatus.Queue.TotalRequestedAmount,
                WeeklyRequest = queueStatus.Queue.WeeklyRequest,
                ApprovedQuantity = queueStatus.Queue.ApprovedQuantity,
                BranchId = queueStatus.Queue.BranchId,
                LocationMapId = queueStatus.Queue.LocationMapId,
                Store = queueStatus.Queue.Store,

                ShippedDate = queueStatus.ShippedDate,
                QueueStatusStatus = queueStatus.QueueStatusStatus,
                ShippedQuantity = queueStatus.ShippedQuantity,
                RemainingQuantity = queueStatus.RemainingQuantity,

                Status = queueStatus.Status,
                IsVoid = queueStatus.IsVoid,
                IsOnlineApproved = queueStatus.IsOnlineApproved,
                OnlineApprovedBy = queueStatus.OnlineApprovedBy,
                IsOnlineTransferred = queueStatus.IsOnlineTransferred,
                IsOnlineTransferredBy = queueStatus.IsOnlineTransferredBy,
                OnlineApprovedTime = queueStatus.OnlineApprovedTime,
                OnlineTransferredTime = queueStatus.OnlineTransferredTime,
                IsSendForApproval = queueStatus.IsSendForApproval,
                SendForApprovalBy = queueStatus.SendForApprovalBy,
                SendForApprovalTime = queueStatus.SendForApprovalTime,
                VoidBy = queueStatus.VoidBy,
                VoidTime = queueStatus.VoidTime,

                CreatedBy = queueStatus.CreatedBy,
                DateCreated = queueStatus.DateCreated,
                LastModified = queueStatus.LastModified,
                ModifiedBy = queueStatus.ModifiedBy,
                OrgBranchName = queueStatus.OrgBranchName,

                Remark = queueStatus.Remark
            });

            return Json(result);
        }
        //private bool ActiveQueueExists(Queue queueStatus)
        //{
        //    return db.Queue
        //        .FirstOrDefault(q => q.QueueCustomerId == queueStatus.QueueCustomerId && 
        //                        q.QueueStatus != CustomerQueueStatus.Closed && 
        //                        q.QueueId != queueStatus.QueueId)
        //                        != null;
        //}
        //private bool ValidateQueueStatus(QueueStatus queueStatus)
        //{
        //    var 
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Queue_Create([DataSourceRequest]DataSourceRequest request, QueueStatus queueStatus)
        {
            if (ModelState.IsValid)
            {
                //if (ActiveQueueExists(queueStatus))
                //{
                //    ModelState.AddModelError("", "Active Queue Exists for this Customer");
                //}
                //else
                //{
                var entity = new QueueStatus
                {
                    //QueueNumber = GetQueNumber(queueStatus),
                    QueueId = queueStatus.QueueId,

                    //TotalRequestedAmount = queueStatus.TotalRequestedAmount,
                    //WeeklyRequest = queueStatus.WeeklyRequest,
                    //ApprovedQuantity = queueStatus.ApprovedQuantity,
                    //BranchId = queueStatus.BranchId,
                    //LocationMapId = queueStatus.LocationMapId,
                    //Store = queueStatus.Store,
                    //QueueDate = queueStatus.QueueDate,
                    QueueStatusStatus = CustomerQueueStatus.Partial,
                    ShippedQuantity = queueStatus.ShippedQuantity,
                    ShippedDate = queueStatus.ShippedDate,
                    RemainingQuantity = queueStatus.RemainingQuantity,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,

                    Remark = queueStatus.Remark
                };
                if (entity.RemainingQuantity <=  0)
                {
                    entity.RemainingQuantity = 0;

                    entity.QueueStatusStatus = CustomerQueueStatus.Closed;
                }

                db.QueueStatus.Add(entity);
                db.SaveChanges();
                queueStatus.QueueStatusId = entity.QueueStatusId;

                var q = db.Queue.Find(entity.QueueId);
                if(q != null)
                {
                    q.QueueStatus = entity.QueueStatusStatus;

                    db.Queue.Attach(q);
                    db.Entry(q).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //}
            }

            return Json(new[] { queueStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Queue_Update([DataSourceRequest]DataSourceRequest request, QueueStatus queueStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueStatus
                {
                    QueueStatusId = queueStatus.QueueStatusId,
                    QueueId = queueStatus.QueueId,
                    //QueueNumber = queueStatus.QueueNumber,
                    //TotalRequestedAmount = queueStatus.TotalRequestedAmount,
                    //WeeklyRequest = queueStatus.WeeklyRequest,
                    //ApprovedQuantity = queueStatus.ApprovedQuantity,
                    //BranchId = queueStatus.BranchId,
                    //LocationMapId = queueStatus.LocationMapId,
                    //Store = queueStatus.Store,
                    QueueStatusStatus = queueStatus.QueueStatusStatus,
                    ShippedQuantity = queueStatus.ShippedQuantity,
                    ShippedDate = queueStatus.ShippedDate,
                    RemainingQuantity = queueStatus.RemainingQuantity,

                    Status = queueStatus.Status,
                    IsVoid = queueStatus.IsVoid,
                    IsOnlineApproved = queueStatus.IsOnlineApproved,
                    OnlineApprovedBy = queueStatus.OnlineApprovedBy,
                    IsOnlineTransferred = queueStatus.IsOnlineTransferred,
                    IsOnlineTransferredBy = queueStatus.IsOnlineTransferredBy,
                    OnlineApprovedTime = queueStatus.OnlineApprovedTime,
                    OnlineTransferredTime = queueStatus.OnlineTransferredTime,
                    IsSendForApproval = queueStatus.IsSendForApproval,
                    SendForApprovalBy = queueStatus.SendForApprovalBy,
                    SendForApprovalTime = queueStatus.SendForApprovalTime,
                    VoidBy = queueStatus.VoidBy,
                    VoidTime = queueStatus.VoidTime,

                    Remark = queueStatus.Remark
                };

                db.QueueStatus.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                var q = db.Queue.Find(entity.QueueId);
                if (q != null)
                {
                    q.QueueStatus = entity.QueueStatusStatus;

                    db.Queue.Attach(q);
                    db.Entry(q).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { queueStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Queue_Destroy([DataSourceRequest]DataSourceRequest request, QueueStatus queueStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueStatus
                {
                    QueueStatusId = queueStatus.QueueStatusId,
                    QueueId = queueStatus.QueueId,
                    //QueueNumber = queueStatus.QueueNumber,
                    //TotalRequestedAmount = queueStatus.TotalRequestedAmount,
                    //WeeklyRequest = queueStatus.WeeklyRequest,
                    //ApprovedQuantity = queueStatus.ApprovedQuantity,
                    //BranchId = queueStatus.BranchId,
                    //LocationMapId = queueStatus.LocationMapId,
                    //Store = queueStatus.Store,
                    QueueStatusStatus = queueStatus.QueueStatusStatus,
                    ShippedQuantity = queueStatus.ShippedQuantity,
                    ShippedDate = queueStatus.ShippedDate,
                    RemainingQuantity = queueStatus.RemainingQuantity,

                    Status = queueStatus.Status,
                    IsVoid = queueStatus.IsVoid,
                    IsOnlineApproved = queueStatus.IsOnlineApproved,
                    OnlineApprovedBy = queueStatus.OnlineApprovedBy,
                    IsOnlineTransferred = queueStatus.IsOnlineTransferred,
                    IsOnlineTransferredBy = queueStatus.IsOnlineTransferredBy,
                    OnlineApprovedTime = queueStatus.OnlineApprovedTime,
                    OnlineTransferredTime = queueStatus.OnlineTransferredTime,
                    IsSendForApproval = queueStatus.IsSendForApproval,
                    SendForApprovalBy = queueStatus.SendForApprovalBy,
                    SendForApprovalTime = queueStatus.SendForApprovalTime,
                    VoidBy = queueStatus.VoidBy,
                    VoidTime = queueStatus.VoidTime,

                    Remark = queueStatus.Remark
                };

                db.QueueStatus.Attach(entity);
                db.QueueStatus.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { queueStatus }.ToDataSourceResult(request, ModelState));
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

        #region local methods
        private int GetQueNumber(Queue queueStatus)
        {
            int order = 1;
            var existingRecords = db.Queue.Where(q => q.QueueTypeId == queueStatus.QueueTypeId && q.LocationMapId == queueStatus.LocationMapId).ToList();
            if (existingRecords.Any())
            {
                order = existingRecords.Max(r => r.QueueNumber) + 1;
            }
            return order;
        }
        #endregion
        #region Global methods
        public JsonResult SendForApproval(int id)
        {
            var record = db.QueueStatus.Find(id);
            if ((record != null && !record.IsSendForApproval && !record.IsOnlineApproved))
            {

                record.Status = OperationStatuses.WaitingApproval.ToString();
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.QueueStatus.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Queue Status Record not found!",
                    Status = "NOT_OK"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
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
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
