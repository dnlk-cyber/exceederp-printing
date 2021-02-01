using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
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
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
        public  class JobRequestDataSource
    {

        


        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobRequestViewModel> GetJobRequest(string Identity, string category, string EquipmentName, string Manufacturer, string PlateNo, string AssetCode, string SerialNo,
           DateTime startingDate, DateTime endingDate)
        {
            List<JobRequestViewModel> FleetList = new List<JobRequestViewModel>();
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
             
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    Fleets = db.Fleets.Where(so => so.ManufacturerYear == Manufacturer).ToList();
                }
                else
                {
                    Fleets = db.Fleets.ToList();
                }
            
            if (!string.IsNullOrEmpty(EquipmentName))
            {
                Fleets = Fleets.Where(so => so.EquipmentName == EquipmentName).ToList();
            }

            if (!string.IsNullOrEmpty(PlateNo))
            {
                Fleets = Fleets.Where(so => so.PlateNumber == PlateNo).ToList();
            }
            if (!string.IsNullOrEmpty(AssetCode))
            {
                Fleets = Fleets.Where(so => so.UnitNum == AssetCode).ToList();
            }
            if (!string.IsNullOrEmpty(SerialNo))
            {
                Fleets = Fleets.Where(so => so.SerialNo == SerialNo).ToList();
            }
            if (Fleets.Any())
            {
                foreach (var Fleet in Fleets)
                {
                    List<JobRequest> jobOrder = db.JobRequest.Where(x => x.DateCreated > startingDate && x.DateCreated < endingDate && x.FleetsID == Fleet.FleetsID).ToList();

                    foreach (var jb in jobOrder)
                    {

                        var RequestViewModel = new JobRequestViewModel();

                        RequestViewModel.EquipmentName = jb.EquipmentName;
                        RequestViewModel.JobOrderNum = jb.Num;

                        RequestViewModel.ProjectName = jb.ProjectName;

                        RequestViewModel.Status = jb.Status;
                        RequestViewModel.Date = jb.DateCreated;
                        FleetList.Add(RequestViewModel);

                    }
                }
            }

            return FleetList;
        }
    }
}

    

