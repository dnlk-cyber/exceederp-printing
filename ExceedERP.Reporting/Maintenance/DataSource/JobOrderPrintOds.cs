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
    public class JobOrderPrintOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public JobOrderVm GetJobOrderPrint(string id)
        {
            JobOrderVm jobOrderVm = new JobOrderVm();
            if(Guid.TryParse(id, out Guid jobOrderId))
            {
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == jobOrderId);
                if(jobOrder != null)
                {
                    var jobRequest = db.JobRequest.FirstOrDefault(x => x.Num == jobOrder.Referance);
                    var jobDetail = db.JobOrderDetail.FirstOrDefault(x => x.JobOrderID == jobOrder.JobOrderID);

                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == jobRequest.RequesterId);
                   return new JobOrderVm
                    {
                        JobNum = jobOrder.Num,
                        
                        PlateNumber = jobOrder.LicensePlate,
                        EquipmentName = jobOrder.EquipmentName,
                        CustomerName = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId)?.TradeName ?? "",
                        Requester = employee?.FirstNameEnglish + "-" + employee?.MiddleNameEnglish,
                        Branch = db.Branch.FirstOrDefault(x => x.BranchId == jobOrder.BranchId)?.Name ?? "",
                        BusinessUnit = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == jobOrder.OrgStructureId).Name,
                        MaintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID == jobOrder.MaintenanceCenterId)?.Name ?? "",
                        DateCreated = jobOrder.DateCreated,
                        DateClosed = jobOrder.DateCreated,

                        VehicleCategory = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == jobDetail.SubCategoryCode)?.Name,
                        Cause = db.JobCauses.FirstOrDefault(x => x.JobCausesId == jobDetail.JobCausesId)?.Name,
                        ServiceGroup = nameof(jobDetail.ServiceGroup),
                        ServiceName = db.RepairType.FirstOrDefault(x => x.RepairTypeID == jobDetail.RepairTypeID)?.Name,
                        ServiceCenter = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == jobDetail.ServiceCenterId)?.Name,
                        
                    };
                }
            }
            return jobOrderVm;
        }
    }

    public class JobOrderVm
    {
        public string PlateNumber { get; set; }
        public string EquipmentName { get; set; }
        public int JobNum { get; set; }
        public string CustomerName { get; set; }
        public string Requester { get; set; }

        public string BusinessUnit { get; set; }
        public string Branch { get; set; }
        public string WorkUnit { get; set; }
        public string MaintenanceCenter { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateClosed { get; set; }
        public string VehicleCategory { get; set; }
        public string Cause { get; set; }
        public string ServiceGroup { get; set; }
        public string ServiceName { get; set; }
        public string ServiceCenter { get; set; }

    }
}
