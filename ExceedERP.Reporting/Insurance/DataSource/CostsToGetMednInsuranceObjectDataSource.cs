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
    public class CostsToGetMednInsuranceObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CostsToGetMednInsuranceViewModel> GetCostsForInsurance()
        {
            List<CostsToGetMednInsuranceViewModel> costsForInsuranceList = new List<CostsToGetMednInsuranceViewModel>();

            var accidents = db.Accident.ToList();
            foreach (var accident in accidents)
            {
                var costsForInsurance = new CostsToGetMednInsuranceViewModel();

                costsForInsuranceList.Add(costsForInsurance);
            }

            return costsForInsuranceList;
        }


    }
}
