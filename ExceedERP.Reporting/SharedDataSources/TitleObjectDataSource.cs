using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class TitleObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetTitle()
        {
            List<string> sourceList = new List<string>
            {

                "W_RO",
                "W_T",
                "ATO",
                //"CAPITAIN",
                //"PASTER",
                // "SHEK",
                // "LOWRET",
                "PROFFESOR",
                "BRIGADIER",
                "GENERAL",
                "CRENEL",
                "AMBASADOR",
                "SIR",
                // "MR",
                "MAJOR",
                //"BISHOP",
                // "POPE",
                // "HISEXCELLENCY",
                // "HONORABLE",
                // "MRS",
                "DR",
                "ENGINEER",
                "DIPLOMAT"



            };

            return sourceList;
        }
    }
}