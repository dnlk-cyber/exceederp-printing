using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class LetterTypeObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetLetterType()
        {
            List<string> sourceList = new List<string>
            {
                "INCOMING",

                "OUTGOING",

                "ALL"



            };

            return sourceList;
        }
    }
}