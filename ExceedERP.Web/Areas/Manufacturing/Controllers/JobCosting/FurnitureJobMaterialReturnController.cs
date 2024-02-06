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
using ExceedERP.Core.Domain.Manufacturing.JobCosting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting

{
    //[AuthorizeRoles(JobCostingRoles.JobCostingJobCostUser)]
    public class FurnitureJobMaterialReturnController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobMaterialReturns_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IList<FurnitureJobMaterialReturn> jobmaterialreturns = new List<FurnitureJobMaterialReturn>();
            var jobCost = db.FurnitureJobCosts.Find(id);
            if (jobCost != null)
            {
                var job = db.Jobs.Find(jobCost.JobId);
                if(job != null)
                {
                    var items = db.StoreReturnItemss.Where(q => q.ProductionJobNo == job.JobNo).ToList();
                    foreach (var item in items)
                    {
                        var dt = DateTime.Now;
                        FurnitureJobMaterialReturn jobMaterialReturn = new FurnitureJobMaterialReturn();

                        jobMaterialReturn.FurnitureJobMaterialReturnId = item.StoreReturnItemsID;
                        jobMaterialReturn.FurnitureJobCostId = id;
                        jobMaterialReturn.ItemCode = item.ItemCode;
                        jobMaterialReturn.ItemCategoryCode = item.ItemCategoryCode;
                        jobMaterialReturn.Date = dt;
                        jobMaterialReturn.Reference = item.ProductionJobNo;
                        jobMaterialReturn.Quantity = item.Quantity;
                        jobMaterialReturn.UnitPrice = item.UnitPrice;
                        jobMaterialReturn.Cost = item.Quantity * item.UnitPrice;
                        jobMaterialReturn.SubTotal = item.UnitPrice * item.Quantity;
                        jobMaterialReturn.TaxAmount = item.Quantity * item.UnitPrice - (item.UnitPrice * item.Quantity);
                        jobMaterialReturn.ItemDescription = jobCost.Description;
                        jobMaterialReturn.DateCreated = item.DateCreated;
                        jobMaterialReturn.LastModified = item.LastModified;
                        jobMaterialReturn.CreatedBy = item.CreatedBy;
                        jobMaterialReturn.ModifiedBy = item.ModifiedBy;
                        jobmaterialreturns.Add(jobMaterialReturn);
                    }
                }
                

            }
            DataSourceResult result = jobmaterialreturns.ToDataSourceResult(request, jobMaterialReturn => new {
                JobMaterialReturnId = jobMaterialReturn.FurnitureJobMaterialReturnId,
                JobCostId = jobMaterialReturn.FurnitureJobCostId,
                ItemCategoryCode = jobMaterialReturn.ItemCategoryCode,
                ItemCode = jobMaterialReturn.ItemCode,
                Date = jobMaterialReturn.Date,
                Reference = jobMaterialReturn.Reference,
                Quantity = jobMaterialReturn.Quantity,
                UnitPrice = jobMaterialReturn.UnitPrice,
                TaxAmount = jobMaterialReturn.TaxAmount,
                SubTotal = jobMaterialReturn.SubTotal,
                Cost = jobMaterialReturn.Cost,
                ItemDescription = jobMaterialReturn.ItemDescription,
                DateCreated = jobMaterialReturn.DateCreated,
                LastModified = jobMaterialReturn.LastModified,
                CreatedBy = jobMaterialReturn.CreatedBy,
                ModifiedBy = jobMaterialReturn.ModifiedBy
            });

            return Json(result,JsonRequestBehavior.AllowGet);
        }

        public ActionResult JobMaterialReturns_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobMaterialReturn jobMaterialReturn, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobMaterialReturn)this.GetObject(jobMaterialReturn, true, id);
                db.FurnitureJobMaterialReturns.Add(entity);
                db.SaveChanges();
                jobMaterialReturn.FurnitureJobMaterialReturnId = entity.FurnitureJobMaterialReturnId;
            }

            return Json(new[] { jobMaterialReturn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobMaterialReturns_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobMaterialReturn jobMaterialReturn)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobMaterialReturn)this.GetObject(jobMaterialReturn, false);

                db.FurnitureJobMaterialReturns.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobMaterialReturn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobMaterialReturns_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobMaterialReturn jobMaterialReturn)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobMaterialReturn)this.GetObject(jobMaterialReturn, false);

                db.FurnitureJobMaterialReturns.Attach(entity);
                db.FurnitureJobMaterialReturns.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobMaterialReturn }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureJobMaterialReturn jobMaterialReturn, bool status, int id = 0)
        {
            if (status)
            {
                jobMaterialReturn.FurnitureJobCostId = id;
            }
            var entity = new FurnitureJobMaterialReturn
            {
                FurnitureJobMaterialReturnId = jobMaterialReturn.FurnitureJobMaterialReturnId,
                FurnitureJobCostId = jobMaterialReturn.FurnitureJobCostId,
                ItemCategoryCode = jobMaterialReturn.ItemCategoryCode,
                ItemCode = jobMaterialReturn.ItemCode,
                Date = jobMaterialReturn.Date,
                Reference = jobMaterialReturn.Reference,
                Quantity = jobMaterialReturn.Quantity,
                UnitPrice = jobMaterialReturn.UnitPrice,
                TaxAmount = jobMaterialReturn.TaxAmount,
                SubTotal = jobMaterialReturn.SubTotal,
                Cost = jobMaterialReturn.Cost,
                ItemDescription = jobMaterialReturn.ItemDescription,
                DateCreated = jobMaterialReturn.DateCreated,
                LastModified = jobMaterialReturn.LastModified,
                CreatedBy = jobMaterialReturn.CreatedBy,
                ModifiedBy = jobMaterialReturn.ModifiedBy
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