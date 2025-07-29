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
   
    public class ServiceJobOrderPrintChildOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<childClassVM> GetChildClass(string id)
        {

            List<childClassVM> childClassVms = new List<childClassVM>();
            if (Guid.TryParse(id, out Guid jobOrderId))
            {
                var fetchExternalJobService = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == id);
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == fetchExternalJobService.JobOrderID);
                if (jobOrder != null)
                {
                    var jobRequest = db.JobRequest.FirstOrDefault(x => x.Num == jobOrder.Referance);
                    var jobDetail = db.JobOrderServiceCenterDetails.Where(x => x.ServiceCenterJobId == fetchExternalJobService.ServiceCenterJobId).ToList();
                   
                    if (jobDetail.Any())
                    {
                        foreach (var childVM in jobDetail)
                        {
                            childClassVM chclass = new childClassVM();
                            chclass.VehicleCategory = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == childVM.SubCategoryCode)?.Name;
                            chclass.Cause = db.JobCauses.FirstOrDefault(x => x.JobCausesId == childVM.JobCausesId)?.Name;
                            chclass.ServiceGroup = nameof(childVM.ServiceGroup);
                            chclass.Description = db.JobOrderDetail.FirstOrDefault(x => x.RepairTypeID == childVM.RepairTypeID && x.JobOrderID == jobOrder.JobOrderID)?.DescriptionOfJobs ?? "";
                            chclass.MaintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == jobOrder.MaintenanceCenterId)?.Name ?? "";
                            chclass.ServiceName = db.RepairType.FirstOrDefault(x => x.RepairTypeID == childVM.RepairTypeID)?.Name;
                            chclass.ServiceCenter = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == childVM.ServiceCenterId)?.Name;
                            childClassVms.Add(chclass);
                        }
                    }

                }

            }
            return childClassVms;
        }
    }
    public class childClassVM
        {
            public string VehicleCategory { get; set; }
            public string Cause { get; set; }
            public string ServiceGroup { get; set; }
            public string ServiceName { get; set; }
            public string ServiceCenter { get; set; }
            public string MechanicName { get; set; }
            public string Description { get; set; }
           public string MaintenanceCenter { get; set; }
        }
        //public class JobOrderVm
        //{
        //    public string PlateNumber { get; set; }
        //    public string EquipmentName { get; set; }
        //    public int JobNum { get; set; }
        //    public string CustomerName { get; set; }
        //    public string Requester { get; set; }

        //    public string BusinessUnit { get; set; }
        //    public string Branch { get; set; }
        //    public string WorkUnit { get; set; }
        //    public string MaintenanceCenter { get; set; }
        //    public DateTime? DateCreated { get; set; }
        //    public DateTime? DateClosed { get; set; }
        //    public string VehicleCategory { get; set; }
        //    public string Cause { get; set; }
        //    public string ServiceGroup { get; set; }
        //    public string ServiceName { get; set; }
        //    public string ServiceCenter { get; set; }
        //    public string RequestedBy { get; set; }
        //    public string TechnicalApproval { get; set; }
        //    public string MaintenanceRequestApproval { get; set; }
        //    public DateTime? RequestDate { get; set; }
        //    public DateTime? ApproveDate { get; set; }
        //    public string CheckedBy { get; set; }
        //    public DateTime? CheckedDate { get; set; }
        //    public virtual List<childClassVM> childClassVms { get; set; }
        //}
    }

