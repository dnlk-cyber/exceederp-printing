//﻿using System;
//using System.Collections.Generic;

//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
//using ExceedERP.DataAccess.Context;
//using ExceedERP.Web.Helpers;

//namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
//{
//    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpProformaUser)]
//    public class FurnitureCostEstimationsController : Controller
//    {
//        private ExceedDbContext db = new ExceedDbContext();

//        public ActionResult Index()
//        {

//            // ViewData["EstimationForms"] = db.EstimationForms.ToList();
//            ViewData["JobTypes"] = db.JobCategories.Select(s => new {
//                JobTypeId = s.JobCategoryId,
//                Type = s.HaveParent ? db.JobCategories.Where(q => q.JobCategoryId == s.ParentId).FirstOrDefault().JobName + "-" + s.JobName : s.JobName
//            }).ToList();
//            var selecteditems = db.InventoryItems.Select(
//               i => new
//               {
//                   Name = i.ItemCode + "_" + i.Name,
//                   Code = i.ItemCode
//               }).ToList();
//            var categories = db.ItemCategorys.Select(
//               i => new
//               {
//                   Name = i.ItemCategoryCode + "_" + i.Name,
//                   Code = i.ItemCategoryCode
//               }).ToList();
//            ViewData["Categorys"] = categories;
//            var myCustomers = db.OrganizationCustomers.Select(
//                s => new {
//                    Code = s.OrganizationCustomerID,
//                    Name = s.TradeName
//                }).ToList();
//            ViewData["Customers"] = myCustomers;

//            //IList<Inventory> inventories = db.Inventories.ToList();
//            ViewData["Inventories"] = selecteditems;
//            ViewData["Category"] = db.FurnitureCategories.Select(x => new { x.FurnitureCategoryId, x.Name }).ToList();
//            ViewBag.data = GetData();
//            return View();
//        }
//        //public IList<OrganizationCustomer> getCustomers(Proforma proforma=null)
//        //{
//        //    var customers = db.OrganizationCustomers.ToList();
//        //    if(proforma != null&& !proforma.HaveEstimation && !proforma.IsExistingCustomer)
//        //    {
//        //        var entity = new OrganizationCustomer()
//        //        {
//        //            Code = proforma.CustomerId,
//        //            BrandName = proforma.BrandName,
//        //            NAme = proforma.CustomerName
//        //        };
//        //        customers.Add(entity);
//        //    }

//        //    ViewData["Customers"] = customers;
//        //    return customers;
//        //}
//        public JsonResult GetAllProformas()
//        {
//            var data = db.Proformas;
//            return Json(data.Select(s => new { s.ProformaId, s.ProformaNo }), JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult FurnitureCostEstimations_Read([DataSourceRequest]DataSourceRequest request)
//        {
//            IQueryable<FurnitureCostEstimation> furnitureCostEstimations = db.FurnitureCostEstimations;
//            DataSourceResult result = furnitureCostEstimations.ToDataSourceResult(request, furnitureCostEstimation => new {
//                FurnitureCostEstimationId = furnitureCostEstimation.FurnitureCostEstimationId,
//                JobTypeId = furnitureCostEstimation.JobTypeId,
//                JT = furnitureCostEstimation.JT,
//                Others = furnitureCostEstimation.Others,
//                Quantity = furnitureCostEstimation.Quantity,
//                CustomerId = furnitureCostEstimation.CustomerId,
//                JobNumber = furnitureCostEstimation.JobNumber,
//                Date = furnitureCostEstimation.Date,
//                Description = furnitureCostEstimation.Description,
//                DateCreated = furnitureCostEstimation.DateCreated,
//                LastModified = furnitureCostEstimation.LastModified,
//                ModifiedBy = furnitureCostEstimation.ModifiedBy,
//                CreatedBy = furnitureCostEstimation.CreatedBy,
//                ApprovedBy = furnitureCostEstimation.ApprovedBy,
//                CheckedBy = furnitureCostEstimation.CheckedBy,
//            });

//            return Json(result);
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult FurnitureCostEstimations_Create([DataSourceRequest]DataSourceRequest request, FurnitureCostEstimation furnitureCostEstimation)
//        {
//            if (ModelState.IsValid)
//            {

//                var entity = GetMappedObject(furnitureCostEstimation);
//                db.FurnitureCostEstimations.Add(entity);
//                db.SaveChanges();
//                furnitureCostEstimation.FurnitureCostEstimationId = entity.FurnitureCostEstimationId;
               
               
//            }

//           return Json(new[] { furnitureCostEstimation }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult FurnitureCostEstimations_Update([DataSourceRequest]DataSourceRequest request, FurnitureCostEstimation furnitureCostEstimation)
//        {
//            if (ModelState.IsValid)
//            {


