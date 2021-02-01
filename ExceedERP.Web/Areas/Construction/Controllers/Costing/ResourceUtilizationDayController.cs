using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ResourceUtilizationDayController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Days_Read([DataSourceRequest]DataSourceRequest request, int dayIdRead)
        {
            IQueryable<ResourceUtilizationDay> days = db.ResourceUtilizationDays.Where(x => x.ResourceUtilizationPeriodId == dayIdRead);
            DataSourceResult result = days.ToDataSourceResult(request, day => new
            {
                ResourceUtilizationDayId = day.ResourceUtilizationDayId,
                ResourceUtilizationPeriodId = day.ResourceUtilizationPeriodId,
                Name = day.Name,
                DateFrom = day.DateFrom,
                DateTo = day.DateTo,

                OrgBranchName = day.OrgBranchName,
                DateCreated = day.DateCreated,
                LastModified = day.LastModified,
                CreatedBy = day.CreatedBy,
                ModifiedBy = day.ModifiedBy
            });

            return Json(result);
        }

        //private bool PeriodExists(ResourceUtilizationPeriod model, int projectId)
        //{
        //    bool dayExists = false;
        //    var temp = db.ResourceUtilizationPeriods.FirstOrDefault(p => p.ConstructionPeriodId == model.ConstructionPeriodId && p.ProjectInformationId == projectId);
        //    if (temp != null)
        //    {
        //        dayExists = true;
        //    }
        //    return dayExists;
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Day_Create([DataSourceRequest]DataSourceRequest request, ResourceUtilizationDay day, int periodIdCreate)
        {
            if (ModelState.IsValid)
            {
                //if (PeriodExists(day, projectIdCreate))
                //{
                //    ModelState.AddModelError("", "Period Exists Already!!");
                //}
                //else
                //{
                var entity = new ResourceUtilizationDay
                {
                    ResourceUtilizationDayId = day.ResourceUtilizationDayId,
                    ResourceUtilizationPeriodId = day.ResourceUtilizationPeriodId,
                    Name = day.Name,
                    DateFrom = day.DateFrom,
                    DateTo = day.DateTo,

                    OrgBranchName = day.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = day.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = day.ModifiedBy

                };

                db.ResourceUtilizationDays.Add(entity);
                db.SaveChanges();
                day.ResourceUtilizationPeriodId = entity.ResourceUtilizationPeriodId;

                //}
            }

            return Json(new[] { day }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Day_Update([DataSourceRequest]DataSourceRequest request, ResourceUtilizationDay day)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationDay
                {
                    ResourceUtilizationDayId = day.ResourceUtilizationDayId,
                    ResourceUtilizationPeriodId = day.ResourceUtilizationPeriodId,
                    Name = day.Name,
                    DateFrom = day.DateFrom,
                    DateTo = day.DateTo,

                    OrgBranchName = day.OrgBranchName,
                    DateCreated = day.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = day.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ResourceUtilizationDays.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { day }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Day_Destroy([DataSourceRequest]DataSourceRequest request, ResourceUtilizationDay day)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationDay
                {
                    ResourceUtilizationDayId = day.ResourceUtilizationDayId,
                    ResourceUtilizationPeriodId = day.ResourceUtilizationPeriodId,
                    Name = day.Name,
                    DateFrom = day.DateFrom,
                    DateTo = day.DateTo,

                    OrgBranchName = day.OrgBranchName,
                    DateCreated = day.DateCreated,
                    LastModified = day.LastModified,
                    CreatedBy = day.CreatedBy,
                    ModifiedBy = day.ModifiedBy
                };

                db.ResourceUtilizationDays.Attach(entity);
                db.ResourceUtilizationDays.Remove(entity);
                db.SaveChanges();

                this.DeleteUtilizationWorkItems(entity);
            }

            return Json(new[] { day }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void DeleteUtilizationWorkItems(ResourceUtilizationDay day)
        {
            var utilizationWorkItems = db.ResourceUtilizationWorkItem
                .Where(wi => wi.ResourceUtilizationDayId == day.ResourceUtilizationDayId).ToList();
            if (utilizationWorkItems.Any())
            {
                db.ResourceUtilizationWorkItem.RemoveRange(utilizationWorkItems);
                db.SaveChanges();
            }
        }

    }
}