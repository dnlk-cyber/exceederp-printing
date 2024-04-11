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
                s=>new {
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
            return View();

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
                IsOnlineApproved = model.IsOnlineApproved
                
            });

            return Json(result);
        }
        public ActionResult FurnitureEstimations_Create([DataSourceRequest]DataSourceRequest request, FurnitureEstimationForm estimationForm)
        {
            if (ModelState.IsValid)
            {

                var entity = new FurnitureEstimationForm
                {
                    FurnitureEstimationId = estimationForm.FurnitureEstimationId,
                    CustomerId = estimationForm.CustomerId,
                    
                    Quantity = estimationForm.Quantity,
                   
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                   
                };

                db.FurnitureEstimationForms.Add(entity);
                db.SaveChanges();
                estimationForm.FurnitureEstimationId = entity.FurnitureEstimationId;
                
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { estimationForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureEstimations_Update([DataSourceRequest]DataSourceRequest request, FurnitureEstimationForm estimationForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureEstimationForm
                {
                    FurnitureEstimationId = estimationForm.FurnitureEstimationId,
                    CustomerId = estimationForm.CustomerId,
                    
                    Quantity = estimationForm.Quantity,
                    //Size = estimationForm.Size,
                    //JobTypeId = int.Parse(estimationForm.JT),
                    //TextPaper = estimationForm.TextPaper,
                    //TextPrint = estimationForm.TextPrint,
                    //CoverPrint = estimationForm.CoverPrint,
                    //BindingStyle = estimationForm.BindingStyle,
                    
            
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                   

                };

                db.FurnitureEstimationForms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { estimationForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationForms_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureEstimationForm estimationForm)
        {

            //if (ModelState.IsValid)
            //{
            var estimation = db.FurnitureEstimationForms.Find(estimationForm.FurnitureEstimationId);
            var labor = db.FurnitureLaborCosts.Where(x => x.EstimationFormId == estimation.FurnitureEstimationId).ToList();
            var material = db.FurnitureMaterialCosts.Where(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId).ToList();
            db.FurnitureLaborCosts.RemoveRange(labor);
            db.FurnitureMaterialCosts.RemoveRange(material);
            db.SaveChanges();
            db.FurnitureEstimationForms.Remove(estimation);
                
            db.SaveChanges();
            //}

            return Json(new[] { estimationForm }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult CheckEstimation(int id)
        {
            var estimation = db.FurnitureEstimationForms.Find(id);
            object response = null;
            var overAllCost = db.FurnitureOverallCosts.FirstOrDefault(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId);
            if(overAllCost == null || overAllCost.FinalPriceIncludingProfit == 0)
            {
                response = new { Success = false, Message = "⚠️ You forgot to calculate overall cost" };
                return Json(response);
            }
            if (estimation!=null)
                {
                estimation.IsOnlineTransferred = true;
                estimation.IsOnlineTransferredBy = User.Identity.Name;
                estimation.OnlineTransferredTime = DateTime.Today;
                
                db.Entry(estimation).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true,Message= "Estimation has been checked Successfully!" };

            }
            else
            {
                response= new { Success = false, Message = "Estimation not found!" };
            }

            return Json(response);
           
        }

        public JsonResult RejectEstimation(int id)
        {
            var estimation = db.EstimationForms.Find(id);
            object response = null;

            if (estimation != null)
            {
                estimation.IsChecked = false;
                db.EstimationForms.Attach(estimation);
                db.Entry(estimation).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Estimation has been rejected Successfully!" };

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
            estimationForm.IsOnlineApproved = true;
            estimationForm.OnlineApprovedBy = User.Identity.Name;
            estimationForm.OnlineApprovedTime = DateTime.Today;

            db.Entry(estimationForm).State = EntityState.Modified;
            db.SaveChanges();
            response = new { Success = true, Message = "Successfully Approved!" };
            //if (estimationForm != null)
            //{
            //    var overAllCost = db.FurnitureOverallCosts.FirstOrDefault(x => x.FurnitureEstimationId == estimationForm.FurnitureEstimationId);

            //    if(overAllCost != null)
            //    {
            //        estimationForm.IsOnlineApproved = true;
            //        estimationForm.OnlineApprovedBy = User.Identity.Name;
            //        estimationForm.OnlineApprovedTime = DateTime.Today;

            //        db.Entry(estimationForm).State = EntityState.Modified;

            //        var existingProforma = db.FurnitureProformaInvoices.FirstOrDefault(x => x.CustomerId == estimationForm.CustomerId);
            //        if(existingProforma == null)
            //        {
            //            // Create Proforma Invoice
            //            var proformaInvoice = new FurnitureProformaInvoice
            //            {
            //                CustomerId = estimationForm.CustomerId,
            //                DateCreated = DateTime.Today,
            //                CreatedBy = User.Identity.Name,                           
            //                DeliveryDate = DateTime.Today,
            //            };
            //            db.FurnitureProformaInvoices.Add(proformaInvoice);
            //            db.SaveChanges();

            //            // Create ProformaInvoiceItem
            //            var jobTypeN = db.FurnitureStandardJobTypes.Find(estimationForm.JobTypeId);
            //            if (jobTypeN == null)
            //            {
            //                response = new { Success = false, Message = "Something's wrong! Job Type Not Found" };
            //                return Json(response);
            //            }

            //            var proformaItemN = new FurnitureProformaInvoiceItem
            //            {
            //                JobTypeId = jobTypeN.FurnitureStandardJobTypeId,
            //                Quantity = estimationForm.Quantity,
            //                UnitPriceBeforeVAT = overAllCost.FinalPriceIncludingProfit,
            //                VAT = 0.15M,/* taxRate.CalculatedRate*/
            //                DateCreated = DateTime.Today,
            //                FurnitureProformaInvoiceId = proformaInvoice.FurnitureProformaInvoiceId,
            //                UnitOfMeasurment = jobTypeN.Unit,
            //                IsActive = true,

            //            };
            //            proformaItemN.UnitPriceWithVAT = proformaItemN.UnitPriceBeforeVAT + proformaItemN.UnitPriceBeforeVAT * proformaItemN.VAT;

            //            db.FurnitureProformaInvoiceItems.Add(proformaItemN);
            //            db.SaveChanges();
            //            response = new { Success = true, Message = "Successfully Created proforma Invoice" };
            //        }
            //        else
            //        {
            //            // Create ProformaInvoiceItem
            //            var jobType = db.FurnitureStandardJobTypes.Find(estimationForm.JobTypeId);
            //            if (jobType == null)
            //            {
            //                response = new { Success = false, Message = "Something's wrong! Job Type Not Found" };
            //                return Json(response);
            //            }

            //            var proformaItem = new FurnitureProformaInvoiceItem
            //            {
            //                JobTypeId = jobType.FurnitureStandardJobTypeId,
            //                Quantity = estimationForm.Quantity,
            //                UnitPriceBeforeVAT = overAllCost.FinalPriceIncludingProfit,
            //                VAT = 0.15M,/* taxRate.CalculatedRate*/
            //                DateCreated = DateTime.Today,
            //                FurnitureProformaInvoiceId = existingProforma.FurnitureProformaInvoiceId,
            //                UnitOfMeasurment = jobType.Unit,
            //                IsActive = true
            //            };
            //            proformaItem.UnitPriceWithVAT = proformaItem.UnitPriceBeforeVAT + proformaItem.UnitPriceBeforeVAT * proformaItem.VAT;

            //            db.FurnitureProformaInvoiceItems.Add(proformaItem);
            //            db.SaveChanges();
            //            response = new { Success = true, Message = "Successfully Created proforma Invoice" };
            //        }

            //    }
            //    else
            //    {
            //        response = new { Success = false, Message = "Something's Wrong!, Overall Cost Not Found" };

            //    }
            //}
            //else
            //{
            //    response = new { Success = false, Message = "Something's Wrong!" };
            //}

            return Json(response);
        }
        public JsonResult ApproveEstimation(int id)
        {
            var estimation = db.EstimationForms.Find(id);
            object response = null;
            if (estimation != null)
            {
                if(estimation.IsChecked)
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
                    proformaItem.Size = estimation.Size ;
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
