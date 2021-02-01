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


namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
      [AuthorizeRoles(FleetManagementRoles.ResourceRequestor)] 
    public class SitesVehicleTransferController : Controller
    {

          ExceedDbContext db = new ExceedDbContext();

                  ISitesVehicleTransferService _SitesVehicleTransferService;

        public SitesVehicleTransferController()
        {
            _SitesVehicleTransferService = new SitesVehicleTransferService();
        }
        //
        // GET: /FleetManagement/SitesVehicleTransfer/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult SitesVehicleTransfer_Read([DataSourceRequest] DataSourceRequest request)
        {
            var SitesVehicleTransfer = _SitesVehicleTransferService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(s => new SitesVehicleTransfer
                {
                    Description = s.Description,
                    FleetsID = s.FleetsID,
                    PlateNo = s.Fleet.PlateNumber,
                    ToDate = s.ToDate,
                    StartingDate = s.StartingDate,
                    DestinationSite = s.DestinationSite,
                    InitialSite = s.InitialSite,
                    EquipmentTypeId = s.Fleet.EquipmentIdentityID.ToString(),
                    Remark = s.Fleet.PlateNumber + " -- " + s.Fleet.EquipmentName,
                    SitesVehicleTransferID = s.SitesVehicleTransferID  ,
                    IsOnlineApproved = s.IsOnlineApproved,
                    IsSendForApproval = s.IsSendForApproval,
                    CreatedBy = s.CreatedBy,
                    DateCreated = s.DateCreated,
                    ModifiedBy = s.ModifiedBy,
                    LastModified = s.LastModified,
                    Status = s.Status
                   

                }
                );

            return Json(SitesVehicleTransfer.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Create([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer SitesVehicleTransfer)
        {
            ModelState.Remove("SitesVehicleTransferID");
            ModelState.Remove("InitialSite");

            Fleets fleet = db.Fleets.Where(x => x.FleetsID == SitesVehicleTransfer.FleetsID).FirstOrDefault();
                   
            if (SitesVehicleTransfer.StartingDate > SitesVehicleTransfer.ToDate)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("", "Start Date is Above End Date");
                }
            else if (SitesVehicleTransfer.DestinationSite == fleet.Location)
            {
                ModelState.Clear();
                ModelState.AddModelError("SameSite", "Intial and Destination Site Must be Different");
            }
            else
            {
                if (SitesVehicleTransfer != null)
                {

                    SitesVehicleTransfer.SitesVehicleTransferID = Guid.NewGuid();
                    SitesVehicleTransfer.InitialSite = fleet.Location;
                    SitesVehicleTransfer.DestinationSite = SitesVehicleTransfer.DestinationSite;
                    SitesVehicleTransfer.CreatedBy = User.Identity.Name;
                    SitesVehicleTransfer.DateCreated = DateTime.Now;
                    SitesVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    bool IsInserted = _SitesVehicleTransferService.Insert(SitesVehicleTransfer);

                    var EquipmentTyres = db.EquipmentTyres.Where(x => x.fleetId == SitesVehicleTransfer.FleetsID && x.Isattached).ToList();

                    foreach (var EquipmentTyre in EquipmentTyres)
                    {

                        var TyreDetail = new SiteVechicleTransferTyreDetail()
                        {
                            SiteVechicleTransferId = SitesVehicleTransfer.SitesVehicleTransferID,
                            SerialNo = EquipmentTyre.SerialNo,
                            Type = EquipmentTyre.Type,
                            Description = EquipmentTyre.Make +" "+EquipmentTyre.Condition,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now

                        };
                        db.SiteVechicleTransferTyreDetailes.Add(TyreDetail);
                        db.SaveChanges();


                    }
                }
            }
            return Json(new[] { SitesVehicleTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Update([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer SitesVehicleTransfer)
        {
            if (SitesVehicleTransfer.StartingDate > SitesVehicleTransfer.ToDate)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("", "Start Date is Above End Date");
                }
            else
                {
                    if (SitesVehicleTransfer != null)
                        {
                            SitesVehicleTransfer.ModifiedBy = User.Identity.Name;
                            SitesVehicleTransfer.LastModified = DateTime.Now;
                            SitesVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                            _SitesVehicleTransferService.Update(SitesVehicleTransfer);
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
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete Site Equipment Transfer number " + sitesVehicleTransfer.SitesVehicleTransferID);
           
            }

            return Json(new[] { SitesVehicleTransfer }.ToDataSourceResult(request));
        }



    }
}