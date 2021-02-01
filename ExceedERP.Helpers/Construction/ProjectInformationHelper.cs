using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Construction
{
    public static class ProjectInformationHelper
    {
        private static ExceedDbContext db;

         static ProjectInformationHelper()
        {
            db = new ExceedDbContext();
        }
        public static string GetProjectLocation(int branchId) {
            var db = new ExceedDbContext();
            var branchName = "";
            var currentBranch = db.Branch.FirstOrDefault(b => b.BranchId == branchId);
            if(currentBranch != null)
            {
                branchName = currentBranch.Name;
            }
            return branchName;
        }

        public static string GetProjectClient(int projectId)
        {
            string client = null;
            var currentProject = db.ProjectInformation.Find(projectId);
            if (currentProject != null)
            {
                client = currentProject.Client;
            }
            return client;
        }

        public static string GetProjectName(int projectId) {
            var name = "";
            var currentProject = db.ProjectInformation.Find(projectId);
            if(currentProject != null)
            {
                name = currentProject.Name;
            }
            return name;
        }
        public static void CleanRemainingDataWhenProjectIsDeleted(int id)
        {
            var db = new ExceedDbContext();
            var currentProject = db.ProjectInformation.Find(id);
            if(currentProject != null)
            {
                var durationItems = db.WorkItemDuration.Where(d => d.Source == Core.Domain.Construction.DurationItemSource.Estimation && d.ProjectInformationId == id).ToList();
                var dIds = new HashSet<int>(durationItems.Select(i => i.WorkItemDurationId));
                var tasks = db.Tasks.Where(t => dIds.Contains(t.WorkItemDurationId.Value)).ToList();

                var workItems = db.WorkItemPerProject.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var pcRates = db.AreaWorkItemes.Where(p => p.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var scworkItems = db.SubContractWorkItem.Where(sc => sc.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var customWorkItems = db.ProjectCustomWorkItems.Where(c => c.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                var ids = new HashSet<int>(workItems.Select(w => w.WorkItemPerProjectId));
                var customContents = db.ProjectCustomWorkItemDetails.Where(c => ids.Contains(c.WorkItemPerProjectId)).ToList();

                var utlWorkItems = db.ResourceUtilizationWorkItem.Where(ru => ru.WorkItemPerProject.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                var blkMapping = db.BlockMappings.Where(bm => bm.Block.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var bloks = db.Blocks.Where(b => b.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                //var durationWorkItem = db.WorkItemDuration.Where(wd => wd.GenericWorkItemPerProject.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                //var durationItems = db.WorkItemDuration.Where(wd => ids.Contains(wd.WorkItemPerProjectId)).ToList();

                var genWorkItems = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var genIds = new HashSet<int>(genWorkItems.Select(w => w.GenericWorkItemPerProjectId));
                var genCustomDetails = db.GenericProjectCustomWorkItemDetails.Where(cd => genIds.Contains(cd.GenericWorkItemPerProjectId)).ToList();
                var genAreaWorkItems = db.GenericAreaWorkItem.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var genSubContractWorkItems = db.GenericSubContractWorkItem.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                //db.TaskDetail.RemoveRange(taskDetail);
                //db.SaveChanges();

                var contracts = db.ProjectContract.Where(c => c.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                db.ProjectContract.RemoveRange(contracts);
                db.GenericWorkItemPerProject.RemoveRange(genWorkItems);
                db.GenericProjectCustomWorkItemDetails.RemoveRange(genCustomDetails);
                db.GenericSubContractWorkItem.RemoveRange(genSubContractWorkItems);
                db.GenericAreaWorkItem.RemoveRange(genAreaWorkItems);
                db.SaveChanges();

                db.WorkItemDuration.RemoveRange(durationItems);
                db.SaveChanges();

                db.Tasks.RemoveRange(tasks);
                db.SaveChanges();

                db.ResourceUtilizationWorkItem.RemoveRange(utlWorkItems);
                db.SaveChanges();

                db.WorkItemPerProject.RemoveRange(workItems);
                db.ProjectCustomWorkItemDetails.RemoveRange(customContents);
                db.AreaWorkItemes.RemoveRange(pcRates);
                db.SubContractWorkItem.RemoveRange(scworkItems);
                db.ProjectCustomWorkItems.RemoveRange(customWorkItems);
                db.SaveChanges();

                db.BlockMappings.RemoveRange(blkMapping);
                db.SaveChanges();

                db.Blocks.RemoveRange(bloks);
                db.SaveChanges();

            }
        }
    }
}
