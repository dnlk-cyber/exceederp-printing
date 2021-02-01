using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Insurance.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Insurance.DataSource
{
   
    [DataObject]
    public class ComplatedWorksInBRPScheduleObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ComplatedWorksInBRPScheduleViewModel> GetComplatedWorks()
        {
            List<ComplatedWorksInBRPScheduleViewModel> complatedWorkList = new List<ComplatedWorksInBRPScheduleViewModel>();

            var accidents = db.Accident.ToList();
            foreach (var accident in accidents)
            {
                var complatedWork = new ComplatedWorksInBRPScheduleViewModel();

                complatedWorkList.Add(complatedWork);
            }

            return complatedWorkList;
        }


    }
}
