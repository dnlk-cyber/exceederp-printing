 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Reporting.Fleet.DataSource
{
     [DataObject]
    public class RentTimeSheetDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RentTimeSheetViewModel> GetSheetInfo(DateTime StartingDate, DateTime EndingDate, string Project, string Category, string Type, string EquipmentName, string PlateNo, string Model, string ChasisNo, string Serial)
        {
            List<RentTimeSheetViewModel> TimeSheetList = new List<RentTimeSheetViewModel>();

            var fleets = db.Fleets.ToList();
            FleetReportHelper fleetReportHelper = new FleetReportHelper();
            Fleets fleet = fleets.FilterSingleFleet(Category, Type, EquipmentName, PlateNo, ChasisNo, Serial, PlateNo, Project, Model);
            #region

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

            #endregion

            if (fleet != null)
            {

                decimal Workingcost = 0;
                decimal Workingtime = 0;
                decimal IdealCosttime = 0;
                string TypeOfWork = string.Empty;
                List<TimeSheet> c = db.TimeSheet.Where(x => x.FleetsID== fleet.FleetsID && x.IsExternal  &&   x.DateCreated >= StartingDate && x.DateCreated <= EndingDate).ToList();



                if (!string.IsNullOrEmpty(Project))
                {
                    c = c.Where(x => x.CustomerName == Project).ToList();
                }
                    

                foreach (var Work in c)
                {
                    TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) - DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                    TimeSpan AfterNoonDuration = DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());

                    Workingtime = Workingtime + (MorningDuration.Hours + AfterNoonDuration.Hours);
                    Workingcost = Workingtime * decimal.Parse(Work.RentalRate.ToString());
                    TypeOfWork = TypeOfWork + " - " + Work.TypeofWork;
                    IdealCosttime = IdealCosttime + ((Work.IdleTime + Work.BreakDownTime + Work.DownTime) * decimal.Parse(Work.IdealRentalRate.ToString()));
                    var fuel = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.PlateNo == fleet.PlateNumber && x.Issue.IsFuel && x.DateCreated >= Work.DateCreated ).ToList();
               
                    
                var TimeSheetsViewModel = new RentTimeSheetViewModel();


                TimeSheetsViewModel.Date = Work.Date;
                TimeSheetsViewModel.ByAmount = fleet.RentType.Equals(RentType.MeterCubePerKm);
                TimeSheetsViewModel.ByHourly = fleet.RentType.Equals(RentType.Hourly);
                TimeSheetsViewModel.ByDay = false;
                TimeSheetsViewModel.ByOther = fleet.RentType.Equals(RentType.Repetition);
                if (Work.CustomerName == "HO")
                {
                    TimeSheetsViewModel.IsRentHeadOffice = true;
                }
                else
                {
                    TimeSheetsViewModel.IsRentProject = true;
                }
                TimeSheetsViewModel.EquipmentType = fleet.EquipmentType.CategoryName;
                TimeSheetsViewModel.FuelIssue = fuel.Sum(x => x.IssuedQuantity);
                TimeSheetsViewModel.TotalFuelIssue += fuel.Sum(x=>x.IssuedQuantity);
                TimeSheetsViewModel.FinalOdometer = Work.FinalKMHrReading;
                TimeSheetsViewModel.FuelIssue = Work.KMHrReading;
                TimeSheetsViewModel.EquipmentType = fleet.EquipmentIdentity.EquipmentCategoryName;
                TimeSheetsViewModel.intialOdometer = Work.KMHrReading;
                TimeSheetsViewModel.Location = Work.CustomerName;
                TimeSheetsViewModel.Manufacturer = fleet.Manufacturer;
                TimeSheetsViewModel.UOW = "KM / HR";
                TimeSheetsViewModel.Model = fleet.Model;
                TimeSheetsViewModel.OperatorName = fleet.OperatorName;
                TimeSheetsViewModel.Project = Work.CustomerName;
                TimeSheetsViewModel.TypeofWork = Work.TypeofWork;
                TimeSheetsViewModel.Amount = (Work.FinalKMHrReading - Work.KMHrReading).ToString();
                TimeSheetsViewModel.SerialNo = fleet.SerialNo;
                TimeSheetsViewModel.AmharicName = AmharicName;
                TimeSheetsViewModel.Name = Name;
                TimeSheetsViewModel.LogoFileName = LogoFileName;
                TimeSheetsViewModel.Address = Address;
                TimeSheetList.Add(TimeSheetsViewModel);


                }


            }
            if(!TimeSheetList.Any())
            {

                var TimeSheetsViewModel = new RentTimeSheetViewModel();
                TimeSheetsViewModel.AmharicName = AmharicName;
                TimeSheetsViewModel.Name = Name;
                TimeSheetsViewModel.LogoFileName = LogoFileName;
                TimeSheetsViewModel.Address = Address;
                TimeSheetList.Add(TimeSheetsViewModel);
            }
            return TimeSheetList;

        }
    }
}

