using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Core.Domain.Manufacturing.Setting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureBillOfQuantityController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobType"] = db.FurnitureStandardJobTypes.Select(s => new
            {
                Value = s.FurnitureStandardJobTypeId,
                Text = s.JobTypeName + "/" + s.JobCode
            }).ToList();
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();

            ViewData["ItemCode"] = db.InventoryItems.Select(s => new
            {
                Value = s.ItemCode,
                Text = s.Name
            }).ToList();

            ViewData["MaterialCategory"] = db.ManufacturingMaterialCategories.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryId,
                Text = s.ManufacturingCategory
            }).ToList();

            ViewData["MaterialCategoryItem"] = db.ManufacturingMaterialCategoryItems.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();

            ViewData["TaskCategory"] = db.ManifucturingTaskCategories.Select(s => new
            {
                Value = s.ManifucturingTaskCategoryId,
                Text = s.Name
            });
            return View();
        }

        public ActionResult FurnitureBillOfQuantities_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureBillOfQuantity> furniturebillofquantities = db.FurnitureBillOfQuantities;
            DataSourceResult result = furniturebillofquantities.ToDataSourceResult(request, furnitureBillOfQuantity => new FurnitureBillOfQuantity
            {
                FurnitureBillOfQuantityId = furnitureBillOfQuantity.FurnitureBillOfQuantityId,
                JobTypeId = furnitureBillOfQuantity.JobTypeId,
                CustomerId = furnitureBillOfQuantity.CustomerId,
                Quantity = furnitureBillOfQuantity.Quantity,
                Size = furnitureBillOfQuantity.Size,
                Status = furnitureBillOfQuantity.Status,
                Remark = furnitureBillOfQuantity.Remark,
                IsVoid = furnitureBillOfQuantity.IsVoid,
                VoidBy = furnitureBillOfQuantity.VoidBy,
                VoidTime = furnitureBillOfQuantity.VoidTime,
                IsOnlineApproved = furnitureBillOfQuantity.IsOnlineApproved,
                OnlineApprovedBy = furnitureBillOfQuantity.OnlineApprovedBy,
                OnlineApprovedTime = furnitureBillOfQuantity.OnlineApprovedTime,
                IsOnlineTransferred = furnitureBillOfQuantity.IsOnlineTransferred,
                IsOnlineTransferredBy = furnitureBillOfQuantity.IsOnlineTransferredBy,
                OnlineTransferredTime = furnitureBillOfQuantity.OnlineTransferredTime,
                IsSendForApproval = furnitureBillOfQuantity.IsSendForApproval,
                OrgBranchName = furnitureBillOfQuantity.OrgBranchName,
                SendForApprovalBy = furnitureBillOfQuantity.SendForApprovalBy,
                SendForApprovalTime = furnitureBillOfQuantity.SendForApprovalTime,
                DateCreated = furnitureBillOfQuantity.DateCreated,
                LastModified = furnitureBillOfQuantity.LastModified,
                CreatedBy = furnitureBillOfQuantity.CreatedBy,
                ModifiedBy = furnitureBillOfQuantity.ModifiedBy,
                JobTypeCategory = furnitureBillOfQuantity.JobTypeCategory,
                IsCustomizedJob = furnitureBillOfQuantity.IsCustomizedJob,
                JobNo = furnitureBillOfQuantity.JobNo,
                JobTypeName = furnitureBillOfQuantity.JobTypeName,
                Unit = furnitureBillOfQuantity.Unit,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBillOfQuantities_Create([DataSourceRequest]DataSourceRequest request, FurnitureBillOfQuantity furnitureBillOfQuantity)
        {

            if (ModelState.IsValid)
            {
                if (furnitureBillOfQuantity.JobTypeCategory == JobTypeCategory.Custom_Made)
                {
                    var entity = new FurnitureBillOfQuantity
                    {
                        JobTypeCategory = furnitureBillOfQuantity.JobTypeCategory,
                        JobTypeId = 0,
                        JobTypeName = furnitureBillOfQuantity.JobTypeName,
                        JobNo = furnitureBillOfQuantity.JobNo,
                        Unit = furnitureBillOfQuantity.Unit,

                        CustomerId = furnitureBillOfQuantity.CustomerId,
                        Quantity = furnitureBillOfQuantity.Quantity,
                        Size = furnitureBillOfQuantity.Size,
                        Status = "Pending",
                        Remark = furnitureBillOfQuantity.Remark,
                        DateCreated = DateTime.Today,
                        CreatedBy = User.Identity.Name,
                        IsCustomizedJob = false,
                    };

                    db.FurnitureBillOfQuantities.Add(entity);
                    db.SaveChanges();
                    furnitureBillOfQuantity.FurnitureBillOfQuantityId = entity.FurnitureBillOfQuantityId;
                }
                if (furnitureBillOfQuantity.JobTypeCategory == JobTypeCategory.Standard)
                {
                    var check = CheckBillOfMaterialAndLaborExistence(furnitureBillOfQuantity);
                if (!check) ModelState.AddModelError(string.Empty, "Something's wrong! couldn't find bill of material or labor");
                var jobType = db.FurnitureStandardJobTypes.Find(furnitureBillOfQuantity.JobTypeId);
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = new FurnitureBillOfQuantity
                        {
                            JobNo = jobType.JobCode,
                            JobTypeCategory = furnitureBillOfQuantity.JobTypeCategory,
                            JobTypeId = furnitureBillOfQuantity.JobTypeId,
                            CustomerId = furnitureBillOfQuantity.CustomerId,
                            Quantity = 1,
                            Size = furnitureBillOfQuantity.Size,
                            Status = furnitureBillOfQuantity.Status,
                            Remark = furnitureBillOfQuantity.Remark,
                            DateCreated = DateTime.Today,
                            CreatedBy = User.Identity.Name,
                            IsCustomizedJob = true,
                            Unit = jobType.Unit,
                            JobTypeName = jobType.JobTypeName,                           
                        };

                        db.FurnitureBillOfQuantities.Add(entity);
                        db.SaveChanges();
                        furnitureBillOfQuantity.FurnitureBillOfQuantityId = entity.FurnitureBillOfQuantityId;

                        // Generate materials and labor if job-type is standard

                        GenerateBillOfMaterialAndLabor(furnitureBillOfQuantity);


                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "An error occurred while creating the bill of quantity.");
                        // Log the exception if needed
                    }
                   }
                }


            }

            return Json(new[] { furnitureBillOfQuantity }.ToDataSourceResult(request, ModelState));
        }
        private bool CheckBillOfMaterialAndLaborExistence(FurnitureBillOfQuantity furnitureBillOfQuantity)
        {
            var standardjobType = db.FurnitureStandardJobTypes.Find(furnitureBillOfQuantity.JobTypeId);
            if (standardjobType == null) return false;
            var materials = db.StandardBillOfMaterials.Where(x => x.FurnitureStandardJobTypeId == standardjobType.FurnitureStandardJobTypeId).ToList();
            if (!materials.Any()) return false;
            var labors = db.StandardBillOfLabors.Where(x => x.FurnitureStandardJobTypeId == standardjobType.FurnitureStandardJobTypeId).ToList();
            if (!labors.Any()) return false;
            return true;

        }
        public void GenerateBillOfMaterialAndLabor(FurnitureBillOfQuantity furnitureBillOfQuantity)
        {
            var standardjobType = db.FurnitureStandardJobTypes.Find(furnitureBillOfQuantity.JobTypeId);
            if (standardjobType != null)
            {
                var materials = db.StandardBillOfMaterials.Where(x => x.FurnitureStandardJobTypeId == standardjobType.FurnitureStandardJobTypeId).ToList();
                var labors = db.StandardBillOfLabors.Where(x => x.FurnitureStandardJobTypeId == standardjobType.FurnitureStandardJobTypeId).ToList();
                var desgins = db.FurniturStandardJobDesigns.Where(x => x.ReferenceId == standardjobType.FurnitureStandardJobTypeId).ToList();
                if (materials.Any() && labors.Any())
                {
                    foreach (var material in materials)
                    {
                        var billOfMaterial = new FurnitureBOQMaterial
                        {
                            ManufacturingMaterialCategoryId = material.ManufacturingMaterialCategoryId,
                            ManufacturingMaterialCategoryItemId = material.ManufacturingMaterialCategoryItemId,
                            UnitOfMeasurement = material.UnitOfMeasurement,
                            Quantity = material.Quantity,
                            WorkshopType = material.WorkshopType,
                            ItemCode = material.ItemCode,
                            FurnitureBillOfQuantityId = furnitureBillOfQuantity.FurnitureBillOfQuantityId,
                            DateCreated = DateTime.Today,
                            CreatedBy = User.Identity.Name,
                            Remark = material.Remark
                        };

                        db.FurnitureBOQMaterials.Add(billOfMaterial);

                    }

                    foreach (var labor in labors)
                    {
                        var billOfLabor = new FurnitureBOQLabor
                        {
                            WorkShopType = labor.WorkShopType,
                            EstimatedMinute = labor.EstimatedMinute,
                            NumberOfEmloyees = labor.NumberOfEmloyees,
                            ManufacturingTaskCategoryId = labor.ManufacturingTaskCategoryId,
                            FurnitureBillOfQuantityId = furnitureBillOfQuantity.FurnitureBillOfQuantityId,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Today
                        };
                        db.FurnitureBOQLabors.Add(billOfLabor);

                    }

                    foreach (var design in desgins)
                    {
                        FurnitureBoqJobDesign entity = new FurnitureBoqJobDesign
                        {
                            ReferenceId = furnitureBillOfQuantity.FurnitureBillOfQuantityId,
                            Description = design.Description,
                            FileExtension = design.FileExtension,
                            FileName = design.FileName,
                            FilePath = design.FilePath,
                            ShortPath = design.ShortPath,

                        };
                        db.FurnitureBoqJobDesigns.Add(entity);
                    }
                    db.SaveChanges();
                }
            }
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBillOfQuantities_Update([DataSourceRequest]DataSourceRequest request, FurnitureBillOfQuantity furnitureBillOfQuantity)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBillOfQuantity
                {
                    FurnitureBillOfQuantityId = furnitureBillOfQuantity.FurnitureBillOfQuantityId,
                    JobTypeId = furnitureBillOfQuantity.JobTypeId,
                    CustomerId = furnitureBillOfQuantity.CustomerId,
                    Quantity = furnitureBillOfQuantity.Quantity,
                    Size = furnitureBillOfQuantity.Size,
                    Status = furnitureBillOfQuantity.Status,
                    Remark = furnitureBillOfQuantity.Remark,
                    IsVoid = furnitureBillOfQuantity.IsVoid,
                    VoidBy = furnitureBillOfQuantity.VoidBy,
                    VoidTime = furnitureBillOfQuantity.VoidTime,
                    IsOnlineApproved = furnitureBillOfQuantity.IsOnlineApproved,
                    OnlineApprovedBy = furnitureBillOfQuantity.OnlineApprovedBy,
                    OnlineApprovedTime = furnitureBillOfQuantity.OnlineApprovedTime,
                    IsOnlineTransferred = furnitureBillOfQuantity.IsOnlineTransferred,
                    IsOnlineTransferredBy = furnitureBillOfQuantity.IsOnlineTransferredBy,
                    OnlineTransferredTime = furnitureBillOfQuantity.OnlineTransferredTime,
                    IsSendForApproval = furnitureBillOfQuantity.IsSendForApproval,
                    OrgBranchName = furnitureBillOfQuantity.OrgBranchName,
                    SendForApprovalBy = furnitureBillOfQuantity.SendForApprovalBy,
                    SendForApprovalTime = furnitureBillOfQuantity.SendForApprovalTime,
                    DateCreated = furnitureBillOfQuantity.DateCreated,
                    LastModified = furnitureBillOfQuantity.LastModified,
                    CreatedBy = furnitureBillOfQuantity.CreatedBy,
                    ModifiedBy = furnitureBillOfQuantity.ModifiedBy
                };

                db.FurnitureBillOfQuantities.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureBillOfQuantity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureBillOfQuantities_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureBillOfQuantity furnitureBillOfQuantity)
        {
            if (ModelState.IsValid)
            {
                var billOfQuantity = db.FurnitureBillOfQuantities.Find(furnitureBillOfQuantity.FurnitureBillOfQuantityId);
                if (billOfQuantity != null)
                {
                    db.FurnitureBillOfQuantities.Remove(billOfQuantity);
                    var labor = db.FurnitureBOQLabors.Where(x => x.FurnitureBillOfQuantityId == billOfQuantity.FurnitureBillOfQuantityId);
                    var material = db.FurnitureBOQMaterials.Where(x => x.FurnitureBillOfQuantityId == billOfQuantity.FurnitureBillOfQuantityId);
                    db.FurnitureBOQLabors.RemoveRange(labor);
                    db.FurnitureBOQMaterials.RemoveRange(material);
                }



                db.SaveChanges();
            }

            return Json(new[] { furnitureBillOfQuantity }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult CheckBOQ(int id)
        {
            object response = null;
            var billOfQuantity = db.FurnitureBillOfQuantities.Find(id);
            if (billOfQuantity != null)
            {
                billOfQuantity.IsSendForApproval = true;
                billOfQuantity.SendForApprovalBy = User.Identity.Name;
                billOfQuantity.SendForApprovalTime = DateTime.Today;
                billOfQuantity.Status = "Checked";
                db.FurnitureBillOfQuantities.Attach(billOfQuantity);
                db.Entry(billOfQuantity).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Bill of Quantity Checked Successfully!" };
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }

        public JsonResult ApproveBOQ(int id)
        {
            object response = null;
            var billOfQuantity = db.FurnitureBillOfQuantities.Find(id);
            if (billOfQuantity != null)
            {
                if (billOfQuantity.JobTypeCategory == JobTypeCategory.Custom_Made)
                {
                    var jobtypeId = GenerateStandardJobType(billOfQuantity);
                    if (jobtypeId == 0)
                    {
                        response = new { Success = false, Message = "Something is wrong! BOQ material or labor missing" };
                        return Json(response);
                    }
                    billOfQuantity.JobTypeId = jobtypeId;
                }

                //--Generate Estimation--//
                GenerateEstimation(billOfQuantity);

                billOfQuantity.IsOnlineApproved = true;
                billOfQuantity.OnlineApprovedBy = User.Identity.Name;
                billOfQuantity.OnlineApprovedTime = DateTime.Today;
                billOfQuantity.Status = "Approved";

                db.FurnitureBillOfQuantities.Attach(billOfQuantity);
                db.Entry(billOfQuantity).State = EntityState.Modified;

                db.SaveChanges();
                
                response = new { Success = true, Message = "Successfully Approved and Created Estimation!" };
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }
        public JsonResult RejectBOQ(int id)
        {
            object response = null;
            var billOfQuantity = db.FurnitureBillOfQuantities.Find(id);
            if (billOfQuantity != null)
            {               
                billOfQuantity.IsOnlineApproved = false;
                billOfQuantity.IsSendForApproval = false;
                billOfQuantity.Status = "Rejected";
                db.FurnitureBillOfQuantities.Attach(billOfQuantity);
                db.Entry(billOfQuantity).State = EntityState.Modified;
                db.SaveChanges();                
                response = new { Success = true, Message = "Successfully Rejected" };
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }

        private void GenerateEstimation(FurnitureBillOfQuantity billOfQuantity)
        {
            var boqMaterials = db.FurnitureBOQMaterials.Where(x => x.FurnitureBillOfQuantityId == billOfQuantity.FurnitureBillOfQuantityId).ToList();
            var boqLabors = db.FurnitureBOQLabors.Where(x => x.FurnitureBillOfQuantityId == billOfQuantity.FurnitureBillOfQuantityId).ToList();

            var estimationExist = db.FurnitureEstimationForms.FirstOrDefault(x => x.FurnitureBillOfQuantityId == billOfQuantity.FurnitureBillOfQuantityId);
            
            if (boqMaterials.Any() && boqLabors.Any())
            {
                if (estimationExist != null)
                {
                    DeleteEstimationDetail(estimationExist.FurnitureEstimationId);
                    UpdateEstimationForm(billOfQuantity, boqMaterials, boqLabors);
                    
                }
                else
                {
                    var estimation = new FurnitureEstimationForm
                    {
                        JobTypeId = (int)billOfQuantity.JobTypeId,
                        CustomerId = billOfQuantity.CustomerId,
                        Quantity = billOfQuantity.Quantity,
                        FurnitureBillOfQuantityId = billOfQuantity.FurnitureBillOfQuantityId,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Status = "Pending"
                    };
                    db.FurnitureEstimationForms.Add(estimation);
                    db.SaveChanges();
                    foreach (var item in boqMaterials)
                    {
                        var materialItem = db.ManufacturingMaterialCategoryItems.FirstOrDefault(x => x.ManufacturingMaterialCategoryItemId == item.ManufacturingMaterialCategoryItemId);
                        var material = new FurnitureMaterialCost
                        {
                            TaskType = item.WorkshopType,
                            ManufacturingMaterialCategoryId = item.ManufacturingMaterialCategoryId,
                            ManufacturingMaterialCategoryItemId = item.ManufacturingMaterialCategoryItemId,
                            UnitOfMeasurment = item.UnitOfMeasurement,
                            Quantity = item.Quantity,
                            UnitPrice = materialItem?.UnitPrice ?? 0,

                            FurnitureEstimationId = estimation.FurnitureEstimationId
                        };

                        // Total Price Calculation
                        material.TotalPrice = material.UnitPrice * material.Quantity;

                        db.FurnitureMaterialCosts.Add(material);
                        db.SaveChanges();
                    }

                    var groupedBoqLabor = boqLabors.GroupBy(x => x.WorkShopType).ToList();

                    foreach (var items in groupedBoqLabor)
                    {
                        var taskCategory = db.ManifucturingTaskCategories.FirstOrDefault(x => x.Type == items.Key);
                       if(taskCategory != null)
                        {
                            var labor = new DirectLaborCost
                            {
                                CostPerMinute = taskCategory.HourRate,
                                TaskType = items.Key,
                                WorkingTime = items.Sum(x => (decimal?)x.EstimatedMinute) ?? 0,
                                FurnitureEstimationId = estimation.FurnitureEstimationId
                            };
                            labor.TotalCost = labor.WorkingTime * labor.CostPerMinute;
                            db.DirectLaborCosts.Add(labor);
                            db.SaveChanges();
                        }
                        
                    }
                }
                
                
            }
        }
        private void DeleteEstimationDetail(int estimationId)
        {
            var laborCosts = db.DirectLaborCosts.Where(x => x.FurnitureEstimationId == estimationId).ToList();
            if (laborCosts.Any())
            {
                db.DirectLaborCosts.RemoveRange(laborCosts);
                
            }
            var materialCosts = db.FurnitureMaterialCosts.Where(x => x.FurnitureEstimationId == estimationId).ToList();
            if (materialCosts.Any())
            {
                db.FurnitureMaterialCosts.RemoveRange(materialCosts);
                
            }
            var overAllCost = db.FurnitureOverallCosts.Where(x => x.FurnitureEstimationId == estimationId).ToList();
            if (overAllCost.Any())
            {
                db.FurnitureOverallCosts.RemoveRange(overAllCost);
                
            }
            db.SaveChanges();
        }
        private void UpdateEstimationForm(FurnitureBillOfQuantity billOfQuantity, List<FurnitureBOQMaterial> boqMaterials, List<FurnitureBOQLabor> boqLabors)
        {
            var estimation = db.FurnitureEstimationForms.FirstOrDefault(e => e.FurnitureBillOfQuantityId == billOfQuantity.FurnitureBillOfQuantityId);
            if(estimation != null)
            {
                estimation.JobTypeId = (int)billOfQuantity.JobTypeId;
                estimation.CustomerId = billOfQuantity.CustomerId;
                estimation.Quantity = billOfQuantity.Quantity;
                estimation.CreatedBy = User.Identity.Name;
                estimation.DateCreated = DateTime.Now;
                estimation.Status = "Pending";
                estimation.IsOnlineApproved = false;
                estimation.IsSendForApproval = false;
                db.Entry(estimation).State = EntityState.Modified;
            }
           
           
            foreach (var item in boqMaterials)
            {
                var materialItem = db.ManufacturingMaterialCategoryItems.FirstOrDefault(x => x.ManufacturingMaterialCategoryItemId == item.ManufacturingMaterialCategoryItemId);
                var material = new FurnitureMaterialCost
                {
                    TaskType = item.WorkshopType,
                    ManufacturingMaterialCategoryId = item.ManufacturingMaterialCategoryId,
                    ManufacturingMaterialCategoryItemId = item.ManufacturingMaterialCategoryItemId,
                    UnitOfMeasurment = item.UnitOfMeasurement,
                    Quantity = item.Quantity,
                    UnitPrice = materialItem?.UnitPrice ?? 0,

                    FurnitureEstimationId = estimation.FurnitureEstimationId
                };

                // Total Price Calculation
                material.TotalPrice = material.UnitPrice * material.Quantity;

                db.FurnitureMaterialCosts.Add(material);
              
            }

            var groupedBoqLabor = boqLabors.GroupBy(x => x.WorkShopType).ToList();

            foreach (var items in groupedBoqLabor)
            {
                var taskCategory = db.ManifucturingTaskCategories.FirstOrDefault(x => x.Type == items.Key);
                if(taskCategory != null)
                {
                    var labor = new DirectLaborCost
                    {
                        CostPerMinute = taskCategory.HourRate,
                        TaskType = items.Key,
                        WorkingTime = items.Sum(x => (decimal?)x.EstimatedMinute) ?? 0,
                        FurnitureEstimationId = estimation.FurnitureEstimationId
                    };
                    labor.TotalCost = labor.WorkingTime * labor.CostPerMinute;
                    db.DirectLaborCosts.Add(labor);
                    
                }
                
            }
            db.SaveChanges();
        }
        private int GenerateStandardJobType(FurnitureBillOfQuantity furnitureBillOfQuantity)
        {
            var jobType = new FurnitureStandardJobType
            {
                JobCode = furnitureBillOfQuantity.JobNo,
                JobTypeName = furnitureBillOfQuantity.JobTypeName,
                Unit = furnitureBillOfQuantity.Unit,
                Remark = furnitureBillOfQuantity.Remark,
                IsStandard = false,
                DateCreated = DateTime.Today,
                CreatedBy = User.Identity.Name

            };
            db.FurnitureStandardJobTypes.Add(jobType);
            db.SaveChanges();

            var boqMaterials = db.FurnitureBOQMaterials.Where(x => x.FurnitureBillOfQuantityId == furnitureBillOfQuantity.FurnitureBillOfQuantityId).ToList();
            var boqLabors = db.FurnitureBOQLabors.Where(x => x.FurnitureBillOfQuantityId == furnitureBillOfQuantity.FurnitureBillOfQuantityId).ToList();
            var designes = db.FurnitureBoqJobDesigns.Where(x => x.ReferenceId == furnitureBillOfQuantity.FurnitureBillOfQuantityId).ToList();
            if (!boqMaterials.Any() || !boqLabors.Any()) return 0;
            foreach (var boqMaterial in boqMaterials)
            {
                var material = new StandardBillOfMaterial
                {
                    ManufacturingMaterialCategoryId = boqMaterial.ManufacturingMaterialCategoryId,
                    ManufacturingMaterialCategoryItemId = boqMaterial.ManufacturingMaterialCategoryItemId,
                    Quantity = boqMaterial.Quantity,
                    UnitOfMeasurement = boqMaterial.UnitOfMeasurement,
                    WorkshopType = boqMaterial.WorkshopType,
                    ItemCode = boqMaterial.ItemCode,
                    FurnitureStandardJobTypeId = jobType.FurnitureStandardJobTypeId,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    Remark = boqMaterial.Remark
                };

                db.StandardBillOfMaterials.Add(material);
            }


            foreach (var boqLabor in boqLabors)
            {
                var labor = new StandardBillOfLabor
                {
                    WorkShopType = boqLabor.WorkShopType,
                    NumberOfEmloyees = boqLabor.NumberOfEmloyees,
                    EstimatedMinute = boqLabor.EstimatedMinute,
                    ManufacturingTaskCategoryId = boqLabor.ManufacturingTaskCategoryId,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    FurnitureStandardJobTypeId = jobType.FurnitureStandardJobTypeId

                };
                db.StandardBillOfLabors.Add(labor);
            }
            db.SaveChanges();

            foreach (var designe in designes)
            {
                var entity = new FurniturStandardJobDesign
                {
                    ReferenceId = jobType.FurnitureStandardJobTypeId,
                    Description = designe.Description,
                    FileExtension = designe.FileExtension,
                    FileName = designe.FileName,
                    FilePath = designe.FilePath,
                    ShortPath = designe.ShortPath
                };
                db.FurniturStandardJobDesigns.Add(entity);
            }
            db.SaveChanges();

            return jobType.FurnitureStandardJobTypeId;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
