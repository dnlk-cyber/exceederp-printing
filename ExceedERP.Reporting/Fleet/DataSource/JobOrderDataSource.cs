using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class JobOrderDataSource
    {

        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetJobStatus()
        {

            List<Helpers.Fleet.FleetBindingHelper.HelperObject> result = new List<Helpers.Fleet.FleetBindingHelper.HelperObject>();

            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.Open });
            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.WaitingTechnical });
            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.WaitingAdminstrative });
            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.Closed });

            return result;

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderViewModel> GetJobList(string Identity, string category, string EquipmentName, string ProductionYear, string status, string FiscalYear, string Period)
        {
            List<JobOrderViewModel> FleetList = new List<JobOrderViewModel>();
            JobOrderViewModel JobOrderModel;
            List<Fleets> Fleets;
            GLPeriod Periods = new GLPeriod();

            FleetMaintenaceHelper fleetMaintenaceHelper = new FleetMaintenaceHelper();

            if (!string.IsNullOrEmpty(Period))
            {
                Periods =
                    db.GLPeriods.Where(x =>
                            x.GLPeriodId.ToString() == Period && x.GlFiscalYearId.ToString() == FiscalYear
                            ).FirstOrDefault();

            }





            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }



            if (!string.IsNullOrEmpty(Identity))
            {
                if (!string.IsNullOrEmpty(category))
                {
                    Fleets =
                        db.Fleets.Where(
                            so =>
                                so.EquipmentIdentityID.ToString() == Identity &&
                                so.EquipmentTypeID.ToString() == category)
                            .ToList();
                }
                else
                {

                    Fleets =
                                          db.Fleets.Where(
                                              so =>

                                                  so.EquipmentIdentityID.ToString() == Identity)
                                              .ToList();
                }

            }
            else
            {

                Fleets = db.Fleets.ToList();

            }

            if (!string.IsNullOrEmpty(EquipmentName))
            {
                Fleets = Fleets.Where(so => so.EquipmentName == EquipmentName).ToList();
            }
            if (Fleets.Any())
            {
                List<Fleets> List = new List<Fleets>();
                if (!string.IsNullOrEmpty(ProductionYear))
                {
                    List = Fleets.Where(x => x.ManufacturerYear == ProductionYear).ToList();
                }

                else
                {
                    List = Fleets;
                }
                foreach (var f in List)
                {
                    //List<JobOrder> jobs = db.JobOrder.Where(x => x.FleetsID == f.FleetsID && x.DateIssued > Periods.DateFrom && x.DateIssued < Periods.DateTo).ToList();

                    //if (!string.IsNullOrEmpty(status))
                    //{
                    //    jobs = jobs.Where(x => x.Status == status).ToList();
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
                    //        if (Std != null)
                    //        {
                    //            Rate = Rate + decimal.Parse(Std.STDTime.ToString());
                    //        }

                    //    }

                    //    if (duration == 0)
                    //    {
                    //        duration = 1;
                    //    }

                    //    Efficiency = (Rate / duration) * 100;

                    //    decimal LabourCost = 0;
                    //    var MechanicLogs = db.ServiceCrewWorkLogs.Where(x => x.JobId == job.JobOrderID).ToList();
                    //    IMechanicCrewService mechanicCrewService = new MechanicCrewService();



                    //    foreach (var log in MechanicLogs.DistinctByField(x => x.MecanicId))
                    //    {
                    //        var Crew = new ServiceCrew();
                    //        var Mechanic = mechanicCrewService.GetById(log.MecanicId);

                    //        if (Mechanic != null)
                    //        {
                    //            var MechanicLog = MechanicLogs.Where(x => x.MecanicId == log.MecanicId).ToList();

                    //            LabourCost += decimal.Parse(fleetMaintenaceHelper.GetMechanicDuration(MechanicLog).ToString()) * Mechanic.HourlyWage;

                    //        }
                    //    }



                    //    JobOrderModel = new JobOrderViewModel();

                    //    JobOrderModel.EquipmentName = job.EquipmentName;
                    //    JobOrderModel.DateCompleted = job.DateCompleted;
                    //    JobOrderModel.DateIssued = job.DateCreated;
                    //    JobOrderModel.EquipmentName = job.EquipmentName;
                    //    JobOrderModel.InspectedBy = job.InspectedBy;
                    //    JobOrderModel.Efficency = Efficiency;
                    //    JobOrderModel.JobOrderNum = job.Num;
                    //    JobOrderModel.LabourCost = LabourCost;
                    //    JobOrderModel.MaintenanceCenter = job.MaintenanceCenter;
                    //    JobOrderModel.MaterialCost = Rate;
                    //    JobOrderModel.PercentCompleted = job.PercentCompleted;
                    //    JobOrderModel.PlateNum = job.AssetNum;
                    //    JobOrderModel.ProjectName = job.ProjectName;
                    //    JobOrderModel.Status = job.Status;
                    //    JobOrderModel.Type = job.Type;

                    //    JobOrderModel.AmharicName = AmharicName;
                    //    JobOrderModel.Name = Name;
                    //    JobOrderModel.LogoFileName = LogoFileName;
                    //    JobOrderModel.Address = Address;

                    //    FleetList.Add(JobOrderModel);
                    //}



                }



            }

            return FleetList;
        }

    }
}