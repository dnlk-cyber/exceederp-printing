using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; 
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceRequestor)] 
    public class SitesVehicleTransferSelfCareController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        ISitesVehicleTransferService _SitesVehicleTransferService;

        public SitesVehicleTransferSelfCareController()
        {
            _SitesVehicleTransferService = new SitesVehicleTransferService();
        }
        //
        // GET: /FleetManagement/SitesVehicleTransfer/
        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers;
            ViewData["Employee"] = CacheData.GetEmployeeCaches().Select(
                i => new
                {
                    Name = i.Name,
                    Code = i.Id
                }).AsQueryable();
            ViewData["VehiclePartDamageType"] = db.VehiclePartDamageType;
            ViewData["VehicleParts"] = db.VehiclePart;
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(s => new {
                Id = s.Id,
                Name = s.Name
            }).AsQueryable();
            ViewData["Fleets"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber ?? "" + "/" + s.SerialNo ?? ""
            });
            ViewData["Branch"] = CacheData.GetBranchCaches().Select(
         i => new
         {
             Name = i.Name,
             BranchId = i.Id
         }).AsQueryable();
            ViewData["Operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.EmployeeCode+" "+s.FullName
            }).AsQueryable();
            ViewData["OperatorTwo"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.EmployeeCode + " " + s.FullName
            });

            ViewData["EquipmentCategory"] = CacheData.GetEquipmentIdentityCaches().Select(
    i => new
    {
        Text = i.Name,
        Value = i.Id
    }).AsQueryable();
            ViewData["EquipmentType"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["EquipmentName"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            return View();
        }


        public ActionResult SitesVehicleTransfer_Read([DataSourceRequest] DataSourceRequest request)
        {
            var defaultBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

            // Now use it in your EF query
            IQueryable<SitesVehicleTransfer> SVT = db.SitesVehicleTransfer
                .Where(x => x.OrgBranchName == defaultBranch);
            DataSourceResult Result= SVT.ToDataSourceResult(request,s => new
            {
                Description = s.Description,
                FleetsID = s.FleetsID,
                PlateNo = s.Fleet.PlateNumber,
                ToDate = s.ToDate,
                StartingDate = s.StartingDate,
                DestinationSite = s.DestinationSite,
                InitialSite = s.InitialSite,
                OperatorId = s.OperatorId,
                SecondOperatorId = s.SecondOperatorId,
                //EquipmentTypeId = s.Fleet.EquipmentIdentityID.ToString(),
                Remark = s.Fleet.PlateNumber + " -- " + s.Fleet.EquipmentName,
                SitesVehicleTransferID = s.SitesVehicleTransferID  ,
                IsOnlineApproved = s.IsOnlineApproved,
                IsSendForApproval = s.IsSendForApproval,
                CreatedBy = s.CreatedBy,
                DateCreated = s.DateCreated,
                ModifiedBy = s.ModifiedBy,
                LastModified = s.LastModified,
                Status = s.Status,
                IsOperationApproved = s.IsOperationApproved,
                IsDepartmentApproved = s.IsDepartmentApproved,
                TransferStatus = s.TransferStatus,
                BranchId = s.BranchId,
                EquipmentNameID = s.EquipmentNameID,
                EquipmentIdentityID = s.EquipmentIdentityID,
                EquipmentTypeID = s.EquipmentTypeID,
                
                   

            });

            return Json(Result,JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Create([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer sitesVehicleTransfer)
        {
            ModelState.Remove("SitesVehicleTransferID");
            ModelState.Remove("InitialSite");
            ModelState.Remove("EquipmentIdentityID");
            ModelState.Remove("EquipmentTypeID");
            ModelState.Remove("EquipmentNameID");
           
            Fleets fleet = db.Fleets.Where(x => x.FleetsID == sitesVehicleTransfer.FleetsID).FirstOrDefault();

            if (sitesVehicleTransfer.StartingDate > sitesVehicleTransfer.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            else if (sitesVehicleTransfer.BranchId == fleet.BranchId)
            {
                ModelState.Clear();
                ModelState.AddModelError("SameSite", "Intial and Destination Site Must be Different");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (sitesVehicleTransfer != null)
                    {
                        sitesVehicleTransfer.SitesVehicleTransferID = Guid.NewGuid();
                        sitesVehicleTransfer.EquipmentIdentityID = fleet.EquipmentIdentityID;
                        sitesVehicleTransfer.EquipmentTypeID = fleet.EquipmentTypeID;
                        sitesVehicleTransfer.EquipmentNameID = fleet.EquipmentNameID;
                        sitesVehicleTransfer.OperatorId = sitesVehicleTransfer.OperatorId;
                        sitesVehicleTransfer.SecondOperatorId = sitesVehicleTransfer.SecondOperatorId;
                        sitesVehicleTransfer.InitialSite = fleet.Location;
                        sitesVehicleTransfer.DestinationSite = sitesVehicleTransfer.DestinationSite;
                        sitesVehicleTransfer.CreatedBy = User.Identity.Name;
                        sitesVehicleTransfer.DateCreated = DateTime.Now;
                        sitesVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        bool IsInserted = _SitesVehicleTransferService.Insert(sitesVehicleTransfer);
                        //ChangeSiteTransferOnFleet(sitesVehicleTransfer.FleetsID, sitesVehicleTransfer.BranchId);

                        var EquipmentTyres = db.EquipmentTyres.Where(x => x.fleetId == sitesVehicleTransfer.FleetsID && x.Isattached).ToList();
                        fleet.Location = db.Branch.FirstOrDefault(x => x.BranchId == sitesVehicleTransfer.BranchId)?.Name ?? "";
                        var AssetRegistration = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == fleet.AssetId);
                        if (AssetRegistration != null && fleet != null)
                        {
                            AssetRegistration.Location = fleet.Location;
                            db.Entry(AssetRegistration).State = EntityState.Modified;
                            db.Entry(fleet).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                       

                        foreach (var EquipmentTyre in EquipmentTyres)
                        {

                            var TyreDetail = new SiteVechicleTransferTyreDetail()
                            {
                                SiteVechicleTransferId = sitesVehicleTransfer.SitesVehicleTransferID,
                                SerialNo = EquipmentTyre.SerialNo,
                                Type = EquipmentTyre.Type,
                                Description = EquipmentTyre.Make + " " + EquipmentTyre.Condition,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now
                            };
                            db.SiteVechicleTransferTyreDetailes.Add(TyreDetail);
                            db.SaveChanges();
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer registration", "Success", User.Identity.Name, $"created site vehicle transfer {TyreDetail.SiteVechicleTransferTyreDetailId}", Modules.FleetManagement);
                            #endregion


                        }
                    }
                }
            }
            return Json(new[] { sitesVehicleTransfer }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Update([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer SitesVehicleTransfer)
        {
            ModelState.Remove("Num");
            ModelState.Remove("FuelAmount");
            ModelState.Remove("BatteryStatus");
            ModelState.Remove("EquipmentCondition");
            ModelState.Remove("TransferMode");
            ModelState.Remove("Hydrolic");
            if (SitesVehicleTransfer.StartingDate > SitesVehicleTransfer.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            
            else
            {
                if (ModelState.IsValid)
                {
                    if (SitesVehicleTransfer != null)
                    {
                        SitesVehicleTransfer.ModifiedBy = User.Identity.Name;
                        SitesVehicleTransfer.LastModified = DateTime.Now;
                        SitesVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        _SitesVehicleTransferService.Update(SitesVehicleTransfer);
                        //ChangeSiteTransferOnFleet(SitesVehicleTransfer.FleetsID, SitesVehicleTransfer.BranchId);
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer registration", "Success", User.Identity.Name, $"updated site vehicle transfer {SitesVehicleTransfer.SitesVehicleTransferID}", Modules.FleetManagement);
                        #endregion
                    }

                }
            }

            return Json(new[] { SitesVehicleTransfer }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Destroy([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer sitesVehicleTransfer)
        {
            var SitesVehicleTransfer = _SitesVehicleTransferService.GetById(sitesVehicleTransfer.SitesVehicleTransferID);
            if (SitesVehicleTransfer != null)
            {
                _SitesVehicleTransferService.Delete(SitesVehicleTransfer);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete site equipment transfer number " + sitesVehicleTransfer.SitesVehicleTransferID);

            }
            return Json(new[] { SitesVehicleTransfer }.ToDataSourceResult(request));
        }
        public ActionResult SiteVehicle_SendForOnlineApproval(string EId)
        {

            //Guid Id = Guid.Parse(EId);
            var SitesVehicleTransfer = db.SitesVehicleTransfer.Where(x => x.SitesVehicleTransferID.ToString() == EId).FirstOrDefault();
            if (SitesVehicleTransfer != null)
            {
                SitesVehicleTransfer.IsSendForApproval = true;
                SitesVehicleTransfer.TransferStatus = TransferStatus.WaitingOperationManager;
                //db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(SitesVehicleTransfer).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formattedData = new
            {
                Message = "Request Send For Operation Approval",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

            return RedirectToActionPermanent("Index", "SitesVehicleTransferSelfCare", new { area = "FleetManagement" });
        }
     

        public JsonResult OperationApproval(Guid id)
        {
           var siteTransfer= db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            siteTransfer.IsOperationApproved = true;
            siteTransfer.OperationApprvedBy = User.Identity.Name;
            siteTransfer.TransferStatus = TransferStatus.WaitingDepartmentGM;
            siteTransfer.OperationApprovalDate = DateTime.Now;
            db.Entry(siteTransfer).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Approved Successfully",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OperationReject(Guid id)
        {
           var siteTransfer= db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            siteTransfer.IsOperationApproved = false;
            siteTransfer.IsSendForApproval = false;
            siteTransfer.OperationApprvedBy = User.Identity.Name;
            siteTransfer.TransferStatus = TransferStatus.Draft;
            siteTransfer.OperationApprovalDate = DateTime.Now;
            db.Entry(siteTransfer).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Rejected Successfully",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DGMApproval(Guid id)
        {
           var siteTransfer= db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            siteTransfer.IsDepartmentApproved = true;
            siteTransfer.OperationApprvedBy = User.Identity.Name;
            siteTransfer.TransferStatus = TransferStatus.Approved;
            siteTransfer.DGMApprovalDate = DateTime.Now;
            db.Entry(siteTransfer).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Approved Successfully",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DGMReject(Guid id)
        {
           var siteTransfer= db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            siteTransfer.IsDepartmentApproved = false;
            siteTransfer.IsOperationApproved = false;
            siteTransfer.DGMApprovedBy = User.Identity.Name;
            siteTransfer.TransferStatus = TransferStatus.WaitingOperationManager;
            siteTransfer.DGMApprovalDate = DateTime.Now;
            db.Entry(siteTransfer).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Rejected Successfully",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}