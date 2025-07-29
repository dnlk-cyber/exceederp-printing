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
    public class GlFiscalYearOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FiscalYearVM> Getequipment()

        {
            List<FiscalYearVM> list = new List<FiscalYearVM>();
            var FetchList = db.GLFiscalYears.ToList();
            foreach (var equipmentList in FetchList)
            {
                FiscalYearVM cat = new FiscalYearVM();
                cat.Name = equipmentList.Name;
                cat.GlFiscalYearId = equipmentList.GlFiscalYearId.ToString();
                list.Add(cat);

            }
            return list;
        }
    }


    public class FiscalYearVM
    {
       public string GlFiscalYearId { get; set; }
       public string Name { get; set; }
    }



}
