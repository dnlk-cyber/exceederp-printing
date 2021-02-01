using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class WorkItemTransportCostController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/WorkItemTransportCost
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WorkItemTransportCost_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var currentWorkItem = db.GenericWorkItemPerProject.Find(id);
            List<GenericWorkItemTransportCostViewwModel> list = new List<GenericWorkItemTransportCostViewwModel>();
            if (currentWorkItem != null && currentWorkItem.HasAdditionalTransportationCost)
            {
                var record = new GenericWorkItemTransportCostViewwModel
                {
                    ItemId = currentWorkItem.GenericWorkItemPerProjectId,
                    TotalMaterailCost = currentWorkItem.MaterialCostPerUnit,
                    CalculatedTransportCost = currentWorkItem.CalculatedTransportCost,
                    TransportCostRate = currentWorkItem.TransportCostRate
                };
                list.Add(record);
            }
            return Json(list.AsQueryable().ToDataSourceResult(request));
        }
        public ActionResult WorkItemTransportCost_Create([DataSourceRequest]DataSourceRequest request, GenericWorkItemTransportCostViewwModel workItemTransportCost, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = db.GenericWorkItemPerProject.Find(id);
                if (entity != null)
                {
                    entity.HasAdditionalTransportationCost = true;
                    entity.TransportCostRate = workItemTransportCost.TransportCostRate;
                    entity.CalculatedTransportCost = workItemTransportCost.CalculatedTransportCost;
                    entity.TotalUnitCostIncludingTransport = entity.TotalUnitCost + entity.CalculatedTransportCost;

                    db.GenericWorkItemPerProject.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { workItemTransportCost }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult WorkItemTransportCost_Update([DataSourceRequest]DataSourceRequest request, GenericWorkItemTransportCostViewwModel workItemTransportCost)
        {
            if (ModelState.IsValid)
            {
                var entity = db.GenericWorkItemPerProject.Find(workItemTransportCost.ItemId);
                if(entity != null)
                {
                    entity.TransportCostRate = workItemTransportCost.TransportCostRate;
                    entity.CalculatedTransportCost = workItemTransportCost.CalculatedTransportCost;
                    entity.TotalUnitCostIncludingTransport = entity.TotalUnitCost + entity.CalculatedTransportCost;

                    db.GenericWorkItemPerProject.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { workItemTransportCost }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult WorkItemTransportCost_Destroy([DataSourceRequest]DataSourceRequest request, GenericWorkItemTransportCostViewwModel workItemTransportCost)
        {
            if (ModelState.IsValid)
            {
                var entity = db.GenericWorkItemPerProject.Find(workItemTransportCost.ItemId);
                if (entity != null)
                {
                    entity.TransportCostRate = 0;
                    entity.CalculatedTransportCost = 0;
                    entity.TotalUnitCostIncludingTransport = entity.TotalUnitCost;
                    entity.HasAdditionalTransportationCost = false;

                    db.GenericWorkItemPerProject.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { workItemTransportCost }.ToDataSourceResult(request, ModelState));
        }

        public decimal GetProjectTransportCostRate(int projectId) {
            var vl = new decimal();
            var settingValue = db.ProjectSpecificSettingAttributes.FirstOrDefault(s => s.ProjectInformationId == projectId && s.Attribute == ProjectSettingAttributes.TransportCostRate);
            if(settingValue != null)
            {
                vl = settingValue.Value;
            }
            return vl;
        }
    }
}