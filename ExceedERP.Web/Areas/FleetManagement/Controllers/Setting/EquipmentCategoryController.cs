using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
		[AuthorizeRoles(FleetManagementRoles.ResourceApprover)]  
	public class EquipmentCategoryController : Controller
	{
		ExceedDbContext db = new ExceedDbContext();
		IEquipmentIdentityService _EquipmentIdentityService;

		public EquipmentCategoryController()
		{
			_EquipmentIdentityService = new EquipmentIdentityService();
		}
		//
		// GET: /EquipmentIdentityManagement/EquipmentIdentity/
		public ActionResult Index()
		{
			var equipCats = db.FixedAssetCategories.Select(s => new {
				Id = s.FixedAssetCategoryID,
				Name = s.CategoryCode + "_" + s.CategoryName
			}).ToList();
			ViewData["FixedAssetCategories"] = equipCats;

			return View();
		}

		public ActionResult EquipmentIdentity_Read([DataSourceRequest] DataSourceRequest request)
		{
			var equipmentIdentity = _EquipmentIdentityService.GetAll().Select(e=> new EquipmentIdentity

				{
				  FixedAssetCategoryId = e.FixedAssetCategoryId,
					EquipmentIdentityID = e.EquipmentIdentityID,
					EquipmentCategoryName = e.EquipmentCategoryName,
					Description = e.Description   


				}
		);

			return Json(equipmentIdentity.ToDataSourceResult(request));
		}
		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult EquipmentIdentity_Create([DataSourceRequest] DataSourceRequest request, EquipmentIdentity EquipmentIdentitys)
			{
			ModelState.Remove("EquipmentIdentityID");
			List<EquipmentIdentity> fleetsplate = _EquipmentIdentityService.GetAll().Where(f => f.EquipmentCategoryName == EquipmentIdentitys.EquipmentCategoryName).ToList();
			 if (fleetsplate.Any())
			 {                                                                                                      
				 ModelState.Clear();
				 ModelState.AddModelError("ExistingName", " Equipment Category Name Already Exists ");
				 return Json(new[] { EquipmentIdentitys }.ToDataSourceResult(request, ModelState));
			 }
			 else
			 {
			
				 if (EquipmentIdentitys != null)
				 {
					 EquipmentIdentitys.EquipmentIdentityID = Guid.NewGuid();
					EquipmentIdentitys.CreatedBy = User.Identity.Name;
					 EquipmentIdentitys.DateCreated = DateTime.Now;
					
					 _EquipmentIdentityService.Insert(EquipmentIdentitys);
				 }

			 }
			return Json(new[] { EquipmentIdentitys }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult EquipmentIdentity_Update([DataSourceRequest] DataSourceRequest request, EquipmentIdentity EquipmentIdentitys)
		{
			List<EquipmentIdentity> fleetsplate = _EquipmentIdentityService.GetAll().Where(f => f.EquipmentCategoryName == EquipmentIdentitys.EquipmentCategoryName && f.EquipmentIdentityID!=EquipmentIdentitys.EquipmentIdentityID).ToList();
			if (fleetsplate.Any())
			{
				ModelState.Clear();
				ModelState.AddModelError("ExistingName", " Equipment Category Name Already Exists ");
				return Json(new[] { EquipmentIdentitys }.ToDataSourceResult(request, ModelState));
			}
			else
			{
				if (EquipmentIdentitys != null)
				{

					EquipmentIdentitys.ModifiedBy = User.Identity.Name;
					EquipmentIdentitys.LastModified = DateTime.Now;
					_EquipmentIdentityService.AddOrUpdate(EquipmentIdentitys);
				}
			}
			return Json(new[] { EquipmentIdentitys }.ToDataSourceResult(request));
		}

		public JsonResult EquipmentIdentity_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentIdentity EquipmentIdentitys)
		{
		   var equipmentIdentity = _EquipmentIdentityService.GetById(EquipmentIdentitys.EquipmentIdentityID);
			if (equipmentIdentity != null)
			{
				_EquipmentIdentityService.Delete(equipmentIdentity);
				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Identity number " + equipmentIdentity.EquipmentIdentityID);
		
			}

			return Json(new[] { EquipmentIdentitys }.ToDataSourceResult(request));
		}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

