using System.Collections.Generic;
using System.ComponentModel;
using ExceedERP.DataAccess.Context;


namespace ExceedERP.Reporting.SharedDataSources
{

    [DataObject]
        public class InternalOrExternalExperienceODS
    {
            ExceedDbContext db = new ExceedDbContext();
            [DataObjectMethod(DataObjectMethodType.Select)]
            public List<string> GetTrainingType()
            {
                List<string> list = new List<string>();

                list.Add("Internal");
                list.Add("External");

                return list;
            }
        }
    }