using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
//using ExceedERP.Helpers.CRM;
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
    [AuthorizeRoles(MaintenanceRoles.MaintenanceOrderUser)]
    public class JobOrderClosedController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        private IJobOrderDetailService JobOrderDetailService;
        private IJobOrderService _JobOrderService;
        private readonly IFleetService _FleetService;
        private readonly IAdministrativeApprovalService _AdministrativeApprovalService;
        private ExceedDbContext db = new ExceedDbContext();

        public const string Finished = "Finished";
        public const string Pending = "Pending";
        public const string Active = "Active";

        public JobOrderClosedController()
        {
            _JobOrderService = new JobOrderService();
            _FleetService = new FleetService();
            JobOrderDetailService = new JobOrderDetailService();
            _AdministrativeApprovalService = new AdministrativeJobApprovalService();
        }

        //
        // GET: /FleetManagement/JobOrder/
        public async Task<ActionResult> Index()  
        {
            var mechanic = db.MechanicCrew;
            ViewData["Mechanic"] = mechanic.Select(s => new {
                Code = s.MechanicCrewID,
                Name = s.Person_Employee.FirstName + " " + s.Person_Employee.MiddleName + " " + s.Person_Employee.LastName
            }).AsQueryable();
            ViewData["Service"] = db.RepairType.Select(s => new {
                Code = s.RepairTypeID,
                Name = s.Code + "-" + s.Name
            }).ToList();
            ViewData["SubCategories"] = db.ItemCategorys.Select(s => new {
                Code = s.ItemCategoryCode,
                Name = s.Name
            }).ToList();
            ViewData["JobCauses"] = db.JobCauses.ToList().Select(s => new {
                Code = s.JobCausesId,
                Name = XAPI.Common.RemoveSpecialChars(s.Description)
            }).ToList();
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
            

            ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new { e.EquipmentInformationId, e.LicensePlate, e.DeviceNumber }).ToList();
            await GetMaintainaceDue();
            ViewBag.ServiceType = db.MaintenanceType.ToList();

            //UpdateJobOrderStatus();
            PopulateViewData();

            // workshop
            ViewData["SubCategories"] = db.ItemCategorys.Select(s => new {
                Code = s.ItemCategoryCode,
                Name = s.Name
            }).ToList();
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();
            var jobOrderDetail = db.JobOrderServiceCenterDetails;

            ViewData["JobOrderDetail"] = jobOrderDetail.Select(s => new {
                Code = s.JobOrderServiceCenterDetailId,
                Name = s.RepairType.Name + "-" + s.JobCauses.Name

            }).AsQueryable();
            ViewBag.FailureReason = db.FailureReasons.ToList();
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var costCenter = CacheData.GetWorkUnitCaches().AsQueryable();
            var branches = CacheData.GetBranchCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

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
            ViewData["storesInCompany"] = store;

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

            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var selectedcostCenter = CacheData.GetWorkUnitCaches().AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;
            ViewData["Service"] = db.RepairType.Select(s => new {
                Code = s.RepairTypeID,
                Name = s.Code + "-" + s.Name
            }).ToList();
            SetInitials();
            return View();
        }
        private void PopulateViewData()
        {
            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
            var branches = CacheData.GetBranchCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();
            var structure = db.MainCompanyStructures;


            ViewData["Structure"] = structure;

            ViewData["Branches"] = branches;

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = CacheData.GetBusinessUnitCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();
                ViewData["BusinessUnits"] = businessUnits;
            }

            var items = db.ApSetupItems.ToList();
            ViewData["Items"] = items;
        }
        public void SetInitials()
        {
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["SalesStore"] = db.SalesStores.Select(s => new { s.SalesStoreId, s.Description }).ToList();
            ViewData["VoucherExtension"] = db.VoucherExtensions.AsQueryable();
            ViewData["Thing"] = db.Products.AsQueryable();
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;

            ViewData["OrganizationCust"] = db.OrganizationCustomers.ToList();

            ViewData["VoucherList"] = db.SalesQuoteVouchers.AsQueryable();
            ViewData["CartList"] = db.Carts.AsQueryable();
            var termList = db.Terms;
            //var lookUpList = db.Lookups.ToList();
            //foreach (Term term in termList)
            //{
            //    Lookup selectedCaegory = lookUpList.Where(l => l.LookupId.ToString() == term.LookupId).ToList().FirstOrDefault();
            //    if (selectedCaegory != null)
            //    {
            //        term.CategoryDescription = selectedCaegory.Description;
            //    }
            //}

            ViewData["TermList"] = termList.ToList();
            ViewData["SerialDefinitionList"] = db.SerialDefinitions.AsQueryable();
            ViewData["VoucherReferenceList"] = db.SalesQuoteVouchers.AsQueryable();
            ViewData["StoreList"] = db.Stores.AsQueryable();
            ViewData["VoucherExtensionList"] = db.VoucherExtensions.AsQueryable();

            ViewData["Branches"] = CacheData.GetBranchCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();

            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            ViewData["AddtionalCostTypes"] = db.AdditionalCosts.Select(a => new { a.AdditionalCostId, a.Name }).ToList();
            ViewData["QuotationTerms"] = db.QuotationTerms.Select(a => new { a.QuotationTermId, a.QuotationInDays, a.Desription }).ToList();

            ViewData["RepairTypes"] = db.RepairType.Select(r => new { RepairTypeId = r.RepairTypeID.ToString(), r.Name });
        }
        //public JsonResult getOrder(string Num)
        //{
        //    var jobOrder = db.JobOrder.Where(q => q.Num.ToString() == Num).FirstOrDefault();

        //    return Json(jobOrder, JsonRequestBehavior.AllowGet);
        
        //}

        public ActionResult UpdateJobOrderSourcesToMuchJobRequest()
        {
            var jobs = db.JobOrder.ToList();
            foreach (JobOrder job in jobs)
            {
                var req = db.JobRequest.FirstOrDefault(r => r.Num == job.Referance);
                if (req != null)
                {
                    job.IsExternal = req.Source == JobRequestSource.ExternalVehicle ? true : false;

                    db.JobOrder.Attach(job);
                    db.Entry(job).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Redirect("Index");
        }
        
        public ActionResult JobOrderClosed_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            IQueryable<JobOrder> jobOrders =
                db.JobOrder
                //.AsNoTracking()
                //.ToList()
                .Where(x => mybraches.Contains(x.CreatedAtBranchId) || x.CreatedAtBranchId == 0)
                .Where(x => x.IsSecondOnlineApproved)
                .AsQueryable();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                jobOrders = jobOrders.Where(x => myBusinessUnits.Contains(x.CreatedAtBusinessUnitId) || x.CreatedAtBusinessUnitId == 0);
            }
              
            
            DataSourceResult result = jobOrders.ToDataSourceResult(request, j => new JobOrder
            {
                IsPM = j.IsPM,
                IsRework = j.IsRework,
                CustomerId = db.JobRequest.Where(q => q.Num == j.Referance).FirstOrDefault()?.CustomerId,
                EquipmentName = j.EquipmentName,
                CurrentFuel = j.CurrentFuel,
                ProjectName = j.ProjectName,
                RejectionReason = j.RejectionReason,
                //Progress = GetProgressPercent(j.JobOrderID),
                Status = j.Status,
                PercentCompleted = j.AssetNum + "--" + j.EquipmentName,
                MaintenanceCenter = j.MaintenanceCenter,
                Num = j.JobOrderNum ?? j.Num,
                JobOrderNum = j.JobOrderNum,
                JobOrderID = j.JobOrderID,
                Type = j.Type,
                LicensePlate = j.LicensePlate,
                IsExternal = j.IsExternal,
                EquipmentInformationId = j.EquipmentInformationId,
                FailureReason = j.FailureReason,
                AdvanceMeasure = j.AdvanceMeasure,
                Referance = j.Referance,
                Odometer = j.Odometer,
                IsOnlineApproved = j.IsOnlineApproved,
                SecondOnlineApprovedTime = j.SecondOnlineApprovedTime,
                OnlineApprovedTime = j.OnlineApprovedTime,
                DateCreated = j.DateCreated,
               
                IsSubmittedToServiceCenter = j.IsSubmittedToServiceCenter,
                IsSendForApproval = j.IsSendForApproval,
                IsSecondSendForApproval = j.IsSecondSendForApproval,
                IsSecondOnlineApproved = j.IsSecondOnlineApproved,
                IsOnlineTransferred = j.IsOnlineTransferred,
                MaintenanceCenterId = j.MaintenanceCenterId,
                BranchId = j.BranchId,
                BusinessUnitId = j.BusinessUnitId,
                OrgStructureId = j.OrgStructureId,
                CreatedAtBranchId = j.CreatedAtBranchId,
                CreatedAtBusinessUnitId = j.CreatedAtBusinessUnitId

            });

            return Json(result);

        }

        public ActionResult JobOrderExtra_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {

            FleetCostHelper fleetCostHelper = new FleetCostHelper();
            var inventoryMapping = db.InventorySparePartMapping.ToList();

            var JobOrder = _JobOrderService.GetAll().Where(x => x.JobOrderID.ToString() == Id)
               .Select(j => new JobOrder
               {
                   IsPM = j.IsPM,
                   EquipmentName = j.EquipmentName,
                   CurrentFuel = j.CurrentFuel,
                   ProjectName = j.ProjectName,
                   Progress = GetProgressPercent(j.JobOrderID),
                   Status = j.Status,
                   PercentCompleted = j.AssetNum + "--" + j.EquipmentName,
                   MaintenanceCenter = j.MaintenanceCenter,
                   Num = j.Num,
                   JobOrderNum = j.JobOrderNum,
                   JobOrderID = j.JobOrderID,
                   Type = j.Type,
                   IsExternal = j.IsExternal,
                   EquipmentInformationId = j.EquipmentInformationId,
                   FailureReason = j.FailureReason,
                   AdvanceMeasure = j.AdvanceMeasure,
                   Referance = j.Referance,
                   Odometer = j.Odometer,
                   InspectorComments = GetMaintenaceDuration(j.JobOrderID),
                   IsOnlineApproved = j.IsOnlineApproved,
                   LabourCost = fleetCostHelper.GetLabourCost(j),
                   SpareCost = fleetCostHelper.GetJobOrderSpareCost(j),
                   //TyreCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Tyre),
                   //OilLubricantCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Oil_Lubricant),
                   //OtherCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Other),
                   //SalvageCost = fleetCostHelper.GetSalvageSpareCost(j.JobOrderID),
                   //BattreyCost = fleetCostHelper.GetJobOrderSpareCost(j, inventoryMapping, SpareType.Battery),
                   Efficiency = fleetCostHelper.GetEfficiency(j.JobOrderID),
                   ComponentCost = fleetCostHelper.GetComponentCost(j.JobOrderID),
                   IsSendForApproval = j.IsSendForApproval,
                   IsSecondSendForApproval = j.IsSecondSendForApproval,
                   IsSecondOnlineApproved = j.IsSecondOnlineApproved,
                   IsOnlineTransferred = j.IsOnlineTransferred


               }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder, JsonRequestBehavior.AllowGet);


        }

        public ActionResult ServiceCenter_Read([DataSourceRequest] DataSourceRequest request)
        {
            var JobOrder = _JobOrderService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(
                    x =>
                        x.IsSecondOnlineApproved == true && x.Type == MaintenanceConst.Internal &&
                        (x.Status == MaintenanceConst.Open || x.Status == MaintenanceConst.Closed))
                .Select(j => new JobOrder
                {
                    IsPM = j.IsPM,
                    ProjectName = j.ProjectName,
                    Status = GetProgressPercent(j.JobOrderID),
                    PercentCompleted = j.AssetNum + "--" + j.EquipmentName,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    Type = j.Type,
                    LicensePlate = j.LicensePlate,
                    EquipmentInformationId = j.EquipmentInformationId,
                    InspectorComments = GetMaintenaceDuration(j.JobOrderID),
                    IsOnlineApproved = j.IsOnlineApproved,
                    IsSendForApproval = j.IsSendForApproval,
                    IsSecondSendForApproval = j.IsSecondSendForApproval,
                    SpareCost = j.SpareCost,
                    LabourCost = j.LabourCost,
                    IsSecondOnlineApproved = j.IsSecondOnlineApproved



                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }

        public ActionResult OutSource_Read([DataSourceRequest] DataSourceRequest request)
        {
            var JobOrder = _JobOrderService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(
                    x =>
                        x.IsOnlineApproved == true && x.IsExternal == true)
                .Select(j => new JobOrder
                {
                    Num = j.Num,
                    ProjectName = j.ProjectName,
                    Status = j.Status,
                    IsPM = j.IsPM,
                    EquipmentName = j.EquipmentName,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    Type = j.Type,
                    EquipmentInformationId = j.EquipmentInformationId,
                    InspectorComments = j.InspectorComments,
                    IsOnlineApproved = j.IsOnlineApproved,
                    IsSendForApproval = j.IsSendForApproval,
                    IsSecondSendForApproval = j.IsSecondSendForApproval,
                    SpareCost = j.SpareCost,
                    LabourCost = j.LabourCost,
                    IsSecondOnlineApproved = j.IsSecondOnlineApproved,
                    IsOnlineTransferred = j.IsOnlineTransferred



                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }


        public ActionResult Request_Read([DataSourceRequest] DataSourceRequest request)
        {
            var JobOrder = _JobOrderService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(j => new JobOrder
            {
                ProjectName = j.ProjectName,
                Status = j.Status,
                PercentCompleted = j.AssetNum,
                MaintenanceCenter = j.MaintenanceCenter,
                JobOrderNum = j.JobOrderNum,
                JobOrderID = j.JobOrderID,
                EquipmentInformationId = j.EquipmentInformationId,
                Type = j.Type,
                SpareCost = j.SpareCost,
                LabourCost = j.LabourCost,
                InspectorComments = j.InspectorComments


            }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }
        public ActionResult Technical_Read([DataSourceRequest] DataSourceRequest request)
        {
            var JobOrder = _JobOrderService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(x => x.Status == MaintenanceConst.WaitingTechnical)
                .Select(j => new JobOrder
                {
                    ProjectName = j.ProjectName,
                    Status = j.Status,
                    PercentCompleted = j.AssetNum,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    EquipmentInformationId = j.EquipmentInformationId,
                    Type = j.Type,
                    SpareCost = j.SpareCost,
                    LabourCost = j.LabourCost,
                    InspectorComments = j.InspectorComments


                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }

        public ActionResult Adminstrative_Read([DataSourceRequest] DataSourceRequest request, string JobID)
        {
            var JobOrder = _JobOrderService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(x => x.Status == MaintenanceConst.WaitingAdminstrative && x.JobOrderID == Guid.Parse(JobID))
                .Select(j => new JobOrder
                {
                    ProjectName = j.ProjectName,
                    Status = j.Status,
                    PercentCompleted = j.AssetNum,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    Type = j.Type,
                    EquipmentInformationId = j.EquipmentInformationId,
                    InspectorComments = j.InspectorComments


                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }

        public ActionResult PropertyAdmin_Read([DataSourceRequest] DataSourceRequest request, string JobID)
        {
            var JobOrder = _JobOrderService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name))
                .Where(
                    x =>
                        x.IsExternal == true &&
                        x.JobOrderID == Guid.Parse(JobID))
                .Select(j => new JobOrder
                {
                    ProjectName = j.ProjectName,
                    Status = j.Status,
                    PercentCompleted = j.AssetNum,
                    MaintenanceCenter = j.MaintenanceCenter,
                    JobOrderNum = j.JobOrderNum,
                    JobOrderID = j.JobOrderID,
                    Type = j.Type,
                    EquipmentInformationId = j.EquipmentInformationId,
                    InspectorComments = j.InspectorComments
                    // Add approval status

                }).OrderByDescending(x => x.DateIssued);

            return Json(JobOrder.ToDataSourceResult(request));
        }



        public ActionResult OnHand_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            List<StoreReturnItems> Returns = new List<StoreReturnItems>();
            FleetReportHelper fleetReportHelper = new FleetReportHelper();
            var JobOrder = db.JobOrder.Where(x => x.JobOrderID.ToString() == Id).FirstOrDefault();
            if (JobOrder != null)
            {
                var StoreIssues = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.JobOrderNo == JobOrder.Num.ToString() || x.Issue.JobOrderNo == JobOrder.JobOrderID.ToString()).Select(s => new { Id = s.IssueID }).ToList();
                foreach (var Issue in StoreIssues)
                {
                    var StoreReturns = db.StoreReturns.Where(x =>
                        x.Reference == Issue.Id.ToString()).ToList();

                    foreach (var StoreReturn in StoreReturns)
                    {
                        var ReturnItems = StoreReturn.StoreReturnItems.Select(
                      s => new StoreReturnItems
                      {
                          CreatedBy = s.CreatedBy,
                          Quantity = s.Quantity,
                          UnitPrice = s.UnitPrice,
                          ItemCode = s.ItemCode,
                          DateCreated = s.DateCreated,
                          OrgBranchName = s.StoreReturn.Reference,
                      }).ToList();
                        Returns.AddRange(ReturnItems);

                    }

                }

            }

            return Json(Returns.ToDataSourceResult(request));
        }


        public ActionResult JobOrder_Create([DataSourceRequest] DataSourceRequest request, JobOrder JobOrder)
        {
            ModelState.Remove("JobOrderID");
            IFleetService _FleetService = new FleetService();
            _JobOrderService = new JobOrderService();
            #region data 
            if (JobOrder.RequesterId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (JobOrder.RequesterId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == JobOrder.RequesterId && x.isCurrent);
                if (placement != null)
                {
                    JobOrder.OrgStructureId = placement.OrgStructureId;
                    JobOrder.BranchId = placement.BranchId;
                    JobOrder.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            JobOrder.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            JobOrder.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            #endregion

            var Orders = _JobOrderService.GetAll();

            List<JobOrder> joborders =
                _JobOrderService.GetAll().Where(j => j.JobOrderNum == JobOrder.JobOrderNum).ToList();
            if (joborders.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingJobOrder", " JobOrder Number Already Exists ");
            }
            else
            {
                //EquipmentInformation fleet = db.EquipmentInformation.Find(JobOrder.EquipmentInformationId);
                //fleet.Status = MaintenanceConst.OnMaintenance;
                //db.EquipmentInformation.Attach(fleet);
                //db.Entry(fleet).State = System.Data.Entity.EntityState.Modified;
                //db.SaveChanges();

                int Num = 0;

                if (Orders != null && Orders.Any())
                {
                    var LastJob = Orders.OrderByDescending(x => x.DateCreated).FirstOrDefault();
                    Num = int.Parse(LastJob.JobOrderNum.ToString()) + 1;
                }

                JobOrder.JobOrderID = Guid.NewGuid();
                //JobOrder.AssetNum = fleet.UnitNum;
                //JobOrder.EquipmentName = fleet.EquipmentName;
                JobOrder.JobOrderNum = Num;
                JobOrder.Status = MaintenanceConst.Pending;
                JobOrder.MaintenanceCenter = "";
                JobOrder.Type = "";
                JobOrder.DateIssued = DateTime.Now;
                JobOrder.CreatedBy = User.Identity.Name;
                JobOrder.DateCreated = DateTime.Now;
                JobOrder.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);


                if (JobOrder != null)
                {
                    _JobOrderService.Insert(JobOrder);
                }
            }

            return Json(new[] { JobOrder }.ToDataSourceResult(request));
        }


        public ActionResult JobOrder_Update([DataSourceRequest] DataSourceRequest request, JobOrder JobOrder)
        {
            #region data 
            if (JobOrder.RequesterId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (JobOrder.RequesterId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == JobOrder.RequesterId && x.isCurrent);
                if (placement != null)
                {
                    JobOrder.OrgStructureId = placement.OrgStructureId;
                    JobOrder.BranchId = placement.BranchId;
                    JobOrder.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }
            #endregion
            List<JobOrder> Order =
                _JobOrderService.GetAll()
                    .Where(j => j.JobOrderNum == JobOrder.JobOrderNum && j.JobOrderID != JobOrder.JobOrderID)
                    .ToList();
            if (Order.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingJobOrder", " JobOrder Number Not Exists ");
            }
            else
            {
                if (JobOrder != null && Order != null)
                {
                    JobOrder jOrder = _JobOrderService.GetById(JobOrder.JobOrderID);

                    jOrder.ModifiedBy = User.Identity.Name;
                    jOrder.LastModified = DateTime.Now;
                    jOrder.LabourCost = JobOrder.LabourCost;
                    jOrder.SpareCost = JobOrder.SpareCost;
                    _JobOrderService.Update(jOrder);
                }
            }

            return Json(new[] { JobOrder }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ReworkJobOrder(Guid id)

        {
            var request = new DataSourceRequest();
            var jobOrder = db.JobOrder.Find(id);
            if(jobOrder!=null)
            {
                var reJobOrder = new JobOrder();
                reJobOrder.JobOrderID = Guid.NewGuid();
                reJobOrder.LicensePlate = jobOrder.LicensePlate;
                reJobOrder.IsRework = true;
                reJobOrder.Referance = jobOrder.Referance;
                reJobOrder.JobOrderNum = jobOrder.Num;
                reJobOrder.CreatedBy = User.Identity.Name;
                reJobOrder.DateCreated = DateTime.Now;
                reJobOrder.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                reJobOrder.Status = MaintenanceConst.Pending;
                db.JobOrder.Add(reJobOrder);
                db.SaveChanges();
                return Json(new[] { reJobOrder }.ToDataSourceResult(request, ModelState));

            }
            else
            {
                var formattedData = new
                {
                    Message = "Job Order is not available.",
                    Status = "Record_Not_Found"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult DeliverJobOrder(Guid id)

        {
            var request = new DataSourceRequest();
            var jobOrder = db.JobOrder.Find(id);
            if(jobOrder!=null&&jobOrder.Status== MaintenanceConst.Closed)
            {
               
                jobOrder.Status = MaintenanceConst.Delivered;
                db.JobOrder.Attach(jobOrder);
                db.Entry(jobOrder).State = EntityState.Modified;
                db.SaveChanges();
                List<UserVM> list = new List<UserVM>();
                list = UserInformationHelper.GetUsersInRole(MaintenanceRoles.ServiceUser);
                foreach (var user in list)
                {
                    NotifModels jobRequesterNotif = new NotifModels
                    {
                        UserId = user.UserId,
                        Title = "Delivered Maintenance",
                        Message = $"There is delivered maintenance",
                        URL = "/Maintenance/JobOrder",

                    };
                    Notify.NotifyUser(jobRequesterNotif);
                }
                list = UserInformationHelper.GetUsersInRole(MaintenanceRoles.MaintenanceOrderUser);
                foreach (var user in list)
                {
                    NotifModels jobApproverNotif = new NotifModels
                    {
                        //UserId = UserInformationHelper.GetEmployeeUserId(empId.),
                        UserId = user.UserId,
                        Title = "Delivered Maintenance",
                        Message = $"There is delivered maintenance",
                        URL = "/Maintenance/JobOrder"

                    };
                    Notify.NotifyUser(jobApproverNotif);
                }
                return Json(new[] { jobOrder }.ToDataSourceResult(request, ModelState));

            }
            else
            {
                var formattedData = new
                {
                    Message = "Job Order is not available.",
                    Status = "Record_Not_Found"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult JobOrder_Destroy([DataSourceRequest] DataSourceRequest request, JobOrder jobOrder)
        {
            try
            {
                IJobOrderDetailService _JobOrderDetailService = new JobOrderDetailService();
                IServiceSpareService _ServiceSpareService = new ServiceSpareService();
                var JobOrder = _JobOrderService.GetById(jobOrder.JobOrderID);

                JobOrder.IsSecondOnlineApproved = true;
                JobOrder.SecondOnlineApprovedBy = User.Identity.Name;
                JobOrder.SecondOnlineApprovedTime = DateTime.Now;
                JobOrder.Status = MaintenanceConst.Closed;


                var updateStatus = _JobOrderService.Update(JobOrder);

                if (updateStatus)
                {
                    //var Fleet = db.EquipmentInformation.FirstOrDefault(j => j.LicensePlate == jobOrder.LicensePlate);
                    //if(Fleet != null)
                    //{
                    //    Fleet.Status = MaintenanceConst.ACTIVE;
                    //    Fleet.OdometerOnLastMaintenance = Fleet.CurrentOdometer;
                    //    Fleet.FutureMaintenanceAlert = FutureMaintenanceAlert.Off;
                    //    Fleet.OdometerOnNextRegularMaintenance = Fleet.OdometerOnLastMaintenance + Fleet.RegularMaintenanceInterval;
                    //    Fleet.RemainingKmForUpComingMaintenance = Fleet.OdometerOnNextRegularMaintenance - Fleet.CurrentOdometer;
                    //    Fleet.LastMaintained = DateTime.Now;

                    //    db.EquipmentInformation.Attach(Fleet);
                    //    db.Entry(Fleet).State = System.Data.Entity.EntityState.Modified;
                    //    db.SaveChanges();
                    //    Notify.NotifyUser(new NotifModels
                    //    {
                    //        DeadLine = DateTime.Now,
                    //        Message = "Equipment with " + Fleet.LicensePlate + " Finished JobOrder ",
                    //        Title = " Finished JobOrder ",
                    //        URL = Url.Action("Index", "JobOrder", new { area = "FleetManagment" }),
                    //    });
                    //}
                    CRMMaintenanceIntegrationHelper.CreateSalesQuoteVoucherFromMaintenance(JobOrder, User.Identity.Name);
                }

                // return Json(new[] { JobOrder }.ToDataSourceResult(request));
                return Redirect("Index");
            }
            catch (Exception)
            {
                return Json(new[] { jobOrder }.ToDataSourceResult(request));
            }


        }
        public string GetJobStatus(string Id)
        {

            JobOrder JobRequests = db.JobOrder.Where(j => j.JobOrderID.ToString() == Id && (j.IsOnlineTransferred == true || j.IsOnlineApproved == true || j.IsSendForApproval == true)).FirstOrDefault();
            if (JobRequests != null)
            {
                return JobRequests.Status;
            }

            return "Pending";


        }


        public ActionResult TechnicalApproval_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var Admin = new List<TechnicalApprovalViewModel>();
            Guid JId = Guid.Parse(Id);
            var Request = db.JobOrder.Where(x => x.JobOrderID.ToString() == Id).FirstOrDefault();
            if (Request != null)
            {
                if (Request.IsOnlineApproved)
                {
                    Admin =
                        db.TechnicalJobApproval
                            .Where(x => x.TechnicalJobId.ToString() == Id).Select(s => new TechnicalApprovalViewModel
                            {
                                JobOrderID = JId,
                                DateCreated = s.DateCreated,
                                CreatedBy = s.CreatedBy,
                                ApproveStatus = ApprovalStatuses.Approve,
                                //Type = Request.IsExternal ? "EXTERNAL" : "INTERNAL",
                                MaintenanceCenter = Request.MaintenanceCenter

                            })
                            .ToList();


                }

                if (Request.IsSendForApproval && !Request.IsOnlineApproved)
                {
                var adminApproval = new TechnicalApprovalViewModel
                    {
                        JobOrderID = JId,

                        Status = ApprovalStatuses.Pending,
                        Remark = string.Empty,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    Admin.Add(adminApproval);

                }
            }
            return Json(Admin.ToDataSourceResult(request));
        }


        public ActionResult Administrative_Approval([DataSourceRequest] DataSourceRequest request, JobOrder jobOrder)
        {

            var JobOrder = _JobOrderService.GetById(jobOrder.JobOrderID);
            JobOrder.Status = MaintenanceConst.Approved;

            if (JobOrder != null)
            {

                _JobOrderService.Update(JobOrder);

            }

            return Json(new[] { JobOrder }.ToDataSourceResult(request));
        }


        public ActionResult Operator_SendForApproval(string JobOrderID)
        {

            var JobOrder = _JobOrderService.GetById(Guid.Parse(JobOrderID));
            JobOrder.Status = MaintenanceConst.WaitingTechnical;
            JobOrder.SendForApprovalBy = User.Identity.Name;
            JobOrder.SendForApprovalTime = DateTime.Now;
            if (JobOrder != null)
            {

                _JobOrderService.Update(JobOrder);

            }
            ViewBag.ServiceType = db.MaintenanceType.ToList();

            return View("Index");
        }

        public ActionResult Technical_SendForApproval(string Id)
        {
            DataSourceRequest request = new DataSourceRequest();
            var JobOrder = _JobOrderService.GetById(Guid.Parse(Id));
            var jobOrderDetail = db.JobOrderDetail.Where(q => q.JobOrderID.ToString() == Id).Count();
            JobOrder.Status = MaintenanceConst.WaitingApproval;
            JobOrder.SendForApprovalBy = User.Identity.Name;
            JobOrder.IsSendForApproval = true;
            JobOrder.SendForApprovalTime = DateTime.Now;
            if (jobOrderDetail == 0)
            {
                ModelState.AddModelError("EmptyService", " Job order has not any service ");
                ViewData["Error"] = "Job order has not any service";
                return RedirectToAction("Index");
            }
            if (JobOrder != null&& jobOrderDetail!=0)
            {

                _JobOrderService.Update(JobOrder);

            }
            ViewBag.ServiceType = db.MaintenanceType.ToList();
            return RedirectToAction("Index");
        }

        public ActionResult UpdateType(string Id, string[] JobType)
        {
            string Type = string.Empty;
            try
            {
                foreach (var type in JobType)
                {
                    Type = Type + " - " + type;
                }

                var JobOrder = _JobOrderService.GetById(Guid.Parse(Id));
                if (JobOrder != null)
                {
                    JobOrder.Type = Type;
                    JobOrder.ModifiedBy = User.Identity.Name;
                    JobOrder.LastModified = DateTime.Now;

                    _JobOrderService.Update(JobOrder);
                }
                ViewBag.ServiceType = db.MaintenanceType.ToList();
            }
            catch (Exception)
            { }

            return RedirectToAction("Index", "JobOrder");
        }



        public ActionResult UpdateAdvanceMeasure(string Id, string JobType)
        {
            try
            {


                var JobOrder = _JobOrderService.GetById(Guid.Parse(Id));
                if (JobOrder != null)
                {
                    JobOrder.AdvanceMeasure = JobType;
                    JobOrder.ModifiedBy = User.Identity.Name;
                    JobOrder.LastModified = DateTime.Now;

                    _JobOrderService.Update(JobOrder);
                }
                ViewBag.ServiceType = db.MaintenanceType.ToList();
            }
            catch (Exception)
            { }

            return RedirectToAction("Index", "ServiceCenter");
        }


        public ActionResult UpdateFailureReason(string Id, string JobType)
        {
            try
            {


                var JobOrder = _JobOrderService.GetById(Guid.Parse(Id));
                if (JobOrder != null)
                {
                    JobOrder.FailureReason = JobType;
                    JobOrder.ModifiedBy = User.Identity.Name;
                    JobOrder.LastModified = DateTime.Now;

                    _JobOrderService.Update(JobOrder);
                }
                ViewBag.ServiceType = db.MaintenanceType.ToList();
            }
            catch (Exception)
            { }

            return RedirectToAction("Index", "WorkShop");
        }


        public ActionResult Admin_SendForApproval(string JobOrderID)
        {

            var JobOrder = _JobOrderService.GetById(Guid.Parse(JobOrderID));
            JobOrder.Status = MaintenanceConst.Approved;
            JobOrder.SendForApprovalBy = User.Identity.Name;
            JobOrder.SendForApprovalTime = DateTime.Now;
            if (JobOrder != null)
            {

                _JobOrderService.Update(JobOrder);

            }
            ViewBag.ServiceType = db.MaintenanceType.ToList();

            return View("Index");
        }

        
        public void UpdateJobOrderStatus()
        {
            var jobOrders=db.JobOrder.Where(x=>x.Status== MaintenanceConst.Open).ToList();

            foreach (var job in jobOrders)
            {
                var services=db.ServiceCenterJob
                    .Where(x=>x.JobOrderID==job.JobOrderID).ToList();

                if(services.Any())
                {
                    var uncompleted = services.Where(x => x.Status != MaintenanceConst.Closed).ToList();
                    if (!uncompleted.Any())
                    {
                        job.IsSubmittedToServiceCenter = true;
                        db.JobOrder.Attach(job);
                        db.Entry(job).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
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

                        var WorkLogs = db.ServiceCrewWorkLogs.Where(x => x.MecanicId == Crew.MechanicCrewID && x.IsOnlineApproved == true && x.JobOrderDetailId == Service.JobOrderDetailID).ToList();

                        foreach (var WorkLog in WorkLogs)
                        {
                            var Duration = ((DateTime.Parse(WorkLog.WorkingHoursMorningto.ToString()) - DateTime.Parse(WorkLog.WorkingHoursMorningFrom.ToString())).TotalHours) + ((DateTime.Parse(WorkLog.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(WorkLog.WorkingHoursAfternoonFrom.ToString())).TotalHours);
                            Cost = Cost + (decimal.Parse(Duration.ToString()) * Crew.MechanicCrews.HourlyWage);

                        }
                    }
                }

                return Cost;
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
            string AssetName = "";

            int CountDue = 0;
            string IsMileageOrDate = null;
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

                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddDays(double.Parse(Services.DateInterval.ToString()));
                                    if (DateTime.Now > newDay)
                                    {
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                        Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                                    }

                                }
                                if (Services.DateIntervalType == MaintenanceConst.Week)
                                {

                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddDays(double.Parse(Services.DateInterval.ToString()) * 7);
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

