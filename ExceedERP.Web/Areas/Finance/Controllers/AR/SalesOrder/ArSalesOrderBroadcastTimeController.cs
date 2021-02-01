using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.SalesOrder
{
    public class ArSalesOrderBroadcastTimeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArSalesOrderBroadcastTimes_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderBroadcastTime> arsalesorderbroadcasttimes = _db.ArSalesOrderBroadcastTimes.Where(x=>x.ArSalesOrderId==id);
            DataSourceResult result = arsalesorderbroadcasttimes.ToDataSourceResult(request, arSalesOrderBroadcastTime => new {
                ArSalesOrderBroadcastTimeId = arSalesOrderBroadcastTime.ArSalesOrderBroadcastTimeId,
                ArSalesOrderId = arSalesOrderBroadcastTime.ArSalesOrderId,
                BroadcastDateTime = arSalesOrderBroadcastTime.BroadcastDateTime,
                Remark = arSalesOrderBroadcastTime.Remark,
                OrgBranchName = arSalesOrderBroadcastTime.OrgBranchName,
                DateCreated = arSalesOrderBroadcastTime.DateCreated,
                LastModified = arSalesOrderBroadcastTime.LastModified,
                CreatedBy = arSalesOrderBroadcastTime.CreatedBy,
                ModifiedBy = arSalesOrderBroadcastTime.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderBroadcastTimes_Create(int id,[DataSourceRequest]DataSourceRequest request, ArSalesOrderBroadcastTime arSalesOrderBroadcastTime)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderBroadcastTime
                {
                    ArSalesOrderId=id,
                    BroadcastDateTime = arSalesOrderBroadcastTime.BroadcastDateTime,
                    Remark = arSalesOrderBroadcastTime.Remark,
                    OrgBranchName = arSalesOrderBroadcastTime.OrgBranchName,
                    DateCreated = arSalesOrderBroadcastTime.DateCreated,
                    LastModified = arSalesOrderBroadcastTime.LastModified,
                    CreatedBy = arSalesOrderBroadcastTime.CreatedBy,
                    ModifiedBy = arSalesOrderBroadcastTime.ModifiedBy
                };

                _db.ArSalesOrderBroadcastTimes.Add(entity);
                _db.SaveChanges();
                arSalesOrderBroadcastTime.ArSalesOrderBroadcastTimeId = entity.ArSalesOrderBroadcastTimeId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order broadcast item", "Success", User.Identity.Name, $"created broadcast item {entity.BroadcastDateTime} under record {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderBroadcastTime }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderBroadcastTimes_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderBroadcastTime arSalesOrderBroadcastTime)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderBroadcastTime
                {
                    ArSalesOrderBroadcastTimeId = arSalesOrderBroadcastTime.ArSalesOrderBroadcastTimeId,
                    BroadcastDateTime = arSalesOrderBroadcastTime.BroadcastDateTime,
                    Remark = arSalesOrderBroadcastTime.Remark,
                    OrgBranchName = arSalesOrderBroadcastTime.OrgBranchName,
                    DateCreated = arSalesOrderBroadcastTime.DateCreated,
                    LastModified = arSalesOrderBroadcastTime.LastModified,
                    CreatedBy = arSalesOrderBroadcastTime.CreatedBy,
                    ModifiedBy = arSalesOrderBroadcastTime.ModifiedBy
                };

                _db.ArSalesOrderBroadcastTimes.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order broadcast item", "Success", User.Identity.Name, $"updated broadcast item {entity.BroadcastDateTime} under record {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderBroadcastTime }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderBroadcastTimes_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderBroadcastTime arSalesOrderBroadcastTime)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderBroadcastTime
                {
                    ArSalesOrderBroadcastTimeId = arSalesOrderBroadcastTime.ArSalesOrderBroadcastTimeId,
                    BroadcastDateTime = arSalesOrderBroadcastTime.BroadcastDateTime,
                    Remark = arSalesOrderBroadcastTime.Remark,
                    OrgBranchName = arSalesOrderBroadcastTime.OrgBranchName,
                    DateCreated = arSalesOrderBroadcastTime.DateCreated,
                    LastModified = arSalesOrderBroadcastTime.LastModified,
                    CreatedBy = arSalesOrderBroadcastTime.CreatedBy,
                    ModifiedBy = arSalesOrderBroadcastTime.ModifiedBy
                };

                _db.ArSalesOrderBroadcastTimes.Attach(entity);
                _db.ArSalesOrderBroadcastTimes.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderBroadcastTime }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
