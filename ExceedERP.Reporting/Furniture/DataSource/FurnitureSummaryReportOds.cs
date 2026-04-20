using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Furniture.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.DataSource
{
    [DataObject]
    public class FurnitureSummaryReportOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FurnitureJobSummaryVm> SummaryHeader(DateTime? from, DateTime? to)
        {
            List<FurnitureJobSummaryVm> list = new List<FurnitureJobSummaryVm>();
            if (from.HasValue && to.HasValue)
            {
                var jobOrders = db.FurnitureJobOrderForms.Where(x => x.JobClosed).ToList();
                if (jobOrders.Any())
                {
                    jobOrders = jobOrders.Where(x => x.DeliveryDate.Date >= from.Value.Date && x.DeliveryDate.Date <= to).ToList();

                    foreach (var jobOrder in jobOrders)
                    {
                        var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);
                        
                        var jobItems = db.FurnitureJobOrderItems.Where(x => x.FurnitureJobOrderFormId == jobOrder.FurnitureJobOrderFormId).ToList();
                        foreach (var item in jobItems)
                        {
                            var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == item.JobTypeId);

                            list.Add(new FurnitureJobSummaryVm
                            {
                                CustomerName = customer?.TradeName ?? "",
                                JobType = jobType?.JobTypeName ?? "",
                                PaymentType = jobOrder.PaymentType,
                                Quantity = item.Quantity,
                                Transport = item.TransportCost,
                                TotalPrice = item.UnitPriceWithVAT,
                                JobNo = item.JobNo
                            });
                        }
                    }
                }
                

            }
            return list;
        }
    }
}
