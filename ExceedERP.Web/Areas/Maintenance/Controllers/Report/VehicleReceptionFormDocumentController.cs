using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class VehicleReceptionFormDocumentController : Controller
    {
       
        public ActionResult Print(string id)
        {
            ViewBag.id = id;
            return View();
        }

     


    }
}
