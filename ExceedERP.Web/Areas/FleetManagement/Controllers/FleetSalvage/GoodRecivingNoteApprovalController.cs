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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceApprover)] 
  
           public class GoodRecivingNoteApprovalController : Controller
    {
        //
        // GET: /FleetManagement/GoodRecivingNoteApproval/
        ExceedDbContext db = new  ExceedDbContext();
      ISalvageStoreGoodReceivingService _SalvageStoreGoodReceivingervice;

      public GoodRecivingNoteApprovalController()
        {
            _SalvageStoreGoodReceivingervice = new SalvageStoreGoodReceivingService();
        }
       

        [AuthorizeRoles(FleetManagementRoles.SalvageGoodReceivingApprover, FleetManagementRoles.StoreGoodReceivingUser)]
        public ActionResult GoodRecivingNoteApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<GoodRecivingNoteApproval> fleetApproval = db.GoodRecivingNoteApproval.Where(x => x.SalvageStoreGoodReceivingId == Id).ToList();
            List<SalvageStoreGoodReceiving> Category = db.SalvageStoreGoodReceiving.Where(x => x.SalvageStoreGoodReceivingID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new GoodRecivingNoteApproval()
                {
                    SalvageStoreGoodReceivingId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new GoodRecivingNoteApproval()
            {
                GoodRecivingNoteApprovalID = approval.GoodRecivingNoteApprovalID,
                SalvageStoreGoodReceivingId = approval.SalvageStoreGoodReceivingId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.StoreGoodReceivingUser)]

        public ActionResult GoodRecivingNote_SendForOnlineApproval(string EId)
        {

            var SalvageStoreGoodReceiving = db.SalvageStoreGoodReceiving.Where(x => x.SalvageStoreGoodReceivingID.ToString() == EId).FirstOrDefault();
            if (SalvageStoreGoodReceiving != null)
            {
                SalvageStoreGoodReceiving.IsSendForApproval = true;
                SalvageStoreGoodReceiving.Status = MaintenanceConst.WaitingApproval;

                db.SalvageStoreGoodReceiving.Attach(SalvageStoreGoodReceiving);
                db.Entry(SalvageStoreGoodReceiving).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "SalvageStoreGoodReceiving", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.StoreGoodReceivingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodRecivingNote_Approve([DataSourceRequest]DataSourceRequest request, GoodRecivingNoteApproval GoodRecivingNoteApproval, string EId)
        {
            try
            {
              //  Guid Id = Guid.Parse(EId);
                if (GoodRecivingNoteApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.SalvageStoreGoodReceiving.Where(x => x.SalvageStoreGoodReceivingID == GoodRecivingNoteApproval.SalvageStoreGoodReceivingId).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;

                    db.SalvageStoreGoodReceiving.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }

                var approval = new GoodRecivingNoteApproval()
                {

                    Status = GoodRecivingNoteApproval.Status,
                    SalvageStoreGoodReceivingId = GoodRecivingNoteApproval.SalvageStoreGoodReceivingId,
                    Remark = GoodRecivingNoteApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.GoodRecivingNoteApproval.Add(approval);
                db.SaveChanges();
                GoodRecivingNoteApproval.GoodRecivingNoteApprovalID = approval.GoodRecivingNoteApprovalID;

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { GoodRecivingNoteApproval }.ToDataSourceResult(request));
        }
    }
          }