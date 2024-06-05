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
    public class VehicleReceptionFormOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public VehicleReceptionFormViewModel GetData(string id)
        {
            VehicleReceptionFormViewModel model = new VehicleReceptionFormViewModel();
            int.TryParse(id, out int vrcId);

            var vehicleReceptionForm = db.VehicleReceptionForm.Find(vrcId);

            if(vehicleReceptionForm!=null)
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
                IList<VehiclePartDamageLevel> vehiclePartDamageTypes1 = new List<VehiclePartDamageLevel>();
                IList<VehiclePartDamageLevel> vehiclePartDamageTypes2 = new List<VehiclePartDamageLevel>();
                var vehiclePartDamageTypeList = db.VehiclePartDamageType.ToList();
                var totalRecord = vehiclePartDamageTypeList.Count();
                var firstTable = totalRecord / 2 + totalRecord % 2;

                var secondTable = totalRecord - firstTable;
                int i = 0;
                foreach (var vpDt in vehiclePartDamageTypeList)
                {
                    VehiclePartDamageLevel vehiclePartDamageType = new VehiclePartDamageLevel();
                    vehiclePartDamageType.Code = vpDt.Code;
                    vehiclePartDamageType.Name = vpDt.Name;
                    if(i<firstTable)
                    {
                        vehiclePartDamageTypes1.Add(vehiclePartDamageType);

                    }
                    else
                    {
                        vehiclePartDamageTypes2.Add(vehiclePartDamageType);

                    }
                    i++;
                }
                model.VehiclePartDamageLevels1 = vehiclePartDamageTypes1;
                model.PlateNo = vehicleReceptionForm.PlateNo;
                model.VehicleType = vehicleReceptionForm.VehicleType;
                model.VehicleModel = vehicleReceptionForm.VehicleModel;
                model.Mileage = vehicleReceptionForm.Mileage;
                model.Fuel = vehicleReceptionForm.Fuel;
                model.Remark = vehicleReceptionForm.Remark;
                model.VehiclePartDamageLevels2 = vehiclePartDamageTypes2;
                var customer = db.Traders.Find(vehicleReceptionForm.CustomerId);
                if (customer != null)
                {
                    model.CustomersAuthorizedAgent = customer.FullName;

                }
                var category = db.EquipmentIdentity.Find(vehicleReceptionForm.VehicleCategoryId);
                if (category != null)
                {
                    model.VehicleCategory = category.EquipmentCategoryName;
                }
                var customerName = db.OrganizationCustomers.Find(vehicleReceptionForm.CustomerId);
                if (customerName != null)
                {
                    model.CustomerName = customerName.TradeName;
                }
                var cust = db.Traders.Find(vehicleReceptionForm.CustomerId);
                List<Contact> tells = new List<Contact>();
                if (cust != null)
                {
                    Contact tell = new Contact();
                    //model.CustomerName = cust.FullName;
                    model.FaxNo = cust.PhoneNumber;
                    tell.PhoneNo = "09233";
                    tell.PhoneNo = "09233";
                    tells.Add(tell);
                    model.Contacts = tells; 
                }
                var emp = db.Person_Employee.Find(vehicleReceptionForm.RecipientId);
                if(emp!=null)
                {
                    var Reception = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                    var techEmp = db.Person_Employee.Find(vehicleReceptionForm.TechnicianAssignedId);
                    var TechnicianAssigned = techEmp.FirstName + " " + techEmp.MiddleName + " " + techEmp.LastName;
                    model.TechnicianAssigned = TechnicianAssigned;
                    model.Reception = Reception;
                }
                var technicalAssigned = db.Person_Employee.Find(vehicleReceptionForm.TechnicianAssignedId);
                if (technicalAssigned != null)
                {
                    model.TechnicianAssigned =  technicalAssigned.FirstNameEnglish+" "+ technicalAssigned.MiddleName+" "+technicalAssigned.LastName;
                }
                model.Date = (vehicleReceptionForm.DateCreated)?.ToString("dd/MM/yyyy");
                IList<VehiclePart> VehicleParts1 = new List<VehiclePart>();
                IList<VehiclePart> VehicleParts2 = new List<VehiclePart>();
                var VehiclePartList = db.VehiclePartManagement.Where(q=>q.VehicleReceptionFormId == vrcId).ToList();
                 totalRecord = VehiclePartList.Count();
                 firstTable = totalRecord / 2 + totalRecord % 2;
                 secondTable = totalRecord - firstTable;
                 i = 0;
                foreach (var vehiclePart in VehiclePartList)
                {
                    VehiclePart vp = new VehiclePart();
                    vp.Part = db.VehiclePart.Find(vehiclePart.VehiclePartId)?.Name;
                    vp.Code = db.VehiclePartDamageType.Find(vehiclePart.VehiclePartDamageTypeId)?.Code;
                    vp.Quantity = vehiclePart.Quantity;
                    if(i<firstTable)
                    {
                        VehicleParts1.Add(vp);

                    }
                    else
                    {
                        VehicleParts2.Add(vp);

                    }
                    i++;
                }
                 model.VehicleParts1 = VehicleParts1;
                 model.VehicleParts2 = VehicleParts2;

            }
            return model;
        }
    }
}
