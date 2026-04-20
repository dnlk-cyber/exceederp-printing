using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
    public class PrintingEstimationSummaryPrintOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public PrintSummaryHeadVM GetSummary(int id)
        {
            var printSumary = db.PrintingEstimationSummaries.FirstOrDefault(x => x.PrintingEstimationSummaryId == id);
            if (printSumary != null)
            {
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == printSumary.CustomerId);

                return new PrintSummaryHeadVM
                {
                    CreatedBy = printSumary.CreatedBy,
                    CustomerName = customer?.TradeName ?? "",
                    DateCreated = printSumary.DateCreated,
                    ApprovedBy = printSumary.OnlineApprovedBy,
                    ApprovedDate = printSumary.OnlineApprovedTime,
                    CheckedBy = printSumary.SendForApprovalBy,
                    CheckedDate = printSumary.SendForApprovalTime,
                    CreatedBySignature = SignatureHelper.GetUserSignature(printSumary.CreatedBy),
                    ApprovedBySignature = SignatureHelper.GetUserSignature(printSumary.OnlineApprovedBy),
                    CheckedBySignature = SignatureHelper.GetUserSignature(printSumary.SendForApprovalBy)
                };
            }
            return new PrintSummaryHeadVM();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrintSummaryDetailVM> GetSummaryDetail(int id)
        {
            List<PrintSummaryDetailVM> list = new List<PrintSummaryDetailVM>();
            var details = db.PrintingEstimationDetails.Where(x => x.PrintingEstimationSummaryId == id).ToList();
            foreach (var item in details)
            {
                list.Add(new PrintSummaryDetailVM
                {
                   PrintJobType = item.PrintJobType,
                   JobDesc = item.JobDescription,
                   AdminCost = item.AdministrativeCost,
                   LaborCost = item.LabourCost,
                   ManufCost = item.ManufacturingCost,
                   MaterialCost = item.MaterialCost,
                   OverHeadCost = item.OverHeadCost,
                   ProfitMargin = item.ProfitMargin,
                   Quantity = item.Quantity,
                   TotalCost = item.TotalCost,
                   TotalPrice = item.TotalPrice,
                   UnitCost = item.UnitCost,
                   UnitPrice = item.UnitPrice
                });
            }
            return list;
        }
    }
}
