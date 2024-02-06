using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Web.Areas.Manufacturing.Models;
using ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.ViewModel;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class FurnitureAutoJobOrderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Proforma"] = db.Proformas.ToList();
            ViewData["JobType"] = db.JobCategories.Select(s => new {
                JobTypeId = s.JobCategoryId,
                type = s.HaveParent?db.JobCategories.Where(q=>q.JobCategoryId==s.ParentId).FirstOrDefault().JobName+"-"+ s.JobName : s.JobName
            }).ToList();
            ViewData["CustomerTypes"] = db.ProductionCustomerTypes.Select(s => new {
                Code = s.ProductionCustomerTypeId,
                Name = s.Name
            }).ToList();
            ViewData["Customers"] = db.OrganizationCustomers.Select(s => new {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();
            ViewData["Machines"] = db.Machines.Select(s => new {
                Code = s.MachineId,
                Name = s.MachineName
            }).ToList();

           
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var costCenter = db.BranchCostCenter;
            var branches = db.Branch;

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

            ViewData["Branches"] = branches;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

         

            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;
            
            ViewBag.data = GetData();

            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var stores = db.InventoryStores;
            var branches = db.Branch;
            var costCenter = db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            var inventoryPeriods = db.InventoryPeriods;
            

            ViewData["costCenters"] = selectedcostCenter;
            ViewData["stores"] = stores;
            ViewData["Branches"] = branches;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;

            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }

        public JsonResult GetAllJobOrders()
        {
            var data = db.Jobs;
            return Json(data.Select(s => new { s.JobId, s.JobNo }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobOrder(int jobId)
        {
            var jobOrder = db.Jobs.Find(jobId);
            return Json(jobOrder,JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobCategoryTree()
        {
            var jobCategories = db.JobCategories.ToList();

            var result = jobCategories.Where(q => !q.HaveParent).Select(q => new
            {
                ID = q.JobCategoryId,
                Name = q.JobName,
                HasChildren = db.JobCategories.Where(xc => xc.HaveParent).Where(s => s.ParentId == q.JobCategoryId)
                .Select(se => new
                {
                    ID = se.JobCategoryId,
                    Name = se.JobName,
                }).ToList()
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Jobs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJob> jobs = db.FurnitureJobs/*.Where(q=>q.IsApproved==true&&q.hv)*/;
            DataSourceResult result = jobs.ToDataSourceResult(request, job => new
            {
                JobId = job.FurnitureJobId,
                Time = job.Time,
                hv = job.hv,
                IsExistingCustomer = job.IsExistingCustomer,
                ProformaId = job.FurnitureProformaId,
                ProductionCustomertypeId = job.ProductionCustomertypeId,
                MachineId = job.MachineId,
                JobNo = job.JobNo,
                CustomerId = job.FurnitureCustomerId,
                EstimatedBy = job.EstimatedBy,
                Code = job.Code,
                IsMaga = job.IsMaga,
                ApprovedBy = job.ApprovedBy,
                BrandName = job.BrandName,
                TinNo = job.TinNo,
                CustomerName = job.CustomerName,
                JobTypeId = job.FurnitureJobTypeId,
                JT = job.FurnitureJobTypeId.ToString(),
                UnitPrice = job.UnitPrice,
                BeforeTax = job.BeforeTax,
                Quantity = job.Quantity,
                Pages = job.Pages,
                ContactPhone = job.ContactPhone,
                Copies = job.Copies,
                TotalPrice = job.TotalPrice,
                Contract = job.Contract,
                TotalImpression = job.TotalImpression,
                DeliveryDate = job.DeliveryDate,
                ReceivedDate = job.ReceivedDate,
                TextWeight = job.TextWeight,
                CoverWeight = job.CoverWeight,
                ColorText = job.ColorText,
                Colorcover = job.Colorcover,
                FinishingSize = job.FinishingSize,
                PrintingLine = job.PrintingLine,
                PrintingMachine = job.PrintingMachine,
                Sewing = job.Sewing,
                New = job.New,
                PerfectBinding = job.PerfectBinding,
                Repeat = job.Repeat,
                Lamnating = job.Lamnating,
                Varnish = job.Varnish,
                Perforate = job.Perforate,
                Camsur = job.Camsur,
                IsApproved = job.IsApproved,
                UnitId = job.UnitId,
                GraphicDesignExpectedCompletetion = job.GraphicDesignExpectedCompletetion,
                PrintingExpectedCompletetion = job.PrintingExpectedCompletetion,
                FinishExpectedCompletetion = job.FinishExpectedCompletetion,
                Status = job.Status,
                FinishingWorkflow = job.FinishingWorkflow,
                CreatedBy = job.CreatedBy,
                DateCreated = job.DateCreated,
                ModifiedBy = job.ModifiedBy,
                LastModified = job.LastModified
            });

            return Json(result);
        }

        public ActionResult MaterialJobOrders_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureMaterialCost> materialcosts = db.FurnitureMaterialCosts.Where(q => q.JobId == id);
            DataSourceResult result = materialcosts.ToDataSourceResult(request, materialCost => new {
                MaterialCostId = materialCost.FurnitureMaterialCostId,
                EstimationFormId = materialCost.FurnitureEstimationId,
                InventoryCategoryId = materialCost.InventoryCategoryId,
                InventoryId = materialCost.InventoryId,
                Quantity = materialCost.Quantity,
                UnitPrice = materialCost.UnitPrice,
                TotalPrice = materialCost.TotalPrice,
                DateCreated = materialCost.DateCreated,
                LastModified = materialCost.LastModified
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Create([DataSourceRequest]DataSourceRequest request, FurnitureJob job)
        {
            job.FurnitureJobTypeId = Int16.Parse(job.JT);
            if (ModelState.IsValid)
            {
                //job.CreatedBy = User.Identity.Name;
                //job.DateCreated = DateTime.Now;
                var proforma = new FurnitureProforma();
                //var customer = new OrganizationCustomer();
                //if (!job.hv && !job.IsExistingCustomer)
                //{
                //    customer = AddNewCustomer.Add(job);
                //    db.OrganizationCustomers.Add(customer);
                //    db.SaveChanges();
                //    Index();

                //}
                //else if (job.hv)
                //{
                //    proforma = db.Proformas.Find(job.ProformaId);

                //} 
                 if (job.hv)
                {
                    proforma = db.FurnitureProformas.Find(job.FurnitureProformaId);

                }
                job.FurnitureCustomerId = job.hv ? proforma.CustomerId :/* customer.OrganizationCustomerID|*/ job.FurnitureCustomerId;
                var entity = (FurnitureJob)this.GetObject(job);

                db.FurnitureJobs.Add(entity);
                db.SaveChanges();
            }
                return Json(new[] { job }.ToDataSourceResult(request, ModelState));
            
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Update([DataSourceRequest]DataSourceRequest request, FurnitureJob job)
        {
            job.FurnitureJobTypeId = Int16.Parse(job.JT);

            if (ModelState.IsValid)
            {
                job.ModifiedBy = User.Identity.Name;
                job.LastModified = DateTime.Now;
                var proforma = new FurnitureProforma();
                var customer = new OrganizationCustomer();
                if (!job.hv && !job.IsExistingCustomer&&job.FurnitureCustomerId.ToString()==null)
                {
                    customer = AddNewCustomer.Add(job);
                    db.OrganizationCustomers.Add(customer);
                    db.SaveChanges();


                }
                else if (job.hv)
                {
                    proforma = db.FurnitureProformas.Find(job.FurnitureProformaId);

                }
                job.FurnitureCustomerId = job.hv ? proforma.CustomerId : customer.OrganizationCustomerID | job.FurnitureCustomerId;
                var entity = (FurnitureJob)this.GetObject(job);
                if(job.FurnitureJobId != 0)
                {
                    db.FurnitureJobs.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                if(job.FurnitureJobId == 0)
                {
                    db.FurnitureJobs.Add(entity);

                }

                db.SaveChanges();
            }

            return Json(new[] { job }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJob job)
        {
            if (ModelState.IsValid)
            {

                var entity = (FurnitureJob)this.GetObject(job);
                db.FurnitureJobs.Attach(entity);
                db.FurnitureJobs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { job }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetCustomer(int proformaId)
        {
            var proforma = db.Proformas.FirstOrDefault(x => x.ProformaId == proformaId);

            if (proforma != null)
            {
                var customerData = Json(new
                {
                    Status = "OK",
                    CustomerName = proforma.CustomerName,
                    BrandName = proforma.BrandName,
                    TinNo = proforma.TinNo
                });

                return customerData;
            }

            var customerNoData = Json(new
            {
                Status = "OK",
                CustomerName = "",
                ContactName = "",
                ContactPhone = ""
            });

            return customerNoData;
        }
        public ActionResult ChangeOrder(ChangeOfOrderVM changeOfOrder)
        {
            var jobOrder = db.FurnitureJobs.FirstOrDefault(x => x.FurnitureJobId == changeOfOrder.JobId);

            if (jobOrder != null)
            {
                //jobOrder.Quant = changeOfOrder.Quantity;
                //jobOrder.FinishingSize = changeOfOrder.Size;
                jobOrder.Status = FurnitureProductionJobOrderStatus.ChangedOrder;
                //jobOrder.Reason = changeOfOrder.Reason;
                //db.Entry(jobOrder).State = EntityState.Modified;
                //db.SaveChanges();
                var jobStatuses = db.FurnitureJobStatuses.Where(q => q.FurnitureJobId == changeOfOrder.JobId && q.JobPhase != FurnitureProductionDepartment.Graphic).ToList();
                foreach(var jobStatus in jobStatuses)
                {
                    var jobStatusLineItem = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.FurnitureJobStatusId).OrderByDescending(or=>or.JobStatusLineItemId).FirstOrDefault();
                    if(jobStatusLineItem!=null)
                    {
                        jobStatusLineItem.RemainigQuantiy = changeOfOrder.ChangedQuantity;
                        db.JobStatusLineItems.Attach(jobStatusLineItem);
                        db.Entry(jobStatusLineItem).State = EntityState.Modified;
                    }
                }
                var changeOrder = new FurnitureChangeOrder();

                changeOrder.Date = DateTime.Now;// changeOfOrder.Date;
                changeOrder.Quantity = changeOfOrder.ChangedQuantity;
                changeOrder.Size = changeOfOrder.Size;
                changeOrder.FurnitureJobId = changeOfOrder.JobId;
                changeOrder.Tax = changeOfOrder.Tax;
                changeOrder.Reason = changeOfOrder.Reason;
                changeOrder.Price = changeOfOrder.Price;
                changeOrder.Total = changeOfOrder.Total;
                changeOrder.TypeAndQuantityOfMaterialNeeded = changeOfOrder.TypeAndQuantityOfMaterialNeeded;

                db.FurnitureChangeOrders.Add(changeOrder);
                db.SaveChanges();

                TempData["Success"] = "Successfully Changed";

                return RedirectToAction("Index");
            }

            TempData["Error"] = "Invalid Data ";

            return RedirectToAction("Index");
        }
        public JsonResult ApproveJobOrder(int id)
        {
            var job = db.FurnitureJobs.Find(id);
            object response = null;

            if (job != null)
            {
                job.IsApproved = true;
                db.FurnitureJobs.Attach(job);
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Job has been approved Successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Job is not found!" };
            }

            return Json(response);

        }
        public ActionResult GetBeforeVatAndTotalPerice(string jobTypeId, int quant, double unitPrice)
        {
            var jobType = db.FurnitureJobCategories.FirstOrDefault(x => x.FurnitureJobCategoryId.ToString() == jobTypeId);
            if (jobType != null)
            {
                var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                if (tax != null)
                {
                    double beforeTax = quant * unitPrice;
                    double withTax = (quant * unitPrice) + (quant * unitPrice * Convert.ToDouble(tax.CalculatedRate));

                    var calculatedField = Json(new
                    {
                        Status = "OK",
                        BeforeTax = Math.Round(beforeTax,5),
                        TotalPrice = Math.Round(withTax,5)
                    });

                    return calculatedField;
                }
            }

            var nodata = Json(new
            {
                Status = "OK",
                BeforeTax = 0,
                TotalPrice = 0
            });

            return nodata;

        }
        public ActionResult GetTaxAndTotalPriceChangeOfOrder(int jobId, int quantity, double unitPrice)
        {
            var job = db.FurnitureJobs.FirstOrDefault(x => x.FurnitureJobId == jobId);
            if (job != null)
            {
                var jobType = db.FurnitureJobCategories.Find(job.FurnitureJobTypeId);
                if (jobType != null)
                {
                    var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                    if (tax != null)
                    {
                        double taxBirr = quantity * unitPrice * Convert.ToDouble(tax.CalculatedRate);
                        double withTax = (quantity * unitPrice) + (quantity * unitPrice * Convert.ToDouble(tax.CalculatedRate));

                        var calculatedField = Json(new
                        {
                            Status = "OK",
                            Tax = Math.Round(taxBirr,5),
                            Total = Math.Round(withTax,5)
                        });

                        return calculatedField;
                    }
                }

            }
            var nodata = Json(new
            {
                Status = "OK",
                Tax = 0,
                Total = 0
            });

            return nodata;

        }
        private object GetObject(FurnitureJob job)
        {
            var entity = new FurnitureJob
            {
                FurnitureJobId = job.FurnitureJobId,
                hv = job.hv,
                MachineId = job.MachineId,
                EstimatedBy = job.EstimatedBy,
                ApprovedBy = job.ApprovedBy,
                Time = job.Time,
                FurnitureProformaId = job.FurnitureProformaId,
                JobNo = job.JobNo,
                IsMaga = job.IsMaga,
                CustomerName = job.CustomerName,
                FurnitureCustomerId = job.FurnitureCustomerId,
                Code = job.Code,
                FurnitureJobTypeId = job.FurnitureJobTypeId,
                BeforeTax = job.BeforeTax,
                UnitPrice = job.UnitPrice,
                Quantity = job.Quantity,
                ContactPhone = job.ContactPhone,
                Pages = job.Pages,
                Copies = job.Copies,
                ProductionCustomertypeId = job.ProductionCustomertypeId,
                TotalPrice = job.TotalPrice,
                Contract = job.Contract,
                TotalImpression = job.TotalImpression,
                DeliveryDate = job.DeliveryDate,
                ReceivedDate = job.ReceivedDate,
                TextWeight = job.TextWeight,
                CoverWeight = job.CoverWeight,
                ColorText = job.ColorText,
                TinNo = job.TinNo,
                IsApproved = job.IsApproved,
                BrandName = job.BrandName,
                Colorcover = job.Colorcover,
                FinishingSize = job.FinishingSize,
                PrintingLine = job.PrintingLine,
                PrintingMachine = job.PrintingMachine,
                Sewing = job.Sewing,
                New = job.New,
                PerfectBinding = job.PerfectBinding,
                Repeat = job.Repeat,
                UnitId = job.UnitId,
                Lamnating = job.Lamnating,
                Varnish = job.Varnish,
                Perforate = job.Perforate,
                Camsur = job.Camsur,
                GraphicDesignExpectedCompletetion = job.GraphicDesignExpectedCompletetion,
                PrintingExpectedCompletetion = job.PrintingExpectedCompletetion,
                FinishExpectedCompletetion = job.FinishExpectedCompletetion,
                Status = job.Status,
                CreatedBy = job.CreatedBy,
                DateCreated = job.DateCreated,
                ModifiedBy = job.ModifiedBy,
                LastModified = job.LastModified
            };
            return entity;
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


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
