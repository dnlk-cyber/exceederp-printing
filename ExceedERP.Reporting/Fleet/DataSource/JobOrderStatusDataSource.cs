using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using ExceedERP.Core.Domain.Maintenance;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class JobOrderStatusDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetMaintenanceCenter()
        {

            var result = db.MaintenanceCenter.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.Name,
                Value = e.MaintenanceCenterID.ToString()

            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetJobStatus()
        {

            List<Helpers.Fleet.FleetBindingHelper.HelperObject> result = new List<Helpers.Fleet.FleetBindingHelper.HelperObject>
            {
                new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.Open },
                new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.WaitingTechnical },
                new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.WaitingAdminstrative },
                new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.Closed }
            };

            return result;

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderViewModel> GetJobList(string Identity, string category, string EquipmentName, string ProductionYear, string status, string FiscalYear, string MaintenaceCenter, string PlateNo, string SerialNo, string AssetCode, DateTime DateFrom, DateTime DateTo)
        {
            List<JobOrderViewModel> FleetList = new List<JobOrderViewModel>();
            JobOrderViewModel JobOrderModel;
            List<Fleets> Fleets= db.Fleets.ToList();
            GLPeriod Periods = new GLPeriod();

              


                        if (!string.IsNullOrEmpty(Identity))
                        {
                            if (!string.IsNullOrEmpty(category))
                            {
                                Fleets =   Fleets.Where(   so =>    so.EquipmentIdentityID.ToString() == Identity &&  so.EquipmentTypeID.ToString() == category).ToList();
                            }
                            else
                            {

                                Fleets = Fleets.Where(so =>   so.EquipmentIdentityID.ToString() == Identity).ToList();
                            }

                        }
          

                        if (!string.IsNullOrEmpty(EquipmentName))
                        {
                            Fleets = Fleets.Where(so => so.EquipmentName == EquipmentName).ToList();
                        } 
                        if (!string.IsNullOrEmpty(ProductionYear))
                        {
                            Fleets = Fleets.Where(x => x.ManufacturerYear == ProductionYear).ToList();
                        }
                        if (!string.IsNullOrEmpty(SerialNo))
                        {
                            Fleets = Fleets.Where(x => x.SerialNo == SerialNo).ToList();
                        }
                        if (!string.IsNullOrEmpty(PlateNo))
                        {
                            Fleets = Fleets.Where(x => x.PlateNumber == PlateNo).ToList();
                        }
                        if (!string.IsNullOrEmpty(AssetCode))
                        {
                            Fleets = Fleets.Where(x => x.UnitNum == AssetCode).ToList();
                        }
 

                foreach (var f in Fleets)
                {
                    //List<JobOrder> jobs = db.JobOrder.Where(x => x.FleetsID == f.FleetsID && x.DateIssued >= DateFrom && x.DateIssued <= DateTo).ToList();

                    //if (!string.IsNullOrEmpty(status))
                    //{
                    //    jobs = jobs.Where(x => x.Status == status).ToList();
                    //}

                    //if (!string.IsNullOrEmpty(MaintenaceCenter))
                    //{
                    //    jobs = jobs.Where(x => x.MaintenanceCenter == MaintenaceCenter).ToList();
                    //}
                    //foreach (var job in jobs)
                    //{


                    //    int duration = 0;
                    //    decimal Rate = 0;
                    //    decimal Efficiency = 0;

                    //    var OrderLine = db.JobOrder.Where(x => x.JobOrderID == job.JobOrderID).FirstOrDefault();


                    //    if (!OrderLine.IsOnlineTransferred)
                    //    {
                    //        TimeSpan Duration = DateTime.Now - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                    //        duration = Duration.Minutes;
                    //    }
                    //    else if (OrderLine.IsOnlineTransferred)
                    //    {
                    //        TimeSpan Duration = DateTime.Parse(OrderLine.OnlineTransferredTime.ToString()) - DateTime.Parse(OrderLine.OnlineApprovedTime.ToString());
                    //        duration = Duration.Minutes;
                    //    }



                    //    var services = db.JobOrderDetail.Where(x => x.JobOrderID == job.JobOrderID).ToList();

                    //    foreach (var service in services)
                    //    {
                    //        var Std = db.Service.Where(x => x.ServiceName == service.ServiceName).FirstOrDefault();
                    //        if (Std != null && Std.STDTime != null)
                    //        {
                    //            Rate = Rate + decimal.Parse(Std.STDTime.ToString());
                    //        }

                    //    }

                    //    if (duration == 0)
                    //    {
                    //        duration = 1;
                    //    }

                    //    Efficiency = (Rate / duration) * 100;




                    //    JobOrderModel = new JobOrderViewModel
                    //    {
                    //        EquipmentName = job.EquipmentName
                    //    };
                    //    if (job.Status == MaintenanceConst.Closed)
                    //    {
                    //        JobOrderModel.DateCompleted = job.DateCompleted;
                    //    }

                    //    JobOrderModel.DateIssued = job.DateIssued;
                    //    JobOrderModel.EquipmentName = job.EquipmentName;
                    //    JobOrderModel.InspectedBy = job.InspectedBy;
                    //    JobOrderModel.Efficency = Efficiency;
                    //    JobOrderModel.JobOrderNum = job.Num;
                    //    JobOrderModel.LabourCost = duration;
                    //    JobOrderModel.MaintenanceCenter = job.MaintenanceCenter;
                    //    JobOrderModel.MaterialCost = Rate;
                    //    JobOrderModel.PercentCompleted = job.PercentCompleted;
                    //    JobOrderModel.PlateNum = job.AssetNum;
                    //    JobOrderModel.ProjectName = job.ProjectName;
                    //    JobOrderModel.Status = job.Status;
                    //    JobOrderModel.Type = job.Type;


                    //    FleetList.Add(JobOrderModel);
                    //}



                }


 

            return FleetList;
        }

    }
}