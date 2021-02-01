
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Helpers.Fleet;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
       [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]  
	public class SparePartController : Controller
	{
           ExceedDbContext db = new ExceedDbContext();
		ISparePartService _SparePartService;
		IFleetService _FleetService;
		public SparePartController()
		{
			_SparePartService = new SparePartService();
			_FleetService = new FleetService();
		}
	 
		//
		// GET: /FleetManagement/SparePart/
		public ActionResult Index()
		{
			return View();
		}

	   
		public ActionResult SparePart_Read([DataSourceRequest] DataSourceRequest request)
		{
			var SparePart = _SparePartService.GetAll();

			return Json(SparePart.ToDataSourceResult(request));
		}
	  

		  [AcceptVerbs(HttpVerbs.Post)]
		public ActionResult SparePart_Create([DataSourceRequest] DataSourceRequest request, SparePart SparePart)
        {
            ModelState.Remove("SparePartID");
				  List<SparePart> SpareParts = _SparePartService.GetAll().Where(f => f.SparePartName == SparePart.SparePartName).ToList();
				  if (SpareParts.Any())
				  {
					  ModelState.Clear();
					  ModelState.AddModelError("ExistingName", "  SparePart with Name Already Exists ");
				  }
				  else
				  {
					 
					  if (SparePart != null)
					  {
					      decimal Price = 0;
                          var Tools = db.InventoryItems.Where(x => x.Name == SparePart.PartNo).FirstOrDefault();
                          if (Tools != null)
                          {
                              Price = Tools.AvgPrice;
                          }
                          
                          SparePart.SparePartID = Guid.NewGuid();
					      SparePart.UnitPrice = Price;
                          SparePart.CreatedBy = User.Identity.Name;

                          SparePart.DateCreated = DateTime.Now;
						  _SparePartService.Insert(SparePart);
					  }
				  }
				  return Json(new[] { SparePart }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult SparePart_Update([DataSourceRequest] DataSourceRequest request, SparePart SparePart)
		{
							
								if (SparePart != null)
								{
                                    SparePart.ModifiedBy = User.Identity.Name;
                                    SparePart.LastModified = DateTime.Now;
									_SparePartService.Update(SparePart);
								}
							

			return Json(new[] {SparePart}.ToDataSourceResult(request,ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult SparePart_Destroy([DataSourceRequest] DataSourceRequest request, SparePart sparePart)
		{
			var SparePart = _SparePartService.GetById(sparePart.SparePartID);
			if (SparePart != null)
			{
				_SparePartService.Delete(SparePart);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete Spare Part number " + sparePart.SparePartID);
           
			}

			return Json(new[] { SparePart }.ToDataSourceResult(request));
		}
     
           
           [AllowAnonymous]
		public ActionResult SparePartDDBind()
		{
            var spareParts = new List<FleetBindingHelper.HelperObject>();
            var mapping = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Spare).ToList();
            foreach (var map in mapping)
            {
                var sparePart = db.InventoryItems.AsNoTracking().Where(x => x.ItemCategoryCode == map.InventoryCategoryId).Select(s => new FleetBindingHelper.HelperObject
                {
                    Value = s.ItemCode,
                    Name = s.Name
                }).Take(10).ToList();
                spareParts.AddRange(sparePart);
            }
			return Json(spareParts, JsonRequestBehavior.AllowGet);
		}
         [AllowAnonymous]
         public ActionResult BatteryDDBind()
         {
             var spareParts = new List<FleetBindingHelper.HelperObject>();
             var mapping = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Battery).ToList();
             foreach (var map in mapping)
             {
                 var sparePart = db.InventoryItems.Where(x => x.ItemCategoryCode == map.InventoryCategoryId).Select(s => new FleetBindingHelper.HelperObject
                 {
                     Value = s.ItemCode,
                     Name = s.Name
                 }).ToList();
                 spareParts.AddRange(sparePart);
             }
             return Json(spareParts, JsonRequestBehavior.AllowGet);
         }
         [AllowAnonymous]
         public ActionResult Oil_LubricantDDBind()
         {
             var spareParts = new List<FleetBindingHelper.HelperObject>();
             var mapping = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Oil_Lubricant).ToList();
             foreach (var map in mapping)
             {
                 var sparePart = db.InventoryItems.Where(x => x.ItemCategoryCode == map.InventoryCategoryId).Select(s => new FleetBindingHelper.HelperObject
                 {
                     Value = s.ItemCode,
                     Name = s.Name
                 }).ToList();
                 spareParts.AddRange(sparePart);
             }
             return Json(spareParts, JsonRequestBehavior.AllowGet);
         }
         [AllowAnonymous]
         public ActionResult TyreDDBind()
         {
             var spareParts = new List<FleetBindingHelper.HelperObject>();
             var mapping = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Tyre).ToList();
             foreach (var map in mapping)
             {
                 var sparePart = db.InventoryItems.Where(x => x.ItemCategoryCode == map.InventoryCategoryId).Select(s => new FleetBindingHelper.HelperObject
                 {
                     Value = s.ItemCode,
                     Name = s.Name
                 }).ToList();
                 spareParts.AddRange(sparePart);
             }
             return Json(spareParts, JsonRequestBehavior.AllowGet);
         }
         [AllowAnonymous]
         public ActionResult OtherDDBind()
         {
             var spareParts = new List<FleetBindingHelper.HelperObject>();
             var mapping = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Other).ToList();
             foreach (var map in mapping)
             {
                 var sparePart = db.InventoryItems.Where(x => x.ItemCategoryCode == map.InventoryCategoryId).Select(s => new FleetBindingHelper.HelperObject
                 {
                     Value = s.ItemCode,
                     Name = s.Name
                 }).ToList();
                 spareParts.AddRange(sparePart);
             }
             return Json(spareParts, JsonRequestBehavior.AllowGet);
         }

	}
}

