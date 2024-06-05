
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetTireManagement;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using System.Data.Entity.Validation;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.TyreUser, FleetManagementRoles.TyreApprover)]
    public class TyreManagementReturnController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        ITireReturnService _TireReturnService;

        public TyreManagementReturnController()
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
            var TireReturn = _TireReturnService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(t => new TireReturn
            {

                UnitPrice = t.UnitPrice,
                OperatorName = t.OperatorName,
                Brand = t.Brand,
                PlateNumber = t.PlateNumber,
                DescriptionReason = t.DescriptionReason,
                InstalledDate = t.InstalledDate,
                KMReading = t.KMReading,
                StoreCode = t.StoreCode,
                SerialNo = t.SerialNo,
                TireReturnID = t.TireReturnID,
                TireSize = t.TireSize,
                TireTube = t.TireTube,
                TireType = t.TireType,
                IsOnlineApproved = t.IsOnlineApproved,
                IsSendForApproval = t.IsSendForApproval,
                Status = t.Status,
                ReturnType = t.ReturnType

            }
                );


            return Json(TireReturn.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireReturn_Create([DataSourceRequest] DataSourceRequest request, TireReturn TireReturn)
        {
            IOperatorService _OperatorService = new OperatorService();
            IFleetService fleetService = new FleetService();


            //try 
            //{ 

            Fleets fleet = fleetService.GetAll().Where(x => x.FleetsID == TireReturn.FleetId).FirstOrDefault();
            EquipmentTyre Tyre = db.EquipmentTyres.Where(x => x.SerialNo == TireReturn.SerialNo.ToString()).FirstOrDefault();






            if (TireReturn != null && fleet != null && Tyre != null)
            {
                TireReturn.TireReturnID = Guid.NewGuid();
                TireReturn.Brand = Tyre.Make;
                TireReturn.FleetId = fleet.FleetsID;
                TireReturn.InstalledDate = Tyre.DateCreated;
                TireReturn.KMReading = fleet.CurentOdoMeter - Tyre.OdoMeter;
                TireReturn.SerialNo = Tyre.SerialNo;
                TireReturn.StoreCode = Tyre.StoreCode;
                TireReturn.TireSize = Tyre.Size;
                TireReturn.TireTube = Tyre.Tube;
                TireReturn.TireType = Tyre.Type;
                TireReturn.UnitPrice = Tyre.UnitPrice;
                TireReturn.PlateNumber = fleet.PlateNumber + " - " + fleet.UnitNum;
                TireReturn.CreatedBy = User.Identity.Name;
                TireReturn.DateCreated = DateTime.Now;
                TireReturn.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                _TireReturnService.Insert(TireReturn);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Tyre management return registration", "Success", User.Identity.Name, $"created tyre management return {TireReturn.TireReturnID}", Modules.FixedAsset);
                #endregion

            }

            //}
            //catch (Exception ee)
            //{

            //}
            return Json(new[] { TireReturn }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireReturn_Update([DataSourceRequest] DataSourceRequest request, TireReturn TireReturn)
        {
            IFleetService fleetService = new FleetService();
            Fleets fleet = fleetService.GetAll().Where(x => x.PlateNumber == TireReturn.PlateNumber).FirstOrDefault();

            if (TireReturn != null)
            {
                TireReturn.KMReading = fleet.CurentOdoMeter;
                TireReturn.ModifiedBy = User.Identity.Name;
                TireReturn.LastModified = DateTime.Now;
                TireReturn.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                _TireReturnService.Update(TireReturn);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Tyre management return registration", "Success", User.Identity.Name, $"updated tyre management return {TireReturn.TireReturnID}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { TireReturn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireReturn_Destroy([DataSourceRequest] DataSourceRequest request, TireReturn tireReturn)
        {
            var TireReturn = _TireReturnService.GetById(tireReturn.TireReturnID);
            if (TireReturn != null)
            {
                _TireReturnService.Delete(TireReturn);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete Tire Return number " + tireReturn.TireReturnID);

            }

            return Json(new[] { TireReturn }.ToDataSourceResult(request));
        }



    }
}
