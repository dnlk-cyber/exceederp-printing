using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers
{
    public class ProductionPeriodController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            var glperiods = _db.GLPeriods.ToList();
            foreach (var period in glperiods)
            {
                var exist = _db.ProductionPeriods.FirstOrDefault(x => x.Name == period.Name);
                if (exist == null)
                {
                    var entity = new ProductionPeriod
                    {
                        Name = period.Name,
                        DateFrom = period.DateFrom,
                        DateTo = period.DateTo,
                        Status = period.Status,
                        OrgBranchName = period.OrgBranchName,
                        DateCreated = period.DateCreated,
                        LastModified = period.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = period.ModifiedBy
                    };
                    _db.ProductionPeriods.Add(entity);
                    _db.SaveChanges();
                }
            }
            return View();
        }

        public ActionResult Periods_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<ProductionPeriod> periods = _db.ProductionPeriods;
            DataSourceResult result = periods.ToDataSourceResult(request, gLPeriod => new
            {

                ProductionPeriodId = gLPeriod.ProductionPeriodId,
                Name = gLPeriod.Name,
                DateFrom = gLPeriod.DateFrom,
                DateTo = gLPeriod.DateTo,
                Status = gLPeriod.Status,
                OrgBranchName = gLPeriod.OrgBranchName,
                DateCreated = gLPeriod.DateCreated,
                LastModified = gLPeriod.LastModified,
                CreatedBy = gLPeriod.CreatedBy,
                ModifiedBy = gLPeriod.ModifiedBy
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Period_Create([DataSourceRequest]DataSourceRequest request, ProductionPeriod period)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionPeriod
                {
                    Name = period.Name,
                    DateFrom = period.DateFrom,
                    DateTo = period.DateTo,
                    Status = period.Status,
                    OrgBranchName = period.OrgBranchName,
                    DateCreated = period.DateCreated,
                    LastModified = period.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = period.ModifiedBy
                };
                _db.ProductionPeriods.Add(entity);
                _db.SaveChanges();
                period.ProductionPeriodId = entity.ProductionPeriodId;
            }
            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ClosePeriod(string id)
        {
            if (id != null)
            {
                var glPeriod = _db.ProductionPeriods.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status == FiscalYearStatus.Open)
                {
                    glPeriod.Status = FiscalYearStatus.Close;
                    _db.ProductionPeriods.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region  log
                    //UserHelper.MiscLog(Request.UserHostAddress, "Inventory Period Close", "Success", User.Identity.Name, "Period " + glPeriod.Name);
                    #endregion
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult OpenPeriod(string id)
        {
            if (id != null)
            {
                var glPeriod = _db.ProductionPeriods.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status != FiscalYearStatus.PermanetlyClosed)
                {
                    glPeriod.Status = FiscalYearStatus.Open;
                    _db.ProductionPeriods.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                    _db.SaveChanges();

                    //#region  log
                    //UserHelper.MiscLog(Request.UserHostAddress, "Inventory Period Open", "Success", User.Identity.Name, "Period " + glPeriod.Name);
                    //#endregion
                }


            }

            return RedirectToAction("Index");

        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}