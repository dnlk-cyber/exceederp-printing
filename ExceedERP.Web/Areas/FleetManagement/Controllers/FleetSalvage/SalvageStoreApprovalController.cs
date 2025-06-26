using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class SalvageStoreApprovalController : Controller
    {
            ExceedDbContext db = new ExceedDbContext();

     
     [AuthorizeRoles(FleetManagementRoles.SalvageStoreIssueUser, FleetManagementRoles.SalvageStoreIssueApprover)]
     public ActionResult SalvageStoreApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
     {

         Guid Id = Guid.Parse(EId);

         List<SalvageStoreApproval> fleetApproval = db.SalvageStoreApproval.Where(x => x.SalvageStoreId == Id).ToList();
         List<SalvageStore> Category = db.SalvageStore.Where(x => x.SalvageStoreID == Id).ToList();
         bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
         if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
         {
             fleetApproval.Add(new SalvageStoreApproval()
             {
                  SalvageStoreId= Id,
                 Remark = string.Empty,
                 Status = MaintenanceConst.Pending,

             });
         }


         return Json(fleetApproval.ToDataSourceResult(request, approval => new SalvageStoreApproval()
         {
             SalvageStoreApprovalID = approval.SalvageStoreApprovalID,
             ApprovedQuantity = approval.ApprovedQuantity,
             Remark = approval.Remark,
             Status = approval.Status,
             DateCreated = DateTime.Now,
             CreatedBy = User.Identity.Name


         }));
     }



     [AuthorizeRoles(FleetManagementRoles.RentRequestor)]

     public ActionResult SalvageStore_SendForOnlineApproval(string EId)
     {

         Guid Id = Guid.Parse(EId);
         var SalvageStore = db.SalvageStore.Where(x => x.SalvageStoreID == Id).FirstOrDefault();
         if (SalvageStore != null)
         {
             SalvageStore.IsSendForApproval = true;
             SalvageStore.Status = MaintenanceConst.WaitingApproval;
             db.SalvageStore.Attach(SalvageStore);
             db.Entry(SalvageStore).State = EntityState.Modified;
             db.SaveChanges();
         }


         return RedirectToActionPermanent("Index", "SalvageStore", new { area = "FleetManagement" });
     }

     [AuthorizeRoles(FleetManagementRoles.RentApprover)]
     [AcceptVerbs(HttpVerbs.Post)]
     public ActionResult SalvageStore_Approve([DataSourceRequest]DataSourceRequest request, SalvageStoreApproval SalvageStoreApproval, string EId)
     {
         try
         {
             Guid Id = Guid.Parse(EId);
             var entity = db.SalvageStore.Where(x => x.SalvageStoreID == Id).FirstOrDefault();
             var GoodRecive = db.SalvageStoreGoodReceiving.Where(x => x.Num == entity.VoucherNo).FirstOrDefault();

             if (GoodRecive != null)
             {
                 if (GoodRecive.Quantity < SalvageStoreApproval.ApprovedQuantity)
                 {
                     ModelState.AddModelError("QuantityExceed", " Quantity Exceed Good Receive Actual Amount!");
                     return Json(new[] { SalvageStoreApproval }.ToDataSourceResult(request, ModelState));
                 }
                 else
                 {
                     if (SalvageStoreApproval.Status == MaintenanceConst.Approved)
                     {

                         entity.IsOnlineApproved = true;
                         entity.OnlineApprovedBy = User.Identity.Name;
                         entity.ApprovedQuantity = SalvageStoreApproval.ApprovedQuantity;
                         entity.OnlineApprovedTime = DateTime.Now;
                         entity.Status = MaintenanceConst.Approved;
                         db.SalvageStore.Attach(entity);
                         db.Entry(entity).State = EntityState.Modified;
                         db.SaveChanges();


                         GoodRecive.Quantity = GoodRecive.Quantity - SalvageStoreApproval.ApprovedQuantity;
                         db.SalvageStoreGoodReceiving.Attach(GoodRecive);
                         db.Entry(GoodRecive).State = EntityState.Modified;
                         db.SaveChanges();
                     }
                     if (SalvageStoreApproval.Status == MaintenanceConst.Rejected)
                     {
                           entity.IsSendForApproval = false;

                           db.SalvageStore.Attach(entity);
                         db.Entry(entity).State = EntityState.Modified;
                         db.SaveChanges();
                     }
                     var approval = new SalvageStoreApproval()
                     {

                         Status = SalvageStoreApproval.Status,
                         SalvageStoreId = Id,
                         ApprovedQuantity = SalvageStoreApproval.ApprovedQuantity,
                         Remark = SalvageStoreApproval.Remark,
                         DateCreated = DateTime.Now,
                         CreatedBy = User.Identity.Name

                     };

                     db.SalvageStoreApproval.Add(approval);
                     db.SaveChanges();
                     SalvageStoreApproval.SalvageStoreApprovalID = approval.SalvageStoreApprovalID;
                 }
             }
         }
         catch (Exception e)
         {
         }
         return Json(new[] { SalvageStoreApproval }.ToDataSourceResult(request));
     }
    }
}