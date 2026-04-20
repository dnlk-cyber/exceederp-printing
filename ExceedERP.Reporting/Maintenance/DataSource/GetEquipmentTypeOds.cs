using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    
    [DataObject]
    public class GetEquipmentTypeOds
    {
       ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentTypeVM> GetType(string EquipmentIdentityId)

        {
            List<EquipmentTypeVM> list = new List<EquipmentTypeVM>();
            List<EquipmentType> TypeList = new List<EquipmentType>();

            if (Guid.TryParse(EquipmentIdentityId, out Guid catId))
            {
                TypeList = db.EquipmentType.Where(x => x.EquipmentIdentityID == catId).ToList();
             }
            foreach(var type in TypeList)
            {
                var TypeVM = new EquipmentTypeVM();
                TypeVM.EquipmentName = type.EquipmentTypeName;
                TypeVM.EquipmentTypeId = type.EquipmentTypeID.ToString();
                list.Add(TypeVM);
            }
            return list;
        }
    }
   
    public class EquipmentTypeVM
    {
        public string EquipmentTypeId { get; set; }
        public string EquipmentName { get; set; }
    }
}
