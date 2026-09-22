using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class WorkshopFieldWorkToolController : Controller
    {
        //
        // GET: /FleetManagement/FollowUp/
        ExceedDbContext db = new ExceedDbContext();
		public ActionResult Index()
		{
			return View();
		}


        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var tools = db.WorkshopFieldWorkTool.Where(x => x.VehicleDemandId == Id).ToList();
            return Json(tools.ToDataSourceResult(request));
        }

		  [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Create([DataSourceRequest] DataSourceRequest request, WorkshopFieldWorkTool tool , string id)
		{
            var existingTool = db.WorkshopFieldWorkTool.FirstOrDefault(x => x.ItemCategoryCode == tool.ItemCategoryCode && x.ItemCode == tool.ItemCode && x.VehicleDemandId == id);
            if (existingTool != null)
            {
                ModelState.AddModelError("exists", "Tool already registered");
            }
			if (tool != null )
			{
				tool.VehicleDemandId = id;
				tool.DateCreated = DateTime.Now;
				tool.CreatedBy = User.Identity.Name;
                tool.ReturnedQuantity = 0;
                tool.LostQuantity = 0;
	            db.WorkshopFieldWorkTool.Add(tool);
                db.SaveChanges();
			}

			return Json(new[] { tool }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Update([DataSourceRequest] DataSourceRequest request, WorkshopFieldWorkTool item)
		{
            var existingTool = db.WorkshopFieldWorkTool.FirstOrDefault(x => x.ItemCategoryCode == item.ItemCategoryCode && x.ItemCode == item.ItemCode && x.VehicleDemandId == item.VehicleDemandId&&x.WorkshopFieldWorkToolId!=item.WorkshopFieldWorkToolId);
            if (existingTool != null)
            {
                ModelState.AddModelError("exists", "Tool already Registered");
            }
            var existingItem = db.WorkshopFieldWorkTool.FirstOrDefault(x => x.WorkshopFieldWorkToolId == item.WorkshopFieldWorkToolId);
			if (ModelState.IsValid)
			{
				existingItem.LastModified = DateTime.Now;
				existingItem.ModifiedBy = User.Identity.Name;
                if (!existingItem.IsSent)
                {
                    existingItem.SentQuantity = item.SentQuantity;
                    existingItem.ItemCategoryCode = item.ItemCategoryCode;
                    existingItem.ItemCode = item.ItemCode;
                }
                if((existingItem.IsSent))
                {
                    if(existingItem.SentQuantity<item.ReturnedQuantity)
                    {
                        ModelState.AddModelError("exceeds", "The quantity you entered is more than sent");
                    }
                    existingItem.LostQuantity = item.SentQuantity - item.ReturnedQuantity;
                    existingItem.ReturnedQuantity = item.ReturnedQuantity;
                }
                db.Entry(existingItem).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
			}

			return Json(new[] {item}.ToDataSourceResult(request,ModelState));
		}



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destory([DataSourceRequest] DataSourceRequest request, WorkshopFieldWorkTool tool)
        {

            var existingItem = db.WorkshopFieldWorkTool.FirstOrDefault(x => x.WorkshopFieldWorkToolId == tool.WorkshopFieldWorkToolId);
            db.WorkshopFieldWorkTool.Remove(existingItem);
                db.SaveChanges();
            
            return Json(new[] { tool }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult SentTools(int id)
        {
            var demand = db.WorkshopFieldWorkTool.FirstOrDefault(x => x.WorkshopFieldWorkToolId == id);
            var tools = db.WorkshopFieldWorkTool.Where(x => x.VehicleDemandId == demand.VehicleDemandId).ToList();
            foreach(var tool in tools)
            {
                tool.IsSent = true;
                db.Entry(tool).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Tools Marked Sent",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReturnTools(int id)
        {
            var demand = db.WorkshopFieldWorkTool.FirstOrDefault(x => x.WorkshopFieldWorkToolId == id);
            var tools = db.WorkshopFieldWorkTool.Where(x => x.VehicleDemandId == demand.VehicleDemandId).ToList();
            foreach(var tool in tools)
            {
                tool.IsReturned = true;
                db.Entry(tool).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Tools Marked Returned",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

