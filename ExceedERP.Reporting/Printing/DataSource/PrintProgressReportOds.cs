using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Printing.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.DataSource
{
    [DataObject]
    public class PrintProgressReportOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PrintProdProgressViewModel GetProgress(string JobNo)
        {
            if(int.TryParse(JobNo, out int id))
            {
                var followUp = db.PrintingProductionFollowUps.FirstOrDefault(x => x.PrintingJobOrderItemId == id);
                if (followUp != null)
                {
                    var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == followUp.CustomerId);
                    var entity = new PrintProdProgressViewModel
                    {
                        JobNo = followUp.PrintingJobOrderItemId.ToString(),
                        CurrentStatus = followUp.PrintingOperationStatus.ToString(),
                        CustomerName = customer?.TradeName ?? "",
                        JobDesc = followUp.JobDescription,
                        JobType = followUp.PrintJobType.ToString(),
                        Page = followUp.Pages,
                        Quantity = followUp.Quantity
                    };
                    return entity;
                }
            }
            
            return new PrintProdProgressViewModel();
        }
    }
}
