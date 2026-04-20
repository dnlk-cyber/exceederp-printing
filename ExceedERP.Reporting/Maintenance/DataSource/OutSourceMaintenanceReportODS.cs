using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class OutSourceMaintenanceReportODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public OutSourcedMaintenanceVM GetOutsourcedJob (string id)
        {
            OutSourcedMaintenanceVM OutSourcedJob = new OutSourcedMaintenanceVM();
            var FetchJobOrder = db.JobOrder.FirstOrDefault(x => x.ServiceCenterJobId == id);
            if(FetchJobOrder!=null)
            {
                OutSourcedJob.KMReading = FetchJobOrder.KMReading;
                var FetchFleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == FetchJobOrder.LicensePlate || x.SerialNo == FetchJobOrder.LicensePlate);
                if(FetchFleet!=null)
                {
                    var FetchEquipmetType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == FetchFleet.EquipmentTypeID);
                    if(FetchEquipmetType!=null)
                        {
                        OutSourcedJob.EquipmentType = FetchEquipmetType.EquipmentTypeName;
                    }
                    var FetchEquipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == FetchFleet.EquipmentNameID);
                    if(FetchEquipmentName!=null)
                    {
                        OutSourcedJob.EquipmentModel = FetchEquipmentName.Name;
                    }
                }
                OutSourcedJob.PlateNumber = FetchJobOrder.LicensePlate;
                var fetchDetail = db.JobOrderDetail.Where(x => x.JobOrderID == FetchJobOrder.JobOrderID).ToList();
                var details = new List<string>();
               

                foreach (var Detail in fetchDetail)
                {
                    var fetchRepair = db.RepairType.FirstOrDefault(x => x.RepairTypeID == Detail.RepairTypeID);
                    if (fetchRepair != null)
                    {
                        details.Add(fetchRepair.Name);
                    }
                }
                var FetchReason = db.InternalJobToExternal.FirstOrDefault(x => x.ServiceCenterJobId == id);
                if(FetchReason!=null)
                {
                    OutSourcedJob.ReasonForOutsource = FetchReason.Description;
                    OutSourcedJob.RequestedBy = FetchReason.CreatedBy;
                    OutSourcedJob.RequestDate = FetchReason.DateCreated?.ToString("yyyy-MM-dd");
                }
                OutSourcedJob.MaintenanceRequested = string.Join(", ", details);
                
                OutSourcedJob.InspectedBy = FetchJobOrder.OnlineApprovedBy;
                OutSourcedJob.InspectedDate = FetchJobOrder.OnlineApprovedTime?.ToString("yyyy-MM-dd");
                OutSourcedJob.ApprovedBy = FetchJobOrder.SecondOnlineApprovedBy;
                OutSourcedJob.ApprovedDate = FetchJobOrder.SecondOnlineApprovedTime?.ToString("yyyy-MM-dd");
                var fetchJob = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == id);
                if(fetchJob!=null)
                {
                    var fetchWorkShop = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == fetchJob.ServiceCenterId);
                    if(fetchWorkShop!=null)
                    {
                        OutSourcedJob.Workshop = fetchWorkShop.Name;
                    }
                }
                
            }

            return OutSourcedJob;
        }

    }

    public class OutSourcedMaintenanceVM
    {
        public string Workshop { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentModel { get; set; }
        public string PlateNumber { get; set; }
        public string KMReading { get; set; }
        public string MaintenanceRequested { get; set; }
        public string ReasonForOutsource { get; set; }
        public string RequestedBy { get; set; }
        public string RequestDate { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string InspectedBy { get; set; }
        public string InspectedDate { get; set; }
             
    }
}