//                var entity = GetMappedObject(furnitureCostEstimation);
//                db.FurnitureCostEstimations.Attach(entity);
//                db.Entry(entity).State = EntityState.Modified;
//                db.SaveChanges();
//            }

//            return Json(new[] { furnitureCostEstimation }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult FurnitureCostEstimations_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureCostEstimation furnitureCostEstimation)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = GetMappedObject(furnitureCostEstimation);

//                db.FurnitureCostEstimations.Attach(entity);
//                db.FurnitureCostEstimations.Remove(entity);
//                db.SaveChanges();
//            }

//            return Json(new[] { furnitureCostEstimation }.ToDataSourceResult(request, ModelState));
//        }
//        private FurnitureCostEstimation GetMappedObject(FurnitureCostEstimation furnitureCostEstimation)
//        {
//            var entity = new FurnitureCostEstimation
//            {
//                FurnitureCostEstimationId = furnitureCostEstimation.FurnitureCostEstimationId,
//                JobTypeId = furnitureCostEstimation.JobTypeId,
//                JT = furnitureCostEstimation.JT,
//                Others = furnitureCostEstimation.Others,
//                Quantity = furnitureCostEstimation.Quantity,
//                CustomerId = furnitureCostEstimation.CustomerId,
//                Date = furnitureCostEstimation.Date,
//                DateCreated = furnitureCostEstimation.DateCreated,
//                Description = furnitureCostEstimation.Description,
//                LastModified = furnitureCostEstimation.LastModified,
//                ModifiedBy = furnitureCostEstimation.ModifiedBy,
//                CreatedBy = furnitureCostEstimation.CreatedBy,
//                ApprovedBy = furnitureCostEstimation.ApprovedBy,
//                CheckedBy = furnitureCostEstimation.CheckedBy,
//            };
//            return entity;
//        }
//        public JsonResult CheckProforma(int id)
//        {
//            var proforma = db.Proformas.Find(id);
//            object response = null;

//            if (proforma != null)
//            {
//                proforma.IsChecked = true;
//                proforma.CheckedBy = User.Identity.Name;

//                db.Proformas.Attach(proforma);
//                db.Entry(proforma).State = EntityState.Modified;
//                db.SaveChanges();
//                response = new { Success = true, Message = "Proforma has been checked Successfully!" };

//            }
//            else
//            {
//                response = new { Success = false, Message = "Proforma not found!" };
//            }

//            return Json(response);

//        }

//        public JsonResult RejectProforma(int id)
//        {
//            var proforma = db.Proformas.Find(id);
//            object response = null;

//            if (proforma != null)
//            {
//                proforma.IsChecked = false;
//                db.Proformas.Attach(proforma);
//                db.Entry(proforma).State = EntityState.Modified;
//                db.SaveChanges();
//                response = new { Success = true, Message = "Proforma has been rejected Successfully!" };

//            }
//            else
//            {
//                response = new { Success = false, Message = "Proforma not found!" };
//            }

//            return Json(response);

//        }

//        public JsonResult ApproveProforma(int id)
//        {
//            var proforma = db.Proformas.Find(id);
//            object response = null;
//            if (proforma != null)
//            {
//                if (proforma.IsChecked)
//                {
//                    var proformaItem = db.PFProformaItems.Where(q => q.ProformaId == proforma.ProformaId).FirstOrDefault();
//                    var estimation = db.EstimationForms.Find(proforma.EstimationFormId);
//                    proforma.IsApproved = true;
//                    proforma.ApprovedBy = User.Identity.Name;
//                    var oc = db.OverallCosts.Where(q => q.EstimationFormId == proforma.EstimationFormId).FirstOrDefault();
//                    db.Proformas.Attach(proforma);
//                    db.Entry(proforma).State = EntityState.Modified;
//                    //add new proforma 
//                    var jobOrder = new Job
//                    {
                        
//                        hv = true,
//                        CustomerId = proforma.CustomerId,
//                        IsExistingCustomer = true,
//                        JobNo = proforma.ProformaId.ToString(),
//                        JobTypeId = (int)proformaItem?.JobTypeId,
//                        JT = proformaItem?.JobTypeId.ToString(),
//                        ProformaId = proforma.ProformaId,
//                        Quantity = (decimal)proformaItem?.Quantity,
//                        UnitPrice = (double)proformaItem?.UnitPrice,
//                        BeforeTax = (double)proformaItem?.BeforeTax,
//                        TotalPrice = (decimal)proformaItem?.TotalPrice,
//                        FinishingSize = proformaItem?.Size,
//                        Copies = 0,
//                        IsMaga = true,
//                        TotalImpression = 0,
//                        Pages = 0,
//                        Status = ProductionJobOrderStatus.Normal,
//                        Time = DateTime.Now,
//                        Colorcover = estimation?.CoverPrint,
//                        ColorText = estimation?.TextPrint,
//                        TextWeight = proformaItem?.Size,
//                        CreatedBy = User.Identity.Name,
//                        ReceivedDate = DateTime.Now,
//                        DateCreated = DateTime.Now,
//                        ModifiedBy = User.Identity.Name,
//                        LastModified = DateTime.Now,
//                    };
//                    var entity = (Job)this.GetObject(jobOrder);

