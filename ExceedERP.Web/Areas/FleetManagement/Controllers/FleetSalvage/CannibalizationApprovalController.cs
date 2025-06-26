using ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceApprover)] 
    public class CannibalizationApprovalController : Controller
    {
        //
        // GET: /FleetManagement/CannibalizationApproval/
     ExceedDbContext db = new  ExceedDbContext();
     ICannibalizationService  _CannibalizationService;

        public CannibalizationApprovalController()
        {
            _CannibalizationService = new CannibalizationService();
        }
       
         [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover)]
            public ActionResult CannibalizationApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
            {

                Guid Id = Guid.Parse(EId);

                List<CannibalizationApproval> fleetApproval = db.CannibalizationApproval.Where(x => x.CannibalizationId == Id).ToList();
                List<Cannibalization> Category = db.Cannibalization.Where(x => x.CannibalizationID == Id).ToList();
                bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
                if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
                {
                    fleetApproval.Add(new CannibalizationApproval()
                    {
                        CannibalizationId = Id,
                        Remark = string.Empty,
                        Status = MaintenanceConst.Pending,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    });
                }


                return Json(fleetApproval.ToDataSourceResult(request, approval => new CannibalizationApproval()
                {
                    CannibalizationApprovalID = approval.CannibalizationApprovalID,
                    CannibalizationId = approval.CannibalizationId,
                    Remark = approval.Remark,
                    Status = approval.Status,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name


                }));
            }



            public ActionResult Cannibalization_SendForOnlineApproval(string EId)
            {

                Guid Id = Guid.Parse(EId);
                var Cannibalization = db.Cannibalization.Where(x => x.CannibalizationID == Id).FirstOrDefault();
                if (Cannibalization != null)
                {
                    Cannibalization.IsSendForApproval = true;
                    Cannibalization.Status = MaintenanceConst.WaitingApproval;
                    db.Cannibalization.Attach(Cannibalization);
                    db.Entry(Cannibalization).State = EntityState.Modified;
                    db.SaveChanges();
                }


                return RedirectToActionPermanent("Index", "Cannibalization", new { area = "FleetManagement" });
            }

                [AuthorizeRoles(FleetManagementRoles.ResourceApprover)]
            [AcceptVerbs(HttpVerbs.Post)]
            public ActionResult Cannibalization_Approve([DataSourceRequest]DataSourceRequest request, CannibalizationApproval CannibalizationApproval, string EId)
            {
                try
                {
                    Guid Id = Guid.Parse(EId);
                    if (CannibalizationApproval.Status == MaintenanceConst.Approved)
                    {
                        var entity = db.Cannibalization.Where(x => x.CannibalizationID == Id).FirstOrDefault();

                        entity.IsOnlineApproved = true;
                        entity.OnlineApprovedBy = User.Identity.Name;
                        entity.OnlineApprovedTime = DateTime.Now;
                        entity.Status = MaintenanceConst.Approved;
                        db.Cannibalization.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (CannibalizationApproval.Status == MaintenanceConst.Rejected)
                    {
                        var entity = db.Cannibalization.Where(x => x.CannibalizationID == Id).FirstOrDefault();

                        entity.IsSendForApproval = false;

                        db.Cannibalization.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var approval = new CannibalizationApproval()
                    {

                        Status = CannibalizationApproval.Status,
                        CannibalizationId = Id,
                        Remark = CannibalizationApproval.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.CannibalizationApproval.Add(approval);
                    db.SaveChanges();
                    CannibalizationApproval.CannibalizationApprovalID = approval.CannibalizationApprovalID;

                }
                catch (Exception e)
                {

                    string ee = e.Message;
                }
                return Json(new[] { CannibalizationApproval }.ToDataSourceResult(request));
            }




    }
}