using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Web.Areas.Maintenance.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
   // [AuthorizeRoles(MaintenanceRoles.ServiceUser)]
    public class ServiceCenterController : Controller
    {
        public async Task<ActionResult> Index()
        {
            ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new { e.EquipmentInformationId, e.LicensePlate, e.DeviceNumber }).ToList();
            await GetMaintainaceDue();
            ViewData["SubCategories"] = db.ItemCategorys.Select(s => new {
                Code = s.ItemCategoryCode,
                Name = s.Name
            }).ToList();
            var employee = db.Person_Employee;
            var mechanic = db.MechanicCrew;
            ViewData["Mechanic"] = mechanic.Select(s => new {
                Code = s.MechanicCrewID,
                Name = s.Person_Employee.EmployeeCode + " "+s.Person_Employee.FirstNameEnglish + " " + s.Person_Employee.MiddleNameEnglish + " " + s.Person_Employee.LastNameEnglish
            }).AsQueryable();
            ViewData["JobOrderDetail"] = db.JobOrderDetail.Select(s => new {
                Code = s.JobOrderDetailID,
                Name = s.RepairType.Code + "-" + s.RepairType.Name

            }).AsQueryable();
            ViewBag.AdvanceMeasure = db.AdvanceMeasures.ToList();
            var inventoryPeriods = db.InventoryPeriods;
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var costCenter = db.BranchCostCenter;
            var branches = db.Branch;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;
            ViewData["JobCauses"] = db.JobCauses.Select(s => new {
                Code = s.JobCausesId,
                Name = s.Description
            }).ToList();

            ViewData["Branches"] = branches;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
            var store = db.InventoryStores.AsQueryable();
            ViewData["stores"] = store;
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();

            ViewData["MaintenanceCenter"] = db.MaintenanceCenter.Select(s => new
            {
                Id = s.MaintenanceCenterID,
                Name = s.Name
            }).ToList();


            ViewData["ServiceCenter"] = db.ServiceCenters.Select(s => new
            {
                Name = s.Name,
                Id = s.ServiceCenterId
            }).ToList();

            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.EmployeeCode + " " + i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;
            ViewData["Service"] = db.RepairType.Select(s => new {
                Code = s.RepairTypeID,
                Name = s.Code + "-" + s.Name
            }).ToList();
            return View();
        }

        private IJobOrderDetailService JobOrderDetailService;
        private IJobOrderService _JobOrderService;
        private IServiceCenterJobService _ServiceCenterJobService;
        private readonly IFleetService _FleetService;
        private IServiceService _ServiceService;
        private IAdministrativeApprovalService _AdministrativeApprovalService;
        private ExceedDbContext db = new ExceedDbContext();

        public const string Finished = "Finished";
        public const string Pending = "Pending";
        public const string Active = "Active";
        public ServiceCenterController()
        {
            JobOrderDetailService = new JobOrderDetailService();
            _JobOrderService = new JobOrderService();
            _ServiceCenterJobService=new ServiceCenterJobService();
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
        public ActionResult EquipmentServiceHistory_Read([DataSourceRequest] DataSourceRequest request, string plateNumber, Guid JId)
        {
            var ServiceList = new List<JobOrderDetail>();
            var JobOrder = _JobOrderService.GetAll()
                .Where(
                    x =>
                        x.IsOnlineApproved == true && x.LicensePlate == plateNumber).ToList();
            foreach (var job in JobOrder)
            {
                var Service = JobOrderDetailService.GetAll().Where(x => x.JobOrderID == job.JobOrderID && x.JobOrderID != JId).ToList();
                ServiceList.AddRange(Service.Select(s => new JobOrderDetail
                {
                    RepairTypeID = s.RepairTypeID,
                    Remark = s.Remark,
                    DateCreated = s.DateCreated,
                    MaintenanceCenter = job.MaintenanceCenter
                    //MaintenanceCenter = GetSection(s.ServiceID)
                }));
            }


            return Json(ServiceList.ToDataSourceResult(request));
        }



        public ActionResult ServiceCenter_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<ServiceCenterJob> jobOrders = db.ServiceCenterJob.Where(x => !x.IsExternalMaintenanceCenter);
            DataSourceResult result = jobOrders.ToDataSourceResult(request, j => new ServiceCenterJob
            {
                ProjectName = j.ProjectName,
                ServiceCenterJobId = j.ServiceCenterJobId,
                Progress = GetProgressPercent(j.JobOrderID),
                CustomerId = db.JobRequest.Where(q => q.Num == j.Referance).FirstOrDefault()?.CustomerId,
                Status = j.Status,
                Num = j.JobOrderNum ?? j.Num,
                EquipmentName = j.EquipmentName,
                RejectionReason = j.RejectionReason,
                MaintenanceCenter = j.MaintenanceCenter,
                MaintenanceCenterId = j.MaintenanceCenterId,
                ServiceCenterId = j.ServiceCenterId,
                JobOrderNum = j.JobOrderNum,
                Referance = j.Referance,
                JobOrderID = j.JobOrderID,
                Type = j.Type,
                LicensePlate = j.LicensePlate,
                //VoidBy = j.Fleets.EquipmentIdentityID.ToString(),
                EquipmentInformationId = j.EquipmentInformationId,
                FailureReason = j.FailureReason,
                AdvanceMeasure = j.AdvanceMeasure,
                InspectorComments = GetMaintenaceDuration(j.JobOrderID),

                IsOnlineApproved = j.IsOnlineApproved,
                IsSendForApproval = j.IsSendForApproval,
                IsSecondSendForApproval = j.IsSecondSendForApproval,
                IsOnlineTransferred = j.IsOnlineTransferred,
                IsSentToWorkShop = j.IsSentToWorkShop,
                IsSubmittedToServiceCenter = j.IsSubmittedToServiceCenter,
                OnlineAssignedTime = j.OnlineAssignedTime,
                OnlineSubmittedTime = j.OnlineSubmittedTime,
                WorkShopRejectedReason = j.WorkShopRejectedReason,
                IsSecondOnlineApproved = j.IsSecondOnlineApproved,
                IsInsured = j.IsInsured
            });

            return Json(result);


        }


        public ActionResult ServiceCenterExtra_Read([DataSourceRequest] DataSourceRequest request)
        {
            FleetCostHelper fleetCostHelper = new FleetCostHelper();
            var inventoryMapping = db.InventorySparePartMapping.ToList();
            var JobOrder = _JobOrderService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Where(x => x.IsOnlineApproved == true && x.IsExternal == false)
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
                    // VoidBy = j.Fleets.EquipmentIdentityID.ToString(),
                    EquipmentInformationId = j.EquipmentInformationId,
                    FailureReason = j.FailureReason,
                    AdvanceMeasure = j.AdvanceMeasure,
                    LabourCost = fleetCostHelper.GetLabourCost(j),
                    //SpareCost = fleetCostHelper.GetJobOrderSpareCost(j),
                    //TyreCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Tyre),
                    //OilLubricantCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Oil_Lubricant),
                    //OtherCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Other),
                    //SalvageCost = fleetCostHelper.GetSalvageSpareCost(j.JobOrderID),
                    //BattreyCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Battery),
                    Efficiency = fleetCostHelper.GetEfficiency(j.JobOrderID),
                    ComponentCost = fleetCostHelper.GetComponentCost(j.JobOrderID),
                    InspectorComments = fleetCostHelper.GetMaintenaceDuration(j.JobOrderID),

                    IsOnlineApproved = j.IsOnlineApproved,
                    IsSendForApproval = j.IsSendForApproval,
                    IsSecondSendForApproval = j.IsSecondSendForApproval,
                    IsOnlineTransferred = j.IsOnlineTransferred,


                    IsSecondOnlineApproved = j.IsSecondOnlineApproved




                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }


        public ActionResult JobOrder_Refer([DataSourceRequest] DataSourceRequest request, ServiceCenterJob JobOrder)
        {



            _ServiceCenterJobService = new ServiceCenterJobService();

            ServiceCenterJob jOrder = _ServiceCenterJobService.GetById(JobOrder.JobOrderID);
            jOrder.IsExternal = true;
            jOrder.ModifiedBy = User.Identity.Name;
            jOrder.LastModified = DateTime.Now;
            _ServiceCenterJobService.Update(jOrder);


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

            var JobOrder = _ServiceCenterJobService.GetById(Guid.Parse(Id));
            JobOrder.Status = MaintenanceConst.InspectionDone;
            JobOrder.SecondSendForApprovalBy = User.Identity.Name;
            JobOrder.IsSecondSendForApproval = true;
            JobOrder.SecondSendForApprovalTime = DateTime.Now;
            if (JobOrder != null)
            {

                _ServiceCenterJobService.Update(JobOrder);

            }

            return RedirectToAction("Index");
        }
        public JsonResult TransferToExternal(Guid Id)
        {
            var jsnMssg = new
            {
                Message = "successfully transfered", 
                Error = false
            };
            var serviceCenter = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId == Id);
            if(serviceCenter != null)
            {
                serviceCenter.IsExternalMaintenanceCenter = true;
                db.Entry(serviceCenter).State = EntityState.Modified;
                db.SaveChanges();
               
                return Json(jsnMssg, JsonRequestBehavior.AllowGet);
            }
            var jMssg = new
            {
                Message = "Something's wrong",
                Error = true
            };
            return Json(jMssg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TransferJob(Guid Id)
        {
            var erviceJob = _ServiceCenterJobService.GetById(Id);
            if (erviceJob != null)
            {
                // check if there are any job-services that are not completed or that are completed
                // but not approved.
                var storeRequis = db.StoreRequisitions.Where(q => q.ServiceJobOrderNo == erviceJob.ServiceCenterJobId.ToString()).FirstOrDefault();
                var unCompletedJobs = db.JobOrderServiceCenterDetails.Where(jd => jd.ServiceCenterJobId == erviceJob.ServiceCenterJobId && jd.ApprovalStatus == ApprovalStatuses.Approve).Count();
                var unApprovedWorkSheet = db.ServiceCrewWorkLogs.Where(s => s.JobId == erviceJob.ServiceCenterJobId && s.Status == "Approved").Count();
                if (unCompletedJobs == 0 || unApprovedWorkSheet == 0)
                {
                    var dt = new
                    {
                        Message = "There are Pendding Job Services or spare parts or work sheet. They should be completed and Aproved for this Job Order to be closed.",
                        Stats = "Error"
                    };
                    return Json(dt, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    erviceJob.Status = MaintenanceConst.SentForClosing;
                    erviceJob.IsOnlineTransferred = true;
                    erviceJob.IsOnlineTransferredBy = User.Identity.Name;
                    erviceJob.OnlineTransferredTime = DateTime.Now;

                    _ServiceCenterJobService.Update(erviceJob);

                    return Json(new { Message = "Job order Sent for closing Successfully!", Status = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "Job order not found", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult TransferJobForWorkShop(Guid Id)
        {
            var serviceJobOrder = _ServiceCenterJobService.GetById(Id);
            if (serviceJobOrder != null)
            {

                var mechanickCrews = db.ServiceCrew.Where(q => q.ServiceID == Id).ToList();

                if (mechanickCrews.Count() <= 0)
                {
                    var dt = new
                    {
                        Message = "There is no one assigned mechanic.Please assign them!",
                        Stats = "Error"
                    };
                    return Json(dt, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    serviceJobOrder.IsSentToWorkShop = true;
                    serviceJobOrder.OnlineAssignedTime = DateTime.Now;

                    _ServiceCenterJobService.Update(serviceJobOrder);

                    return Json(new { Message = "Job order has assigned to workshop Successfully!", Status = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "Job order not found", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RejectJobOrderClosing()
        {
            var jobid = Guid.Parse(Request.Form["id"]);
            var JobOrder = _JobOrderService.GetById(jobid);
            JobOrder.Status = MaintenanceConst.Rejected;
            JobOrder.RejectionReason = Request.Form["RejectionReason"];
            JobOrder.FailureReason = MaintenanceConst.Open;
            JobOrder.IsOnlineTransferred = true;
            JobOrder.IsOnlineTransferredBy = string.Empty;
            JobOrder.OnlineTransferredTime = null;



            _JobOrderService.Update(JobOrder);
            return Redirect("/Maintenance/Joborder");
        }

        public ActionResult RejectWorkShop(Guid id, string RejectionReason)
        {
            var jobid = id;
            var jobOrder = _ServiceCenterJobService.GetById(jobid);
            jobOrder.IsSubmittedToServiceCenter = false;
            jobOrder.WorkShopRejectedReason = RejectionReason;
            db.ServiceCenterJob.Attach(jobOrder);
            db.Entry(jobOrder).State = EntityState.Modified;
            db.SaveChanges();
            //_JobOrderService.Update(jobOrder);
            return RedirectToAction("index", "ServiceCenter");
        }
        //public ActionResult CloseWorkShop(Guid id)
        //{
        //    var jobid = id;
        //    var serviceCenterJob = _ServiceCenterJobService.GetById(jobid);
        //    serviceCenterJob.IsSecondOnlineApproved = true;
        //    serviceCenterJob.SecondOnlineApprovedBy = User.Identity.Name;
        //    serviceCenterJob.SecondOnlineApprovedTime = DateTime.Now;
        //    serviceCenterJob.Status = MaintenanceConst.Closed;
        //    db.ServiceCenterJob.Attach(serviceCenterJob);
        //    db.Entry(serviceCenterJob).State = EntityState.Modified;
        //    db.SaveChanges();
        //    return RedirectToAction("index", "ServiceCenter");
        //}
        
            //Copy 
        public ActionResult CloseWorkShop(Guid id)
        {
            var jobid = id;
            var serviceCenterJob = _ServiceCenterJobService.GetById(jobid);
            serviceCenterJob.IsSecondOnlineApproved = true;
            serviceCenterJob.SecondOnlineApprovedBy = User.Identity.Name;
            serviceCenterJob.SecondOnlineApprovedTime = DateTime.Now;
            serviceCenterJob.Status = MaintenanceConst.Closed;
            db.ServiceCenterJob.Attach(serviceCenterJob);
            db.Entry(serviceCenterJob).State = EntityState.Modified;
            db.SaveChanges();

            // Added After JobOrder Stoped Closing 2/26/2024
            var jobOrder = db.JobOrder.Find(serviceCenterJob.JobOrderID);
            if(jobOrder != null)
            {
                jobOrder.IsSubmittedToServiceCenter = true;
                db.JobOrder.Attach(jobOrder);
                db.Entry(jobOrder).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("index", "ServiceCenter");
        }

        public ActionResult OpenJobOrder(Guid id)
        {
            var JobOrder = _JobOrderService.GetById(id);
            JobOrder.Status = MaintenanceConst.Open;
            JobOrder.FailureReason = MaintenanceConst.Open;
            JobOrder.IsOnlineTransferred = false;
            JobOrder.IsOnlineTransferredBy = string.Empty;
            JobOrder.OnlineTransferredTime = null;



            _JobOrderService.Update(JobOrder);
            return Redirect("/Maintenance/Joborder");
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
        public ActionResult RequistionResult(string JobOrderId)
        {
            ViewBag.JobOrderId = JobOrderId;
            return View();
        }

        public ActionResult PettyCashStatus()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaintenacePettyCashes_Create(FleetPettyCash fleetPettyCash)
        {
            ModelState.Remove("FleetPettyCashId");
            ModelState.Remove("Equipment");
            var Job = db.JobOrder.Where(x => x.Num.ToString() == fleetPettyCash.JobOrderId).FirstOrDefault();

            var entity = new FleetPettyCash
            {
                FleetPettyCashId = Guid.NewGuid(),
                //FleetsID = Job.FleetsID,
                Amount = fleetPettyCash.Amount,
                JobOrderId = fleetPettyCash.JobOrderId,
                Type = fleetPettyCash.Type,
                Description = fleetPettyCash.Description,
                Status = fleetPettyCash.Status,
                Remark = fleetPettyCash.Remark,
                IsVoid = fleetPettyCash.IsVoid,
                VoidBy = fleetPettyCash.VoidBy,
                VoidTime = fleetPettyCash.VoidTime,
                IsOnlineApproved = fleetPettyCash.IsOnlineApproved,
                OnlineApprovedBy = fleetPettyCash.OnlineApprovedBy,
                OnlineApprovedTime = fleetPettyCash.OnlineApprovedTime,
                IsOnlineTransferred = fleetPettyCash.IsOnlineTransferred,
                IsOnlineTransferredBy = fleetPettyCash.IsOnlineTransferredBy,
                OnlineTransferredTime = fleetPettyCash.OnlineTransferredTime,
                IsSendForApproval = fleetPettyCash.IsSendForApproval,
                OrgBranchName = fleetPettyCash.OrgBranchName,
                SendForApprovalBy = fleetPettyCash.SendForApprovalBy,
                SendForApprovalTime = fleetPettyCash.SendForApprovalTime,
                DateCreated = DateTime.Now,
                LastModified = fleetPettyCash.LastModified,
                CreatedBy = User.Identity.Name,
                ModifiedBy = fleetPettyCash.ModifiedBy
            };

            db.FleetPettyCashes.Add(entity);
            db.SaveChanges();
            fleetPettyCash.FleetPettyCashId = entity.FleetPettyCashId;

            return View("PettyCashStatus");
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
                        //var InventoryItem = db.InventoryItems.Where(x => x.ItemCode == Requestion.InventoryItemId)
                        //    .FirstOrDefault();
                        //var SpareRequest = new StoreRequisitionItem
                        //{

                        //    RequestedQuantity = Requestion.Quantity,
                        //    PartNo = InventoryItem.PartNo,
                        //    ItemSpecification = InventoryItem.Name,
                        //    ItemCategoryCode = InventoryItem.ItemCategoryCode,
                        //    ItemCode = InventoryItem.ItemCode,
                        //    ApprovedQuantity = 0,
                        //    IssuedQuantity = 0,
                        //    PurchaseQuantity = 0,
                        //    IsApproved = false,
                        //    GeneratePurchaseReq = true,
                        //    DateCreated = DateTime.Now,
                        //    CreatedBy = User.Identity.Name,
                        //    StoreRequisitionID = RequestionNo,
                        //};
                        //db.StoreRequisitionItems.Add(SpareRequest);
                        //db.SaveChanges();

                        //var JobStoreRequistion = new JobStoreRequisition
                        //{
                        //    JobStoreRequisitionId = Guid.NewGuid(),
                        //    JobId = Id,
                        //    RequisitionId = SpareRequest.StoreRequisitionItemID,
                        //    CreatedBy = User.Identity.Name,
                        //    DateCreated = DateTime.Now,
                        //    Type = Enum.GetName(Requestion.Type.GetType(), Requestion.Type)
                        //};
                        //db.JobStoreRequisition.Add(JobStoreRequistion);
                        //db.SaveChanges();

                    }
                }
            }
            catch (Exception)
            {

            }

            return RedirectToAction("RequistionResult", "ServiceCenter");
        }

        public ActionResult GetServiceRequisition([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            List<RequistionItemViewModel> StoreRequisitions = new List<RequistionItemViewModel>();


            var Spares = db.ServiceSpare.Where(x => x.ServiceID == Id && x.IsOnlineApproved == true).ToList();

            foreach (var Spare in Spares)
            {

                var Requestions = db.JobStoreRequisition.Where(x => x.JobId == Id).ToList();

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
                            StoreRequisitionID = RequesitionItem.StoreRequisitionID,
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
            catch (Exception)
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
            catch (Exception)
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
            catch (Exception)
            {

                return 0;
            }
        }
        public async Task GetMaintainaceDue()
        {
            IFleetService _FleetService = new FleetService();
            IServiceService _ServiceService = new ServiceService();
            IPlannerService _PlannerService = new PlannerService();
            List<MaintainceDueViewModel> FinalAssetsDues = new List<MaintainceDueViewModel>();
            Guid AssetsId = Guid.Empty;
            Guid AssetID = Guid.Empty;
            List<Fleets> AssetsDue = _FleetService.GetAll().Where(x => x.Status != MaintenanceConst.OnMaintenance).ToList();
            String AssetName = "";

            int CountDue = 0;
            String IsMileageOrDate = null;
            try
            {

                foreach (var AssetsDues in AssetsDue)
                {
                    AssetID = Guid.Empty;
                    AssetName = AssetsDues.EquipmentName + "--" + AssetsDues.PlateNumber;
                    List<Service> AssetsDuess = _ServiceService.GetAll().Where(x => x.PlannerID == AssetsDues.PlannerID).ToList();// MDueMgt.getMaintenaceDueInforamation().Where(x => x.Status != "OnMaintenance" && x.AssetId == AssetsDues).ToList();
                    int TotDues = AssetsDuess.Count;

                    foreach (var Services in AssetsDuess)
                    {
                        CountDue = CountDue + 1;
                        IsMileageOrDate = null;

                        Service PMM = Services;

                        if (PMM.IsPm == MaintenanceConst.Planned)
                        {
                            if (PMM.Planner.TrackByDate)
                            {

                                if (Services.DateIntervalType == MaintenanceConst.Day)
                                {

                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddDays(Double.Parse(Services.DateInterval.ToString()));
                                    if (DateTime.Now > newDay)
                                    {
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                        Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                                    }

                                }
                                if (Services.DateIntervalType == MaintenanceConst.Week)
                                {

                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddDays(Double.Parse(Services.DateInterval.ToString()) * 7);
                                    if (DateTime.Now > newDay)
                                    {
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                        Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                                    }

                                }
                                else if (Services.DateIntervalType == MaintenanceConst.Month)
                                {
                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddMonths(int.Parse(Services.DateInterval.ToString()));
                                    if (DateTime.Now > newDay)
                                    {
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                        Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                                    }

                                }
                                else if (Services.DateIntervalType == MaintenanceConst.Year)
                                {
                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddYears(int.Parse(Services.DateInterval.ToString()));
                                    if (DateTime.Now > newDay)
                                    {
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                        Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                                    }

                                }
                            }

                            if (PMM.Planner.TrackByMileage)
                            {

                                int OdoMeterMeasure = int.Parse(AssetsDues.CurentOdoMeter.ToString()) - int.Parse(AssetsDues.LastMaintenanceOdometer.ToString());
                                if (OdoMeterMeasure > Services.MileageInterval)
                                {
                                    IsMileageOrDate = IsMileageOrDate + " By Mileage";
                                    Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Mileage " + OdoMeterMeasure + " KM", URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });


                                }

                            }
                        }



                    }


                }
            }
            catch (Exception)
            {

            }

        }
        #endregion


    }
}