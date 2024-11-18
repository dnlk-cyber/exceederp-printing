using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    public class FurnitureBoqRequestController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Manufacturing/FurnitureBoqRequest
        public ActionResult Index()
        {
            var myCustomers = db.OrganizationCustomers.Select(
               s => new {
                   Code = s.OrganizationCustomerID,
                   Name = s.TradeName
               }).ToList();
            ViewData["Customers"] = myCustomers;
            return View();
        }
        public ActionResult FurnitureBoqRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureBoqRequest> furnitureBoqRequest = db.FurnitureBoqRequests;
            DataSourceResult result = furnitureBoqRequest.ToDataSourceResult(request, model => new FurnitureBoqRequest
            {
                FurnitureBoqRequestId = model.FurnitureBoqRequestId,
                CustomerId = model.CustomerId,
                JobType = model.JobType,
                Remark = model.Remark,
                RequestDate = model.RequestDate
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBoqRequest_Create([DataSourceRequest]DataSourceRequest request, FurnitureBoqRequest model)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBoqRequest
                {
                    JobType = model.JobType,
                    CustomerId = model.CustomerId,
                    Remark = model.Remark,
                    RequestDate = model.RequestDate
                };

                db.FurnitureBoqRequests.Add(entity);
                db.SaveChanges();
                model.FurnitureBoqRequestId = entity.FurnitureBoqRequestId;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBoqRequest_Update([DataSourceRequest]DataSourceRequest request, FurnitureBoqRequest model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FurnitureBoqRequest_Delete([DataSourceRequest]DataSourceRequest request, FurnitureBoqRequest model)
        {
            if (ModelState.IsValid)
            {
                var boqRequest = db.FurnitureBoqRequests.FirstOrDefault(x => x.FurnitureBoqRequestId == model.FurnitureBoqRequestId);
                db.FurnitureBoqRequests.Remove(boqRequest);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}