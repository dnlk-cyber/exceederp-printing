using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingJobOrderItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Printing/PrintingJobOrderItem
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingJobOrderItem_Read([DataSourceRequest]DataSourceRequest request)
        {
            var jobOrders = db.PrintingJobOrderItems;
            return Json(jobOrders.ToDataSourceResult(request));
        }

    }
}