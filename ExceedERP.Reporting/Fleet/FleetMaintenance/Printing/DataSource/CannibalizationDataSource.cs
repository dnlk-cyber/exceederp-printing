using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
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
using CannibalizationViewModel = ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.ViewModel.CannibalizationViewModel;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.Printing.DataSource
{


    [DataObject]
    public class CannibalizationDataSource : List<CannibalizationViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();
        IJobOrderDetailService JobOrderDetailService = new JobOrderDetailService();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CannibalizationViewModel> GetCannibalization(string Id)
        {
            ICannibalizationService _CannibalizationService = new CannibalizationService();
            Cannibalization Cannibalization = new Cannibalization();
            IFleetService _FleetSerivce = new FleetService();
            List<CannibalizationViewModel> CannibalizationList = new List<CannibalizationViewModel>();


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


            if (!string.IsNullOrEmpty(Id))
            {
                Cannibalization = _CannibalizationService.GetAll().Where(x => x.CannibalizationID.ToString() == Id).FirstOrDefault();


            }




            var CannibalizationDetails = db.CannibalizationDetails.Where(x => x.CannibalizationId == Cannibalization.CannibalizationID).ToList();




            foreach (var CannibalizationDetail in CannibalizationDetails)
            {

                var model = new CannibalizationViewModel();


                if (Cannibalization.PartNeededFleetsID != null && Cannibalization.PartTakenFleetsID != null)
                {
                    var FromFleet = _FleetSerivce.GetById(Cannibalization.PartTakenFleetsID);
                    var ToFleet = _FleetSerivce.GetById(Cannibalization.PartNeededFleetsID);
                    model.ProjectName = FromFleet.Location;

                    model.PartNeededChasisNo = ToFleet.ChasisNumber;
                    model.PartNeededModel = ToFleet.Model;
                    model.PartNeededSerialNo = ToFleet.SerialNo;
                    model.PartNeededType = ToFleet.EquipmentType.EquipmentTypeName;

                    model.PartTakenChasisNo = FromFleet.ChasisNumber;
                    model.PartTakenSerialNo = FromFleet.SerialNo;
                    model.PartTakenType = FromFleet.EquipmentType.EquipmentTypeName;
                    model.PartTakenModel = FromFleet.Model;
                }
                model.Num = Cannibalization.Num;
                model.IsPermanent = Cannibalization.IsPermanent;
                 model.IsOperational = Cannibalization.IsOperational;
                model.IsPoor = Cannibalization.IsPoor;
                model.IsBreakDown = Cannibalization.IsBreakDown;
                model.IsScrap = Cannibalization.IsScrap;
                model.IsFromBreakDown = Cannibalization.IsFromBreakDown;
                model.IsFromOperational = Cannibalization.IsFromOperational;
                model.IsFromPoor = Cannibalization.IsFromPoor;
                model.IsFromScrap = Cannibalization.IsFromScrap;
                model.Operator = Cannibalization.CreatedBy;
                model.Inspector = Cannibalization.SendForApprovalBy;
                model.Approver = Cannibalization.OnlineApprovedBy;
                 model.Model = CannibalizationDetail.Model;
                model.WHPRNo = CannibalizationDetail.WHPRNo;
                model.PartName = CannibalizationDetail.PartName;
                model.PartType = CannibalizationDetail.PartType;
                model.Quantity = CannibalizationDetail.Quantity;
                model.TakenBy = CannibalizationDetail.TakenBy;
                model.Status = Cannibalization.Status;

                model.AmharicName = AmharicName;
                model.Name = Name;
                model.Address = Address;
                model.LogoFileName = LogoFileName;

                CannibalizationList.Add(model);
            }

            return CannibalizationList;
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








