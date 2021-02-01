using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Construction.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    //class MinifiedBoQSummaryOds
    //{
    //}

    [DataObject]
    public class MinifiedBoQSummaryOds : List<MinBoQSummarySubProjectViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MinBoQSummarySubProjectViewModel> GetAllSubProjects(string projectId)
        {
            var list = new List<MinBoQSummarySubProjectViewModel>();
            if (projectId == null)
            {
                return list;
            }
            int id = Convert.ToInt32(projectId);
            var currentProject = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (currentProject != null)
            {
                #region Sub-Projects block or ..
                var blocks = db.Blocks.Where(b => b.ProjectInformationId == id).ToList();
                if (blocks.Any())
                {
                    foreach (Block block in blocks)
                    {
                        #region block cost calculation
                        var blockCost = new decimal();
                        var blockWorkItems = db.WorkItemPerProject.Where(w => w.BlockId == block.BlockId).ToList();
                        if (blockWorkItems.Any())
                        {
                            blockCost = blockWorkItems.Sum(wi => (wi.TotalUnitCost * wi.TotalQuantityOfWork));
                        }
                        #endregion

                        var blockRecord = new MinBoQSummarySubProjectViewModel
                        {
                            Id = block.BlockId,
                            Name = block.Name,
                            //TotalPrice = blockCost.ToString("N2", CultureInfo.InvariantCulture)
                        };

                        #region structures
                        #region  LINQ for structure types under this very block
                        var structureTypes = (from c in db.Configurations
                                              join w in db.ProjectTypeMappings
                                                  on c.ConfigurationId equals w.ConfigurationId

                                              join p in db.ProjectInformation
                                                  on w.ProjectType equals p.ProjectType
                                              join b in db.BlockMappings
                                              on c.ConfigurationId equals b.ConfigurationId

                                              where 
                                              p.ProjectInformationId == id &&
                                              c.Reference == "STRUCTURE_TYPE" &&
                                              b.BlockId == block.BlockId
                                              select (new
                                              {
                                                  c.CurrentValue,
                                                  c.ConfigurationId
                                              })).ToList();
                        #endregion
                        if (structureTypes.Any())
                        {
                            foreach (var structure in structureTypes)
                            {
                                
                                #region work categories under this very structure
                                var workCategoriesUnderStructure = db.WorkCategory.Where(wc => wc.ConfigurationId == structure.ConfigurationId).ToList();
                                if (workCategoriesUnderStructure.Any())
                                {
                                    foreach (WorkCategory category in workCategoriesUnderStructure)
                                    {
                                        #region total cost of work category calculation

                                        var categoryCost = new decimal();
                                        var categoryWorkItems = db.WorkItemPerProject.Where(w => w.WorkCategoryId == category.WorkCategoryId).ToList();
                                        if (categoryWorkItems.Any())
                                        {
                                            categoryCost = categoryWorkItems.Sum(wi => (wi.TotalUnitCost * wi.TotalQuantityOfWork));
                                        }
                                        #endregion
                                        var categoryRecord = new MinBoQSummaryWorkCategoryViewModel
                                        {
                                            Name = category.Name,
                                            TotalPrice = categoryCost,
                                            StructureType = structure.CurrentValue
                                        };
                                        blockRecord.WorkCategoryList.Add(categoryRecord);
                                    }
                                }
                                #endregion
                            }
                        }
                        blockRecord.TotalPrice = blockRecord.WorkCategoryList.Sum(wc => wc.TotalPrice)
                            .ToString("N2", CultureInfo.InvariantCulture);
                        #endregion
                        list.Add(blockRecord);
                    }
                }
                #endregion
            }
            return list;
        }
    }
}
