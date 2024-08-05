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
    public class FleetParameters
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetEquipmentCategory()
        {
            var list = db.EquipmentIdentity.Select(s => new GenericVm
            {
                Text = s.EquipmentCategoryName,
                Value = s.EquipmentIdentityID.ToString()
            }).ToList();
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetEquipmentSubCategory(string categoryId)
        {
            Guid.TryParse(categoryId, out Guid id);
            var list = db.EquipmentType.Where(x => x.EquipmentIdentityID == id).Select(s => new GenericVm
            {
                Text = s.EquipmentTypeName,
                Value = s.EquipmentTypeID.ToString()
            }).ToList();
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetEquipmentSubCategory()
        {
           
            var list = db.EquipmentType.Select(s => new GenericVm
            {
                Text = s.EquipmentTypeName,
                Value = s.EquipmentTypeID.ToString()
            }).ToList();
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetFiscalYear()
        {
           
            var list = db.GLFiscalYears.Select(s => new GenericVm
            {
                Text = s.Name,
                Value = s.GlFiscalYearId.ToString()
            }).ToList();
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetGlPeriod(string fiscalId)
        {
            int.TryParse(fiscalId, out int fiscalYearId);
            var list = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYearId).Select(s => new GenericVm
            {
                Text = s.Name,
                Value = s.GLPeriodId.ToString()
            }).ToList();
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetEquipmentName(string equipmentTypeId)
        {
            Guid.TryParse(equipmentTypeId, out Guid id);
            var list = db.EquipmentName.Where(x => x.EquipmentTypeId == id).Select(s => new GenericVm
            {
                Text = s.Name,
                Value = s.EquipmentNameID.ToString()
            }).ToList();
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetEquipmentNameNoParameter()
        {
           
            var list = db.EquipmentName.Select(s => new GenericVm
            {
                Text = s.Name,
                Value = s.EquipmentNameID.ToString()
            }).ToList();
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetPlateNo(string equipmentNameId)
        {
            Guid.TryParse(equipmentNameId, out Guid id);
            var list = db.Fleets.Where(x => x.EquipmentNameID == id).Select(s => new GenericVm
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.FleetsID.ToString()
            }).ToList();
            return list;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GenericVm> GetPlateNo()
        {          
            var list = db.Fleets.Select(s => new GenericVm
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.FleetsID.ToString()
            }).ToList();
            return list;

        }
        public class GenericVm
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}
