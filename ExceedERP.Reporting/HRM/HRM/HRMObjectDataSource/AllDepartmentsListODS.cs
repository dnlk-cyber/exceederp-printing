using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
   
    [DataObject]
    public class AllDepartmentsListODS :  IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MainCompanyStructure> GetDepartments()
        {
            var allStructures = db.MainCompanyStructures
                                  .AsNoTracking()
                                  .ToList();

            var result = new List<MainCompanyStructure>();

            // Group by Parent for fast lookup
            var lookup = allStructures
                .GroupBy(x => x.Parent)
                .ToDictionary(g => g.Key, g => g.ToList());

            var added = new HashSet<int>();

            // Find true roots:
            // (items whose Parent doesn't exist as a valid OrgStructureId OR Parent == null/0)
            var allIds = new HashSet<int>( allStructures.Select(x => x.OrgStructureId));

            var roots = allStructures
                .Where(x => x.Parent == 0 || !allIds.Contains(x.Parent))
                .ToList();

            foreach (var root in roots)
            {
                Traverse(root, lookup, result, added);
            }

            return result;
        }
        private void Traverse(MainCompanyStructure node,Dictionary<int, List<MainCompanyStructure>> lookup,List<MainCompanyStructure> result,HashSet<int> added)
        {
            if (!added.Add(node.OrgStructureId))
                return;

            result.Add(node);

            if (lookup.TryGetValue(node.OrgStructureId, out var children))
            {
                foreach (var child in children)
                {
                    Traverse(child, lookup, result, added);
                }
            }
        }
        public void Dispose()
        {
            db.Dispose();
        }
       
    }
}

