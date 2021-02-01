using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class TrainingOrEducationODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetTrainingType()
        {
            List<string> list = new List<string>();

            list.Add("TRAINING");
            list.Add("EDUCATION");

            return list;
        }
    }
}
