
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Letters;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Letter
{
   
     [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRLettersUser)]
    public class OutgoingStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
       
         [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRLettersUser)]
        public ActionResult OutgoingStatus_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<OutgoingStatus> outgoingstatus = db.OutgoingStatus.Where(o=> o.OutgoingId ==id);
            DataSourceResult result = outgoingstatus.ToDataSourceResult(request, outgoingStatus => new {
                StatusId = outgoingStatus.StatusId,
                OutgoingId = outgoingStatus.OutgoingId,
                NoOfAttachment = outgoingStatus.NoOfAttachment,
                Subject = outgoingStatus.Subject,
                Destination = outgoingStatus.Destination
            });

            return Json(result);
        }
       
         [AuthorizeRoles( HrmConstants.HRLettersUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutgoingStatus_Create([DataSourceRequest]DataSourceRequest request, OutgoingStatus outgoingStatus, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new OutgoingStatus
                {
                    OutgoingId = id,
                    NoOfAttachment = outgoingStatus.NoOfAttachment,
                    Subject = outgoingStatus.Subject,
                    Destination = outgoingStatus.Destination
                };

                db.OutgoingStatus.Add(entity);
                db.SaveChanges();
                outgoingStatus.StatusId = entity.StatusId;
            }

            return Json(new[] { outgoingStatus }.ToDataSourceResult(request, ModelState));
        }
    
         [AuthorizeRoles( HrmConstants.HRLettersUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutgoingStatus_Update([DataSourceRequest]DataSourceRequest request, OutgoingStatus outgoingStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new OutgoingStatus
                {
                    StatusId = outgoingStatus.StatusId,
                    OutgoingId = outgoingStatus.OutgoingId,
                    NoOfAttachment = outgoingStatus.NoOfAttachment,
                    Subject = outgoingStatus.Subject,
                    Destination = outgoingStatus.Destination
                };

                db.OutgoingStatus.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { outgoingStatus }.ToDataSourceResult(request, ModelState));
        }
      
         [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutgoingStatus_Destroy([DataSourceRequest]DataSourceRequest request, OutgoingStatus outgoingStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new OutgoingStatus
                {
                    StatusId = outgoingStatus.StatusId,
                    OutgoingId = outgoingStatus.OutgoingId,
                    NoOfAttachment = outgoingStatus.NoOfAttachment,
                    Subject = outgoingStatus.Subject,
                    Destination = outgoingStatus.Destination
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted outgoing status of  outgoing id" + outgoingStatus.OutgoingId);
                #endregion
                db.OutgoingStatus.Attach(entity);
                db.OutgoingStatus.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { outgoingStatus }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
