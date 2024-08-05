using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using System.Data.Entity.Validation;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Hubs;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    // [AuthorizeRoles(FleetManagementRoles.RentRequestor)] 
    public class RentFromExternalController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        IRentFromExternalService _RentFromExternalervice;

        public RentFromExternalController()
        {
            _RentFromExternalervice = new RentFromExternalService();

        }
        //
        // GET: /FleetManagement/RentFromExternal/
        public ActionResult Index()
        {
            ViewData["Category"] = db.EquipmentIdentity.Select(s => new
            {
                Value = s.EquipmentIdentityID,
                Text = s.EquipmentCategoryName
            }).ToList();

            ViewData["Type"] = db.EquipmentType.Select(s => new
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();

            ViewData["EquipmentName"] = db.EquipmentName.Select(x => new { Value = x.EquipmentNameID, Text = x.Name }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.FleetsID
            }).ToList();

            ViewData["Supplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).ToList();

            return View();
        }
        public JsonResult RentFromExternal_Read([DataSourceRequest] DataSourceRequest request)
        {
            FleetCostHelper FleetCostHelper = new FleetCostHelper();

            var RentFromExternal = _RentFromExternalervice.GetAll().ToList();//.Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));
            
            var RentFromExternals = RentFromExternal.Select(a => new RentFromExternal
            {
                Num = a.Num,
                RentFromExternalID = a.RentFromExternalID,
                AdvancePayment = a.AdvancePayment,
                BankFullAddress = a.BankFullAddress,
                BankSlipNo = a.BankSlipNo,
                CompanyAddress = a.CompanyAddress,
                CompanyName = a.CompanyName,
                CompanyEmail = a.CompanyEmail,
                CompanyPhno = a.CompanyPhno,
                Description = a.Description,
                EquipmentType = a.EquipmentType,
                FuelCoveredBy = a.FuelCoveredBy,
                NameBankBranch = a.NameBankBranch,
                ProjectLocation = a.ProjectLocation,
                Quantity = a.Quantity,
                RentalRateIncludingVAT = a.RentalRateIncludingVAT,
                Status = a.Status,
                UnitOfWorkMeasurement = a.UnitOfWorkMeasurement,
                WorkingCapacity = a.WorkingCapacity,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                DateDifference = a.EndDate != null ? Convert.ToInt32((a.EndDate.Value.Date - DateTime.Now).TotalDays) : 0,
                IsOnlineApproved = a.IsOnlineApproved,
                IsSendForApproval = a.IsSendForApproval,
                DateCreated = a.DateCreated,
                LastModified = a.LastModified,
                CreatedBy = a.CreatedBy,
                ModifiedBy = a.ModifiedBy,
                ProcurementSupplierID = a.ProcurementSupplierID,
                TotalCost = a.TotalCost,
                PlateNo = GetPlateNo(a.RentFromExternalID),
                HP_CC = a.HP_CC
            }).ToList();
            return Json(RentFromExternals.ToDataSourceResult(request));
        }
        public string GetPlateNo(Guid id)
        {

            var fleet = db.Fleets.FirstOrDefault(x => x.RentId == id);
            if(fleet != null)
            {
                return fleet.PlateNumber ?? fleet.SerialNo;
            }
            return "Empty";
        }
        public JsonResult RentFromExternal_Create([DataSourceRequest] DataSourceRequest request, RentFromExternal RentFromExternal)
        {
            ModelState.Remove("RentFromExternalID");

            if (RentFromExternal.StartDate > RentFromExternal.EndDate)
            {
                ModelState.AddModelError("InvalidDate", " Start Date Must Be Greater Thhan  End Date");

            }
            else
            {
                RentFromExternal.RentFromExternalID = Guid.NewGuid();
                RentFromExternal.CreatedBy = User.Identity.Name;
                RentFromExternal.DateCreated = DateTime.Now;
                RentFromExternal.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                _RentFromExternalervice.Insert(RentFromExternal);
                if (RentFromExternal.EndDate != null)
                {
                    Notify.NotifyUser(new NotifModels { UserId = User.Identity.Name, DeadLine = RentFromExternal.EndDate.Value, Message = "Rent From External DeadLine On" + RentFromExternal.EndDate.Value.Date, Title = " Rent From External DeadLine ", URL = Url.Action("Index", "RentFromExternal", new { area = "FleetManagment" }) });
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rent from external", "Success", User.Identity.Name, $"created rent from external reg {RentFromExternal.RentFromExternalID}", Modules.FleetManagement);
                #endregion
            }



            return Json(new[] { RentFromExternal }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RentFromExternal_Update([DataSourceRequest] DataSourceRequest request, RentFromExternal RentFromExternal)
        {


            if (RentFromExternal.StartDate > RentFromExternal.EndDate)
            {
                ModelState.AddModelError("InvalidDate", " Start Date Must Be Greater Thhan  End Date");

            }

            else
            {
                RentFromExternal.ModifiedBy = User.Identity.Name;
                RentFromExternal.LastModified = DateTime.Now;
                RentFromExternal.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                _RentFromExternalervice.Update(RentFromExternal);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rent from external", "Success", User.Identity.Name, $"updated rent from external reg {RentFromExternal.RentFromExternalID}", Modules.FleetManagement);
                #endregion

            }

            return Json(new[] { RentFromExternal }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RentFromExternal_Destroy([DataSourceRequest] DataSourceRequest request, RentFromExternal rentFromExternal)
        {
            var RentFromExternal = _RentFromExternalervice.GetById(rentFromExternal.RentFromExternalID);

            if (RentFromExternal != null)
            {
                _RentFromExternalervice.Delete(RentFromExternal);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Rent for External number " + rentFromExternal.RentFromExternalID);

            }

            return Json(new[] { RentFromExternal }.ToDataSourceResult(request));
        }




    }
}