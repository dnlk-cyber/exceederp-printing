using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class PensionNumberObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Identification> GetPensionNumber()
        {
            List<Identification> list = new List<Identification> { };


            var identifications = db.Identifications.Where(x => x.Type == IdentificationType.PENSION_NO).ToList();
            if (!identifications.Any())
            {
                return list;
            }

            return identifications;
        }
    }
}