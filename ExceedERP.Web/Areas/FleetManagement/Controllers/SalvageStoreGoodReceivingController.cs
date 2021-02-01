using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.SalvageGoodReceivingApprover, FleetManagementRoles.StoreGoodReceivingUser)]

    public class SalvageStoreGoodReceivingController : Controller
    {
        ISalvageStoreGoodReceivingService _SalvageStoreGoodReceivingervice;
        IFleetService _FleetService;
        ExceedDbContext db = new ExceedDbContext();
        public SalvageStoreGoodReceivingController()
        {
            _SalvageStoreGoodReceivingervice = new SalvageStoreGoodReceivingService();
            _FleetService = new FleetService();
        }
        //
        // GET: /FleetManagement/SalvageStoreGoodReceiving/
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult SalvageStoreGoodReceiving_Read([DataSourceRequest] DataSourceRequest request)
        {
            var SalvageStoreGoodReceiving = _SalvageStoreGoodReceivingervice.GetAll();
            var SalvageStoreGoodReceivings = SalvageStoreGoodReceiving.Select(a => new SalvageStoreGoodReceiving
               {
                   Num = a.Num,
                   FleetsID = a.FleetsID,
                   Date = a.Date,
                   PlateNumber = a.Fleet.PlateNumber,
                   Description = a.Description,
                       VoucherNo = a.VoucherNo,
                   SparePartName = a.SparePartName,
                   Quantity = a.Quantity,
                   UnitPrice = a.UnitPrice,
                   TotalPrice = a.Quantity * a.UnitPrice,
                   SalvageStoreGoodReceivingID = a.SalvageStoreGoodReceivingID,
                   IsSendForApproval = a.IsSendForApproval,
                   IsOnlineApproved = a.IsOnlineApproved,
                   DateCreated = a.DateCreated,
                   CreatedBy = a.CreatedBy,
                   LastModified = a.LastModified,
                   ModifiedBy = a.ModifiedBy,
                            Status = a.Status

               }).ToList();

            return Json(SalvageStoreGoodReceivings.ToDataSourceResult(request));
        }
        public JsonResult SalvageStoreGoodReceiving_Create([DataSourceRequest] DataSourceRequest request, SalvageStoreGoodReceiving SalvageStoreGoodReceiving)
        {

            ModelState.Remove("SalvageStoreGoodReceivingID");

            var Receive = _SalvageStoreGoodReceivingervice.GetAll().Where(x => x.VoucherNo == SalvageStoreGoodReceiving.VoucherNo).ToList();
            if (Receive.Any())
            {
                ModelState.AddModelError("VoucherNumExist", " Voucher Number Already Registered !");
                return Json(new[] { SalvageStoreGoodReceiving }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                if (SalvageStoreGoodReceiving != null)
                {
                    SalvageStoreGoodReceiving.SalvageStoreGoodReceivingID = Guid.NewGuid();
                    SalvageStoreGoodReceiving.Date = DateTime.Now;
                    SalvageStoreGoodReceiving.Status = MaintenanceConst.Pending;
                    SalvageStoreGoodReceiving.CreatedBy = User.Identity.Name;
                    SalvageStoreGoodReceiving.DateCreated = DateTime.Now;

                    _SalvageStoreGoodReceivingervice.Insert(SalvageStoreGoodReceiving);
                }
            }

            return Json(new[] { SalvageStoreGoodReceiving }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SalvageStoreGoodReceiving_Update([DataSourceRequest] DataSourceRequest request, SalvageStoreGoodReceiving SalvageStoreGoodReceiving)
        {
            var Receive = _SalvageStoreGoodReceivingervice.GetAll().Where(x => x.VoucherNo == SalvageStoreGoodReceiving.VoucherNo && x.SalvageStoreGoodReceivingID != SalvageStoreGoodReceiving.SalvageStoreGoodReceivingID).ToList();
            if (Receive.Any())
            {
                ModelState.AddModelError("VoucherNumExist", " Voucher Number Already Registered !");
                return Json(new[] { SalvageStoreGoodReceiving }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                if (SalvageStoreGoodReceiving != null)
                {
                    _SalvageStoreGoodReceivingervice = new SalvageStoreGoodReceivingService();

                    SalvageStoreGoodReceiving.ModifiedBy = User.Identity.Name;
                    SalvageStoreGoodReceiving.LastModified = DateTime.Now;
                    _SalvageStoreGoodReceivingervice.Update(SalvageStoreGoodReceiving);
                }
            }
            return Json(new[] { SalvageStoreGoodReceiving }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SalvageStoreGoodReceiving_Destroy([DataSourceRequest] DataSourceRequest request, SalvageStoreGoodReceiving salvageStoreGoodReceiving)
        {
            var SalvageStoreGoodReceiving = _SalvageStoreGoodReceivingervice.GetById(salvageStoreGoodReceiving.SalvageStoreGoodReceivingID);

            if (SalvageStoreGoodReceiving != null)
            {
                _SalvageStoreGoodReceivingervice.Delete(SalvageStoreGoodReceiving);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete Salvage Store Good Receiving number " + salvageStoreGoodReceiving.SalvageStoreGoodReceivingID);

            }

            return Json(new[] { SalvageStoreGoodReceiving }.ToDataSourceResult(request));
        }

        [AllowAnonymous]
        public ActionResult StoreItemDDBind()
        {
         
            var sparePart = db.SalvageStoreGoodReceiving.Where(x=> x.Quantity > 0).Select(s => new
            {
                Value = s.Num,
                Name = s.SparePartName + " - " + s.VoucherNo
            });

            return Json(sparePart, JsonRequestBehavior.AllowGet);
        }
	


    }
}