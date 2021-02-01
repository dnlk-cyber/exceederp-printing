using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EntityState = System.Data.Entity.EntityState;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class ProjectEstimationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {

            var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            ViewData["PeriodList"] = periods;

            var branches = db.Branch.ToList();
            ViewData["Branch"] = branches;

            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["StrTypes"] = types;

            var lis = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            ViewData["StructureType"] = lis;

            //var workItemCategories = from w in db.WorkCategory
            //                         join p in db.ProjectTypeMappings
            //                         on w.ConfigurationId equals p.ConfigurationId
            //                         select (new { w.WorkCategoryId, w.ConfigurationId, w.Description, w.Name, w.Index });
            ViewData["Categories"] = db.WorkCategory.Select(c => new { c.ConfigurationId, c.WorkCategoryId, c.Name }).ToList();

            var workItems = db.ProjectEstimationWorkItem.Select(w => new { w.ItemName, w.ProjectEstimationWorkItemId, w.WorkCategoryId }).ToList();
            ViewData["WorkItems"] = workItems;

            var uom = db.UnitOfMeasurement.ToList();
            ViewData["uom"] = uom.Select(
                x =>
                    new
                    {
                        UnitOfMeasurementId = x.UnitOfMeasurementId,
                        Abbreviation = x.Abbreviation
                    }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(p => new { p.ItemCategoryCode, p.Name });

            var items = db.InventoryItems.Select(i => new { i.ItemCode, i.Name }).ToList();
            ViewData["Items"] = items.ToList();

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();

            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));
            ViewData["Equipment"] = combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName });

            return View();
        }
        public ActionResult WorkQuantity()
        {
            var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            ViewData["PeriodList"] = periods;

            //ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();

            var branches = db.Branch.ToList();
            ViewData["Branch"] = branches;

            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["StrTypes"] = types;

            var lis = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            ViewData["StructureType"] = lis;

            //var workItemCategories = from w in db.WorkCategory
            //                         join p in db.ProjectTypeMappings
            //                         on w.ConfigurationId equals p.ConfigurationId
            //                         select (new { w.WorkCategoryId, w.ConfigurationId, w.Description, w.Name, w.Index });
            ViewData["Categories"] = db.WorkCategory.Select(c => new { c.Name, c.WorkCategoryId, c.ConfigurationId }).ToList();

            var workItems = db.ProjectEstimationWorkItem.Select(w => new { w.ItemName, w.ProjectEstimationWorkItemId }).ToList();
            ViewData["WorkItems"] = workItems;

            var uom = db.UnitOfMeasurement.ToList();
            ViewData["uom"] = uom.Select(
                x =>
                    new
                    {
                        UnitOfMeasurementId = x.UnitOfMeasurementId,
                        Abbreviation = x.Abbreviation
                    }).ToList();

            var itemCategories = db.ItemCategorys.Select(c => new { c.Name, c.ItemCategoryCode }).ToList();
            ViewData["ItemCategorys"] = itemCategories;

            var items = db.InventoryItems.Select(i => new { i.ItemCode, i.Name }).ToList();
            ViewData["Items"] = items;

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();

            //var categories = db.EquipmentIdentity.ToList();
            //ViewData["EquipmentIdentity"] = categories.Select(
            //    x =>
            //        new
            //        {
            //            EquipmentCategoryName = x.EquipmentCategoryName,
            //            EquipmentIdentityID = x.EquipmentIdentityID,
            //        }).ToList();

            //var equipmentTypes = db.EquipmentType.ToList();
            //ViewData["EquipmentType"] = equipmentTypes.Select(
            //    x =>
            //        new
            //        {
            //            EquipmentTypeName = x.EquipmentTypeName,
            //            EquipmentTypeID = x.EquipmentTypeID,
            //        }).ToList();

            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));
            ViewData["Equipment"] = combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName });



            return View();
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        public ActionResult ImportQuantitiesFromExcel(IEnumerable<HttpPostedFileBase> file)
        {
            if (file != null)
            {
                string fileNamePrefix = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day.ToString() + DateTime.Now.Hour + DateTime.Now.Minute;

                //return new JavaScriptResult() { Script = "alert('" + "Data Copied Successfully!!" + "');" };
                DataSet ds = new DataSet();
                #region import

                //foreach (HttpPostedFileBase fileBase in file)
                //{
                foreach (HttpPostedFileBase postedFileBase in file)
                {
                    //var postedFileBase = Request.Files["file"];
                    if (postedFileBase != null)
                    {
                        string fileExtension = System.IO.Path.GetExtension(postedFileBase.FileName);
                        if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                        {
                            string fileLocation = Server.MapPath("~/App_Data/Import/Construction/Estimation/") + fileNamePrefix +
                                                  DateTime.Now.Second.ToString() + postedFileBase.FileName;
                            if (System.IO.File.Exists(fileLocation))
                            {
                                System.IO.File.Delete(fileLocation);
                            }
                            postedFileBase.SaveAs(fileLocation);
                            var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                            if (fileExtension == ".xls")
                            {
                                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            }
                            //connection String for xlsx file format.
                            else if (fileExtension == ".xlsx")
                            {

                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }
                            else if (fileExtension == ".xlsm")
                            {

                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }
                            //Create Connection to Excel work book and add oledb namespace
                            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                            excelConnection.Open();
                            DataTable dt = new DataTable();

                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            if (dt == null)
                            {
                                return null;
                            }

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                // hanle the case when any filter is applied to the excell workbook
                                // excell adds other additional datatables with the name something like '_xlnm#_FilterDatabase'
                                // So, in such cases, some mechanism of removing the default _FilterDataBase have to be excluded from the count
                                if (row["TABLE_NAME"].ToString().Contains("_FilterDatabase")) { }
                                else
                                {
                                    excelSheets[t] = row["TABLE_NAME"].ToString();
                                    t++;
                                }
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                            {
                                dataAdapter.Fill(ds);
                            }

                        }
                    }
                }
                #endregion

                string errors = "";
                //decimal debit = 0;
                //decimal credit = 0;

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                    {
                        //GlBalanceImport import = new GlBalanceImport
                        //{
                        //    AccountCode = ds.Tables[0].Rows[j].ItemArray[0].ToString(),
                        //    AccountDesc = ds.Tables[0].Rows[j].ItemArray[1].ToString(),
                        //    Debit = ds.Tables[0].Rows[j].ItemArray[2].ToString(),
                        //    Credit = ds.Tables[0].Rows[j].ItemArray[3].ToString(),
                        //};

                        //glBalanceImports.Add(import);
                        //var workItem = new GenericWorkItemPerProject();
                        var id = 0;
                        var quantity = new decimal();
                        var idInString = ds.Tables[0].Rows[j].ItemArray[3].ToString();
                        var qtyInString = ds.Tables[0].Rows[j].ItemArray[6].ToString();
                        try
                        {
                            id = int.Parse(idInString);
                            //workItem = db.GenericWorkItemPerProject.Find(id);
                        }
                        catch (Exception)
                        {
                            errors = errors + " Id should be a number : " + idInString + "\n";
                        }
                        try
                        {
                            quantity = decimal.Parse(qtyInString);
                        }
                        catch (Exception)
                        {
                            errors = errors + " Quantity of Work should be decimal: " + qtyInString + "\n";
                        }
                        var workItem = db.GenericWorkItemPerProject.Find(id);
                        if (workItem != null)
                        {
                            workItem.TotalQuantityOfWork = quantity;

                            db.GenericWorkItemPerProject.Attach(workItem);
                            db.Entry(workItem).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }

            return RedirectToAction("WorkQuantity");
        }
        public ActionResult WorkItem_Read([DataSourceRequest] DataSourceRequest request, int currentProjectId)
        {
            var workItems =
                db.GenericWorkItemPerProject.Where(x => x.ProjectInformationId == currentProjectId && x.ParentId == 0);
            var workItemsToReturn = new List<GenericWorkItemPerProjectViewModel>();
            foreach (GenericWorkItemPerProject workItemPerProject in workItems)
            {
                var record = new GenericWorkItemPerProjectViewModel
                {
                    ProjectInformationId = workItemPerProject.ProjectInformationId,
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeOrderIndex(workItemPerProject.SubProjectTypeId) + " " + ProjectEstimationHelper.GetSubProjectTypeName(workItemPerProject.SubProjectTypeId),
                    StructureyType = ConsSettingHelper.GetStructureTypeCode(workItemPerProject.WorkCategory.ConfigurationId, workItemPerProject.ProjectType) + " " + workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Index + " " + workItemPerProject.WorkCategory.Name,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.GenericWorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId ?? 0,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<GenericWorkItemPerProjectViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult WorkItemUnderSubProjectType_Read([DataSourceRequest] DataSourceRequest request, int subProjectTypeId)
        {
            var workItems =
                db.GenericWorkItemPerProject.Where(x => x.SubProjectTypeId == subProjectTypeId && x.ParentId == 0);
            var workItemsToReturn = new List<GenericWorkItemPerProjectViewModel>();
            foreach (GenericWorkItemPerProject workItemPerProject in workItems)
            {
                var record = new GenericWorkItemPerProjectViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItemPerProject.SubProjectTypeId),
                    StructureyType = ConsSettingHelper.GetStructureTypeCode(workItemPerProject.WorkCategory.ConfigurationId, workItemPerProject.ProjectType) + " " + workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Index + " " + workItemPerProject.WorkCategory.Name,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.GenericWorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId ?? 0,
                    //UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<GenericWorkItemPerProjectViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        #region methods for quantity export and import
        public ActionResult WorkItemsForQualityImportExport_Read([DataSourceRequest] DataSourceRequest request, int currentProjectId)
        {
            var subCategoryWorkItems =
                db.GenericWorkItemPerProject.Where(x => x.ProjectInformationId == currentProjectId && x.IsSubCategory && x.ParentId == 0);
            var allChildWorkItems = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == currentProjectId && w.ParentId > 0).ToList();
            var workItemsToReturn = new List<GenericWorkItemQuantityImportViewModel>();
            foreach (GenericWorkItemPerProject workItemPerProject in subCategoryWorkItems)
            {
                var record = new GenericWorkItemQuantityImportViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItemPerProject.SubProjectTypeId),
                    StructureyType = ConsSettingHelper.GetStructureTypeCode(workItemPerProject.WorkCategory.ConfigurationId, workItemPerProject.ProjectType) + " " + workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Index + " " + workItemPerProject.WorkCategory.Name,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.GenericWorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,
                };
                workItemsToReturn.Add(record);
                var childOfThisWorkItem = allChildWorkItems.Where(c => c.ParentId == workItemPerProject.GenericWorkItemPerProjectId).ToList();
                foreach (GenericWorkItemPerProject childWorkItem in childOfThisWorkItem)
                {
                    var recordChild = new GenericWorkItemQuantityImportViewModel
                    {
                        SubProjectTypeName = record.SubProjectTypeName,
                        StructureyType = record.StructureyType,
                        WorkCategory = record.WorkCategory,
                        ContainsWorkItemAsInput = childWorkItem.ContainsWorkItemAsInput,
                        IsSubCategory = childWorkItem.IsSubCategory,
                        ItemCode = childWorkItem.ItemCode,
                        ItemName = childWorkItem.ItemName,
                        Id = childWorkItem.GenericWorkItemPerProjectId,
                        TotalQuantityOfWork = childWorkItem.TotalQuantityOfWork,
                        DirectCost = childWorkItem.DirectCost,
                        MaterialCost = childWorkItem.MaterialCost,
                        LabourOutPut = childWorkItem.LabourOutPut,
                        EquipmentOutput = childWorkItem.EquipmentOutput,
                        MaterialCostPerUnit = childWorkItem.MaterialCostPerUnit,
                        LaborCostPerUnit = childWorkItem.LaborCostPerUnit,
                        EquipmentCostPerUnit = childWorkItem.EquipmentCostPerUnit,
                        TotalUnitCost = childWorkItem.TotalUnitCost,
                        OverHeadCost = childWorkItem.OverHeadCost,
                        ProfitCost = childWorkItem.ProfitCost,
                        Remark = childWorkItem.Remark,
                        LabourHourlyOutPut = childWorkItem.LabourHourlyOutPut,
                        UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(childWorkItem.UnitOfMeasurementId ?? 0),
                        OverheadRate = childWorkItem.OverheadRate,
                        ProfitMargin = childWorkItem.ProfitMargin,

                        HasParent = true
                    };
                    workItemsToReturn.Add(recordChild);
                }
            }
            var idsAdded = new HashSet<int>(workItemsToReturn.Select(w => w.Id));
            var remainingWorkItems = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == currentProjectId && !(idsAdded.Contains(w.GenericWorkItemPerProjectId))).ToList();
            foreach (GenericWorkItemPerProject otherWorkItem in remainingWorkItems)
            {
                var recordRemaining = new GenericWorkItemQuantityImportViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(otherWorkItem.SubProjectTypeId),
                    StructureyType = otherWorkItem.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = otherWorkItem.WorkCategory.Name,
                    IsSubCategory = otherWorkItem.IsSubCategory,
                    ContainsWorkItemAsInput = otherWorkItem.ContainsWorkItemAsInput,
                    ItemCode = otherWorkItem.ItemCode,
                    ItemName = otherWorkItem.ItemName,
                    Id = otherWorkItem.GenericWorkItemPerProjectId,
                    TotalQuantityOfWork = otherWorkItem.TotalQuantityOfWork,
                    DirectCost = otherWorkItem.DirectCost,
                    MaterialCost = otherWorkItem.MaterialCost,
                    LabourOutPut = otherWorkItem.LabourOutPut,
                    EquipmentOutput = otherWorkItem.EquipmentOutput,
                    MaterialCostPerUnit = otherWorkItem.MaterialCostPerUnit,
                    LaborCostPerUnit = otherWorkItem.LaborCostPerUnit,
                    EquipmentCostPerUnit = otherWorkItem.EquipmentCostPerUnit,
                    TotalUnitCost = otherWorkItem.TotalUnitCost,
                    OverHeadCost = otherWorkItem.OverHeadCost,
                    ProfitCost = otherWorkItem.ProfitCost,
                    Remark = otherWorkItem.Remark,
                    LabourHourlyOutPut = otherWorkItem.LabourHourlyOutPut,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(otherWorkItem.UnitOfMeasurementId ?? 0),
                    OverheadRate = otherWorkItem.OverheadRate,
                    ProfitMargin = otherWorkItem.ProfitMargin,
                };
                workItemsToReturn.Add(recordRemaining);
            }
            IQueryable<GenericWorkItemQuantityImportViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult WorkItemForQualityImportExport_Update([DataSourceRequest] DataSourceRequest request, GenericWorkItemQuantityImportViewModel workItemModel)
        {
            if (ModelState.IsValid)
            {
                var entity = db.GenericWorkItemPerProject.Find(workItemModel.Id);
                if (entity != null)
                {

                    entity.TotalQuantityOfWork = workItemModel.TotalQuantityOfWork;



                    db.GenericWorkItemPerProject.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    //ProjectListController.UpdateProjectTotalCost(entity.WorkItemPerProjectId);
                    ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);

                }
            }
            return Json(new[] { workItemModel }.ToDataSourceResult(request, ModelState));
        }
        #endregion
        public ActionResult WorkItemForCostNegotiation_Read([DataSourceRequest] DataSourceRequest request, int currentProjectId)
        {
            var workItems =
                db.GenericWorkItemPerProject.Where(x => x.ProjectInformationId == currentProjectId && x.ParentId == 0);
            var workItemsToReturn = new List<GenericWorkItemPerProjectViewModel>();
            foreach (GenericWorkItemPerProject workItemPerProject in workItems)
            {
                var record = new GenericWorkItemPerProjectViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeOrderIndex(workItemPerProject.SubProjectTypeId) + " " + ProjectEstimationHelper.GetSubProjectTypeName(workItemPerProject.SubProjectTypeId),
                    StructureyType = ConsSettingHelper.GetStructureTypeCode(workItemPerProject.WorkCategory.ConfigurationId, workItemPerProject.ProjectType) + " " + workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Index + " " + workItemPerProject.WorkCategory.Name,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.GenericWorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,

                    Amount = workItemPerProject.TotalQuantityOfWork * workItemPerProject.TotalUnitCost
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<GenericWorkItemPerProjectViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult WorkItemsToImport_Read([DataSourceRequest] DataSourceRequest request, int currentProjectId, int? workBreakDownStructureId)
        {
            var workItems =
                db.WorkItemPerProject.Where(x => x.ProjectInformationId == currentProjectId && x.ParentId == 0);
            var workItemsToReturn = new List<ImportWorkItemToScheduleViewModel>();
            var currentWBSId = workBreakDownStructureId ?? 0;
            foreach (WorkItemPerProject workItemPerProject in workItems)
            {
                var record = new ImportWorkItemToScheduleViewModel
                {
                    BlockName = ProjectEstimationHelper.GetBlockName(workItemPerProject.BlockId),
                    StructureyType = workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Name,

                    IsImported = this.WorkItemIsImportedToToWBS(currentWBSId, workItemPerProject.WorkItemPerProjectId),

                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.WorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<ImportWorkItemToScheduleViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult WorkItemUnderWorkItem_Read([DataSourceRequest] DataSourceRequest request, int parentWorkItemId)
        {
            var workItems =
                db.GenericWorkItemPerProject.Where(x => x.ParentId == parentWorkItemId);
            var workItemsToReturn = new List<GenericWorkItemPerProjectViewModel>();
            foreach (GenericWorkItemPerProject workItemPerProject in workItems)
            {
                var record = new GenericWorkItemPerProjectViewModel
                {
                    ProjectInformationId = workItemPerProject.ProjectInformationId,
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItemPerProject.SubProjectTypeId),
                    StructureyType = workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Name,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.GenericWorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurementId = workItemPerProject.UnitOfMeasurementId??0,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<GenericWorkItemPerProjectViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult WorkItemUnderWorkItemForCosNegotiation_Read([DataSourceRequest] DataSourceRequest request, int parentWorkItemId)
        {
            var workItems =
                db.GenericWorkItemPerProject.Where(x => x.ParentId == parentWorkItemId);
            var workItemsToReturn = new List<GenericWorkItemPerProjectViewModel>();
            foreach (GenericWorkItemPerProject workItemPerProject in workItems)
            {
                var record = new GenericWorkItemPerProjectViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItemPerProject.SubProjectTypeId),
                    StructureyType = workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Name,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.GenericWorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,

                    Amount = workItemPerProject.TotalQuantityOfWork * workItemPerProject.TotalUnitCost
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<GenericWorkItemPerProjectViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItem_Update([DataSourceRequest] DataSourceRequest request,
            GenericWorkItemPerProjectViewModel workItemModel)
        {
            if (ModelState.IsValid)
            {
                var entity = db.GenericWorkItemPerProject.Find(workItemModel.Id);
                if (entity != null)
                {
                    entity.ItemName = workItemModel.ItemName;
                    entity.LabourHourlyOutPut = workItemModel.LabourHourlyOutPut;
                    entity.TotalQuantityOfWork = workItemModel.TotalQuantityOfWork;
                    entity.ItemCode = workItemModel.ItemCode;
                    entity.UnitOfMeasurementId = workItemModel.UnitOfMeasurementId;
                    entity.Remark = workItemModel.Remark;


                    db.GenericWorkItemPerProject.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    //ProjectListController.UpdateProjectTotalCost(entity.WorkItemPerProjectId);
                    ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);

                    // consider the scenario where the item has been defined as subcategory item and had child items ... and 
                    // on demand if that item is changed to regular item, meaning the child items will remain within the project
                    // but with false reference to their previous parent which currently couldnt have any child.
                    if (!entity.IsSubCategory)
                    {
                        ProjectEstimationHelper.CleanFalseChildItems(entity.GenericWorkItemPerProjectId);
                    }

                }
            }
            return Json(new[] { workItemModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItem_Destroy([DataSourceRequest]DataSourceRequest request, GenericWorkItemPerProjectViewModel workItemModel)
        {
            if (ModelState.IsValid)
            {
                var entity = db.GenericWorkItemPerProject.Find(workItemModel.Id);
                if (entity != null)
                {
                    db.GenericWorkItemPerProject.Attach(entity);
                    db.GenericWorkItemPerProject.Remove(entity);
                    db.SaveChanges();
                }
                if (entity.IsSubCategory || entity.ContainsWorkItemAsInput)
                {
                    ProjectEstimationHelper.CleanReferencesFromProjectOnGenericWorkItemDelete(entity);
                }

            }

            return Json(new[] { workItemModel }.ToDataSourceResult(request, ModelState));
        }

        // 
        //  Summary:
        //      Piece rate under a given project
        public ActionResult PieceRate_Read([DataSourceRequest] DataSourceRequest request, int projIdRead)
        {
            var workItems =
                db.GenericAreaWorkItem.Where(x => x.ProjectInformationId == projIdRead);
            var workItemsToReturn = new List<GenericPieceRateViewModel>();
            foreach (GenericAreaWorkItem workItem in workItems)
            {
                var record = new GenericPieceRateViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItem.SubProjectTypeId),
                    StructureyType = workItem.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItem.WorkCategory.Name,
                    Id = workItem.GenericAreaWorkItemId,
                    Price = workItem.Price,
                    WorkItemName = workItem.WorkItemName,
                    ItemCode = workItem.ItemCode,
                    DirectCost = workItem.DirectCost,
                    Width = workItem.Width,
                    Height = workItem.Height,
                    Area = workItem.Area,
                    CylinderLock = workItem.CylinderLock,
                    OverHeadProfitCost = workItem.OverHeadProfitCost,
                    ProjectInformationId = workItem.ProjectInformationId,
                    MaterialCost = workItem.MaterialCost,

                    TransportCost = workItem.TransportCost,
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<GenericPieceRateViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        // 
        //  Summary:
        //      Sub-contract work items 
        public ActionResult SubContractWorkItem_Read([DataSourceRequest] DataSourceRequest request, int projIdReadSub)
        {
            var workItems =
                db.GenericSubContractWorkItem.Where(x => x.ProjectInformationId == projIdReadSub);
            var workItemsToReturn = new List<GenericSubContractWorkItemViewModel>();
            foreach (GenericSubContractWorkItem workItem in workItems)
            {
                var record = new GenericSubContractWorkItemViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItem.SubProjectTypeId),
                    StructureyType = workItem.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItem.WorkCategory.Name,
                    Id = workItem.GenericSubContractWorkItemId,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId),
                    WorkItemName = workItem.WorkItemName,
                    ItemCode = workItem.ItemCode,
                    Quntity = workItem.Quntity,
                    SubContractCostOne = workItem.SubContractCostOne,
                    SubContractCostTwo = workItem.SubContractCostTwo,
                    SubContractCostThree = workItem.SubContractCostThree,
                    SelectedCost = workItem.SelectedCost,
                    TotalCost = workItem.TotalCost,
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<GenericSubContractWorkItemViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //
        //  Summary:
        //      custom work item
        public ActionResult CustomWorkItems_Read([DataSourceRequest] DataSourceRequest request, int projIdReadCustom)
        {
            var workItems =
                db.GenericProjectCustomWorkItem.Where(x => x.ProjectInformationId == projIdReadCustom);
            var workItemsToReturn = new List<GenericProjectCustomWorkItemViewModel>();
            foreach (GenericProjectCustomWorkItem workItem in workItems)
            {
                var record = new GenericProjectCustomWorkItemViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItem.SubProjectTypeId),
                    WorkCategory = ProjectEstimationHelper.GetCustomWorkItemCategory(workItem.WorkCategoryId).Name,
                    StructureyType = ProjectEstimationHelper.GetCustomWorkItemCategory(workItem.WorkCategoryId).Configuration.CurrentValue.Replace('_', ' '),
                    Id = workItem.GenericProjectCustomWorkItemId,
                    WorkItemName = workItem.WorkItemName,
                    TotalUnitRate = workItem.TotalUnitRate,
                    TotalQuantityOfWork = workItem.TotalQuantityOfWork,

                    DirectCost = workItem.DirectCost,
                    MaterialCostPerUnit = workItem.MaterialCostPerUnit,
                    LaborCostPerUnit = workItem.LaborCostPerUnit,
                    EquipmentCostPerUnit = workItem.EquipmentCostPerUnit,
                    OverHeadCost = workItem.OverHeadCost,
                    ProfitCost = workItem.ProfitCost,
                    HourlyOutPut = workItem.HourlyOutPut,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItem.OverheadRate,
                    ProfitRate = workItem.ProfitRate,

                    TotalCost = workItem.TotalCost,
                    ItemCode = workItem.ItemCode
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<GenericProjectCustomWorkItemViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Update([DataSourceRequest] DataSourceRequest request,
            ProjectCustomWorkItemViewModel workItemModel)
        {
            if (ModelState.IsValid)
            {
                var entity = db.ProjectCustomWorkItems.Find(workItemModel.Id);
                if (entity != null)
                {


                    entity.WorkItemName = workItemModel.WorkItemName;
                    entity.Quantity = workItemModel.Quantity;
                    entity.TotalCost = workItemModel.Quantity * workItemModel.TotalUnitRate;

                    db.ProjectCustomWorkItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();


                }
            }
            return Json(new[] { workItemModel }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Destroy([DataSourceRequest] DataSourceRequest request,
            ProjectCustomWorkItemViewModel workItemModel)
        {
            if (ModelState.IsValid)
            {
                var entity = db.ProjectCustomWorkItems.Find(workItemModel.Id);
                if (entity != null)
                {
                    db.ProjectCustomWorkItems.Attach(entity);
                    db.ProjectCustomWorkItems.Remove(entity);
                    db.SaveChanges();
                }
            }
            return Json(new[] { workItemModel }.ToDataSourceResult(request, ModelState));
        }
        private string GetTitle(int id)
        {
            var pJTitle = db.ProjectJobTitles.Where(x => x.ProjectJobTitleId == id).FirstOrDefault();
            if (pJTitle != null)
            {
                return pJTitle.JobTitle;
            }
            return "";
        }
        private bool WorkItemIsImportedToToWBS(int wbsId, int workItemId)
        {
            var isImported = false;
            //var temp = db.Tasks.FirstOrDefault(t => t.ProjectWorkBreakDownId == wbsId &&
            //                            t.WorkItemDuration.WorkItemPerProjectId == workItemId);
            //if(temp != null)
            //{
            //    isImported = true;
            //}
            return isImported;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}