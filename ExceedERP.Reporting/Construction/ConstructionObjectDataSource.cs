using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Construction
{
    public class ConstructionObjectDataSource
    {
    }
    [DataObject]
    public class BoqSummaryObjectDataSource : List<SummaryOfBOQ>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SummaryOfBOQ> GetBoqSummary(string projectId)
        {
            var list = new List<SummaryOfBOQ>();
            var boq = new SummaryOfBOQ();
            var wItemCatgegories = new List<WorkItemCategory>();
            var superwItemCatgegories = new List<SuperWorkItemCategory>();
            var wItem = new WorkItemCategory();
            var superwItem = new SuperWorkItemCategory();
            boq.Categories = new List<WorkItemCategory>();
            if (projectId == null) return list;
            int id = Convert.ToInt32(projectId);
            Structures structure = new Structures();
            var project = db.ProjectInformation.Where(x => x.ProjectInformationId == id).FirstOrDefault().Name;
            var workItems = db.WorkItemPerProject.ToList();

            var structureType = from c in db.Configurations
                                join w in db.ProjectTypeMappings

                                    on c.ConfigurationId equals w.ConfigurationId
                                //join p in db.ProjectInformation
                                //on w.ProjectTypeId equals p.ProjectTypeId

                                //where p.ProjectInformationId == id && c.Reference == "STRUCTURE_TYPE"
                                select (new
                                {
                                    c.CurrentValue,
                                    c.ConfigurationId
                                });
            if (structureType.Any())
            {
                boq = new SummaryOfBOQ();
                boq.Structures = new List<Structures>();
                foreach (var ss in structureType)
                {
                    db = new ExceedDbContext();

                    var Subsummaries =
                        db.ProjectEstimationBoqBreakdown.Where(
                            x => x.ProjectInformationId == id && x.ConfigurationId == ss.ConfigurationId).ToList();

                    structure = new Structures();
                    structure.SuperCategories = new List<SuperWorkItemCategory>();
                    superwItemCatgegories = new List<SuperWorkItemCategory>();
                    structure.StructureType = ss.CurrentValue;
                    if (Subsummaries.Any())
                    {
                        boq.ProjectName = project;
                        foreach (var s in Subsummaries)
                        {
                            superwItem = new SuperWorkItemCategory();
                            var catgeo =
                                workItems.Where(x => x.WorkItemPerProjectId == s.GenericWorkItemPerProjectId)
                                    .FirstOrDefault();
                            if (catgeo != null)
                            {
                                superwItem.CategoryName = GetWorkCategoName(catgeo.WorkCategoryId);
                                superwItem.Total = s.Quantity * s.UnitRate;
                                superwItem.Unit = "Birr";
                                superwItem.Quntity = s.Quantity;
                                superwItem.UnitRate = s.UnitRate;

                            }

                            superwItemCatgegories.Add(superwItem);
                            boq.Total += wItem.Total;

                        }
                        structure.SuperCategories.AddRange(superwItemCatgegories);
                    }
                    boq.Structures.Add(structure);
                    boq.SubTotal += Convert.ToDouble(structure.SuperCategories.Sum(x => x.Total));
                }
                boq.Vat = Convert.ToDecimal(boq.SubTotal * 0.15);
                boq.GrandTotal = boq.Vat + Convert.ToDecimal(boq.SubTotal);
                list.Add(boq);
            }
            return list.Distinct().ToList();
        }

        public string GetWorkCategoName(int categoId)
        {
            var category = db.WorkCategory.Where(x => x.WorkCategoryId == categoId).FirstOrDefault();
            if (category != null)
            {
                return category.Name;
            }
            return "";
        }
    }

    [DataObject]
    public class BlockGrandSummaryObjectDataSource : List<BlockGrandSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BlockGrandSummary> GetBlockGrandSummary(string projectId)
        {
            var list = new List<BlockGrandSummary>();
            var one = new BlockGrandSummary();



            return list.Distinct().ToList();

        }

        public string GetWorkCategoName(int categoId)
        {
            var category = db.WorkCategory.Where(x => x.WorkCategoryId == categoId).FirstOrDefault();
            if (category != null)
            {
                return category.Name;
            }
            return "";
        }
    }

    [DataObject]
    public class GrandSummaryObjectDataSource : List<GrandSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GrandSummary> GetGrandSummary(string projectId)
        {
            var list = new List<GrandSummary>();
            var boq = new GrandSummary();
            var wItemCatgegories = new List<GrandSubWorkItemCategory>();
            var superwItemCatgegories = new List<GrandSuperWorkItemCategory>();
            var wItem = new GrandSubWorkItemCategory();
            var superwItem = new GrandSuperWorkItemCategory();
            var item = new WorkItem();
            var itemList = new List<WorkItem>();
            var takeOffSheet = new TakeOffSheet();
            var takeoffs = db.TakeOffPerProject.ToList();
            var takeOffSheetLineList = db.TakeOffLinePerProject.ToList();
            var takeOffSheetKgList = db.TakeOffLineForKgPerProject.ToList();
            GrandStructureType grandStructureType = new GrandStructureType();
            grandStructureType.GrandSuperWorkItemCategory = new List<GrandSuperWorkItemCategory>();

            if (projectId == null) return list;
            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(x => x.ProjectInformationId == id).FirstOrDefault().Name;
            var workItems = db.WorkItemPerProject.Where(x => x.ProjectInformationId == id).ToList();

            var structureType = from c in db.Configurations
                                join w in db.ProjectTypeMappings

                                    on c.ConfigurationId equals w.ConfigurationId
                                //join p in db.ProjectInformation
                                //on w.ProjectTypeId equals p.ProjectTypeId

                                //where p.ProjectInformationId == id && c.Reference == "STRUCTURE_TYPE"
                                select (new
                                {
                                    c.CurrentValue,
                                    c.ConfigurationId
                                });

            if (structureType.Any())
            {
                boq = new GrandSummary();
                boq.GrandStructureType = new List<GrandStructureType>();
                foreach (var ss in structureType)
                {
                    db = new ExceedDbContext();
                    grandStructureType = new GrandStructureType();
                    grandStructureType.StructureType = ss.CurrentValue;
                    grandStructureType.GrandSuperWorkItemCategory = new List<GrandSuperWorkItemCategory>();
                    var Subsummaries =
                        db.ProjectEstimationBoqBreakdown.Where(
                            x => x.ProjectInformationId == id && x.ConfigurationId == ss.ConfigurationId).ToList();
                    if (Subsummaries.Any())
                    {


                        boq.ProjectName = project;
                        superwItemCatgegories = new List<GrandSuperWorkItemCategory>();
                        //boq.StructureType = StructureType.SUPER_STRUCTURE;
                        itemList = new List<WorkItem>();
                        foreach (var s in Subsummaries)
                        {
                            db = new ExceedDbContext();
                            superwItem = new GrandSuperWorkItemCategory();
                            superwItem.SuperWorkItems = new List<WorkItem>();
                            item = new WorkItem();
                            item.TakeOffSheet = new List<TakeOffSheet>();
                            var catgeo = workItems.Where(x => x.WorkItemPerProjectId == s.GenericWorkItemPerProjectId).FirstOrDefault();

                            if (catgeo != null && catgeo.TotalQuantityOfWork > 0)
                            {
                                superwItem.CategoryName = GetWorkCategoName(catgeo.WorkCategoryId);
                                superwItem.Total = s.Quantity * s.UnitRate;
                                superwItem.Unit = "Birr";
                                superwItem.Quntity = s.Quantity;
                                superwItem.UnitRate = s.UnitRate;
                                item.Name = GetWorkItem(catgeo.ProjectEstimationWorkItemId);
                                item.Unit = GetUnitOfM(catgeo.UnitOfMeasurementId ?? 0);
                                item.Quntity = catgeo.TotalQuantityOfWork;
                                item.UnitRate = catgeo.TotalUnitCost;
                                item.Total = item.Quntity * item.UnitRate;

                            }
                            db = new ExceedDbContext();
                            if (takeoffs.Any())
                            {
                                var takeoff = takeoffs.Where(x => x.WorkItemPerProjectId == s.ProjectInformationId).ToList();
                                if (takeoff.Any())
                                {
                                    foreach (var t in takeoff)
                                    {

                                        var takeofflins =
                                            takeOffSheetLineList.Where(x => x.TakeOffPerProjectId == t.TakeOffPerProjectId);
                                        if (takeofflins.Any())
                                        {
                                            foreach (var tt in takeofflins)
                                            {
                                                takeOffSheet.Name = tt.Description;
                                                takeOffSheet.Unit = GetUnitOfM(tt.UnitOfMeasurementId);
                                                takeOffSheet.Quntity = tt.Quantity;
                                                item.TakeOffSheet.Add(takeOffSheet);
                                            }
                                        }
                                    }
                                }
                            }
                            superwItem.SuperWorkItems.Add(item);
                            superwItem.Total += wItem.Total;
                            superwItemCatgegories.Add(superwItem);

                            boq.Total += wItem.Total;


                        }



                    }
                    grandStructureType.GrandSuperWorkItemCategory.AddRange(superwItemCatgegories);
                    boq.SubTotal += Convert.ToDouble(grandStructureType.GrandSuperWorkItemCategory.Sum(x => x.Total));
                    boq.GrandStructureType.Add(grandStructureType);
                }
            }




            boq.Vat = Convert.ToDecimal(boq.SubTotal * 0.15);
            boq.GrandTotal = boq.Vat + Convert.ToDecimal(boq.SubTotal);
            list.Add(boq);
            return list.Distinct().ToList(); ;

        }

        public string GetUnitOfM(int id)
        {
            var uom = db.UnitOfMeasurement.Where(x => x.UnitOfMeasurementId == id).FirstOrDefault();
            if (uom != null)
            {
                return uom.Abbreviation;
            }
            return "";

        }
        public string GetWorkItem(int id)
        {
            var estimation = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();
            if (estimation != null)
            {
                return estimation.ItemName;
            }
            return "";

        }
        public string GetWorkCategoName(int categoId)
        {
            var category = db.WorkCategory.Where(x => x.WorkCategoryId == categoId).FirstOrDefault();
            if (category != null)
            {
                return category.Name;
            }
            return "";
        }
    }



    [DataObject]

    public class ProjectInformationObjectDataSource : List<ProjectInformation>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectInformation> GetProjectInformation()
        {
            var projects = db.ProjectInformation.ToList();
            return projects;
        }
    }


    /// <summary>
    /// Modified Object data source for BoQ Summary with blocks
    /// </summary>

    //[DataObject]
    //public class BoQSummaryObjectDataSource : List<BoQSummaryViewModel>
    //{
    //    private ExceedDbContext db = new ExceedDbContext();

    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public List<BoQSummaryViewModel> GetBoQSummary(string projectId)
    //    {
    //        var list = new List<BoQSummaryViewModel>();
    //        var boq = new BoQSummaryViewModel();
    //        var structureTypes = new List<StructureTypeViewModel>();
    //        var structure = new StructureTypeViewModel();
    //        var workItemsOfSubStructure = new List<WorkItemViewModel>();
    //        var workItemOfSuperStructure = new List<WorkItemViewModel>();
    //        var workCategories = new List<WorkCategoryViewModel>();
    //        var works = new List<WorkItemViewModel>();
    //        var work = new WorkItemViewModel();

    //        var wItemCatgegories = new List<GrandSubWorkItemCategory>();
    //        var superwItemCatgegories = new List<GrandSuperWorkItemCategory>();
    //        var wItem = new GrandSubWorkItemCategory();
    //        var superwItem = new GrandSuperWorkItemCategory();
    //        var item = new WorkItem();
    //        var itemList = new List<WorkItem>();
    //        //var takeOffSheet = new TakeOffSheet();
    //        //var takeoffs = db.TakeOffPerProject.ToList();
    //        //var takeOffSheetLineList = db.TakeOffLinePerProject.ToList();
    //        //var takeOffSheetKgList = db.TakeOffLineForKgPerProject.ToList();
    //        GrandStructureType grandStructureType = new GrandStructureType();
    //        grandStructureType.GrandSuperWorkItemCategory = new List<GrandSuperWorkItemCategory>();

    //        if (projectId == null) {
    //            return list;
    //        }
    //        int id = Convert.ToInt32(projectId);
    //        var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault().Name;
    //        var workItems = db.WorkItemPerProject.Where(p => p.ProjectInformationId == id).ToList();

    //        var blocks = db.Blocks.Where(b => b.ProjectInformationId == id).ToList();

    //        var structureType = from c in db.Configurations
    //                            join w in db.ProjectTypeMappings

    //                                on c.ConfigurationId equals w.ConfigurationId
    //                            join p in db.ProjectInformation
    //                                on w.ProjectTypeId equals p.ProjectTypeId

    //                            where p.ProjectInformationId == id && c.Reference == "STRUCTURE_TYPE"
    //                            select (new
    //                            {
    //                                c.CurrentValue,
    //                                c.ConfigurationId
    //                            });

    //        if (structureType.Any())
    //        {
    //            boq = new BoQSummaryViewModel();
    //            boq.Blocks = new List<BlockViewModel>();
    //            foreach (var ss in structureType)
    //            {
    //                db = new ExceedDbContext();
    //                structure = new StructureTypeViewModel();
    //                structure.StructureType = ss.CurrentValue;
    //                structure.Categories = new List<WorkCategoryViewModel>();
    //                var Subsummaries =
    //                    db.ProjectEstimationBoqBreakdown.Where(
    //                        x => x.ProjectInformationId == id && x.ConfigurationId == ss.ConfigurationId).ToList();
    //                if (Subsummaries.Any())
    //                {

    //                    boq.ProjectName = project;
    //                    superwItemCatgegories = new List<GrandSuperWorkItemCategory>();
    //                    //boq.StructureType = StructureType.SUPER_STRUCTURE;
    //                    itemList = new List<WorkItem>();
    //                    foreach (var s in Subsummaries)
    //                    {
    //                        db = new ExceedDbContext();
    //                        superwItem = new GrandSuperWorkItemCategory();
    //                        superwItem.SuperWorkItems = new List<WorkItem>();
    //                        item = new WorkItem();
    //                        item.TakeOffSheet = new List<TakeOffSheet>();
    //                        var catgeo = workItems.Where(x => x.WorkItemPerProjectId == s.WorkItemPerProjectId).FirstOrDefault();

    //                        if (catgeo != null)
    //                        {
    //                            superwItem.CategoryName = GetWorkCategoName(catgeo.WorkCategoryId);
    //                            superwItem.Total = s.Quantity * s.UnitRate;
    //                            superwItem.Unit = "Birr";
    //                            superwItem.Quntity = s.Quantity;
    //                            superwItem.UnitRate = s.UnitRate;
    //                            item.Name = GetWorkItem(catgeo.ProjectEstimationWorkItemId);
    //                            item.Unit = GetUnitOfM(catgeo.UnitOfMeasurementId);
    //                            item.Quntity = catgeo.TotalQuantityOfWork;
    //                            item.UnitRate = catgeo.TotalUnitCost;
    //                            item.Total = item.Quntity * item.UnitRate;

    //                        }
    //                        db = new ExceedDbContext();
    //                        //if (takeoffs.Any())
    //                        //{
    //                        //    var takeoff = takeoffs.Where(x => x.WorkItemPerProjectId == s.ProjectInformationId).ToList();
    //                        //    if (takeoff.Any())
    //                        //    {
    //                        //        foreach (var t in takeoff)
    //                        //        {

    //                        //            var takeofflins =
    //                        //                takeOffSheetLineList.Where(x => x.TakeOffPerProjectId == t.TakeOffPerProjectId);
    //                        //            if (takeofflins.Any())
    //                        //            {
    //                        //                foreach (var tt in takeofflins)
    //                        //                {
    //                        //                    takeOffSheet.Name = tt.Description;
    //                        //                    takeOffSheet.Unit = GetUnitOfM(tt.UnitOfMeasurementId);
    //                        //                    takeOffSheet.Quntity = tt.Quantity;
    //                        //                    item.TakeOffSheet.Add(takeOffSheet);
    //                        //                }
    //                        //            }
    //                        //        }
    //                        //    }
    //                        //}
    //                        superwItem.SuperWorkItems.Add(item);
    //                        superwItem.Total += wItem.Total;
    //                        superwItemCatgegories.Add(superwItem);

    //                        boq.Total += wItem.Total;


    //                    }



    //                }
    //                grandStructureType.GrandSuperWorkItemCategory.AddRange(superwItemCatgegories);
    //                boq.SubTotal += Convert.ToDouble(grandStructureType.GrandSuperWorkItemCategory.Sum(x => x.Total));
    //                boq.GrandStructureType.Add(grandStructureType);
    //            }
    //        }




    //        boq.TaxAmount = Convert.ToDecimal(boq.SubTotal * 0.15);
    //        boq.GrandTotal = boq.TaxAmount + Convert.ToDecimal(boq.SubTotal);
    //        list.Add(boq);
    //        return list.Distinct().ToList(); ;

    //    }

    //    public string GetUnitOfM(int id)
    //    {
    //        var uom = db.UnitOfMeasurement.Where(x => x.UnitOfMeasurementId == id).FirstOrDefault();
    //        if (uom != null)
    //        {
    //            return uom.Abbreviation;
    //        }
    //        return "";

    //    }
    //    public string GetWorkItem(int id)
    //    {
    //        var estimation = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();
    //        if (estimation != null)
    //        {
    //            return estimation.ItemName;
    //        }
    //        return "";

    //    }
    //    public string GetWorkCategoName(int categoId)
    //    {
    //        var category = db.WorkCategory.Where(x => x.WorkCategoryId == categoId).FirstOrDefault();
    //        if (category != null)
    //        {
    //            return category.Name;
    //        }
    //        return "";
    //    }
    //}
}
