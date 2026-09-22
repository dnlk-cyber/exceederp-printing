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
using static ExceedERP.Core.Domain.FleetManagement.FleetTireManagement.TireReturn;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
   // [AuthorizeRoles(FleetManagementRoles.TyreUser, FleetManagementRoles.TyreApprover)]
    public class TyreManagementIssueController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
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
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
     i => new
     {
         Text = i.Name,
         Value = i.Id
     }).AsQueryable();
            ViewData["type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["fleet"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + "-" + s.SerialNo
            });
            return View();
        }


        public ActionResult TireIssue_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<TireIssue> vehicleDistributions = db.TireIssue;
            DataSourceResult result = vehicleDistributions.ToDataSourceResult(request, t => new
            {

                UnitPrice = t.UnitPrice,
                OperatorName = t.OperatorName,
                Brand = t.Brand,
                PlateNumber = t.PlateNumber,
                IssueType = t.IssueType,
                DescriptionReason = t.DescriptionReason,
                InstalledDate = t.InstalledDate,
                KMReading = t.KMReading,
                StoreCode = t.StoreCode,
                SerialNo = t.SerialNo,                
                TireIssueID = t.TireIssueID,
                TireSize = t.TireSize,
                TireTube = t.TireTube,
                IssueReference = t.IssueReference,
                IssueDate = t.IssueDate,
                IsCreatedOnFleet = t.IsCreatedOnFleet,
                TireType = t.TireType,
                IsOnlineApproved = t.IsOnlineApproved,
                IsSendForApproval = t.IsSendForApproval,
                Status = t.Status,
                DateCreated = t.DateCreated,
                CreatedBy = t.CreatedBy,
                ModifiedBy = t.ModifiedBy,
                LastModified = t.LastModified,
                IssueId = t.IssueId,
                FleetId = t.FleetId,
                IssueItemId = t.IssueItemId,
                EquipmentIdentityID = t.EquipmentIdentityID,
                EquipmentNameID = t.EquipmentNameID,
                EquipmentTireId = t.EquipmentTireId,
                EquipmentTypeID = t.EquipmentTypeID,
                TireReturnId = t.TireReturnId,
                SerialNoB = t.SerialNoB,
                SerialNoC = t.SerialNoC,
                SerialNoD = t.SerialNoD

            }
             );

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssue_Create([DataSourceRequest] DataSourceRequest request, TireIssue TireIssue)
        {
            ModelState.Remove("TireReturnId");
            ModelState.Remove("TireIssueID");
            ModelState.Remove("EquipmentTireId");
            ModelState.Remove("IssueDate");
            ModelState.Remove("IsCreatedOnFleet");
            IOperatorService _OperatorService = new OperatorService();
            IFleetService fleetService = new FleetService();
            ITireReturnService _TireReturnService = new TireReturnService();
            //Fleets fleet = fleetService.GetAll().ToList().Where(x => x.FleetsID == TireIssue.FleetId).FirstOrDefault();
            var fleet = db.Fleets.Find(TireIssue.FleetId);
            TireIssue.TireIssueID = Guid.NewGuid();
            TireIssue.TireReturnId = Guid.NewGuid();
            //TireIssue.EquipmentTypeID=
            //TireIssue.FleetId = fleet.FleetsID;
            TireIssue.IssueDate = DateTime.Now;
            TireIssue.EquipmentTireId = Guid.NewGuid();
            TireIssue.Brand = TireIssue.Brand;
            TireIssue.IssueReference = TireIssue.IssueReference;
            TireIssue.IssueDate = TireIssue.IssueDate;
            TireIssue.InstalledDate = DateTime.Now;
            TireIssue.StoreCode = TireIssue.StoreCode;
            TireIssue.IsCreatedOnFleet = true;
            TireIssue.IssueDate = TireIssue.IssueDate;
           // TireIssue.IssueReference = TireIssue.IssueReference;
            TireIssue.KMReading = TireIssue.KMReading;
            TireIssue.SerialNo = TireIssue.SerialNo;
            TireIssue.IssueType = TireIssue.IssueType;
            TireIssue.TireSize = TireIssue.TireSize;
            TireIssue.TireTube = TireIssue.TireTube;
            TireIssue.TireType = TireTypeenum.Wire.ToString();
            TireIssue.UnitPrice = TireIssue.UnitPrice;
            TireIssue.PlateNumber = fleet.PlateNumber;
            TireIssue.CreatedBy = User.Identity.Name;
            TireIssue.DateCreated = DateTime.Now;
            TireIssue.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            if(ModelState.IsValid)
            {
                db.TireIssue.Add(TireIssue);
                db.SaveChanges();
            }
           
            //_TireIssueService.Insert(TireIssue);
            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "Tyre management issue registration", "Success", User.Identity.Name, $"created tyre mangement issue {TireIssue.TireIssueID}", Modules.FleetManagement);
            #endregion


            return Json(new[] { TireIssue }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssue_Update([DataSourceRequest] DataSourceRequest request, TireIssue TireIssue)
        {
            IFleetService fleetService = new FleetService();
            Fleets fleet = fleetService.GetAll().Where(x => x.PlateNumber == TireIssue.PlateNumber || x.FleetsID == TireIssue.FleetId).FirstOrDefault();

            if (TireIssue != null)
            {
                TireIssue.KMReading = (int)fleet.CurentOdoMeter;
                TireIssue.ModifiedBy = User.Identity.Name;
                TireIssue.LastModified = DateTime.Now;
                TireIssue.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                _TireIssueService.Update(TireIssue);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Tyre management issue registration", "Success", User.Identity.Name, $"updated tyre management issue {TireIssue.TireIssueID}", Modules.FleetManagement);
                #endregion
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


        
        public ActionResult TyreForIssue()
        {

            var Tyres = db.TireReturn.ToList().Where(x => x.IsOnlineApproved == true && x.IsVoid == false && x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(e => new
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
