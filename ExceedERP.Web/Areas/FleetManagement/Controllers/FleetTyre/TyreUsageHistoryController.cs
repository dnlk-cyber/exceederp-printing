using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
	public class TyreUsageHistoryController : Controller
	{
		ExceedDbContext db = new ExceedDbContext();
		//
		// GET: /FleetManagement/TyreUsageHistory/
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult TyreUsageHistory_Read([DataSourceRequest] DataSourceRequest request)
		{
			var EquipmentTyreLog = db.EquipmentTyreLog.ToList();

			return Json(EquipmentTyreLog.ToDataSourceResult(request));
		}
	  

	}
}