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

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpProformaUser)]
    public class FurnitureEstimationProformaController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            ViewData["EstimationForms"] = db.EstimationForms.ToList();
            var jobCategories = db.JobCategories.Select(s => new {
                Code = s.JobCategoryId,
                Name = s.HaveParent ? s.ParentId + " " + s.JobName : s.JobName
            }).ToList();
            ViewData["JobTypes"] = jobCategories;
            ViewBag.data = GetData();
            var customers = db.OrganizationCustomers.Select(s => new
            {
                Code = s.OrganizationCustomerID,
                Name = s.TradeName
            }).ToList();
            ViewData["Customers"] = customers;

            return View();
        }
        //public IList<OrganizationCustomer> getCustomers(Proforma proforma=null)
        //{
        //    var customers = db.OrganizationCustomers.ToList();
        //    if(proforma != null&& !proforma.HaveEstimation && !proforma.IsExistingCustomer)
        //    {
        //        var entity = new OrganizationCustomer()
        //        {
        //            Code = proforma.CustomerId,
        //            BrandName = proforma.BrandName,
        //            NAme = proforma.CustomerName
        //        };
        //        customers.Add(entity);
        //    }

        //    ViewData["Customers"] = customers;
        //    return customers;
        //}
        public JsonResult GetAllProformas()
        {
            var data = db.Proformas;
            return Json(data.Select(s => new { s.ProformaId, s.ProformaNo }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Proformas_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureProforma> proformas = db.FurnitureProformas;
            DataSourceResult result = proformas.ToDataSourceResult(request, proforma => new {
                FurnitureProformaId = proforma.FurnitureProformaId,
                Time = proforma.Time,
                EstimationFormId = proforma.EstimationFormId,
                CustomerId = proforma.CustomerId,
                HaveEstimation = proforma.HaveEstimation,
                IsExistingCustomer = proforma.IsExistingCustomer,
                InvoiceDate = proforma.InvoiceDate,
                DeliveryDate = proforma.DeliveryDate,
                ValidityDate = proforma.ValidityDate,
                PaymentTerm = proforma.PaymentTerm,
                InvoiceNo = proforma.InvoiceNo,
                CustomerName = proforma.CustomerName,
                BrandName = proforma.BrandName,
                ProformaNo = proforma.ProformaNo,
                TinNo = proforma.TinNo,
                IsApproved = proforma.IsApproved,
                IsChecked = proforma.IsChecked
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Proformas_Create([DataSourceRequest]DataSourceRequest request, FurnitureProforma proforma)
        {
            if (ModelState.IsValid)
            {
                var estimationForm = new FurnitureEstimationForm();
                var customer = new OrganizationCustomer();
                if (!proforma.HaveEstimation && !proforma.IsExistingCustomer)
                {
                    customer = AddNewCustomer.Save(proforma);
                    db.OrganizationCustomers.Add(customer);
                    db.SaveChanges();
                }
                else if (proforma.HaveEstimation)
                {
                    estimationForm = db.FurnitureEstimationForms.Find(proforma.EstimationFormId);

                }
                var entity = new FurnitureProforma
                {
                    Time = proforma.Time,
                    HaveEstimation = proforma.HaveEstimation,
                    IsExistingCustomer = proforma.IsExistingCustomer,
                    CustomerId = proforma.HaveEstimation ? estimationForm.CustomerId : customer.OrganizationCustomerID | proforma.CustomerId,
                    EstimationFormId = proforma.EstimationFormId,
                    InvoiceDate = proforma.InvoiceDate,
                    DeliveryDate = proforma.DeliveryDate,
                    ValidityDate = proforma.ValidityDate,
                    PaymentTerm = proforma.PaymentTerm,
                    InvoiceNo = proforma.InvoiceNo,
                    CustomerName = proforma.CustomerName,
                    BrandName = proforma.BrandName,
                    ProformaNo = proforma.ProformaNo,
                    TinNo = proforma.TinNo

                };
                db.FurnitureProformas.Add(entity);
                db.SaveChanges();
                proforma.FurnitureProformaId = entity.FurnitureProformaId;
                proforma.CustomerId = entity.CustomerId;
                //Index();

                //  return RedirectToAction("Index", "Proforma", new { area = "ProductionFollowUp" });

                #region

                if (proforma.FurnitureProformaId.ToString().Length == 1)
                {
                    entity.ProformaNo = "pro-000000" + proforma.FurnitureProformaId;
                }
                else if (proforma.FurnitureProformaId.ToString().Length == 2)
                {
                    entity.ProformaNo = "pro-00000" + proforma.FurnitureProformaId;
                }
                else if (proforma.FurnitureProformaId.ToString().Length == 3)
                {
                    entity.ProformaNo = "pro-0000" + proforma.FurnitureProformaId;
                }
                else if (proforma.FurnitureProformaId.ToString().Length == 4)
                {
                    entity.ProformaNo = "pro-000" + proforma.FurnitureProformaId;
                }
                else if (proforma.FurnitureProformaId.ToString().Length == 5)
                {
                    entity.ProformaNo = "pro-00" + proforma.FurnitureProformaId;
                }
                else if (proforma.FurnitureProformaId.ToString().Length == 2)
                {
                    entity.ProformaNo = "pro-0" + proforma.FurnitureProformaId;
                }
                else if (proforma.FurnitureProformaId.ToString().Length == 2)
                {
                    entity.ProformaNo = "pro-" + proforma.FurnitureProformaId;
                }

                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #endregion
            }

            return Json(new[] { proforma }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Proformas_Update([DataSourceRequest]DataSourceRequest request, FurnitureProforma proforma)
        {
            if (ModelState.IsValid)
            {
                var estimationForm = new FurnitureEstimationForm();
                var customer = new OrganizationCustomer();
                if (!proforma.HaveEstimation && !proforma.IsExistingCustomer && proforma.CustomerId.ToString() == null)
                {
                    customer = AddNewCustomer.Save(proforma);
                    db.OrganizationCustomers.Add(customer);

                }
                else if (proforma.HaveEstimation)
                {
                    estimationForm = db.FurnitureEstimationForms.Find(proforma.EstimationFormId);

                }

                var entity = new FurnitureProforma
                {
                    FurnitureProformaId = proforma.FurnitureProformaId,
                    Time = proforma.Time,
                    HaveEstimation = proforma.HaveEstimation,
                    IsExistingCustomer = proforma.IsExistingCustomer,
                    CustomerId = proforma.HaveEstimation ? estimationForm.CustomerId : customer.OrganizationCustomerID | proforma.CustomerId,
                    EstimationFormId = proforma.EstimationFormId,
                    InvoiceDate = proforma.InvoiceDate,
                    DeliveryDate = proforma.DeliveryDate,
                    ValidityDate = proforma.ValidityDate,
                    PaymentTerm = proforma.PaymentTerm,
                    InvoiceNo = proforma.InvoiceNo,
                    BrandName = proforma.BrandName,
                    CustomerName = proforma.CustomerName,
                    ProformaNo = proforma.ProformaNo,
                    TinNo = proforma.TinNo
                };
                db.FurnitureProformas.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Proforma", new { area = "ProductionFollowUp" });

            return Json(new[] { proforma }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureProformas_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProforma proforma)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProforma
                {
                    FurnitureProformaId = proforma.FurnitureProformaId,
                    Time = proforma.Time,
                    HaveEstimation = proforma.HaveEstimation,
                    IsExistingCustomer = proforma.IsExistingCustomer,
                    CustomerId = proforma.CustomerId,
                    EstimationFormId = proforma.EstimationFormId,
                    InvoiceDate = proforma.InvoiceDate,
                    DeliveryDate = proforma.DeliveryDate,
                    ValidityDate = proforma.ValidityDate,
                    PaymentTerm = proforma.PaymentTerm,
                    InvoiceNo = proforma.InvoiceNo,
                    ProformaNo = proforma.ProformaNo,
                };

                db.FurnitureProformas.Attach(entity);
                db.FurnitureProformas.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { proforma }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult CheckProforma(int id)
        {
            var proforma = db.Proformas.Find(id);
            object response = null;

            if (proforma != null)
            {
                proforma.IsChecked = true;
                proforma.CheckedBy = User.Identity.Name;

                db.Proformas.Attach(proforma);
                db.Entry(proforma).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Proforma has been checked Successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Proforma not found!" };
            }

            return Json(response);

        }

        public JsonResult RejectProforma(int id)
        {
            var proforma = db.Proformas.Find(id);
            object response = null;

            if (proforma != null)
            {
                proforma.IsChecked = false;
                db.Proformas.Attach(proforma);
                db.Entry(proforma).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Proforma has been rejected Successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Proforma not found!" };
            }

            return Json(response);

        }

        public JsonResult ApproveProforma(int id)
        {
            var proforma = db.Proformas.Find(id);
            object response = null;
            if (proforma != null)
            {
                if (proforma.IsChecked)
                {
                    var proformaItem = db.PFProformaItems.Where(q => q.ProformaId == proforma.ProformaId).FirstOrDefault();
                    var estimation = db.EstimationForms.Find(proforma.EstimationFormId);
                    proforma.IsApproved = true;
                    proforma.ApprovedBy = User.Identity.Name;
                    var oc = db.OverallCosts.Where(q => q.EstimationFormId == proforma.EstimationFormId).FirstOrDefault();
                    db.Proformas.Attach(proforma);
                    db.Entry(proforma).State = EntityState.Modified;
                    //add new proforma 
                    var jobOrder = new FurnitureJob
                    {

                        hv = true,
                        FurnitureCustomerId = proforma.CustomerId,
                        IsExistingCustomer = true,
                        JobNo = proforma.ProformaId.ToString(),
                        FurnitureJobTypeId = (int)proformaItem?.JobTypeId,
                        JT = proformaItem?.JobTypeId.ToString(),
                        FurnitureProformaId = proforma.ProformaId,
                        Quantity = (decimal)proformaItem?.Quantity,
                        UnitPrice = (double)proformaItem?.UnitPrice,
                        BeforeTax = (double)proformaItem?.BeforeTax,
                        TotalPrice = (decimal)proformaItem?.TotalPrice,
                        FinishingSize = proformaItem?.Size,
                        Copies = 0,
                        IsMaga = true,
                        TotalImpression = 0,
                        Pages = 0,
                        Status = FurnitureProductionJobOrderStatus.Normal,
                        Time = DateTime.Now,
                        Colorcover = estimation?.CoverPrint,
                        ColorText = estimation?.TextPrint,
                        TextWeight = proformaItem?.Size,
                        CreatedBy = User.Identity.Name,
                        ReceivedDate = DateTime.Now,
                        DateCreated = DateTime.Now,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now,
                    };
                    var entity = (FurnitureJob)this.GetObject(jobOrder);

                    db.FurnitureJobs.Add(entity);
                    db.SaveChanges();
                    response = new { Success = true, Message = "Proforma has been approved Successfully!" };
                }
                else
                {
                    response = new { Success = false, Message = "Proforma not found!" };
                }


            }
            else
            {
                response = new { Success = false, Message = "Proforma not found!" };

            }

            return Json(response);
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
                CustomerName = job.CustomerName,
                FurnitureCustomerId = job.FurnitureCustomerId,
                Code = job.Code,
                FurnitureJobTypeId = job.FurnitureJobTypeId,
                IsMaga = job.IsMaga,
                BeforeTax = job.BeforeTax,
                UnitPrice = job.UnitPrice,
                Quantity = job.Quantity,
                ContactPhone = job.ContactPhone,
                Pages = job.Pages,
                Copies = job.Copies,
                Status = job.Status,
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
                CreatedBy = job.CreatedBy,
                DateCreated = job.DateCreated,
                ModifiedBy = job.ModifiedBy,
                LastModified = job.LastModified
            };
            return entity;
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
