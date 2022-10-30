using System;
using System.Collections.Generic;
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
    [AuthorizeRoles( CRMRoles.CRMQueueApprover, CRMRoles.CRMQueueUser)] 
    public class QueueController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["QueueTypes"] = db.QueueTypes.Select(q => new { q.QueueTypeId, q.QueueName }).ToList();
            ViewData["InvestmentTypes"] = db.InvestmentTypes.ToList();
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["LocationMaps"] = db.LocationMap.Select(l => new { l.LocationMapId, l.LocationName }).ToList();
            return View();
        }

        public ActionResult Queue_Read([DataSourceRequest]DataSourceRequest request)
        {
            var locIds = new HashSet<int>(db.LocationUserAssignment.Where(uas => uas.UserName == User.Identity.Name).ToList().Select(m => m.LocationMapId));

            IQueryable<Queue> queues = db.Queue.Where(q => locIds.Contains(q.LocationMapId));
            DataSourceResult result = queues.ToDataSourceResult(request, queue => new Queue {
                QueueId = queue.QueueId,
                QueueNumber = queue.QueueNumber,
                QueueTypeId = queue.QueueTypeId,
                QueueCustomerId = queue.QueueCustomerId,

                CustomerName = queue.QueueCustomer.CustomerName,
                TinNumber = queue.QueueCustomer.TinNumber,
                Phone = queue.QueueCustomer.Phone,
                InvestmentType = queue.QueueCustomer.InvestmentType.InvestmentName,
                InvestmentAmount = queue.QueueCustomer.InvestmentAmount,
                Grade = queue.QueueCustomer.Grade,

                TotalRequestedAmount = queue.TotalRequestedAmount,
                WeeklyRequest = queue.WeeklyRequest,
                ApprovedQuantity = queue.ApprovedQuantity,
                BranchId = queue.BranchId,
                LocationMapId = queue.LocationMapId,
                Store = queue.Store,
                QueueDate = queue.QueueDate,
                QueueStatus = queue.QueueStatus,
                AdvancePayment = queue.AdvancePayment,

                Status = queue.Status,
                IsVoid = queue.IsVoid,
                IsOnlineApproved = queue.IsOnlineApproved,
                OnlineApprovedBy = queue.OnlineApprovedBy,
                IsOnlineTransferred = queue.IsOnlineTransferred,
                IsOnlineTransferredBy = queue.IsOnlineTransferredBy,
                OnlineApprovedTime = queue.OnlineApprovedTime,
                OnlineTransferredTime = queue.OnlineTransferredTime,
                IsSendForApproval = queue.IsSendForApproval,
                SendForApprovalBy = queue.SendForApprovalBy,
                SendForApprovalTime = queue.SendForApprovalTime,
                VoidBy = queue.VoidBy,
                VoidTime = queue.VoidTime,

                CreatedBy = queue.CreatedBy,
                DateCreated = queue.DateCreated,
                LastModified = queue.LastModified,
                ModifiedBy = queue.ModifiedBy,
                OrgBranchName = queue.OrgBranchName,

                Remark = queue.Remark
            });

            return Json(result);
        }
        private bool ActiveQueueExists(Queue queue)
        {
            return db.Queue
                .FirstOrDefault(q => q.QueueCustomerId == queue.QueueCustomerId && 
                                q.QueueStatus != CustomerQueueStatus.Closed && 
                                q.QueueId != queue.QueueId)
                                != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Queue_Create([DataSourceRequest]DataSourceRequest request, Queue queue)
        {
            if (ModelState.IsValid)
            {
                if (ActiveQueueExists(queue))
                {
                    ModelState.AddModelError("", "Active Queue Exists for this Customer");
                }
                else
                {

                    var entity = new Queue
                    {
                        QueueNumber = GetQueNumber(queue),
                        QueueTypeId = queue.QueueTypeId,
                        QueueCustomerId = queue.QueueCustomerId,

                        TotalRequestedAmount = queue.TotalRequestedAmount,
                        WeeklyRequest = queue.WeeklyRequest,
                        ApprovedQuantity = queue.ApprovedQuantity,
                        BranchId = queue.BranchId,
                        LocationMapId = queue.LocationMapId,
                        Store = queue.Store,
                        QueueDate = queue.QueueDate,
                        QueueStatus = CustomerQueueStatus.Waiting,
                        AdvancePayment = queue.AdvancePayment,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,

                        Remark = queue.Remark
                    };

                    if(entity.ApprovedQuantity == 0)
                    {
                        entity.ApprovedQuantity = entity.TotalRequestedAmount;
                    }

                    db.Queue.Add(entity);
                    db.SaveChanges();
                    queue.QueueId = entity.QueueId;
                }
            }

            return Json(new[] { queue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Queue_Update([DataSourceRequest]DataSourceRequest request, Queue queue)
        {
            if (ModelState.IsValid)
            {
                if (ActiveQueueExists(queue))
                {
                    ModelState.AddModelError("", "Active Queue Exists for this Customer");
                }
                else
                {
                    var entity = new Queue
                    {
                        QueueId = queue.QueueId,
                        QueueNumber = GetQueNumber(queue),
                        QueueTypeId = queue.QueueTypeId,
                        QueueCustomerId = queue.QueueCustomerId,

                        TotalRequestedAmount = queue.TotalRequestedAmount,
                        WeeklyRequest = queue.WeeklyRequest,
                        ApprovedQuantity = queue.ApprovedQuantity,
                        BranchId = queue.BranchId,
                        LocationMapId = queue.LocationMapId,
                        Store = queue.Store,
                        QueueDate = queue.QueueDate,
                        QueueStatus = queue.QueueStatus,
                        AdvancePayment = queue.AdvancePayment,

                        Status = queue.Status,
                        IsVoid = queue.IsVoid,
                        IsOnlineApproved = queue.IsOnlineApproved,
                        OnlineApprovedBy = queue.OnlineApprovedBy,
                        IsOnlineTransferred = queue.IsOnlineTransferred,
                        IsOnlineTransferredBy = queue.IsOnlineTransferredBy,
                        OnlineApprovedTime = queue.OnlineApprovedTime,
                        OnlineTransferredTime = queue.OnlineTransferredTime,
                        IsSendForApproval = queue.IsSendForApproval,
                        SendForApprovalBy = queue.SendForApprovalBy,
                        SendForApprovalTime = queue.SendForApprovalTime,
                        VoidBy = queue.VoidBy,
                        VoidTime = queue.VoidTime,

                        CreatedBy = queue.CreatedBy,
                        DateCreated = queue.DateCreated,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name,
                        OrgBranchName = queue.OrgBranchName,

                        Remark = queue.Remark
                    };

                    if (entity.ApprovedQuantity == 0)
                    {
                        entity.ApprovedQuantity = entity.TotalRequestedAmount;
                    }
                    db.Queue.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { queue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Queue_Destroy([DataSourceRequest]DataSourceRequest request, Queue queue)
        {
            if (ModelState.IsValid)
            {
                var entity = new Queue
                {
                    QueueId = queue.QueueId,
                    QueueNumber = queue.QueueNumber,
                    TotalRequestedAmount = queue.TotalRequestedAmount,
                    WeeklyRequest = queue.WeeklyRequest,
                    ApprovedQuantity = queue.ApprovedQuantity,
                    BranchId = queue.BranchId,
                    LocationMapId = queue.LocationMapId,
                    Store = queue.Store,
                    QueueDate = queue.QueueDate,
                    QueueTypeId = queue.QueueTypeId,
                    QueueCustomerId = queue.QueueCustomerId,
                    QueueStatus = queue.QueueStatus,
                    AdvancePayment = queue.AdvancePayment,

                    Status = queue.Status,
                    IsVoid = queue.IsVoid,
                    IsOnlineApproved = queue.IsOnlineApproved,
                    OnlineApprovedBy = queue.OnlineApprovedBy,
                    IsOnlineTransferred = queue.IsOnlineTransferred,
                    IsOnlineTransferredBy = queue.IsOnlineTransferredBy,
                    OnlineApprovedTime = queue.OnlineApprovedTime,
                    OnlineTransferredTime = queue.OnlineTransferredTime,
                    IsSendForApproval = queue.IsSendForApproval,
                    SendForApprovalBy = queue.SendForApprovalBy,
                    SendForApprovalTime = queue.SendForApprovalTime,
                    VoidBy = queue.VoidBy,
                    VoidTime = queue.VoidTime,

                    CreatedBy = queue.CreatedBy,
                    DateCreated = queue.DateCreated,
                    LastModified = queue.LastModified,
                    ModifiedBy = queue.ModifiedBy,
                    OrgBranchName = queue.OrgBranchName,

                    Remark = queue.Remark
                };

                db.Queue.Attach(entity);
                db.Queue.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { queue }.ToDataSourceResult(request, ModelState));
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
        private int GetQueNumber(Queue queue)
        {
            int order = 1;
            var existingRecords = 
                db.Queue.Where(q => q.QueueTypeId == queue.QueueTypeId && 
                                    q.LocationMapId == queue.LocationMapId && 
                                    q.QueueId != queue.QueueId
                                    ).ToList();
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
            var record = db.Queue.Find(id);
            if ((record != null && !record.IsSendForApproval && !record.IsOnlineApproved))
            {
                
                record.Status = OperationStatuses.WaitingApproval.ToString();
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.Queue.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent.", Status = "OK" }, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var d = new
                {
                    Message = "Queue Record not found!",
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
