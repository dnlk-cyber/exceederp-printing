using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Reporting.Construction.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    //class GeneralOds
    //{
    //}

    [DataObject]
    public class ProjectsOds : List<ProjectViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectViewModel> GetAvailableProjects()
        {
            var list = new List<ProjectViewModel>();
            var projects = db.ProjectInformation.ToList();

            if (projects.Any())
            {
                foreach (ProjectInformation project in projects)
                {
                    var record = new ProjectViewModel
                    {
                        Name = project.Name,
                        ProjectInformationId = project.ProjectInformationId
                    };
                    list.Add(record);
                }
            }
            return list;
        }
    }

    /// <summary>
    /// Building projects only
    /// </summary>
    [DataObject]
    public class BuildingProjectsOds : List<ProjectViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectViewModel> GetAvailableProjects()
        {
            var list = new List<ProjectViewModel>();
            var projects = db.ProjectInformation.Where(p => p.ProjectType == AvailableProjectTypes.Building).ToList();

            if (projects.Any())
            {
                foreach (ProjectInformation project in projects)
                {
                    var record = new ProjectViewModel
                    {
                        Name = project.Name,
                        ProjectInformationId = project.ProjectInformationId
                    };
                    list.Add(record);
                }
            }
            return list;
        }
    }
    /// <summary>
    /// Road projects only
    /// </summary>
    [DataObject]
    public class RoadProjectsOds : List<ProjectViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectViewModel> GetAvailableProjects()
        {
            var list = new List<ProjectViewModel>();
            var projects = db.ProjectInformation.Where(p => p.ProjectType == AvailableProjectTypes.Road).ToList();

            if (projects.Any())
            {
                foreach (ProjectInformation project in projects)
                {
                    var record = new ProjectViewModel
                    {
                        Name = project.Name,
                        ProjectInformationId = project.ProjectInformationId
                    };
                    list.Add(record);
                }
            }
            return list;
        }
    }


    [DataObject]
    public class SubProjectTypesOds : List<SubProjectTypeViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SubProjectTypeViewModel> GetAvailableSubProjectTypes(string projectId)
        {
            int id = Convert.ToInt32(projectId);

            var list = new List<SubProjectTypeViewModel>();
            var subProjects = db.SubProjectType.Where(t => t.ProjectInformationId == id).ToList();

            if (subProjects.Any())
            {
                foreach (SubProjectType subProject in subProjects)
                {
                    var record = new SubProjectTypeViewModel
                    {
                        Name = subProject.Name,
                        Id = subProject.SubProjectTypeId
                    };
                    list.Add(record);
                }
            }
            return list;
        }
    }

    [DataObject]
    public class SubSubProjectsOds : List<SubSubProjectViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SubSubProjectViewModel> GetAvailableSubSubProjects(string subProjectTypeId)
        {
            int id = Convert.ToInt32(subProjectTypeId);

            var list = new List<SubSubProjectViewModel>();
            var subSubProjects = db.Blocks.Where(t => t.SubProjectTypeId == id).ToList();

            if (subSubProjects.Any())
            {
                foreach (Block subSubProject in subSubProjects)
                {
                    var record = new SubSubProjectViewModel
                    {
                        Name = subSubProject.Name,
                        Id = subSubProject.BlockId
                    };
                    list.Add(record);
                }
            }
            return list;
        }
    }

    [DataObject]
    public class SubPeriodsOds : List<SubPeriodViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SubPeriodViewModel> GetAvailableSubPeriods(string periodId)
        {
            int id = Convert.ToInt32(periodId);

            var list = new List<SubPeriodViewModel>();
            var subPeriods = db.ConstructionSubPeriod.Where(sp => sp.ConstructionPeriodId == id).ToList();

            if (subPeriods.Any())
            {
                foreach (ConstructionSubPeriod subPeriod in subPeriods)
                {
                    var record = new SubPeriodViewModel
                    {
                        Name = subPeriod.Name,
                        Id = subPeriod.ConstructionSubPeriodId
                    };
                    list.Add(record);
                }
            }
            return list;
        }
    }

    [DataObject]
    public class ProjectTypeOds : List<ProjectTypeViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectTypeViewModel> GetAvailableProjectTypes()
        {
            var list = new List<ProjectTypeViewModel>();
            int[] enumValues = new int[] { 0, 1, 2, 3 };
            foreach (int enumValue in enumValues)
            {
                var record = new ProjectTypeViewModel
                {
                    Id = enumValue,
                    Name = Enum.GetName(typeof(AvailableProjectTypes), enumValue)
                };
                list.Add(record);
            }

            //if (projectTypes.Any())
            //{
            //    foreach (ProjectType projectType in projectTypes)
            //    {
            //        var record = new ProjectTypeViewModel
            //        {
            //            Name = projectType.Name,
            //            Id = projectType.ProjectTypeId
            //        };
            //        list.Add(record);
            //    }
            //}
            return list;
        }
    }
    [DataObject]
    public class BranchOds : List<BranchViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BranchViewModel> GetAllBranches()
        {
            List<BranchViewModel> list = new List<ViewModel.BranchViewModel>();

            var availableBranches = _db.Branch.ToList();
            if (availableBranches.Any())
            {
                foreach (Branch branch in availableBranches)
                {
                    var record = new BranchViewModel
                    {
                        Id = branch.BranchId,
                        Name = branch.Name,
                    };
                    list.Add(record);
                }
            }
            return list;
        }
    }

    [DataObject]
    public class StructureTypeOds : List<StructureVM>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StructureVM> GetStructureTypesUnderGivenProjectType( string projectType)
        {
            List<StructureVM> list = new List<StructureVM>();
            var id = Convert.ToInt32(projectType);
            var selectedProjectType = (AvailableProjectTypes)id;
            var workItems = _db.ProjectEstimationWorkItem.Where(w => w.ProjectType == selectedProjectType).ToList();
            var categoryIds = new HashSet<int>(workItems.Select(w => w.WorkCategoryId));
            var workCategories = _db.WorkCategory.Where(w => categoryIds.Contains(w.WorkCategoryId)).ToList();
            var strIds = new HashSet<int>(workCategories.Select(w => w.ConfigurationId));
            var structures = _db.Configurations.Where(cn => strIds.Contains(cn.ConfigurationId)).ToList();
            if (structures.Any())
            {
                foreach (Configuration configuration in structures)
                {
                    var record = new StructureVM
                    {
                        Id = configuration.ConfigurationId,
                        Name = configuration.CurrentValue,
                    };
                    list.Add(record);
                }
            }
            return list;
        }
    }

    [DataObject]
    public class WorkCatogoryOds : List<WorkCategoryVM>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WorkCategoryVM> GetWorkCategories(string projectType, string strTypeId)
        {
            List<WorkCategoryVM> list = new List<WorkCategoryVM>();
            var id = Convert.ToInt32(projectType);
            var strId = Convert.ToInt32(strTypeId);
            var selectedProjectType = (AvailableProjectTypes)id;
            var workItems = _db.ProjectEstimationWorkItem.Where(w => w.ProjectType == selectedProjectType).ToList();
            var categoryIds = new HashSet<int>(workItems.Select(w => w.WorkCategoryId));
            var workCategories = _db.WorkCategory.Where(w => w.ConfigurationId == strId && (categoryIds.Contains(w.WorkCategoryId))).ToList();
            //var strIds = new HashSet<int>(workCategories.Select(w => w.ConfigurationId));
            //var structures = _db.Configurations.Where(cn => strIds.Contains(cn.ConfigurationId)).ToList();
            if (workCategories.Any())
            {
                foreach (WorkCategory category in workCategories)
                {
                    var record = new WorkCategoryVM
                    {
                        Id = category.WorkCategoryId,
                        Name = category.Name,
                    };
                    list.Add(record);
                }
            }
            return list;
        }
    }

}
