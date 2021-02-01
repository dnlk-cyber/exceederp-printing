using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
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
    public class MaterialExpenseOds : List<MaterialExpenseViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MaterialExpenseViewModel> GetProjectMaterialExpense(string projectId, string periodId, string itemCategoryCode)
        {
            List<MaterialExpenseViewModel> list = new List<MaterialExpenseViewModel>();

            if (projectId == null || periodId == null)
            {
                return list;
            }
            int projId = Convert.ToInt32(projectId);
            var currentBranch = new Branch();
            var currentProject = db.ProjectInformation.FirstOrDefault(p => p.ProjectInformationId == projId);
            if (currentProject != null)
            {
                var temp = db.Branch.FirstOrDefault(b => b.Code == currentProject.Code);
                if (temp != null)
                {
                    currentBranch = temp;
                }
            }
            //var branch = db.Branch.Where(p => p.BranchId == id).FirstOrDefault();

            if (currentProject != null)
            {
                int targetPeriodId = Convert.ToInt32(periodId);
                var materialExpense = db.ProjectMaterialExpenses.Where(me => me.BranchId == currentBranch.BranchId && me.GLPeriodId == targetPeriodId).ToList();
                if (itemCategoryCode != null)
                {
                    materialExpense = materialExpense.Where(mm => mm.ItemCategoryCode == itemCategoryCode).ToList();
                }
                if (materialExpense.Any())
                {
                    foreach (ProjectMaterialExpense expense in materialExpense)
                    {
                        var record = new MaterialExpenseViewModel
                        {
                            MaterialType = expense.InventoryItem.Name,
                            Unit = expense.Unit,
                            UnitPrice = expense.UnitPrice,
                            DeliveredQuantityPrevMonth = expense.DeliveredQuantityPrevMonth,
                            ConsumedQuantityPrevMonth = expense.ConsumedQuantityPrevMonth,
                            MaterilOnSitePrevMonth = expense.MaterilOnSitePrevMonth,
                            DeliveredQuantity = expense.DeliveredQuantity,
                            ConsumedQuantity = expense.ConsumedQuantity,
                            TranIn = expense.TranIn,
                            TranOut = expense.TranOut,
                            DeliveredQuantityToDate = expense.DeliveredQuantityToDate,
                            ConsumedQuantityToDate = expense.ConsumedQuantityToDate,
                            ShipedQuantityToDate = expense.ShipedQuantityToDate,
                            MaterialOnSiteToDate = expense.MaterialOnSiteToDate,
                            StockBalance = expense.StockBalance,
                            CostOfConsumedMaterialPrevMonth = expense.CostOfConsumedMaterialPrevMonth,
                            CostOfConsumedMaterialThisMonth = expense.CostOfConsumedMaterialThisMonth,
                            CostOfConsumedMaterialToDate = expense.CostOfConsumedMaterialToDate,
                            StockAmount = expense.StockAmount,
                            MaterialOnSiteAmount = expense.MaterialOnSiteAmount
                        };
                        list.Add(record);
                    }
                }
            }

            return list;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PeriodViewModel> GetAvailablePeriods(string projectId)
        {

            List<PeriodViewModel> periodsList = new List<PeriodViewModel>();
            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var currentBranch = db.Branch.FirstOrDefault(b => b.Code == project.Code);
                if (currentBranch != null)
                {
                    var materialExpenses = db.ProjectMaterialExpenses.Where(m => m.BranchId == currentBranch.BranchId).ToList();
                    if (materialExpenses.Any())
                    {
                        var periodIds = new HashSet<int>(materialExpenses.Select(s => s.GLPeriodId));
                        var availablePeriods = db.InventoryPeriods.Where(p => periodIds.Contains(p.InventoryPeriodId)).ToList();
                        foreach (InventoryPeriod period in availablePeriods)
                        {
                            var record = new PeriodViewModel
                            {
                                Name = period.Name,
                                PeriodId = period.InventoryPeriodId
                            };
                            periodsList.Add(record);
                        }
                    }
                }

            }
            return periodsList;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ItemCategoryViewModel> GetAllItemCategories(string projectId)
        {
            List<ItemCategoryViewModel> list = new List<ViewModel.ItemCategoryViewModel>();
            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var currentBranch = db.Branch.FirstOrDefault(b => b.Code == project.Code);
                if (currentBranch != null)
                {
                    var materialExpenses = db.ProjectMaterialExpenses.Where(m => m.BranchId == currentBranch.BranchId).ToList();
                    if (materialExpenses.Any())
                    {
                        var categoryCodes = new HashSet<string>(materialExpenses.Select(s => s.ItemCategoryCode));
                        var availableCategories = db.ItemCategorys.Where(cat => categoryCodes.Contains(cat.ItemCategoryCode)).ToList();
                        foreach (ItemCategory category in availableCategories)
                        {
                            var categoryRecord = new ItemCategoryViewModel
                            {
                                ItemCategoryCode =category.ItemCategoryCode,
                                Name = category.Name
                            };
                            list.Add(categoryRecord);
                        }
                    }
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

    [DataObject]
    public class DirectMaterialExpenseOds : List<MaterialExpenseViewModel> {

        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MaterialExpenseViewModel> GetDirectProjectMaterialExpense(string projectId, string periodId, string itemCategoryCode)
        {
            List<MaterialExpenseViewModel> list = new List<MaterialExpenseViewModel>();

            if (projectId == null || periodId == null)
            {
                return list;
            }
            int projId = Convert.ToInt32(projectId);
            var currentBranch = new Branch();
            var currentProject = db.ProjectInformation.FirstOrDefault(p => p.ProjectInformationId == projId);
            if (currentProject != null)
            {
                var temp = db.Branch.FirstOrDefault(b => b.Code == currentProject.Code);
                if (temp != null)
                {
                    currentBranch = temp;
                }
            }

            if (currentProject != null)
            {
                int targetPeriodId = Convert.ToInt32(periodId);
                var materialExpense = db.ProjectDirectMaterialExpenses.Where(me => me.BranchId == currentBranch.BranchId && me.GLPeriodId == targetPeriodId).ToList();
                if (itemCategoryCode != null)
                {
                    materialExpense = materialExpense.Where(mm => mm.ItemCategoryCode == itemCategoryCode).ToList();
                }
                if (materialExpense.Any())
                {
                    foreach (ProjectDirectMaterialExpense expense in materialExpense)
                    {
                        var record = new MaterialExpenseViewModel
                        {
                            MaterialType = expense.InventoryItem.Name,
                            Unit = expense.Unit,
                            UnitPrice = expense.UnitPrice,
                            DeliveredQuantityPrevMonth = expense.DeliveredQuantityPrevMonth,
                            ConsumedQuantityPrevMonth = expense.ConsumedQuantityPrevMonth,
                            MaterilOnSitePrevMonth = expense.MaterilOnSitePrevMonth,
                            DeliveredQuantity = expense.DeliveredQuantity,
                            ConsumedQuantity = expense.ConsumedQuantity,
                            TranIn = expense.TranIn,
                            TranOut = expense.TranOut,
                            DeliveredQuantityToDate = expense.DeliveredQuantityToDate,
                            ConsumedQuantityToDate = expense.ConsumedQuantityToDate,
                            ShipedQuantityToDate = expense.ShipedQuantityToDate,
                            MaterialOnSiteToDate = expense.MaterialOnSiteToDate,
                            StockBalance = expense.StockBalance,
                            CostOfConsumedMaterialPrevMonth = expense.CostOfConsumedMaterialPrevMonth,
                            CostOfConsumedMaterialThisMonth = expense.CostOfConsumedMaterialThisMonth,
                            CostOfConsumedMaterialToDate = expense.CostOfConsumedMaterialToDate,
                            StockAmount = expense.StockAmount,
                            MaterialOnSiteAmount = expense.MaterialOnSiteAmount
                        };
                        list.Add(record);
                    }
                }
            }

            return list;
        }
    }
    [DataObject]
    public class IndirectMaterialExpenseOds : List<MaterialExpenseViewModel> {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MaterialExpenseViewModel> GetIndirectProjectMaterialExpense(string projectId, string periodId, string itemCategoryCode)
        {
            List<MaterialExpenseViewModel> list = new List<MaterialExpenseViewModel>();

            if (projectId == null || periodId == null)
            {
                return list;
            }
            int projId = Convert.ToInt32(projectId);
            var currentBranch = new Branch();
            var currentProject = db.ProjectInformation.FirstOrDefault(p => p.ProjectInformationId == projId);
            if (currentProject != null)
            {
                var temp = db.Branch.FirstOrDefault(b => b.Code == currentProject.Code);
                if (temp != null)
                {
                    currentBranch = temp;
                }
            }

            if (currentProject != null)
            {
                int targetPeriodId = Convert.ToInt32(periodId);
                var materialExpense = db.ProjectDirectMaterialExpenses.Where(me => me.BranchId == currentBranch.BranchId && me.GLPeriodId == targetPeriodId).ToList();
                if (itemCategoryCode != null)
                {
                    materialExpense = materialExpense.Where(mm => mm.ItemCategoryCode == itemCategoryCode).ToList();
                }
                if (materialExpense.Any())
                {
                    foreach (ProjectDirectMaterialExpense expense in materialExpense)
                    {
                        var record = new MaterialExpenseViewModel
                        {
                            MaterialType = expense.InventoryItem.Name,
                            Unit = expense.Unit,
                            UnitPrice = expense.UnitPrice,
                            DeliveredQuantityPrevMonth = expense.DeliveredQuantityPrevMonth,
                            ConsumedQuantityPrevMonth = expense.ConsumedQuantityPrevMonth,
                            MaterilOnSitePrevMonth = expense.MaterilOnSitePrevMonth,
                            DeliveredQuantity = expense.DeliveredQuantity,
                            ConsumedQuantity = expense.ConsumedQuantity,
                            TranIn = expense.TranIn,
                            TranOut = expense.TranOut,
                            DeliveredQuantityToDate = expense.DeliveredQuantityToDate,
                            ConsumedQuantityToDate = expense.ConsumedQuantityToDate,
                            ShipedQuantityToDate = expense.ShipedQuantityToDate,
                            MaterialOnSiteToDate = expense.MaterialOnSiteToDate,
                            StockBalance = expense.StockBalance,
                            CostOfConsumedMaterialPrevMonth = expense.CostOfConsumedMaterialPrevMonth,
                            CostOfConsumedMaterialThisMonth = expense.CostOfConsumedMaterialThisMonth,
                            CostOfConsumedMaterialToDate = expense.CostOfConsumedMaterialToDate,
                            StockAmount = expense.StockAmount,
                            MaterialOnSiteAmount = expense.MaterialOnSiteAmount
                        };
                        list.Add(record);
                    }
                }
            }

            return list;
        }
    }
}


