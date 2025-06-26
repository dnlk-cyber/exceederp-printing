using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class RentFromExternalPrintOds
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public RentFromExternalPrintViewModel GetRentFromExternalPrint(string rentId)
        {
            if(Guid.TryParse(rentId, out Guid rentFromExternalId))
            {
                var rentFromExternal = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == rentFromExternalId);
                if(rentFromExternal != null)
                {
                    var equip = db.RentFromExternalEquipments.FirstOrDefault(x => x.RentFromExternalId == rentFromExternal.RentFromExternalID);

                    if (equip != null)
                    {

                        var supp = db.ProcurementSuppliers.FirstOrDefault(x => x.ProcurementSupplierID == rentFromExternal.ProcurementSupplierID);
                        var branch = db.Branch.Find(rentFromExternal.BranchId);
                        return new RentFromExternalPrintViewModel
                        {                           
                            
                            SupplierName = supp?.Name ?? "",
                            ProectLocation = branch?.Name ?? "",
                            AdvancePayment = rentFromExternal.AdvancePayment,
                            CompanyAddress = rentFromExternal.CompanyAddress,
                            EndDate = rentFromExternal.EndDate,
                            StartDate = rentFromExternal.StartDate,
                            Hour = rentFromExternal.Hour,
                            HpCC = rentFromExternal.HP_CC,
                            Operator = rentFromExternal.Operator,
                            Quantity = rentFromExternal.Quantity,
                            UnitOfWork = rentFromExternal.UnitOfWorkMeasurement,
                            WorkingCapacity = rentFromExternal.WorkingCapacity,
                            ApprovedBy = rentFromExternal.OnlineApprovedBy,
                            CreateBy = rentFromExternal.CreatedBy,
                            DateApproved = rentFromExternal.OnlineApprovedTime,
                            DateCreated = rentFromExternal.DateCreated
                        };
                   }

                }
            }
            return new RentFromExternalPrintViewModel();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RentFromExternalPrintViewModel> GetEquipments(string rentId)
        {
            List<RentFromExternalPrintViewModel> list = new List<RentFromExternalPrintViewModel>();
            if (Guid.TryParse(rentId, out Guid rentFromExternalId))
            {
                var rentFromExternal = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == rentFromExternalId);
                if (rentFromExternal != null)
                {
                    var equips = db.RentFromExternalEquipments.Where(x => x.RentFromExternalId == rentFromExternal.RentFromExternalID);
                    foreach (var item in equips)
                    {
                        var cat = db.EquipmentIdentity.Find(item.EquipmentIdentityID)?.EquipmentCategoryName;
                        var type = db.EquipmentType.Find(item.EquipmentTypeID)?.EquipmentTypeName;
                        var name = db.EquipmentName.Find(item.EquipmentNameID)?.Name;
                        var fleet = db.Fleets.Find(item.FleetId);
                        list.Add(new RentFromExternalPrintViewModel
                        {
                            EquipmentCategory = cat,
                            EquipmentType = type,
                            EquipmentName = name,
                            PlateNo = fleet?.PlateNumber ?? fleet?.SerialNo ?? "",
                            RentalRate = item.RentalRate,
                            IdleRentalRate = item.IdleRentalRate,
                            RentType = item.RentType.ToString(),
                        });
                    }
                }
            }
            return list;

        }
    }

    public class RentFromExternalPrintViewModel
    {
        public string ProectLocation { get; set; }
        public int? Quantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SupplierName { get; set; }
        public string CompanyAddress { get; set; }
        public string UnitOfWork { get; set; }
        public string WorkingCapacity { get; set; }
        public int? AdvancePayment { get; set; }
        public double Hour { get; set; }
        public string HpCC { get; set; }
        public string Operator { get; set; } 
        public string EquipmentCategory { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentName { get; set; }
        public string PlateNo { get; set; }
        public decimal RentalRate { get; set; }
        public decimal IdleRentalRate { get; set; }
        public string RentType { get; set; }
        public string CreateBy { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateApproved { get; set; } 

    }
}
