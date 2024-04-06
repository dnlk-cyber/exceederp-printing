using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class BenefitODS
    {

        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BenefitVM> GetBenefit()
        {
           List<BenefitVM> list = new List<BenefitVM>();

            var benefits = db.OtherBenefits.ToList();
            foreach(var benefit in benefits)
            {
                var otherBenefit = new BenefitVM();
                var jobtitle = db.Lookups.Find(benefit.LookupId);
                if(jobtitle != null)
                {
                    otherBenefit.JobTitle = jobtitle.Description;
                }
               
                var benefitName = db.Lookups.Find(benefit.BenefitName);
                if(benefitName != null)
                {
                    otherBenefit.BenefitName = benefitName.Description;

                }
                var employees = db.JobTitleMapping.Where(x => x.LookupId == benefit.LookupId).FirstOrDefault();
                if(employees !=null)
                {
                    otherBenefit.NumberOfEmployee = employees.Quntity;
                }
                otherBenefit.Quantity = benefit.Quantity;
                otherBenefit.TotalQuantity = otherBenefit.NumberOfEmployee*benefit.Quantity;
                var uOM = db.Lookups.Find(benefit.UnitofMeasurement);
                otherBenefit.UnitOFMeasuremnet = uOM !=null?uOM.Description:" ";
                otherBenefit.Description = benefit.TimeInterval;
                list.Add(otherBenefit);

            }
           

            return list;
        }
    }
}
