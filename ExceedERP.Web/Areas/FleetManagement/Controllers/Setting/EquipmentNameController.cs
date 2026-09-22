using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
		[AuthorizeRoles(FleetManagementRoles.ResourceApprover)]  
	public class EquipmentNameController : Controller
	{
	   IEquipmentNameService _EquipmentNameService;
		ExceedDbContext db = new ExceedDbContext();

		public EquipmentNameController()
		{
			_EquipmentNameService = new EquipmentNameService();
		}
		//
		// GET: /FleetManagement/EquipmentName/
		public ActionResult Index()
		{
			var equipCatNames = db.FixedAssetSubCategoryNames.Select(s => new {
				Id = s.FixedAssetSubCategoryNameId,
				Name = s.Name
			}).ToList();

			ViewData["FixedAssetSubCategoryNames"] = equipCatNames;

			var equipmentTypes = db.EquipmentType.Select(s => new {
				Id = s.EquipmentTypeID,
				Name = s.EquipmentTypeName
			}).ToList();

			ViewData["EquipmentTypes"] = equipmentTypes;
			return View();
		}


		public JsonResult EquipmentName_Read([DataSourceRequest] DataSourceRequest request,Guid? id)
		{
			_EquipmentNameService = new EquipmentNameService();
			var opers = _EquipmentNameService.GetAll();
			if(id!=null)
			{
				opers = opers.Where(q => q.EquipmentTypeId == id).ToList();
			}
			var Opers = opers.Select(o => new EquipmentName
			{
				FixedAssetSubCategoryNameId = o.FixedAssetSubCategoryNameId,
				EquipmentTypeId = o.EquipmentTypeId,
				EquipmentNameID = o.EquipmentNameID,
				Name = o.Name,
				Description = o.Description

			});

			return this.Json(Opers.ToDataSourceResult(request,ModelState));
		}


		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult EquipmentName_Create([DataSourceRequest] DataSourceRequest request, EquipmentName EquipmentName,Guid? id)
		{
			ModelState.Remove("EquipmentNameID");
			ModelState.Remove("EquipmentTypeID");
			List<EquipmentName> EquipmentNames = _EquipmentNameService.GetAll().Where(f => f.Name == EquipmentName.Name).ToList();
			if (EquipmentNames.Any())
			{
				ModelState.Clear();
				ModelState.AddModelError("ExistingName", " Equipment Name  Already Exists ");
			}
			else
			{
				

				if (EquipmentName != null)
				{
					EquipmentName.EquipmentNameID = Guid.NewGuid();
					if(id!=null)
					{
						EquipmentName.EquipmentTypeId = (Guid)id;

					}
					EquipmentName.CreatedBy = User.Identity.Name;
					EquipmentName.DateCreated = DateTime.Now;
					EquipmentName.IsAssigned = false;
					_EquipmentNameService.Insert(EquipmentName);
				}
			}
			return this.Json(new[] { EquipmentName }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult EquipmentName_Update([DataSourceRequest] DataSourceRequest request, EquipmentName EquipmentName)
		{
		   
				if (EquipmentName != null)
				{
					EquipmentName.ModifiedBy = User.Identity.Name;
					EquipmentName.LastModified = DateTime.Now;
				
					_EquipmentNameService.Update(EquipmentName);
				}
			

			return this.Json(new[] { EquipmentName }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult EquipmentName_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentName EquipmentName)
		{
			var EquipmentNames = _EquipmentNameService.GetById(EquipmentName.EquipmentNameID);
			if (EquipmentNames != null)
			{
				_EquipmentNameService.Delete(EquipmentNames);
				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Name number " + EquipmentName.EquipmentNameID);

			}

			return this.Json(new[] { EquipmentName }.ToDataSourceResult(request));
		}
        public JsonResult GetEquipmentTypesForNames(string text)
        {
            var result = db.EquipmentType.ToList();
            if (!string.IsNullOrWhiteSpace(text))
            {
                result = result.Where(x => x.EquipmentTypeName.ToLower().Contains(text.ToLower())).ToList();
            }
            var types = result.Select(s => new
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();
           

            return Json(types, JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}