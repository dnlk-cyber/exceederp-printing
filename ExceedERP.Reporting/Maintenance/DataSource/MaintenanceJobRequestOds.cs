using ExceedERP.DataAccess.Context;
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
    public class MaintenanceJobRequestOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public MaintenanceJobRequestViewModel GetData(string id)
        {
            MaintenanceJobRequestViewModel model = new MaintenanceJobRequestViewModel();
            Guid jobRequestId = new Guid(id);

            var jobRequest = db.JobRequest.Find(jobRequestId);

            if (jobRequest != null)
            {
                var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
                Image LogoFileName = null;
                string AmharicName = "";
                string Name = "";
                if (organization != null)
                {
                    Name = organization.TradeName;
                    AmharicName = organization.AmharicName;
                    var logo =
                                       db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                               a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                    if (logo != null)
                    {
                        LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }

                model.LogoFileName = LogoFileName;
                model.Name = Name;
                model.AmharicName = AmharicName;
                model.Date = DateTime.Now.ToString("yyyy-MM-dd");
                
                var customerName = db.OrganizationCustomers.Find(jobRequest.CustomerId);
                if (customerName != null)
                {
                    model.CustomerName = customerName.TradeName;
                }
                model.Date = (jobRequest.DateCreated)?.ToString("dd/MM/yyyy");
                //model.IsPM = jobRequest.IsPM;
                if (jobRequest.IsPM)
                {
                    model.IsPM = "Yes";
                }
                model.LastKmHr = jobRequest.LastKmHr;
                model.LicensePlate = jobRequest.LicensePlate;
                model.Odometer = jobRequest.Odometer;
                model.EquipmentName = jobRequest.EquipmentName;
                model.Source = jobRequest.Source.ToString();
                var requester = db.Person_Employee.Find(jobRequest.RequesterId);
                if (requester != null)
                {
                    model.Requester = requester.FirstNameEnglish + " " + requester.MiddleNameEnglish + " " + requester.LastNameEnglish;
                }
                model.LicensePlate = jobRequest.LicensePlate;
                model.OperatorName = jobRequest.OperatorName;
                var maintenanceCenter = db.MaintenanceCenter.Find(jobRequest.MaintenanceCenterId);
                if (maintenanceCenter != null)
                {
                    model.MaintenanceCenter = maintenanceCenter.Name;
                }
                var vehicleReceptionForm = db.VehicleReceptionForm.Find(jobRequest.VeheicleReceptionFormId);
                if (vehicleReceptionForm != null)
                {
                    model.FuelAmount = vehicleReceptionForm.Fuel.ToString();
                    model.Model = vehicleReceptionForm.VehicleModel;
                    model.EquipmentType = vehicleReceptionForm.VehicleType;
                    var equipmentCategory = db.EquipmentIdentity.Find(vehicleReceptionForm.VehicleCategoryId);
                    if (equipmentCategory != null)
                    {
                        model.EquipmentCategory = equipmentCategory.EquipmentCategoryName;
                    }
                    
                }
                model.RequestNumber = jobRequest.Num;
                
                

            }
            return model;
        }
    }
}
