using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace ExceedERP.Helpers.Construction
{
    public static class ProjectProgressHelper
    {
        #region Detailed Work Progress Report
        public static decimal GetPlannedQuantityForThisMonth(int periodId,int subPeriodId, int workItemId)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal qty = new decimal();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);

            if (currentWorkItem != null)
            {
                var durationItem = db.WorkItemDuration.FirstOrDefault(w => w.Source == DurationItemSource.Estimation && w.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId);
                var currentSubPeriod = db.ConstructionSubPeriod.Find(subPeriodId);

                //Task task = new Task();
                if (durationItem != null && currentSubPeriod == null)
                {
                    var task = db.Tasks.FirstOrDefault(t => t.WorkItemDurationId == durationItem.WorkItemDurationId && !(t.IsSubActivityCreatedInSchedule));
                    if (task != null)
                    {
                        var taskDetail = db.TaskDetail.FirstOrDefault(td => td.ID == task.ID && td.ConstructionPeriodId == periodId);
                        if (taskDetail != null)
                        {
                            qty = taskDetail.PlannedQuantityOfWork;
                        }
                    }
                }
                else if (durationItem != null && currentSubPeriod != null)
                {
                    //var earlyPeriods = db.ConstructionSubPeriod.Where(p => p.DateTo < currentSubPeriod.DateFrom).ToList();
                    //var ids = new HashSet<int>(earlyPeriods.Select(p => p.ConstructionSubPeriodId));
                    var task = db.Tasks.FirstOrDefault(t => t.WorkItemDurationId == durationItem.WorkItemDurationId && !(t.IsSubActivityCreatedInSchedule));
                    if (task != null)
                    {
                        var detailBreakDown = db.TaskDetailBreakDown.FirstOrDefault(b => b.TaskDetail.ID == task.ID && b.ConstructionSubPeriodId == currentSubPeriod.ConstructionSubPeriodId);
                        if (detailBreakDown != null)
                        {
                            qty = detailBreakDown.PlannedQuantityOfWork;
                        }
                    }
                }
            }
            return qty;
        }
        public static decimal GetPlannedAmountForThisMonth(int periodId, int workItemId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            decimal amnt = new decimal();
            if (currentWorkItem != null)
            {
                //var task = db.Tasks.FirstOrDefault(t => t.WorkItemDuration.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId);
                //if (task != null)
                //{
                //    var taskDetail = db.TaskDetail.FirstOrDefault(td => td.ID == task.ID && td.ConstructionPeriodId == periodId);
                //    if (taskDetail != null)
                //    {
                //        amnt = taskDetail.TotalCost;
                //    }
                //}
            }
            return amnt;
        }
        public static decimal GetPlannedAmountOfCostTillPrevMonth(int periodId, int workItemId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            decimal amnt = new decimal();
            if (currentWorkItem != null)
            {
                //var earlyPeriods = db.ConstructionPeriod.Where(p => p.DateTo < currentPeriod.DateFrom).ToList();
                //var ids = new HashSet<int>(earlyPeriods.Select(p => p.ConstructionPeriodId));
                //var task = db.Tasks.FirstOrDefault(t => t.WorkItemDuration.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId);
                //if (task != null)
                //{
                //    var taskDetails = db.TaskDetail.Where(td => td.ID == task.ID && (ids.Contains(td.ConstructionPeriodId))).ToList();
                //    if (taskDetails.Any())
                //    {
                //        amnt = taskDetails.Sum(d => d.TotalCost);
                //    }
                //}
            }
            return amnt;
        }
        public static decimal GetPlannedQuantityOfWorkTillPrevMonth(int periodId, int subPeriodId, int workItemId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            var currentSubPeriod = db.ConstructionSubPeriod.Find(subPeriodId);
            decimal qty = new decimal();
            if (currentWorkItem != null && currentPeriod != null && currentSubPeriod == null)
            {
                var earlyPeriods = db.ConstructionPeriod.Where(p => p.DateTo < currentPeriod.DateFrom).ToList();
                var ids = new HashSet<int>(earlyPeriods.Select(p => p.ConstructionPeriodId));
                var task = db.Tasks.FirstOrDefault(t => t.WorkItemDuration.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId && !(t.IsSubActivityCreatedInSchedule));
                if (task != null)
                {
                    var taskDetails = db.TaskDetail.Where(td => td.ID == task.ID && (ids.Contains(td.ConstructionPeriodId))).ToList();
                    if (taskDetails.Any())
                    {
                        qty = taskDetails.Sum(d => d.PlannedQuantityOfWork);
                    }
                }
            }
            else if (currentWorkItem != null && currentPeriod != null && currentSubPeriod != null)
            {
                var earlyPeriods = db.ConstructionSubPeriod.Where(p => p.DateTo < currentSubPeriod.DateFrom).ToList();
                var ids = new HashSet<int>(earlyPeriods.Select(p => p.ConstructionSubPeriodId));
                var task = db.Tasks.FirstOrDefault(t => t.WorkItemDuration.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId && !(t.IsSubActivityCreatedInSchedule));
                if (task != null)
                {
                    var detailBreakDown = db.TaskDetailBreakDown.Where(b => b.TaskDetail.ID == task.ID && (ids.Contains(b.ConstructionSubPeriodId))).ToList();
                    if (detailBreakDown.Any())
                    {
                        qty = detailBreakDown.Sum(d => d.PlannedQuantityOfWork);
                    }
                    //var taskDetails = db.TaskDetail.Where(td => td.ID == task.ID && (ids.Contains(td.ConstructionPeriodId))).ToList();
                    //if (taskDetails.Any())
                    //{
                    //    qty = taskDetails.Sum(d => d.PlannedQuantityOfWork);
                    //}
                }
            }
            return qty;
        }
        public static decimal GetExecudtedQuantityOfWorkThisMonth(int periodId, int subPeriodId, int workItemId)
        {
            decimal qty = new decimal();
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            var currentSubPeriod = db.ConstructionSubPeriod.Find(subPeriodId);
            //decimal amnt = new decimal();
            if (currentWorkItem != null && currentPeriod != null && currentSubPeriod == null)
            {
                var takeOffSheet = db.ProjectTakeOff.FirstOrDefault(t => t.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId);
                if (takeOffSheet != null)
                {
                    var takeOffContents = db.ProjectTakeOffLine.Where(
                        tc => tc.ProjectTakeOffId == takeOffSheet.ProjectTakeOffId &&
                        tc.DateOfInformation > currentPeriod.DateFrom &&
                        tc.DateOfInformation < currentPeriod.DateTo
                        ).ToList();

                    if (takeOffContents.Any())
                    {
                        qty = takeOffContents.Sum(tc => tc.Quantity);
                    }
                }
                #region existing code
                //var resourceUtilizationPeriods = db.ResourceUtilizationPeriods.Where(rup => rup.ConstructionPeriodId == currentPeriod.ConstructionPeriodId && rup.ProjectInformationId == currentWorkItem.ProjectInformationId).ToList();
                //var ids = new HashSet<int>(resourceUtilizationPeriods.Select(r => r.ResourceUtilizationPeriodId));
                //var days = db.ResourceUtilizationDays.Where(d => (ids.Contains(d.ResourceUtilizationPeriodId))).ToList();
                //var dayIds = new HashSet<int>(days.Select(e => e.ResourceUtilizationDayId));

                //var utilizationWorkItems = db.ResourceUtilizationWorkItem.Where(w => w.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId && (dayIds.Contains(w.ResourceUtilizationDayId))).ToList();

                //if (utilizationWorkItems.Any())
                //{
                //    qty = utilizationWorkItems.Sum(i => i.ExecutedWork);
                //} 
                #endregion
            }
            else if(currentSubPeriod != null && currentPeriod != null && currentSubPeriod != null)
            {
                var takeOffSheet = db.ProjectTakeOff.FirstOrDefault(t => t.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId);
                if (takeOffSheet != null)
                {
                    var takeOffContents = db.ProjectTakeOffLine.Where(
                        tc => tc.ProjectTakeOffId == takeOffSheet.ProjectTakeOffId &&
                        tc.DateOfInformation > currentSubPeriod.DateFrom &&
                        tc.DateOfInformation < currentSubPeriod.DateTo
                        ).ToList();

                    if (takeOffContents.Any())
                    {
                        qty = takeOffContents.Sum(tc => tc.Quantity);
                    }
                }
            }
            return qty;
        }
        public static decimal GetExecudtedQuantityOfWorkTillPrevMonth(int periodId, int subPeriodId, int workItemId)
        {
            decimal qty = new decimal();
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            var currentSubPeriod = db.ConstructionSubPeriod.Find(subPeriodId);
            //decimal amnt = new decimal();
            if (currentWorkItem != null && currentPeriod != null && currentSubPeriod == null)
            {
                var takeOffSheet = db.ProjectTakeOff.FirstOrDefault(t => t.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId);
                if (takeOffSheet != null)
                {
                    var takeOffContents = db.ProjectTakeOffLine.Where(
                        tc => tc.ProjectTakeOffId == takeOffSheet.ProjectTakeOffId &&
                        tc.DateOfInformation < currentPeriod.DateFrom
                        ).ToList();

                    if (takeOffContents.Any())
                    {
                        qty = takeOffContents.Sum(tc => tc.Quantity);
                    }
                }
                #region existing code
                //var earlyPeriods = db.ConstructionPeriod.Where(p => p.DateTo < currentPeriod.DateFrom).ToList();
                //var pIds = new HashSet<int>(earlyPeriods.Select(p => p.ConstructionPeriodId));
                //var resourceUtilizationPeriods = db.ResourceUtilizationPeriods.Where(rup => (pIds.Contains(rup.ConstructionPeriodId)) && rup.ProjectInformationId == currentWorkItem.ProjectInformationId).ToList();
                //var ids = new HashSet<int>(resourceUtilizationPeriods.Select(r => r.ResourceUtilizationPeriodId));
                //var days = db.ResourceUtilizationDays.Where(d => (ids.Contains(d.ResourceUtilizationPeriodId))).ToList();
                //var dayIds = new HashSet<int>(days.Select(e => e.ResourceUtilizationDayId));

                //var utilizationWorkItems = db.ResourceUtilizationWorkItem.Where(w => w.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId && (dayIds.Contains(w.ResourceUtilizationDayId)));

                //if (utilizationWorkItems.Any())
                //{
                //    qty = utilizationWorkItems.Sum(w => w.ExecutedWork);
                //} 
                #endregion
            }
            else if(currentWorkItem != null && currentPeriod != null && currentSubPeriod != null)
            {
                var takeOffSheet = db.ProjectTakeOff.FirstOrDefault(t => t.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId);
                if (takeOffSheet != null)
                {
                    var takeOffContents = db.ProjectTakeOffLine.Where(
                        tc => tc.ProjectTakeOffId == takeOffSheet.ProjectTakeOffId &&
                        tc.DateOfInformation < currentSubPeriod.DateFrom
                        ).ToList();

                    if (takeOffContents.Any())
                    {
                        qty = takeOffContents.Sum(tc => tc.Quantity);
                    }
                }
            }
            return qty;

        }


        // 
        //public static decimal GetExcutedAmountThisMonth(int periodId, int workItemId)
        //{
        //    ExceedDbContext db = new ExceedDbContext();
        //    var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
        //    var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
        //    decimal amnt = new decimal();
        //    if (currentWorkItem != null && currentPeriod != null)
        //    {
        //        var resourceUtilizationPeriods = db.ResourceUtilizationPeriods.Where(rup => rup.ConstructionPeriodId == currentPeriod.ConstructionPeriodId && rup.ProjectInformationId == currentWorkItem.ProjectInformationId).ToList();
        //        var ids = new HashSet<int>(resourceUtilizationPeriods.Select(r => r.ResourceUtilizationPeriodId));
        //        var days = db.ResourceUtilizationDays.Where(d => (ids.Contains(d.ResourceUtilizationPeriodId))).ToList();
        //        var dayIds = new HashSet<int>(days.Select(e => e.ResourceUtilizationDayId));

        //        var utilizationWorkItems = db.ResourceUtilizationWorkItem.Where(w => w.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId && (dayIds.Contains(w.ResourceUtilizationDayId)));

        //        if (utilizationWorkItems.Any())
        //        {
        //            amnt = utilizationWorkItems.Sum(itm => itm.TotalCost);
        //        }
        //    }
        //    return amnt;
        //}
        public static decimal GetExcutedAmountTillPrevMonth(int periodId, int workItemId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            decimal amnt = new decimal();
            if (currentWorkItem != null && currentPeriod != null)
            {
                var earlyPeriods = db.ConstructionPeriod.Where(p => p.DateTo < currentPeriod.DateFrom).ToList();
                var pIds = new HashSet<int>(earlyPeriods.Select(p => p.ConstructionPeriodId));
                var resourceUtilizationPeriods = db.ResourceUtilizationPeriods.Where(rup => (pIds.Contains(rup.ConstructionPeriodId)) && rup.ProjectInformationId == currentWorkItem.ProjectInformationId).ToList();
                var ids = new HashSet<int>(resourceUtilizationPeriods.Select(r => r.ResourceUtilizationPeriodId));
                var days = db.ResourceUtilizationDays.Where(d => (ids.Contains(d.ResourceUtilizationPeriodId))).ToList();
                var dayIds = new HashSet<int>(days.Select(e => e.ResourceUtilizationDayId));

                var utilizationWorkItems = db.ResourceUtilizationWorkItem.Where(w => w.WorkItemPerProjectId == currentWorkItem.WorkItemPerProjectId && (dayIds.Contains(w.ResourceUtilizationDayId)));

                if (utilizationWorkItems.Any())
                {
                    amnt = utilizationWorkItems.Sum(w => w.TotalCost);
                }
            }
            return amnt;

        }
        #endregion

        #region Planned and actual utilized per category
        public static decimal GetActualAccomplishedAmountThisMonth(int blockId, int categoryId, int periodId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkCategory = db.WorkCategory.Find(categoryId);
            var currentBlock = db.Blocks.Find(blockId);
            decimal amnt = new decimal();
            if (currentBlock != null && currentWorkCategory != null && currentPeriod != null)
            {
                var workItemsOfThisCategory = db.WorkItemPerProject.Where(w => w.WorkCategoryId == categoryId && w.BlockId == blockId).ToList();
                var itemIds = new HashSet<int>(workItemsOfThisCategory.Select(w => w.WorkItemPerProjectId));

                var resourceUtilizationPeriods = db.ResourceUtilizationPeriods.Where(rup => rup.ConstructionPeriodId == currentPeriod.ConstructionPeriodId && rup.ProjectInformationId == currentBlock.ProjectInformationId).ToList();
                var ids = new HashSet<int>(resourceUtilizationPeriods.Select(r => r.ResourceUtilizationPeriodId));
                var days = db.ResourceUtilizationDays.Where(d => (ids.Contains(d.ResourceUtilizationPeriodId))).ToList();
                var dayIds = new HashSet<int>(days.Select(e => e.ResourceUtilizationDayId));

                var utilizationWorkItems = db.ResourceUtilizationWorkItem.Where(w => (itemIds.Contains(w.WorkItemPerProjectId)) && (dayIds.Contains(w.ResourceUtilizationDayId)));

                if (utilizationWorkItems.Any())
                {
                    amnt = utilizationWorkItems.Sum(w => w.TotalCost);
                }
            }
            return amnt;
        }

        public static decimal GetPlannedAmount(int blockId, int categoryId, int periodId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkCategory = db.WorkCategory.Find(categoryId);
            var currentBlock = db.Blocks.Find(blockId);
            decimal amnt = new decimal();
            if (currentBlock != null && currentWorkCategory != null && currentPeriod != null)
            {
                var workItemsOfThisCategory = db.WorkItemPerProject.Where(w => w.WorkCategoryId == categoryId && w.BlockId == blockId).ToList();
                var itemIds = new HashSet<int>(workItemsOfThisCategory.Select(w => w.WorkItemPerProjectId));

                var latestVersion = GetLatestScheduleVersion(currentBlock.ProjectInformationId);
                var tasks = db.Tasks.Where(t => (itemIds.Contains(t.WorkItemDuration.WorkItemPerProjectId)) &&
                !(t.IsSubActivityCreatedInSchedule) &&
                t.Version == latestVersion)
                .ToList();
                var tIds = new HashSet<int>(tasks.Select(t => t.ID));
                var allTaskBreakDownDetails = db.TaskDetail.Where(td => (tIds.Contains(td.ID))).ToList();

                if (allTaskBreakDownDetails.Any())
                {
                    amnt = allTaskBreakDownDetails.Sum(w => w.TotalCost);
                }
            }
            return amnt;
        }
        // 
        //  summary:
        //      planned cost for a given work category 
        public static decimal GetPlannedAmountTillPrevMonth(int blockId, int categoryId, int periodId)
        {
            var db = new ExceedDbContext();
            decimal amnt = new decimal();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkCategory = db.WorkCategory.Find(categoryId);
            var currentBlock = db.Blocks.Find(blockId);
            if (currentBlock != null && currentWorkCategory != null && currentPeriod != null)
            {
                var earlyPeriods = db.ConstructionPeriod.Where(p => p.DateTo < currentPeriod.DateFrom).ToList();
                var ids = new HashSet<int>(earlyPeriods.Select(p => p.ConstructionPeriodId));


                var workItemsOfThisCategory = db.WorkItemPerProject.Where(w => w.WorkCategoryId == categoryId && w.BlockId == blockId).ToList();
                var itemIds = new HashSet<int>(workItemsOfThisCategory.Select(w => w.WorkItemPerProjectId));

                var latestVersion = GetLatestScheduleVersion(currentBlock.ProjectInformationId);
                var tasks = db.Tasks.Where(t => (itemIds.Contains(t.WorkItemDuration.WorkItemPerProjectId)) &&
                !(t.IsSubActivityCreatedInSchedule) &&
                t.Version == latestVersion).ToList();
                var tIds = new HashSet<int>(tasks.Select(t => t.ID));
                var allTaskBreakDownDetails = db.TaskDetail.Where(td => (tIds.Contains(td.ID)) && (ids.Contains(td.ConstructionPeriodId))).ToList();
                if (allTaskBreakDownDetails.Any())
                {
                    amnt = allTaskBreakDownDetails.Sum(td => td.TotalCost);
                }
            }
            return amnt;

        }
        public static decimal GetActualAccomplishedAmountTillPrevMonth(int blockId, int categoryId, int periodId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            var currentWorkCategory = db.WorkCategory.Find(categoryId);
            var currentBlock = db.Blocks.Find(blockId);
            decimal amnt = new decimal();
            if (currentBlock != null && currentWorkCategory != null && currentPeriod != null)
            {
                var workItemsOfThisCategory = db.WorkItemPerProject.Where(w => w.WorkCategoryId == categoryId && w.BlockId == blockId).ToList();
                var itemIds = new HashSet<int>(workItemsOfThisCategory.Select(w => w.WorkItemPerProjectId));

                var earlyPeriods = db.ResourceUtilizationPeriods.Where(rup => rup.Period.DateTo < currentPeriod.DateFrom && rup.ProjectInformationId == currentBlock.ProjectInformationId).ToList();
                var ids = new HashSet<int>(earlyPeriods.Select(r => r.ResourceUtilizationPeriodId));
                var days = db.ResourceUtilizationDays.Where(d => (ids.Contains(d.ResourceUtilizationPeriodId))).ToList();
                var dayIds = new HashSet<int>(days.Select(e => e.ResourceUtilizationDayId));

                var utilizationWorkItems = db.ResourceUtilizationWorkItem.Where(w => (itemIds.Contains(w.WorkItemPerProjectId)) && (dayIds.Contains(w.ResourceUtilizationDayId)));

                if (utilizationWorkItems.Any())
                {
                    amnt = utilizationWorkItems.Sum(w => w.TotalCost);
                }
            }
            return amnt;
        }

        #endregion

        #region planned and actual utilization per block
        public static decimal GetPlannedAmountOfBlockThisMonth(int blockId, int periodId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            //var currentWorkCategory = db.WorkCategory.Find(categoryId);
            var currentBlock = db.Blocks.Find(blockId);
            decimal amnt = new decimal();
            if (currentBlock != null && currentPeriod != null)
            {
                var workItemsUnderBlock = db.WorkItemPerProject.Where(w => w.BlockId == blockId).ToList();
                var itemIds = new HashSet<int>(workItemsUnderBlock.Select(w => w.WorkItemPerProjectId));

                var latestVersion = GetLatestScheduleVersion(currentBlock.ProjectInformationId);
                var tasks = db.Tasks.Where(t => (itemIds.Contains(t.WorkItemDuration.WorkItemPerProjectId)) &&
                !(t.IsSubActivityCreatedInSchedule) &&
                t.Version == latestVersion).ToList();
                var tIds = new HashSet<int>(tasks.Select(t => t.ID));
                var allTaskBreakDownDetails = db.TaskDetail.Where(td => (tIds.Contains(td.ID))).ToList();

                if (allTaskBreakDownDetails.Any())
                {
                    amnt = allTaskBreakDownDetails.Sum(w => w.TotalCost);
                }
            }
            return amnt;
        }
        public static decimal GetActualAccomplishedAmountForBlockThisMonth(int blockId, int periodId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            //var currentWorkCategory = db.WorkCategory.Find(categoryId);
            var currentBlock = db.Blocks.Find(blockId);
            decimal amnt = new decimal();
            if (currentBlock != null && currentPeriod != null)
            {
                var workItemsUnderBlock = db.WorkItemPerProject.Where(w => w.BlockId == blockId).ToList();
                var itemIds = new HashSet<int>(workItemsUnderBlock.Select(w => w.WorkItemPerProjectId));

                var resourceUtilizationPeriods = db.ResourceUtilizationPeriods.Where(rup => rup.ConstructionPeriodId == currentPeriod.ConstructionPeriodId && rup.ProjectInformationId == currentBlock.ProjectInformationId).ToList();
                var ids = new HashSet<int>(resourceUtilizationPeriods.Select(r => r.ResourceUtilizationPeriodId));
                var days = db.ResourceUtilizationDays.Where(d => (ids.Contains(d.ResourceUtilizationPeriodId))).ToList();
                var dayIds = new HashSet<int>(days.Select(e => e.ResourceUtilizationDayId));

                var utilizationWorkItems = db.ResourceUtilizationWorkItem.Where(w => (itemIds.Contains(w.WorkItemPerProjectId)) && (dayIds.Contains(w.ResourceUtilizationDayId)));

                if (utilizationWorkItems.Any())
                {
                    amnt = utilizationWorkItems.Sum(w => w.TotalCost);
                }
            }
            return amnt;
        }
        public static decimal GetPlannedAmountForBlockTillPrevMonth(int blockId, int periodId)
        {
            var db = new ExceedDbContext();
            decimal amnt = new decimal();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            //var currentWorkCategory = db.WorkCategory.Find(categoryId);
            var currentBlock = db.Blocks.Find(blockId);
            if (currentBlock != null && currentPeriod != null)
            {
                var earlyPeriods = db.ConstructionPeriod.Where(p => p.DateTo < currentPeriod.DateFrom).ToList();
                var ids = new HashSet<int>(earlyPeriods.Select(p => p.ConstructionPeriodId));


                var workItemsUnderBlock = db.WorkItemPerProject.Where(w => w.BlockId == blockId).ToList();
                var itemIds = new HashSet<int>(workItemsUnderBlock.Select(w => w.WorkItemPerProjectId));

                var latestVersion = GetLatestScheduleVersion(currentBlock.ProjectInformationId);
                var tasks = db.Tasks.Where(t => (itemIds.Contains(t.WorkItemDuration.WorkItemPerProjectId)) &&
                !(t.IsSubActivityCreatedInSchedule) &&
                t.Version == latestVersion).ToList();
                var tIds = new HashSet<int>(tasks.Select(t => t.ID));
                var allTaskBreakDownDetails = db.TaskDetail.Where(td => (tIds.Contains(td.ID)) && (ids.Contains(td.ConstructionPeriodId))).ToList();
                if (allTaskBreakDownDetails.Any())
                {
                    amnt = allTaskBreakDownDetails.Sum(td => td.TotalCost);
                }
            }
            return amnt;

        }
        public static decimal GetActualAccomplishedAmountForBlockTillPrevMonth(int blockId, int periodId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var currentPeriod = db.ConstructionPeriod.FirstOrDefault(p => p.ConstructionPeriodId == periodId);
            //var currentWorkCategory = db.WorkCategory.Find(categoryId);
            var currentBlock = db.Blocks.Find(blockId);
            decimal amnt = new decimal();
            if (currentBlock != null && currentPeriod != null)
            {
                var workItemsUnderBlock = db.WorkItemPerProject.Where(w => w.BlockId == blockId).ToList();
                var itemIds = new HashSet<int>(workItemsUnderBlock.Select(w => w.WorkItemPerProjectId));

                var earlyPeriods = db.ResourceUtilizationPeriods.Where(rup => rup.Period.DateTo < currentPeriod.DateFrom && rup.ProjectInformationId == currentBlock.ProjectInformationId).ToList();
                var ids = new HashSet<int>(earlyPeriods.Select(r => r.ResourceUtilizationPeriodId));
                var days = db.ResourceUtilizationDays.Where(d => (ids.Contains(d.ResourceUtilizationPeriodId))).ToList();
                var dayIds = new HashSet<int>(days.Select(e => e.ResourceUtilizationDayId));

                var utilizationWorkItems = db.ResourceUtilizationWorkItem.Where(w => (itemIds.Contains(w.WorkItemPerProjectId)) && (dayIds.Contains(w.ResourceUtilizationDayId)));

                if (utilizationWorkItems.Any())
                {
                    amnt = utilizationWorkItems.Sum(w => w.TotalCost);
                }
            }
            return amnt;
        }
        #endregion

        private static int GetLatestScheduleVersion(int projectId)
        {
            var db = new ExceedDbContext();
            var allVersions = db.ScheduleRevisions.Where(v => v.ProjectInformationId == projectId).ToList();
            var version = 0;
            if (allVersions.Any())
            {
                var temp = allVersions.FirstOrDefault();
                if (temp != null)
                {
                    version = temp.Version;
                    foreach (ScheduleRevision rev in allVersions)
                    {
                        if (rev.Version > version)
                        {
                            version = rev.Version;
                        }
                    }
                }
            }
            return version;
        }
    }

}

