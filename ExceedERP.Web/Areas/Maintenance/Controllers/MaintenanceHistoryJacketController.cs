using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetViewModel;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
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
    public class MaintenanceHistoryJacketController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
         private readonly CacheData CacheData = new CacheData();
        // GET: FleetManagement/FleetEquipmentHistoryJacket
        public ActionResult Index()
        {
           
            ViewData["Category"] = CacheData.GetEquipmentIdentityCaches().Select(
       i => new
       {
           Text = i.Name,
           Value = i.Id
       }).AsQueryable();
            ViewData["Type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["Name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Text = s.PlateNumber,
                Value = s.FleetsID
            }).ToList();
            return View();
        }

        public ActionResult Equipment_Read([DataSourceRequest] DataSourceRequest request)
        {
            var equip = db.Fleets.AsQueryable();
            var equipmentHistoryJacket = equip.Select(s => new FleetEquipmentHistoryJacketViewModel
            {
                FleetId = s.FleetsID,
                EquipmentCategoryName = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == s.EquipmentIdentityID).EquipmentCategoryName,
                EquipmentTypeName = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == s.EquipmentTypeID).EquipmentTypeName,
                EquipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID).Name,
                PlateNo = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetsID).PlateNumber,
                PurchasingCost = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetsID).PurchasingCost.ToString(),
                SerialNo = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetsID).SerialNo,
                PlateNumber = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetsID).ChasisNumber
            });
            return Json(equipmentHistoryJacket.ToDataSourceResult(request, ModelState));
        }
        public ActionResult JobRequest_Read([DataSourceRequest] DataSourceRequest request,string id)
        {
            var Plate = db.Fleets.FirstOrDefault(x => x.FleetsID.ToString() == id).PlateNumber;
            var JobRequests = db.JobRequest.Where(x => x.LicensePlate == Plate).AsQueryable();
            var JobRequestHistoryJacket = JobRequests.Select(x => new JobRequestVM {
                JobRequestId = x.JobRequestId.ToString(),
                RequestNumber = x.RequestNumber,
                CustomerName = db.OrganizationCustomers.FirstOrDefault(c => c.OrganizationCustomerID == x.CustomerId).TradeName,
                IsPM = x.IsPM,
                Source = x.Source.ToString(),
                OperatorName = x.OperatorName,
                Odometer = x.Odometer,
                EstmatedCostAmount = x.EstmatedCostAmount,
                MaintenanceCenter = x.MaintenanceCenter,
                BranchName = db.Branch.FirstOrDefault(b => b.BranchId == x.BranchId).OrgBranchName,
                BusinessUnit = db.BusinessUnits.FirstOrDefault(b => b.BusinessUnitId == x.BusinessUnitId).Name,
                JobOrderStatus = x.JobOrderStatus.ToString(),
                DeliveredDate = DbFunctions.TruncateTime(x.DeliveredDate),
                DateCreated = DbFunctions.TruncateTime(x.DateCreated),
               
            });

            return Json(JobRequestHistoryJacket.ToDataSourceResult(request, ModelState));
        }
        public ActionResult JobOrder_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            //var JobOrders = db.JobOrder.Where(x => x.FleetsID == id).AsQueryable();
            var JobOrders = db.JobOrder.Where(x => x.FleetsID == id).ToList();
            var JobRequestHistoryJacket = JobOrders.Select(x => new JobOrderVM {
            FleetId=x.FleetsID,
            JobOrderId=x.JobOrderID,
            Description=GetCauses(x.JobOrderID),
            MaintenanceCenter=GetMaintenanceCenter(x.JobOrderID),
            DateIn=x.OnlineApprovedTime,
            DateClosed=x.SecondOnlineApprovedTime,
            RequisitionDate=GetRequistionDate(x.JobOrderID),
            Num=x.Num
            });
            return Json(JobRequestHistoryJacket.ToDataSourceResult(request, ModelState));

        }
        private string GetCauses(Guid JobOrderId)
        {
            string Causes = null;
            var fetchJobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == JobOrderId);
             if(fetchJobOrder!=null)
            {
                var fetchRequest = db.JobRequest.FirstOrDefault(x => x.Num == fetchJobOrder.Referance);
                if(fetchRequest!=null)
                {
                    var fetchCauses = db.JobCauses.Where(x => x.OrderId == fetchRequest.JobRequestId).ToList();
                    foreach(var cause in fetchCauses)
                    {
                        Causes = Causes + " " + cause.Description;
                    }
                }
            }
            

            return Causes;
        }
        private string GetMaintenanceCenter(Guid JobOrderId)
        {
            var fetchJobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == JobOrderId);
            var center = fetchJobOrder.MaintenanceCenter;
            return center;
        }
        public ActionResult ServiceCenterJob_Read([DataSourceRequest] DataSourceRequest request, Guid JobOrderId)
        {
            // Fetch data from database first (step 1)
            var fetchServiceJob = db.ServiceCenterJob
                .Where(x => x.JobOrderID == JobOrderId)
                .ToList();  

            var selectedJobs = fetchServiceJob.Select(x => new JobServiceVM
            {
                ServiceCenterJobId = x.ServiceCenterJobId,
                SparePart = GetSparePart(x.ServiceCenterJobId),
                UOM=GetUnitOfMeas(x.ServiceCenterJobId),
                ServiceGroup=GetServiceGroup(x.ServiceCenterJobId),
                Qty=GetQuantity(x.ServiceCenterJobId),
                WorkExcuted=GetWorkExcecuted(x.ServiceCenterJobId),
                UnitPrice=GetUnitPrice(x.ServiceCenterJobId),
                LabourCost=GetLabourCost(x.ServiceCenterJobId),
                AssignedTechnician=GetAssignedTech(x.ServiceCenterJobId),
                TotalPrice=GetTotalPrice(x.ServiceCenterJobId),
                Forman=GetForman(x.SecondOnlineApprovedBy)
            }).ToList();  // Convert to List after processing

            return Json(selectedJobs.ToDataSourceResult(request, ModelState));
        }
        private string GetForman(string Approver)
        {
            string formanName = null;          
                formanName= EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(Approver));
            return formanName;
        }
        private string GetLabourCost(Guid JobId)
        {
            string labourcost = null;
            var fetchServiceDetail = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == JobId);
              if(fetchServiceDetail!=null)
            {
                labourcost = db.RepairType.FirstOrDefault(x => x.RepairTypeID == fetchServiceDetail.RepairTypeID)?.LabourRateAtCompany.ToString()??"";

            }
            return labourcost;
        }
        private string GetAssignedTech(Guid JobId)
        {
            string labourcost = null;
            var fetchServiceDetail = db.ServiceCrewWorkLogs.Where(x => x.JobId == JobId);
         foreach(var crew in fetchServiceDetail)
            {
                 var crewName=db.MechanicCrew.FirstOrDefault(x => x.MechanicCrewID == crew.MecanicId);
                if(crewName!=null)
                {
                    var fetchFullName = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == crewName.EmployeeId);
                    if(fetchFullName!=null)
                    {
                        labourcost = labourcost + " & " + fetchFullName.FirstNameEnglish + "_" + fetchFullName.MiddleNameEnglish + "_" + fetchFullName.LastNameEnglish;
                    }
                }
            }
            return labourcost;
        }
        private string GetSparePart(Guid JobId)
        {
            var fetchStoreRequisition = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == JobId.ToString());
            string ItemNames = null;
                if(fetchStoreRequisition!=null)
            {
                var fetchIssue = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == fetchStoreRequisition.StoreRequisitionID);
                 if(fetchIssue!=null)
                {
                    var fetchItem = db.IssueItems.Where(x => x.IssueID == fetchIssue.IssueID).ToList();
                    foreach(var item in fetchItem)
                    {
                        var fetchItemName = db.InventoryItems.FirstOrDefault(x => x.ItemCategoryCode == item.ItemCategoryCode && x.ItemCode == item.ItemCode);
                        if(fetchItemName!=null)
                        {
                            ItemNames = ItemNames + " " + fetchItemName.Name;
                        }
                        
                    }
                }
            }


            return ItemNames;
        }
        private string GetUnitPrice(Guid JobId)
        {
            var fetchStoreRequisition = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == JobId.ToString());
            string ItemNames = null;
                if(fetchStoreRequisition!=null)
            {
                var fetchIssue = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == fetchStoreRequisition.StoreRequisitionID);
                 if(fetchIssue!=null)
                {
                    var fetchItem = db.IssueItems.Where(x => x.IssueID == fetchIssue.IssueID).ToList();
                    foreach(var item in fetchItem)
                    {
                        ItemNames = ItemNames + " " + item.Name+"_"+item.UnitPrice;
                    }
                }
            }


            return ItemNames;
        }
        private decimal GetTotalPrice(Guid JobId)
        {
            var fetchStoreRequisition = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == JobId.ToString());
            decimal ItemNames = 0;
                if(fetchStoreRequisition!=null)
            {
                var fetchIssue = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == fetchStoreRequisition.StoreRequisitionID);
                 if(fetchIssue!=null)
                {
                    var fetchItem = db.IssueItems.Where(x => x.IssueID == fetchIssue.IssueID).ToList();
                    foreach(var item in fetchItem)
                    {
                        ItemNames = ItemNames +item.UnitPrice*item.IssuedQuantity;
                    }
                }
            }


            return ItemNames;
        }
        public string GetRequistionDate(Guid JobOrderId)
        {
            string Causes = null;
            var fetchJobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == JobOrderId);
            if (fetchJobOrder != null)
            {
                var fetchRequest = db.JobRequest.FirstOrDefault(x => x.Num == fetchJobOrder.Referance);
                if (fetchRequest != null)
                {
                    Causes = fetchRequest.DateCreated.ToString();
                }
            }
            return Causes;
        }
        private string GetWorkExcecuted(Guid JobId)
        {
            var fetchServiceName = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == JobId);
            string ItemNames = null;
            if (fetchServiceName!=null)
            {
                 ItemNames = db.RepairType.FirstOrDefault(x => x.RepairTypeID == fetchServiceName.RepairTypeID)?.Name ?? " ";

            }
            return ItemNames;
        }
        private string GetServiceGroup(Guid JobId)
        {
            var fetchServiceName = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == JobId);
            string ItemNames = null;
            if (fetchServiceName!=null)
            {
                 ItemNames = db.RepairType.FirstOrDefault(x => x.RepairTypeID == fetchServiceName.RepairTypeID)?.ServiceGroup.ToString() ?? " ";
            }
            return ItemNames;
        }

        private string GetUnitOfMeas(Guid JobId)
        {
            var fetchStoreRequisition = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == JobId.ToString());
            string ItemNames = null;
            if (fetchStoreRequisition != null)
            {
                var fetchIssue = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == fetchStoreRequisition.StoreRequisitionID);
                if (fetchIssue != null)
                {
                    var fetchItem = db.IssueItems.Where(x => x.IssueID == fetchIssue.IssueID).ToList();
                    foreach (var item in fetchItem)
                    {
                        var fetchInventoryItem = db.InventoryItems.FirstOrDefault(x => x.ItemCode == item.ItemCode && x.ItemCategoryCode == item.ItemCategoryCode);
                        if (fetchInventoryItem != null)
                        {
                            ItemNames = ItemNames + " " + item.Name+"_"+ item.InventoryItem.UoMName;
                           
                        }

                    }
                }
            }


            return ItemNames;
        }
        private string GetQuantity(Guid JobId)
        {
            var fetchStoreRequisition = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == JobId.ToString());
            string ItemNames =null;
            if (fetchStoreRequisition != null)
            {
                var fetchIssue = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == fetchStoreRequisition.StoreRequisitionID);
                if (fetchIssue != null)
                {
                    var fetchItem = db.IssueItems.Where(x => x.IssueID == fetchIssue.IssueID).ToList();
                    foreach (var item in fetchItem)
                    {
                        ItemNames = ItemNames + " " + item.Name+"_"+item.IssuedQuantity;
                    }
                }
            }


            return ItemNames;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}