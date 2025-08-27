using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class FurnitureEstimationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewDataMethods();
            return View();

        }
        public ActionResult EstimationSummary()
        {
            ViewDataMethods();
            return View();
        }
        private void ViewDataMethods()
        {
            ViewData["TaskCategory"] = db.ManifucturingTaskCategories.Select(s => new
            {
                Value = s.ManifucturingTaskCategoryId,
                Text = s.Name
            });
            ViewData["JobType"] = db.FurnitureStandardJobTypes.Select(s => new
            {
                Value = s.FurnitureStandardJobTypeId,
                Text = s.JobTypeName
            }).ToList();
            var selecteditems = db.InventoryItems.Select(
               i => new
               {
                   Name = i.ItemCode + "_" + i.Name,
                   Code = i.ItemCode
               }).ToList();
            var categories = db.ItemCategorys.Select(
               i => new
               {
                   Name = i.ItemCategoryCode + "_" + i.Name,
                   Code = i.ItemCategoryCode
               }).ToList();
            ViewData["Categorys"] = categories;
            var myCustomers = db.OrganizationCustomers.Select(
                s => new
                {
                    Code = s.OrganizationCustomerID,
                    Name = s.TradeName
                }).ToList();
            ViewData["Customers"] = myCustomers;

            //IList<Inventory> inventories = db.Inventories.ToList();
            ViewData["Inventories"] = selecteditems;

            IList<FurnitureStep> FurnitureSteps = db.FurnitureSteps.ToList();
            ViewData["FurnitureSteps"] = FurnitureSteps;
            ViewBag.data = GetData();

            var materialCategories = db.ManufacturingMaterialCategories.Select(s => new
            {
                Text = s.ManufacturingCategory,
                Value = s.ManufacturingMaterialCategoryId
            }).ToList();
            ViewData["MaterialCategory"] = materialCategories;
            ViewData["MaterialItem"] = db.ManufacturingMaterialCategoryItems.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();
        }
        public JsonResult GetAllEstimationForms()
        {
            var data = db.EstimationForms;
            return Json(data.Select(s => new { s.EstimationFormId, s.JobNumber }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCustomers()
        {
            var data = db.OrganizationCustomers;
            return Json(data.Select(s => new { Code = s.OrganizationCustomerID, Name = s.TradeName }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EstimationForms_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<FurnitureEstimationForm> estimationforms = db.FurnitureEstimationForms;

            DataSourceResult result = estimationforms.ToDataSourceResult(request, model => new FurnitureEstimationForm
            {
                FurnitureEstimationId = model.FurnitureEstimationId,
                CustomerId = model.CustomerId,
                JobTypeId = model.JobTypeId,
                Quantity = model.Quantity,
                IsOnlineTransferred = model.IsOnlineTransferred,
                IsOnlineApproved = model.IsOnlineApproved,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                Description = model.Description,
                FurnitureBillOfQuantityId = model.FurnitureBillOfQuantityId,
                IsCurrent = model.IsCurrent,
                IsOnlineTransferredBy = model.IsOnlineTransferredBy,
                IsSendForApproval = model.IsSendForApproval,
                IsVoid = model.IsVoid,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                OnlineApprovedBy = model.OnlineApprovedBy,
                OnlineApprovedTime = model.OnlineApprovedTime,
                OnlineTransferredTime = model.OnlineTransferredTime,
                OrgBranchName = model.OrgBranchName,
                Remark = model.Remark,
                SendForApprovalBy = model.SendForApprovalBy,
                SendForApprovalTime = model.SendForApprovalTime,
                Status = model.Status,
                VoidBy = model.VoidBy,
                VoidTime = model.VoidTime,
                IsMarginAssigned = model.IsMarginAssigned,
                JobCreated = model.JobCreated
            });
            return Json(result);
        }
        public ActionResult FurnitureEstimations_Create([DataSourceRequest]DataSourceRequest request, FurnitureEstimationForm model)
        {
            if (ModelState.IsValid)
            {
                GenerateEstimation(model.JobTypeId, model.CustomerId, model.Quantity, model.Description);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureEstimations_Update([DataSourceRequest]DataSourceRequest request, FurnitureEstimationForm model)
        {
            if (ModelState.IsValid)
            {
                Delete(model.FurnitureEstimationId);
                GenerateEstimation(model.JobTypeId, model.CustomerId, model.Quantity, model.Description);

            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationForms_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureEstimationForm model)
        {

            Delete(model.FurnitureEstimationId);

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Delete(int id)
        {
            var estimation = db.FurnitureEstimationForms.Find(id);
            if (estimation != null)
            {
                var labor = db.FurnitureLaborCosts.Where(x => x.EstimationFormId == estimation.FurnitureEstimationId).ToList();
                var material = db.FurnitureMaterialCosts.Where(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId).ToList();
                if (labor.Any() && material.Any())
                {
                    db.FurnitureLaborCosts.RemoveRange(labor);
                    db.FurnitureMaterialCosts.RemoveRange(material);
                }
                db.FurnitureEstimationForms.Remove(estimation);

                db.SaveChanges();

            }

        }
        private void GenerateEstimation(int id, int customerId, decimal quantity, string description)
        {
            var standardob = db.FurnitureStandardJobTypes.Find(id);
            var boqMaterial = db.StandardBillOfMaterials.Where(x => x.FurnitureStandardJobTypeId == standardob.FurnitureStandardJobTypeId).ToList();
            var boqLabor = db.StandardBillOfLabors.Where(x => x.FurnitureStandardJobTypeId == standardob.FurnitureStandardJobTypeId).ToList();
            if (boqMaterial.Any() && boqLabor.Any())
            {
                var estimation = new FurnitureEstimationForm
                {
                    JobTypeId = standardob.FurnitureStandardJobTypeId,
                    CustomerId = customerId,
                    Quantity = quantity,
                    Description = description,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Status = "Created"
                };
                db.FurnitureEstimationForms.Add(estimation);
                db.SaveChanges();

                foreach (var item in boqMaterial)
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

                var groupedBoqLabor = boqLabor.GroupBy(x => x.WorkShopType).ToList();

                foreach (var items in groupedBoqLabor)
                {
                    var taskCategory = db.ManifucturingTaskCategories.FirstOrDefault(x => x.Type == items.Key);

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

        public JsonResult CheckEstimation(int id)
        {
            var estimation = db.FurnitureEstimationForms.Find(id);
            object response = null;
            var overAllCost = db.FurnitureOverallCosts.FirstOrDefault(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId);
            if (overAllCost == null)
            {
                response = new { Success = false, Message = "⚠️ You forgot to calculate overall cost" };
                return Json(response);
            }
            if (estimation != null)
            {
                estimation.IsSendForApproval = true;
                estimation.SendForApprovalBy = User.Identity.Name;
                estimation.Status = "Checked";
                estimation.SendForApprovalTime = DateTime.Now;

                db.Entry(estimation).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Estimation has been checked Successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Estimation not found!" };
            }

            return Json(response);

        }

        
        public JsonResult ApproveEstimationForm(int id)
        {
            Object response = null;
            var estimationForm = db.FurnitureEstimationForms.Find(id);
            var overAllCost = db.FurnitureOverallCosts.Where(x => x.FurnitureEstimationId == estimationForm.FurnitureEstimationId).ToList();
            if (overAllCost.Any())
            {
                estimationForm.IsOnlineApproved = true;
                estimationForm.OnlineApprovedBy = User.Identity.Name;
                estimationForm.OnlineApprovedTime = DateTime.Today;
                estimationForm.Status = "Approved";
                // Generates Estimation Summary
                CreateEstimationSummary(estimationForm);
                db.Entry(estimationForm).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Successfully Approved!" };
            }
            else
            {
                response = new { Success = false, Message = "" };

            }
            return Json(response);
        }
        public JsonResult RejectEstimationForm(int id)
        {
            Object response = null;
            var estimationForm = db.FurnitureEstimationForms.Find(id);

            estimationForm.IsOnlineApproved = false;
            estimationForm.IsSendForApproval = false;
            estimationForm.Status = "Rejected";
            
            // reverse BOQ 
            ReverseBOQ(estimationForm.FurnitureBillOfQuantityId);
            db.Entry(estimationForm).State = EntityState.Modified;
            db.SaveChanges();
            response = new { Success = true, Message = "Successfully Rejected!" };

            return Json(response);
        }

        private void ReverseBOQ(int id)
        {
            var fBOQ = db.FurnitureBillOfQuantities.FirstOrDefault(x => x.FurnitureBillOfQuantityId == id);
            if(fBOQ != null)
            {
                fBOQ.IsOnlineApproved = false;
                fBOQ.IsSendForApproval = false;
                fBOQ.Status = "Rejected";
                db.Entry(fBOQ).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private void CreateEstimationSummary(FurnitureEstimationForm furnitureEstimationForm)
        {
            var OverAllCost = db.FurnitureOverallCosts.FirstOrDefault(x => x.FurnitureEstimationId == furnitureEstimationForm.FurnitureEstimationId);
            var estimationSummary = db.EstimationSummaries.FirstOrDefault(x => x.CustomerId == furnitureEstimationForm.CustomerId && !x.IsCalculated);
            if (estimationSummary == null)
            {
                EstimationSummary summary = new EstimationSummary
                {
                    CustomerId = furnitureEstimationForm.CustomerId,
                    DateCreated = DateTime.Today,
                    CreatedBy = User.Identity.Name,
                    Status = "Pending"
                };

                db.EstimationSummaries.Add(summary);
                db.SaveChanges();

                EstimationDetail estimationDetailNew = new EstimationDetail
                {
                    JobTypeId = furnitureEstimationForm.JobTypeId,
                    MaterialCost = OverAllCost.MaterialCost,
                    LabourCost = OverAllCost.LabourCost,
                    AdministrativeCost = OverAllCost.AdministrativeCost,
                    OverHeadCost = OverAllCost.OverHeadCost,
                    ManufacturingCost = OverAllCost.MaterialCost,
                    GrandTotalCost = OverAllCost.GrandTotalCost,
                    Quantity = furnitureEstimationForm.Quantity,
                    EstimationSummaryId = summary.EstimationSummaryId,
                    FurnitureEstimationId = furnitureEstimationForm.FurnitureEstimationId,                    
                };

                db.EstimationDetails.Add(estimationDetailNew);
                db.SaveChanges();
            }
            else
            {
                EstimationDetail estimationDetail = new EstimationDetail
                {
                    JobTypeId = furnitureEstimationForm.JobTypeId,
                    MaterialCost = OverAllCost.MaterialCost,
                    LabourCost = OverAllCost.LabourCost,
                    AdministrativeCost = OverAllCost.AdministrativeCost,
                    OverHeadCost = OverAllCost.OverHeadCost,
                    ManufacturingCost = OverAllCost.MaterialCost,
                    GrandTotalCost = OverAllCost.GrandTotalCost,
                    Quantity = furnitureEstimationForm.Quantity,
                    EstimationSummaryId = estimationSummary.EstimationSummaryId,
                    FurnitureEstimationId = furnitureEstimationForm.FurnitureEstimationId
                };

                db.EstimationDetails.Add(estimationDetail);
                db.SaveChanges();
            }

        }
        public JsonResult ApproveEstimation(int id)
        {
            var estimation = db.EstimationForms.Find(id);
            object response = null;
            if (estimation != null)
            {
                if (estimation.IsChecked)
                {
                    estimation.IsApproved = true;
                    estimation.ApprovedBy = User.Identity.Name;

                    var oc = db.OverallCosts.Where(q => q.EstimationFormId == estimation.EstimationFormId).FirstOrDefault();
                    db.EstimationForms.Attach(estimation);
                    db.Entry(estimation).State = EntityState.Modified;
                    //add new proforma 
                    var proforma = new FurnitureProforma();
                    proforma.HaveEstimation = true;
                    proforma.IsExistingCustomer = true;
                    proforma.FurnitureProformaId = proforma.FurnitureProformaId;
                    proforma.ProformaNo = estimation.EstimationFormId.ToString();
                    proforma.CustomerId = estimation.CustomerId;
                    proforma.EstimationFormId = estimation.EstimationFormId;
                    proforma.DateCreated = DateTime.Now;
                    proforma.CreatedBy = User.Identity.Name;
                    db.FurnitureProformas.Add(proforma);
                    db.SaveChanges();
                    decimal beforeTax = 0;
                    decimal withTax = 0;
                    decimal vat = 0;
                    var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId.ToString() == estimation.JobTypeId.ToString());
                    if (jobType != null)
                    {
                        var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                        if (tax != null)
                        {
                            beforeTax = estimation.Quantity * oc.EstimatedNetPrice;
                            withTax = beforeTax + (beforeTax * Convert.ToDecimal(tax.CalculatedRate));
                            vat = withTax - beforeTax;
                        }
                    }
                    var proformaItem = new FurniturePFProformaItems();
                    proformaItem.FurniturePFProformaItemsId = proformaItem.FurniturePFProformaItemsId;
                    proformaItem.FurnitureProformaId = proforma.FurnitureProformaId;
                    proformaItem.FurnitureProformaId = proforma.FurnitureProformaId;
                    proformaItem.FurnitureJobTypeId = estimation.JobTypeId;
                    proformaItem.Quantity = estimation.Quantity;
                    proformaItem.UnitPrice = (decimal)oc?.EstimatedNetPrice;
                    proformaItem.BeforeTax = beforeTax;
                    proformaItem.TotalPrice = withTax;
                    proformaItem.Size = estimation.Size;
                    proformaItem.Tax = vat;

                    db.FurniturePFProformaItems.Add(proformaItem);
                    db.SaveChanges();
                    response = new { Success = true, Message = "Estimation has been checked Successfully!" };
                }
                else
                {
                    response = new { Success = false, Message = "Estimation not found!" };
                }
            }
            else
            {
                response = new { Success = false, Message = "Estimation not found!" };

            }

            return Json(response);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        private IEnumerable<DropDownTreeItemModel> GetData()
        {
            var jobCategories = db.JobCategories.Where(q => q.HaveParent == false).ToList();
            List<DropDownTreeItemModel> jobs = new List<DropDownTreeItemModel>();
            foreach (var jobCategory in jobCategories)
            {
                int i = 0;
                DropDownTreeItemModel jobType = new DropDownTreeItemModel();
                List<DropDownTreeItemModel> jobTypes = new List<DropDownTreeItemModel>();

                var childs = db.JobCategories.Where(q => q.ParentId == jobCategory.JobCategoryId).ToList();
                foreach (var child in childs)
                {
                    i++;
                    DropDownTreeItemModel prt = new DropDownTreeItemModel();
                    prt.Text = child.JobName;
                    prt.Selected = true;
                    prt.HasChildren = false;
                    prt.Value = child.JobCategoryId.ToString();
                    jobTypes.Add(prt);
                }
                jobType.Text = jobCategory.JobName;

                jobType.Value = jobCategory.JobCategoryId.ToString();
                jobType.Items = jobTypes;
                jobs.Add(jobType);


            }



            return jobs;
        }

    }
}
