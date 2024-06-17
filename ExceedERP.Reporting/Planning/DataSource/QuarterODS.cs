using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExceedERP.Reporting.Planning.DataSource
{
    [DataObject]
    public class QuarterODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetQuater()
        {
            List<string> quarter = new List<string>();

            quarter.Add("Quarter 1");
            quarter.Add("Quarter 2");
            quarter.Add("Quarter 3");
            quarter.Add("Quarter 4");

            return quarter;
        }
    }
}