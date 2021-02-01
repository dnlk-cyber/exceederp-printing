using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class DiciplineLevelObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DisciplineMeasuresDetail> GetDiciplineMeasure()
        {
            List<DisciplineMeasuresDetail> list = new List<DisciplineMeasuresDetail>();
            var Measures = db.DisciplineMeasuresDetail.ToList();
            if (!Measures.Any())
            {
                return list;
            }

            return Measures;//.
        }
    }
}