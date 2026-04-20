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
    public class CategoryOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentCategoryVM> Getequipment()

        {
            List<EquipmentCategoryVM> list = new List<EquipmentCategoryVM>();
            var FetchList = db.EquipmentIdentity.ToList();
            foreach (var equipmentList in FetchList)
            {
                EquipmentCategoryVM cat = new EquipmentCategoryVM();
                cat.EquipmentCategoryName = equipmentList.EquipmentCategoryName;
                cat.EquipmentIdentityId = equipmentList.EquipmentIdentityID.ToString();
                list.Add(cat);

            }
            return list;
        }
    }


    public class EquipmentCategoryVM
    {
       public string EquipmentIdentityId { get; set; }
       public string EquipmentCategoryName { get; set; }
    }



}
