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
    class FurnitureWeeklyReportds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FurnitureWeeklyReportViewModel> GetReport(DateTime from, DateTime to)
        {
            List<FurnitureWeeklyReportViewModel> list = new List<FurnitureWeeklyReportViewModel>();

            var jobOrders = db.FurnitureJobOrderForms
                              .Where(x => x.IsOnlineApproved)
                              .Where(x => DbFunctions.TruncateTime(x.DateCreated) >= from.Date && DbFunctions.TruncateTime(x.DateCreated) <= to.Date)
                              .ToList();

            //var jobOrderProductions = db.FurnitureJobOrderProductions
            //   .Where(x => x.IsClosed)              
            //   .ToList();

            foreach (var jobOrder in jobOrders)
            {
                
                var items = db.FurnitureJobOrderItems
                              .Where(x => x.FurnitureJobOrderFormId == jobOrder.FurnitureJobOrderFormId)
                              .ToList();

                foreach (var item in items)
                {
                    var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == item.JobTypeId);
                    list.Add(new FurnitureWeeklyReportViewModel
                    {
                        JobNo = item.JobNo,
                        JobType = jobType?.JobTypeName ?? "",
                        ProducedQuanttity = 0,
                        Quantity = item.Quantity,
                        TotalPrice = item.UnitPriceWithVAT * item.Quantity,
                        UnitPrice = item.UnitPriceWithVAT,
                        UOM = item.UnitOfMeasurment
                    });
                }
                
            }
            return list;
        }
    }
}
