using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.SharedDataSources;
using System.Drawing;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [DevLogHelper("Henok Solomon","5-28-18")]
    [AuthorizeRoles(FleetManagementRoles.ServiceUser)]
    public class StoreRequestionController : Controller
    {
        //
        // GET: /FleetManagement/ServiceCenter/
        public ActionResult Index()
        {
            ViewData["Mechanic"] = db.MechanicCrew.ToList();
            ViewData["InventoryItem"] = db.InventoryItems.ToList();

            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;

            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    ViewBag.Image = logo.BinaryFile;
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }

            
            }
            ViewBag.Name = Name;
            ViewBag.AmharicName = AmharicName;
            ViewBag.Address = Address;
            return View();
        }

        private IJobOrderDetailService JobOrderDetailService;
        IJobOrderService _JobOrderService;
        IFleetService _FleetService;
        IServiceService _ServiceService;
        private IAdministrativeApprovalService _AdministrativeApprovalService;
        ExceedDbContext db = new ExceedDbContext();

        public const string Finished = "Finished";
        public const string Pending = "Pending";
        public const string Active = "Active";
        public StoreRequestionController()
        {
            JobOrderDetailService = new JobOrderDetailService();
            _JobOrderService = new JobOrderService();
            _FleetService = new FleetService();
            _AdministrativeApprovalService = new AdministrativeJobApprovalService();
            _ServiceService = new ServiceService();
        }

        public string GetSection(Guid ServiceId)
        {
            var Service = _ServiceService.GetById(ServiceId);
            if (Service != null)
            {
                var Sections = db.WorkUnitSection.Where(x => x.WorkUnitSectionId == Service.SectionId).FirstOrDefault();
                if (Sections != null)
                {
                    return Sections.Name;
                }

            }
            return string.Empty;
        }
        public ActionResult EquipmentServiceHistory_Read([DataSourceRequest] DataSourceRequest request, Guid Id, Guid JId)
        {
            var ServiceList = new List<JobOrderDetail>();
            var JobOrder = _JobOrderService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(
                    x =>
                        x.IsOnlineApproved == true && x.FleetsID == Id).ToList();
            foreach (var job in JobOrder)
            {
                var Service = JobOrderDetailService.GetAll().Where(x => x.JobOrderID == job.JobOrderID && x.JobOrderID != JId).ToList();
                ServiceList.AddRange(Service.Select(s => new JobOrderDetail
                    {
                        ServiceName = s.ServiceName,
                        Remark = s.Remark,
                        DateCreated = s.DateCreated,
                        Section = GetSection(s.ServiceID)
                    }));
            }


            return Json(ServiceList.ToDataSourceResult(request));
        }



        public ActionResult ServiceCenter_Read([DataSourceRequest] DataSourceRequest request)
        {
            var JobOrder = _JobOrderService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(
                    x =>
                        x.IsOnlineApproved == true && x.IsExternal == false)
                .Select(j => new JobOrder
                {
                    ProjectName = j.ProjectName,
                    Progress = GetProgressPercent(j.JobOrderID),
                    Status = j.Status,
                    Num = j.Num,
                    EquipmentName = j.EquipmentName,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    Type = j.Type,
                    FleetsID = j.FleetsID,
                    LabourCost = GetLabourCost(j.JobOrderID),
                    SpareCost = GetSpareCost(j.JobOrderID),
                    InspectorComments = GetMaintenaceDuration(j.JobOrderID),

                    IsOnlineApproved = j.IsOnlineApproved,
                    IsSendForApproval = j.IsSendForApproval,
                    IsSecondSendForApproval = j.IsSecondSendForApproval,
                    IsOnlineTransferred = j.IsOnlineTransferred,


                    IsSecondOnlineApproved = j.IsSecondOnlineApproved




                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }


        public ActionResult JobOrder_Refer([DataSourceRequest] DataSourceRequest request, JobOrder JobOrder)
        {



            _JobOrderService = new JobOrderService();

            JobOrder jOrder = _JobOrderService.GetById(JobOrder.JobOrderID);
            jOrder.IsExternal = true;
            jOrder.ModifiedBy = User.Identity.Name;
            jOrder.LastModified = DateTime.Now;
            _JobOrderService.Update(jOrder);


            return Json(new[] { jOrder }.ToDataSourceResult(request));



        }

        public ActionResult TechnicalApproval_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var Admin = new List<AdministrativeJobApproval>();
            var Request = db.JobOrder.Where(x => x.JobOrderID.ToString() == Id).FirstOrDefault();
            if (Request != null)
            {
                if (Request.IsOnlineApproved)
                {
                    Admin =
                        _AdministrativeApprovalService.GetAll()
                            .Where(x => x.AdministrativeJobId.ToString() == Id)
                            .ToList();


                }

                if (Request.IsSendForApproval && !Request.IsOnlineApproved)
                {



                    var adminApproval = new AdministrativeJobApproval();
                    adminApproval.AdministrativeJobApprovalID = 0;
                    adminApproval.AdministrativeJobId = Guid.NewGuid();
                    adminApproval.Status = ApprovalStatuses.Pending;
                    adminApproval.Remark = string.Empty;
                    adminApproval.CreatedBy = User.Identity.Name;
                    adminApproval.DateCreated = DateTime.Now;

                    Admin.Add(adminApproval);

                }
            }
            return Json(Admin.ToDataSourceResult(request));
        }

     


        public ActionResult SendForApproval(string Id)
        {

            var JobOrder = _JobOrderService.GetById(Guid.Parse(Id));
            JobOrder.Status = MaintenanceConst.InspectionDone;
            JobOrder.SecondSendForApprovalBy = User.Identity.Name;
            JobOrder.IsSecondSendForApproval = true;
            JobOrder.SecondSendForApprovalTime = DateTime.Now;
            if (JobOrder != null)
            {

                _JobOrderService.Update(JobOrder);

            }

            return RedirectToAction("Index");
        }
        public ActionResult TransferJob(Guid Id)
        {

            var JobOrder = _JobOrderService.GetById(Id);
            JobOrder.Status = MaintenanceConst.SentForClosing;
            JobOrder.IsOnlineTransferred = true;
            JobOrder.IsOnlineTransferredBy = User.Identity.Name;
            JobOrder.OnlineTransferredTime = DateTime.Now;



            _JobOrderService.Update(JobOrder);

            return RedirectToActionPermanent("Index", "ServiceCenter", new { area = "FleetManagement" });

        }


        public ActionResult InventoryStoreSelector(string Id)
        {
            ViewBag.Id = Id;

            return View();
        }
        public string GetJobStatus(string Id)
        {

            JobOrder JobRequests = db.JobOrder.Where(j => j.JobOrderID.ToString() == Id && (j.IsOnlineTransferred == true || j.IsSecondOnlineApproved == true)).FirstOrDefault();
            if (JobRequests != null)
            {
                return JobRequests.Status;
            }

            return "Pending";


        }
        public ActionResult RequistionResult()
        {
            return View();
        }

        public ActionResult GenrateStoreRequistion(SelectStoreViewModel model)
        {
            try
            {

                Guid Id = Guid.Parse(model.Remark);
                int BranchId = 1;
           
               
                    var Job = _JobOrderService.GetById(Id);
                    var Branch = db.Branch.Where(x => x.Name == Job.ProjectName).FirstOrDefault();
                    var Store = db.InventoryStores.Where(x => x.StoreCode == model.Name).FirstOrDefault();
                    var Period = db.GLPeriods.Where(x => x.DateFrom < DateTime.Now && x.DateTo > DateTime.Now)
                    .FirstOrDefault();

                    if (Branch != null)
                    {
                        BranchId = Branch.BranchId;
                    }
                    List<StoreRequisitionItem> StoreRequisition = new List<StoreRequisitionItem>();
                    var Requestions = db.ServiceSpare.Where(x => x.ServiceID == Id).ToList();



                    foreach (var Requestion in Requestions)
                    {
                        var Requestionlogs = db.JobStoreRequisition.Where(x => x.JobId == Requestion.ServiceSpareID).ToList();
                        int Count = 0;
                        int RequestionNo = 0;
                        if (Requestionlogs.Count == 0 || Requestionlogs == null)
                        {
                            if (Count == 0)
                            {
                                StoreRequisition Request = new StoreRequisition
                                {
                                    RequestDate = DateTime.Now,
                                    StoreCode = model.Name,
                                    ToStoreCode = string.Empty,
                                    BranchId = BranchId,
                                    OrgStructureId = 1,
                                    IsFuel = false,
                                    IsTransfer = false,
                                    GLPeriodId = Period.GLPeriodId,
                                    IsSecondOnlineApproved = false,
                                    IsSecondSendForApproval = false,
                                    IsVoid = false,
                                    IsOnlineApproved = false,
                                    IsOnlineTransferred = false,
                                    IsSendForApproval = false

                                };

                                db.StoreRequisitions.Add(Request);
                                db.SaveChanges();

                                RequestionNo = Request.StoreRequisitionID;
                                Count++;
                            }
                            var InventoryItem = db.InventoryItems.Where(x => x.ItemCode == Requestion.InventoryItemId)
                                .FirstOrDefault();
                            var SpareRequest = new StoreRequisitionItem
                            {

                                RequestedQuantity = Requestion.Quantity,
                                PartNo = InventoryItem.PartNo,
                                ItemSpecification = InventoryItem.Name,
                                ItemCategoryCode = InventoryItem.ItemCategoryCode,
                                ItemCode = InventoryItem.ItemCode,
                                ApprovedQuantity = 0,
                                IssuedQuantity = 0,
                                PurchaseQuantity = 0,
                                IsApproved = false,
                                GeneratePurchaseReq = true,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                StoreRequisitionID = RequestionNo,
                            };
                            db.StoreRequisitionItems.Add(SpareRequest);
                            db.SaveChanges();

                            var JobStoreRequistion = new JobStoreRequisition
                            {
                                JobStoreRequisitionId = Guid.NewGuid(),
                                JobId = Id,
                                RequisitionId = SpareRequest.StoreRequisitionItemID,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now,
                                Type = Enum.GetName(Requestion.Type.GetType(), Requestion.Type)
                            };
                            db.JobStoreRequisition.Add(JobStoreRequistion);
                            db.SaveChanges();

                        }
                    }
                }
          catch (Exception e)
            {

            }

            return RedirectToAction("RequistionResult", "StoreRequestion");
        }

        public ActionResult GetServiceRequisition([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            List<RequistionItemViewModel> StoreRequisitions = new List<RequistionItemViewModel>();


            var Spares = db.ServiceSpare.Where(x => x.ServiceID == Id && x.IsOnlineApproved == true).ToList();

            foreach (var Spare in Spares)
            {

                var Requestions = db.JobStoreRequisition.Where(x => x.JobId == Spare.ServiceSpareID).ToList();

                foreach (var Requestion in Requestions)
                {
                    string StoreName = string.Empty;

                    var RequesitionItem = db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == Requestion.RequisitionId).FirstOrDefault();

                    if (RequesitionItem != null)
                    {

                        var Store = db.InventoryStores.Where(x => x.StoreCode == RequesitionItem.StoreRequisition.StoreCode).FirstOrDefault();

                        if (Store != null)
                        {
                            StoreName = Store.Name;
                        }
                        var Item = new RequistionItemViewModel
                           {
                                IssuedQuantity = RequesitionItem.IssuedQuantity,
                               ItemCode = RequesitionItem.InventoryItem.Name,
                               DateCreated = RequesitionItem.DateCreated,
                               ItemCategoryCode = StoreName,
                               RemainingAmount = Spare.Quantity - RequesitionItem.IssuedQuantity,
                               UsedAmount = Spare.UsedQuantity,
                               UnitPrice = RequesitionItem.InventoryItem.AvgPrice,
                               TotalPrice = RequesitionItem.InventoryItem.AvgPrice * RequesitionItem.IssuedQuantity,
                               Remark = "Pending"

                           };
                        StoreRequisitions.Add(Item);
                    }
                }
            }
            return Json(StoreRequisitions.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        #region Support Method
        public string GetProgressPercent(Guid Id)
        {
            try
            {

                decimal RequestOrderLine = 0;
                decimal RequestOrderLineFinished = 0;
                var Order = _JobOrderService.GetById(Id);
                var Requests = db.JobRequest.Where(x => x.Num == Order.Referance).FirstOrDefault();
                if (Requests != null)
                {
                    RequestOrderLine = JobOrderDetailService.GetAll().Where(x => x.JobOrderID == Requests.JobRequestId).ToList().Count;
                    RequestOrderLineFinished = JobOrderDetailService.GetAll().Where(x => x.JobOrderID == Requests.JobRequestId && x.Status == MaintenanceConst.Completed).ToList().Count;
                }
                decimal OrderLine = JobOrderDetailService.GetAll().Where(x => x.JobOrderID == Id).ToList().Count;
                decimal OrderLineFinished = JobOrderDetailService.GetAll().Where(x => x.JobOrderID == Id && x.Status == MaintenanceConst.Completed).ToList().Count;
                var progress = (OrderLineFinished + RequestOrderLineFinished) / (OrderLine + RequestOrderLine);

                progress = progress * 100;
                return ((int)progress).ToString();

            }
            catch (Exception ee)
            {

                return "0";
            }
        }
        public string GetMaintenaceDuration(Guid Id)
        {
            try
            {
                var OrderLine = _JobOrderService.GetAll().Where(x => x.JobOrderID == Id && x.IsOnlineApproved == true).FirstOrDefault();

                if (!OrderLine.IsOnlineTransferred)
                {
                    TimeSpan Duration = DateTime.Now - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                    return Duration.Days + " Days and " + Duration.Hours + " Hours";
                }

                TimeSpan FinishedDuration = DateTime.Parse(OrderLine.OnlineTransferredTime.ToString()) - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                return FinishedDuration.Days + " Days and " + FinishedDuration.Hours + " Hours";
            }
            catch (Exception ee)
            {

                return "0";
            }

        }
        public double GetMaintenaceDurationNum(Guid Id)
        {
            try
            {


                var OrderLine = _JobOrderService.GetAll().Where(x => x.JobOrderID == Id).FirstOrDefault();

                if (!OrderLine.IsOnlineTransferred)
                {
                    TimeSpan Duration = DateTime.Now - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                    return Duration.TotalHours;
                }

                TimeSpan FinishedDuration = DateTime.Parse(OrderLine.OnlineTransferredTime.ToString()) - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                return FinishedDuration.TotalHours;
            }
            catch (Exception ee)
            {

                return 0;
            }
        }
        public decimal GetLabourCost(Guid Id)
        {
            ExceedDbContext db = new ExceedDbContext();
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            try
            {
                decimal Cost = 0;

                var JobOrder = db.JobOrder.Where(x => x.JobOrderID == Id).FirstOrDefault();

                var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();

                var Services = db.JobOrderDetail.Where(x => x.JobOrderID == JobOrder.JobOrderID || x.JobOrderID == Request.JobRequestId).ToList();
                foreach (var Service in Services)
                {

                    var Crews = _ServiceCrewService.GetAll().Where(x => x.ServiceID == Service.ServiceID).ToList();



                    foreach (var Crew in Crews)
                    {

                        var WorkLogs = db.ServiceCrewWorkLogs.Where(x => x.MecanicId == Crew.MechanicCrewID && x.IsOnlineApproved == true).ToList();

                        foreach (var WorkLog in WorkLogs)
                        {
                            var Duration = ((DateTime.Parse(WorkLog.WorkingHoursMorningto.ToString()) - DateTime.Parse(WorkLog.WorkingHoursMorningFrom.ToString())).TotalHours) + ((DateTime.Parse(WorkLog.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(WorkLog.WorkingHoursAfternoonFrom.ToString())).TotalHours);
                            Cost = Cost + (decimal.Parse(Duration.ToString()) * Crew.MechanicCrews.HourlyWage);

                        }
                    }
                }

                return Cost;
            }

            catch (Exception ee)
            {

                return 0;
            }
        }
        public decimal GetEfficiency(Guid Id)
        {
            ExceedDbContext db = new ExceedDbContext();
            int duration = 1;
            decimal Rate = 0;
            decimal Efficiency = 0;
            Guid RequestId = Guid.Empty;

            try
            {


                var OrderLine = _JobOrderService.GetAll().Where(x => x.JobOrderID == Id).FirstOrDefault();
                if (!OrderLine.IsOnlineTransferred)
                {
                    TimeSpan Duration = DateTime.Now - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                    duration = Duration.Minutes;
                }
                if (OrderLine.IsOnlineTransferred)
                {
                    TimeSpan Duration = DateTime.Parse(OrderLine.LastModified.ToString()) - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                    duration = Duration.Minutes;
                }



                var Request = db.JobRequest.Where(x => x.Num == OrderLine.Referance).FirstOrDefault();
                if (Request != null)
                {
                    RequestId = Request.JobRequestId;
                }
                var services = db.JobOrderDetail.Where(x => x.JobOrderID == Id || x.JobOrderID == RequestId).ToList();


                foreach (var service in services)
                {
                    var Std = db.Service.Where(x => x.ServiceName == service.ServiceName).FirstOrDefault();
                    if (Std != null)
                    {
                        Rate = Rate + decimal.Parse(Std.STDTime.ToString());
                    }

                }
                Efficiency = (Rate / duration) * 100;

                return Efficiency;
            }
            catch (Exception ee)
            {

                return 0;
            }
        }
        public decimal GetSpareCost(Guid Id)
        {
            decimal cost = 0;
            Guid RequestId = Guid.Empty;
            try
            {
                var JobOrder = _JobOrderService.GetById(Id);
                if (!JobOrder.IsExternal)
                {

                    var Spares = db.ServiceSpare
                        .Where(x => x.ServiceID == JobOrder.JobOrderID && x.Type == SpareType.Spare).ToList();
                    foreach (var Spare in Spares)
                    {

                        var InventoryItem =
                            db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                        var RequestionRef =
                                 db.JobStoreRequisition.Where(x => x.JobId == JobOrder.JobOrderID).FirstOrDefault();
                        var Requestion =
                            db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId)
                                .FirstOrDefault();

                        if (Spare.UsedQuantity <= Requestion.IssuedQuantity)
                        {
                            cost = cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                        }
                    }

                }
                else
                {
                    var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();
                    if (Request != null)
                    {
                        RequestId = Request.JobRequestId;
                    }
                    var ExternalServices = db.JobOrderDetail.Where(x => x.JobOrderID == Id || x.JobOrderID == RequestId).ToList();

                    foreach (var Service in ExternalServices)
                    {
                        var SparesInternal = db.ServiceSpare.Where(x => x.ServiceID == Service.JobOrderDetailID && x.Type == SpareType.Oil_Lubricant).ToList();
                        foreach (var Spare in SparesInternal)
                        {

                            var InventoryItem =
                                db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                            var RequestionRef =
                                     db.JobStoreRequisition.Where(x => x.JobId == JobOrder.JobOrderID).FirstOrDefault();
                            var Requestion =
                                db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId)
                                    .FirstOrDefault();

                            if (Spare.UsedQuantity <= Requestion.IssuedQuantity)
                            {
                                cost = cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                            }
                        }
                        var Spares = db.ExternalMaterial
                            .Where(x => x.JobId == Service.JobOrderDetailID && x.Type == SpareType.Spare).ToList();
                        cost = cost + Spares.Sum(x => x.TotalAmount);

                    }
                }

                return cost;
            }
            catch (Exception ee)
            {

                return 0;
            }
        }


        public decimal GetBatterySpareCost(Guid Id)
        {
            decimal cost = 0;
            Guid RequestId = Guid.Empty;
            try
            {
                var JobOrder = _JobOrderService.GetById(Id);
                if (!JobOrder.IsExternal)
                {
                    var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();
                    if (Request != null)
                    {
                        RequestId = Request.JobRequestId;
                    }

                    var Spares = db.ServiceSpare.Where(x => (x.ServiceID == Id || x.ServiceID == RequestId) && x.Type == SpareType.Battery).ToList();
                    foreach (var Spare in Spares)
                    {

                        var InventoryItem =
                            db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                        var RequestionRef =
                                 db.JobStoreRequisition.Where(x => x.JobId == JobOrder.JobOrderID).FirstOrDefault();
                        var Requestion =
                            db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId)
                                .FirstOrDefault();

                        if (Spare.UsedQuantity <= Requestion.IssuedQuantity)
                        {
                            cost = cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                        }
                    }

                }

                else
                {
                    var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();
                    if (Request != null)
                    {
                        RequestId = Request.JobRequestId;
                    }
                    var ExternalServices = db.JobOrderDetail.Where(x => x.JobOrderID == Id || x.JobOrderID == RequestId).ToList();

                    foreach (var Service in ExternalServices)
                    {

                        var SparesInternal = db.ServiceSpare.Where(x => x.ServiceID == Service.JobOrderDetailID && x.Type == SpareType.Battery).ToList();
                        foreach (var Spare in SparesInternal)
                        {

                            var InventoryItem =
                                db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                            var RequestionRef =
                                     db.JobStoreRequisition.Where(x => x.JobId == JobOrder.JobOrderID).FirstOrDefault();
                            var Requestion =
                                db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId)
                                    .FirstOrDefault();

                            if (Spare.UsedQuantity <= Requestion.IssuedQuantity)
                            {
                                cost = cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                            }
                        }

                        var Spares = db.ExternalMaterial
                            .Where(x => x.JobId == Service.JobOrderDetailID && x.Type == SpareType.Battery).ToList();
                        cost = cost + Spares.Sum(x => x.TotalAmount);

                    }
                }
                return cost;
            }
            catch (Exception ee)
            {

                return 0;
            }
        }


        public decimal GetOilSpareCost(Guid Id)
        {
            decimal cost = 0;
            Guid RequestId = Guid.Empty;
            try
            {
                var JobOrder = _JobOrderService.GetById(Id);
                if (!JobOrder.IsExternal)
                {

                    var Spares = db.ServiceSpare.Where(x => x.ServiceID == JobOrder.JobOrderID && x.Type == SpareType.Oil_Lubricant).ToList();
                    foreach (var Spare in Spares)
                    {

                        var InventoryItem =
                            db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                        var RequestionRef =
                                 db.JobStoreRequisition.Where(x => x.JobId == JobOrder.JobOrderID).FirstOrDefault();
                        var Requestion =
                            db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId)
                                .FirstOrDefault();

                        if (Spare.UsedQuantity <= Requestion.IssuedQuantity)
                        {
                            cost = cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                        }
                    }

                }

                else
                {
                    var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();
                    if (Request != null)
                    {
                        RequestId = Request.JobRequestId;
                    }
                    var ExternalServices = db.JobOrderDetail.Where(x => x.JobOrderID == Id || x.JobOrderID == RequestId).ToList();

                    foreach (var Service in ExternalServices)
                    {

                        var SparesInternal = db.ServiceSpare.Where(x => x.ServiceID == Service.JobOrderDetailID && x.Type == SpareType.Oil_Lubricant).ToList();
                        foreach (var Spare in SparesInternal)
                        {

                            var InventoryItem =
                                db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                            var RequestionRef =
                                     db.JobStoreRequisition.Where(x => x.JobId == JobOrder.JobOrderID).FirstOrDefault();
                            var Requestion =
                                db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId)
                                    .FirstOrDefault();

                            if (Spare.UsedQuantity <= Requestion.IssuedQuantity)
                            {
                                cost = cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                            }
                        }

                        var Spares = db.ExternalMaterial
                            .Where(x => x.JobId == Service.JobOrderDetailID && x.Type == SpareType.Oil_Lubricant).ToList();
                        cost = cost + Spares.Sum(x => x.TotalAmount);

                    }
                }
                return cost;
            }
            catch (Exception ee)
            {

                return 0;
            }
        }


        public decimal GetTyreSpareCost(Guid Id)
        {
            decimal cost = 0;
            Guid RequestId = Guid.Empty;
            try
            {
                var JobOrder = _JobOrderService.GetById(Id);
                if (!JobOrder.IsExternal)
                {

                    var Spares = db.ServiceSpare.Where(x => x.ServiceID == JobOrder.JobOrderID && x.Type == SpareType.Tyre).ToList();
                    foreach (var Spare in Spares)
                    {

                        var InventoryItem =
                            db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                        var RequestionRef =
                                 db.JobStoreRequisition.Where(x => x.JobId == JobOrder.JobOrderID).FirstOrDefault();
                        var Requestion =
                            db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId)
                                .FirstOrDefault();
                        if (Spare.UsedQuantity <= Requestion.IssuedQuantity)
                        {
                            cost = cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                        }
                    }

                }
                else
                {
                    var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();
                    if (Request != null)
                    {
                        RequestId = Request.JobRequestId;
                    }
                    var ExternalServices = db.JobOrderDetail.Where(x => x.JobOrderID == Id || x.JobOrderID == RequestId).ToList();

                    foreach (var Service in ExternalServices)
                    {
                        var SparesInternal = db.ServiceSpare.Where(x => x.ServiceID == Service.JobOrderDetailID && x.Type == SpareType.Oil_Lubricant).ToList();
                        foreach (var Spare in SparesInternal)
                        {

                            var InventoryItem =
                                db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                            var RequestionRef =
                                     db.JobStoreRequisition.Where(x => x.JobId == JobOrder.JobOrderID).FirstOrDefault();
                            var Requestion =
                                db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId)
                                    .FirstOrDefault();

                            if (Spare.UsedQuantity <= Requestion.IssuedQuantity)
                            {
                                cost = cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                            }
                        }
                        var Spares = db.ExternalMaterial
                            .Where(x => x.JobId == Service.JobOrderDetailID && x.Type == SpareType.Tyre).ToList();
                        cost = cost + Spares.Sum(x => x.TotalAmount);

                    }
                }
                return cost;
            }
            catch (Exception ee)
            {

                return 0;
            }
        }


        public decimal GetOtherSpareCost(Guid Id)
        {
            decimal cost = 0;
            Guid RequestId = Guid.Empty;
            try
            {
                var JobOrder = _JobOrderService.GetById(Id);
                if (!JobOrder.IsExternal)
                {

                    var Spares = db.ServiceSpare.Where(x => x.ServiceID == JobOrder.JobOrderID && x.Type == SpareType.Other).ToList();
                    foreach (var Spare in Spares)
                    {

                        var InventoryItem =
                            db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                        var RequestionRef =
                                 db.JobStoreRequisition.Where(x => x.JobId == JobOrder.JobOrderID).FirstOrDefault();
                        var Requestion =
                            db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId)
                                .FirstOrDefault();

                        if (Spare.UsedQuantity <= Requestion.IssuedQuantity)
                        {
                            cost = cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                        }
                    }

                }
                else
                {
                    var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();
                    if (Request != null)
                    {
                        RequestId = Request.JobRequestId;
                    }
                    var ExternalServices = db.JobOrderDetail.Where(x => x.JobOrderID == Id || x.JobOrderID == RequestId).ToList();

                    foreach (var Service in ExternalServices)
                    {
                        var SparesInternal = db.ServiceSpare.Where(x => x.ServiceID == Service.JobOrderDetailID && x.Type == SpareType.Oil_Lubricant).ToList();
                        foreach (var Spare in SparesInternal)
                        {

                            var InventoryItem =
                                db.InventoryItems.Where(x => x.ItemCode == Spare.InventoryItemId).FirstOrDefault();
                            var RequestionRef =
                                     db.JobStoreRequisition.Where(x => x.JobId == JobOrder.JobOrderID).FirstOrDefault();
                            var Requestion =
                                db.StoreRequisitionItems.Where(x => x.StoreRequisitionItemID == RequestionRef.RequisitionId)
                                    .FirstOrDefault();

                            if (Spare.UsedQuantity <= Requestion.IssuedQuantity)
                            {
                                cost = cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                            }
                        }
                        var Spares = db.ExternalMaterial
                            .Where(x => x.JobId == Service.JobOrderDetailID && x.Type == SpareType.Other).ToList();
                        cost = cost + Spares.Sum(x => x.TotalAmount);

                    }
                }
                return cost;
            }
            catch (Exception ee)
            {

                return 0;
            }
        }


        public decimal GetSalvageSpareCost(Guid Id)
        {
            decimal cost = 0;
            Guid RequestId = Guid.Empty;
            try
            {
                var JobOrder = _JobOrderService.GetById(Id);

                var Spares = db.SalvageStore.Where(x => x.JobId == JobOrder.JobOrderID).ToList();
                if (Spares != null)
                {
                    cost = cost + (Spares.Sum(x => x.Price) * Spares.Sum(x => x.ApprovedQuantity));
                }


                return cost;
            }
            catch (Exception ee)
            {

                return 0;
            }
        }

        public decimal GetComponentCost(Guid Id)
        {
            decimal UnitPrice = 0;
            decimal Quantity = 0;
            try
            {

                var components = db.ExternalComponent.Where(x => x.JobId == Id).ToList();
                foreach (var component in components)
                {

                    var TotalUnitPrice = db.ExternalMaterial.Where(x => x.JobId == component.ExternalComponentId).Sum(x => x.UnitPrice);
                    var TotalUnit = db.ExternalMaterial.Where(x => x.JobId == component.ExternalComponentId).Sum(x => x.Quantity);

                    UnitPrice = UnitPrice + TotalUnitPrice;
                    Quantity = Quantity + TotalUnit;

                }
            }
            catch (Exception e)
            {
                return 0;
            }
            return Quantity * UnitPrice;
        }

        #endregion
  


    }
}