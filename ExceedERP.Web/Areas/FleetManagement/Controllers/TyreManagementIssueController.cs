using ExceedERP.ApplicationServices.FleetManagement.FleetTireManagement;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using System.Data.Entity.Validation;



namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.TyreUser, FleetManagementRoles.TyreApprover)]
    public class TyreManagementIssueController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        ITireIssueService _TireIssueService;

        public TyreManagementIssueController()
        {
            _TireIssueService = new TireIssueService();
        }
        //
        // GET: /FleetManagement/TireIssue/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult TireIssue_Read([DataSourceRequest] DataSourceRequest request)
        {
            var TireIssue = _TireIssueService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(t => new TireIssue
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
                    TireIssueID = t.TireIssueID,
                    TireSize = t.TireSize,
                    TireTube = t.TireTube,
                    TireType = t.TireType,
                    IsOnlineApproved = t.IsOnlineApproved,
                    IsSendForApproval = t.IsSendForApproval,
                    Status = t.Status,
                    DateCreated = t.DateCreated,
                    CreatedBy = t.CreatedBy,
                    ModifiedBy = t.ModifiedBy,
                    LastModified = t.LastModified

                }
                );

            return Json(TireIssue.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssue_Create([DataSourceRequest] DataSourceRequest request, TireIssue TireIssue)
        {
            IOperatorService _OperatorService = new OperatorService();
            IFleetService fleetService = new FleetService();
            ITireReturnService _TireReturnService = new TireReturnService();
            try
            {
                TireReturn Return = _TireReturnService.GetAll().Where(x => x.TireReturnID.ToString() == TireIssue.SerialNo).FirstOrDefault();
                EquipmentTyre Tyre = db.EquipmentTyres.Where(x => x.fleetId == Return.FleetId && x.SerialNo == Return.SerialNo).FirstOrDefault();

                Fleets fleet = fleetService.GetAll().Where(x => x.PlateNumber == TireIssue.PlateNumber).FirstOrDefault();

                if (TireIssue != null && Return != null && fleet != null)
                {




                    TireIssue.TireIssueID = Guid.NewGuid();
                    TireIssue.TireReturnId = Return.TireReturnID;
                    TireIssue.FleetId = fleet.FleetsID;
                    TireIssue.Brand = Return.Brand;
                    TireIssue.InstalledDate = Return.InstalledDate;
                    TireIssue.StoreCode = Return.StoreCode;
                    TireIssue.KMReading = Return.KMReading;
                    TireIssue.SerialNo = Return.SerialNo;
                    TireIssue.TireSize = Return.TireSize;
                    TireIssue.TireTube = Return.TireTube;
                    TireIssue.TireType = Return.TireType;
                    TireIssue.UnitPrice = Return.UnitPrice;
                    TireIssue.PlateNumber = fleet.PlateNumber;
                    TireIssue.CreatedBy = User.Identity.Name;
                    TireIssue.DateCreated = DateTime.Now;
                    TireIssue.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    _TireIssueService.Insert(TireIssue);

                }
            }
            catch (DbEntityValidationException dbEx)
            {
                string e = string.Empty;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        e += validationError.PropertyName + "  -- " + validationError.ErrorMessage;
                    }
                }
                return Json(new[] { e },JsonRequestBehavior.AllowGet);
            }
            return Json(new[] { TireIssue }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssue_Update([DataSourceRequest] DataSourceRequest request, TireIssue TireIssue)
        {
            IFleetService fleetService = new FleetService();
            Fleets fleet = fleetService.GetAll().Where(x => x.PlateNumber == TireIssue.PlateNumber).FirstOrDefault();

            if (TireIssue != null)
            {
                TireIssue.KMReading = (int)fleet.CurentOdoMeter;
                TireIssue.ModifiedBy = User.Identity.Name;
                TireIssue.LastModified = DateTime.Now;
                TireIssue.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                _TireIssueService.Update(TireIssue);
            }

            return Json(new[] { TireIssue }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssue_Destroy([DataSourceRequest] DataSourceRequest request, TireIssue tireIssue)
        {
            var TireIssue = _TireIssueService.GetById(tireIssue.TireIssueID);
            if (TireIssue != null)
            {
                _TireIssueService.Delete(TireIssue);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete Tire Issue number " + tireIssue.TireIssueID);

            }

            return Json(new[] { TireIssue }.ToDataSourceResult(request));
        }


        [AllowAnonymous]
        public ActionResult TyreForIssue()
        {

            var Tyres = db.TireReturn.Where(x => x.IsOnlineApproved == true && x.IsVoid == false && x.OrgBranchName ==  UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(e => new

            {
                SerialNo = e.SerialNo,
                Value = e.TireReturnID,
                Text = e.SerialNo + "-" + e.Brand,


            }).ToList();
            var Issues = db.TireIssue.ToList();

            foreach (var Issue in Issues)
            {
                var Battery = Tyres.Where(x => x.SerialNo == Issue.SerialNo).FirstOrDefault();
                if (Battery != null)
                {
                    Tyres.Remove(Battery);
                }
            }

            return Json(Tyres, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GetTyreInfo(Guid Id)
        {
            string Response = string.Empty;
            var Tyre = db.TireReturn.Where(x => x.TireReturnID == Id && x.IsOnlineApproved == true && x.IsVoid == false).FirstOrDefault();
            if (Tyre != null)
            {
                Response = Tyre.Brand + "_" + Tyre.TireTube + "_" + Tyre.TireSize + "_" + Tyre.TireType + "_" + Tyre.KMReading + "_" + Tyre.UnitPrice + "_" + DateTime.Parse(Tyre.DateCreated.ToString()).ToString("dd/mm/yyyy");
            }

            return Json(Response, JsonRequestBehavior.AllowGet);
        }

    }
}
