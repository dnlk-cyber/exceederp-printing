using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.ViewModel;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobOrderViewModel = ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.ViewModel.JobOrderViewModel;


namespace ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.DataSource
{


    [DataObject]
    public class JobOrderDetailDataSource : List<JobOrderViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();
        IJobOrderDetailService JobOrderDetailService = new JobOrderDetailService();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderViewModel> GetJobList(string oId)
        {
            IJobOrderDetailService service = new JobOrderDetailService();
            IJobOrderService JService = new JobOrderService();
            IFleetService fservice = new FleetService();
            IOperatorService oservice = new OperatorService();
            List<JobOrderViewModel> JobList = new List<JobOrderViewModel>();
            JobOrderViewModel OrderViewModel;
            var Job = new ExceedERP.Core.Domain.FleetManagement.FleetMaintenance.JobOrder(); ;
            var Fleets = fservice.GetAll();
            var operators = oservice.GetAll();

            decimal TotalTireCost = 0;
            decimal TotalSpareCost = 0;
            decimal TotalOilLubricantCost = 0;
            decimal TotalOtherCost = 0;
            decimal TotalLabourCost = 0;
            double TotalHr = 0;
            decimal TotalSalvageCost = 0;

            try
            {
                #region Shared Company DS
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


                if (!string.IsNullOrEmpty(oId))
                {
                    int OrderNum = int.Parse(oId);
                    Job = JService.GetAll().Where(so => so.Num == OrderNum).FirstOrDefault();


                    TotalTireCost = 0;
                    TotalSpareCost = 0;
                    TotalOilLubricantCost = 0;
                    TotalOtherCost = 0;
                    TotalLabourCost = 0;
                    TotalHr = 0;
                    TotalSalvageCost = 0;

                    var Requests = db.JobRequest.Where(x => x.Num == Job.Referance).FirstOrDefault();

                    List<JobOrderDetail> lineItems = new List<JobOrderDetail>();
                    if (Requests != null)
                    {
                        lineItems = service.GetAll().Where(x => x.JobOrderID == Job.JobOrderID || x.JobOrderID == Requests.JobRequestId).ToList();
                    }
                    else
                    {
                        lineItems = service.GetAll().Where(x => x.JobOrderID == Job.JobOrderID).ToList();
                    }

                    TotalLabourCost = GetLabourCost(Job.JobOrderID);
                    TotalSpareCost = GetSpareCost(Job.JobOrderID);
                    TotalTireCost = GetTyreSpareCost(Job.JobOrderID);
                    TotalSalvageCost = GetSalvageSpareCost(Job.JobOrderID);
                    TotalOilLubricantCost = GetOilSpareCost(Job.JobOrderID);
                    TotalOtherCost = GetOtherSpareCost(Job.JobOrderID);





                    foreach (JobOrderDetail l in lineItems)
                    {

                        var Fleet = Fleets.Where(y => y.FleetsID == Job.FleetsID).FirstOrDefault(); //== l.ThingID).FirstOrDefault();
                        var ops = operators.Where(x => x.FleetsID == Job.FleetsID).FirstOrDefault();
                        string joperator = "";

                        if (ops != null)
                        {
                            joperator = ops.FullName;
                        }
                        OrderViewModel = new JobOrderViewModel();

                        if (Fleet != null)
                        {

                            OrderViewModel.Section = l.Service.WorkUnitSection.Name;
                            OrderViewModel.ServiceCode = l.Service.ServiceCode;
                            OrderViewModel.ProjectName = Fleet.Location;
                            OrderViewModel.Manufacture = Fleet.Manufacturer;
                            OrderViewModel.Model = Fleet.Model;
                            OrderViewModel.AssetNum = Fleet.UnitNum;
                            OrderViewModel.Odometer = Fleet.CurentOdoMeter;
                            OrderViewModel.EquipmentName = Fleet.SerialNo;
                            OrderViewModel.EquipmentType = Fleet.EquipmentType.EquipmentTypeName;
                            OrderViewModel.ChassisNumber = Fleet.ChasisNumber;
                            OrderViewModel.EngineNumber = Fleet.EngineNumber;
                            OrderViewModel.ProjectName = Fleet.Location;
                            OrderViewModel.Driver = Fleet.OperatorName;

                        }


                        string ApprovalStatus = Enum.GetName(l.ApprovalStatus.GetType(), l.ApprovalStatus);



                        OrderViewModel.JobOrderNum = Job.Num;

                        OrderViewModel.Type = Job.Type;
                        OrderViewModel.odometer = Job.Odometer.ToString();
                        OrderViewModel.IsM = !Job.IsPM;
                        OrderViewModel.IsPM = Job.IsPM;
                        OrderViewModel.InspectedBy = Job.OnlineApprovedBy;
                        OrderViewModel.MaintenanceCenter = Job.MaintenanceCenter;
                        OrderViewModel.DateCompleted = Job.DateCompleted;
                        OrderViewModel.TecnicallyApproved = Job.SecondOnlineApprovedBy;
                        OrderViewModel.AdminstrativeApproved = Job.CreatedBy;
                        OrderViewModel.ServiceName = l.ServiceName;
                        OrderViewModel.Status = l.Status;
                        OrderViewModel.ApprovalStatus = ApprovalStatus;
                        OrderViewModel.Cause = l.JobCauses.Name;
                        OrderViewModel.Requester = Job.CreatedBy;
                        OrderViewModel.RequestNumber = int.Parse(Job.Referance.ToString());
                        OrderViewModel.Duration = TotalHr;
                        //OrderViewModel.TyreCost = TotalTireCost;
                        //OrderViewModel.SpareCost = TotalSpareCost;
                        //OrderViewModel.SalvageCost = TotalSalvageCost;
                        //OrderViewModel.OilCost = TotalOilLubricantCost;
                        //OrderViewModel.LabourCost = TotalLabourCost;
                        //OrderViewModel.TotalCost = TotalOtherCost + TotalLabourCost + TotalOilLubricantCost + TotalSalvageCost + TotalSpareCost + TotalTireCost;


                        OrderViewModel.AmharicName = AmharicName;
                        OrderViewModel.Name = Name;
                        OrderViewModel.Address = Address;
                        OrderViewModel.LogoFileName = LogoFileName;

                        JobList.Add(OrderViewModel);
                    }
                }

            }
            catch(Exception e)
            { return JobList; }
            return JobList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderViewModel> GetJobMechanics(string oId)
        {
            IJobOrderDetailService service = new JobOrderDetailService();
            IJobOrderService JService = new JobOrderService();
            List<JobOrderViewModel> JobList = new List<JobOrderViewModel>();

            if (!string.IsNullOrEmpty(oId))
            {
                int OrderNum = int.Parse(oId);
                var Job = JService.GetAll().Where(so => so.Num == OrderNum).FirstOrDefault();


                var Logs = db.ServiceCrewWorkLogs.Where(x => x.JobId == Job.JobOrderID && x.IsOnlineApproved == true).ToList();

                foreach (var Log in Logs)
                {

                    var OrderViewModel = new JobOrderViewModel();

                    TimeSpan MorningDuration = DateTime.Parse(Log.WorkingHoursMorningto.ToString()) - DateTime.Parse(Log.WorkingHoursMorningFrom.ToString());
                    TimeSpan AfterNoonDuration = DateTime.Parse(Log.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Log.WorkingHoursAfternoonFrom.ToString());

                    OrderViewModel.TotalCost = (MorningDuration.Hours + AfterNoonDuration.Hours);

                    OrderViewModel.InspectedBy = Log.MechanicCrew.FullName;
                    OrderViewModel.ServiceName = Log.JobOrderDetail.ServiceName + "-" + Log.JobOrderDetail.Service.ServiceCode;

                    JobList.Add(OrderViewModel);

                }
            }
            return JobList;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderViewModel> GetJobSpares(string oId)
        {
            IJobOrderDetailService service = new JobOrderDetailService();
            IJobOrderService JService = new JobOrderService();
            IFleetService fservice = new FleetService();
            IOperatorService oservice = new OperatorService();
            List<JobOrderViewModel> JobList = new List<JobOrderViewModel>();
            JobOrderViewModel OrderViewModel;

            if (!string.IsNullOrEmpty(oId))
            {
                int OrderNum = int.Parse(oId);
                var Job = JService.GetAll().Where(so => so.Num == OrderNum).FirstOrDefault();



                var Requistions = db.Issues.Where(x => x.JobOrderNo == Job.Num.ToString()).ToList();
                var SpareMap = db.InventorySparePartMapping.ToList();
                foreach (var Requistion in Requistions)
                {
                    foreach (var Item in Requistion.IssueItems)
                    {
                        foreach (var Map in SpareMap)
                        {
                            if (Map.InventoryCategoryId == Item.ItemCategory.ItemCategoryCode)
                            {
                                OrderViewModel = new JobOrderViewModel();
                                OrderViewModel.Type = Map.SpareType.ToString();
                                OrderViewModel.RequestNumber = Item.IssueItemID;
                                OrderViewModel.TotalCost = Item.InventoryItem.AvgPrice * Item.ApprovedQuantity;
                                JobList.Add(OrderViewModel);

                            }
                        }
                    }
                }

            }
            return JobList;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderViewModel> GetJobSupplemtary(string oId)
        {
            IJobOrderDetailService service = new JobOrderDetailService();
            IJobOrderService JService = new JobOrderService();
            IFleetService fservice = new FleetService();
            IOperatorService oservice = new OperatorService();
            List<JobOrderViewModel> JobList = new List<JobOrderViewModel>();
            JobOrderViewModel OrderViewModel;

            if (!string.IsNullOrEmpty(oId))
            {
                int OrderNum = int.Parse(oId);
                var Job = JService.GetAll().Where(so => so.Num == OrderNum).FirstOrDefault();






                var Spares = db.ServiceTool.Where(x => x.ServiceID == Job.JobOrderID && x.IsOnlineApproved == true).ToList();

                foreach (var Spare in Spares)
                {



                    OrderViewModel = new JobOrderViewModel();

                    OrderViewModel.ServiceName = Spare.MaintenanceTools.ToolsName;
                    OrderViewModel.TotalCost = Spare.MaintenanceTools.Price;
                    JobList.Add(OrderViewModel);
                }
            }
            return JobList;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderViewModel> GetJobComponent(string oId)
        {
            IJobOrderDetailService service = new JobOrderDetailService();
            IJobOrderService JService = new JobOrderService();
            IFleetService fservice = new FleetService();
            IOperatorService oservice = new OperatorService();
            List<JobOrderViewModel> JobList = new List<JobOrderViewModel>();
            JobOrderViewModel OrderViewModel;

            if (!string.IsNullOrEmpty(oId))
            {
                int OrderNum = int.Parse(oId);
                var Job = JService.GetAll().Where(so => so.Num == OrderNum).FirstOrDefault();






                //var Components = db.ExternalComponent.Where(x => x.JobId == Job.JobOrderID ).ToList();

                //foreach (var Component in Components)
                //{

                //    var Items = db.ExternalComponentDetail.Where(x => x.ExternalComponentId == Component.ExternalComponentId).ToList();
                //    foreach (var Item in Items)
                //    {
                var ExternalMaterials = db.ExternalMaterial.Where(x => x.JobId == Job.JobOrderID).ToList();
                foreach (var ExternalMaterial in ExternalMaterials)
                {
                    OrderViewModel = new JobOrderViewModel();

                    OrderViewModel.ServiceName = ExternalMaterial.MaterialName;
                    OrderViewModel.TotalCost = ExternalMaterial.Quantity;
                    JobList.Add(OrderViewModel);
                }
            }

            return JobList;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderViewModel> GetJobDetail(string oId)
        {
            IJobOrderDetailService service = new JobOrderDetailService();
            IJobOrderService JService = new JobOrderService();
            IFleetService fservice = new FleetService();
            IOperatorService oservice = new OperatorService();
            List<JobOrderViewModel> JobList = new List<JobOrderViewModel>();
            JobOrderViewModel OrderViewModel;

            if (!string.IsNullOrEmpty(oId))
            {
                int OrderNum = int.Parse(oId);
                var Job = JService.GetAll().Where(so => so.Num == OrderNum).FirstOrDefault();

                var JobOrderDetails = db.JobOrderDetail.Where(x => x.JobOrderID == Job.JobOrderID).ToList();
                foreach (var JobOrderDetail in JobOrderDetails)
                {
                    OrderViewModel = new JobOrderViewModel();

                    OrderViewModel.ServiceName = JobOrderDetail.Service.ServiceName;
                    OrderViewModel.Section = JobOrderDetail.Service.WorkUnitSection.Name;
                    if (JobOrderDetail.DateCreated.Value != null)
                    {
                        OrderViewModel.StartedDate = JobOrderDetail.DateCreated.Value.Date.Day.ToString() + "-" + JobOrderDetail.DateCreated.Value.Date.Month.ToString() + "-" + JobOrderDetail.DateCreated.Value.Date.Year.ToString();
                        OrderViewModel.StartedTime = JobOrderDetail.DateCreated.Value.ToString("hh:mm");
                    }
                    if (JobOrderDetail.OnlineApprovedTime.Value != null)
                    {
                        OrderViewModel.CompletedDate = JobOrderDetail.OnlineApprovedTime.Value.Date.Day.ToString() + "-" + JobOrderDetail.OnlineApprovedTime.Value.Date.Month.ToString() + "-" + JobOrderDetail.OnlineApprovedTime.Value.Date.Year.ToString();  
                        OrderViewModel.CompletedTime = JobOrderDetail.OnlineApprovedTime.Value.ToString("hh:mm") ;
                    }


                    JobList.Add(OrderViewModel);
                }
            }

            return JobList;
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

                    var Crews = _ServiceCrewService.GetAll().Where(x => x.ServiceID == Service.JobOrderDetailID).ToList();



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








