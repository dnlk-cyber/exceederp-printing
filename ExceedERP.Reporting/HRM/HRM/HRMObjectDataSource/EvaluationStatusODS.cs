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
    public class EvaluationStatusODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetEvaluationStatus()
        {
            List<string> status = new List<string>();

            status.Add("የተሞላላቸው");
            status.Add("ያልተሞላላቸው");

            return status;
        }
    }
}
