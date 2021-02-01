using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Construction.DataSource
{
    //class EstimationWorkItemsObjectDataSource
    //{
    //}

    [DataObject]
    public class EstimationWorkItemsOds : List<EstimationWorkItemViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EstimationWorkItemViewModel> GetWorkItems(string projectTypeId, string strTypeId, string workCategoryId)
        {
            List<EstimationWorkItemViewModel> list = new List<EstimationWorkItemViewModel> { };

            var workItems = _db.ProjectEstimationWorkItem.Where(w => !w.IsSubCategory).ToList();

            int typeId = Convert.ToInt32(projectTypeId);
            int strId = Convert.ToInt32(strTypeId);
            int catId = Convert.ToInt32(workCategoryId);
            if (projectTypeId != null)
            {
                workItems = workItems.Where(wi => wi.ProjectType == (AvailableProjectTypes)typeId).ToList();
            }
            if (strTypeId != null)
            {
                workItems = workItems.Where(w => w.WorkCategory.ConfigurationId == strId).ToList();
            }
            if (workCategoryId != null)
            {
                workItems = workItems.Where(w => w.WorkCategoryId == catId).ToList();
            }
            foreach (ProjectEstimationWorkItem item in workItems)
            {
                var record = new EstimationWorkItemViewModel
                {
                    Category = this.GetCategoryName(item.WorkCategoryId),
                    ItemName = item.ItemName,
                    TotalQuantityOfWork = item.TotalQuantityOfWork,
                    TotalUnitCost = item.TotalUnitCost,
                    HourlyOutput = item.LabourHourlyOutPut,
                    UnitOfMeasurement = this.GetUnitOfMearurnment(item.UnitOfMeasurementId),

                    ProjectType = item.ProjectType.ToString(),

                    ProfitRate = ResourceUtilizationHelper.GetCurrentProfitRate(),
                    OverHeadRate = ResourceUtilizationHelper.GetCurrentOverheadRate(),
                    MaterialUnitCost = item.MaterialCostPerUnit,
                    LaborUnitCost = item.LaborCostPerUnit,
                    EquipmentUnitCost = item.EquipmentCostPerUnit,
                    DirectCost = item.DirectCost,
                    ProfitCost = item.ProfitCost,
                    OverheadCost = item.OverHeadCost,

                };
                var materials = _db.ProjectEstimationMaterial.Where(m => m.ProjectEstimationWorkItemId == item.ProjectEstimationWorkItemId).ToList();
                var labors = _db.ProjectEstimationDirectLabour.Where(l => l.ProjectEstimationWorkItemId == item.ProjectEstimationWorkItemId).ToList();
                var equipments = _db.ProjectEstimationEquipment.Where(e => e.ProjectEstimationWorkItemId == item.ProjectEstimationWorkItemId).ToList();
                if (item.ContainsWorkItemAsInput)
                {
                    var customItems = _db.EstimationTemplateCustomWorkItemDetails.Where(w => w.ParentId == item.ProjectEstimationWorkItemId).ToList();
                    if (customItems.Any())
                    {
                        foreach(EstimationTemplateCustomWorkItemDetail detailItem in customItems)
                        {
                            var customItemRecord = new EstimationMaterialViewModel
                            {
                                Name = detailItem.RootWorkItem.ItemName,
                                Quantity = detailItem.QuantityPerUnit,
                                Unit = ConsSettingHelper.GetUnitOfMearurnment(detailItem.RootWorkItem.UnitOfMeasurementId ?? 0),
                                UnitPrice = detailItem.UnitCost,
                                TotalCost = detailItem.TotalCost
                            };
                            record.MaterialList.Add(customItemRecord);
                        }
                    }
                }
                if (materials.Any())
                {
                    foreach (ProjectEstimationMaterial m in materials)
                    {
                        var workItemMaterial = new EstimationMaterialViewModel
                        {
                            Name = InventoryHelper.GetItemName(m.ItemCode),
                            Unit = InventoryHelper.GetItemUnitOfMeasurnment(m.ItemCode),
                            Quantity = Math.Round(m.Quantity, 2),
                            UnitPrice = m.UnitPrice,
                            TotalCost = m.TotalCost
                        };
                        record.MaterialList.Add(workItemMaterial);
                    }
                }
                if (labors.Any())
                {
                    foreach (ProjectEstimationDirectLabour l in labors)
                    {
                        var workItemLabor = new EstimationLabourViewModel
                        {
                            Title = ConstructionInputsHelper.GetJobTitleName(l.ProjectEstimationIndexLabour.ProjectJobTitleId),
                            HourlySalary = l.IndexedHourlySalary,
                            Quantity = Math.Round(l.Quantity, 0),
                            UnitFactor = l.UnitFactory,
                            TotalHourlyCost = l.TotalCost
                        };
                        record.LaborList.Add(workItemLabor);
                    }
                }
                if (equipments.Any())
                {
                    foreach (ProjectEstimationEquipment e in equipments)
                    {
                        var workItemEquipment = new EstimationEquipmentViewModel
                        {
                            Name = InventoryHelper.GetEquipmentName(e.FleetsID),
                            Quantity = Math.Round(e.Quantity, 0),
                            UnitFactor = e.UnitFactory,
                            HourlyRentalCost = e.RentalCost,
                            HourlyCost = e.TotalCost
                        };
                        record.EquipmentList.Add(workItemEquipment);
                    }
                }
                record.MaterialUnitCost = Math.Round(record.MaterialUnitCost, 2);
                record.LaborUnitCost = Math.Round(record.LaborUnitCost, 2);
                record.EquipmentUnitCost = Math.Round(record.EquipmentUnitCost, 2);
                record.DirectCost = Math.Round(record.DirectCost, 4);
                record.ProfitCost = Math.Round(record.ProfitCost, 2);
                record.OverheadCost = Math.Round(record.OverheadCost, 2);
                //record.TotalUnitCost = ;
                list.Add(record);
            }
            return list;
        }

        private decimal CalculateTotalUnitRate(EstimationWorkItemViewModel model)
        {
            var currentDirectCost = model.DirectCost;
            var overHeadRate = ResourceUtilizationHelper.GetCurrentOverheadRate();
            var profitRate = ResourceUtilizationHelper.GetCurrentProfitRate();
            var profitCost = currentDirectCost * profitRate / 100;
            var overheadCost = currentDirectCost * overHeadRate / 100;
            return currentDirectCost + profitCost + overheadCost;
        }

        private string GetCategoryName(int id)
        {
            var category = new WorkCategory();
            var temp = _db.WorkCategory.Find(id);
            if (temp != null)
            {
                category = temp;
            }
            return category.Name;
        }
        private string GetUnitOfMearurnment(int? id)
        {
            var unit = new Core.Domain.Construction.UnitOfMeasurement();
            var temp = _db.UnitOfMeasurement.Find(id);
            if (temp != null)
            {
                unit = temp;
            }
            return unit.Abbreviation;
        }
        private string GetProjectTypeName(int id)
        {
            //ProjectType projType = new ProjectType();
            //var temp = _db.ProjectTypes.Find(id);
            //if (temp != null)
            //{
            //    projType = temp;
            //}
            return "Temporarily Empty";
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProjectTypeHeaderVM GetProjectTypeHeader(string typeId)
        {
            int id = Convert.ToInt32(typeId);
            var item = (AvailableProjectTypes)id;
            ProjectTypeHeaderVM headerDs = new ProjectTypeHeaderVM();
            var organization = _db.Companies.FirstOrDefault();

            if (typeId != null)
            {
                headerDs.TypeName = item.ToString();
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

    [DataObject]
    public class EstimationCustomWorkItemOds : List<EstimationCustomWorkItemViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EstimationCustomWorkItemViewModel> GetAvailableCustomWorkItems(string projectTypeId)
        {
            List<EstimationCustomWorkItemViewModel> list = new List<EstimationCustomWorkItemViewModel>();
            var workItems = _db.ProjectEstimationCustomWorkItems.ToList();

            int typeId = Convert.ToInt32(projectTypeId);

            if (projectTypeId != null)
            {
                workItems = workItems.Where(wi => wi.ProjectType == (AvailableProjectTypes)typeId).ToList();
            }
            foreach (ProjectEstimationCustomWorkItem workItem in workItems)
            {
                var record = new EstimationCustomWorkItemViewModel
                {
                    WorkItemName = workItem.WorkItemName,
                    TotalUnitRate = workItem.TotalUnitRate
                };
                var contets = _db.EstimationCustomWorkItemContents
                    .Where(wi => wi.ProjectEstimationCustomWorkItemId == workItem.ProjectEstimationCustomWorkItemId)
                    .ToList();
                if (contets.Any())
                {
                    foreach (EstimationCustomWorkItemContent itemContent in contets)
                    {
                        var contentRecord = new EstimationCustomWorkItemContentViewModel
                        {
                            WorkItem = itemContent.RootWorkItem.ItemName,
                            QuantityPerUnit = itemContent.QuantityPerUnit,
                            UnitCost = itemContent.UnitCost,
                            UnitPrice = itemContent.UnitPrice,

                        };
                        record.WorkItems.Add(contentRecord);
                    }
                }
                list.Add(record);
            }
            return list;
        }
    }
}
