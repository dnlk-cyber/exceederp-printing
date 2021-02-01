using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{                          
        [AuthorizeRoles(FleetManagementRoles.ResourceApprover)] 
    public class VehicleDistributionApprovalController : Controller
    {
            ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover)]
        public ActionResult VehicleDistributionApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<VehicleDistributionApproval> fleetApproval = db.VehicleDistributionApproval.Where(x => x.VehicleDistributionId == Id).ToList();
            List<VehicleDistribution> Category = db.VehicleDistribution.Where(x => x.VehicleDistributionID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new VehicleDistributionApproval()
                {
                    VehicleDistributionId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,

                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new VehicleDistributionApproval()
            {
                VehicleDistributionApprovalID = approval.VehicleDistributionApprovalID,

                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.RentRequestor)]

        public ActionResult VehicleDistribution_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var fleet = db.VehicleDistribution.Where(x => x.VehicleDistributionID == Id).FirstOrDefault();
            if (fleet != null)
            {
                fleet.IsSendForApproval = true;
                fleet.Status = MaintenanceConst.WaitingApproval;

                db.VehicleDistribution.Attach(fleet);
                db.Entry(fleet).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "EquipmentDistribution", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDistribution_Approve([DataSourceRequest]DataSourceRequest request, VehicleDistributionApproval VehicleDistributionApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (VehicleDistributionApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.VehicleDistribution.Where(x => x.VehicleDistributionID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;

                    db.VehicleDistribution.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (VehicleDistributionApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.VehicleDistribution.Where(x => x.VehicleDistributionID == Id).FirstOrDefault();
                    entity.IsSendForApproval = false;

                    db.VehicleDistribution.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
                var approval = new VehicleDistributionApproval()
                {
                    
                    Status = VehicleDistributionApproval.Status,
                       VehicleDistributionId =  Id,
                    Remark = VehicleDistributionApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.VehicleDistributionApproval.Add(approval);
                db.SaveChanges();
                VehicleDistributionApproval.VehicleDistributionApprovalID = approval.VehicleDistributionApprovalID;

            }
            catch (Exception e)
            {

             
            }
            return Json(new[] { VehicleDistributionApproval }.ToDataSourceResult(request));
        }




    }
    }