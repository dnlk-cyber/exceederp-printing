using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class JobRankObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Rank> GetJobRank()
        {
            List<Rank> list = new List<Rank> { };


            var ranks = db.Ranks.ToList();
            if (!ranks.Any())
            {
                return list;
            }

            return ranks;
        }
    }
}