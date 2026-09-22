using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.ApplicationServices.FleetManagement.EquipmentInspection;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
	public class DisposalandReplacementRecommendationController : Controller
	{
		IDisposalandReplacementRecommendationService _DisposalandReplacementRecommendationService;

		public DisposalandReplacementRecommendationController()
		{
			_DisposalandReplacementRecommendationService = new DisposalandReplacementRecommendationService();
		}
		//
		// GET: /FleetManagement/DisposalandReplacementRecommendation/
		public ActionResult Index()
		{
			return View();
		}


		public ActionResult DisposalandReplacementRecommendation_Read([DataSourceRequest] DataSourceRequest request)
		{
			var DisposalandReplacementRecommendation = _DisposalandReplacementRecommendationService.GetAll();

			return Json(DisposalandReplacementRecommendation.ToDataSourceResult(request));
		}


		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult DisposalandReplacementRecommendation_Create([DataSourceRequest] DataSourceRequest request, DisposalandReplacementRecommendation DisposalandReplacementRecommendation)
		{
			ModelState.Remove("DisposalandReplacementRecommendationID");
			 if (DisposalandReplacementRecommendation != null)
			{
				DisposalandReplacementRecommendation.DisposalandReplacementRecommendationID = Guid.NewGuid();
				DisposalandReplacementRecommendation.DateCreated = DateTime.Now;
				DisposalandReplacementRecommendation.CreatedBy = User.Identity.Name;
		 
				_DisposalandReplacementRecommendationService.Insert(DisposalandReplacementRecommendation);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Disposal and replacement recommendation", "Success", User.Identity.Name, $"created disposal and replacement recommendation reg {DisposalandReplacementRecommendation.DisposalandReplacementRecommendationID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { DisposalandReplacementRecommendation }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult DisposalandReplacementRecommendation_Update([DataSourceRequest] DataSourceRequest request, DisposalandReplacementRecommendation DisposalandReplacementRecommendation)
		{
			if (DisposalandReplacementRecommendation != null)
			{
                DisposalandReplacementRecommendation.LastModified = DateTime.Now;
                DisposalandReplacementRecommendation.ModifiedBy = User.Identity.Name;
		 
				_DisposalandReplacementRecommendationService.Update(DisposalandReplacementRecommendation);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Disposal and Replacement Recommendation", "Success", User.Identity.Name, $"updated disposal and replacement recommendation reg {DisposalandReplacementRecommendation.DisposalandReplacementRecommendationID} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { DisposalandReplacementRecommendation }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult DisposalandReplacementRecommendation_Destroy([DataSourceRequest] DataSourceRequest request, DisposalandReplacementRecommendation disposalandReplacementRecommendation)
		{
			var DisposalandReplacementRecommendation = _DisposalandReplacementRecommendationService.GetById(disposalandReplacementRecommendation.DisposalandReplacementRecommendationID);
			if (DisposalandReplacementRecommendation != null)
			{
				_DisposalandReplacementRecommendationService.Delete(DisposalandReplacementRecommendation);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Disposal Recommendation number " + disposalandReplacementRecommendation.DisposalandReplacementRecommendationID);
		
			}

			return Json(new[] { DisposalandReplacementRecommendation }.ToDataSourceResult(request));
		}


       
    }
}

