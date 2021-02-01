using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class VehicleReturnController : Controller
    {
        IVehicleReturnService _VehicleReturnService;

        public VehicleReturnController()
        {
            _VehicleReturnService = new VehicleReturnService();
        }
        //
        // GET: /FleetManagement/VehicleReturn/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult VehicleReturn_Read([DataSourceRequest] DataSourceRequest request)
        {
            var VehicleReturn = _VehicleReturnService.GetAll().Select(v=> new VehicleReturn
                {
                    FleetsID = v.FleetsID,
                    Description = v.Description,
                    ReasonforReturn = v.ReasonforReturn,
                    ReturnDate =v.ReturnDate,
                    ReturnedBy = v.ReturnedBy,
                    TotalTimeElapsed = v.TotalTimeElapsed,
                    VehicleReturnID = v.VehicleReturnID

                }
                );

            return Json(VehicleReturn.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReturn_Create([DataSourceRequest] DataSourceRequest request, VehicleReturn VehicleReturn)
        {
            VehicleReturn.VehicleReturnID = Guid.NewGuid();
            VehicleReturn.ReturnDate = DateTime.Now;

            if (VehicleReturn != null)
            {
                VehicleReturn.CreatedBy = User.Identity.Name;
                VehicleReturn.DateCreated = DateTime.Now;
                _VehicleReturnService.Insert(VehicleReturn);
            }

            return Json(new[] { VehicleReturn }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReturn_Update([DataSourceRequest] DataSourceRequest request, VehicleReturn VehicleReturn)
        {
            if (VehicleReturn != null)
            {
                VehicleReturn.ModifiedBy = User.Identity.Name;
                VehicleReturn.LastModified = DateTime.Now;
                _VehicleReturnService.Update(VehicleReturn);
            }

            return Json(new[] { VehicleReturn }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleReturn_Destroy([DataSourceRequest] DataSourceRequest request, VehicleReturn vehicleReturn)
        {
            var VehicleReturn = _VehicleReturnService.GetById(vehicleReturn.VehicleReturnID);
            if (VehicleReturn != null)
            {
                _VehicleReturnService.Delete(VehicleReturn);
            }

            return Json(new[] { VehicleReturn }.ToDataSourceResult(request));
        }



    }
}

