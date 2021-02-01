using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{
    [DataObject]
    public class DailyWorkOrderDataSource
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();
        public decimal GetSpareCost(Guid Id)
        {
            decimal cost = 0;
            Guid RequestId = Guid.Empty;
            try
            {
                var JobOrder = _JobOrderService.GetById(Id);
                if (!JobOrder.IsExternal)
                {

                    var Requistions = db.Issues.Where(x => x.JobOrderNo == JobOrder.Num.ToString()).ToList();
                    var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Spare).ToList();
                    foreach (var Requistion in Requistions)
                    {
                        foreach (var Item in Requistion.IssueItems)
                        {
                            foreach (var Map in SpareMap)
                            {
                                if (Map.InventoryCategoryId == Item.ItemCategory.ItemCategoryCode)
                                {
                                    cost = cost + (Item.InventoryItem.AvgPrice * Item.ApprovedQuantity);

                                }
                            }
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
                    var Requistions = db.Issues.Where(x => x.JobOrderNo == JobOrder.Num.ToString()).ToList();
                    var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Battery).ToList();
                    foreach (var Requistion in Requistions)
                    {
                        foreach (var Item in Requistion.IssueItems)
                        {
                            foreach (var Map in SpareMap)
                            {
                                if (Map.InventoryCategoryId == Item.ItemCategory.ItemCategoryCode)
                                {
                                    cost = cost + (Item.InventoryItem.AvgPrice * Item.ApprovedQuantity);

                                }
                            }
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
                    var Requistions = db.Issues.Where(x => x.JobOrderNo == JobOrder.Num.ToString()).ToList();
                    var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Oil_Lubricant).ToList();
                    foreach (var Requistion in Requistions)
                    {
                        foreach (var Item in Requistion.IssueItems)
                        {
                            foreach (var Map in SpareMap)
                            {
                                if (Map.InventoryCategoryId == Item.ItemCategory.ItemCategoryCode)
                                {
                                    cost = cost + (Item.InventoryItem.AvgPrice * Item.ApprovedQuantity);

                                }
                            }
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

                    var Requistions = db.Issues.Where(x => x.JobOrderNo == JobOrder.Num.ToString()).ToList();
                    var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Tyre).ToList();
                    foreach (var Requistion in Requistions)
                    {
                        foreach (var Item in Requistion.IssueItems)
                        {
                            foreach (var Map in SpareMap)
                            {
                                if (Map.InventoryCategoryId == Item.ItemCategory.ItemCategoryCode)
                                {
                                    cost = cost + (Item.InventoryItem.AvgPrice * Item.ApprovedQuantity);

                                }
                            }
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

                    var Requistions = db.Issues.Where(x => x.JobOrderNo == JobOrder.Num.ToString()).ToList();
                    var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == SpareType.Other).ToList();
                    foreach (var Requistion in Requistions)
                    {
                        foreach (var Item in Requistion.IssueItems)
                        {
                            foreach (var Map in SpareMap)
                            {
                                if (Map.InventoryCategoryId == Item.ItemCategory.ItemCategoryCode)
                                {
                                    cost = cost + (Item.InventoryItem.AvgPrice * Item.ApprovedQuantity);

                                }
                            }
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
                var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();
                if (Request != null)
                {
                    RequestId = Request.JobRequestId;
                }
                var Services = db.JobOrderDetail.Where(x => x.JobOrderID == Id || x.JobOrderID == RequestId).ToList();
                foreach (var Service in Services)
                {
                    var Spares = db.SalvageStore.Where(x => x.JobId == Service.JobOrderDetailID).ToList();
                    if (Spares != null)
                    {
                        cost = cost + (Spares.Sum(x => x.Price) * Spares.Sum(x => x.ApprovedQuantity));
                    }

                }

                return cost;
            }
            catch (Exception ee)
            {

                return 0;
            }
        }



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderViewModel> GetJobList(DateTime Date, string Location, string Category, string Type, string PlateNo, string Model, string ChasisNo, string Serial  )
        {
            IJobOrderDetailService service = new JobOrderDetailService();
            IJobOrderService JService = new JobOrderService();
            IFleetService fservice = new FleetService();
            IOperatorService oservice = new OperatorService();
            List<JobOrderViewModel> WorkOrderList = new List<JobOrderViewModel>();
            JobOrderViewModel OrderViewModel;
            List<ExceedERP.Core.Domain.FleetManagement.FleetMaintenance.JobOrder> Jobs = new List<ExceedERP.Core.Domain.FleetManagement.FleetMaintenance.JobOrder>(); ;
            var Fleets = fservice.GetAll();
            var operators = oservice.GetAll();

            decimal TotalTireCost = 0;
            decimal TotalSpareCost = 0;
            decimal TotalOilLubricantCost = 0;
            decimal TotalOtherCost = 0;
            decimal TotalLabourCost = 0;
            double TotalHr = 0;
            decimal TotalSalvageCost = 0;

            #region Company SharedDataSource
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
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            #endregion

               if (true)
            {
                //.Where(so => so.IsSecondOnlineApproved == false)
                Jobs = JService.GetAll().ToList();


                string ChassisNum = "Not Set";
                string EngineNum = "Not Set";
                string OdoMeter = "Not Set";
                string Equipment = "External Equipment";
                foreach (var job in Jobs)
                {
                    TotalTireCost = 0;
                    TotalSpareCost = 0;
                    TotalOilLubricantCost = 0;
                    TotalOtherCost = 0;
                    TotalLabourCost = 0;
                    TotalHr = 0;
                    TotalSalvageCost = 0;

                    var Requests = db.JobRequest.Where(x => x.Num == job.Referance).FirstOrDefault();

                    List<JobOrderDetail> lineItems = new List<JobOrderDetail>();
                    if (Requests != null)
                    {
                        lineItems = service.GetAll().Where(x => x.JobOrderID == job.JobOrderID || x.JobOrderID == Requests.JobRequestId).ToList();
                    }
                    else
                    {
                        lineItems = service.GetAll().Where(x => x.JobOrderID == job.JobOrderID).ToList();
                    }

                    TotalLabourCost = GetLabourCost(job.JobOrderID);
                    TotalSpareCost = GetSpareCost(job.JobOrderID);
                    TotalTireCost = GetTyreSpareCost(job.JobOrderID);
                    TotalSalvageCost = GetSalvageSpareCost(job.JobOrderID);
                    TotalOilLubricantCost = GetOilSpareCost(job.JobOrderID);
                    TotalOtherCost = GetOtherSpareCost(job.JobOrderID);






                    var fleet = Fleets.Where(y => y.FleetsID == job.FleetsID).FirstOrDefault(); //== l.ThingID).FirstOrDefault();

                    OrderViewModel = new JobOrderViewModel();
                    if (fleet != null)
                    {
                        OrderViewModel.Manufacture = fleet.Manufacturer;
                        OrderViewModel.Model = fleet.Model;
                        OrderViewModel.ChassisNumber = fleet.ChasisNumber;
                        OrderViewModel.EngineNumber = fleet.EngineNumber;
                        OrderViewModel.OperatorName = fleet.OperatorName;
                        OrderViewModel.ProjectName = fleet.Location;
                        OdoMeter = fleet.CurentOdoMeter.ToString();
                    }


                    OrderViewModel.JobOrderNum = job.Num;
                    OrderViewModel.Type = job.Type;
                    OrderViewModel.InspectedBy = job.InspectedBy;
                    OrderViewModel.MaintenanceCenter = job.MaintenanceCenter;
                    OrderViewModel.DateCompleted = job.DateCompleted;
                    OrderViewModel.Status = job.Status;
                    OrderViewModel.IsPM = job.IsPM ? "✓" : ""; ;
                    OrderViewModel.IsM = job.IsPM ? "" : "✓";
                    OrderViewModel.Duration = TotalHr;
                    OrderViewModel.TyreCost = TotalTireCost;
                    OrderViewModel.SpareCost = TotalSpareCost;
                    OrderViewModel.SalvageCost = TotalSalvageCost;
                    OrderViewModel.OilCost = TotalOilLubricantCost;
                    OrderViewModel.LabourCost = TotalLabourCost;
                    OrderViewModel.TotalCost = TotalOtherCost + TotalLabourCost + TotalOilLubricantCost + TotalSalvageCost + TotalSpareCost + TotalTireCost;


                    OrderViewModel.AmharicName = AmharicName;
                    OrderViewModel.Name = Name;
                    OrderViewModel.Address = Address;
                    OrderViewModel.LogoFileName = LogoFileName;

                    WorkOrderList.Add(OrderViewModel);

                }
            }


            return WorkOrderList;
        }



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderViewModel> GetJobPending(DateTime Date, string Location)
        {
            IJobOrderDetailService service = new JobOrderDetailService();
            IJobOrderService JService = new JobOrderService();
            IFleetService fservice = new FleetService();
            IOperatorService oservice = new OperatorService();
            List<JobOrderViewModel> WorkOrderList = new List<JobOrderViewModel>();
            JobOrderViewModel OrderViewModel;
            List<ExceedERP.Core.Domain.FleetManagement.FleetMaintenance.JobOrder> Jobs = new List<ExceedERP.Core.Domain.FleetManagement.FleetMaintenance.JobOrder>(); ;
          


            if (Date != null)
            {
                Jobs = _JobOrderService.GetAll();


                //.Where(x => x.IsSecondOnlineApproved == false && x.DateCreated >  Date)
                foreach (var job in Jobs)
                {




                    OrderViewModel = new JobOrderViewModel();

                    OrderViewModel.JobOrderNum = job.Num;
                    OrderViewModel.OperatorName = job.AdvanceMeasure;
                    OrderViewModel.ProjectName = job.FailureReason;


                    WorkOrderList.Add(OrderViewModel);

                }
            }


            return WorkOrderList;
        }


    }

}


