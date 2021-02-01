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
    public class ArSalesOrderPaymentScheduleController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArSalesOrderPaymentSchedules_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderPaymentSchedule> arsalesorderpaymentschedules = _db.ArSalesOrderPaymentSchedules.Where(x=>x.ArSalesOrderId==id);
            DataSourceResult result = arsalesorderpaymentschedules.ToDataSourceResult(request, arSalesOrderPaymentSchedule => new {
                ArSalesOrderPaymentScheduleId = arSalesOrderPaymentSchedule.ArSalesOrderPaymentScheduleId,
                Date = arSalesOrderPaymentSchedule.Date,
                Amount = arSalesOrderPaymentSchedule.Amount,
                Description = arSalesOrderPaymentSchedule.Description,
                Remark = arSalesOrderPaymentSchedule.Remark,
                OrgBranchName = arSalesOrderPaymentSchedule.OrgBranchName,
                DateCreated = arSalesOrderPaymentSchedule.DateCreated,
                LastModified = arSalesOrderPaymentSchedule.LastModified,
                CreatedBy = arSalesOrderPaymentSchedule.CreatedBy,
                ModifiedBy = arSalesOrderPaymentSchedule.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPaymentSchedules_Create(int id,[DataSourceRequest]DataSourceRequest request, ArSalesOrderPaymentSchedule arSalesOrderPaymentSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPaymentSchedule
                {
                    ArSalesOrderId=id,
                    Date = arSalesOrderPaymentSchedule.Date,
                    Amount = arSalesOrderPaymentSchedule.Amount,
                    Description = arSalesOrderPaymentSchedule.Description,
                    Remark = arSalesOrderPaymentSchedule.Remark,
                    OrgBranchName = arSalesOrderPaymentSchedule.OrgBranchName,
                    DateCreated = arSalesOrderPaymentSchedule.DateCreated,
                    LastModified = arSalesOrderPaymentSchedule.LastModified,
                    CreatedBy = arSalesOrderPaymentSchedule.CreatedBy,
                    ModifiedBy = arSalesOrderPaymentSchedule.ModifiedBy
                };

                _db.ArSalesOrderPaymentSchedules.Add(entity);
                _db.SaveChanges();
                arSalesOrderPaymentSchedule.ArSalesOrderPaymentScheduleId = entity.ArSalesOrderPaymentScheduleId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order payment schedule item", "Success", User.Identity.Name, $"created sales order payment schedule item {entity.Description} under record {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderPaymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPaymentSchedules_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderPaymentSchedule arSalesOrderPaymentSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPaymentSchedule
                {
                    ArSalesOrderPaymentScheduleId = arSalesOrderPaymentSchedule.ArSalesOrderPaymentScheduleId,
                    ArSalesOrderId = arSalesOrderPaymentSchedule.ArSalesOrderId,
                    Date = arSalesOrderPaymentSchedule.Date,
                    Amount = arSalesOrderPaymentSchedule.Amount,
                    Description = arSalesOrderPaymentSchedule.Description,
                    Remark = arSalesOrderPaymentSchedule.Remark,
                    OrgBranchName = arSalesOrderPaymentSchedule.OrgBranchName,
                    DateCreated = arSalesOrderPaymentSchedule.DateCreated,
                    LastModified = arSalesOrderPaymentSchedule.LastModified,
                    CreatedBy = arSalesOrderPaymentSchedule.CreatedBy,
                    ModifiedBy = arSalesOrderPaymentSchedule.ModifiedBy
                };

                _db.ArSalesOrderPaymentSchedules.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order payment schedule item", "Success", User.Identity.Name, $"updated sales order payment schedule item {entity.Description} under record {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderPaymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPaymentSchedules_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderPaymentSchedule arSalesOrderPaymentSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPaymentSchedule
                {
                    ArSalesOrderPaymentScheduleId = arSalesOrderPaymentSchedule.ArSalesOrderPaymentScheduleId,
                    Date = arSalesOrderPaymentSchedule.Date,
                    Amount = arSalesOrderPaymentSchedule.Amount,
                    Description = arSalesOrderPaymentSchedule.Description,
                    Remark = arSalesOrderPaymentSchedule.Remark,
                    OrgBranchName = arSalesOrderPaymentSchedule.OrgBranchName,
                    DateCreated = arSalesOrderPaymentSchedule.DateCreated,
                    LastModified = arSalesOrderPaymentSchedule.LastModified,
                    CreatedBy = arSalesOrderPaymentSchedule.CreatedBy,
                    ModifiedBy = arSalesOrderPaymentSchedule.ModifiedBy
                };

                _db.ArSalesOrderPaymentSchedules.Attach(entity);
                _db.ArSalesOrderPaymentSchedules.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderPaymentSchedule }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
