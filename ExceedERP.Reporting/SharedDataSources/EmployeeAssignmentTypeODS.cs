using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class EmployeeAssignmentTypeODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetAssignmentTypes()
        {
            List<string> sourceList = new List<string>
            {


            "NEW",

            "PROMOTION",

            "YEARLY_PROMOTION",

            "TRANSFER",

            "DEMOTION",

            "RECOMENDATION",

            "ASSIGNMENT",

            "STRUCTURE_CHANGE"

            };

            return sourceList;
        }
    }
}
