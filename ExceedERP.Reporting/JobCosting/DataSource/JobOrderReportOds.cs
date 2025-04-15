using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.JobCosting.DataSource
{
    [DataObject]
    public class JobOrderReportOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderReportOdsVM> getJobCost(string id)
        {
            List<JobOrderReportOdsVM> JobOrder = new List<JobOrderReportOdsVM>();
            var jobCostQuery = db.JobCosts.AsQueryable();

            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out int jobcostId))
            {
                jobCostQuery = jobCostQuery.Where(j => j.JobId == jobcostId);
            }
            var jobCosts = jobCostQuery.ToList();

            foreach (var listofJobs in jobCosts)
            {
                var tradeName = db.OrganizationCustomers
                                  .Where(c => c.OrganizationCustomerID == listofJobs.CustomerId)
                                  .Select(c => c.TradeName)
                                  .FirstOrDefault();

                var jobCategory = db.JobCategories
                                    .Where(j => j.JobCategoryId == listofJobs.JobCategoryId)
                                    .Select(j => j.JobName)
                                    .FirstOrDefault();

                var jobName = db.Jobs
                                .Where(j => j.JobId == listofJobs.JobId)
                                .Select(j => j.CustomerName)
                                .FirstOrDefault();

                var records = new JobOrderReportOdsVM
                {
                    JobCostId = listofJobs.JobCostId,
                    CustomerId = listofJobs.CustomerId,
                    CustomerName = tradeName,
                    JobCategory = jobCategory,
                     JobRecDate = listofJobs.JobReceivedDate,
                    ReciptNo = listofJobs.ReciptNo,
                    Quantity = listofJobs.Quantity,
                    SellingPrice = listofJobs.SellingPrice,
                    OrderSize = listofJobs.OrderSize,
                    Void = listofJobs.Void,
                    Status = listofJobs.Status.ToString(),
                    Description = listofJobs.Description,
                    DateCreated = listofJobs.DateCreated,
                    LastModified = listofJobs.LastModified,
                    CreatedBy = listofJobs.CreatedBy,
                    ModifiedBy = listofJobs.ModifiedBy,
                };

                JobOrder.Add(records);
            }

            return JobOrder;
        }

    }

        public class JobOrderReportOdsVM
        {
        public int JobCostId { get; set; }
        public int JobId { get; set; }
        public string JobCategory { get; set; }
        public int JobCategoryId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string BrandName { get; set; }

        public string JobOrder { get; set; }

        public DateTime? JobRecDate { get; set; }
        public DateTime? Date { get; set; }
        public string ReciptNo { get; set; }
        public decimal Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal OrderSize { get; set; }
        public bool Void { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? LastModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }




    }
}


      //    public List<JobOrderReportOdsVM> getJobCost(string id, DateTime? From, DateTime? To)
      //    {
      //        List<JobOrderReportOdsVM> JobOrder = new List<JobOrderReportOdsVM>();
      //        if (!string.IsNullOrEmpty(id))
      //        {
      //            int.TryParse(id, out int jobcostId);
      //            var jobCost = db.JobCosts.Where(j => j.JobCostId == jobcostId).ToList();


//            if (jobCost != null)
//            {
//                jobCost = jobCost.Where(x => x.DateCreated >= From && x.DateCreated <= To).ToList();
//                if (jobCost != null)
//                {

//                    foreach (var listofJobs in jobCost)
//                    {
//                        var tradeName = db.OrganizationCustomers
//                                        .Where(c => c.OrganizationCustomerID == listofJobs.CustomerId).Select(c => c.TradeName).FirstOrDefault();
//                        var jobCategory = db.JobCategories
//                                      .Where(j => j.JobCategoryId == listofJobs.JobCategoryId).Select(j => j.JobName).FirstOrDefault();
//                        var jobName = db.Jobs.Where(j => j.JobId == listofJobs.JobId).Select(j => j.CustomerName).FirstOrDefault();
//                        var records = new JobOrderReportOdsVM
//                        {
//                            JobCostId = listofJobs.JobCostId,
//                            CustomerId = listofJobs.CustomerId,
//                            CustomerName = tradeName,
//                            JobCategory = jobCategory,
//                            JobReceivedDate = listofJobs.JobReceivedDate,
//                            ReciptNo = listofJobs.ReciptNo,
//                            Quantity = listofJobs.Quantity,
//                            SellingPrice = listofJobs.SellingPrice,
//                            OrderSize = listofJobs.OrderSize,
//                            Void = listofJobs.Void,
//                            Status = listofJobs.Status.ToString(),
//                            Description = listofJobs.Description,
//                            DateCreated = listofJobs.DateCreated,
//                            LastModified = listofJobs.LastModified,
//                            CreatedBy = listofJobs.CreatedBy,
//                            ModifiedBy = listofJobs.ModifiedBy,

//                        };
//                        JobOrder.Add(records);
//                    }
//            }   }
//        }
//        return JobOrder;
//    }
//}