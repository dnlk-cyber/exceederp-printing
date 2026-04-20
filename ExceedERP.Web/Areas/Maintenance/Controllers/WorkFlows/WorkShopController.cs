using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
      //[AuthorizeRoles(MaintenanceRoles.MaintenanceWorkShopUser)] 
    public class WorkShopController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        private IJobOrderDetailService JobOrderDetailService;
        private IJobOrderService _JobOrderService;
        private readonly IFleetService _FleetService;
        private IServiceService _ServiceService;
        private IAdministrativeApprovalService _AdministrativeApprovalService;
        private IServiceCenterJobService _ServiceCenterJobService;
        private IServiceJobOrderDetailService _ServiceJobOrderDetailService;
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult IndexForFleet()
        {
            //PopulateViewData();

            ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new { e.EquipmentInformationId, e.LicensePlate, e.DeviceNumber }).ToList();
            ViewData["VehicleParts"] = db.VehiclePart.Select(x => new
            {
                Name = x.Name,
                VehiclePartId = x.VehiclePartId
            }).ToList();
            ViewData["CanibalizePlates"] = db.Fleets.Select(x => new
            {
                Name = x.PlateNumber,
                PlateNumber = x.PlateNumber
            }).ToList();
            var mechanic = db.MechanicCrew;
            ViewData["Mechanic"] = mechanic.Select(s => new {
                Code = s.MechanicCrewID,
                Name = s.Person_Employee.FirstName + " " + s.Person_Employee.MiddleName + " " + s.Person_Employee.LastName
            }).AsQueryable();
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
            ViewData["Service"] = db.RepairType.Select(s => new
            {
                Code = s.RepairTypeID,
                Name = s.Code + "-" + s.Name
            }).ToList();
            ViewData["SubCategories"] = db.ItemCategorys.Select(s => new
            {
                Code = s.ItemCategoryCode,
                Name = s.Name
            }).ToList();
            ViewData["JobCauses"] = db.JobCauses.ToList().Select(s => new
            {
                Code = s.JobCausesId,
                Name = XAPI.Common.RemoveSpecialChars(s.Description)
            }).ToList();
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new
            {
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
            ViewBag.FailureReason = db.FailureReasons.ToList();
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var costCenter = db.BranchCostCenter;
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

            ViewData["Branches"] = CacheData.GetBranchCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();

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
            SetInitials();

            ViewData["ServiceGroup"] = db.ServiceGroupSettings.Select(s => new
            {
                Code = s.ServiceGroupSettingId,
                Name = s.ServiceGroupName
            }).ToList();
            ViewData["SubCategories"] = db.ItemCategorys.Select(s => new
            {
                Code = s.ItemCategoryCode,
                Name = s.Name
            }).ToList();
            ViewData["JobCauses"] = db.JobCauses.ToList().Select(s => new
            {
                Code = s.JobCausesId,
                Name = XAPI.Common.RemoveSpecialChars(s.Description)
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


            //ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new { e.EquipmentInformationId, e.LicensePlate, e.DeviceNumber }).ToList();
            //await GetMaintainaceDue();
            ViewBag.ServiceType = db.MaintenanceType.ToList();

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

            //UpdateJobOrderStatus();
            PopulateViewData();
            return View();
        }
        public ActionResult Index()
        {
            //PopulateViewData();

            ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new { e.EquipmentInformationId, e.LicensePlate, e.DeviceNumber }).ToList();
            ViewData["VehicleParts"] = db.VehiclePart.Select(x => new
            {
                Name = x.Name,
                VehiclePartId = x.VehiclePartId
            }).ToList();
            ViewData["CanibalizePlates"] = db.Fleets.Select(x => new
            {
                Name = x.PlateNumber,
                PlateNumber = x.PlateNumber
            }).ToList();
            var mechanic = db.MechanicCrew;
            ViewData["Mechanic"] = mechanic.Select(s => new {
                Code = s.MechanicCrewID,
                Name = s.Person_Employee.FirstName + " " + s.Person_Employee.MiddleName + " " + s.Person_Employee.LastName
            }).AsQueryable();
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
            ViewData["Service"] = db.RepairType.Select(s => new
            {
                Code = s.RepairTypeID,
                Name = s.Code + "-" + s.Name
            }).ToList();
            ViewData["SubCategories"] = db.ItemCategorys.Select(s => new
            {
                Code = s.ItemCategoryCode,
                Name = s.Name
            }).ToList();
            ViewData["JobCauses"] = db.JobCauses.ToList().Select(s => new
            {
                Code = s.JobCausesId,
                Name = XAPI.Common.RemoveSpecialChars(s.Description)
            }).ToList();
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new
            {
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
            ViewBag.FailureReason = db.FailureReasons.ToList();
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var costCenter = db.BranchCostCenter;
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
            SetInitials();

            ViewData["ServiceGroup"] = db.ServiceGroupSettings.Select(s => new
            {
                Code = s.ServiceGroupSettingId,
                Name = s.ServiceGroupName
            }).ToList();
            ViewData["SubCategories"] = db.ItemCategorys.Select(s => new
            {
                Code = s.ItemCategoryCode,
                Name = s.Name
            }).ToList();
            ViewData["JobCauses"] = db.JobCauses.ToList().Select(s => new
            {
                Code = s.JobCausesId,
                Name = XAPI.Common.RemoveSpecialChars(s.Description)
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


            //ViewData["EquipmentsList"] = db.EquipmentInformation.Select(e => new { e.EquipmentInformationId, e.LicensePlate, e.DeviceNumber }).ToList();
            //await GetMaintainaceDue();
            ViewBag.ServiceType = db.MaintenanceType.ToList();

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

            //UpdateJobOrderStatus();
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var stores = db.InventoryStores;
            var branches = CacheData.GetBranchCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();
            var costCenter = db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;


            ViewData["costCenters"] = selectedcostCenter;
            ViewData["stores"] = stores;
            ViewData["Branches"] = CacheData.GetBranchCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            // var branches = db.Branch;
            var structure = db.MainCompanyStructures;


            ViewData["Structure"] = structure;

            // ViewData["Branches"] = branches;

            ViewData["defaultCategory"] = categories.FirstOrDefault();
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
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            ViewData["AddtionalCostTypes"] = db.AdditionalCosts.Select(a => new { a.AdditionalCostId, a.Name }).ToList();
            ViewData["QuotationTerms"] = db.QuotationTerms.Select(a => new { a.QuotationTermId, a.QuotationInDays, a.Desription }).ToList();

            ViewData["RepairTypes"] = db.RepairType.Select(r => new { RepairTypeId = r.RepairTypeID.ToString(), r.Name });
        }
        public WorkShopController()
        {
            JobOrderDetailService = new JobOrderDetailService();
            _JobOrderService = new JobOrderService();
            _FleetService = new FleetService();
            _AdministrativeApprovalService = new AdministrativeJobApprovalService();
            _ServiceService = new ServiceService();
            _ServiceCenterJobService=new ServiceCenterJobService();
            _ServiceJobOrderDetailService=new ServiceJobOrderDetailService();
        }

        #region SalesQuote
        public ActionResult Vouchers_Read([DataSourceRequest]DataSourceRequest request, Guid id)
        {
            List<SalesQuoteVoucherView> voulist = new List<SalesQuoteVoucherView>();
            List<SalesQuoteVoucher> quoteVouchers = new List<SalesQuoteVoucher>();

            //var assignedStores = db.SalesPersonAssignments.Where(a => a.UserName == User.Identity.Name).Select(a => a.SalesStoreId).ToList();
            var serviceCenterJobOrder = db.ServiceCenterJob.Find(id);
            quoteVouchers = db.SalesQuoteVouchers
                    .Where(b => b.Source == SalesQuoteSource.Maintenance && b.JobOrderNumber == serviceCenterJobOrder.JobOrderID.ToString()).ToList();
            foreach (var vou in quoteVouchers)
            {
                decimal tottax = 0;
                decimal totgrand = 0;

                var lineItems = db.LineItems.Where(t => t.Voucher == vou.SalesQuoteVoucherID.ToString() && t.VoucherDefinitionID == "0").ToList();
                var serviceLineItems = db.ServiceLineItems.Where(t => t.VoucherId == vou.SalesQuoteVoucherID && t.VoucherType == CRMVoucherType.SalesQuote).ToList();

                
                if (lineItems.Any())
                {
                    tottax += lineItems.Sum(x => x.TaxAmount);
                    totgrand += lineItems.Sum(x => x.TotalAmount);
                }
                if (serviceLineItems.Any())
                {
                    tottax += serviceLineItems.Sum(x => x.TaxAmount);
                    totgrand += serviceLineItems.Sum(x => x.TotalAmount);
                }

                voulist.Add(new SalesQuoteVoucherView
                {
                    SalesQuoteVoucherID = vou.SalesQuoteVoucherID,
                    Source = vou.Source,
                    Period = vou.Period,
                    OrganizationCustomerId = vou.OrganizationCustomerId,
                    Date = vou.Date,
                    GrandTotal = totgrand,
                    IsIssued = vou.IsIssued,
                    IssuedDate = vou.IssuedDate,
                    DueDate = vou.DueDate,
                    QuotationTermId = vou.QuotationTermId,
                    IsVoid = vou.IsVoid,
                    BranchId = vou.BranchId,
                    StoreID = vou.StoreID,
                    CRMPeriodId = vou.CRMPeriodId,
                    SubTotal = 0,
                    PaymentMethod = vou.PaymentMethod,
                    LastObjectState = vou.LastObjectState,
                    Remark = vou.Remark,
                    ModifiedBy = vou.ModifiedBy,
                    LastModified = vou.LastModified,
                    DateCreated = vou.DateCreated,
                    CreatedBy = vou.CreatedBy,
                    Status = vou.LastObjectState,
                    JobOrderNumber = vou.JobOrderNumber,
                    PlateNo = vou.PlateNo,

                    IsMiddleSendForApproval = vou.IsMiddleSendForApproval,
                    MiddleOnlineApprovedBy = vou.MiddleOnlineApprovedBy,
                    MiddleOnlineApprovedTime = vou.MiddleOnlineApprovedTime,
                    MiddleSendForApprovalBy = vou.MiddleSendForApprovalBy,
                    MiddleSendForApprovalTime = vou.MiddleSendForApprovalTime,
                    IsMiddleOnlineApproved = vou.IsMiddleOnlineApproved,

                    SecondOnlineApprovedBy = vou.SecondOnlineApprovedBy,
                    SecondSendForApprovalTime = vou.SecondSendForApprovalTime,
                    SecondOnlineApprovedTime = vou.SecondOnlineApprovedTime,
                    SecondSendForApprovalBy = vou.SecondSendForApprovalBy,
                    IsSecondSendForApproval = vou.IsSecondSendForApproval,
                    IsSecondOnlineApproved = vou.IsSecondOnlineApproved,
                    SecondVoidBy = vou.SecondVoidBy,
                    SecondVoidTime = vou.SecondVoidTime,
                    IsSecondVoid = vou.IsSecondVoid,

                    IsOnlineApproved = vou.IsOnlineApproved,
                    IsOnlineTransferred = vou.IsOnlineTransferred,
                    IsSendForApproval = vou.IsSendForApproval,
                    OnlineApprovedBy = vou.OnlineApprovedBy,
                    IsOnlineTransferredBy = vou.IsOnlineTransferredBy,
                    SendForApprovalTime = vou.SendForApprovalTime,
                    SendForApprovalBy = vou.SendForApprovalBy,
                    OnlineApprovedTime = vou.OnlineApprovedTime,
                    OnlineTransferredTime = vou.OnlineTransferredTime,

                });
            }
            voulist.OrderByDescending(x => x.SalesQuoteVoucherID);

            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }
        #endregion

        public ActionResult WorkShops_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<ServiceCenterJob> job = new List<ServiceCenterJob>();
            IQueryable<ServiceCenterJob> jobOrders = Enumerable.Empty<ServiceCenterJob>().AsQueryable();
            //var FetchUserId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //if (FetchUserId != null)
            //{
            //    var FetchMechanicCrews = db.MechanicCrew.FirstOrDefault(x => x.EmployeeId == FetchUserId && x.IsActive == true);
            //    if (FetchMechanicCrews != null)
            //    {
            //        jobOrders = db.ServiceCenterJob.Where(x => x.IsOnlineApproved == true && x.IsSentToWorkShop && x.ServiceCenterId.ToString() == FetchMechanicCrews.ServiceCenterId);
            //    }

            //}
            jobOrders = db.ServiceCenterJob;

            DataSourceResult result = jobOrders.ToDataSourceResult(request, j => new {
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
                    IsSentToExternal=j.IsSentToExternal,
                    IsOnlineApproved = j.IsOnlineApproved,
                    IsSendForApproval = j.IsSendForApproval,
                    IsSecondSendForApproval = j.IsSecondSendForApproval,
                    IsOnlineTransferred = j.IsOnlineTransferred,
                    IsSentToWorkShop = j.IsSentToWorkShop,
                    IsSubmittedToServiceCenter = j.IsSubmittedToServiceCenter,
                    OnlineAssignedTime = j.OnlineAssignedTime,
                    OnlineSubmittedTime = j.OnlineSubmittedTime,
                    WorkShopRejectedReason = j.WorkShopRejectedReason,
                    IsSecondOnlineApproved = j.IsSecondOnlineApproved

                });

                return Json(result);
            //}


            //return Json(job.ToDataSourceResult(request));
        }
        public ActionResult WorkShopsForFleet_Read([DataSourceRequest] DataSourceRequest request)
        {
            //List<ServiceCenterJob> job = new List<ServiceCenterJob>();
            //IQueryable<ServiceCenterJob> jobOrders = Enumerable.Empty<ServiceCenterJob>().AsQueryable();

            //jobOrders = db.ServiceCenterJob;
            var jobOrders = db.ServiceCenterJob
    .Where(j => db.Canibalizations
        .Any(c => c.ServiceCenterJobId == j.ServiceCenterJobId.ToString() && c.IsSendForApproval))
    .AsQueryable();
            DataSourceResult result = jobOrders.ToDataSourceResult(request, j => new {
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
                IsSentToExternal = j.IsSentToExternal,
                IsOnlineApproved = j.IsOnlineApproved,
                IsSendForApproval = j.IsSendForApproval,
                IsSecondSendForApproval = j.IsSecondSendForApproval,
                IsOnlineTransferred = j.IsOnlineTransferred,
                IsSentToWorkShop = j.IsSentToWorkShop,
                IsSubmittedToServiceCenter = j.IsSubmittedToServiceCenter,
                OnlineAssignedTime = j.OnlineAssignedTime,
                OnlineSubmittedTime = j.OnlineSubmittedTime,
                WorkShopRejectedReason = j.WorkShopRejectedReason,
                IsSecondOnlineApproved = j.IsSecondOnlineApproved

            });

            return Json(result);
            //}


            //return Json(job.ToDataSourceResult(request));
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

        public JsonResult GetJob()
        {
            var jobOrderDetail = db.JobOrderDetail;

            var job = db.JobOrderDetail.Select(s => new {
                Code = s.JobOrderDetailID,
                Name = s.RepairType.Name+"-"+s.RepairType.Remark

            }).AsQueryable();

            return Json(job, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SubmitWorkShop(Guid Id)
        {
            var serviceJobOrder = _ServiceCenterJobService.GetById(Id);
            var fetchRemark = db.CloseWorkShopWithRemarks.Where(x => x.JobId == Id.ToString()).ToList();
            if(fetchRemark.Any())
            {

                serviceJobOrder.IsSubmittedToServiceCenter = true;
                serviceJobOrder.OnlineSubmittedTime = DateTime.Now;
                serviceJobOrder.Status = MaintenanceConst.ClosedWithRemark;
                _ServiceCenterJobService.Update(serviceJobOrder);

                return Json(new { Message = "Workshop  sent to service center Successfully!", Status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            
            if (serviceJobOrder != null&&!fetchRemark.Any())
            {
                var unCompletedJobs = db.JobOrderServiceCenterDetails.Where(jd => jd.ServiceCenterJobId == serviceJobOrder.ServiceCenterJobId && jd.Status == MaintenanceConst.Completed).Count();
                var unApprovedWorkSheet = db.ServiceCrewWorkLogs.Where(s => s.JobId == serviceJobOrder.ServiceCenterJobId).Count();
                if (unCompletedJobs == 0 || unApprovedWorkSheet == 0)
                {
                    var dt = new
                    {
                        Message = "There are pending job Services or spare parts or work sheet.",
                        Stats = "Error"
                    };
                    return Json(dt, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    serviceJobOrder.Status = MaintenanceConst.SentForClosing;
                    serviceJobOrder.IsSubmittedToServiceCenter = true;
                    serviceJobOrder.OnlineSubmittedTime = DateTime.Now;

                    _ServiceCenterJobService.Update(serviceJobOrder);

                    return Json(new { Message = "Workshop  sent to service center Successfully!", Status = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "Job order not found", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}