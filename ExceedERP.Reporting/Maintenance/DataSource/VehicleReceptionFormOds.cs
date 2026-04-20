using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Maintenance.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
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
    public class VehicleReceptionFormOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public VehicleReceptionViewModel GetData(string id)
        {
            VehicleReceptionViewModel model = new VehicleReceptionViewModel();
            int.TryParse(id, out int vrcId);

            var vehicleReceptionForm = db.VehicleReceptionForm.Find(vrcId);
            //if (vehicleReceptionForm != null)
            //{
            //var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            //Image LogoFileName = null;
            //string AmharicName = "";
            //string Name = "";
            //if (organization != null)
            //{
            //    Name = organization.TradeName;
            //    AmharicName = organization.AmharicName;
            //    var logo =
            //                       db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
            //                               a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
            //    if (logo != null)
            //    {
            //        LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
            //    }
            //}

            //model.LogoFileName = LogoFileName;
            //model.Name = Name;
            //model.AmharicName = AmharicName;
            if (vehicleReceptionForm != null)
            {
                model.Date = DateTime.Now.ToString("yyyy-MM-dd");
                model.Reciever = "";
                model.CurrentLocation = "";
                model.DateOfInventory = "";
                model.EquipmentModel = vehicleReceptionForm.VehicleModel ?? "";
                model.Deliverer = "";
                model.PlateNumber = vehicleReceptionForm.PlateNo ?? "";
                model.ReasonForHandingover = "";
                model.YOM = "";
                model.Remark = vehicleReceptionForm.Remark ?? "";
                model.CustomerPhoneNo = "";
                model.InspectedDate = vehicleReceptionForm.DateCreated?.ToString("yyyy-MM-dd") ?? string.Empty;

                // Handle null safely when fetching the Inspector's name
                var inspector = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == vehicleReceptionForm.TechnicianAssignedId)?.FullNameaEnglish;
                model.Inspector = inspector ?? ""; // If inspector is null, assign an empty string

                model.VehicleParts = new List<VehiclePart>();

                var fetchParts = db.VehiclePartManagement.Where(p => p.VehicleReceptionFormId == vehicleReceptionForm.VehicleReceptionFormId).ToList();
                if (fetchParts.Any())
                {
                    foreach (var part in fetchParts)
                    {
                        var vehiclePart = new VehiclePart();

                        // Handle potential null values in related tables
                        vehiclePart.AV = db.VehiclePart.SingleOrDefault(x => x.VehiclePartId == part.VehiclePartDamageTypeId)?.Name ?? "";
                        vehiclePart.Quantiti = part.Quantity.ToString() ?? "0";
                        vehiclePart.DamageType = db.VehiclePartDamageType.FirstOrDefault(x => x.VehiclePartDamageTypeId == part.VehiclePartDamageTypeId)?.Name ?? "";

                        model.VehicleParts.Add(vehiclePart);
                    }
                }
            }

            return model;
        }
    }
    public class VehicleReceptionViewModel:Organization
    {
      public string Reciever { get; set; }
      public string CustomerPhoneNo { get; set; }
      public string YOM { get; set; }
      public string DateOfInventory { get; set; }
      public string EquipmentModel { get; set; }
      public string Deliverer { get; set; }
      public string PlateNumber { get; set; }
      public string ReasonForHandingover { get; set; }
      public string CurrentLocation { get; set; }
      public string Inspector { get; set; }
      public string InspectedDate { get; set; }
      public string Remark { get; set; }
      public IList<VehiclePart> VehicleParts { get; set; }
    
    }
    public class VehiclePart
    {
        public string Part { get; set; }
        public string AV { get; set; }
        public string Quantiti { get; set; }
        public string DamageType { get; set; }
        //public string UnOp { get; set; }
        //public string Scratch { get; set; }
        //public string Damage { get; set; }
        //public string Missed { get; set; }
    }


}