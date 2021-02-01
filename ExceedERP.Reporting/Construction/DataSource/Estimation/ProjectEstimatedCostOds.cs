using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Helpers.Inventory;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class ProjectEstimatedCostOds : List<ProjectEstimatedCostProjectVM>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectEstimatedCostProjectVM> GetProjectEstimatedCost(string projectId) {
            List<ProjectEstimatedCostProjectVM> list = new List<ProjectEstimatedCostProjectVM>();

            var id = Convert.ToInt32(projectId);
            
            var projects = _db.ProjectInformation.ToList();
            if (projectId != null)
            {
                projects = projects.Where(p => p.ProjectInformationId == id).ToList();
            }
            if (projects.Any())
            {
                foreach(ProjectInformation project in projects)
                {
                    var record = new ProjectEstimatedCostProjectVM
                    {
                        Name = project.Name,
                        Id = project.ProjectInformationId
                    };
                    var workItems = _db.WorkItemPerProject.Where(w => w.ProjectInformationId == project.ProjectInformationId).ToList();
                    var itemIds = new HashSet<int>(workItems.Select(w => w.WorkItemPerProjectId));
                    var customInputItems = _db.ProjectCustomWorkItemDetails.Where(w => itemIds.Contains(w.WorkItemPerProjectId)).ToList();
                    var rootItemIds = new HashSet<int>(customInputItems.Select(i => i.ProjectEstimationWorkItemId));
                    var rootItems = _db.ProjectEstimationWorkItem.Where(w => rootItemIds.Contains(w.ProjectEstimationWorkItemId)).ToList();
                    if (rootItems.Any())
                    {
                        foreach(var item in rootItems)
                        {
                            var customItemsOfThisType = customInputItems.Where(i => i.ProjectEstimationWorkItemId == item.ProjectEstimationWorkItemId).ToList();
                            if (customItemsOfThisType.Any())
                            {
                                var customItemRecord = new ProjectEstimatedCostMaterialVM
                                {
                                    Name = item.ItemName,
                                    Unit = ConsSettingHelper.GetUnitOfMearurnment(item.UnitOfMeasurementId ?? 0),
                                    TotalCost = customItemsOfThisType.Sum(m => m.TotalCost ),
                                    Quantity = customItemsOfThisType.Sum(m => m.QuantityPerUnit),
                                    UnitPrice = customItemsOfThisType.Average(m => m.UnitCost)
                                };
                                record.MaterialList.Add(customItemRecord);

                            }
                        }
                    }
                    

                    var materials = _db.ProjectMaterial.Where(m => m.WorkItemPerProject.ProjectInformationId == project.ProjectInformationId).ToList();
                    var matIds = new HashSet<string>(materials.Select(m => m.ItemCode));
                    var items = _db.InventoryItems.Where(itm => matIds.Contains(itm.ItemCode)).ToList();
                    if (items.Any())
                    {
                        foreach(InventoryItem item in items)
                        {
                            var materialInuseofThisType = materials.Where(m => m.ItemCode == item.ItemCode).ToList();
                            if (materialInuseofThisType.Any())
                            {
                                var materialRecord = new ProjectEstimatedCostMaterialVM
                                {
                                    Name = item.Name,
                                    Unit = InventoryHelper.GetItemUnitOfMeasurnment(item.ItemCode),
                                    TotalCost = materialInuseofThisType.Sum(m => m.TotalCost * m.WorkItemPerProject.TotalQuantityOfWork),
                                    Quantity = materialInuseofThisType.Sum(m => m.Quantity * m.WorkItemPerProject.TotalQuantityOfWork),
                                    UnitPrice = materialInuseofThisType.Average(m => m.UnitPrice)
                                };
                                materialRecord.UnitPrice = materialRecord.Quantity > 0 ? materialRecord.TotalCost / materialRecord.Quantity : materialRecord.UnitPrice;
                                record.MaterialList.Add(materialRecord);
                            }
                        }
                    }
                    var equipments = _db.ProjectEquipment.Where(e => e.WorkItemPerProject.ProjectInformationId == project.ProjectInformationId).ToList();
                    var eqpIds = new HashSet<Guid>(equipments.Select(e => e.FleetsID));
                    var eqpts = _db.Fleets.Where(f => eqpIds.Contains(f.FleetsID)).ToList();
                    if (eqpts.Any())
                    {
                        foreach(Fleets equipment in eqpts)
                        {
                            var eqInUseOThisType = equipments.Where(e => e.FleetsID == equipment.FleetsID).ToList();
                            if (eqInUseOThisType.Any())
                            {
                                var equipmentRecord = new ProjectEstimatedCostEquipmentVM
                                {
                                    Name = equipment.EquipmentName,
                                    TotalCost = eqInUseOThisType.Sum(e => e.TotalCost * e.WorkItemPerProject.TotalQuantityOfWork),
                                    Quantity = eqInUseOThisType.Sum(e => e.Quantity),
                                    UnitPrice = eqInUseOThisType.Average(e => e.RentalCost * e.WorkItemPerProject.TotalQuantityOfWork),
                                };
                                equipmentRecord.UnitPrice = equipmentRecord.Quantity > 0 ? equipmentRecord.TotalCost / equipmentRecord.Quantity : equipmentRecord.UnitPrice;
                                record.EquipmentList.Add(equipmentRecord);
                            }
                        }
                    }
                    var allLabors = _db.ProjectDirectLabour.Where(l => l.WorkItemPerProject.ProjectInformationId == project.ProjectInformationId).ToList();
                    var ids = new HashSet<int>(allLabors.Select(l => l.ProjectEstimationIndexLabourId));
                    var labors = _db.ProjectEstimationIndexLabour.Where(l => ids.Contains(l.ProjectEstimationIndexLabourId)).ToList();
                    foreach(ProjectEstimationIndexLabour labor in labors)
                    {
                        var laborInUseOfThisType = allLabors.Where(l => l.ProjectEstimationIndexLabourId == labor.ProjectEstimationIndexLabourId).ToList();
                        if (laborInUseOfThisType.Any())
                        {
                            var laborRecord = new ProjectEstimatedCostLaborVM
                            {
                                Name = ConstructionInputsHelper.GetJobTitleName(labor.ProjectJobTitleId),
                                TotalCost = laborInUseOfThisType.Sum(l => l.TotalCost * l.WorkItemPerProject.TotalQuantityOfWork),
                                Quantity = laborInUseOfThisType.Sum(l => l.Quantity * l.WorkItemPerProject.TotalQuantityOfWork),
                                IndexedHourlySalary = laborInUseOfThisType.Average(l => l.IndexedHourlySalary)
                            };
                            laborRecord.IndexedHourlySalary = laborRecord.Quantity > 0 ? laborRecord.TotalCost / laborRecord.Quantity : laborRecord.IndexedHourlySalary;
                            record.LaborList.Add(laborRecord); 
                        }
                    }
                    record.MaterialCost = record.MaterialList.Sum(m => m.TotalCost);
                    record.EquipmentCost = record.EquipmentList.Sum(e => e.TotalCost);
                    record.LaborCost = record.LaborList.Sum(l => l.TotalCost);
                    if(record.LaborList.Any() || record.MaterialList.Any() || record.EquipmentList.Any())
                    {
                        list.Add(record);
                    }
                }
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

        private void CalculateResultsForCustomInputItems()
        {

        }
    }

}

