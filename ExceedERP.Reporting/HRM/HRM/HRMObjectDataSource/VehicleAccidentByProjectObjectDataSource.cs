using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class VehicleAccidentByProjectObjectDataSource : List<VehicleAccidentByProject>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<VehicleAccidentByProject> GetVehicleAccidentByProject(string period)
        {
            List<VehicleAccidentByProject> InfoList = new List<VehicleAccidentByProject>();
            VehicleAccidentByProject Info = new VehicleAccidentByProject();
            var allprojects = db.Branch.Where(x => x.BranchTypes == BranchTypes.PROJECT).ToList();

            if (period == null)
            {

                var mainPeriod = db.GLFiscalYears.Where(x => x.Name == period).FirstOrDefault();

                foreach (var b in allprojects)
                {
                    Info = new VehicleAccidentByProject();
                    Info.ProjectName = b.Name;
                    if (mainPeriod != null)
                    {
                        var asstesPeriodOne = db.FixedAssetAccidentRecords.
                            Where(x => x.GlFiscalYear.Name == mainPeriod.Name && x.BranchId == b.BranchId);
                        Info.PeriodOne = asstesPeriodOne.Sum(x => x.Amount);
                        Info.PeriodOneCount = asstesPeriodOne.Count();
                        var secondPeriod =
                            db.GLFiscalYears.Where(x => x.DateTo < mainPeriod.DateFrom && x.DateFrom <= mainPeriod.DateTo)
                                .FirstOrDefault();
                        if (secondPeriod != null)
                        {
                            var asstesPeriodTwo = db.FixedAssetAccidentRecords.
                                Where(x => x.GlFiscalYear.Name == secondPeriod.Name && x.BranchId == b.BranchId);
                            Info.PeriodTwo = asstesPeriodTwo.Sum(x => x.Amount);
                            Info.PeriodTwoCount = asstesPeriodTwo.Count();
                        }
                    }
                    InfoList.Add(Info);
                }
            }

            return InfoList;
        }
    }
}