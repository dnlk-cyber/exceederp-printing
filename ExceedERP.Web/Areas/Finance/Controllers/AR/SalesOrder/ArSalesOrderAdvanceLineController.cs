using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.SalesOrder
{
    public class ArSalesOrderAdvanceLineController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

      

        public ActionResult ArSalesOrderAdvanceLines_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderAdvanceLine> arsalesorderadvancelines = _db.ArSalesOrderAdvanceLines;
            DataSourceResult result = arsalesorderadvancelines.ToDataSourceResult(request, arSalesOrderAdvanceLine => new {
                ArSalesOrderAdvanceLineId = arSalesOrderAdvanceLine.ArSalesOrderAdvanceLineId,
                DepositDate = arSalesOrderAdvanceLine.DepositDate,
                Reason = arSalesOrderAdvanceLine.Reason,
                Amount = arSalesOrderAdvanceLine.Amount,
                Reference = arSalesOrderAdvanceLine.Reference,
                OrgBranchName = arSalesOrderAdvanceLine.OrgBranchName,
                DateCreated = arSalesOrderAdvanceLine.DateCreated,
                LastModified = arSalesOrderAdvanceLine.LastModified,
                CreatedBy = arSalesOrderAdvanceLine.CreatedBy,
                ModifiedBy = arSalesOrderAdvanceLine.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderAdvanceLines_Create([DataSourceRequest]DataSourceRequest request, ArSalesOrderAdvanceLine arSalesOrderAdvanceLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderAdvanceLine
                {
                    DepositDate = arSalesOrderAdvanceLine.DepositDate,
                    Reason = arSalesOrderAdvanceLine.Reason,
                    Amount = arSalesOrderAdvanceLine.Amount,
                    Reference = arSalesOrderAdvanceLine.Reference,
                    OrgBranchName = arSalesOrderAdvanceLine.OrgBranchName,
                    DateCreated = arSalesOrderAdvanceLine.DateCreated,
                    LastModified = arSalesOrderAdvanceLine.LastModified,
                    CreatedBy = arSalesOrderAdvanceLine.CreatedBy,
                    ModifiedBy = arSalesOrderAdvanceLine.ModifiedBy
                };

                _db.ArSalesOrderAdvanceLines.Add(entity);
                _db.SaveChanges();
                arSalesOrderAdvanceLine.ArSalesOrderAdvanceLineId = entity.ArSalesOrderAdvanceLineId;
            }

            return Json(new[] { arSalesOrderAdvanceLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderAdvanceLines_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderAdvanceLine arSalesOrderAdvanceLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderAdvanceLine
                {
                    ArSalesOrderAdvanceLineId = arSalesOrderAdvanceLine.ArSalesOrderAdvanceLineId,
                    DepositDate = arSalesOrderAdvanceLine.DepositDate,
                    Reason = arSalesOrderAdvanceLine.Reason,
                    Amount = arSalesOrderAdvanceLine.Amount,
                    Reference = arSalesOrderAdvanceLine.Reference,
                    OrgBranchName = arSalesOrderAdvanceLine.OrgBranchName,
                    DateCreated = arSalesOrderAdvanceLine.DateCreated,
                    LastModified = arSalesOrderAdvanceLine.LastModified,
                    CreatedBy = arSalesOrderAdvanceLine.CreatedBy,
                    ModifiedBy = arSalesOrderAdvanceLine.ModifiedBy
                };

                _db.ArSalesOrderAdvanceLines.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderAdvanceLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderAdvanceLines_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderAdvanceLine arSalesOrderAdvanceLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderAdvanceLine
                {
                    ArSalesOrderAdvanceLineId = arSalesOrderAdvanceLine.ArSalesOrderAdvanceLineId,
                    DepositDate = arSalesOrderAdvanceLine.DepositDate,
                    Reason = arSalesOrderAdvanceLine.Reason,
                    Amount = arSalesOrderAdvanceLine.Amount,
                    Reference = arSalesOrderAdvanceLine.Reference,
                    OrgBranchName = arSalesOrderAdvanceLine.OrgBranchName,
                    DateCreated = arSalesOrderAdvanceLine.DateCreated,
                    LastModified = arSalesOrderAdvanceLine.LastModified,
                    CreatedBy = arSalesOrderAdvanceLine.CreatedBy,
                    ModifiedBy = arSalesOrderAdvanceLine.ModifiedBy
                };

                _db.ArSalesOrderAdvanceLines.Attach(entity);
                _db.ArSalesOrderAdvanceLines.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderAdvanceLine }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
