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
    public class PriceAdjustmentOds : List<PriceAdjustmentWeightViewMoedl>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PriceAdjustmentWeightViewMoedl> GetRepresentativeElementsWeight(string projectId)
        {

            List<PriceAdjustmentWeightViewMoedl> list = new List<PriceAdjustmentWeightViewMoedl>();

            if (projectId == null)
            {
                return list;
            }
            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var priceAdWeights = db.PriceAdjustmentWeights.Where(p => p.ProjectInformationId == id).ToList();
                if (priceAdWeights.Any())
                {
                    foreach (PriceAdjustmentWeight weight in priceAdWeights)
                    {
                        //var record = new PriceAdjustmentWeightViewMoedl
                        //{
                        //    CategoryName = weight.WorkCategory.Name,
                        //    ContractAmount = weight.ContractAmount,
                        //    ActualAmount = weight.ActualAmount,
                        //};
                        var weightItems = db.PriceAdjustmentWeightItems.Where(itm => itm.PriceAdjustmentWeightId == weight.PriceAdjustmentWeightId).ToList();
                        foreach (var item in weightItems)
                        {
                            var itemRecord = new PriceAdjustmentWeightViewMoedl
                            {
                                CategoryName = weight.WorkCategory.Name,
                                ContractAmount = weight.ContractAmount,
                                ActualAmount = weight.ActualAmount,

                                //MaterialName = item.InventoryItem.Name,
                                Amount = item.Amount
                            };
                            list.Add(itemRecord);
                        }

                        //list.Add(record);
                    }
                }
            }
            return list;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PriceAdjustmentProjectViewModel> GetProjectFormulaComponents(string projectId)
        {

            List<PriceAdjustmentProjectViewModel> list = new List<PriceAdjustmentProjectViewModel>();

            if (projectId == null)
            {
                return list;
            }
            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                //var concatnatedWeightValue = "";
                //var concatnatedFormula = "";
                //decimal noneAdjustable = 1;

                //var priceAdWeights = db.PriceAdjustmentWeights.Where(p => p.ProjectInformationId == id).ToList();
                //var ids = new HashSet<int>(priceAdWeights.Select(w => w.PriceAdjustmentWeightId));
                //var projectWeightComponents = db.PriceAdjustmentWeightItems.Where(item => (ids.Contains(item.PriceAdjustmentWeightId))).ToList();
                //var representatives = db.ProjectRepresentativeElements.Where(rep => rep.ProjectInformationId == project.ProjectInformationId).ToList();
                //foreach (var rep in representatives)
                //{
                //    var availableCost = projectWeightComponents.Where(i => i.ItemCode == rep.ItemCode).ToList();
                //    decimal itemWeight = priceAdWeights.Sum(w => w.ActualAmount) > 0 ? Math.Round((availableCost.Sum(c => c.Amount)) / (priceAdWeights.Sum(w => w.ActualAmount)), 4) : 0;
                //    noneAdjustable = noneAdjustable - (priceAdWeights.Sum(w => w.ActualAmount) > 0 ? (availableCost.Sum(c => c.Amount)) / (priceAdWeights.Sum(w => w.ActualAmount)) : 0);
                //    concatnatedWeightValue = concatnatedWeightValue + " " + rep.InventoryItem.Name + " = " + itemWeight;

                //    concatnatedFormula = concatnatedFormula +"+"+ itemWeight + "(MLI-BLI)/BLI";
                //}
                //concatnatedWeightValue = concatnatedWeightValue + "\nNone Adjustable: " + Math.Round(noneAdjustable, 4);
                //concatnatedFormula = "PA = " + Math.Round(noneAdjustable, 4) +  concatnatedFormula;
                //if (priceAdWeights.Any())
                //{
                //    var record = new PriceAdjustmentProjectViewModel
                //    {
                //        Name = project.Name,
                //        TotalContractAmount = priceAdWeights.Sum(w => w.ContractAmount),
                //        TotalActualAmount = priceAdWeights.Sum(w => w.ActualAmount),
                //        ConcatnatedWeightValue = concatnatedWeightValue,
                //        Formula = concatnatedFormula
                //    };
                //    foreach (PriceAdjustmentWeight weight in priceAdWeights)
                //    {
                //        var weightRecord = new PriceAdjFormulaWeightViewMoedl
                //        {
                //            CategoryName = weight.WorkCategory.Name,
                //            ContractAmount = weight.ContractAmount,
                //            ActualAmount = weight.ActualAmount,
                //        };
                //        var weightItems = db.PriceAdjustmentWeightItems.Where(itm => itm.PriceAdjustmentWeightId == weight.PriceAdjustmentWeightId).ToList();
                //        foreach (var item in weightItems)
                //        {
                //            var itemRecord = new PriceAdjustmentWeightItemViewModel
                //            {

                //                ItemName = item.InventoryItem.Name,
                //                Amount = item.Amount
                //            };
                //            weightRecord.CostItems.Add(itemRecord);
                //        }
                //        record.CategoryWeightList.Add(weightRecord);
                //    }
                //    list.Add(record);
                //}
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
}
