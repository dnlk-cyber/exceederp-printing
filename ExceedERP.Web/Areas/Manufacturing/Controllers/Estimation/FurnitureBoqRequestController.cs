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
            return Json(furnitureBoqRequest.ToDataSourceResult(request));
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
                    RequestDate = model.RequestDate,
                    Status = "Pending"
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

        public JsonResult CheckBoqRequest(int id)
        {
            var boqRequest = db.FurnitureBoqRequests.Find(id);
            var docs = db.FurnitureBoqDocuments.Where(x => x.FurnitureBoqRequestId == boqRequest.FurnitureBoqRequestId).ToList();
            object response = null;
            if (!docs.Any())
            {
                response = new { Success = false, Message = "no document was found!" };
                return Json(response);
            }
            if (boqRequest != null && docs.Any())
            {
                boqRequest.IsChecked = true;
                boqRequest.CheckedBy = User.Identity.Name;
                boqRequest.CheckedDate = DateTime.Today;
                boqRequest.Status = "Checked";

                db.Entry(boqRequest).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Document has been checked Successfully!" };
            }
            else
            {
                response = new { Success = false, Message = "not found!" };
            }
            return Json(response);
        }

        public JsonResult ApproveBoqRequest(int id)
        {
            var boqRequest = db.FurnitureBoqRequests.Find(id);
            object response = null;
            if (boqRequest != null)
            {
                boqRequest.IsApproved = true;
                boqRequest.ApprovedBy = User.Identity.Name;
                boqRequest.ApprovedDate = DateTime.Today;
                boqRequest.Status = "Approved";

                db.Entry(boqRequest).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Document has been approved Successfully!" };
            }
            else
            {
                response = new { Success = false, Message = "not found!" };
            }
            return Json(response);
        }
        public JsonResult RejectBoqRequest(int id)
        {
            var boqRequest = db.FurnitureBoqRequests.Find(id);
            object response = null;
            if (boqRequest != null)
            {
                boqRequest.IsRejected = true;
                boqRequest.RejectedBy = User.Identity.Name;
                boqRequest.RejectedDate = DateTime.Today;
                boqRequest.Status = "Rejected";

                db.Entry(boqRequest).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Document has been rejected Successfully!" };
            }
            else
            {
                response = new { Success = false, Message = "not found!" };
            }
            return Json(response);
        }
    }
}