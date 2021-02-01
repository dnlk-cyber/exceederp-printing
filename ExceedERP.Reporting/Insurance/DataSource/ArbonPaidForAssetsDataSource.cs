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
    public class ArbonPaidForAssetsDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ArbonPaidForAssetsViewModel> GetArbonPaidAmount()
        {
            List<ArbonPaidForAssetsViewModel> arbonPaidAmountList = new List<ArbonPaidForAssetsViewModel>();

            var accidents = db.Accident.ToList();
            foreach (var accident in accidents)
            {
                var arbonPaidAmount = new ArbonPaidForAssetsViewModel();

                arbonPaidAmountList.Add(arbonPaidAmount);
            }

            return arbonPaidAmountList;
        }


    }
}
