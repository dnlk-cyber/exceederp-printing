using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
    public class TenderMessageController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        private static string _filePath;
        private static string _uploadedFileName;

        public void UploadDocument([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> tenderdocument, string id)
        {
            int tendId = int.Parse(id);
            if (tenderdocument != null)
            {

                string subPath = "~/App_Data/Uploads/Procurement/TenderMessages/" + "Tender" + id.ToString();

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));

                foreach (var file in tenderdocument)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string physicalPath = Server.MapPath(subPath + "/" + fileName);
                    _uploadedFileName = fileName;
                    file.SaveAs(physicalPath);
                    _filePath = physicalPath;

                }


            }
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult TenderMessages_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TenderMessages> tendermessages = _db.TenderMessages;
            DataSourceResult result = tendermessages.ToDataSourceResult(request, tenderMessages => new
            {
                TenderMessagesId = tenderMessages.TenderMessagesId,
                TenderId = tenderMessages.TenderId,
                Sender = tenderMessages.Sender,
                Receiver = tenderMessages.Receiver,
                Message = tenderMessages.Message,
                FileName = tenderMessages.FileName,
                PhysicalPath = tenderMessages.PhysicalPath,
                MessageStatus = tenderMessages.MessageStatus,
                SentDate = tenderMessages.SentDate,
                ReceiveDate = tenderMessages.ReceiveDate
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderMessages_Create([DataSourceRequest]DataSourceRequest request, TenderMessages tenderMessages, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderMessages
                {

                    FileName = _uploadedFileName,
                    TenderId = id,
                    MessageStatus = MessageStatus.NotSeen,
                    PhysicalPath = _filePath,
                    Sender = User.Identity.Name,
                    Receiver = tenderMessages.Receiver,
                    ReceiverDepartment = tenderMessages.ReceiverDepartment,
                    Message = tenderMessages.Message,
                    SentDate = DateTime.Now,
                    ReceiveDate = DateTime.Now// should be nullable
                };

                _db.TenderMessages.Add(entity);
                _db.SaveChanges();
                tenderMessages.TenderMessagesId = entity.TenderMessagesId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender message", "Success", User.Identity.Name, $"created message {entity.TenderMessagesId} under record {entity.TenderId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderMessages }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderMessages_Update([DataSourceRequest]DataSourceRequest request, TenderMessages tenderMessages)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderMessages
                {
                    TenderMessagesId = tenderMessages.TenderMessagesId,
                    Sender = tenderMessages.Sender,
                    TenderId = tenderMessages.TenderId,
                    Receiver = tenderMessages.Receiver,
                    FileName = tenderMessages.FileName,
                    Message = tenderMessages.Message,
                    PhysicalPath = tenderMessages.PhysicalPath,
                    MessageStatus = tenderMessages.MessageStatus,
                    SentDate = tenderMessages.SentDate,
                    ReceiveDate = tenderMessages.ReceiveDate
                };

                _db.TenderMessages.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender message", "Success", User.Identity.Name, $"updated message {entity.TenderMessagesId} under record {entity.TenderId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderMessages }.ToDataSourceResult(request, ModelState));
        }
        
        [HttpGet]
        public ActionResult FileDwonload(int id)
        {
            TenderMessages record = _db.TenderMessages.Find(id);
            record.MessageStatus = MessageStatus.Seen;
            _db.Entry(record).State = EntityState.Modified;
            _db.SaveChanges();
            return File(record.PhysicalPath, "application/force-download", Path.GetFileName(record.PhysicalPath));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderMessages_Destroy([DataSourceRequest]DataSourceRequest request, TenderMessages tenderMessages)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderMessages
                {
                    TenderMessagesId = tenderMessages.TenderMessagesId,
                    Sender = tenderMessages.Sender,
                    TenderId = tenderMessages.TenderId,
                    FileName = tenderMessages.FileName,
                    Receiver = tenderMessages.Receiver,
                    Message = tenderMessages.Message,
                    PhysicalPath = tenderMessages.PhysicalPath,
                    MessageStatus = tenderMessages.MessageStatus,
                    SentDate = tenderMessages.SentDate,
                    ReceiveDate = tenderMessages.ReceiveDate
                };

                _db.TenderMessages.Attach(entity);
                _db.TenderMessages.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderMessages }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
