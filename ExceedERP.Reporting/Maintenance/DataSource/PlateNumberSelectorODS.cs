using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class PlateNumberSelectorODS
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GetStandardPlate> GetPlates()
        {
            List<GetStandardPlate> list = new List<GetStandardPlate>();

            var FetchPlates = db.JobOrder
                                .Select(x => x.LicensePlate)
                                .Distinct()
                                .ToList();

            // Ensure mapping to GetStandardPlate
            foreach (var plate in FetchPlates)
            {
                var SelectedPlate = new GetStandardPlate();
                SelectedPlate.PlateNumber = plate;
                list.Add(SelectedPlate);
            }

            return list;
        }
    }
    public class GetStandardPlate
    {
        public string PlateNumber { get; set; }
      
    }
}
