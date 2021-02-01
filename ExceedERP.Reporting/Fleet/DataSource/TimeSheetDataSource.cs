using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    public class TimeSheetDataSource
    {

        ExceedDbContext db = new ExceedDbContext();
        /*
    [DataObjectMethod(DataObjectMethodType.Select)]
    public List<TimeSheetViewModel> GetSheetInfo(string Identity, string category, string ManufacturerYear, string Manufacturer,
       DateTime startingDate, DateTime endingDate)
    {
        List<TimeSheetViewModel> FleetList = new List<TimeSheetViewModel>();
         List<Fleets> Fleets;
            




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
                    db.Fleets.Where(so => so.EquipmentIdentityID.ToString() == Identity)
                        .ToList();
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(Manufacturer))
            {
                Fleets = db.Fleets.Where(so => so.ManufacturerYear == Manufacturer).ToList();
            }
            else
            {
                Fleets = db.Fleets.ToList();
            }
        }
        if (Fleets.Any())
        {
            if (!string.IsNullOrEmpty(Manufacturer))
            {
                Fleets = Fleets.Where(so => so.Manufacturer == Manufacturer).ToList();
            }
            if (!string.IsNullOrEmpty(ManufacturerYear))
            {
                Fleets = Fleets.Where(so => so.ManufacturerYear == ManufacturerYear).ToList();
            }
            List<Fleets> List = Fleets;
            string TakeFleet = string.Empty;
            string NeededFleet = string.Empty;
            foreach (Fleets sov in List)
            {


                List<TimeSheet> TimeSheets = db.TimeSheet.Where(x => x.FleetsID == sov.FleetsID  && x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();


                foreach (var c in TimeSheets)
                {

                    var TimeSheetsViewModel = new TimeSheetViewModel();

                    TimeSheetsViewModel.Equipment = sov.PlateNumber +" - "+sov.EquipmentName;
                    TimeSheetsViewModel.CustomerName = c.CustomerName;
                    TimeSheetsViewModel.Date = c.DateCreated;
                    TimeSheetsViewModel.DownTime = c.DownTime;
                    TimeSheetsViewModel.EquipmentAdministratorName = c.EquipmentAdministratorName;
                    TimeSheetsViewModel.IdleTime = c.IdleTime;
                    TimeSheetsViewModel.KMHrReading = c.KMHrReading;
                    TimeSheetsViewModel.Location = c.Location;
                    TimeSheetsViewModel.RentalRate = c.RentalRate;
                    TimeSheetsViewModel.TypeofWork = c.TypeofWork;
                    TimeSheetsViewModel.WorkingHoursAfternoonFrom = c.WorkingHoursAfternoonFrom;
                    TimeSheetsViewModel.WorkingHoursAfternoonTo = c.WorkingHoursAfternoonTo;
                    TimeSheetsViewModel.WorkingHoursMorningFrom = c.WorkingHoursMorningFrom;
                    TimeSheetsViewModel.WorkingHoursMorningto = c.WorkingHoursMorningto;

 
                    TimeSheetsViewModel.AmharicName = AmharicName;
                    TimeSheetsViewModel.Name = Name;
                    TimeSheetsViewModel.LogoFileName = LogoFileName;
                    TimeSheetsViewModel.Address = Address;

                    FleetList.Add(TimeSheetsViewModel);

                }
            }
        }
        return FleetList;

    }
}
         * */
    }
}

    