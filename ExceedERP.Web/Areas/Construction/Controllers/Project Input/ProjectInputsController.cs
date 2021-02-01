using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project_Input
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class ProjectInputsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static decimal DeseretAllowance { get; set; }
        public static decimal IndexFactor { get; set; }
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;

            var jobTitle = db.ProjectJobTitles.ToList();
            ViewData["titles"] = jobTitle;
            #region temp
            //var desertAllowance =
            //    db.AllowanceDefinations.Where(x => x.AllowanceType == AllowanceType.DESERT).FirstOrDefault();
            //if (desertAllowance != null)
            //{
            //    DeseretAllowance = desertAllowance.Amount;
            //}
            //var setting =
            //    db.Configurations.Where(x => x.Category == "Construction" && x.Reference == "INDEX_FaCTOR")
            //        .FirstOrDefault();
            //if (setting != null)
            //{
            //    IndexFactor = Convert.ToDecimal(setting.CurrentValue);
            //} 
            #endregion

            #region Structure types concatnated with code/series value(if any)
            var list = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            List<Configuration> listToReturn = new List<Configuration>();
            if (list.Any())
            {
                foreach (var config in list)
                {
                    var strToReturn = new Configuration
                    {
                        ConfigurationId = config.ConfigurationId,
                        CurrentValue = config.CurrentValue
                    };
                    var currentProjectTypeMapping = db.ProjectTypeMappings.FirstOrDefault(m => m.ConfigurationId == config.ConfigurationId);

                    if (currentProjectTypeMapping != null && currentProjectTypeMapping.ProjectType == AvailableProjectTypes.Road)
                    {
                        strToReturn.CurrentValue = strToReturn.CurrentValue + " (Series " + currentProjectTypeMapping.CodeOrSeries + " )";
                    }
                    listToReturn.Add(strToReturn);
                }
            }
            ViewData["StructureType"] = listToReturn;
            #endregion

            var externalEquipments = db.Fleets.Where(fl => fl.IsExternal).Select(x => new { x.FleetsID, x.EquipmentName });
            //var itemsList = externalEquipments;
            ViewData["ExternalEquipments"] = externalEquipments.ToList();

            var fleetItems = db.Fleets.Where(x => !x.IsExternal).Select(x => new { x.FleetsID, x.EquipmentName });
            ViewData["InternalItems"] = fleetItems.ToList();

            var itempriceidecis = db.InventoryItems.Select(i => new { i.ItemCode, i.Name});
            ViewData["ItemPriceIndex"] = itempriceidecis.ToList();

            var itempriceidecisCat = db.ItemCategorys.Select(c => new { c.ItemCategoryCode,c.Name});
            ViewData["ItemPriceCatego"] = itempriceidecisCat.ToList();

            var branchList = db.Branch.Select(b => new { b.BranchId, b.Name});
            ViewData["Branches"] = branchList.ToList();

            ViewData["GLPeriods"] = db.InventoryPeriods.Select(p => new { p.InventoryPeriodId, p.Name}).ToList();

            //var fuelOnly = db.ConstructionItemCategories.Where(c => c.IsFuel);
            var fuels = (from i in db.InventoryItems
                         join f in db.ConstructionItemCategories
                         on i.ItemCategoryCode equals f.ItemCategoryCode
                         where f.IsFuel
                         select (new { i.ItemCode, i.Name })).ToList();
            ViewData["FuelTypes"] = fuels;

            //PrepareMaterialIndex();

            this.SetExpiryInfo();

            return View();
        }

        /// <summary>
        /// check if this material exists in MaterialPriceIndex table
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        private bool CheckItemExistance(string materialId)
        {
            var material = db.MaterialPriceIndexIndices.Where(x => x.ItemCode == materialId).FirstOrDefault();
            if (material != null)
            {
                return true;
            }
            return false;
        }
        private bool IsValidBranch(int branchId)
        {
            bool isValid = false;
            var currentBrach = db.Branch.Where(b => b.BranchId == branchId).FirstOrDefault();
            if (currentBrach != null)
            {
                isValid = true;
            }
            return isValid;
        }
        private bool IsValidPeriod(int periodId)
        {
            bool isValid = false;
            var currentPeriod = db.GLPeriods.Find(periodId);
            if (currentPeriod != null)
            {
                isValid = true;
            }
            return isValid;
        }
        private void PrepareMaterialIndex()
        {
            var constructionItemCategories = db.ConstructionItemCategories.ToList();
            if (constructionItemCategories.Any())
            {
                foreach (ConstructionItemCategory itemCategory in constructionItemCategories)
                {
                    var constructionMaterialList = db.ItemPriceIndexes.Where(item => item.ItemCategoryCode == itemCategory.ItemCategoryCode).ToList();
                    if (constructionMaterialList.Any())
                    {
                        foreach (ItemPriceIndex itemPriceIndex in constructionMaterialList)
                        {
                            if (CheckItemExistance(itemPriceIndex.ItemCode))
                            {
                                var currentMaterialIndex = db.MaterialPriceIndexIndices.Where(m => m.ItemCode == itemPriceIndex.ItemCode).FirstOrDefault();

                                // the current item has registered in the construction material price index
                                // and the only thing needed here is to get the latest cost
                                if (currentMaterialIndex != null)
                                {
                                    //var coreMaterial = db.ItemPriceIndexes.Where(m => m.ItemCode == currentMaterialIndex.ItemCode).FirstOrDefault();
                                    var entity = new MaterialPriceIndex
                                    {
                                        MaterialPriceIndexId = currentMaterialIndex.MaterialPriceIndexId,
                                        BranchId = currentMaterialIndex.BranchId,
                                        SourceType = currentMaterialIndex.SourceType,
                                        GLPeriodId = currentMaterialIndex.GLPeriodId,
                                        DateOfInformation = currentMaterialIndex.DateOfInformation,
                                        ItemCategoryCode = currentMaterialIndex.ItemCategoryCode,
                                        ItemCode = currentMaterialIndex.ItemCode,
                                        ApprovedCost = currentMaterialIndex.ApprovedCost,
                                        Remark = currentMaterialIndex.Remark,
                                        OrginalCost = itemPriceIndex.PriceBeforeVAT,
                                        OrginalCostGRN = itemPriceIndex.PriceBeforeVAT,
                                        ExternalCost = currentMaterialIndex.ExternalCost,
                                        TotalCost = currentMaterialIndex.TotalCost,
                                        TransportCost = currentMaterialIndex.TransportCost,
                                        Uom = InventoryHelper.GetUnitOfMeasurment(currentMaterialIndex.ItemCode),
                                        //LoadingUnloading = currentMaterialIndex.LoadingUnloading,
                                        CreatedBy = currentMaterialIndex.CreatedBy,
                                        DateCreated = currentMaterialIndex.DateCreated,
                                        ModifiedBy = User.Identity.Name,
                                        LastModified = DateTime.Now.Date
                                    };
                                    entity.TotalCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost;

                                    db = new ExceedDbContext();
                                    db.MaterialPriceIndexIndices.Attach(entity);
                                    db.Entry(entity).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                            }
                            // add that material to the material price index
                            else
                            {
                                //if (IsValidBranch(itemPriceIndex.BranchId) && IsValidPeriod(itemPriceIndex.GLPeriodId))
                                //{
                                var entity = new MaterialPriceIndex
                                {
                                    BranchId = itemPriceIndex.BranchId,
                                    SourceType = itemPriceIndex.SourceType,

                                    ItemCategoryCode = itemPriceIndex.ItemCategoryCode,
                                    ItemCode = itemPriceIndex.ItemCode,
                                    GLPeriodId = itemPriceIndex.GLPeriodId,
                                    DateOfInformation = itemPriceIndex.DateOfInformation,
                                    ApprovedCost = 0,
                                    Remark = itemPriceIndex.Remark,
                                    OrginalCost = itemPriceIndex.PriceBeforeVAT,
                                    OrginalCostGRN = itemPriceIndex.PriceBeforeVAT,
                                    ExternalCost = itemPriceIndex.PriceBeforeVAT,
                                    TotalCost = 0,
                                    TransportCost = 0,
                                    Uom = InventoryHelper.GetUnitOfMeasurment(itemPriceIndex.ItemCode),
                                    //LoadingUnloading = 0,
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now.Date,
                                    ModifiedBy = null,
                                    LastModified = null
                                };
                                entity.TotalCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost;
                                entity.ApprovedCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost;

                                db = new ExceedDbContext();
                                db.MaterialPriceIndexIndices.Add(entity);
                                db.SaveChanges();
                                //}


                                //materialPriceIndex.MaterialPriceIndexId = entity.MaterialPriceIndexId;
                            }
                        }
                    }
                }
            }
            #region Existing code
            //var existingMaterials = db.MaterialPriceIndexIndices.ToList();
            ////var itemPrice = db.ItemPriceIndexes.ToList();
            //MaterialPriceIndex materialPriceIndex = new MaterialPriceIndex();
            //foreach (var existineMaterial in existingMaterials)
            //{
            //    materialPriceIndex = new MaterialPriceIndex();

            //    if (CheckItemExistance(existineMaterial.ItemCode))
            //    {
            //        var coreMaterial = db.ItemPriceIndexes.Where(m => m.ItemCode == existineMaterial.ItemCode).FirstOrDefault();

            //        materialPriceIndex =
            //            db.MaterialPriceIndexIndices.Where(x => x.ItemCode == existineMaterial.ItemCode).FirstOrDefault();
            //        if (materialPriceIndex != null)
            //        {
            //            var entity = new MaterialPriceIndex
            //            {

            //                MaterialPriceIndexId = materialPriceIndex.MaterialPriceIndexId,
            //                BranchId = materialPriceIndex.BranchId,
            //                SourceType = materialPriceIndex.SourceType,
            //                //InventoryItem = materialPriceIndex.InventoryItem,
            //                ItemCategoryCode = materialPriceIndex.ItemCategoryCode,
            //                ItemCode = materialPriceIndex.ItemCode,
            //                ApprovedCost = materialPriceIndex.ApprovedCost,
            //                Remark = materialPriceIndex.Remark,
            //                OrginalCost = coreMaterial.PriceBeforeVAT,
            //                OrginalCostGRN = coreMaterial.PriceBeforeVAT,
            //                ExternalCost = materialPriceIndex.ExternalCost,
            //                TotalCost = materialPriceIndex.TotalCost,
            //                TransportCost = materialPriceIndex.TransportCost,
            //                Uom = InventoryHelper.GetUnitOfMeasurment(existineMaterial.ItemCode),
            //                LoadingUnloading = materialPriceIndex.LoadingUnloading,
            //                CreatedBy = materialPriceIndex.CreatedBy,
            //                DateCreated = materialPriceIndex.DateCreated,
            //                ModifiedBy = User.Identity.Name,
            //                LastModified = DateTime.Now.Date
            //            };
            //            entity.TotalCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost + entity.LoadingUnloading;

            //            db = new ExceedDbContext();
            //            db.MaterialPriceIndexIndices.Attach(entity);
            //            db.Entry(entity).State = EntityState.Modified;
            //            db.SaveChanges();
            //        }

            //    }
            //    else
            //    {
            //        var entity = new MaterialPriceIndex
            //        {

            //            BranchId = i.BranchId,
            //            SourceType = i.SourceType,

            //            ItemCategoryCode = i.ItemCategoryCode,
            //            ItemCode = i.ItemCode,
            //            ApprovedCost = 0,
            //            Remark = i.Remark,
            //            OrginalCost = i.PriceBeforeVAT,
            //            OrginalCostGRN = i.PriceBeforeVAT,
            //            ExternalCost = i.PriceBeforeVAT,
            //            TotalCost = 0,
            //            TransportCost = 0,
            //            Uom = InventoryHelper.GetUnitOfMeasurment(i.ItemCode),
            //            LoadingUnloading = 0,
            //            CreatedBy = User.Identity.Name,
            //            DateCreated = DateTime.Now.Date,
            //            ModifiedBy = null,
            //            LastModified = null
            //        };
            //        entity.TotalCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost + entity.LoadingUnloading;
            //        entity.TotalCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost + entity.LoadingUnloading;

            //        db = new ExceedDbContext();
            //        db.MaterialPriceIndexIndices.Add(entity);
            //        db.SaveChanges();
            //        materialPriceIndex.MaterialPriceIndexId = entity.MaterialPriceIndexId;
            //    }
            //} 
            #endregion
        }
        public ActionResult FetchLatestPricesFromInventoryPriceIndex()
        {
            var constructionItemCategories = db.ConstructionItemCategories.ToList();
            if (constructionItemCategories.Any())
            {
                foreach (ConstructionItemCategory itemCategory in constructionItemCategories)
                {
                    var constructionMaterialList = db.ItemPriceIndexes.Where(item => item.ItemCategoryCode == itemCategory.ItemCategoryCode).ToList();
                    if (constructionMaterialList.Any())
                    {
                        foreach (ItemPriceIndex itemPriceIndex in constructionMaterialList)
                        {
                            if (CheckItemExistance(itemPriceIndex.ItemCode))
                            {
                                var currentMaterialIndex = db.MaterialPriceIndexIndices.Where(m => m.ItemCode == itemPriceIndex.ItemCode).FirstOrDefault();

                                // the current item has registered in the construction material price index
                                // and the only thing needed here is to get the latest cost
                                if (currentMaterialIndex != null)
                                {
                                    //var coreMaterial = db.ItemPriceIndexes.Where(m => m.ItemCode == currentMaterialIndex.ItemCode).FirstOrDefault();
                                    var entity = new MaterialPriceIndex
                                    {
                                        MaterialPriceIndexId = currentMaterialIndex.MaterialPriceIndexId,
                                        BranchId = currentMaterialIndex.BranchId,
                                        SourceType = currentMaterialIndex.SourceType,
                                        GLPeriodId = currentMaterialIndex.GLPeriodId,
                                        DateOfInformation = currentMaterialIndex.DateOfInformation,
                                        ItemCategoryCode = currentMaterialIndex.ItemCategoryCode,
                                        ItemCode = currentMaterialIndex.ItemCode,
                                        ApprovedCost = currentMaterialIndex.ApprovedCost,
                                        Remark = currentMaterialIndex.Remark,
                                        OrginalCost = itemPriceIndex.PriceBeforeVAT,
                                        OrginalCostGRN = itemPriceIndex.PriceBeforeVAT,
                                        ExternalCost = currentMaterialIndex.ExternalCost,
                                        TotalCost = currentMaterialIndex.TotalCost,
                                        TransportCost = currentMaterialIndex.TransportCost,
                                        Uom = InventoryHelper.GetUnitOfMeasurment(currentMaterialIndex.ItemCode),
                                        //LoadingUnloading = currentMaterialIndex.LoadingUnloading,
                                        CreatedBy = currentMaterialIndex.CreatedBy,
                                        DateCreated = currentMaterialIndex.DateCreated,
                                        ModifiedBy = User.Identity.Name,
                                        LastModified = DateTime.Now.Date
                                    };
                                    entity.TotalCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost;

                                    db = new ExceedDbContext();
                                    db.MaterialPriceIndexIndices.Attach(entity);
                                    db.Entry(entity).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                            }
                            // add that material to the material price index
                            else
                            {
                                //if (IsValidBranch(itemPriceIndex.BranchId) && IsValidPeriod(itemPriceIndex.GLPeriodId))
                                //{
                                var entity = new MaterialPriceIndex
                                {
                                    BranchId = itemPriceIndex.BranchId,
                                    SourceType = itemPriceIndex.SourceType,

                                    ItemCategoryCode = itemPriceIndex.ItemCategoryCode,
                                    ItemCode = itemPriceIndex.ItemCode,
                                    GLPeriodId = itemPriceIndex.GLPeriodId,
                                    DateOfInformation = itemPriceIndex.DateOfInformation,
                                    ApprovedCost = 0,
                                    Remark = itemPriceIndex.Remark,
                                    OrginalCost = itemPriceIndex.PriceBeforeVAT,
                                    OrginalCostGRN = itemPriceIndex.PriceBeforeVAT,
                                    ExternalCost = itemPriceIndex.PriceBeforeVAT,
                                    TotalCost = 0,
                                    TransportCost = 0,
                                    Uom = InventoryHelper.GetUnitOfMeasurment(itemPriceIndex.ItemCode),
                                    //LoadingUnloading = 0,
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now.Date,
                                    ModifiedBy = null,
                                    LastModified = null
                                };
                                entity.TotalCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost;
                                entity.ApprovedCost = (entity.OrginalCost > entity.ExternalCost ? entity.OrginalCost : entity.ExternalCost) + entity.TransportCost;

                                db = new ExceedDbContext();
                                db.MaterialPriceIndexIndices.Add(entity);
                                db.SaveChanges();
                                //}


                                //materialPriceIndex.MaterialPriceIndexId = entity.MaterialPriceIndexId;
                            }
                        }
                    }
                }
            }

            return Redirect("Index");
        }
        private void SetExpiryInfo()
        {
            var feulTypes = db.ConstructionItemCategories.Where(c => c.IsFuel).ToList();
            var codes = new HashSet<string>(feulTypes.Select(c => c.ItemCategoryCode));
            var allMaterialPrices = db.MaterialPriceIndexIndices.Where(p => (codes.Contains(p.ItemCategoryCode))).ToList();
            if (allMaterialPrices.Any())
            {
                foreach (MaterialPriceIndex priceIndex in allMaterialPrices)
                {
                    var currentDate = DateTime.Now;
                    var pricePeriod = db.InventoryPeriods.Find(priceIndex.GLPeriodId);
                    if (pricePeriod != null)
                    {
                        if (pricePeriod.DateTo < currentDate)
                        {
                            priceIndex.IsExpired = true;

                            db.MaterialPriceIndexIndices.Attach(priceIndex);
                            db.Entry(priceIndex).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        //public ActionResult ProjectEstimationIndexLabour_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    IQueryable<ProjectEstimationIndexLabour> projectestimationindexlabour = db.ProjectEstimationIndexLabour;
        //    DataSourceResult result = projectestimationindexlabour.ToDataSourceResult(request,
        //        projectEstimationIndexLabour => new
        //        {
        //            ProjectEstimationIndexLabourId = projectEstimationIndexLabour.ProjectEstimationIndexLabourId,

        //            SalaryperMonthMin = projectEstimationIndexLabour.SalaryperMonthMin,
        //            SalaryperMonthMax = projectEstimationIndexLabour.SalaryperMonthMax,
        //            SalaryperdayMin = projectEstimationIndexLabour.SalaryperdayMin,
        //            SalaryperdayMax = projectEstimationIndexLabour.SalaryperdayMax,
        //            HourlyAverageSalary = projectEstimationIndexLabour.HourlyAverageSalary,
        //            IndexedHourlySalary = projectEstimationIndexLabour.IndexedHourlySalary > 0 ? projectEstimationIndexLabour.IndexedHourlySalary :
        //                                    projectEstimationIndexLabour.HourlyAverageSalary * projectEstimationIndexLabour.IndexFactory,
        //            ApprovedIndexedHourlySalary = projectEstimationIndexLabour.ApprovedIndexedHourlySalary,
        //            ProjectJobTitleId = projectEstimationIndexLabour.ProjectJobTitleId,
        //            Unit = projectEstimationIndexLabour.Unit,
        //            IndexFactory = projectEstimationIndexLabour.IndexFactory,
        //            BirrMonth = projectEstimationIndexLabour.BirrMonth,
        //            IdealBirrDay = projectEstimationIndexLabour.IdealBirrDay,
        //            ActualBirrDay = projectEstimationIndexLabour.ActualBirrDay,
        //            IdealBirrDays = projectEstimationIndexLabour.IdealBirrDays,
        //            ActualBirrDays = projectEstimationIndexLabour.ActualBirrDays,
        //            DesertBirrDays = projectEstimationIndexLabour.DesertBirrDays,
        //            NetBirrDays = projectEstimationIndexLabour.NetBirrDays,
        //            NetBirrHour = projectEstimationIndexLabour.NetBirrHour,
        //            FormHr = projectEstimationIndexLabour.FormHr,

        //        });

        //    return Json(result);
        //}

        //private bool CheckExisatnce(int titleId)
        //{
        //    var labours = db.ProjectEstimationIndexLabour.Where(x => x.ProjectJobTitleId == titleId).FirstOrDefault();
        //    if (labours != null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ProjectEstimationIndexLabour_Create([DataSourceRequest] DataSourceRequest request,
        //    ProjectEstimationIndexLabour projectEstimationIndexLabour)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new ProjectEstimationIndexLabour();
        //        if (CheckExisatnce(projectEstimationIndexLabour.ProjectJobTitleId))
        //        {

        //            ModelState.AddModelError("", "This Job title is already registered!!");
        //        }
        //        else
        //        {
        //            entity.ProjectEstimationIndexLabourId = projectEstimationIndexLabour.ProjectEstimationIndexLabourId;

        //            entity.ProjectJobTitleId = projectEstimationIndexLabour.ProjectJobTitleId;
        //            entity.Unit = GetBasicSalary(projectEstimationIndexLabour.ProjectJobTitleId,
        //                projectEstimationIndexLabour.Unit);

        //            entity.SalaryperMonthMin = this.GetMinMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId);
        //            entity.SalaryperMonthMax = this.GetMaxMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId);
        //            entity.SalaryperdayMin = entity.SalaryperMonthMin / 26;
        //            entity.SalaryperdayMax = entity.SalaryperMonthMax / 26;
        //            entity.HourlyAverageSalary = (this.GetAverageMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId)) / (26 * 8);
        //            entity.IndexedHourlySalary = entity.HourlyAverageSalary * IndexFactor;
        //            entity.ApprovedIndexedHourlySalary = entity.ApprovedIndexedHourlySalary > 0 ? entity.ApprovedIndexedHourlySalary : entity.IndexedHourlySalary;

        //            entity.IndexFactory = IndexFactor;
        //            entity.BirrMonth = entity.Unit * entity.IndexFactory;
        //            entity.IdealBirrDay = entity.BirrMonth / 24;
        //            entity.ActualBirrDay = (entity.IdealBirrDay * 30) / 24;
        //            entity.IdealBirrDays = GetPerdimeAmount(entity.Unit);
        //            entity.ActualBirrDays = (entity.IdealBirrDays * 30) / 26;
        //            entity.DesertBirrDays = (DeseretAllowance / 100) * entity.ActualBirrDays;
        //            entity.NetBirrDays = entity.ActualBirrDay + entity.ActualBirrDays + entity.DesertBirrDays;
        //            entity.NetBirrHour = entity.NetBirrDays / 8;
        //            entity.FormHr = projectEstimationIndexLabour.FormHr;

        //            db.ProjectEstimationIndexLabour.Add(entity);
        //            db.SaveChanges();
        //            projectEstimationIndexLabour.ProjectEstimationIndexLabourId = entity.ProjectEstimationIndexLabourId;
        //        }
        //    }

        //    return Json(new[] { projectEstimationIndexLabour }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ProjectEstimationIndexLabour_Update([DataSourceRequest] DataSourceRequest request,
        //    ProjectEstimationIndexLabour projectEstimationIndexLabour)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new ProjectEstimationIndexLabour();

        //        entity.ProjectEstimationIndexLabourId = projectEstimationIndexLabour.ProjectEstimationIndexLabourId;

        //        entity.SalaryperMonthMin = this.GetMinMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId);
        //        entity.SalaryperMonthMax = this.GetMaxMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId);
        //        entity.SalaryperdayMin = entity.SalaryperMonthMin / 26;
        //        entity.SalaryperdayMax = entity.SalaryperMonthMax / 26;
        //        entity.HourlyAverageSalary = (this.GetAverageMonthlySalary(projectEstimationIndexLabour.ProjectJobTitleId)) / (26 * 8);
        //        entity.IndexedHourlySalary = entity.HourlyAverageSalary * IndexFactor;
        //        entity.ApprovedIndexedHourlySalary = projectEstimationIndexLabour.ApprovedIndexedHourlySalary;

        //        entity.ProjectJobTitleId = projectEstimationIndexLabour.ProjectJobTitleId;
        //        entity.Unit = GetBasicSalary(projectEstimationIndexLabour.ProjectJobTitleId, projectEstimationIndexLabour.Unit);
        //        entity.BirrMonth = entity.Unit * projectEstimationIndexLabour.IndexFactory;
        //        entity.IdealBirrDay = entity.BirrMonth / 24;
        //        entity.ActualBirrDay = (entity.IdealBirrDay * 30) / 24;
        //        entity.IdealBirrDays = GetPerdimeAmount(entity.Unit);
        //        entity.ActualBirrDays = (entity.IdealBirrDays * 30) / 26;
        //        entity.DesertBirrDays = (DeseretAllowance / 100) * entity.ActualBirrDays;
        //        entity.NetBirrDays = entity.ActualBirrDay + entity.ActualBirrDays + entity.DesertBirrDays;
        //        entity.NetBirrHour = entity.NetBirrDays / 8;
        //        entity.IndexFactory = projectEstimationIndexLabour.IndexFactory;
        //        entity.FormHr = projectEstimationIndexLabour.FormHr;


        //        db.ProjectEstimationIndexLabour.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { projectEstimationIndexLabour }.ToDataSourceResult(request, ModelState));
        //}

        //private decimal GetPerdimeAmount(decimal salary)
        //{
        //    decimal perdimeAmount = 0;
        //    var perdimSeting = db.PerdimeSetting.Where(x => salary >= x.Minmum && salary <= x.Maximum).FirstOrDefault();
        //    if (perdimSeting != null)
        //    {
        //        return perdimSeting.Amount;
        //    }
        //    return 0;
        //}
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ProjectEstimationIndexLabour_Destroy([DataSourceRequest] DataSourceRequest request,
        //    ProjectEstimationIndexLabour projectEstimationIndexLabour)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new ProjectEstimationIndexLabour
        //        {
        //            ProjectEstimationIndexLabourId = projectEstimationIndexLabour.ProjectEstimationIndexLabourId,

        //            ProjectJobTitleId = projectEstimationIndexLabour.ProjectJobTitleId,
        //            Unit = projectEstimationIndexLabour.Unit,
        //            IndexFactory = projectEstimationIndexLabour.IndexFactory,
        //            BirrMonth = projectEstimationIndexLabour.BirrMonth,
        //            IdealBirrDay = projectEstimationIndexLabour.IdealBirrDay,
        //            ActualBirrDay = projectEstimationIndexLabour.ActualBirrDay,
        //            IdealBirrDays = projectEstimationIndexLabour.IdealBirrDays,
        //            ActualBirrDays = projectEstimationIndexLabour.ActualBirrDays,
        //            DesertBirrDays = projectEstimationIndexLabour.DesertBirrDays,
        //            NetBirrDays = projectEstimationIndexLabour.NetBirrDays,
        //            NetBirrHour = projectEstimationIndexLabour.NetBirrHour,
        //            FormHr = projectEstimationIndexLabour.FormHr,


        //        };

        //        db.ProjectEstimationIndexLabour.Attach(entity);
        //        db.ProjectEstimationIndexLabour.Remove(entity);
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { projectEstimationIndexLabour }.ToDataSourceResult(request, ModelState));
        //}

        //public decimal GetBasicSalary(int title, decimal salary)
        //{
        //    db = new ExceedDbContext();
        //    decimal SalaryAmount;
        //    if (title == 0) return SalaryAmount = salary;
        //    var struc = db.ProjectJobTitles.Where(
        //    x => x.ProjectJobTitleId == title).FirstOrDefault();
        //    if (struc != null)
        //    {
        //        SalaryAmount = struc.Salary;


        //        return SalaryAmount;
        //    }
        //    return 0;
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //public decimal GetAverageMonthlySalary(int jobTitleId)
        //{
        //    db = new ExceedDbContext();

        //    var currentJobTitle = db.ProjectJobTitles.Where(pjt => pjt.ProjectJobTitleId == jobTitleId).FirstOrDefault();
        //    decimal averageMonthlySalary = new decimal();
        //    if (currentJobTitle != null)
        //    {
        //        //var allSimilarTitles = db.ProjectJobTitles.Where(pjt => pjt.LookupId == currentJobTitle.LookupId).ToList();
        //        var allSimilarTitles = db.SalariesJobTitles.Where(pjt => pjt.LookupId == currentJobTitle.LookupId).ToList();
        //        if (allSimilarTitles.Count() > 0)
        //        {
        //            var salaries = new HashSet<decimal>(allSimilarTitles.Select(t => t.Salary));
        //            var min = salaries.Min();
        //            var max = salaries.Max();
        //            var av = (min + max) / 2;
        //            averageMonthlySalary = av;
        //        }


        //    }
        //    return averageMonthlySalary;
        //}
        //public decimal GetMinMonthlySalary(int jobTitleId)
        //{
        //    db = new ExceedDbContext();

        //    var currentJobTitle = db.ProjectJobTitles.Where(pjt => pjt.ProjectJobTitleId == jobTitleId).FirstOrDefault();
        //    decimal minMonthlySalary = new decimal();
        //    if (currentJobTitle != null)
        //    {
        //        var allSimilarTitles = db.ProjectJobTitles.Where(pjt => pjt.LookupId == currentJobTitle.LookupId).ToList();
        //        var salaries = new HashSet<decimal>(allSimilarTitles.Select(t => t.Salary));
        //        var min = salaries.Min();
        //        minMonthlySalary = min;

        //    }
        //    return minMonthlySalary;
        //}
        //public decimal GetMaxMonthlySalary(int jobTitleId)
        //{
        //    db = new ExceedDbContext();

        //    var currentJobTitle = db.ProjectJobTitles.Where(pjt => pjt.ProjectJobTitleId == jobTitleId).FirstOrDefault();
        //    decimal maxMonthlySalary = new decimal();
        //    if (currentJobTitle != null)
        //    {
        //        var allSimilarTitles = db.ProjectJobTitles.Where(pjt => pjt.LookupId == currentJobTitle.LookupId).ToList();
        //        var salaries = new HashSet<decimal>(allSimilarTitles.Select(t => t.Salary));
        //        var max = salaries.Max();
        //        maxMonthlySalary = max;

        //    }
        //    return maxMonthlySalary;
        //}
    }
}