//                    db.Jobs.Add(entity);
//                    db.SaveChanges();
//                    response = new { Success = true, Message = "Proforma has been approved Successfully!" };
//                }
//                else
//                {
//                    response = new { Success = false, Message = "Proforma not found!" };
//                }


//            }
//            else
//            {
//                response = new { Success = false, Message = "Proforma not found!" };

//            }

//            return Json(response);
//        }
//        private object GetObject(Job job)
//        {
//            var entity = new Job
//            {
//                JobId = job.JobId,
//                hv = job.hv,
//                MachineId = job.MachineId,
//                EstimatedBy = job.EstimatedBy,
//                ApprovedBy = job.ApprovedBy,
//                Time = job.Time,
//                ProformaId = job.ProformaId,
//                JobNo = job.JobNo,
//                CustomerName = job.CustomerName,
//                CustomerId = job.CustomerId,
//                Code = job.Code,
//                JobTypeId = job.JobTypeId,
//                IsMaga = job.IsMaga,
//                BeforeTax = job.BeforeTax,
//                UnitPrice = job.UnitPrice,
//                Quantity = job.Quantity,
//                ContactPhone = job.ContactPhone,
//                Pages = job.Pages,
//                Copies = job.Copies,
//                Status = job.Status,
//                ProductionCustomertypeId = job.ProductionCustomertypeId,
//                TotalPrice = job.TotalPrice,
//                Contract = job.Contract,
//                TotalImpression = job.TotalImpression,
//                DeliveryDate = job.DeliveryDate,
//                ReceivedDate = job.ReceivedDate,
//                TextWeight = job.TextWeight,
//                CoverWeight = job.CoverWeight,
//                ColorText = job.ColorText,
//                TinNo = job.TinNo,
//                BrandName = job.BrandName,
//                Colorcover = job.Colorcover,
//                FinishingSize = job.FinishingSize,
//                PrintingLine = job.PrintingLine,
//                PrintingMachine = job.PrintingMachine,
//                Sewing = job.Sewing,
//                New = job.New,
//                PerfectBinding = job.PerfectBinding,
//                Repeat = job.Repeat,
//                UnitId = job.UnitId,
//                Lamnating = job.Lamnating,
//                Varnish = job.Varnish,
//                Perforate = job.Perforate,
//                Camsur = job.Camsur,
//                GraphicDesignExpectedCompletetion = job.GraphicDesignExpectedCompletetion,
//                PrintingExpectedCompletetion = job.PrintingExpectedCompletetion,
//                FinishExpectedCompletetion = job.FinishExpectedCompletetion, 
//                CreatedBy = job.CreatedBy,
//                DateCreated = job.DateCreated,
//                ModifiedBy = job.ModifiedBy,
//                LastModified = job.LastModified
//            };
//            return entity;
//        }

//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//        private IEnumerable<DropDownTreeItemModel> GetData()
//        {
//            var jobCategories = db.JobCategories.Where(q => q.HaveParent == false).ToList();
//            List<DropDownTreeItemModel> jobs = new List<DropDownTreeItemModel>();
//            foreach (var jobCategory in jobCategories)
//            {
//                int i = 0;
//                DropDownTreeItemModel jobType = new DropDownTreeItemModel();
//                List<DropDownTreeItemModel> jobTypes = new List<DropDownTreeItemModel>();

//                var childs = db.JobCategories.Where(q => q.ParentId == jobCategory.JobCategoryId).ToList();
//                foreach (var child in childs)
//                {
//                    i++;
//                    DropDownTreeItemModel prt = new DropDownTreeItemModel();
//                    prt.Text = child.JobName;
//                    prt.HasChildren = false;
//                    prt.Value = child.JobCategoryId.ToString();
//                    jobTypes.Add(prt);
//                }
//                jobType.Text = jobCategory.JobName;

//                jobType.Value = jobCategory.JobCategoryId.ToString();
//                jobType.Items = jobTypes;
//                jobs.Add(jobType);


//            }



//            return jobs;
//        }

//    }
//}
