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
   
    public class JobOrderPrintChildOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<childClassVM> GetChildClass(string id)
        {

            List<childClassVM> childClassVms = new List<childClassVM>();
            if (Guid.TryParse(id, out Guid jobOrderId))
            {
                //var fetchExternalJobService = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == id);
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID.ToString() == id);
                if (jobOrder != null)
                {
                    var jobRequest = db.JobRequest.FirstOrDefault(x => x.Num == jobOrder.Referance);
                    var jobDetail = db.JobOrderDetail.Where(x => x.JobOrderID.ToString() == id).ToList();
                   
                    if (jobDetail.Any())
                    {
                        foreach (var childVM in jobDetail)
                        {
                            childClassVM chclass = new childClassVM();
                            chclass.VehicleCategory = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == childVM.SubCategoryCode)?.Name;
                            chclass.Cause = db.JobCauses.FirstOrDefault(x => x.JobCausesId == childVM.JobCausesId)?.Name;
                            chclass.ServiceGroup = nameof(childVM.ServiceGroup);
                            chclass.Description = db.JobOrderDetail.FirstOrDefault(x => x.RepairTypeID == childVM.RepairTypeID && x.JobOrderID == jobOrder.JobOrderID)?.DescriptionOfJobs ?? "";
                            chclass.ServiceName = db.RepairType.FirstOrDefault(x => x.RepairTypeID == childVM.RepairTypeID)?.Name;
                            chclass.ServiceCenter = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == childVM.ServiceCenterId)?.Name;
                            childClassVms.Add(chclass);
                        }
                    }

                }

            }
            return childClassVms;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<jobCausesVM> GetCauses(string id)
        {
            List<jobCausesVM> Causes = new List<jobCausesVM>();
            var fetchCauses = db.ExternalServiceCenterDetail.Where(x => x.ServiceCenterJobId == id).ToList();
            foreach (var cause in Causes)
            {
                jobCausesVM causevm = new jobCausesVM();
                causevm.WorkDescription = cause.WorkDescription;
                causevm.Quantity = cause.Quantity;
                causevm.Remark = cause.Remark;
                causevm.Unit = cause.Unit;
                Causes.Add(causevm);
            }
            return Causes;
        }
    }
     
   
       
    }

