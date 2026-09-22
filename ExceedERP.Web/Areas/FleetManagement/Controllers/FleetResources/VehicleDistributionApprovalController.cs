using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Maintenance;
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

            IQueryable<VehicleDistributionApproval> fleetApproval = db.VehicleDistributionApproval.Where(x => x.VehicleDistributionId == Id);
            var dsResult = fleetApproval.ToDataSourceResult(request, model => new VehicleDistributionApproval
            {
                ApprovedBy = model.ApprovedBy,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                OrgBranchName = model.OrgBranchName,
                Remark = model.Remark,
                Status = model.Status,
                VehicleDistributionApprovalID = model.VehicleDistributionApprovalID,
                VehicleDistributionId = model.VehicleDistributionId

            });
            return Json(dsResult);
            
        }



        [AuthorizeRoles(FleetManagementRoles.RentRequestor)]

        public ActionResult VehicleDistribution_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var vehicleDistrubution = db.VehicleDistribution.FirstOrDefault(x => x.VehicleDistributionID == Id);
            if (vehicleDistrubution != null)
            {
                var assignedEquipments = db.AssignedEquipment.Where(x => x.RequestedEquipmentId == vehicleDistrubution.RequestID);
                if (assignedEquipments.Any())
                {
                    vehicleDistrubution.IsSendForApproval = true;
                    vehicleDistrubution.Status = MaintenanceConst.WaitingApproval;

                    db.VehicleDistribution.Attach(vehicleDistrubution);
                    db.Entry(vehicleDistrubution).State = EntityState.Modified;
                    db.SaveChanges();
                }
                
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
                    var checkAssignment=db.VehicleDemand.Where(x => x.VehicleDemandId == entity.RequestID).FirstOrDefault();
                    if (checkAssignment != null)
                    {
                        checkAssignment.IsAssigned = true;
                        db.VehicleDemand.Attach(checkAssignment);
                        db.Entry(checkAssignment).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    var fetchRequst = db.RequestedEquipments.FirstOrDefault(x => x.RequestNumber == entity.RequestID);
                        if(fetchRequst!=null)
                    {
                        fetchRequst.IsAssigned = true;
                        db.Entry(fetchRequst).State = EntityState.Modified;
                        db.SaveChanges();
                        var assignments = db.AssignedEquipment.Where(x => x.RequestedEquipmentId == fetchRequst.RequestedEquipmentId).ToList();
                        foreach(var ae in assignments)
                        {
                            ae.IsAssigned = true;
                            db.Entry(ae).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                        var additionalDistributions = db.VehicleDistribution.Where(x => x.ParentId == Id.ToString()).ToList();
                        foreach(var distribution in additionalDistributions)
                        {
                            entity = db.VehicleDistribution.Where(x => x.VehicleDistributionID == distribution.VehicleDistributionID).FirstOrDefault();
                            entity.IsOnlineApproved = true;
                            entity.OnlineApprovedBy = User.Identity.Name;
                            entity.OnlineApprovedTime = DateTime.Now;
                            entity.Status = MaintenanceConst.Approved;
                            db.VehicleDistribution.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();
                           checkAssignment = db.VehicleDemand.Where(x => x.VehicleDemandId == entity.RequestID).FirstOrDefault();
                            if (checkAssignment != null)
                            {
                                checkAssignment.IsAssigned = true;
                                //db.VehicleDemand.Attach(checkAssignment);
                                db.Entry(checkAssignment).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            fetchRequst = db.RequestedEquipments.FirstOrDefault(x => x.RequestNumber == entity.RequestID);
                            if (fetchRequst != null)
                            {
                                fetchRequst.IsAssigned = true;
                                db.Entry(fetchRequst).State = EntityState.Modified;
                                db.SaveChanges();
                                var assignments = db.AssignedEquipment.Where(x => x.RequestedEquipmentId == fetchRequst.RequestedEquipmentId).ToList();
                                foreach (var ae in assignments)
                                {
                                    ae.IsAssigned = true;
                                    db.Entry(ae).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
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
                    VehicleDistributionId = Id,
                    Remark = VehicleDistributionApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ApprovedBy = User.Identity.Name,
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
    }