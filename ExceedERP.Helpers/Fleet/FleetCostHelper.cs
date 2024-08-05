using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;

using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.Fleet
{

    #region Const

    public class MaintenanceCostHelperConst
    {
        public const string ABC = "ABCDEFGHIJKLMNOPQRSTVXYZ";
        public const string Rent = "Rent";
        public const string Closed = "Closed";
        public const string Open = "Open";
        public const string Received = "Received";
        public const string Company = "Company";
        public const string Internal = "INTERNAL";
        public const string EXTERNAL = "EXTERNAL";
        public const string Request = "Pending";
        public const string Rejected = "Rejected";
        public const string R = "Request";

        public const string Completed = "Completed";

        public const string ACTIVE = "ACTIVE";
        public const string ONDUTY = "On Duty";
        public const string OnMaintenance = "On Maintenance";

        public const string WaitingTechnical = "Waiting Technical Approval";
        public const string WaitingAdminstrative = "Waiting Administrative Approval";
        public const string Approved = "Approved";
        public const string WaitingApproval = "Waiting Approval";

        public const string Day = "Day";
        public const string Week = "Week";
        public const string Month = "Month";
        public const string Year = "Year";

        public const string Planned = "Planned";
        public const string UnPlanned = "UnPlanned";
        public const string Male = "Male";
        public const string Female = "Female";

        public const string Pending = "Pending";
        public const string Finished = "Finished";

        public const string SINO = "SINO";
        public const string NISSAN = "NISSAN";
        public const string TOYOTA = "TOYOTA";

        public const string BLACK = "BLACK";
        public const string BLUE = "BLUE";
        public const string RED = "RED";
        public const string WHITE = "WHITE";



        public const string Issue = "Issue";
        public const string GoodReceive = "Good Receive";

        public const string RentRequest = "Rent Request";
        public const string CompanyRequest = "Company Request";

    }
    #endregion
    public class FleetCostHelper
    {
        private ExceedDbContext db = new ExceedDbContext();
        private IJobOrderService _JobOrderService = new JobOrderService();
        private IJobOrderDetailService _JobOrderDetailService = new JobOrderDetailService();
        public decimal GetSpareTypeCost(SpareType spareType, Guid Id)
        {
            decimal Cost = 0;
            Guid RequestId = Guid.Empty;
            try
            {
                var JobOrder = _JobOrderService.GetById(Id);
                if (!JobOrder.IsExternal && JobOrder != null)
                {
                    var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
                    foreach (var StoreRequestion in StoreRequestions)
                    {
                        var Requistions = db.IssueItems.AsNoTracking().Where(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
                        var SpareMap = db.InventorySparePartMapping.Where(x => x.SpareType == spareType).ToList();
                        foreach (var Map in SpareMap)
                        {

                            foreach (var Requistion in Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId))
                            {


                                if (Requistion.InventoryItem.AvgPrice != 0)
                                {
                                    Cost = Cost + (Requistion.InventoryItem.AvgPrice * Requistion.ApprovedQuantity);
                                }

                                if (Requistion.InventoryItem.TotalPrice != 0)
                                {
                                    Cost = Cost + (Requistion.InventoryItem.TotalPrice * Requistion.ApprovedQuantity);
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
                        var SparesInternal = db.ServiceSpare.Where(x => x.ServiceID == Service.JobOrderDetailID && x.Type == spareType).ToList();
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
                                Cost = Cost + (InventoryItem.AvgPrice * Spare.UsedQuantity);
                            }
                        }
                        var Spares = db.ExternalMaterial
                            .Where(x => x.JobId == Service.JobOrderDetailID && x.Type == spareType).ToList();
                        Cost = Cost + Spares.Sum(x => x.TotalAmount);

                    }
                }

                return Cost;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public decimal GetEquipmentSpareCost(List<JobOrder> JobOrders, List<InventorySparePartMapping> Mapping, SpareType SpareType)
        {
            FleetReportHelper fleetReportHelper = new FleetReportHelper();

            decimal Cost = decimal.Zero;

            foreach (var JobOrder in JobOrders)
            {
                var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
                foreach (var StoreRequestion in StoreRequestions)
                {
                    var SpareMap = Mapping.Where(x => x.SpareType == SpareType).ToList();

                    var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
                    foreach (var Map in SpareMap)
                    {
                        foreach (var Item in Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId))
                        {



                            //if (Item.InventoryItem.AvgPrice != 0)
                            //{
                            //    Cost = Cost + (Item.InventoryItem.AvgPrice * Item.ApprovedQuantity);
                            //}

                            //else
                            //{
                            //    Cost = Cost + (Item.InventoryItem.TotalPrice * Item.ApprovedQuantity);
                            //}
                            Cost = Cost + Item.Total;
                        }
                    }
                }
            }

            return Cost;
        }
        public decimal GetEquipmentSpareCost(Fleets fleet, List<InventorySparePartMapping> Mapping, SpareType SpareType)
        {
            FleetReportHelper fleetReportHelper = new FleetReportHelper();

            decimal Cost = decimal.Zero;
            var JobOrders = db.JobOrder.Where(x => x.FleetsID == fleet.FleetsID || x.EquipmentNo == fleet.PlateNumber).ToList();
            foreach (var JobOrder in JobOrders)
            {
                var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString() || x.PlateNo == fleet.PlateNumber);
                foreach (var StoreRequestion in StoreRequestions)
                {
                    var SpareMap = Mapping.Where(x => x.SpareType == SpareType).ToList();
                    var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
                    foreach (var Map in SpareMap)
                    {
                        var Item = Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId);


                        Cost = Cost + Item.Sum(x => x.Total);
                    }
                }
            }


            return Cost;
        }

        public decimal GetSalvageSpareCost(Guid Id)
        {
            decimal cost = 0;
            Guid RequestId = Guid.Empty;
            IJobOrderService _JobOrderService = new JobOrderService();
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
            catch (Exception)
            {

                return 0;
            }
        }
        public decimal GetSalvageSpareCost(Fleets fleet)
        {
            decimal cost = 0;
            Guid RequestId = Guid.Empty;
            IJobOrderService _JobOrderService = new JobOrderService();
            try
            {
                var JobOrders = _JobOrderService.GetAll().Where(x => x.FleetsID == fleet.FleetsID).ToList();
                foreach (var JobOrder in JobOrders)
                {

                    var Spares = db.SalvageStore.Where(x => x.JobId == JobOrder.JobOrderID).ToList();
                    if (Spares != null)
                    {
                        cost = cost + (Spares.Sum(x => x.Price) * Spares.Sum(x => x.ApprovedQuantity));
                    }
                }

                return cost;
            }
            catch (Exception)
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
            catch (Exception)
            {
                return 0;
            }
            return Quantity * UnitPrice;
        }

        public decimal GetSingleJobLabourCost(Guid Id)
        {
            ExceedDbContext db = new ExceedDbContext();
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            FleetMaintenaceHelper fleetMaintenaceHelper = new FleetMaintenaceHelper();
            try
            {
                decimal Cost = 0;

                var MechanicLogs = db.ServiceCrewWorkLogs.Where(x => x.JobId == Id).ToList();
                IMechanicCrewService mechanicCrewService = new MechanicCrewService();



                foreach (var log in MechanicLogs.DistinctByField(x => x.MecanicId))
                {
                    var Crew = new ServiceCrew();
                    var Mechanic = mechanicCrewService.GetById(log.MecanicId);

                    if (Mechanic != null)
                    {
                        var MechanicLog = MechanicLogs.Where(x => x.MecanicId == log.MecanicId).ToList();

                        Cost += decimal.Parse(fleetMaintenaceHelper.GetMechanicDuration(MechanicLog).ToString()) * Mechanic.HourlyWage;

                    }
                }
                

                return Cost;
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
                    RequestOrderLine = _JobOrderDetailService.GetAll().Where(x => x.JobOrderID == Requests.JobRequestId).ToList().Count;
                    RequestOrderLineFinished = _JobOrderDetailService.GetAll().Where(x => x.JobOrderID == Requests.JobRequestId && x.Status == MaintenanceCostHelperConst.Completed).ToList().Count;
                }
                decimal OrderLine = _JobOrderDetailService.GetAll().Where(x => x.JobOrderID == Id).ToList().Count;
                decimal OrderLineFinished = _JobOrderDetailService.GetAll().Where(x => x.JobOrderID == Id && x.Status == MaintenanceCostHelperConst.Completed).ToList().Count;
                var progress = (OrderLineFinished + RequestOrderLineFinished) / (OrderLine + RequestOrderLine);

                progress = progress * 100;
                return ((int)progress).ToString();

            }
            catch (Exception)
            {

                return "0";
            }
        }
        public decimal GetEfficiency(Guid Id)
        {
            ExceedDbContext db = new ExceedDbContext();
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            decimal duration = 1;
            decimal Rate = 0;
            decimal Efficiency = 0;
            Guid RequestId = Guid.Empty;

            try
            {

                var Services = db.JobOrderDetail.Where(x => x.JobOrderID == Id || x.JobOrderID == RequestId).ToList();
                foreach (var Service in Services)
                {

                    var Crews = _ServiceCrewService.GetAll().Where(x => x.ServiceID == Service.ServiceID).ToList();
                    foreach (var Crew in Crews)
                    {

                        var WorkLogs = db.ServiceCrewWorkLogs.Where(x => x.MecanicId == Crew.MechanicCrewID && x.IsOnlineApproved == true && x.JobOrderDetailId == Service.JobOrderDetailID).ToList();

                        foreach (var WorkLog in WorkLogs)
                        {
                            duration = decimal.Parse(((DateTime.Parse(WorkLog.WorkingHoursMorningto.ToString()) - DateTime.Parse(WorkLog.WorkingHoursMorningFrom.ToString())).TotalMinutes) + ((DateTime.Parse(WorkLog.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(WorkLog.WorkingHoursAfternoonFrom.ToString())).TotalMinutes).ToString());

                        }
                    }
                }



                foreach (var service in Services)
                {
                    var Std = db.Service.Where(x => x.ServiceName == service.RepairTypeID.ToString()).FirstOrDefault();
                    if (Std != null)
                    {
                        Rate = Rate + decimal.Parse(Std.STDTime.ToString());
                    }

                }
                Efficiency = (Rate / duration.DivideByZeroHelper()) * 100;
                Efficiency = Efficiency.TranslateBnOneAndHundered();

                return Efficiency;
            }
            catch (Exception)
            {

                return 0;
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



        public decimal GetPettyCashByFleet(Guid FleetId)
        {
            ExceedDbContext db = new ExceedDbContext();


            var Pettys = db.FleetPettyCashes.Where(x => x.FleetsID == FleetId).ToList();
            if (Pettys.Any())
            {
                return Pettys.Sum(x => x.Amount);
            }
            else
            {
                return decimal.Zero;
            }
        }
        public decimal GetEquipmentCost(List<JobOrder> JobOrders, List<InventorySparePartMapping> Mapping)
        {
            FleetReportHelper fleetReportHelper = new FleetReportHelper();
            decimal Cost = decimal.Zero;

            foreach (var JobOrder in JobOrders)
            {
                var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
                foreach (var StoreRequestion in StoreRequestions)
                {
                    var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
                    foreach (var Map in Mapping)
                    {
                        foreach (var Item in Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId))
                        {


                            //if (Item.InventoryItem.AvgPrice != 0)
                            //{
                            //    Cost = Cost + (Item.InventoryItem.AvgPrice * Item.ApprovedQuantity);
                            //}

                            //else
                            //{
                            //    Cost = Cost + (Item.InventoryItem.TotalPrice * Item.ApprovedQuantity);
                            //}

                            Cost = Cost + Item.Total;
                        }
                    }
                }
            }

            return Cost;
        }

        public decimal GetJobOrderSpareCost(JobOrder JobOrder)
        {

            FleetReportHelper fleetReportHelper = new FleetReportHelper();

            decimal Cost = decimal.Zero;

            //var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
            //foreach (var StoreRequestion in StoreRequestions)
            //{
            //    var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
            //    var SpareMap = Mapping.Where(x => x.SpareType == SpareType).ToList();
            //    foreach (var Map in SpareMap)
            //    {
            //        var Item = Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId).ToList();


            //        Cost = Cost + Item.Sum(x => x.Total);


            //    }
            //}
            var sparePartsAndMaterials = db.ServiceSpare.Where(s => s.ServiceID == JobOrder.JobOrderID).ToList();
            if (sparePartsAndMaterials.Any())
            {
                Cost = sparePartsAndMaterials.Sum(s => s.TotalPrice);
            }


            return Cost;
        }
        public decimal GetEquipmentTotalServiceCost(List<JobOrder> JobOrders, List<InventorySparePartMapping> Mapping)
        {
            FleetReportHelper fleetReportHelper = new FleetReportHelper();
            IFleetService fleetService = new FleetService();
            decimal Cost = decimal.Zero;
            string plateNo = "3-";



            foreach (var JobOrder in JobOrders)
            {
                var fleet = fleetService.GetById(JobOrder.FleetsID);

                if (fleet != null)
                {
                    plateNo = fleet.PlateNumber;
                }
                var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
                foreach (var StoreRequestion in StoreRequestions)
                {
                    var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID || x.Issue.PlateNo == plateNo).ToList();
                    foreach (var Map in Mapping)
                    {
                        foreach (var Item in Requistions)
                        {



                            Cost = Cost + Item.Total;
                        }
                    }
                }
            }

            return Cost;
        }
        public decimal GetJobOrderSpareCost(JobOrder JobOrder, List<InventorySparePartMapping> Mapping, SpareType SpareType)
        {

            FleetReportHelper fleetReportHelper = new FleetReportHelper();

            decimal Cost = decimal.Zero;

            var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
            foreach (var StoreRequestion in StoreRequestions)
            {
                var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
                var SpareMap = Mapping.Where(x => x.SpareType == SpareType).ToList();
                foreach (var Map in SpareMap)
                {
                    var Item = Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId).ToList();


                    Cost = Cost + Item.Sum(x => x.Total);


                }
            }



            return Cost;
        }


        public decimal GetJobOrderSpareCost(JobOrder JobOrder, List<InventorySparePartMapping> Mapping)
        {

            FleetReportHelper fleetReportHelper = new FleetReportHelper();

            decimal Cost = decimal.Zero;

            var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == JobOrder.Num.ToString() || x.JobOrderNo == JobOrder.JobOrderID.ToString());
            foreach (var StoreRequestion in StoreRequestions)
            {
                var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
                foreach (var Map in Mapping)
                {
                    foreach (var Item in Requistions.Where(x => x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId))
                    {


                        //if (Item.InventoryItem.AvgPrice != 0)
                        //{
                        //    Cost = Cost + (Item.InventoryItem.AvgPrice * Item.ApprovedQuantity);
                        //}

                        //else
                        //{
                        //    Cost = Cost + (Item.InventoryItem.TotalPrice * Item.ApprovedQuantity);
                        //}


                        Cost = Cost + Item.Total;
                    }
                }
            }



            return Cost;
        }


        public decimal GetSingleEquipmentExternalRentCost(RentFromExternal RentFromExternals, Guid FleetId)
        {
            IFleetService _FleetService = new FleetService();
            ITimeSheetService _TimeSheetService = new TimeSheetService();
            var Fleet = _FleetService.GetAll().Where(x => x.RentId == FleetId).FirstOrDefault();
            decimal TotalWorkingCost = decimal.Zero;

            if (Fleet != null)
            {

                List<TimeSheet> TimeSheets = _TimeSheetService.GetAll().Where(x => x.FleetsID == Fleet.FleetsID).ToList();
                decimal Workingtime = decimal.Zero;
                decimal Workingcost = decimal.Zero;

                foreach (var Work in TimeSheets)
                {
                    TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) - DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                    TimeSpan AfterNoonDuration = DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());

                    Workingtime = Workingtime + (MorningDuration.Hours + AfterNoonDuration.Hours);
                    Workingcost = Workingtime * decimal.Parse(Work.RentalRate.ToString());


                }
                TotalWorkingCost = TotalWorkingCost + Workingcost;

            }
            return TotalWorkingCost;
        }

        public decimal GetLabourCost(JobOrder JobOrder)
        {
            ExceedDbContext db = new ExceedDbContext();
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            FleetMaintenaceHelper fleetMaintenaceHelper = new FleetMaintenaceHelper();
            try
            {
                decimal Cost = 0;


                var MechanicLogs = db.ServiceCrewWorkLogs.Where(x => x.JobId == JobOrder.JobOrderID).ToList();
                IMechanicCrewService mechanicCrewService = new MechanicCrewService();



                foreach (var log in MechanicLogs.DistinctByField(x => x.MecanicId))
                {
                    var Crew = new ServiceCrew();
                    var Mechanic = mechanicCrewService.GetById(log.MecanicId);

                    if (Mechanic != null)
                    {
                        var MechanicLog = MechanicLogs.Where(x => x.MecanicId == log.MecanicId).ToList();

                        Cost += decimal.Parse(fleetMaintenaceHelper.GetMechanicDuration(MechanicLog).ToString()) * Mechanic.HourlyWage;

                    }
                }

                return Cost;
            }

            catch (Exception)
            {

                return 0;
            }
        }
        public decimal GetSingleJobLabourCost(JobOrder JobOrder)
        {
            ExceedDbContext db = new ExceedDbContext();
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            FleetMaintenaceHelper fleetMaintenaceHelper = new FleetMaintenaceHelper();
            try
            {
                decimal Cost = 0;


                var MechanicLogs = db.ServiceCrewWorkLogs.Where(x => x.JobId == JobOrder.JobOrderID).ToList();
                IMechanicCrewService mechanicCrewService = new MechanicCrewService();



                foreach (var log in MechanicLogs.DistinctByField(x => x.MecanicId))
                {
                    var Crew = new ServiceCrew();
                    var Mechanic = mechanicCrewService.GetById(log.MecanicId);

                    if (Mechanic != null)
                    {
                        var MechanicLog = MechanicLogs.Where(x => x.MecanicId == log.MecanicId).ToList();

                        Cost += decimal.Parse(fleetMaintenaceHelper.GetMechanicDuration(MechanicLog).ToString()) * Mechanic.HourlyWage;

                    }
                }

                return Cost;
            }
            catch(Exception e)
            {
                return 0;
            }
            
        }

        public decimal GetLabourCost(List<JobOrder> JobOrders)
        {
            ExceedDbContext db = new ExceedDbContext();
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            FleetMaintenaceHelper fleetMaintenaceHelper = new FleetMaintenaceHelper();
            try
            {
                decimal Cost = 0;
                foreach (var JobOrder in JobOrders)
                {


                    var MechanicLogs = db.ServiceCrewWorkLogs.Where(x => x.JobId == JobOrder.JobOrderID).ToList();
                    IMechanicCrewService mechanicCrewService = new MechanicCrewService();



                    foreach (var log in MechanicLogs.DistinctByField(x => x.MecanicId))
                    {
                        var Crew = new ServiceCrew();
                        var Mechanic = mechanicCrewService.GetById(log.MecanicId);

                        if (Mechanic != null)
                        {
                            var MechanicLog = MechanicLogs.Where(x => x.MecanicId == log.MecanicId).ToList();

                            Cost += decimal.Parse(fleetMaintenaceHelper.GetMechanicDuration(MechanicLog).ToString()) * Mechanic.HourlyWage;

                        }
                    }
                }

                return Cost;
            }

            catch (Exception e)
            {
                string ee = e.Message;
                return 0;
            }
        }
        public decimal GetExternalRentTotalCost(RentFromExternal RentFromExternals)
        {
            IFleetService _FleetService = new FleetService();
            ITimeSheetService _TimeSheetService = new TimeSheetService();
            var Fleets = _FleetService.GetAll().Where(x => x.RentId == RentFromExternals.RentFromExternalID).ToList();
            decimal TotalWorkingCost = decimal.Zero;

            foreach (var Fleet in Fleets)
            {

                List<TimeSheet> TimeSheets = _TimeSheetService.GetAll().Where(x => x.FleetsID == Fleet.FleetsID).ToList();
                decimal Workingtime = decimal.Zero;
                decimal Workingcost = decimal.Zero;

                foreach (var Work in TimeSheets)
                {
                    TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) - DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                    TimeSpan AfterNoonDuration = DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());

                    Workingtime = Workingtime + (MorningDuration.Hours + AfterNoonDuration.Hours);
                    Workingcost = Workingtime * decimal.Parse(Work.RentalRate.ToString());


                }
                TotalWorkingCost = TotalWorkingCost + Workingcost;

            }
            return TotalWorkingCost;
        }
    }
}
