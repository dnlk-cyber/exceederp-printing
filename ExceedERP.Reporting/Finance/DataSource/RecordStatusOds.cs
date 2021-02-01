using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;


namespace ExceedERP.Reporting.Finance.DataSource
{
    [DataObject]
    public class RecordStatusOds : List<string>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetRecordStatusList()
        {
           return   Enum.GetNames(typeof(OperationStatuses)).ToList();;
        }

       
    }
}
