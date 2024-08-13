using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.Maintenance.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class MaintenanceDurationOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<MaintenanceDurationViewModel> GetData(DateTime startDate, DateTime endDate)
        {
            IList<MaintenanceDurationViewModel> models = new List<MaintenanceDurationViewModel>();
            var jobOrders = db.JobOrder.Where(q=>q.IsOnlineApproved).ToList();
            if(startDate!=null  && endDate!=null)
            {
                jobOrders = jobOrders.Where(q => q.OnlineApprovedTime.HasValue? q.OnlineApprovedTime.Value.Date >= startDate.Date  && q.OnlineApprovedTime.Value.Date <= endDate.Date: false).ToList();
            }
            foreach(var jobOrder in jobOrders)
            {
                MaintenanceDurationViewModel model = new MaintenanceDurationViewModel();
                if (jobOrder != null && jobOrder.LicensePlate!=null)
                {
                   
                  
                    model.Date = DateTime.Now.ToString("yyyy-MM-dd");
                    var jobRequest = db.JobRequest.Where(q => q.Num == jobOrder.Referance).FirstOrDefault();
                    if(jobRequest!=null)
                    {
                        model.Customer = db.OrganizationCustomers.Where(q => q.OrganizationCustomerID == jobRequest.CustomerId).FirstOrDefault()?.TradeName;
                        model.CustomerRequestDate = jobRequest.DateCreated?.ToString("MMM dd ,yyyy") + " - " + jobRequest.OnlineApprovedTime?.ToString("MMM dd ,yyyy");
                        model.CustomerRequestDuration = ((jobRequest.OnlineApprovedTime?.Date - jobRequest.DateCreated?.Date)?.Days+1).ToString();
                        model.JobOrderDate = (jobOrder.DateCreated?.Date.ToString("MMM dd ,yyyy") + " - " +jobOrder.OnlineApprovedTime?.Date.ToString("MMM dd ,yyyy"))+" &&\n"+ (jobOrder.OnlineTransferredTime?.Date.ToString("MMM dd ,yyyy") +" - "+ jobOrder.SecondOnlineApprovedTime?.Date.ToString("MMM dd ,yyyy"));
                        var jobOrderDuration = ((((jobOrder.OnlineApprovedTime?.Date - jobOrder.DateCreated?.Date)?.Days??-1) + 1) + 
                            (((jobOrder.SecondOnlineApprovedTime?.Date - jobOrder.OnlineTransferredTime?.Date)?.Days??-1) + 1));
                        if(jobOrder.OnlineApprovedTime?.Date== jobOrder.OnlineTransferredTime?.Date)
                        {
                         
                                jobOrderDuration--;
                        }
                        model.JobOrderDuration = jobOrderDuration.ToString();
                        model.ServiceCenterDate = jobRequest.OnlineApprovedTime?.ToString("MMM dd ,yyyy") + " - " + jobOrder.OnlineTransferredTime?.ToString("MMM dd ,yyyy")??"0";
                        model.ServiceCenterDuration =((jobOrder.OnlineTransferredTime?.Date - jobOrder.OnlineApprovedTime?.Date)?.Days+1).ToString()??"0";
                        model.WorkShopDate = jobOrder.OnlineAssignedTime?.ToString("MMM dd ,yyyy") + " - " + jobOrder.OnlineSubmittedTime?.ToString("MMM dd ,yyyy") ?? "0";
                        model.WorkShopDuration = ((jobOrder.OnlineSubmittedTime?.Date - jobOrder.OnlineAssignedTime?.Date)?.Days+1).ToString() ?? "0";
                        model.TotalDurationDate = jobRequest.DateCreated?.ToString("MMM dd ,yyyy") + " - " + jobOrder.SecondOnlineApprovedTime?.ToString("MMM dd ,yyyy") ?? "0";
                        model.TotalDurationDays = ((jobOrder.SecondOnlineApprovedTime?.Date - jobRequest.DateCreated?.Date)?.Days+1).ToString() ?? "0";
                    }
                    model.PlateNo = jobOrder.LicensePlate;



                    models.Add(model);
                }

            }
            

            return models;
        }

    }
}
