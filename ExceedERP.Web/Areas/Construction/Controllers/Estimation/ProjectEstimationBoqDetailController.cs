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
﻿using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectEstimationBoqDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int ProjectId { get; set; }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectEstimationBoqDetail_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            //IQueryable<ProjectEstimationBoqDetail> projectestimationboqdetail =
            //    db.ProjectEstimationBoqDetail.Where(x => x.ProjectEstimationBoqBreakdownId == id);
            //if (!projectestimationboqdetail.Any())
            //{
            //    var detail = new ProjectEstimationBoqDetail();
            //    var workItems =
            //        db.ProjectEstimationWorkItem.Where(x => x.ProjectType == (AvailableProjectTypes)ProjectEstimationBoqBreakdownController.ProjectType )
            //            .OrderBy(x => x.UnitOfMeasurementId)
            //            .ToList();

            //    var details = new List<ProjectEstimationBoqDetail>();
            //    if (workItems.Any())
            //    {
            //        var project = new ProjectEstimationBoqBreakdown();

            //        for (int w = 0; w < workItems.Count; w++)
            //        {
            //            int wrokItemid = workItems[w].ProjectEstimationWorkItemId;
            //            var takeoffHeader =
            //                db.ProjectTakeOff.Where(
            //                    x => x.ProjectEstimationWorkItemId == wrokItemid)
            //                    .FirstOrDefault();
            //            if (takeoffHeader != null)
            //            {

            //                var takofflist =
            //                    db.ProjectTakeOffLine.Where(x => x.ProjectTakeOffId == takeoffHeader.ProjectTakeOffId)
            //                        .ToList();
            //                if (takofflist.Any())
            //                {
            //                    foreach (var t in takofflist)
            //                    {
            //                        detail = new ProjectEstimationBoqDetail();
            //                        detail.UnitOfMeasurementId = workItems[w].UnitOfMeasurementId?? 0;
            //                        detail.UnitRate = workItems[w].TotalUnitCost;
            //                        detail.Quntity = t.S;
            //                        detail.TotalCost = detail.Quntity*detail.UnitRate;
            //                        detail.Description = t.Description;
            //                        details.Add(detail);

            //                    }
            //                }
            //                var takoffKglist =
            //                    db.ProjectTakeOffLineForKg.Where(
            //                        x => x.ProjectTakeOffId == takeoffHeader.ProjectTakeOffId).ToList();
            //                if (takoffKglist.Any())
            //                {
            //                    foreach (var t in takoffKglist)
            //                    {
            //                        detail = new ProjectEstimationBoqDetail();
            //                        detail.UnitOfMeasurementId  = workItems[w].UnitOfMeasurementId?? 0;
            //                        detail.UnitRate = workItems[w].TotalUnitCost;
            //                        detail.Quntity = t.GrandTotal;
            //                        detail.TotalCost = detail.Quntity*detail.UnitRate;
            //                        detail.Description = t.Description;
            //                        details.Add(detail);

            //                    }
            //                }
            //            }
            //        }
            //    }
            //    projectestimationboqdetail = details.AsQueryable();
            //}
            IQueryable<ProjectEstimationBoqDetail> projectestimationboqdetails = db.ProjectEstimationBoqDetail.Where(d => d.ProjectEstimationBoqBreakdownId == id);
            DataSourceResult result = projectestimationboqdetails.ToDataSourceResult(request, projectEstimationBoqDetail => new {
                ProjectEstmnBoqDetailId = projectEstimationBoqDetail.ProjectEstmnBoqDetailId,
                BillNumber = projectEstimationBoqDetail.BillNumber,
                Description = projectEstimationBoqDetail.Description,
                TotalCost = projectEstimationBoqDetail.TotalCost,
                ProjectEstimationBoqBreakdownId = projectEstimationBoqDetail.ProjectEstimationBoqBreakdownId,
                UnitOfMeasurementId = projectEstimationBoqDetail.UnitOfMeasurementId,
                Quntity = projectEstimationBoqDetail.Quntity,
                UnitRate = projectEstimationBoqDetail.UnitRate,

                OrgBranchName = projectEstimationBoqDetail.OrgBranchName,
                DateCreated = projectEstimationBoqDetail.DateCreated,
                LastModified = projectEstimationBoqDetail.LastModified,
                CreatedBy = projectEstimationBoqDetail.CreatedBy,
                ModifiedBy = projectEstimationBoqDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationBoqDetail_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationBoqDetail projectEstimationBoqDetail,int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationBoqDetail
                {
                    Description = projectEstimationBoqDetail.Description,
                    TotalCost = projectEstimationBoqDetail.TotalCost,
                    ProjectEstimationBoqBreakdownId = id,
                    UnitOfMeasurementId = projectEstimationBoqDetail.UnitOfMeasurementId,
                    Quntity = projectEstimationBoqDetail.Quntity,
                    UnitRate = projectEstimationBoqDetail.UnitRate,
                    OrgBranchName = projectEstimationBoqDetail.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = projectEstimationBoqDetail.LastModified,
                    CreatedBy =User.Identity .Name ,
                    ModifiedBy = projectEstimationBoqDetail.ModifiedBy
                };

                db.ProjectEstimationBoqDetail.Add(entity);
                db.SaveChanges();
                projectEstimationBoqDetail.ProjectEstmnBoqDetailId = entity.ProjectEstmnBoqDetailId;
            }

            return Json(new[] { projectEstimationBoqDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationBoqDetail_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationBoqDetail projectEstimationBoqDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationBoqDetail
                {
                    ProjectEstmnBoqDetailId = projectEstimationBoqDetail.ProjectEstmnBoqDetailId,
                    Description = projectEstimationBoqDetail.Description,
                    TotalCost = projectEstimationBoqDetail.TotalCost,
                    ProjectEstimationBoqBreakdownId = projectEstimationBoqDetail.ProjectEstimationBoqBreakdownId,
                    UnitOfMeasurementId = projectEstimationBoqDetail.UnitOfMeasurementId,
                    Quntity = projectEstimationBoqDetail.Quntity,
                    UnitRate = projectEstimationBoqDetail.UnitRate,
                    OrgBranchName = projectEstimationBoqDetail.OrgBranchName,
                    DateCreated = projectEstimationBoqDetail.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectEstimationBoqDetail.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.ProjectEstimationBoqDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationBoqDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationBoqDetail_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationBoqDetail projectEstimationBoqDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationBoqDetail
                {
                    ProjectEstmnBoqDetailId = projectEstimationBoqDetail.ProjectEstmnBoqDetailId,
                    Description = projectEstimationBoqDetail.Description,
                    TotalCost = projectEstimationBoqDetail.TotalCost,
                    ProjectEstimationBoqBreakdownId = projectEstimationBoqDetail.ProjectEstimationBoqBreakdownId,
                    UnitOfMeasurementId = projectEstimationBoqDetail.UnitOfMeasurementId,
                    Quntity = projectEstimationBoqDetail.Quntity,
                    UnitRate = projectEstimationBoqDetail.UnitRate,
                    OrgBranchName = projectEstimationBoqDetail.OrgBranchName,
                    DateCreated = projectEstimationBoqDetail.DateCreated,
                    LastModified = projectEstimationBoqDetail.LastModified,
                    CreatedBy = projectEstimationBoqDetail.CreatedBy,
                    ModifiedBy = projectEstimationBoqDetail.ModifiedBy
                };

                db.ProjectEstimationBoqDetail.Attach(entity);
                db.ProjectEstimationBoqDetail.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationBoqDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
