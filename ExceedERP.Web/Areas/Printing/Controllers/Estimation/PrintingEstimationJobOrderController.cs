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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;
using ExceedERP.Web.Areas.Printing.Models;
using ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.ViewModel;

namespace ExceedERP.Web.Areas.Printing.Controllers.Estimation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class PrintingEstimationJobOrderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            
            ViewBag.Proforma = db.Proformas.Select(s => new
            {
                Text = s.BrandName,
                Value = s.ProformaId
            }).ToList();

            ViewData["JobType"] = db.JobCategories.Select(s => new
            {
                Value = s.JobCategoryId,
                Text = s.HaveParent ? db.JobCategories.Where(q => q.JobCategoryId == s.ParentId).FirstOrDefault().JobName + "-" + s.JobName : s.JobName
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
            var costCenter = db.OrgBranchUserMappings;
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

            //var selectedcostCenter = costCenter.Select(
            //    i => new
            //    {
            //        Name = i.MainCompanyStructure.Name,
            //        Code = i.OrgStructureId
            //    }).AsQueryable();

            //ViewData["costCenters"] = selectedcostCenter;

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
            //var costCenter = db.BranchCostCenter;
            //var selectedcostCenter = costCenter.Select(
            //  i => new
            //  {
            //      Name = i.MainCompanyStructure.Name,
            //      Code = i.OrgStructureId
            //  }).AsQueryable();

            var inventoryPeriods = db.InventoryPeriods;
            

            //ViewData["costCenters"] = selectedcostCenter;
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
            IQueryable<Job> jobs = db.Jobs.Where(q =>q.hv);
            DataSourceResult result = jobs.ToDataSourceResult(request, job => new
            {
                JobId = job.JobId,
                Time = job.Time,
                hv = job.hv,
                IsExistingCustomer = job.IsExistingCustomer,
                ProformaId = job.ProformaId,
                ProductionCustomertypeId = job.ProductionCustomertypeId,
                MachineId = job.MachineId,
                JobNo = job.JobNo,
                CustomerId = job.CustomerId,
                EstimatedBy = job.EstimatedBy,
                Code = job.Code,
                IsMaga = job.IsMaga,
                ApprovedBy = job.ApprovedBy,
                BrandName = job.BrandName,
                TinNo = job.TinNo,
                CustomerName = job.CustomerName,
                JobTypeId = job.JobTypeId,
                JT = job.JobTypeId.ToString(),
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
                IsChecked = job.IsChecked,
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

        public ActionResult ChangeOrders_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ChangeOrder> changeorders = db.ChangeOrders.Where(x => x.JobId == id);
            DataSourceResult result = changeorders.ToDataSourceResult(request, changeOrder => new {
                ChangeOrderId = changeOrder.ChangeOrderId,
                JobId = changeOrder.JobId,
                Date = changeOrder.Date,
                Reason = changeOrder.Reason,
                Quantity = changeOrder.Quantity,
                Size = changeOrder.Size,
                Price = changeOrder.Price,
                Tax = changeOrder.Tax,
                Total = changeOrder.Total,
                InvoiceNo = changeOrder.InvoiceNo,
                OrderNo = changeOrder.OrderNo,
                PreparedBy = changeOrder.PreparedBy,
                TypeAndQuantityOfMaterialNeeded = changeOrder.TypeAndQuantityOfMaterialNeeded,
                Checked = changeOrder.Checked
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Create([DataSourceRequest]DataSourceRequest request, Job job)
        {
            int.TryParse(job.JT, out int jt);
            job.JobTypeId = jt;
            //job.JobTypeId = Int16.Parse(job.JT);
            if (ModelState.IsValid)
            {
                //job.CreatedBy = User.Identity.Name;
                //job.DateCreated = DateTime.Now;
                var proforma = new Proforma();
                var customer = new OrganizationCustomer();
                if (!job.hv && !job.IsExistingCustomer)
                {
                    customer = AddNewCustomer.Add(job);
                    db.OrganizationCustomers.Add(customer);
                    db.SaveChanges();
                    Index();

                }
                else if (job.hv)
                {
                    proforma = db.Proformas.Find(job.ProformaId);

                }
                job.CustomerId = job.hv ? proforma.CustomerId : customer.OrganizationCustomerID| job.CustomerId;
                var entity = (Job)this.GetObject(job);

                db.Jobs.Add(entity);
                db.SaveChanges();
            }
                return Json(new[] { job }.ToDataSourceResult(request, ModelState));
            
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Update([DataSourceRequest]DataSourceRequest request, Job job)
        {
            job.JobTypeId = Int16.Parse(job.JT);

            if (ModelState.IsValid)
            {
                job.ModifiedBy = User.Identity.Name;
                job.LastModified = DateTime.Now;
                var proforma = new Proforma();
                var customer = new OrganizationCustomer();
                if (!job.hv && !job.IsExistingCustomer&&job.CustomerId.ToString()==null)
                {
                    customer = AddNewCustomer.Add(job);
                    db.OrganizationCustomers.Add(customer);
                    db.SaveChanges();


                }
                else if (job.hv)
                {
                    proforma = db.Proformas.Find(job.ProformaId);

                }
                job.CustomerId = job.hv ? proforma.CustomerId : customer.OrganizationCustomerID | job.CustomerId;
                var entity = (Job)this.GetObject(job);
                if(job.JobId!=0)
                {
                    db.Jobs.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                }
                if(job.JobId==0)
                {
                    db.Jobs.Add(entity);

                }

                db.SaveChanges();
            }

            return Json(new[] { job }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Jobs_Destroy([DataSourceRequest]DataSourceRequest request, Job job)
        {
            if (ModelState.IsValid)
            {

                var entity = (Job)this.GetObject(job);
                db.Jobs.Attach(entity);
                db.Jobs.Remove(entity);
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
            var jobOrder = db.Jobs.FirstOrDefault(x => x.JobId == changeOfOrder.JobId);

            if (jobOrder != null)
            {
                //jobOrder.Quant = changeOfOrder.Quantity;
                //jobOrder.FinishingSize = changeOfOrder.Size;
                //jobOrder.TotalPrice = changeOfOrder.Total;
                //jobOrder.Reason = changeOfOrder.Reason;
                //db.Entry(jobOrder).State = EntityState.Modified;
                //db.SaveChanges();

                var changeOrder = new ChangeOrder();

                changeOrder.Date = DateTime.Now;// changeOfOrder.Date;
                changeOrder.Quantity = changeOfOrder.ChangedQuantity;
                changeOrder.Size = changeOfOrder.Size;
                changeOrder.JobId = changeOfOrder.JobId;
                changeOrder.Tax = changeOfOrder.Tax;
                changeOrder.Reason = changeOfOrder.Reason;
                changeOrder.Price = changeOfOrder.Price;
                changeOrder.Total = changeOfOrder.Total;
                changeOrder.TypeAndQuantityOfMaterialNeeded = changeOfOrder.TypeAndQuantityOfMaterialNeeded;

                db.ChangeOrders.Add(changeOrder);
                db.SaveChanges();

                TempData["Success"] = "Successfully Changed";

                return RedirectToAction("Index");
            }

            TempData["Error"] = "Invalid Data ";

            return RedirectToAction("Index");
        }
        public ActionResult GetBeforeVatAndTotalPerice(string jobTypeId, int quant, double unitPrice)
        {
            var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId.ToString() == jobTypeId);
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
            var job = db.Jobs.FirstOrDefault(x => x.JobId == jobId);
            if (job != null)
            {
                var jobType = db.JobCategories.Find(job.JobTypeId);
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
        private object GetObject(Job job)
        {
            var entity = new Job
            {
                JobId = job.JobId,
                hv = job.hv,
                MachineId = job.MachineId,
                EstimatedBy = job.EstimatedBy,
                ApprovedBy = job.ApprovedBy,
                Time = job.Time,
                ProformaId = job.ProformaId,
                JobNo = job.JobNo,
                CustomerName = job.CustomerName,
                CustomerId = job.CustomerId,
                Code = job.Code,
                JobTypeId = job.JobTypeId,
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
                IsMaga = job.IsMaga,
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

        public JsonResult ApproveJobOrder(int id)
        {
            var job = db.Jobs.Find(id);
            object response = null;
            
            if (job != null)
            {
                if (job.IsChecked)
                {
                    job.ApprovedBy = User.Identity.Name;
                    job.IsApproved = true;
                    db.Jobs.Attach(job);
                    db.Entry(job).State = EntityState.Modified;
                    var proforma = db.Proformas.Find(job.ProformaId);
                    if(proforma!=null)
                    {
                        var estimation = db.EstimationForms.Find(proforma.EstimationFormId);
                        if(estimation!=null)
                        {
                            var materials = db.MaterialCosts.Where(q => q.EstimationFormId == estimation.EstimationFormId).ToList();
                            foreach(var material in materials)
                            {
                                material.JobId = job.JobId;
                                db.MaterialCosts.Attach(material);
                                db.Entry(material).State = EntityState.Modified;
                            }
                        }
                    }

                    db.SaveChanges();
                    response = new { Success = true, Message = "Job has been approved Successfully!" };
                }
                else
                {
                    response = new { Success = false, Message = "Job has not been checked!" };

                }


            }
            else
            {
                response = new { Success = false, Message = "Job is not found!" };
            }

            return Json(response);

        }
        public JsonResult CheckJobOrder(int id)
        {
            var job = db.Jobs.Find(id);
            object response = null;

            if (job != null)
            {
                job.IsChecked = true;
                job.CheckedBy = User.Identity.Name;

                db.Jobs.Attach(job);
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Job has been checked Successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Job is not found!" };
            }

            return Json(response);

        }
        public JsonResult RejectJobOrder(int id)
        {
            var job = db.Jobs.Find(id);
            object response = null;

            if (job != null)
            {
                job.IsChecked = false;
                db.Jobs.Attach(job);
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Job has been rejected Successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Job is not found!" };
            }

            return Json(response);

        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
