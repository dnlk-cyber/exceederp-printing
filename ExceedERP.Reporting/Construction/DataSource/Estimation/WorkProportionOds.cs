using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class WorkProportionOds : List<WorkProportionProjectViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WorkProportionProjectViewModel> GetProjectsAndWorkCategoryCostPercentage(string projectId)
        {
            var list = new List<WorkProportionProjectViewModel>();
            var projects = db.ProjectInformation.ToList();
           
            int id = Convert.ToInt32(projectId);
            //var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();
            if(projectId != null)
            {
                projects = projects.Where(p => p.ProjectInformationId == id).ToList();
            }
            foreach (ProjectInformation project in projects)
            {
                var totalCost = new decimal();
                var projectRecord = new WorkProportionProjectViewModel
                {
                    Name = project.Name
                };
                //var workItems = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == project.ProjectInformationId).ToList();
                var workItemsInProject = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == project.ProjectInformationId).ToList();
                var pieceRatesInProject = db.GenericAreaWorkItem.Where(pr => pr.ProjectInformationId == project.ProjectInformationId).ToList();
                var subContractWorkItemsInProject = db.GenericSubContractWorkItem.Where(sc => sc.ProjectInformationId == project.ProjectInformationId).ToList();

                var categoryIdsInWorkItems = new HashSet<int>(workItemsInProject.Select(c => c.WorkCategoryId));
                var categoryIdsInPieceRates = new HashSet<int>(pieceRatesInProject.Select(pr => pr.WorkCategoryId));
                var categoryIdsInSubContract = new HashSet<int>(subContractWorkItemsInProject.Select(sc => sc.WorkCategoryId));
                var categoriesInCurrentProject = 
                    db.WorkCategory
                    .Where(wc => categoryIdsInWorkItems.Contains(wc.WorkCategoryId) ||
                        categoryIdsInPieceRates.Contains(wc.WorkCategoryId) ||
                        categoryIdsInSubContract.Contains(wc.WorkCategoryId)
                    ).ToList();
                if (workItemsInProject.Any())
                {
                    totalCost = workItemsInProject.Sum(i => (i.TotalQuantityOfWork + i.AddedQuantityOfWork - i.OmittedQuantityOfWork) * i.TotalUnitCost);
                }
                if (pieceRatesInProject.Any())
                {
                    totalCost += pieceRatesInProject.Sum(pr => pr.OverHeadProfitCost);
                }
                if (subContractWorkItemsInProject.Any())
                {
                    totalCost += subContractWorkItemsInProject.Sum(sc => sc.Quntity * sc.TotalCost);
                }
                foreach(WorkCategory category in categoriesInCurrentProject)
                {
                    var workItemsUnderThisCategory = workItemsInProject.Where(w => w.WorkCategoryId == category.WorkCategoryId).ToList();
                    var pieceRatesUnderThisCategory = pieceRatesInProject.Where(w => w.WorkCategoryId == category.WorkCategoryId).ToList();
                    var subContractWorkItemsUnderThisCategory = subContractWorkItemsInProject.Where(w => w.WorkCategoryId == category.WorkCategoryId).ToList();

                    var workCategoryRecord = new WorkProportionWorkCategoryViewModel
                    {
                        Name = category.Name,
                        Code = category.Index,
                        //TotalAmount = itemsOfThisType.Sum(i => (i.TotalQuantityOfWork + i.AddedQuantityOfWork - i.OmittedQuantityOfWork) * i.TotalUnitCost)
                    };
                    if (workItemsUnderThisCategory.Any())
                    {
                        workCategoryRecord.TotalAmount = workItemsUnderThisCategory.Sum(i => (i.TotalQuantityOfWork + i.AddedQuantityOfWork - i.OmittedQuantityOfWork) * i.TotalUnitCost);
                    }
                    if (pieceRatesUnderThisCategory.Any())
                    {
                        workCategoryRecord.TotalAmount += pieceRatesUnderThisCategory.Sum(pr => pr.OverHeadProfitCost);
                    }
                    if (subContractWorkItemsUnderThisCategory.Any())
                    {
                        workCategoryRecord.TotalAmount += subContractWorkItemsUnderThisCategory.Sum(sc => sc.Quntity * sc.TotalCost);
                    }
                    workCategoryRecord.CostPercentage = totalCost > 0 ? workCategoryRecord.TotalAmount / totalCost : 0;
                    projectRecord.WorkCategoryList.Add(workCategoryRecord);
                }
                if (projectRecord.WorkCategoryList.Any())
                {
                    list.Add(projectRecord);
                }
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProjectHeaderVM GetProjectHeader(string projectId)
        {
            int id = Convert.ToInt32(projectId);
            var item = db.ProjectInformation.Find(id);
            ProjectHeaderVM headerDs = new ProjectHeaderVM();
            var organization = db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.ProjectName = item.Name;

            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return headerDs;
        }
    }
    //[DataObject]
    //public class ProjectBlocksOds : List<ProjectBlockVM>
    //{
    //    private ExceedDbContext db = new ExceedDbContext();
    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public List<ProjectBlockVM> GetAllBlocks(string projectId)
    //    {
    //        var list = new List<ProjectBlockVM>();
    //        int id = Convert.ToInt32(projectId);

    //        var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

    //        if (project != null)
    //        {
    //            var blocks = db.Blocks.Where(b => b.ProjectInformationId == project.ProjectInformationId).ToList();
    //            if (blocks.Any())
    //            {
    //                //var yearsRawList = new HashSet<string>(yearlyEarnedValueAnalysisList.Select(yv => yv.Year));
    //                foreach (Block block in blocks)
    //                {
    //                    var record = new ProjectBlockVM
    //                    {
    //                        Id  = block.BlockId,
    //                        Name = block.Name
    //                    };
    //                    list.Add(record);
    //                }
    //            }
    //        }
    //        return list;
    //    }
    //}
}
