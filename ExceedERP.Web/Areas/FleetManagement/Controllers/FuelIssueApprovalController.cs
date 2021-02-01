using ExceedERP.ApplicationServices.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Repository;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using System.Data.Entity;
using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
                  public class FuelIssueApprovalController : Controller
    {
        //
        // GET: /FleetManagement/FuelIssueApproval/
       ExceedDbContext db = new  ExceedDbContext();
       IFleetService _FleetService;
     public FuelIssueApprovalController()
        {
            _FleetService = new FleetService();
        }
                 
        public ActionResult Index()
        {
           
            return View();
        }

              [AuthorizeRoles(FleetManagementRoles.FuelRequestor, FleetManagementRoles.FuelApprover)] 
        public ActionResult FuelIssueApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<FuelIssueApproval> fleetApproval = db.FuelIssueApproval.Where(x => x.FuelIssueId == Id).ToList();
            List<Fuel> Category = db.Fuel.Where(x => x.FuelID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new FuelIssueApproval()
                {
                    FuelIssueId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new FuelIssueApproval()
            {
                FuelIssueApprovalID = approval.FuelIssueApprovalID,
                FuelIssueId = approval.FuelIssueId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



                                [AuthorizeRoles(FleetManagementRoles.FuelRequestor)] 
  
        public ActionResult Fuel_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var Fuel = db.Fuel.Where(x => x.FuelID == Id).FirstOrDefault();
            if (Fuel != null)
            {
                Fuel.IsSendForApproval = true;
                Fuel.Status = MaintenanceConst.WaitingApproval;

                db.Fuel.Attach(Fuel);
                db.Entry(Fuel).State = EntityState.Modified;
                db.SaveChanges();
            }

        
            return RedirectToActionPermanent("Index", "Fuel", new { area = "FleetManagement" });
        }

              [AuthorizeRoles(FleetManagementRoles.FuelApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Fuel_Approve([DataSourceRequest]DataSourceRequest request, FuelIssueApproval FuelIssueApproval, string EId)
        {
            try
            {

                var Fuel = db.Fuel.Where(x => x.FuelID == FuelIssueApproval.FuelIssueId).FirstOrDefault();
                Fleets fleet = _FleetService.GetById(Fuel.FleetsID);

                var Inventory = db.FuelInventorys.Where(x => x.FuelType == fleet.TypeOfFuel && x.SiteName == fleet.Location && x.Quantity > Fuel.FuelFilledAmount).FirstOrDefault();
                if (Inventory == null)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("InsufficientQuantity", " Insufficient Amount In Depot !");
                }
                else
                {

                    Guid Id = Guid.Parse(EId);
                    if (FuelIssueApproval.Status == MaintenanceConst.Approved)
                    {
                        var entity = db.Fuel.Where(x => x.FuelID == Id).FirstOrDefault();

                        entity.IsOnlineApproved = true;
                        entity.OnlineApprovedBy = User.Identity.Name;
                        entity.OnlineApprovedTime = DateTime.Now;
                        entity.Status = MaintenanceConst.Approved;
                        db.Fuel.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();

                        Inventory.Quantity = Inventory.Quantity - Fuel.FuelFilledAmount;
                        db.FuelInventorys.Attach(Inventory);
                        db.Entry(Inventory).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                    var approval = new FuelIssueApproval()
                    {

                        Status = FuelIssueApproval.Status,
                        FuelIssueId = Id,
                        Remark = FuelIssueApproval.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.FuelIssueApproval.Add(approval);
                    db.SaveChanges();
                    FuelIssueApproval.FuelIssueApprovalID = approval.FuelIssueApprovalID;
                }
            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { FuelIssueApproval }.ToDataSourceResult(request,ModelState));
        }


    
	
	}
}