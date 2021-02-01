using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class PayrollDepartmentObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CostCenterViewModel> GetPayrollDepartment()
        {
            List<CostCenterViewModel> centerViewModels = new List<CostCenterViewModel>();
           
            var list = from m in db.MainCompanyStructures
                join c in db.BranchCostCenter
                    on m.OrgStructureId equals c.OrgStructureId
                select (new { m,c });


            foreach (var l in list)
            {
                var costCenter = new CostCenterViewModel
                {
                    Name = l.m.Name,
                    OrgStructureId = l.m.OrgStructureId,
                    Code = l.c.Values
                };
                centerViewModels.Add(costCenter);
            }

            return centerViewModels.DistinctByField(x=>x.Code).ToList();
        }

        public List<MainCompanyStructure> GetPayrollDepartment1()
        {


            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            MainCompanyStructure structure = new MainCompanyStructure();
            var list = from m in db.MainCompanyStructures
                join c in db.BranchCostCenter
                    on m.OrgStructureId equals c.OrgStructureId
                select (new { m });


            if (list.Any())
            {
                foreach (var l in list)
                {
                    //structure.Name = l.m.;
                    allDepartment.Add(l.m);
                }

            }

            return allDepartment.Distinct().ToList();
        }
    }
}