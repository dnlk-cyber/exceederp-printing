using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class ReceiveTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReceiveTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ReceiveType> receiveTypes = db.ReceiveTypes;
            DataSourceResult result = receiveTypes.ToDataSourceResult(request, receiveType => new {
                ReceiveTypeId = receiveType.ReceiveTypeId,
                Name = receiveType.Name,
                Type = receiveType.Type,
                Desctiption = receiveType.Desctiption,
                Status = receiveType.Status
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveTypes_Create([DataSourceRequest]DataSourceRequest request, ReceiveType receiveType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReceiveType
                {
                    Name = receiveType.Name,
                    Type = receiveType.Type,
                    Desctiption = receiveType.Desctiption,
                    Status = receiveType.Status
                };

                db.ReceiveTypes.Add(entity);
                db.SaveChanges();
                receiveType.ReceiveTypeId = entity.ReceiveTypeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "receive type ", "Success", User.Identity.Name, $"created receive type {entity.Name} of type {entity.Type.ToString()}  ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { receiveType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveTypes_Update([DataSourceRequest]DataSourceRequest request, ReceiveType receiveType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReceiveType
                {
                    ReceiveTypeId = receiveType.ReceiveTypeId,
                    Name = receiveType.Name,
                    Type = receiveType.Type,
                    Desctiption = receiveType.Desctiption,
                    Status = receiveType.Status
                };

                db.ReceiveTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "receive type ", "Success", User.Identity.Name, $"created receive type {entity.Name} of type {entity.Type.ToString()}  ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { receiveType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveTypes_Destroy([DataSourceRequest]DataSourceRequest request, ReceiveType receiveType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReceiveType
                {
                    ReceiveTypeId = receiveType.ReceiveTypeId,
                    Name = receiveType.Name,
                    Type = receiveType.Type,
                    Desctiption = receiveType.Desctiption,
                    Status = receiveType.Status
                };

                db.ReceiveTypes.Attach(entity);
                db.ReceiveTypes.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted receive type setting " + receiveType.ReceiveTypeId + " " + receiveType.Name);
                #endregion
            }

            return Json(new[] { receiveType }.ToDataSourceResult(request, ModelState));
        }

        

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
