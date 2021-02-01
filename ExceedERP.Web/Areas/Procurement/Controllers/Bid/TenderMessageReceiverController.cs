using System.Data.Entity;
using System.IO;
using System.Linq;
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
    [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser)]
    public class TenderMessageReceiverController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TenderMessages_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TenderMessages> tendermessages = _db.TenderMessages;
            DataSourceResult result = tendermessages.ToDataSourceResult(request, tenderMessages => new {
                TenderMessagesId = tenderMessages.TenderMessagesId,
                ReceiverDepartment = tenderMessages.ReceiverDepartment,
                Sender = tenderMessages.Sender,
                Receiver = tenderMessages.Receiver,
                Message = tenderMessages.Message,
                FileName = tenderMessages.FileName,
                PhysicalPath = tenderMessages.PhysicalPath,
                MessageStatus = tenderMessages.MessageStatus,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderMessages_Create([DataSourceRequest]DataSourceRequest request, TenderMessages tenderMessages)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderMessages
                {
                    ReceiverDepartment = tenderMessages.ReceiverDepartment,
                    Sender = tenderMessages.Sender,
                    Receiver = tenderMessages.Receiver,
                    Message = tenderMessages.Message,
                    FileName = tenderMessages.FileName,
                    PhysicalPath = tenderMessages.PhysicalPath,
                    MessageStatus = tenderMessages.MessageStatus,
                };

                _db.TenderMessages.Add(entity);
                _db.SaveChanges();
                tenderMessages.TenderMessagesId = entity.TenderMessagesId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender message receiver", "Success", User.Identity.Name, $"created receiver under tender {entity.TenderId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderMessages }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderMessages_Update([DataSourceRequest]DataSourceRequest request, TenderMessages tenderMessages)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderMessages
                {
                    TenderMessagesId = tenderMessages.TenderMessagesId,
                    ReceiverDepartment = tenderMessages.ReceiverDepartment,
                    Sender = tenderMessages.Sender,
                    Receiver = tenderMessages.Receiver,
                    Message = tenderMessages.Message,
                    FileName = tenderMessages.FileName,
                    PhysicalPath = tenderMessages.PhysicalPath,
                    MessageStatus = tenderMessages.MessageStatus,
                };

                _db.TenderMessages.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender message receiver", "Success", User.Identity.Name, $"updated receiver under tender {entity.TenderId} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderMessages }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderMessages_Destroy([DataSourceRequest]DataSourceRequest request, TenderMessages tenderMessages)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderMessages
                {
                    TenderMessagesId = tenderMessages.TenderMessagesId,
                    ReceiverDepartment = tenderMessages.ReceiverDepartment,
                    Sender = tenderMessages.Sender,
                    Receiver = tenderMessages.Receiver,
                    Message = tenderMessages.Message,
                    FileName = tenderMessages.FileName,
                    PhysicalPath = tenderMessages.PhysicalPath,
                    MessageStatus = tenderMessages.MessageStatus,
                };

                _db.TenderMessages.Attach(entity);
                _db.TenderMessages.Remove(entity);
                _db.SaveChanges();
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
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
