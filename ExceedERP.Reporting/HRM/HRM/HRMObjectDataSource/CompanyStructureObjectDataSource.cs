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
    public class CompanyStructureObjectDataSource:IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CompanyStructureVM> GetDepartments(string status, string businessUnit)
        {
            List<CompanyStructureVM> structures = new List<CompanyStructureVM>();

            // Filter based on status
            var filteredStructures = db.MainCompanyStructures.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Active")
                {
                    filteredStructures = filteredStructures.Where(x => x.IsActive);
                }
                else if (status == "InActive")
                {
                    filteredStructures = filteredStructures.Where(x => !x.IsActive);
                }
            }
            if (!string.IsNullOrEmpty(businessUnit))
            {
                int.TryParse(businessUnit, out int businessUnitid);
                var ids = db.JobTitleMapping.Where(x => x.BusinessUnitId == businessUnitid).Select(x => x.OrgStructureId);
                if (!string.IsNullOrEmpty(status))
                {
                    if (status == "Active")
                    {
                        ids = db.JobTitleMapping.Where(x => x.BusinessUnitId == businessUnitid && x.IsActive).Select(x => x.OrgStructureId);
                    }
                    else if (status == "InActive")
                    {
                        ids = db.JobTitleMapping.Where(x => x.BusinessUnitId == businessUnitid && !x.IsActive).Select(x => x.OrgStructureId);
                    }
                }
                if (ids.Any())
                {
                    filteredStructures = filteredStructures.Where(x => ids.Contains(x.OrgStructureId));

                }
            }
            // Get distinct parents
            var distinctParents = filteredStructures
                                  .Select(x => x.Parent)
                                  .Distinct()
                                  .ToList();

            foreach (var parentId in distinctParents)
            {
                var company = new CompanyStructureVM
                {
                    ChildStructure = new List<ChildStructureVM>()
                };

                // Get parent name
                var parent = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == parentId);
                if (parent != null)
                {
                    company.Parent = parent.Name;
                }

                // Get child structures filtered by status
                var childStructures = filteredStructures
                                      .Where(x => x.Parent == parentId)
                                      .ToList();

                foreach (var child in childStructures)
                {
                    company.ChildStructure.Add(new ChildStructureVM { Child = child.Name });
                }

                structures.Add(company);
            }

            return structures;
        }
        public void Dispose()
        {
            db.Dispose();
        }
        //public List<CompanyStructureVM> GetDepartments(string status,string businessUnit)
        //{
        //    List<CompanyStructureVM> structures = new List<CompanyStructureVM>();

        //    var companies = db.MainCompanyStructures.Select(x => x.Parent).Distinct().ToList();
        //    if (!string.IsNullOrEmpty(status))
        //    {
        //        if(status == "Active")
        //        {
        //            companies = db.MainCompanyStructures.Where(x => x.IsActive).Select(x => x.Parent).Distinct().ToList();

        //        }
        //        if (status == "InActive")
        //        {
        //            companies = db.MainCompanyStructures.Where(x => !x.IsActive).Select(x => x.Parent).Distinct().ToList();

        //        }
        //    }


        //    foreach (var com in companies)
        //    {
        //        var company = new CompanyStructureVM();
        //        company.ChildStructure = new List<ChildStructureVM>();

        //        var parentName = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == com);
        //        if (parentName != null)
        //        {
        //            company.Parent = parentName.Name;
        //        }

        //        var structs = db.MainCompanyStructures.Where(x => x.Parent == com).ToList();
        //        if (!string.IsNullOrEmpty(status))
        //        {
        //            if (status == "Active")
        //            {
        //                 structs = db.MainCompanyStructures.Where(x => x.Parent == com&&x.IsActive).ToList();

        //            }
        //            if (status == "InActive")
        //            {
        //                structs = db.MainCompanyStructures.Where(x => x.Parent == com && !x.IsActive).ToList();

        //            }
        //        }
        //        foreach (var st in structs)
        //        {
        //            var childClass = new ChildStructureVM();

        //            childClass.Child = st.Name;

        //            company.ChildStructure.Add(childClass);
        //        }
        //        structures.Add(company);
        //    }
        //    return structures;
        //}
    }
}
