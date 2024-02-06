﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Manufacturing.Models;
using ExceedERP.Core.Domain.Manufacturing.JobCosting;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation.Setting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting

{
    //[AuthorizeRoles(JobCostingRoles.JobCostingDailyLaborRate)]
    public class FurnitureDLRSalaryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private UpdateDLR dlr = new UpdateDLR();
        private FurnitureDLRHour hr = new FurnitureDLRHour();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DLRSalaries_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureDLRSalary> dlrsalaries = db.FurnitureDLRSalaries.Where(qr=>qr.DailyLaborRateId == id);
            DataSourceResult result = dlrsalaries.ToDataSourceResult(request, dLRSalary => new {
                FurnitureDLRSalaryId = dLRSalary.FurnitureDLRSalaryId,
                DailyLaborRateId = dLRSalary.DailyLaborRateId,
                BasicSalary = dLRSalary.BasicSalary,
                LessIndirectLabor = dLRSalary.LessIndirectLabor,
                NetSalary = dLRSalary.NetSalary,
                LessAbsence = dLRSalary.LessAbsence,
                Total = dLRSalary.Total,
                Pension = dLRSalary.Pension,
                TotalSalary = dLRSalary.TotalSalary,
                NoOfDay = dLRSalary.NoOfDay,
                RatePerDay = dLRSalary.RatePerDay,
                NoOfEmployee = dLRSalary.NoOfEmployee,
                DateCreated = dLRSalary.DateCreated,
                LastModified = dLRSalary.LastModified,
                CreatedBy = dLRSalary.CreatedBy,
                ModifiedBy = dLRSalary.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult DLRSalaries_Create([DataSourceRequest]DataSourceRequest request, FurnitureDLRSalary dLRSalary, int id)
        {

            if (ModelState.IsValid)
            {
                var dlrRec = db.FurnitureDailyLaborRates.Find(id);
                var pensionRate = db.JcPensions.FirstOrDefault();
                if(pensionRate!=null)
                {
                    dLRSalary.Pension = Math.Round((decimal)dLRSalary.Total * pensionRate.Rate/100,3);

                }
                else
                {
                    dLRSalary.Pension = 0;
                }
                if (dlrRec != null && dlrRec.EmployeeType == FurnitureEmployementType.Permanent)
                {
                    dLRSalary.TotalSalary = dLRSalary.Pension + dLRSalary.Total;

                }
                else
                {
                    dLRSalary.TotalSalary = dLRSalary.Pension + dLRSalary.TotalSalary;

                }
                var entity = (FurnitureDLRSalary)this.GetObject(dLRSalary, true, id);
                dlr.UpdateDLRRow(entity, hr = null, db);
                db.FurnitureDLRSalaries.Add(entity);
                db.SaveChanges();
                dLRSalary.FurnitureDLRSalaryId = entity.FurnitureDLRSalaryId;
            }

            return Json(new[] { dLRSalary }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateSalary(FurnitureDLRSalary entity)
        {
             FurnitureDailyLaborRate dailyLaborRate = db.FurnitureDailyLaborRates
                                              .Find(entity.DailyLaborRateId);
            var TotalSalary = db.DLRSalaries
                                   .Where(q => q.DailyLaborRateId == entity.DailyLaborRateId).ToList().Sum(s => s.TotalSalary)+ entity.TotalSalary;
            var TotalHour = db.DLRHours
                                  .Where(q => q.DailyLaborRateId == entity.DailyLaborRateId).ToList().Sum(s => s.ExpectedTime);
            decimal? dlr = 0;
            if(TotalHour>0)
            {
                dlr = TotalSalary / TotalHour;
            }
            dailyLaborRate.TotalSalary = TotalSalary;
            dailyLaborRate.DLR = dlr;
            db.FurnitureDailyLaborRates.Attach(dailyLaborRate);
            db.Entry(dailyLaborRate).State = EntityState.Modified;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DLRSalaries_Update([DataSourceRequest]DataSourceRequest request, FurnitureDLRSalary dLRSalary)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureDLRSalary)this.GetObject(dLRSalary, false);

                db.FurnitureDLRSalaries.Attach(entity);
                dlr.UpdateDLRRow(entity, hr = null, db);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { dLRSalary }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DLRSalaries_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureDLRSalary dLRSalary)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureDLRSalary)this.GetObject(dLRSalary, false);
                db.FurnitureDLRSalaries.Attach(entity);
                dlr.UpdateDLRRow(entity, hr = null, db, false);
                db.FurnitureDLRSalaries.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { dLRSalary }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureDLRSalary dLRSalary, bool status, int id = 0)
        {
            if (status)
            {
                dLRSalary.DailyLaborRateId = id;
            }
            var entity = new FurnitureDLRSalary
            {
                FurnitureDLRSalaryId = dLRSalary.FurnitureDLRSalaryId,
                DailyLaborRateId = dLRSalary.DailyLaborRateId,
                BasicSalary = dLRSalary.BasicSalary,
                LessIndirectLabor = dLRSalary.LessIndirectLabor,
                NetSalary = dLRSalary.NetSalary,
                LessAbsence = dLRSalary.LessAbsence,
                Total = dLRSalary.Total,
                Pension = dLRSalary.Pension,
                TotalSalary = dLRSalary.TotalSalary,
                NoOfDay = dLRSalary.NoOfDay,
                RatePerDay = dLRSalary.RatePerDay,
                NoOfEmployee = dLRSalary.NoOfEmployee,
                DateCreated = dLRSalary.DateCreated,
                LastModified = dLRSalary.LastModified,
                CreatedBy = dLRSalary.CreatedBy,
                ModifiedBy = dLRSalary.ModifiedBy

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
