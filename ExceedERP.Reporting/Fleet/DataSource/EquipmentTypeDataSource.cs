using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
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
        public class EquipmentTypeDataSource
        {
            ExceedDbContext db = new ExceedDbContext();
            [DataObjectMethod(DataObjectMethodType.Select)]
            public List<Fleets> GetEquipmentReport()
            {

                return db.Fleets.ToList();
            }


        }
        

   
}
