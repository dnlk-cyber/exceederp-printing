using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Maintenance.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{

    [DataObject]
    public class MaintenanceSummaryOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IList<MaintenanceSummaryViewModel> GetData( DateTime startDate, DateTime endDate,string orderType,string status= null)
        {
            IList<MaintenanceSummaryViewModel> models = new List<MaintenanceSummaryViewModel>();
            IList<ServiceTypeViewModel> serviceTypes = new List<ServiceTypeViewModel>();

            var jobOrders = db.JobOrder.Where(s => s.OnlineApprovedTime >= startDate && s.OnlineApprovedTime <= endDate).ToList();
            if (status != null)
            {
                jobOrders = jobOrders.Where(q => q.Status == status).ToList();
            }
            if (jobOrders.Any())
            {
                foreach (var jobOrder in jobOrders)
                {
                    IList<DownTimeViewModel> downTimeViews = new List<DownTimeViewModel>();

                    var jobRequest = db.JobRequest.Where(q=>q.Num ==jobOrder.Referance).FirstOrDefault();
                    MaintenanceSummaryViewModel model = new MaintenanceSummaryViewModel();
                    model.JobNo = jobOrder.Num.ToString();
                    if (jobRequest != null)
                    {
                        model.PlateNo = jobRequest.LicensePlate;
                        model.EnteredDate = jobRequest.DateCreated?.ToString("yyyy-MM-dd");
                        if (jobRequest.Source == JobRequestSource.CompanyVehicle)
                        {
                            model.Owner = JobRequestSource.CompanyVehicle.ToString();
                            var asset = db.AssetRegistrations.Where(q => q.PlateNo == jobRequest.LicensePlate).FirstOrDefault();
                            if (asset != null)
                            {
                                model.vehicleTypeModel = asset.PlateType + "-" + asset.ModelNo;

                            }
                        }
                        else
                        {
                            var vehicle = db.EquipmentInformation.Where(q => q.LicensePlate == jobRequest.LicensePlate).FirstOrDefault();
                            if (vehicle != null)
                            {
                                model.vehicleTypeModel = vehicle.GroupName + "-" + vehicle.DeviceNumber;

                            }
                            if(jobRequest.CustomerId!=null)
                            {
                                var customerName = db.OrganizationCustomers.Find(jobRequest.CustomerId)?.TradeName;

                                model.Owner = customerName;
                            }

                            

                        }


                    }

                    var services = db.JobOrderDetail.Where(x => x.JobOrderID == jobOrder.JobOrderID).ToList();
                    IList<ServiceTypeViewModel> serviceTypeViewModels = new List<ServiceTypeViewModel>();

                    if (services.Any())
                    {
                        foreach (var service in services)
                        {
                            ServiceTypeViewModel serviceTypeViewModel = new ServiceTypeViewModel();
                           // var jobOrderDetail = db.JobOrderDetail.Find(service.JobOrderDetailId);
                          
                                var repairType = db.RepairType.Find(service.RepairTypeID);
                                if (repairType != null)
                                {
                                    serviceTypeViewModel.Service = repairType.Name;
                                    serviceTypeViewModel.ST = repairType.StandaredTime;
                                    serviceTypeViewModel.AT = repairType.PayableTime;
                                    serviceTypeViewModel.AtStDifferenceNumber = Math.Abs(repairType.PayableTime - repairType.StandaredTime);
                                    if (repairType.StandaredTime == 0)
                                    {
                                        serviceTypeViewModel.AtStDifferencePercent = 0;
                                    }
                                    else
                                    {
                                        serviceTypeViewModel.AtStDifferencePercent = Math.Round(serviceTypeViewModel.AtStDifferenceNumber / repairType.StandaredTime, 2);

                                    }

                                }
                            
                            //int.TryParse(service.CreatedBy, out int creatorId);
                            //var creator = db.OrganizationCustomers.Find(creatorId)?.FullName;
                            //var creator = db.OrganizationCustomers.Find(creatorId)?.FullName;
                            serviceTypeViewModel.RepairingStartDate = jobOrder.OnlineApprovedTime?.ToString("yyyy-MM-dd");
                            serviceTypeViewModel.InvestigatedBy = jobOrder.CreatedBy;

                            var worksheets = db.ServiceCrewWorkLogs.Where(q => q.JobOrderDetailId == service.JobOrderDetailID).ToList();
                            IList<DownTimeViewModel> downTimeViewModels = new List<DownTimeViewModel>();

                            if (worksheets.Any())
                            {

                                foreach (var worksheet in worksheets)
                                {
                                    var downTimes = db.FollowUp.Where(q => q.ServiceCrewWorkLogId == worksheet.ServiceCrewWorkLogId).ToList();

                                    if (downTimes.Any())
                                    {

                                        foreach (var downTime in downTimes)
                                        {
                                            DownTimeViewModel downTimeViewModel = new DownTimeViewModel();

                                            Decimal.TryParse(downTime.DurationHr, out decimal duration);
                                            downTimeViewModel.DW = duration;
                                            downTimeViewModel.DWReason = downTime.Status;

                                            downTimeViewModels.Add(downTimeViewModel);

                                        }
                                    }
                                    else
                                    {
                                        DownTimeViewModel downTimeViewModel = new DownTimeViewModel();

                                        downTimeViewModel.DW = 0;
                                        downTimeViewModel.DWReason = null;
                                        downTimeViewModels.Add(downTimeViewModel);

                                    }
                                    
                                }
                            }
                            else
                            {
                                DownTimeViewModel downTimeViewModel = new DownTimeViewModel();

                                downTimeViewModel.DW = 0;
                                downTimeViewModel.DWReason = null;
                                downTimeViewModels.Add(downTimeViewModel);
                            }



                            serviceTypeViewModel.DownTimeViewModels = downTimeViewModels;
                            serviceTypeViewModels.Add(serviceTypeViewModel);

                        }

                    }
                    else
                    {

                        DownTimeViewModel downTimeViewModel = new DownTimeViewModel();
                        ServiceTypeViewModel serviceTypeViewModel = new ServiceTypeViewModel();
                        serviceTypeViewModel.RepairingStartDate = null;
                        serviceTypeViewModel.Service = null;
                        serviceTypeViewModel.ST = 0;
                        serviceTypeViewModel.AT = 0;
                        serviceTypeViewModel.AtStDifferenceNumber = 0;
                        serviceTypeViewModel.AtStDifferencePercent = 0;
                        serviceTypeViewModel.AtStDifferencePercent = 0;
                        downTimeViewModel.DW = 0;
                        downTimeViewModel.DWReason = null;
                        downTimeViews.Add(downTimeViewModel);
                        serviceTypeViewModel.DownTimeViewModels = downTimeViews;
                        serviceTypeViewModels.Add(serviceTypeViewModel);



                    }

                    model.Status = jobOrder.Status;
                    model.ServiceTypeViewModels = serviceTypeViewModels;

                    models.Add(model);

                    }

            }

            return models;
        }
    }
}

