using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class WorkunitObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MainCompanyStructure> GetWorkunits()
        {
            List<MainCompanyStructure> list = new List<MainCompanyStructure> { };

            var deptlevelOne = db.MainCompanyStructures.ToList();
            if (deptlevelOne.Any())
            {
                list.AddRange(deptlevelOne);
            }


            return list;
        }
    }
}