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
    public class GetPeriodODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GLPeriodVM> GetPeriod(string FiscalYearId)
        {
            List<GLPeriodVM> PeriodList = new List<GLPeriodVM>();
           List<GLPeriod> FetchPeriod = new List<GLPeriod>();
            if(int.TryParse(FiscalYearId,out int FId))
            {
               FetchPeriod = db.GLPeriods.Where(x=>x.GlFiscalYearId==FId).ToList();
               
            }
            var PeriodVm = new GLPeriodVM();
            
            
            foreach (var period in FetchPeriod)
            {
                var TypeVM = new GLPeriodVM();
                TypeVM.GLPeriodName = period.Name;
                TypeVM.GLPeriodId = period.GLPeriodId.ToString();
                PeriodList.Add(TypeVM);
            }
            return PeriodList;
        }
    }

public class GLPeriodVM
    {
        public string GLPeriodName { get; set; }
        public string GLPeriodId { get; set; }
    }


}
