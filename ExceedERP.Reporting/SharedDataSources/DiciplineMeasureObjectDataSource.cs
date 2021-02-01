using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class DiciplineMeasureObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DisciplineMeasures> GetDiciplineMeasure()
        {
            List<DisciplineMeasures> list = new List<DisciplineMeasures>();
            var Measures = db.DisciplineMeasures.ToList();
            if (!Measures.Any())
            {
                return list;
            }

            return Measures;//.
        }
    }
}