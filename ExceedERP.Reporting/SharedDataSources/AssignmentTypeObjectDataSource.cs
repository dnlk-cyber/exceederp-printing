using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class AssignmentTypeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetAssignemnt()
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

                "STRUCTURE_CHANGE",

                "HOUSE_OF_REPRESENTATIVE",

                "JUDGES_ADMINISTRATION_OFFICE"

            };

            return sourceList;
        }
    }
}