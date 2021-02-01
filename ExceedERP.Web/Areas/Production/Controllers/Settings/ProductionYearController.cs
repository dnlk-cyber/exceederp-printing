using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.Settings
{
    public class ProductionYearController : Controller
    {
        private ExceedDbContext db;
        public ProductionYearController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductionYears_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionYear> years = db.ProductionYears;
            DataSourceResult result = years.ToDataSourceResult(request, year => new
            {
                ProductionYearId = year.ProductionYearId,
                Name = year.Name,
                DateFrom = year.DateFrom,
                DateTo = year.DateTo,
                Status = year.Status,
                IsPermanentlyClosed = year.IsPermanentlyClosed,

                DateCreated = year.DateCreated,
                CreatedBy = year.CreatedBy,
                ModifiedBy = year.ModifiedBy,
                LastModified = year.LastModified,
                OrgBranchName = year.OrgBranchName
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionYear_Create([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ProductionYear> years)
        {
            if (ModelState.IsValid)
            {
                foreach (var year in years)
                {
                    var frm = year.DateFrom;
                    var to = year.DateTo;
                    if(to <= frm)
                    {
                        to = frm.AddDays(364);
                    }
                    var entity = new ProductionYear
                    {
                        ProductionYearId = year.ProductionYearId,
                        Name = year.Name,
                        DateFrom = new DateTime(frm.Year, frm.Month, frm.Day),
                        DateTo = new DateTime(to.Year, to.Month, to.Day),
                        Status = year.Status,
                        IsPermanentlyClosed = year.IsPermanentlyClosed,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = year.ModifiedBy,
                        LastModified = year.LastModified,
                        OrgBranchName = year.OrgBranchName
                    };
                    if(year.DateFrom > DateTime.Now)
                    {
                        entity.Status = FiscalYearStatus.FuturePeriod;
                    }
                    else if(to < DateTime.Now)
                    {
                        entity.Status = FiscalYearStatus.Close;
                    }
                    else
                    {
                        entity.Status = FiscalYearStatus.Open;
                    }
                    db.ProductionYears.Add(entity);
                    db.SaveChanges();
                    year.ProductionYearId = entity.ProductionYearId;
                }
            }

            return Json(years.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionYear_Update([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ProductionYear> years)
        {
            if (ModelState.IsValid)
            {
                foreach (var year in years)
                {
                    var frm = year.DateFrom;
                    var to = year.DateTo;
                    if (to <= frm)
                    {
                        to = frm.AddDays(364);
                    }
                    var entity = new ProductionYear
                    {
                        ProductionYearId = year.ProductionYearId,
                        Name = year.Name,
                        DateFrom = new DateTime(frm.Year, frm.Month, frm.Day),
                        DateTo = new DateTime(to.Year, to.Month, to.Day),
                        Status = year.Status,
                        IsPermanentlyClosed = year.IsPermanentlyClosed,

                        DateCreated = year.DateCreated,
                        CreatedBy = year.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now,
                        OrgBranchName = year.OrgBranchName
                    };

                    db.ProductionYears.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(years.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionYear_Destroy([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ProductionYear> years)
        {
            if (ModelState.IsValid)
            {
                foreach (var year in years)
                {
                    var entity = new ProductionYear
                    {
                        ProductionYearId = year.ProductionYearId,
                        Name = year.Name,
                        DateFrom = year.DateFrom,
                        DateTo = year.DateTo,
                        Status = year.Status,
                        IsPermanentlyClosed = year.IsPermanentlyClosed,

                        DateCreated = year.DateCreated,
                        CreatedBy = year.CreatedBy,
                        ModifiedBy = year.ModifiedBy,
                        LastModified = year.LastModified,
                        OrgBranchName = year.OrgBranchName
                    };


                    db.ProductionYears.Attach(entity);
                    db.ProductionYears.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(years.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}