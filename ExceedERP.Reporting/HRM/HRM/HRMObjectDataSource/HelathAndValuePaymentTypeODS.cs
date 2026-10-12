using ExceedERP.Core.Domain.Health;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class HelathAndValuePaymentTypeODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPaymentType()
        {

            List<string> paymentTypes = Enum.GetValues(typeof(HealthPaymentType))
                                            .Cast<HealthPaymentType>()
                                            .Select(at => at.ToString())
                                            .ToList();

            return paymentTypes;
        }
    }
}
