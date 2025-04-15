using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.JobCosting.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExceedERP.Reporting.JobCosting.DataSource
{
[DataObject]
 public class JobCostingOds
  {
    ExceedDbContext db = new ExceedDbContext();
    [DataObjectMethod(DataObjectMethodType.Select)]

    public List<JobCostingVM> GetJobcost(string id)
    {
       List<JobCostingVM> JobCosting = new List<JobCostingVM>();
        if (!string.IsNullOrEmpty(id))
        {
            int.TryParse(id, out int jobcostId);
            var jobCost = db.JobCosts.Where(j => j.JobCostId == jobcostId).ToList();
                

                foreach (var listofJobs in jobCost)
                {
                  var tradeName = db.OrganizationCustomers
                                  .Where(c => c.OrganizationCustomerID == listofJobs.CustomerId).Select(c => c.TradeName).FirstOrDefault();
                    var jobCategory = db.JobCategories
                                  .Where(j => j.JobCategoryId == listofJobs.JobCategoryId).Select(j => j.JobName).FirstOrDefault();
                    var jobName = db.Jobs.Where(j => j.JobId == listofJobs.JobId).Select(j => j.CustomerName).FirstOrDefault();
               var records = new JobCostingVM
                {
                JobCostId = listofJobs.JobCostId,
                CustomerId = listofJobs.CustomerId,
                CustomerName= tradeName,
                JobCategory = jobCategory,
                JobReceivedDate = listofJobs.JobReceivedDate,
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
        JobCosting.Add(records);
         }
        }
    return JobCosting;
       }
      }
    }