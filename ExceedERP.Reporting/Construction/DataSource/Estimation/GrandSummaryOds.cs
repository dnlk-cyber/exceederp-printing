using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class GrandSummaryOds : List<GSProjectViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GSProjectViewModel> GetProjectBlocks(string projectId)
        {
            List<GSProjectViewModel> list = new List<GSProjectViewModel>();
            if(projectId == null)
            {
                return list;
            }
            //List<GSSubProjectTypeViewModel> blocks = new List<GSSubProjectTypeViewModel>();
            int id = Convert.ToInt32(projectId);
            var currentProject = _db.ProjectInformation.Find(id);
            if(currentProject != null)
            {
                var record = new GSProjectViewModel
                {
                    ProjectName = currentProject.Name
                    //TotalCost = _db.WorkItemPerProject.Sum(projWI => projWI.ProjectInformationId == currentProject.ProjectInformationId));
                };
                var subProjectTypes = _db.SubProjectType.Where(b => b.ProjectInformationId == id).ToList();
                if (subProjectTypes.Any())
                {
                    foreach (SubProjectType subProjectType in subProjectTypes)
                    {
                        var subProjectCost = new decimal();
                        var subProjectTypeWorkItems = _db.GenericWorkItemPerProject.Where(w => w.SubProjectTypeId == subProjectType.SubProjectTypeId).ToList();
                        var subProjectTypePieceRates = _db.GenericAreaWorkItem.Where(w => w.SubProjectTypeId == subProjectType.SubProjectTypeId).ToList();
                        var subProjectTypeSubContractWorkItems = _db.GenericSubContractWorkItem.Where(w => w.SubProjectTypeId == subProjectType.SubProjectTypeId).ToList();
                        if (subProjectTypeWorkItems.Any())
                        {
                            subProjectCost = subProjectTypeWorkItems.Sum(wi => (wi.TotalUnitCost * wi.TotalQuantityOfWork));
                        }
                        if (subProjectTypePieceRates.Any())
                        {
                            subProjectCost += subProjectTypePieceRates.Sum(pr => pr.OverHeadProfitCost);
                        }
                        if (subProjectTypeSubContractWorkItems.Any())
                        {
                            subProjectTypeSubContractWorkItems.Sum(sc => (sc.Quntity * sc.TotalCost));
                        }
                        var blockToAdd = new GSBlockViewModel
                        {
                            Name = subProjectType.Name,
                            NumberOfBlocks = subProjectType.NumberOfBlocks,
                            PricePerBlock = subProjectCost
                        };
                        blockToAdd.PricePerTotalBlock = blockToAdd.NumberOfBlocks * blockToAdd.PricePerBlock;
                        record.Blocks.Add(blockToAdd);
                    }

                }
                var totalCost = record.Blocks.Sum(b => b.PricePerTotalBlock);
                var vat = (record.Blocks.Sum(b => b.PricePerTotalBlock)) * 15 / 100;
                var totalAmount = totalCost + vat;
                record.TotalCost = totalCost.ToString("N4", CultureInfo.InvariantCulture);
                record.VAT = vat.ToString("N4", CultureInfo.InvariantCulture);
                record.TotalWithVAT = totalAmount.ToString("N4", CultureInfo.InvariantCulture);
                list.Add(record);
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProjectHeaderVM GetProjectHeader(string projectId)
        {
            int id = Convert.ToInt32(projectId);
            var item = _db.ProjectInformation.Find(id);
            ProjectHeaderVM headerDs = new ProjectHeaderVM();
            var organization = _db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.ProjectName = item.Name;

            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
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
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return headerDs;
        }

    }
}
