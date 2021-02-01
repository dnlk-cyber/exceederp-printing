﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class TakeOffLineForKgPerProjectController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return View();
        }

        public ActionResult TakeOffLineForKgPerProject_Read([DataSourceRequest]DataSourceRequest request, int idKg)
        {
            IQueryable<TakeOffLineForKgPerProject> takeofflineforkgperproject = db.TakeOffLineForKgPerProject.Where (x=> x.TakeOffPerProjectId ==idKg );
            DataSourceResult result = takeofflineforkgperproject.ToDataSourceResult(request, takeOffLineForKgPerProject => new {
                TakeOffLineForKgPerProjectId = takeOffLineForKgPerProject.TakeOffLineForKgPerProjectId,
                Description = takeOffLineForKgPerProject.Description,
                Type = takeOffLineForKgPerProject.Type,
                Shape = takeOffLineForKgPerProject.Shape,
                Length = takeOffLineForKgPerProject.Length,
                NoOfBars = takeOffLineForKgPerProject.NoOfBars,
                NoOfElement = takeOffLineForKgPerProject.NoOfElement,
                TotalBar = takeOffLineForKgPerProject.TotalBar,
                TotalLength = CalculateTotalBarLength(takeOffLineForKgPerProject),
                GrandTotal = takeOffLineForKgPerProject.GrandTotal,
                OrgBranchName = takeOffLineForKgPerProject.OrgBranchName,
                DateCreated = takeOffLineForKgPerProject.DateCreated,
                LastModified = takeOffLineForKgPerProject.LastModified,
                CreatedBy = takeOffLineForKgPerProject.CreatedBy,
                ModifiedBy = takeOffLineForKgPerProject.ModifiedBy,
                TakeOffPerProjectId = takeOffLineForKgPerProject.TakeOffPerProjectId,
                DeformedBarId = takeOffLineForKgPerProject.DeformedBarId,
                Location = takeOffLineForKgPerProject.Location,
                Diameter = GetBarDiameter(takeOffLineForKgPerProject),
                TotalWeight = CalculateBarWeight(GetBarUnitWeight(takeOffLineForKgPerProject), CalculateTotalBarLength(takeOffLineForKgPerProject))
                //UnitRateId = takeOffLineForKgPerProject.UnitRateId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TakeOffLineForKgPerProject_Create([DataSourceRequest]DataSourceRequest request, TakeOffLineForKgPerProject takeOffLineForKgPerProject, int idTwoKg)
        {
            if (ModelState.IsValid)
            {
                var entity = new TakeOffLineForKgPerProject
                {
                    Description = takeOffLineForKgPerProject.Description,
                    Type = takeOffLineForKgPerProject.Type,
                    Shape = takeOffLineForKgPerProject.Shape,
                    Length = takeOffLineForKgPerProject.Length,
                    NoOfBars = takeOffLineForKgPerProject.NoOfBars,
                    NoOfElement = takeOffLineForKgPerProject.NoOfElement,
                    TotalBar = takeOffLineForKgPerProject.TotalBar,
                    TotalLength = CalculateTotalBarLength(takeOffLineForKgPerProject),
                    GrandTotal = takeOffLineForKgPerProject.GrandTotal,
                    OrgBranchName = takeOffLineForKgPerProject.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = takeOffLineForKgPerProject.LastModified,
                    CreatedBy = User .Identity .Name ,
                    ModifiedBy = takeOffLineForKgPerProject.ModifiedBy,
                    TakeOffPerProjectId = idTwoKg,
                    DeformedBarId = takeOffLineForKgPerProject.DeformedBarId,
                    Location = takeOffLineForKgPerProject.Location,
                    Diameter = GetBarDiameter(takeOffLineForKgPerProject),
                    TotalWeight = CalculateBarWeight(GetBarUnitWeight(takeOffLineForKgPerProject), CalculateTotalBarLength(takeOffLineForKgPerProject))
                    //UnitRateId = takeOffLineForKgPerProject.UnitRateId,
                };

                db.TakeOffLineForKgPerProject.Add(entity);
                db.SaveChanges();
                takeOffLineForKgPerProject.TakeOffLineForKgPerProjectId = entity.TakeOffLineForKgPerProjectId;

                ProjectEstimationHelper.UpdateWorkItemPerProjecQuantityOfWork(entity.TakeOffPerProjectId);

            }

            return Json(new[] { takeOffLineForKgPerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TakeOffLineForKgPerProject_Update([DataSourceRequest]DataSourceRequest request, TakeOffLineForKgPerProject takeOffLineForKgPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new TakeOffLineForKgPerProject
                {
                    TakeOffLineForKgPerProjectId = takeOffLineForKgPerProject.TakeOffLineForKgPerProjectId,
                    Description = takeOffLineForKgPerProject.Description,
                    Type = takeOffLineForKgPerProject.Type,
                    Shape = takeOffLineForKgPerProject.Shape,
                    Length = takeOffLineForKgPerProject.Length,
                    NoOfBars = takeOffLineForKgPerProject.NoOfBars,
                    NoOfElement = takeOffLineForKgPerProject.NoOfElement,
                    TotalBar = takeOffLineForKgPerProject.TotalBar,
                    TotalLength = CalculateTotalBarLength(takeOffLineForKgPerProject),
                    GrandTotal = takeOffLineForKgPerProject.GrandTotal,
                    OrgBranchName = takeOffLineForKgPerProject.OrgBranchName,
                    DateCreated = takeOffLineForKgPerProject.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = takeOffLineForKgPerProject.CreatedBy,
                    ModifiedBy = User .Identity .Name ,
                    TakeOffPerProjectId = takeOffLineForKgPerProject.TakeOffPerProjectId,
                    DeformedBarId = takeOffLineForKgPerProject.DeformedBarId,
                    Location = takeOffLineForKgPerProject.Location,
                    Diameter = GetBarDiameter(takeOffLineForKgPerProject),
                    TotalWeight = CalculateBarWeight(GetBarUnitWeight(takeOffLineForKgPerProject), CalculateTotalBarLength(takeOffLineForKgPerProject))
                    //UnitRateId = takeOffLineForKgPerProject.UnitRateId,
                };

                db.TakeOffLineForKgPerProject.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationHelper.UpdateWorkItemPerProjecQuantityOfWork(entity.TakeOffPerProjectId);
            }

            return Json(new[] { takeOffLineForKgPerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TakeOffLineForKgPerProject_Destroy([DataSourceRequest]DataSourceRequest request, TakeOffLineForKgPerProject takeOffLineForKgPerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new TakeOffLineForKgPerProject
                {
                    TakeOffLineForKgPerProjectId = takeOffLineForKgPerProject.TakeOffLineForKgPerProjectId,
                    Description = takeOffLineForKgPerProject.Description,
                    Type = takeOffLineForKgPerProject.Type,
                    Shape = takeOffLineForKgPerProject.Shape,
                    Length = takeOffLineForKgPerProject.Length,
                    NoOfBars = takeOffLineForKgPerProject.NoOfBars,
                    NoOfElement = takeOffLineForKgPerProject.NoOfElement,
                    TotalBar = takeOffLineForKgPerProject.TotalBar,
                    TotalLength = CalculateTotalBarLength(takeOffLineForKgPerProject),
                    GrandTotal = takeOffLineForKgPerProject.GrandTotal,
                    OrgBranchName = takeOffLineForKgPerProject.OrgBranchName,
                    DateCreated = takeOffLineForKgPerProject.DateCreated,
                    LastModified = takeOffLineForKgPerProject.LastModified,
                    CreatedBy = takeOffLineForKgPerProject.CreatedBy,
                    ModifiedBy = takeOffLineForKgPerProject.ModifiedBy,
                    TakeOffPerProjectId = takeOffLineForKgPerProject.TakeOffPerProjectId,
                    DeformedBarId = takeOffLineForKgPerProject.DeformedBarId,
                    Location = takeOffLineForKgPerProject.Location,
                    Diameter = GetBarDiameter(takeOffLineForKgPerProject),
                    TotalWeight = CalculateBarWeight(GetBarUnitWeight(takeOffLineForKgPerProject), CalculateTotalBarLength(takeOffLineForKgPerProject))
                    //UnitRateId = takeOffLineForKgPerProject.UnitRateId,
                };

                db.TakeOffLineForKgPerProject.Attach(entity);
                db.TakeOffLineForKgPerProject.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.UpdateWorkItemPerProjecQuantityOfWork(entity.TakeOffPerProjectId);
            }

            return Json(new[] { takeOffLineForKgPerProject }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private decimal CalculateTotalBarLength(TakeOffLineForKgPerProject model) {
            decimal total = new decimal();
            if(model != null)
            {
                total = model.Length * model.NoOfBars * model.NoOfElement;
            }
            return total;
        }

        private decimal GetBarDiameter(TakeOffLineForKgPerProject model) {
            decimal diameter = new decimal();
            if(model != null)
            {
                var bar = db.DeformedBars.Find(model.DeformedBarId);
                diameter = bar.Diameter;
            }
            return diameter;
        }

        private decimal GetBarUnitWeight(TakeOffLineForKgPerProject model) {
            decimal unitWeight = new decimal();
            if (model != null)
            {
                var bar = db.DeformedBars.Find(model.DeformedBarId);
                unitWeight = bar.UnitWeight;
            }
            return unitWeight;
        }
        private decimal CalculateBarWeight(decimal unitWeight, decimal totalLength)
        {
            decimal weight = unitWeight * totalLength;
            weight = Math.Round(weight, 4);
            return weight;
            
        }
    }
}
