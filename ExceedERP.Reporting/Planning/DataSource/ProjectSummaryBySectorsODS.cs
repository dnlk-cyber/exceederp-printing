using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Planning.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Planning.DataSource
{
    public class ProjectSummaryBySectorsODS
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectSummaryBySectorsVM> GetProjectExecutiveSummaryData(string planyear,string periodId,string Sector,string projectTypeId,string projectKpiType)
        {

            var models = new List<ProjectSummaryBySectorsVM>();
            if (planyear != null)
            {
                var projectTypes = db.ConsProjectTypes.ToList();

                int.TryParse(planyear, out int glFiscalYearId);
                var projectActuals = db.ProjectActuals.Where(s => s.GlFiscalYearId == glFiscalYearId).ToList();
                var projectPlans = db.ProjectPlans.Where(s => s.GlFiscalYearId == glFiscalYearId).ToList();
             

                if (projectTypeId != null && projectTypes.Count() > 0)
                {
                    int.TryParse(projectTypeId, out int typeId);

                    projectTypes = projectTypes.Where(x => x.ConsProjectTypeId == typeId).ToList();
                }
                if (projectKpiType != null && projectActuals.Count() > 0)
                {
                    var kpiIds = db.ProjectKPIs.Where(x => x.ProjectKPIType.ToString() == projectKpiType.ToString()).Select(x => x.ProjectKPIId);
                    if (kpiIds.Any())
                    {
                        projectActuals = projectActuals.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();
                        projectPlans = projectPlans.Where(x => kpiIds.Contains(x.ProjectKPIId)).ToList();

                    }
                }
              
            }
            return models;

        }


                     
                   
           
          
    }
     
}       
        
       
    

