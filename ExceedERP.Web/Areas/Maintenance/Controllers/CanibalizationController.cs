using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class CanibalizationController : Controller
    {
        // GET: Maintenance/Canibalization
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult Canibalization_Read([DataSourceRequest]DataSourceRequest request,string id)
        {
            var FetchCanibalization = db.Canibalizations.Where(x => x.ServiceCenterJobId == id).AsQueryable();
           //if(FetchCanibalization==null)
           // {
           //     return Json()
           // }
            DataSourceResult Result = FetchCanibalization.ToDataSourceResult(request, CanibalizationRecord => new
            {
                CanibalizationId = CanibalizationRecord.CanibalizationId,
                ServiceCenterJobId = CanibalizationRecord.ServiceCenterJobId,
                ProvidingMachineEngineModel = CanibalizationRecord.ProvidingMachineEngineModel,
                ProvidingMachineEngineNumber = CanibalizationRecord.ProvidingMachineEngineNumber,
                ProvidingMachinePlateNumber = CanibalizationRecord.ProvidingMachinePlateNumber,
                ProvidingMachinModel = CanibalizationRecord.ProvidingMachinModel,
                RequestingMachineEngineModel = CanibalizationRecord.RequestingMachineEngineModel,
                RequestingMachineEngineNumber = CanibalizationRecord.RequestingMachineEngineNumber,
                RequestingMachinePlateNumber = CanibalizationRecord.RequestingMachinePlateNumber,
                RequestingMachinModel = CanibalizationRecord.RequestingMachinModel,
                CreatedBy = CanibalizationRecord.CreatedBy,
                DateCreated = CanibalizationRecord.DateCreated,
                LastModified = CanibalizationRecord.LastModified,
                ModifiedBy = CanibalizationRecord.ModifiedBy,
                IsSendForApproval = CanibalizationRecord.IsSendForApproval,
                IsVoid = CanibalizationRecord.IsVoid,
                OnlineApprovedBy = CanibalizationRecord.OnlineApprovedBy,
                OnlineApprovedTime = CanibalizationRecord.OnlineApprovedTime,
                OnlineTransferredTime = CanibalizationRecord.OnlineTransferredTime,
                OrgBranchName = CanibalizationRecord.OrgBranchName,
                Remark = CanibalizationRecord.Remark,
                SecondOnlineApprovedBy = CanibalizationRecord.SecondOnlineApprovedBy,
                SecondOnlineApprovedTime = CanibalizationRecord.SecondOnlineApprovedTime,
                SecondSendForApprovalBy = CanibalizationRecord.SecondSendForApprovalBy,
                SecondSendForApprovalTime = CanibalizationRecord.SecondSendForApprovalTime,
                SendForApprovalBy = CanibalizationRecord.SendForApprovalBy,
                SendForApprovalTime = CanibalizationRecord.SendForApprovalTime,
                Status = CanibalizationRecord.Status,
                VoidBy = CanibalizationRecord.VoidBy,
                VoidTime = CanibalizationRecord.VoidTime,
                IsDepartmentOnlineApproved = CanibalizationRecord.IsDepartmentOnlineApproved,
                IsOnlineApproved = CanibalizationRecord.IsOnlineApproved,
                IsOnlineTransferred = CanibalizationRecord.IsOnlineTransferred,
                IsOnlineTransferredBy = CanibalizationRecord.IsOnlineTransferredBy,
                IsSecondOnlineApproved = CanibalizationRecord.IsSecondOnlineApproved,
                IsSecondSendForApproval = CanibalizationRecord.IsSecondSendForApproval,
                IsECWC=GetIsECWC()

                
            });

            return Json(Result,JsonRequestBehavior.AllowGet);
        }

        private bool GetIsECWC()
        {
            bool IsSet = false;
            var fetchSetting = db.RequestToInspections.FirstOrDefault();
               if(fetchSetting!=null&&fetchSetting.Remark=="OnWorkShop")
            {
                IsSet = true;
            }
                return IsSet;
        }
        public ActionResult Canibalization_Create([DataSourceRequest]DataSourceRequest request,Canibalization canibalization,string id)
        {
            var RequestingMachinePlateNumber = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == id).LicensePlate;
            if(RequestingMachinePlateNumber!=null)
            {
                var FetchFleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == RequestingMachinePlateNumber);
                if(FetchFleet!=null)
                {
                    canibalization.RequestingMachinePlateNumber = FetchFleet.PlateNumber;
                    canibalization.RequestingMachinModel = FetchFleet.Model;
                    canibalization.RequestingMachineEngineNumber = FetchFleet.EngineNumber;
                    canibalization.ProvidingMachineEngineModel = FetchFleet.EngineModel;
                }
            }
            if(canibalization.ProvidingMachinePlateNumber!=null)
            {
                var FetchFleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == canibalization.ProvidingMachinePlateNumber);
                if (FetchFleet != null)
                {                    
                    canibalization.ProvidingMachinModel = FetchFleet.Model;
                    canibalization.ProvidingMachineEngineNumber = FetchFleet.EngineNumber;
                    canibalization.ProvidingMachineEngineModel = FetchFleet.EngineModel;
                }
            }
            canibalization.ServiceCenterJobId = id;
            if (RequestingMachinePlateNumber == null)
                ModelState.AddModelError("Requesting Machine Plate Not Found", "Requesting Machine Plate Not Found");
            if(canibalization.ProvidingMachinePlateNumber==null)
                ModelState.AddModelError("Providing Machine Plate Not Found", "Providing Machine Plate Not Found");
            canibalization.CreatedBy = User.Identity.Name;
            canibalization.DateCreated = DateTime.Now;
            if(ModelState.IsValid)
            {
                var entity = (Canibalization)this.GetMappedCanibalization(canibalization);
                db.Canibalizations.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { canibalization }.ToDataSourceResult(request, ModelState));
        }
        private object GetMappedCanibalization(Canibalization CanibalizationRecord)
        {
            var entity = new Canibalization
            {
                CanibalizationId = CanibalizationRecord.CanibalizationId,
                ServiceCenterJobId = CanibalizationRecord.ServiceCenterJobId,
                ProvidingMachineEngineModel = CanibalizationRecord.ProvidingMachineEngineModel,
                ProvidingMachineEngineNumber = CanibalizationRecord.ProvidingMachineEngineNumber,
                ProvidingMachinePlateNumber = CanibalizationRecord.ProvidingMachinePlateNumber,
                ProvidingMachinModel = CanibalizationRecord.ProvidingMachinModel,
                RequestingMachineEngineModel = CanibalizationRecord.RequestingMachineEngineModel,
                RequestingMachineEngineNumber = CanibalizationRecord.RequestingMachineEngineNumber,
                RequestingMachinePlateNumber = CanibalizationRecord.RequestingMachinePlateNumber,
                RequestingMachinModel = CanibalizationRecord.RequestingMachinModel,
                CreatedBy = CanibalizationRecord.CreatedBy,
                DateCreated = CanibalizationRecord.DateCreated,
                LastModified = CanibalizationRecord.LastModified,
                ModifiedBy = CanibalizationRecord.ModifiedBy,
                IsSendForApproval = CanibalizationRecord.IsSendForApproval,
                IsVoid = CanibalizationRecord.IsVoid,
                OnlineApprovedBy = CanibalizationRecord.OnlineApprovedBy,
                OnlineApprovedTime = CanibalizationRecord.OnlineApprovedTime,
                OnlineTransferredTime = CanibalizationRecord.OnlineTransferredTime,
                OrgBranchName = CanibalizationRecord.OrgBranchName,
                Remark = CanibalizationRecord.Remark,
                SecondOnlineApprovedBy = CanibalizationRecord.SecondOnlineApprovedBy,
                SecondOnlineApprovedTime = CanibalizationRecord.SecondOnlineApprovedTime,
                SecondSendForApprovalBy = CanibalizationRecord.SecondSendForApprovalBy,
                SecondSendForApprovalTime = CanibalizationRecord.SecondSendForApprovalTime,
                SendForApprovalBy = CanibalizationRecord.SendForApprovalBy,
                SendForApprovalTime = CanibalizationRecord.SendForApprovalTime,
                Status = CanibalizationRecord.Status,
                VoidBy = CanibalizationRecord.VoidBy,
                VoidTime = CanibalizationRecord.VoidTime,
                IsOnlineApproved = CanibalizationRecord.IsOnlineApproved,
                IsOnlineTransferred = CanibalizationRecord.IsOnlineTransferred,
                IsOnlineTransferredBy = CanibalizationRecord.IsOnlineTransferredBy,
                IsSecondOnlineApproved = CanibalizationRecord.IsSecondOnlineApproved,
                IsSecondSendForApproval = CanibalizationRecord.IsSecondSendForApproval,
            };

            return entity;
        }
        public ActionResult Canibalization_Update([DataSourceRequest]DataSourceRequest request,Canibalization canibalization)
        {
            canibalization.ModifiedBy = User.Identity.Name;
            canibalization.LastModified = DateTime.Now;
            if(ModelState.IsValid)
            {
                var entity = (Canibalization)this.GetMappedCanibalization(canibalization);
                db.Canibalizations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { canibalization }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Canibalization_Destroy([DataSourceRequest]DataSourceRequest request, Canibalization canibalization)
        {
            var FetchCanibalization = db.Canibalizations.FirstOrDefault(x => x.CanibalizationId == canibalization.CanibalizationId);
            if(FetchCanibalization==null)
            {
                ModelState.AddModelError("No record Found", "No record found");
            }
            db.Canibalizations.Remove(FetchCanibalization);
            db.SaveChanges();
            return Json(new[] { canibalization }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult ApproveJobRequest(int id)
        {
            var Request = db.Canibalizations.FirstOrDefault(x => x.CanibalizationId == id);
               if (Request != null/* && fleet != null*/)
            {
                Request.IsOnlineApproved = true;
                Request.OnlineApprovedBy = User.Identity.Name;
                Request.OnlineApprovedTime = DateTime.Now;
               // Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Request with No " + Request.Num + " Approved ", Title = " Approved Job Request ", URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                Request.Status = ApprovalStatuses.Approve.ToString();
                db.Canibalizations.Attach(Request);
                db.Entry(Request).State = EntityState.Modified;
                db.SaveChanges();
                var CanApproval = db.CanibalizationApprovals.FirstOrDefault(x => x.CanibalizationId == id);
                if(CanApproval!=null)
                {
                    CanApproval.CanibalizationId = Request.CanibalizationId;
                    CanApproval.Status = ApprovalStatuses.Approve;
                    CanApproval.CreatedBy = User.Identity.Name;
                    CanApproval.DateCreated = DateTime.Now;
                    db.Entry(CanApproval).State = EntityState.Modified;
                    db.SaveChanges();
                }

               
            }
            var formattedData = new
            {
                Message = "Approved Successefuly.",
                Status = "Approved"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Admin_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var Admin = new List<CanibalizationApproval>();         
            var Request = db.Canibalizations.Where(x => x.CanibalizationId.ToString() == Id).FirstOrDefault();
            var CheckApproval = db.CanibalizationApprovals.Where(x => x.CanibalizationId.ToString() == Id).ToList();
            if (CheckApproval.Count()==0&&Request!=null&&Request.IsSendForApproval && !Request.IsOnlineApproved)
            {
                var adminApproval = new CanibalizationApproval();
                    adminApproval.CanibalizationApprovalId = adminApproval.CanibalizationApprovalId;
                    adminApproval.CanibalizationId = int.Parse(Id);
                    adminApproval.Status = ApprovalStatuses.Pending;
                    adminApproval.Remark = string.Empty;
                    adminApproval.CreatedBy = User.Identity.Name;
                    adminApproval.DateCreated = DateTime.Now;
                    db.CanibalizationApprovals.Add(adminApproval);
                    db.SaveChanges();
            }
            Admin = db.CanibalizationApprovals.Where(x => x.CanibalizationId.ToString() == Id).ToList();            
           
            return Json(Admin.ToDataSourceResult(request));
        }

        public ActionResult RejectJobRequest(string id)

        {
            var request = new DataSourceRequest();
            // var jobRequest = db.JobRequest.Find(id);

            try
            {

                var Request = db.Canibalizations.Where(x => x.CanibalizationId.ToString() == id).FirstOrDefault();
                if (Request != null)
                {

                    Request.IsSendForApproval = false;
                    Request.IsOnlineApproved = false;
                    Request.Status = MaintenanceConst.Rejected;
                    db.Canibalizations.Attach(Request);
                    db.Entry(Request).State = EntityState.Modified;
                    db.SaveChanges();
                    var adminApproval = db.CanibalizationApprovals.FirstOrDefault(x=>x.CanibalizationId.ToString()==id);

                   
                    adminApproval.Status = ApprovalStatuses.Reject;
                    // adminApproval.Remark = model.Remark;
                    db.Entry(adminApproval).State = EntityState.Modified;                    
                    db.SaveChanges();

                }
            }

            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);

            }
            var formattedData = new
            {
                Message = "Rejected Successefuly.",
                Status = "Rejected"
            };
            return Json("Rejected Successefuly.", JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendForApproval(int id)
        {
            var record = db.Canibalizations.Find(id);
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var checkItems = db.CanibalizedParts.Where(sli => sli.CanibalizationId == id).ToList();
            if (!checkItems.Any())
            {
                var formattedData = new
                {
                    Message = "You have to Add Items under The Lot",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (checkItems.Any())
            {
                if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
                {

                    record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                    record.IsSendForApproval = true;
                    record.SendForApprovalBy = User.Identity.Name;
                    record.SendForApprovalTime = DateTime.Now;
                    db.Canibalizations.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "Successfully Sent.",
                        Status = "OK"
                    };
                    List<UserVM> list = new List<UserVM>();
                    list = UserInformationHelper.GetUsersInRole(FleetManagementRoles.ResourceRequestApprover);
                    foreach (var user in list)
                    {
                        NotifModels jobRequesterNotif = new NotifModels
                        {
                            UserId = user.UserId,
                            Title = "Canibalization Approval Request",
                            Message = $"There is canibalization request",
                            URL = "/Maintenance/WorkShop",

                        };
                        Notify.NotifyUser(jobRequesterNotif);
                    }
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }


            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;


        }
    }
}