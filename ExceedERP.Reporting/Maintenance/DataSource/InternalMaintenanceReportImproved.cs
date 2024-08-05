using ExceedERP.Core.Domain.CRM;
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
    public class InternalMaintenanceReportImproved
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalMaintenanceDTO> GetInternalMaintenanceCosts(string fleetId, DateTime? from, DateTime? to)
        {
            List<InternalMaintenanceDTO> list = new List<InternalMaintenanceDTO>();
            var jobOrder = db.JobOrder.AsNoTracking().ToList();
            if (Guid.TryParse(fleetId, out Guid id))
            {
                jobOrder = jobOrder.Where(x => x.FleetsID == id).ToList();
            }
            if (from.HasValue)
            {
                jobOrder = jobOrder.Where(x => x.DateCompleted >= from.Value).ToList();
            }
            if (to.HasValue)
            {
                jobOrder = jobOrder.Where(x => x.DateCompleted <= to.Value).ToList();
            }

            var jobNums = jobOrder.Select(s => s.Num);
            foreach (var item in jobNums)
            {
                list.Add(new InternalMaintenanceDTO
                {
                    MaitenanceServiceCost = CalculateServiceCost(item),
                    AdditionalCost = CalculateAdditionalCost(item),
                    SparePartsCost = CalculateSparePartCost(item),
                    PlateNumber = GetPlateNumber(item),
                    TotalCost = CalculateTotalCost(item),
                    JobNumber = GetJobNum(item),
                    ServiceCenter = GetServiceCenter(item)
                });
            }
            return list;

        }
        private string GetPlateNumber(int jobNum)
        {
            return db.JobOrder.FirstOrDefault(x => x.Num == jobNum)?.LicensePlate ?? "";
        }
        private string GetJobNum(int jobNum)
        {
            return db.JobOrder.FirstOrDefault(x => x.Num == jobNum)?.Num.ToString() ?? "";
        }
        private string GetServiceCenter(int jobNum)
        {
            return db.JobOrder.FirstOrDefault(x => x.Num == jobNum)?.MaintenanceCenter ?? "";
        }
        private decimal CalculateServiceCost(int jobNum)
        {
            
            var salesQuote = db.SalesQuoteVouchers
              .Where(x => x.Source == SalesQuoteSource.Maintenance)
              .AsEnumerable() // Switch to in-memory filtering to use TryParse
              .FirstOrDefault(x => int.TryParse(x.JobOrderNumber, out var jobOrderNumber) && jobOrderNumber == jobNum);
            if (salesQuote == null)
                return 0;
            var serviceLineItems = db.ServiceLineItems.Where(t => t.VoucherId == salesQuote.SalesQuoteVoucherID && t.VoucherType == CRMVoucherType.SalesQuote).ToList();

            return serviceLineItems.Sum(x => (decimal?)x.UnitAmount) ?? 0;
 
        }

        private decimal CalculateSparePartCost(int jobNum)
        {
            var salesQuote = db.SalesQuoteVouchers
              .Where(x => x.Source == SalesQuoteSource.Maintenance)
              .AsEnumerable() // Switch to in-memory filtering to use TryParse
              .FirstOrDefault(x => int.TryParse(x.JobOrderNumber, out var jobOrderNumber) && jobOrderNumber == jobNum);
            if (salesQuote == null)
                return 0;
            var lineItems = db.LineItems.Where(t => t.Voucher == salesQuote.SalesQuoteVoucherID.ToString() && t.VoucherDefinitionID == "0").ToList();

                return lineItems.Sum(x => (decimal?)x.UnitAmount) ?? 0;
            
        }

        private decimal CalculateAdditionalCost(int jobNum)
        {          
            var workShop = db.ServiceCenterJob.FirstOrDefault(x => x.Num == jobNum);
            if (workShop == null)
                return 0;
            return db.AdditionalWorkShopDetails.Where(x => x.ServiceCenterJobId == workShop.ServiceCenterJobId).Sum(x => (decimal?)x.OtherCosts) ?? 0;
        }

        private decimal CalculateTotalCost(int jobNum)
        {
            return CalculateServiceCost(jobNum) + CalculateSparePartCost(jobNum) + CalculateAdditionalCost(jobNum);
        }
    }

    public class InternalMaintenanceDTO
    {
        public string JobNumber { get; set; }
        public string ServiceCenter { get; set; }
        public string PlateNumber { get; set; }
        public decimal MaitenanceServiceCost { get; set; }
        public decimal SparePartsCost { get; set; }
        public decimal AdditionalCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
