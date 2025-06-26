using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.DataAccess.Context;
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
//using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.SalvageStoreIssueApprover, FleetManagementRoles.SalvageStoreIssueApprover)]
    public class SalvageStoreController : Controller
    {
        IFleetSalvageStoreService _SalvageStoreService;
        IFleetService _FleetService;
        public SalvageStoreController()
        {
            _SalvageStoreService = new FleetSalvageStoreService();
            _FleetService = new FleetService();
        }

        //
        // GET: /FleetManagement/SalvageStore/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult SalvageStore_Read([DataSourceRequest] DataSourceRequest request)
        {
            var SalvageStore = _SalvageStoreService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(s => new SalvageStore
            {
                Date = s.Date,
                Description = s.Description,
                PlateNumber = s.Fleet.PlateNumber,
                FleetsID = s.FleetsID,
                SalvageStoreID = s.SalvageStoreID,
                SalvageStoreGoodReceivingId = s.SalvageStoreGoodReceivingId,
                SparePartName = s.SparePartName,
                VoucherNo = s.VoucherNo,
                Quantity = s.Quantity,
                ApprovedQuantity = s.ApprovedQuantity,
                IsOnlineApproved = s.IsOnlineApproved,
                IsSendForApproval = s.IsSendForApproval,
                DateCreated = s.DateCreated,
                CreatedBy = s.CreatedBy,
                LastModified = s.LastModified,
                ModifiedBy = s.ModifiedBy,
                   Status = s.Status

            });

            return Json(SalvageStore.ToDataSourceResult(request));
        }
        public ActionResult SalvageStoreRequstion_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var SalvageStore = _SalvageStoreService.GetAll().Where(x=>x.JobId.ToString() == Id).Select(s => new SalvageStore
            {
                Date = s.Date,
                Description = s.Description,
                PlateNumber = s.Fleet.PlateNumber,
                FleetsID = s.FleetsID,
                SalvageStoreID = s.SalvageStoreID,
                SalvageStoreGoodReceivingId = s.SalvageStoreGoodReceivingId,
                SparePartName = s.SparePartName,
                VoucherNo = s.VoucherNo,
                Quantity = s.Quantity,
                Price = s.Price,
                ApprovedQuantity = s.ApprovedQuantity,
                IsOnlineApproved = s.IsOnlineApproved,
                IsSendForApproval = s.IsSendForApproval,
                DateCreated = s.DateCreated,
                CreatedBy = s.CreatedBy,
                LastModified = s.LastModified,
                ModifiedBy = s.ModifiedBy,
                  OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                Status = s.Status

            });

            return Json(SalvageStore.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalvageStoreRequistion_Create([DataSourceRequest] DataSourceRequest request, SalvageStore SalvageStore, Guid Id)
        {
            ExceedDbContext db = new ExceedDbContext();
            IJobOrderService _JobOrderService = new JobOrderService();
            IJobOrderDetailService _JobOrderDetailService = new JobOrderDetailService();

            Guid FleetId = Guid.Empty;
            int ReceiveNo = int.Parse(SalvageStore.SparePartName);
            var GoodRecive = db.SalvageStoreGoodReceiving.Where(x => x.Num == ReceiveNo).FirstOrDefault();
             var Job = _JobOrderService.GetById(Id);
            if(Job != null)
            {
                FleetId = Job.FleetsID;
            }

            if (SalvageStore != null)
            {
                SalvageStore.SalvageStoreID = Guid.NewGuid();
                SalvageStore.Date = DateTime.Now;
                SalvageStore.JobId = Id;
                SalvageStore.FleetsID = FleetId;
                SalvageStore.SalvageStoreGoodReceivingId = GoodRecive.SalvageStoreGoodReceivingID;
                SalvageStore.VoucherNo = GoodRecive.Num;
                SalvageStore.SparePartName = GoodRecive.SparePartName;
                SalvageStore.Price = GoodRecive.UnitPrice;
                SalvageStore.CreatedBy = User.Identity.Name;
                SalvageStore.DateCreated = DateTime.Now;
                SalvageStore.Status = MaintenanceConst.Pending;
                SalvageStore.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                _SalvageStoreService.Insert(SalvageStore);
            }

            return Json(new[] { SalvageStore }.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalvageStore_Create([DataSourceRequest] DataSourceRequest request, SalvageStore SalvageStore)
        {
            ExceedDbContext db = new ExceedDbContext();

            int ReceiveNo = int.Parse(SalvageStore.SparePartName.ToString());
            var GoodRecive = db.SalvageStoreGoodReceiving.Where(x => x.Num == ReceiveNo).FirstOrDefault();
          

            if (SalvageStore != null)
            {
                SalvageStore.SalvageStoreID = Guid.NewGuid();
                SalvageStore.Date = DateTime.Now;
                SalvageStore.JobId =Guid.Empty;
                SalvageStore.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                SalvageStore.SalvageStoreGoodReceivingId = GoodRecive.SalvageStoreGoodReceivingID;
                SalvageStore.VoucherNo = GoodRecive.Num;
                SalvageStore.SparePartName = GoodRecive.SparePartName;
                SalvageStore.Price = GoodRecive.UnitPrice;
                SalvageStore.CreatedBy = User.Identity.Name;
                SalvageStore.DateCreated = DateTime.Now;
                SalvageStore.Status = MaintenanceConst.Pending;
                _SalvageStoreService.Insert(SalvageStore);
            }


            return Json(new[] { SalvageStore }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalvageStore_Update([DataSourceRequest] DataSourceRequest request, SalvageStore SalvageStore)
        {
            if (SalvageStore != null)
            {
                SalvageStore.ModifiedBy = User.Identity.Name;
                SalvageStore.LastModified = DateTime.Now;
                SalvageStore.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                _SalvageStoreService.Update(SalvageStore);
            }

            return Json(new[] { SalvageStore }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalvageStore_Destroy([DataSourceRequest] DataSourceRequest request, SalvageStore salvageStore)
        {
            var SalvageStore = _SalvageStoreService.GetById(salvageStore.SalvageStoreID);
            if (SalvageStore != null)
            {
                _SalvageStoreService.Delete(SalvageStore);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete Salvage Store number " + salvageStore.SalvageStoreID);

            }

            return Json(new[] { SalvageStore }.ToDataSourceResult(request));
        }



    }
}

