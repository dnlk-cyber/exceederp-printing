using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using  ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{
    [DataObject]
    public class DownTimeDataSource
    {
            ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetEquipmentType()
        {

            var result = db.EquipmentIdentity.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.EquipmentCategoryName,
                Value = e.EquipmentIdentityID.ToString()

            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetDurationType()
        {
            var list = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();

            var result = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = "Above 6 Month",
                Id = 1

            };
            list.Add(result);

             result = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = "Between 6  & 3 Month",
                Id = 2

            };
             list.Add(result);

             result = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = "Between 3  & 2 Month",
                Id = 3

            };
             list.Add(result);

              result = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = "Between 2  & 1 Month",
                Id = 4

            };

              list.Add(result);

              result = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = "Below 1 Month",
                Id = 5

            };
              list.Add(result);

            return list;

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DownTimeViewModel> GetDownTime(DateTime StartingDate, DateTime EndingDate, string Category, string Type, string PlateNo, string Model, string ChasisNo, string Serial, string Location, string Duration)
        {

            List<DownTimeViewModel> FleetList = new List<DownTimeViewModel>();
            DownTimeViewModel JobOrderModel;
            GLPeriod Periods = new GLPeriod();


     




           

            List<JobOrder> Jobs = db.JobOrder.ToList();


            if (!string.IsNullOrEmpty(Type))
            {
                Jobs = Jobs.Where(x => x.EquipmentTypeId.ToString() == Type).ToList();
            }
            foreach (var job in Jobs)
            {


                var Diff = (decimal) (DateTime.Now - DateTime.Parse( job.DateIssued.ToString())).TotalDays;

                if (string.IsNullOrEmpty(Duration))
                {
                    if (Diff > 180 && Duration == "1")
                    {
                        var Fleet = db.Fleets.Where(x => x.FleetsID == job.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            var model = new DownTimeViewModel();
                            model.DownCost = Fleet.RentalRate * Diff;
                            model.DownDate = job.DateIssued;
                            model.DownDuration = " Above 6 Month";
                            model.EquipmentType = job.EquipmentType;
                            model.PlateNo = Fleet.PlateNumber;
                            model.Model = Fleet.Model;

                             FleetList.Add(model);
                        }
                    }

                    if (Diff > 90 && Diff < 180 && Duration == "2") 
                    {
                        var Fleet = db.Fleets.Where(x => x.FleetsID == job.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            var model = new DownTimeViewModel();
                            model.DownCost = Fleet.RentalRate * Diff;
                            model.DownDate = job.DateIssued;
                            model.DownDuration = "Between 6 & 3 Month";
                            model.EquipmentType = job.EquipmentType;
                            model.PlateNo = Fleet.PlateNumber;
                            model.Model = Fleet.Model;

                             FleetList.Add(model);
                        }
                    }
                    if (Diff > 60 && Diff < 90 && Duration == "3")
                    {
                        var Fleet = db.Fleets.Where(x => x.FleetsID == job.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            var model = new DownTimeViewModel();
                            model.DownCost = Fleet.RentalRate * Diff;
                            model.DownDate = job.DateIssued;
                            model.DownDuration = "Between 3 & 2 Month";
                            model.EquipmentType = job.EquipmentType;
                            model.PlateNo = Fleet.PlateNumber;
                            model.Model = Fleet.Model;

                       
                            FleetList.Add(model);
                        }
                    }
                    if (Diff > 30 && Diff < 60 && Duration == "4")
                    {
                        var Fleet = db.Fleets.Where(x => x.FleetsID == job.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            var model = new DownTimeViewModel();
                            model.DownCost = Fleet.RentalRate * Diff;
                            model.DownDate = job.DateIssued;
                            model.DownDuration = "Between 1 & 2 Month";
                            model.EquipmentType = job.EquipmentType;
                            model.PlateNo = Fleet.PlateNumber;
                            model.Model = Fleet.Model;

                           
                            FleetList.Add(model);
                        }
                    }
                    if (Diff < 30 && Duration == "5")
                    {
                        var Fleet = db.Fleets.Where(x => x.FleetsID == job.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            var model = new DownTimeViewModel();
                            model.DownCost = Fleet.RentalRate * Diff;
                            model.DownDate = job.DateIssued;
                            model.DownDuration = "Less Than 1 Month";
                            model.EquipmentType = job.EquipmentType;
                            model.PlateNo = Fleet.PlateNumber;
                            model.Model = Fleet.Model;

                         
                            FleetList.Add(model);
                        }
                    }

                }
                else
                {
                    if (Diff > 180)
                    {
                        var Fleet = db.Fleets.Where(x => x.FleetsID == job.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            var model = new DownTimeViewModel();
                            model.DownCost = Fleet.RentalRate * Diff;
                            model.DownDate = job.DateIssued;
                            model.DownDuration = " Above 6 Month";
                            model.EquipmentType = job.EquipmentType;
                            model.PlateNo = Fleet.PlateNumber;
                            model.Model = Fleet.Model;

                              FleetList.Add(model);
                        }
                    }

                    if (Diff > 90 && Diff < 180)
                    {
                        var Fleet = db.Fleets.Where(x => x.FleetsID == job.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            var model = new DownTimeViewModel();
                            model.DownCost = Fleet.RentalRate * Diff;
                            model.DownDate = job.DateIssued;
                            model.DownDuration = "Between 6 & 3 Month";
                            model.EquipmentType = job.EquipmentType;
                            model.PlateNo = Fleet.PlateNumber;
                            model.Model = Fleet.Model;
                              FleetList.Add(model);
                        }
                    }
                    if (Diff > 60 && Diff < 90)
                    {
                        var Fleet = db.Fleets.Where(x => x.FleetsID == job.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            var model = new DownTimeViewModel();
                            model.DownCost = Fleet.RentalRate * Diff;
                            model.DownDate = job.DateIssued;
                            model.DownDuration = "Between 3 & 2 Month";
                            model.EquipmentType = job.EquipmentType;
                            model.PlateNo = Fleet.PlateNumber;
                            model.Model = Fleet.Model;

                           
                            FleetList.Add(model);
                        }
                    }
                    if (Diff > 30 && Diff < 60)
                    {
                        var Fleet = db.Fleets.Where(x => x.FleetsID == job.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            var model = new DownTimeViewModel();
                            model.DownCost = Fleet.RentalRate * Diff;
                            model.DownDate = job.DateIssued;
                            model.DownDuration = "Between 1 & 2 Month";
                            model.EquipmentType = job.EquipmentType;
                            model.PlateNo = Fleet.PlateNumber;
                            model.Model = Fleet.Model;

                         

                            FleetList.Add(model);
                        }
                    }
                    if (Diff < 30)
                    {
                        var Fleet = db.Fleets.Where(x => x.FleetsID == job.FleetsID).FirstOrDefault();
                        if (Fleet != null)
                        {
                            var model = new DownTimeViewModel();
                            model.DownCost = Fleet.RentalRate * Diff;
                            model.DownDate = job.DateIssued;
                            model.DownDuration = "Less Than 1 Month";
                            model.EquipmentType = job.EquipmentType;
                            model.PlateNo = Fleet.PlateNumber;
                            model.Model = Fleet.Model;

                         

                            FleetList.Add(model);
                        }
                    }
                }

            }



            return FleetList;
        }
    }
}
