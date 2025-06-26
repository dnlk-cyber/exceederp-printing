
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetTireManagement;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class TireReturnController : Controller
    {
      ITireReturnService _TireReturnService;
        ExceedDbContext db = new ExceedDbContext();
        public TireReturnController()
        {
            _TireReturnService = new TireReturnService();
        }
        //
        // GET: /FleetManagement/TireReturn/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult TireReturn_Read([DataSourceRequest] DataSourceRequest request)
        {
            var TireReturn = _TireReturnService.GetAll().Select(t => new TireReturn
            {

                UnitPrice = t.UnitPrice,
                OperatorName = t.OperatorName,
                Brand = t.Brand,
                PlateNumber = t.PlateNumber,
                DescriptionReason = t.DescriptionReason,
                InstalledDate = t.InstalledDate,
                KMReading = t.KMReading,
                //Quantity = t.Quantity,
                SerialNo = t.SerialNo,
                TireReturnID = t.TireReturnID,
                TireSize = t.TireSize,
                TireTube = t.TireTube,
                TireType = t.TireType,


            }
                );


            return Json(TireReturn.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireReturn_Create([DataSourceRequest] DataSourceRequest request, TireReturn TireReturn)
        {
            IOperatorService _OperatorService = new OperatorService();
            IFleetService fleetService = new FleetService();
            Fleets fleet = fleetService.GetAll().Where(x => x.PlateNumber == TireReturn.PlateNumber).FirstOrDefault();
            var operatorName = _OperatorService.GetAll().Where(x => x.FleetsID != fleet.FleetsID).FirstOrDefault();
            TireReturn.OperatorName = operatorName.FullName;
            TireReturn.TireReturnID = Guid.NewGuid();
          
            if (TireReturn != null)
            {
                TireReturn.CreatedBy = User.Identity.Name;
                TireReturn.DateCreated = DateTime.Now;
                _TireReturnService.Insert(TireReturn);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Tyre return registration", "Success", User.Identity.Name, $"created tyre return {TireReturn.TireReturnID}", Modules.FleetManagement);
                #endregion


            }

            return Json(new[] { TireReturn }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireReturn_Update([DataSourceRequest] DataSourceRequest request, TireReturn TireReturn)
        {
            if (TireReturn != null )
            {
                TireReturn.ModifiedBy = User.Identity.Name;
                TireReturn.LastModified = DateTime.Now;
                _TireReturnService.Update(TireReturn);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Tyre return registration", "Success", User.Identity.Name, $"updated tyre return reg {TireReturn.TireReturnID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { TireReturn }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireReturn_Destroy([DataSourceRequest] DataSourceRequest request, TireReturn tireReturn)
        {
            //var TireReturn = _TireReturnService.GetById(tireReturn.TireReturnID);
            var TireReturn = db.TireReturn.Find(tireReturn.TireReturnID);
            if (TireReturn != null)
            {
                _TireReturnService.Delete(TireReturn);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted tyre return registration number {TireReturn.TireReturnID }");
                #endregion
            }

            return Json(new[] { TireReturn }.ToDataSourceResult(request));
        }
        
    }
}
