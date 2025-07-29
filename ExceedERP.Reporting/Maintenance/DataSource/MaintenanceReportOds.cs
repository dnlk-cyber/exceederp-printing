using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.Maintenance.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.Core.Domain.Maintenance;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class MaintenanceReportOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<MaintenanceReportViewModel> GetData(DateTime startDate, DateTime endDate)
        {
            IList<MaintenanceReportViewModel> models = new List<MaintenanceReportViewModel>();
            var jobOrders = db.JobOrder.Where(q=>q.Status == MaintenanceConst.Closed).ToList();
            if(startDate!=null && endDate!=null)
                {
                jobOrders = jobOrders.Where(q => q.DateCompleted >= startDate && q.DateCompleted <= endDate).ToList();
            }
            foreach(var jobOrder in jobOrders)
            {
                MaintenanceReportViewModel model = new MaintenanceReportViewModel();
                if (jobOrder != null&&jobOrder.LicensePlate!=null)
                {
                    var jobRequest = db.JobRequest.Where(q => q.Num == jobOrder.Referance).FirstOrDefault();
                     model.Customer = db.OrganizationCustomers.Where(q=>q.OrganizationCustomerID == jobRequest.CustomerId).FirstOrDefault()?.TradeName;
                    model.PlateNo = jobOrder.LicensePlate;
                    var salesQuoteVoucher = db.SalesQuoteVouchers.Where(q => q.Source == Core.Domain.CRM.SalesQuoteSource.Maintenance && q.JobOrderNumber == jobOrder.Num.ToString()).FirstOrDefault();
                    if(salesQuoteVoucher!=null)
                    {
                        var lineItems = db.LineItems.Where(q => q.VoucherDefinitionID == "0" && q.Voucher == salesQuoteVoucher.SalesQuoteVoucherID.ToString()).ToList();
                        var serviceLineItems = db.ServiceLineItems.Where(q => q.VoucherType == Core.Domain.CRM.CRMVoucherType.SalesQuote && q.VoucherId == salesQuoteVoucher.SalesQuoteVoucherID).ToList();
                        model.SparePartCost = lineItems.ToList().Sum(s => s.TotalAmount);
                        model.MaintenanceCost = serviceLineItems.ToList().Sum(s => s.TotalAmount);

                    }
                    models.Add(model);
                }

            }
            

            return models;
        }

    }
}
