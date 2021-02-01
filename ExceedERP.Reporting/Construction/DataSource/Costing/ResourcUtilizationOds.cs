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
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
   
    [DataObject]
    public class ResourcUtilizationOds : List<ResourceUtilizationDayViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ResourceUtilizationDayViewModel> GetUtilizationWorkItems(string projectId, string periodId, string dayId)
        {
            List<ResourceUtilizationDayViewModel> list = new List<ResourceUtilizationDayViewModel> { };

            //var workItems = _db.ResourceUtilizationWorkItem.ToList();
            int id = Convert.ToInt32(projectId);
            int pId = Convert.ToInt32(periodId);
            int dId = Convert.ToInt32(dayId);
            var currentProject = _db.ProjectInformation.Find(id);
            var currentPeriod = _db.ResourceUtilizationPeriods.Find(pId);
            if(projectId == null || periodId == null)
            {
                return list;
            }
            if(currentProject != null && currentPeriod != null)
            {
                var days = _db.ResourceUtilizationDays.Where(d => d.ResourceUtilizationPeriodId == currentPeriod.ResourceUtilizationPeriodId).ToList();
                if (days.Any())
                {
                    foreach(ResourceUtilizationDay day in days)
                    {
                        var dayRecord = new ResourceUtilizationDayViewModel
                        {
                            Id = day.ResourceUtilizationDayId,
                            Name = day.Name
                        };

                        var utilizationWorkItems = _db.ResourceUtilizationWorkItem.Where(
                            w => w.ResourceUtilizationDayId == day.ResourceUtilizationDayId && 
                            w.TotalDirectCost > 0).ToList();
                        if(dayId != null)
                        {
                            utilizationWorkItems = utilizationWorkItems.Where(w => w.ResourceUtilizationDayId == dId).ToList();
                        }
                        if (utilizationWorkItems.Any())
                        {
                            foreach(ResourceUtilizationWorkItem item in utilizationWorkItems)
                            {
                                var record = new ResourceUtilizationWorkItemViewModel
                                {
                                    Category = item.WorkCategory,
                                    WorkItem = item.WorkItemPerProject.ItemName,
                                    MethodOfWork = item.MethodOfWork.Abbreviation,
                                    UnitOfMeasurnment = item.UnitOfMeasurnment,
                                    ExecutedWork = item.ExecutedWork,

                                    ProjectName = ResourceUtilizationHelper.GetConcatnatedProjectandBlockName(item.WorkItemPerProjectId),

                                    WorkDurationInHoursPerDay = item.WorkDurationInHoursPerDay,
                                    OutPutPerDay = item.OutPutPerDay,
                                    DirectContractUnitPrice = item.DirectContractUnitPrice,
                                    Income = item.Income,
                                    LastModified = this.GetStringfiedDateValue(item)

                                };
                                var materials = _db.ResourceUtilizationMaterials.Where(m => m.ResourceUtilizationWorkItemId == item.ResourceUtilizationWorkItemId).ToList();
                                var labors = _db.ResourceUtilizationDirectLabours.Where(l => l.ResourceUtilizationWorkItemId == item.ResourceUtilizationWorkItemId).ToList();
                                var equipments = _db.ResourceUtilizationEquipment.Where(e => e.ResourceUtilizationWorkItemId == item.ResourceUtilizationWorkItemId).ToList();
                                if (materials.Any())
                                {
                                    foreach (ResourceUtilizationMaterial m in materials)
                                    {
                                        var workItemMaterial = new ResourceUtilizationaterialViewModel
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
                                    foreach (ResourceUtilizationDirectLabour l in labors)
                                    {
                                        var workItemLabor = new ResourceUtilizationLabourViewModel
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
                                    foreach (ResourceUtilizationEquipment e in equipments)
                                    {
                                        var workItemEquipment = new ResourceUtilizationEquipmentViewModel
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
                                record.MaterialCost = Math.Round((record.MaterialList.Sum(m => m.TotalCost)), 2);
                                record.LaborCost = Math.Round((record.LaborList.Sum(l => l.TotalHourlyCost)), 2);
                                record.EquipmentCost = Math.Round((record.EquipmentList.Sum(e => e.HourlyCost)), 2);
                                record.TotalDirectCost = Math.Round(record.MaterialCost + record.LaborCost + record.EquipmentCost, 4);
                                record.DirectIncomeExpenseRatio = record.TotalDirectCost > 0 ? Math.Round((record.Income / record.TotalDirectCost), 2) : Math.Round(record.Income / 1, 2);
                                record.PercentageOfEquipmentCostWrtTotalCost = record.TotalDirectCost > 0 ? Math.Round((record.EquipmentCost / record.TotalDirectCost), 2) : Math.Round((record.EquipmentCost / 1), 2);
                                record.PercentageOfMaterialCostWrtTotalCost = record.TotalDirectCost > 0 ? Math.Round((record.MaterialCost / record.TotalDirectCost), 2) : Math.Round((record.MaterialCost / 1), 2);
                                record.PercentageOfLaborCostWrtTotalCost = record.TotalDirectCost > 0 ? Math.Round((record.LaborCost / record.TotalDirectCost), 2) : Math.Round((record.LaborCost / 1), 2);
                                
                                dayRecord.WorkItemList.Add(record);
                            }
                        }
                        if (dayRecord.WorkItemList.Any())
                        {
                            list.Add(dayRecord);
                        }
                    }
                }
               //var workItems = _db.ResourceUtilizationWorkItem.Where(wi => wi.WorkItemPerProject.ProjectInformationId == id).ToList();

                //if(periodId != null)
                //{
                //    workItems = workItems.Where(w => w.ResourceUtilizationPeriodId == targetPeriod).ToList();
                //}
            }
            //foreach (ResourceUtilizationWorkItem item in workItems)
            //{
                
            //}
            return list;
        }
        private string GetStringfiedDateValue(ResourceUtilizationWorkItem model) {
            var dateCreated = model.DateCreated;
            var lastModified = model.LastModified;
            var stringValue = "";
            if(lastModified != null && dateCreated != null && lastModified > dateCreated)
            {
                stringValue = ((DateTime)lastModified).ToString("MMMM dd, yyyy");
            }
            else if (lastModified == null && dateCreated != null)
            {
                stringValue = ((DateTime)dateCreated).ToString("MMMM dd, yyyy");
            }
            stringValue = DateTime.Now.ToString("MMMM dd, yyyy"); 
            {

            }
            return stringValue;
        }

        //private string GetCategoryName(int id)
        //{
        //    var category = new WorkCategory();
        //    var temp = _db.WorkCategory.Find(id);
        //    if (temp != null)
        //    {
        //        category = temp;
        //    }
        //    return category.Name;
        //}
        //private string GetUnitOfMearurnment(int id)
        //{
        //    var unit = new UnitOfMeasurement();
        //    var temp = _db.UnitOfMeasurement.Find(id);
        //    if (temp != null)
        //    {
        //        unit = temp;
        //    }
        //    return unit.Abbreviation;
        //}
        //private string GetProjectTypeName(int id)
        //{
        //    ProjectType projType = new ProjectType();
        //    var temp = _db.ProjectTypes.Find(id);
        //    if (temp != null)
        //    {
        //        projType = temp;
        //    }
        //    return projType.Name;
        //}
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PeriodViewModel> GetAvailablePeriods(string projectId)
        {

            List<PeriodViewModel> periodsList = new List<PeriodViewModel>();
            int id = Convert.ToInt32(projectId);
            var project = _db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var utilizations = _db.ResourceUtilizationWorkItem.Where(m => m.WorkItemPerProject.ProjectInformationId == id).ToList();
                if (utilizations.Any())
                {
                    var dayIds = new HashSet<int>(utilizations.Select(s => s.ResourceUtilizationDayId));
                    var days = _db.ResourceUtilizationDays.Where(d => (dayIds.Contains(d.ResourceUtilizationDayId))).ToList();
                    var periodIds = new HashSet<int>(days.Select(d => d.ResourceUtilizationPeriodId));

                    var availablePeriods = _db.ResourceUtilizationPeriods.Where(p => periodIds.Contains(p.ResourceUtilizationPeriodId)).ToList();
                    foreach (ResourceUtilizationPeriod period in availablePeriods)
                    {
                        var record = new PeriodViewModel
                        {
                            Name = period.Period.Name,
                            PeriodId = period.ResourceUtilizationPeriodId
                        };
                        periodsList.Add(record);
                    }
                }
            }
            return periodsList;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ResourceUtilizationDayViewModel> GetDaysWithinPeriod(string periodId) {
            List<ResourceUtilizationDayViewModel> list = new List<ResourceUtilizationDayViewModel>();

            var id = Convert.ToInt32(periodId);
            var days = _db.ResourceUtilizationDays.Where(d => d.ResourceUtilizationPeriodId == id).ToList();
            if (days.Any())
            {
                foreach(ResourceUtilizationDay day in days)
                {
                    var record = new ResourceUtilizationDayViewModel
                    {
                        Id = day.ResourceUtilizationDayId,
                        Name = day.Name
                    };
                    list.Add(record);
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
    }
}
