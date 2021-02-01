using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.SalesOrder
{
    public class ArSalesOrderStatusController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArSalesOrderStatus_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderStatus> arsalesorderstatus = _db.ArSalesOrderStatus.Where(x=>x.ArSalesOrderId==id);
            DataSourceResult result = arsalesorderstatus.ToDataSourceResult(request, arSalesOrderStatus => new {
                ArSalesOrderStatusId = arSalesOrderStatus.ArSalesOrderStatusId,
                Date = arSalesOrderStatus.Date,
                OrderStatus = arSalesOrderStatus.OrderStatus,
                OrgBranchName = arSalesOrderStatus.OrgBranchName,
                DateCreated = arSalesOrderStatus.DateCreated,
                LastModified = arSalesOrderStatus.LastModified,
                CreatedBy = arSalesOrderStatus.CreatedBy,
                ModifiedBy = arSalesOrderStatus.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderStatus_Create(int id,[DataSourceRequest]DataSourceRequest request, ArSalesOrderStatus arSalesOrderStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderStatus
                {
                    ArSalesOrderId=id,
                    Date = arSalesOrderStatus.Date,
                    OrderStatus = arSalesOrderStatus.OrderStatus,
                    OrgBranchName = arSalesOrderStatus.OrgBranchName,
                    DateCreated = arSalesOrderStatus.DateCreated,
                    LastModified = arSalesOrderStatus.LastModified,
                    CreatedBy = arSalesOrderStatus.CreatedBy,
                    ModifiedBy = arSalesOrderStatus.ModifiedBy
                };

                _db.ArSalesOrderStatus.Add(entity);
                _db.SaveChanges();
                arSalesOrderStatus.ArSalesOrderStatusId = entity.ArSalesOrderStatusId;

                #region update parent order status
                var salesorder = _db.ArSalesOrders.Find(id);
                if (salesorder != null) salesorder.OrderStatus = entity.OrderStatus;
                _db.Entry(salesorder).State = EntityState.Modified;
                _db.SaveChanges();
                #endregion
            }

            return Json(new[] { arSalesOrderStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderStatus_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderStatus arSalesOrderStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderStatus
                {
                    ArSalesOrderId = arSalesOrderStatus.ArSalesOrderId,
                    ArSalesOrderStatusId = arSalesOrderStatus.ArSalesOrderStatusId,
                    Date = arSalesOrderStatus.Date,
                    OrderStatus = arSalesOrderStatus.OrderStatus,
                    OrgBranchName = arSalesOrderStatus.OrgBranchName,
                    DateCreated = arSalesOrderStatus.DateCreated,
                    LastModified = arSalesOrderStatus.LastModified,
                    CreatedBy = arSalesOrderStatus.CreatedBy,
                    ModifiedBy = arSalesOrderStatus.ModifiedBy
                };

                _db.ArSalesOrderStatus.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region update parent order status
                var salesorder = _db.ArSalesOrders.Find(entity.ArSalesOrderId);
                if (salesorder != null) salesorder.OrderStatus = entity.OrderStatus;
                _db.Entry(salesorder).State = EntityState.Modified;
                _db.SaveChanges();
                #endregion
            }

            return Json(new[] { arSalesOrderStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderStatus_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderStatus arSalesOrderStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderStatus
                {
                    ArSalesOrderId = arSalesOrderStatus.ArSalesOrderId,
                    ArSalesOrderStatusId = arSalesOrderStatus.ArSalesOrderStatusId,
                    Date = arSalesOrderStatus.Date,
                    OrderStatus = arSalesOrderStatus.OrderStatus,
                    OrgBranchName = arSalesOrderStatus.OrgBranchName,
                    DateCreated = arSalesOrderStatus.DateCreated,
                    LastModified = arSalesOrderStatus.LastModified,
                    CreatedBy = arSalesOrderStatus.CreatedBy,
                    ModifiedBy = arSalesOrderStatus.ModifiedBy
                };

                _db.ArSalesOrderStatus.Attach(entity);
                _db.ArSalesOrderStatus.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderStatus }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
