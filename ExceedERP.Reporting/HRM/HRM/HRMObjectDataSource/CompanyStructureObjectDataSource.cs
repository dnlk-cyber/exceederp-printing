using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class CompanyStructureObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CompanyStructureVM> GetDepartments()
        {
            List<CompanyStructureVM> structures = new List<CompanyStructureVM>();

            var companies = db.MainCompanyStructures.Select(x => x.Parent).Distinct().ToList();
            foreach (var com in companies)
            {
                var company = new CompanyStructureVM();
                company.ChildStructure = new List<ChildStructureVM>();

                var parentName = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == com);
                if (parentName != null)
                {
                    company.Parent = parentName.Name;
                }

                var structs = db.MainCompanyStructures.Where(x => x.Parent == com).ToList();
                foreach (var st in structs)
                {
                    var childClass = new ChildStructureVM();

                    childClass.Child = st.Name;

                    company.ChildStructure.Add(childClass);
                }
                structures.Add(company);
            }
            return structures;
        }
    }
}
