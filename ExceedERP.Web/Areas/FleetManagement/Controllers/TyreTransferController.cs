using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class TyreTransferController : Controller
    {
        //
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/TyreTransfer/
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult TyreTransfer_Read([DataSourceRequest] DataSourceRequest request)
        {
            var AllTransfer = db.TyreTransfers.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));
            List<TyreTransfer> Transfers = new List<TyreTransfer>();

            string Site = string.Empty;

            foreach (var Transfer in AllTransfer)
            {
                if (Site != Transfer.FromEquipment)
                {
                    Site = Transfer.FromEquipment;

                    var TransferBySite = AllTransfer.Where(x => x.FromEquipment == Site).ToList();
                    var TyreTransfer = new TyreTransfer

                    {
                        TyreTransferId = Transfer.TyreTransferId,
                        ToEquipment = Transfer.ToEquipment,
                        Description = TransferBySite.Count.ToString(),
                        ModifiedBy = Transfer.ModifiedBy,
                        DateCreated = Transfer.DateCreated,
                        CreatedBy = Transfer.CreatedBy,
                        OrgBranchName =   UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),


                    };
                    Transfers.Add(TyreTransfer);

                }


            }



            return Json(Transfers.ToDataSourceResult(request));

        }

        public ActionResult TyreTransferByFleet_Read([DataSourceRequest] DataSourceRequest request)
        {

            var TyreTransfer = db.TyreTransfers.Select(e => new

            {
                TyreTransferId = e.TyreTransferId,

                FromEquipment = e.FromEquipment,
                ToEquipment = e.ToEquipment,
                TyreSerialNo = e.TyreSerialNo,
                LastModified = e.LastModified,
                Description = e.Description,
    
                Remark = e.Remark,
                IsOnlineApproved = e.IsOnlineApproved,
                IsSendForApproval = e.IsSendForApproval,
                Status = e.Status,
                ModifiedBy = e.ModifiedBy,
                DateCreated = e.DateCreated,
                CreatedBy = e.CreatedBy,

            }
        ).ToList();

            return Json(TyreTransfer.ToDataSourceResult(request));

        }


        public ActionResult TyreTransfer_Create([DataSourceRequest] DataSourceRequest request, TyreTransfer TyreTransfer)
        {
            ModelState.Remove("TyreTransferId");

            if (TyreTransfer.FromEquipment == TyreTransfer.ToEquipment)
            {
                ModelState.AddModelError("SameFleet","Choose Diffrent Equipment!");
            }
            else
            {
                
            
            if (TyreTransfer != null)
                {
                    var Fromfleet = db.Fleets.Where(x => x.FleetsID.ToString() == TyreTransfer.FromEquipment).FirstOrDefault();
                    var Tofleet = db.Fleets.Where(x => x.FleetsID.ToString() == TyreTransfer.ToEquipment).FirstOrDefault();

                    var entity = new TyreTransfer
                    {

                        TyreTransferId = Guid.NewGuid(),
                        FromEquipment = Fromfleet.UnitNum ,
                        TyreSerialNo = TyreTransfer.TyreSerialNo,
                        ToEquipment = Tofleet.UnitNum,
                        Description = TyreTransfer.Description,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        Remark = TyreTransfer.Remark,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db = new ExceedDbContext();

                    db.TyreTransfers.Add(entity);
                    db.SaveChanges();
                    TyreTransfer.TyreTransferId = entity.TyreTransferId;

                }
            }
            return Json(new[] { TyreTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TyreTransfer_Update([DataSourceRequest] DataSourceRequest request, TyreTransfer TyreTransfer)
        {


            if (TyreTransfer != null)
            {
                TyreTransfer.LastModified = DateTime.Now;
                TyreTransfer.ModifiedBy = User.Identity.Name;
                TyreTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                db.TyreTransfers.Attach(TyreTransfer);
                db.Entry(TyreTransfer).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { TyreTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TyreTransfer_Destroy([DataSourceRequest] DataSourceRequest request, TyreTransfer TyreTransfer)
        {
            var TyreTransferObj = db.TyreTransfers.Where(x => x.TyreTransferId == TyreTransfer.TyreTransferId).FirstOrDefault();
            if (TyreTransfer != null)
            {

                db.TyreTransfers.Attach(TyreTransferObj);
                db.TyreTransfers.Remove(TyreTransferObj);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Driver Assignment number " + TyreTransfer.TyreTransferId);

            }

            return Json(new[] { TyreTransfer }.ToDataSourceResult(request));
        }


        [AllowAnonymous]
        public ActionResult UnAssignedEquipment()
        {

            var FixedAssets = db.Fleets.Where(x => x.Status.ToString() == MaintenanceConst.ACTIVE && x.OrgBranchName ==    UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).FirstOrDefault();

            return Json(FixedAssets, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult UnAssigned()
        {

            var FixedAssets = db.Fleets.Where(x => x.Status.ToString() == MaintenanceConst.ACTIVE && x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).FirstOrDefault();

            return Json(FixedAssets, JsonRequestBehavior.AllowGet);
        }
    }
}