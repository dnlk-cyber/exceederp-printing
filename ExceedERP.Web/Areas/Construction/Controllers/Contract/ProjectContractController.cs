using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using ExceedERP.Core.Domain.Common;
using System.IO;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    [AuthorizeRoles(ConstructionConstants.ContractManagementUser)]
    public class ProjectContractController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var orderTypeList = db.VariationOrderType.ToList();
            ViewData["types"] = orderTypeList;

            var claimType = db.ContractClaimType.ToList();
            ViewData["claim_types"] = claimType;

            var itemCategory = db.ItemCategorys.Select(c => new { c.Name, c.ItemCategoryCode}).ToList();
            ViewData["ItemCategories"] = itemCategory;

            var items = db.InventoryItems.Select(i => new { i.Name, i.ItemCode}).ToList();
            ViewData["Items"] = items;

            var workCategories = db.WorkCategory.Select(wc => new { wc.Name, wc.ConfigurationId, wc.WorkCategoryId}).ToList();
            ViewData["WorkCategories"] = workCategories;

            #region Structure types concatnated with code/series value(for road projects)
            var projectTypeMapping = db.ProjectTypeMappings.Where(p => p.ProjectType == AvailableProjectTypes.Road);
            var ids = new HashSet<int>(projectTypeMapping.Select(p => p.ConfigurationId));
            var list = db.Configurations.Where(x => (ids.Contains(x.ConfigurationId)) && x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
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

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();


            var internalIds = new HashSet<Guid>(db.InternalMachinaryRent.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(db.MachinaryRent.Select(ee => ee.FleetsID));
            var combinedEquipments =
                db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)))
                .Select(eq => new { eq.FleetsID, eq.EquipmentName })
                .ToList();
            ViewData["Equipment"] = combinedEquipments;

            this.UpdateTimeElapsedToDate();
            return View();
        }
        public ActionResult ProjectInformation_Read([DataSourceRequest] DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));
            //IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.STARTED || x.ProjectStatus == ProjectStatus.APPROVED);
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(p => !p.IsDeleted && ids.Contains(p.ProjectInformationId));
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new
            {
                ProjectInformationId = projectInformation.ProjectInformationId,
                ProjectType = projectInformation.ProjectType,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
                ProjectStatus = projectInformation.ProjectStatus
            });

            return Json(result);
        }

        public ActionResult ProjectContract_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            var sub = db.SubContrat.ToList();
            var allContracts = db.ProjectContract.Where(x => x.ProjectInformationId == id);
            var ids = new HashSet<int>(sub.Select(s => s.ProjectContractId));
            var mainContrats = allContracts.Where(main => !ids.Contains(main.ProjectContractId));

            DataSourceResult result = mainContrats.ToDataSourceResult(request, projectContract => new
            {
                ProjectContractId = projectContract.ProjectContractId,
                ContractSigningDate = projectContract.ContractSigningDate,
                CommencementDate = projectContract.CommencementDate,
                // project amount without any rebate, variation and supplemetary
                ContractAmount = projectContract.ContractAmount,
                // completion date without any time extensions
                ContractTime = projectContract.ContractTime,
                ProvisionalSum = projectContract.ProvisionalSum,

                SiteHandoverTime = projectContract.SiteHandoverTime,
                ContractPeriod = projectContract.ContractPeriod,
                ApprovedTimeExtension = projectContract.ApprovedTimeExtension,
                TotalProjectPeriod = projectContract.TotalProjectPeriod,
                RevisedCompletionDate = projectContract.RevisedCompletionDate,
                TimeElapsedToDate = projectContract.TimeElapsedToDate,
                Rebate = projectContract.Rebate,
                Variation = projectContract.Variation,
                Supplemetary = projectContract.Supplemetary,
                TotalProjectAmount = projectContract.TotalProjectAmount,
                NetGrand = projectContract.NetGrand,
                ExecutedToDate = projectContract.ExecutedToDate,

                ProjectInformationId = projectContract.ProjectInformationId,
                OrgBranchName = projectContract.OrgBranchName,
                DateCreated = projectContract.DateCreated,
                LastModified = projectContract.LastModified,
                CreatedBy = projectContract.CreatedBy,
                ModifiedBy = projectContract.ModifiedBy
            });

            return Json(result);
        }

        private bool ProjectHasContract(int projectId)
        {
            bool hasContract = false;
            var currentProject = db.ProjectInformation.Find(projectId);
            if (currentProject != null)
            {
                var sub = db.SubContrat.ToList();
                var projectcontract = db.ProjectContract.Where(c => c.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var ids = new HashSet<int>(sub.Select(s => s.ProjectContractId));
                var mainContrats = projectcontract.Where(main => !ids.Contains(main.ProjectContractId));
                var temp = mainContrats.FirstOrDefault();

                if (temp != null)
                {
                    hasContract = true;
                }
            }
            return hasContract;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectContract_Create([DataSourceRequest]DataSourceRequest request, ProjectContract projectContract, int idTwo)
        {
            if (ModelState.IsValid)
            {
                if (ProjectHasContract(idTwo))
                {
                    ModelState.AddModelError("", "Multiple main contracts cannot be used for one project!!!");
                }
                else
                {
                    var entity = new ProjectContract
                    {
                        ProjectContractId = projectContract.ProjectContractId,
                        ContractSigningDate = projectContract.ContractSigningDate,
                        CommencementDate = projectContract.CommencementDate,
                        ContractAmount = projectContract.ContractAmount,
                        ContractTime = projectContract.ContractTime,
                        ProvisionalSum = projectContract.ProvisionalSum,

                        SiteHandoverTime = projectContract.SiteHandoverTime,
                        ContractPeriod = projectContract.ContractPeriod,
                        ApprovedTimeExtension = projectContract.ApprovedTimeExtension,
                        RevisedCompletionDate = projectContract.RevisedCompletionDate,
                        TimeElapsedToDate = projectContract.TimeElapsedToDate,
                        Rebate = projectContract.Rebate,
                        Variation = projectContract.Variation,
                        Supplemetary = projectContract.Supplemetary,
                        ExecutedToDate = projectContract.ExecutedToDate,


                        ProjectInformationId = idTwo,
                        OrgBranchName = projectContract.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectContract.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectContract.ModifiedBy
                    };
                    entity.TotalProjectPeriod = entity.ApprovedTimeExtension == null ? entity.ContractPeriod : entity.ContractPeriod + entity.ApprovedTimeExtension;
                    entity.ContractTime = entity.CommencementDate.AddDays((double)entity.ContractPeriod);
                    entity.RevisedCompletionDate = entity.CommencementDate.AddDays(((double)entity.TotalProjectPeriod));
                    if(entity.Variation == null && entity.Supplemetary == null)
                    {
                        entity.TotalProjectAmount = entity.ContractAmount;
                    }
                    else if(entity.Variation != null && entity.Supplemetary  == null)
                    {
                        entity.TotalProjectAmount = entity.ContractAmount + entity.Variation;
                    }
                    else if (entity.Supplemetary != null && entity.Variation == null) {
                        entity.TotalProjectAmount = entity.ContractAmount + entity.Supplemetary;
                    }
                    else
                    {
                        entity.TotalProjectAmount = entity.ContractAmount + entity.Variation + entity.Supplemetary;
                    }
                    entity.NetGrand = entity.TotalProjectAmount - entity.ContractAmount * (entity.Rebate / 100);


                    db.ProjectContract.Add(entity);
                    db.SaveChanges();

                    projectContract.ProjectContractId = entity.ProjectContractId;
                    projectContract.NetGrand = entity.NetGrand;
                    projectContract.TotalProjectPeriod = entity.TotalProjectPeriod;
                    projectContract.TotalProjectAmount = entity.TotalProjectAmount;
                    projectContract.ContractTime = entity.ContractTime;
                    projectContract.RevisedCompletionDate = entity.RevisedCompletionDate;

                    ContractMgmtHelper.UpdateProjectAmountAccordingToContract(entity.ProjectInformationId, entity.ContractAmount);
                    ContractMgmtHelper.UpdateContract(entity.ProjectContractId);
                }
            }

            return Json(new[] { projectContract }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectContract_Update([DataSourceRequest]DataSourceRequest request, ProjectContract projectContract)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectContract
                {
                    ProjectContractId = projectContract.ProjectContractId,
                    ContractSigningDate = projectContract.ContractSigningDate,
                    CommencementDate = projectContract.CommencementDate,
                    ContractAmount = projectContract.ContractAmount,
                    ContractTime = projectContract.ContractTime,
                    ProvisionalSum = projectContract.ProvisionalSum,

                    SiteHandoverTime = projectContract.SiteHandoverTime,
                    ContractPeriod = projectContract.ContractPeriod,
                    ApprovedTimeExtension = projectContract.ApprovedTimeExtension,
                    RevisedCompletionDate = projectContract.RevisedCompletionDate,
                    TimeElapsedToDate = projectContract.TimeElapsedToDate,
                    Rebate = projectContract.Rebate,
                    Variation = projectContract.Variation,
                    Supplemetary = projectContract.Supplemetary,
                    ExecutedToDate = projectContract.ExecutedToDate,

                    ProjectInformationId = projectContract.ProjectInformationId,
                    OrgBranchName = projectContract.OrgBranchName,
                    DateCreated = projectContract.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectContract.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.TotalProjectPeriod = entity.ApprovedTimeExtension == null ? entity.ContractPeriod : entity.ContractPeriod + entity.ApprovedTimeExtension;
                entity.ContractTime = entity.CommencementDate.AddDays((double)entity.ContractPeriod);
                entity.RevisedCompletionDate = entity.CommencementDate.AddDays(((double)entity.TotalProjectPeriod));

                if (entity.Variation == null && entity.Supplemetary == null)
                {
                    entity.TotalProjectAmount = entity.ContractAmount;
                }
                else if (entity.Variation != null && entity.Supplemetary == null)
                {
                    entity.TotalProjectAmount = entity.ContractAmount + entity.Variation;
                }
                else if (entity.Supplemetary != null && entity.Variation == null)
                {
                    entity.TotalProjectAmount = entity.ContractAmount + entity.Supplemetary;
                }
                else
                {
                    entity.TotalProjectAmount = entity.ContractAmount + entity.Variation + entity.Supplemetary;
                }

                entity.NetGrand = entity.TotalProjectAmount - entity.ContractAmount * (entity.Rebate / 100);

                db.ProjectContract.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                projectContract.ProjectContractId = entity.ProjectContractId;
                projectContract.NetGrand = entity.NetGrand;
                projectContract.TotalProjectPeriod = entity.TotalProjectPeriod;
                projectContract.TotalProjectAmount = entity.TotalProjectAmount;
                projectContract.ContractTime = entity.ContractTime;
                projectContract.RevisedCompletionDate = entity.RevisedCompletionDate;

                ContractMgmtHelper.UpdateProjectAmountAccordingToContract(entity.ProjectInformationId, entity.ContractAmount);
            }

            return Json(new[] { projectContract }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectContract_Destroy([DataSourceRequest]DataSourceRequest request, ProjectContract projectContract)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectContract
                {
                    ProjectContractId = projectContract.ProjectContractId,
                    ContractSigningDate = projectContract.ContractSigningDate,
                    CommencementDate = projectContract.CommencementDate,
                    ContractAmount = projectContract.ContractAmount,
                    ContractTime = projectContract.ContractTime,
                    ProvisionalSum = projectContract.ProvisionalSum,

                    SiteHandoverTime = projectContract.SiteHandoverTime,
                    ContractPeriod = projectContract.ContractPeriod,
                    ApprovedTimeExtension = projectContract.ApprovedTimeExtension,
                    TotalProjectPeriod = projectContract.TotalProjectPeriod,
                    RevisedCompletionDate = projectContract.RevisedCompletionDate,
                    TimeElapsedToDate = projectContract.TimeElapsedToDate,
                    Rebate = projectContract.Rebate,
                    Variation = projectContract.Variation,
                    Supplemetary = projectContract.Supplemetary,
                    TotalProjectAmount = projectContract.TotalProjectAmount,
                    NetGrand = projectContract.NetGrand,
                    ExecutedToDate = projectContract.ExecutedToDate,

                    ProjectInformationId = projectContract.ProjectInformationId,
                    OrgBranchName = projectContract.OrgBranchName,
                    DateCreated = projectContract.DateCreated,
                    LastModified = projectContract.LastModified,
                    CreatedBy = projectContract.CreatedBy,
                    ModifiedBy = projectContract.ModifiedBy
                };

                db.ProjectContract.Attach(entity);
                db.ProjectContract.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectContract }.ToDataSourceResult(request, ModelState));
        }

        public void Download(int id)
        {
            DataProviderController.SelectedFile = id;

        }
        public ActionResult DownloadDoument(string filename)
        {

            if (DataProviderController.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.PROJECT_DOCUMENT && x.Remark == DataProviderController.Owner && x.AttachmentId == DataProviderController.SelectedFile)
                            .FirstOrDefault();
                if (attachment != null)
                {
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.FileName + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + DataProviderController.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    DataProviderController.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
            DataProviderController.SelectedFile = 0;
            return null;
        }
        public ActionResult UploadDocument(HttpPostedFileBase file, string url, string txtValue, string txtDecsion, string id)
        {
            try
            {

                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + DataProviderController.EmployeeId.ToString() + "-" +
                                           AttachmentType.PROJECT_DOCUMENT), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + DataProviderController.EmployeeId.ToString() + "-" +
                                           AttachmentType.PROJECT_DOCUMENT));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {


                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = DataProviderController.EmployeeId.ToString(),
                            FileName = file.FileName,
                            Description = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.PROJECT_DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = DataProviderController.EmployeeId.ToString()
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "This file exists";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment
                {
                    AttachmentId = attachment.AttachmentId,
                    ReferenceType = attachment.ReferenceType,
                    Reference = attachment.Reference,
                    Description = attachment.Description,
                    URL = attachment.URL,
                    BinaryFile = attachment.BinaryFile,
                    FileType = attachment.FileType,
                    Index = attachment.Index,
                    Remark = attachment.Remark
                };

                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult UpdatePriceAdjustmentWeights(int projectId) {
            var currentProject = db.ProjectInformation.Find(projectId);
            if(currentProject != null)
            {
                var cat = (from c in db.WorkCategory
                           join w in db.GenericWorkItemPerProject
                           on c.WorkCategoryId equals w.WorkCategoryId
                           where w.ProjectInformationId == currentProject.ProjectInformationId
                           select new
                           {
                               Id = c.WorkCategoryId
                           }).ToList();
                var ids = new HashSet<int>(cat.Select(c => c.Id));
                var categoryUnderProject = db.WorkCategory.Where(category => (ids.Contains(category.WorkCategoryId))).ToList();
                var representativeElements = db.ProjectRepresentativeElements.Where(re => re.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                if (categoryUnderProject.Any())
                {
                    foreach (WorkCategory category in categoryUnderProject)
                    {
                        var workItemsInCategory = db.GenericWorkItemPerProject.Where(w => w.WorkCategoryId == category.WorkCategoryId && w.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                        var nonAdjustableWorkItems = workItemsInCategory.Where(wi => wi.Remark == "PP" || wi.Remark == "PS" || wi.Remark == "LS" || wi.Remark == "CC").ToList();
                        //var materialsInCategory = 

                        var categoryCost = workItemsInCategory.Sum(wi => wi.TotalQuantityOfWork * wi.TotalUnitCost);
                        var deductiveCost = nonAdjustableWorkItems.Sum(wi => wi.TotalQuantityOfWork * wi.TotalUnitCost);
                        var priceAdjWeightToAdd = new PriceAdjustmentWeight
                        {
                            ProjectInformationId = currentProject.ProjectInformationId,
                            WorkCategoryId = category.WorkCategoryId,
                            ContractAmount = categoryCost,
                            ActualAmount = categoryCost - deductiveCost
                        };
                        // check for existence and add if it doesn't exist
                        var temp = db.PriceAdjustmentWeights.FirstOrDefault(paw => paw.WorkCategoryId == priceAdjWeightToAdd.WorkCategoryId && paw.ProjectInformationId == currentProject.ProjectInformationId);
                        if (temp == null)
                        {
                            db.PriceAdjustmentWeights.Add(priceAdjWeightToAdd);
                            db.SaveChanges();
                        }
                        // modify if it exists
                        else
                        {
                            temp.ContractAmount = categoryCost;
                            temp.ActualAmount = categoryCost - deductiveCost;

                            db.PriceAdjustmentWeights.Attach(temp);
                            db.Entry(temp).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        var weight = db.PriceAdjustmentWeights.Where(p => p.WorkCategoryId == category.WorkCategoryId && p.ProjectInformationId == currentProject.ProjectInformationId).FirstOrDefault();

                        foreach (ProjectRepresentativeElement representativeElement in representativeElements)
                        {
                            #region material
                            if (representativeElement.IndexType == RepresentativeElementIndexType.Material)
                            {
                                var materials = db.GenericProjectMaterial.Where(m =>
                                                                m.WorkItem.WorkCategoryId == category.WorkCategoryId &&
                                                                m.WorkItem.ProjectInformationId == currentProject.ProjectInformationId &&
                                                                m.ItemCode == representativeElement.ItemCode
                                                                ).ToList();
                                var costOfSpecificMaterialInCategory = materials.Sum(m => m.TotalCost);
                                // check for existence
                                var existingMaterial = db.PriceAdjustmentWeightItems.FirstOrDefault(wi => wi.ProjectRepresentativeElementId == representativeElement.ProjectRepresentativeElementId && wi.PriceAdjustmentWeightId == weight.PriceAdjustmentWeightId);
                                if (existingMaterial == null && weight != null)
                                {
                                    var itemToAdd = new PriceAdjustmentWeightItem
                                    {
                                        ItemCode = representativeElement.ItemCode,
                                        FleetsID = representativeElement.FleetsID,
                                        ProjectEstimationIndexLabourId = representativeElement.ProjectEstimationIndexLabourId,
                                        PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                                        ProjectRepresentativeElementId = representativeElement.ProjectRepresentativeElementId,
                                        ItemName = representativeElement.Description + "( " + representativeElement.IndexCode + " )",
                                        Amount = costOfSpecificMaterialInCategory,
                                        ApprovedAmount = costOfSpecificMaterialInCategory,

                                    };
                                    db.PriceAdjustmentWeightItems.Add(itemToAdd);
                                    db.SaveChanges();
                                }
                                else if (existingMaterial != null && weight != null)
                                {
                                    existingMaterial.Amount = costOfSpecificMaterialInCategory;

                                    db.PriceAdjustmentWeightItems.Attach(existingMaterial);
                                    db.Entry(existingMaterial).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            #endregion
                            #region equipment
                            else if (representativeElement.IndexType == RepresentativeElementIndexType.Equipment)
                            {
                                var equipments = db.GenericProjectEquipment.Where(e =>
                                                               e.WorkItem.WorkCategoryId == category.WorkCategoryId &&
                                                               e.WorkItem.ProjectInformationId == currentProject.ProjectInformationId &&
                                                               e.FleetsID == representativeElement.FleetsID
                                                               ).ToList();
                                var costOfSpecificEquipmentInCategory = equipments.Sum(m => m.TotalCost);
                                // check for existence
                                var existingEquipmentRepresentative = db.PriceAdjustmentWeightItems.FirstOrDefault(wi => wi.ProjectRepresentativeElementId == representativeElement.ProjectRepresentativeElementId && wi.PriceAdjustmentWeightId == weight.PriceAdjustmentWeightId);
                                if (existingEquipmentRepresentative == null && weight != null)
                                {
                                    var itemToAdd = new PriceAdjustmentWeightItem
                                    {
                                        ItemCode = representativeElement.ItemCode,
                                        FleetsID = representativeElement.FleetsID,
                                        ProjectEstimationIndexLabourId = representativeElement.ProjectEstimationIndexLabourId,
                                        PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                                        ProjectRepresentativeElementId = representativeElement.ProjectRepresentativeElementId,
                                        ItemName = representativeElement.Description + "( " + representativeElement.IndexCode + " )",
                                        Amount = costOfSpecificEquipmentInCategory,
                                        ApprovedAmount = costOfSpecificEquipmentInCategory,

                                    };
                                    db.PriceAdjustmentWeightItems.Add(itemToAdd);
                                    db.SaveChanges();
                                }
                                else if (existingEquipmentRepresentative != null && weight != null)
                                {
                                    existingEquipmentRepresentative.Amount = costOfSpecificEquipmentInCategory;

                                    db.PriceAdjustmentWeightItems.Attach(existingEquipmentRepresentative);
                                    db.Entry(existingEquipmentRepresentative).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            #endregion
                            #region labor
                            else if (representativeElement.IndexType == RepresentativeElementIndexType.Labor)
                            {
                                var labor = db.GenericProjectDirectLabour.Where(l =>
                                                               l.WorkItem.WorkCategoryId == category.WorkCategoryId &&
                                                               l.WorkItem.ProjectInformationId == currentProject.ProjectInformationId &&
                                                               l.ProjectEstimationIndexLabourId == representativeElement.ProjectEstimationIndexLabourId
                                                               ).ToList();
                                var costOfSpecificLaborInCategory = labor.Sum(m => m.TotalCost);

                                // check for existence
                                var existingLaborRepresentative = db.PriceAdjustmentWeightItems.FirstOrDefault(wi => wi.ProjectRepresentativeElementId == representativeElement.ProjectRepresentativeElementId && wi.PriceAdjustmentWeightId == weight.PriceAdjustmentWeightId);
                                if (existingLaborRepresentative == null && weight != null)
                                {
                                    var itemToAdd = new PriceAdjustmentWeightItem
                                    {
                                        ItemCode = representativeElement.ItemCode,
                                        FleetsID = representativeElement.FleetsID,
                                        ProjectEstimationIndexLabourId = representativeElement.ProjectEstimationIndexLabourId,
                                        PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                                        ProjectRepresentativeElementId = representativeElement.ProjectRepresentativeElementId,
                                        ItemName = representativeElement.Description + "( " + representativeElement.IndexCode + " )",
                                        Amount = costOfSpecificLaborInCategory,
                                        ApprovedAmount = costOfSpecificLaborInCategory,

                                    };
                                    db.PriceAdjustmentWeightItems.Add(itemToAdd);
                                    db.SaveChanges();
                                }
                                else if (existingLaborRepresentative != null && weight != null)
                                {
                                    existingLaborRepresentative.Amount = costOfSpecificLaborInCategory;

                                    db.PriceAdjustmentWeightItems.Attach(existingLaborRepresentative);
                                    db.Entry(existingLaborRepresentative).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                            #endregion
                            #region other
                            else
                            {
                                var existingRepresentative = db.PriceAdjustmentWeightItems.FirstOrDefault(wi => wi.ProjectRepresentativeElementId == representativeElement.ProjectRepresentativeElementId && wi.PriceAdjustmentWeightId == weight.PriceAdjustmentWeightId);
                                if (existingRepresentative == null)
                                {
                                    var itemToAdd = new PriceAdjustmentWeightItem
                                    {
                                        ItemCode = representativeElement.ItemCode,
                                        FleetsID = representativeElement.FleetsID,
                                        ProjectEstimationIndexLabourId = representativeElement.ProjectEstimationIndexLabourId,
                                        PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                                        ProjectRepresentativeElementId = representativeElement.ProjectRepresentativeElementId,
                                        ItemName = representativeElement.Description + "( " + representativeElement.IndexCode + " )",

                                    };
                                    db.PriceAdjustmentWeightItems.Add(itemToAdd);
                                    db.SaveChanges();
                                }
                                else
                                {

                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            return Redirect("Index");
        }
        private void CreatePriceAdjustmentWeights()
        {
            //var projects = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.STARTED || x.ProjectStatus == ProjectStatus.APPROVED).ToList();
            var projects = db.ProjectInformation.Where(p => p.ProjectType == AvailableProjectTypes.Building).ToList();
            if (projects.Any())
            {
                foreach (ProjectInformation project in projects)
                {
                    var cat = (from c in db.WorkCategory
                               join w in db.GenericWorkItemPerProject
                               on c.WorkCategoryId equals w.WorkCategoryId
                               where w.ProjectInformationId == project.ProjectInformationId
                               select new
                               {
                                   Id = c.WorkCategoryId
                               }).ToList();
                    var ids = new HashSet<int>(cat.Select(c => c.Id));
                    var categoryUnderProject = db.WorkCategory.Where(category => (ids.Contains(category.WorkCategoryId))).ToList();
                    var representativeElements = db.ProjectRepresentativeElements.Where(re => re.ProjectInformationId == project.ProjectInformationId).ToList();

                    if (categoryUnderProject.Any()) {
                        foreach(WorkCategory category in categoryUnderProject)
                        {
                            var workItemsInCategory = db.GenericWorkItemPerProject.Where(w => w.WorkCategoryId == category.WorkCategoryId && w.ProjectInformationId == project.ProjectInformationId).ToList();
                            var nonAdjustableWorkItems = workItemsInCategory.Where(wi => wi.Remark == "PP" || wi.Remark == "PS" || wi.Remark == "LS" || wi.Remark == "CC").ToList();
                            //var materialsInCategory = 

                            var categoryCost = workItemsInCategory.Sum(wi => wi.TotalQuantityOfWork * wi.TotalUnitCost);
                            var diductiveCost = nonAdjustableWorkItems.Sum(wi => wi.TotalQuantityOfWork * wi.TotalUnitCost);
                            var priceAdjWeightToAdd = new PriceAdjustmentWeight
                            {
                                ProjectInformationId = project.ProjectInformationId,
                                WorkCategoryId = category.WorkCategoryId,
                                ContractAmount = categoryCost,
                                ActualAmount = categoryCost - diductiveCost
                            };
                            // check for existence and add if it doesn't exist
                            var temp = db.PriceAdjustmentWeights.FirstOrDefault(paw => paw.WorkCategoryId == priceAdjWeightToAdd.WorkCategoryId && paw.ProjectInformationId == project.ProjectInformationId);
                            if(temp == null)
                            {
                                db.PriceAdjustmentWeights.Add(priceAdjWeightToAdd);
                                db.SaveChanges();
                            }

                            var weight = db.PriceAdjustmentWeights.Where(p => p.WorkCategoryId == category.WorkCategoryId && p.ProjectInformationId == project.ProjectInformationId).FirstOrDefault();
                            
                            foreach (ProjectRepresentativeElement representativeElement in representativeElements)
                            {
                                #region material
                                if (representativeElement.IndexType == RepresentativeElementIndexType.Material)
                                {
                                    var materials = db.GenericProjectMaterial.Where(m =>
                                                                    m.WorkItem.WorkCategoryId == category.WorkCategoryId &&
                                                                    m.WorkItem.ProjectInformationId == project.ProjectInformationId &&
                                                                    m.ItemCode == representativeElement.ItemCode
                                                                    ).ToList();
                                    var costOfSpecificMaterialInCategory = materials.Sum(m => m.TotalCost);
                                    // check for existence
                                    var existingMaterial = db.PriceAdjustmentWeightItems.FirstOrDefault(wi => wi.ProjectRepresentativeElementId == representativeElement.ProjectRepresentativeElementId && wi.PriceAdjustmentWeightId == weight.PriceAdjustmentWeightId);
                                    if (existingMaterial == null && weight != null)
                                    {
                                        var itemToAdd = new PriceAdjustmentWeightItem
                                        {
                                            ItemCode = representativeElement.ItemCode,
                                            FleetsID = representativeElement.FleetsID,
                                            ProjectEstimationIndexLabourId = representativeElement.ProjectEstimationIndexLabourId,
                                            PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                                            ProjectRepresentativeElementId = representativeElement.ProjectRepresentativeElementId,
                                            ItemName = representativeElement.Description + "( " + representativeElement.IndexCode + " )",
                                            Amount = costOfSpecificMaterialInCategory,
                                            ApprovedAmount = costOfSpecificMaterialInCategory,

                                        };
                                        db.PriceAdjustmentWeightItems.Add(itemToAdd);
                                        db.SaveChanges();
                                    }
                                    else if (existingMaterial != null && weight != null)
                                    {
                                        existingMaterial.Amount = costOfSpecificMaterialInCategory;
                                        db.PriceAdjustmentWeightItems.Attach(existingMaterial);
                                        db.Entry(existingMaterial).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                #endregion
                                #region equipment
                                else if (representativeElement.IndexType == RepresentativeElementIndexType.Equipment)
                                {
                                    var equipments = db.GenericProjectEquipment.Where(e =>
                                                                   e.WorkItem.WorkCategoryId == category.WorkCategoryId &&
                                                                   e.WorkItem.ProjectInformationId == project.ProjectInformationId &&
                                                                   e.FleetsID == representativeElement.FleetsID
                                                                   ).ToList();
                                    var costOfSpecificEquipmentInCategory = equipments.Sum(m => m.TotalCost);
                                    // check for existence
                                    var existingEquipmentRepresentative = db.PriceAdjustmentWeightItems.FirstOrDefault(wi => wi.ProjectRepresentativeElementId == representativeElement.ProjectRepresentativeElementId && wi.PriceAdjustmentWeightId == weight.PriceAdjustmentWeightId);
                                    if (existingEquipmentRepresentative == null && weight != null)
                                    {
                                        var itemToAdd = new PriceAdjustmentWeightItem
                                        {
                                            ItemCode = representativeElement.ItemCode,
                                            FleetsID = representativeElement.FleetsID,
                                            ProjectEstimationIndexLabourId = representativeElement.ProjectEstimationIndexLabourId,
                                            PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                                            ProjectRepresentativeElementId = representativeElement.ProjectRepresentativeElementId,
                                            ItemName = representativeElement.Description + "( " + representativeElement.IndexCode + " )",
                                            Amount = costOfSpecificEquipmentInCategory,
                                            ApprovedAmount = costOfSpecificEquipmentInCategory,

                                        };
                                        db.PriceAdjustmentWeightItems.Add(itemToAdd);
                                        db.SaveChanges();
                                    }
                                    else if (existingEquipmentRepresentative != null && weight != null)
                                    {
                                        existingEquipmentRepresentative.Amount = costOfSpecificEquipmentInCategory;

                                        db.PriceAdjustmentWeightItems.Attach(existingEquipmentRepresentative);
                                        db.Entry(existingEquipmentRepresentative).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                #endregion
                                #region labor
                                else if (representativeElement.IndexType == RepresentativeElementIndexType.Labor)
                                {
                                    var labor = db.GenericProjectDirectLabour.Where(l =>
                                                                   l.WorkItem.WorkCategoryId == category.WorkCategoryId &&
                                                                   l.WorkItem.ProjectInformationId == project.ProjectInformationId &&
                                                                   l.ProjectEstimationIndexLabourId == representativeElement.ProjectEstimationIndexLabourId
                                                                   ).ToList();
                                    var costOfSpecificLaborInCategory = labor.Sum(m => m.TotalCost);

                                    // check for existence
                                    var existingLaborRepresentative = db.PriceAdjustmentWeightItems.FirstOrDefault(wi => wi.ProjectRepresentativeElementId == representativeElement.ProjectRepresentativeElementId && wi.PriceAdjustmentWeightId == weight.PriceAdjustmentWeightId);
                                    if (existingLaborRepresentative == null && weight != null)
                                    {
                                        var itemToAdd = new PriceAdjustmentWeightItem
                                        {
                                            ItemCode = representativeElement.ItemCode,
                                            FleetsID = representativeElement.FleetsID,
                                            ProjectEstimationIndexLabourId = representativeElement.ProjectEstimationIndexLabourId,
                                            PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                                            ProjectRepresentativeElementId = representativeElement.ProjectRepresentativeElementId,
                                            ItemName = representativeElement.Description + "( " + representativeElement.IndexCode + " )",
                                            Amount = costOfSpecificLaborInCategory,
                                            ApprovedAmount = costOfSpecificLaborInCategory,

                                        };
                                        db.PriceAdjustmentWeightItems.Add(itemToAdd);
                                        db.SaveChanges();
                                    }
                                    else if (existingLaborRepresentative != null && weight != null)
                                    {
                                        existingLaborRepresentative.Amount = costOfSpecificLaborInCategory;

                                        db.PriceAdjustmentWeightItems.Attach(existingLaborRepresentative);
                                        db.Entry(existingLaborRepresentative).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                #endregion
                                #region other
                                else
                                {
                                    var existingRepresentative = db.PriceAdjustmentWeightItems.FirstOrDefault(wi => wi.ProjectRepresentativeElementId == representativeElement.ProjectRepresentativeElementId && wi.PriceAdjustmentWeightId == weight.PriceAdjustmentWeightId);
                                    if(existingRepresentative == null)
                                    {
                                        var itemToAdd = new PriceAdjustmentWeightItem
                                        {
                                            ItemCode = representativeElement.ItemCode,
                                            FleetsID = representativeElement.FleetsID,
                                            ProjectEstimationIndexLabourId = representativeElement.ProjectEstimationIndexLabourId,
                                            PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                                            ProjectRepresentativeElementId = representativeElement.ProjectRepresentativeElementId,
                                            ItemName = representativeElement.Description + "( " + representativeElement.IndexCode + " )",

                                        };
                                        db.PriceAdjustmentWeightItems.Add(itemToAdd);
                                        db.SaveChanges();
                                    }
                                    else
                                    {

                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
        }
        private void UpdateSiteHandOverTime()
        {
            var contracts = db.ProjectContract.ToList();
            if (contracts.Any())
            {
                foreach (ProjectContract contract in contracts) {
                    if(contract.SiteHandoverTime < contract.ContractSigningDate)
                    {
                        contract.SiteHandoverTime = contract.ContractSigningDate;

                        db.ProjectContract.Attach(contract);
                        db.Entry(contract).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        private  void UpdateTimeElapsedToDate()
        {
            var sub = db.SubContrat.ToList();
            var allContracts = db.ProjectContract;
            var ids = new HashSet<int>(sub.Select(s => s.ProjectContractId));
            var mainContrats = allContracts.Where(main => !ids.Contains(main.ProjectContractId)).ToList();

            if (mainContrats.Any())
            {
                foreach (ProjectContract contract in mainContrats)
                {
                    var difference = DateTime.Now - contract.CommencementDate;
                    var days = (decimal)difference.Days + 1;
                    if (days > contract.TotalProjectPeriod)
                    {
                        contract.TimeElapsedToDate = contract.TotalProjectPeriod;
                    }
                    else if (days < 0)
                    {
                        contract.TimeElapsedToDate = 0;
                    }
                    else
                    {
                        contract.TimeElapsedToDate = days;
                    }

                    db.ProjectContract.Attach(contract);
                    db.Entry(contract).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
