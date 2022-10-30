using System;
using System.Collections.Generic;
////using System.Data
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMQueueApprover, CRMRoles.CRMQueueUser)]
    public class QueueCustomerController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["InvestmentTypes"] = db.InvestmentTypes.ToList();
            return View();
        }

        public ActionResult QueueCustomer_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<QueueCustomer> queueCustomers = db.QueueCustomer;
            List<QueueCustomer> listToReturn = new List<QueueCustomer>();
            foreach(QueueCustomer customer in queueCustomers)
            {
                if (HasActiveTransactions(customer))
                {
                    customer.HasTransactions = true;
                }
                listToReturn.Add(customer);
            }
            //DataSourceResult result = queuecustomer.ToDataSourceResult(request, queueCustomer => new {
            //    QueueCustomerId = queueCustomer.QueueCustomerId,
            //    CustomerName = queueCustomer.CustomerName,
            //    TinNumber = queueCustomer.TinNumber,
            //    Phone = queueCustomer.Phone,
            //    InvestmentTypeId = queueCustomer.InvestmentTypeId,
            //    InvestmentAmount = queueCustomer.InvestmentAmount,
            //    Grade = queueCustomer.Grade,
            //    OrgBranchName = queueCustomer.OrgBranchName,
            //    DateCreated = queueCustomer.DateCreated,
            //    LastModified = queueCustomer.LastModified,
            //    CreatedBy = queueCustomer.CreatedBy,
            //    ModifiedBy = queueCustomer.ModifiedBy
            //});

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request));
        }
        private bool ValidateCustomer(QueueCustomer customer)
        {
            bool isValid = false;
            bool isDuplicatePhone = false;
            bool isDuplicateTin = false;
            bool isDuplicateName = false;
            var customerWithTin = db.QueueCustomer.FirstOrDefault(c => c.TinNumber == customer.TinNumber && c.QueueCustomerId != customer.QueueCustomerId);
            if(customerWithTin != null)
            {
                ModelState.AddModelError("", "Duplicate TIN not allowed");
                isDuplicateTin = true;
            }
            var customerWithPhone = db.QueueCustomer.FirstOrDefault(c => c.Phone == customer.Phone && c.QueueCustomerId != customer.QueueCustomerId);
            if (customerWithPhone != null)
            {
                ModelState.AddModelError("", "Duplicate Phone not allowed");
                isDuplicatePhone = true;
            }
            var customerWithName = db.QueueCustomer.FirstOrDefault(c => c.CustomerName == customer.CustomerName && c.QueueCustomerId != customer.QueueCustomerId);
            if (customerWithName != null)
            {
                ModelState.AddModelError("", "Duplicate Customer Name not allowed");
                isDuplicateName = true;
            }
            if(!isDuplicateName && !isDuplicatePhone && !isDuplicateTin)
            {
                isValid = true;
            }
            return isValid;

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QueueCustomer_Create([DataSourceRequest]DataSourceRequest request, QueueCustomer queueCustomer)
        {
            if (ModelState.IsValid)
            {
                var valid = ValidateCustomer(queueCustomer);
                if (valid)
                {
                    var entity = new QueueCustomer
                    {
                        CustomerName = queueCustomer.CustomerName,
                        TinNumber = queueCustomer.TinNumber,
                        Phone = queueCustomer.Phone,
                        InvestmentTypeId = queueCustomer.InvestmentTypeId,
                        InvestmentAmount = queueCustomer.InvestmentAmount,
                        Grade = queueCustomer.Grade,
                        OrgBranchName = queueCustomer.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = queueCustomer.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = queueCustomer.ModifiedBy
                    };

                    db.QueueCustomer.Add(entity);
                    db.SaveChanges();
                    queueCustomer.QueueCustomerId = entity.QueueCustomerId;
                    queueCustomer.DateCreated = entity.DateCreated;
                }
            }
            return Json(new[] { queueCustomer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QueueCustomer_Update([DataSourceRequest]DataSourceRequest request, QueueCustomer queueCustomer)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueCustomer
                {
                    QueueCustomerId = queueCustomer.QueueCustomerId,
                    CustomerName = queueCustomer.CustomerName,
                    TinNumber = queueCustomer.TinNumber,
                    Phone = queueCustomer.Phone,
                    InvestmentTypeId = queueCustomer.InvestmentTypeId,
                    InvestmentAmount = queueCustomer.InvestmentAmount,
                    Grade = queueCustomer.Grade,
                    OrgBranchName = queueCustomer.OrgBranchName,
                    DateCreated = queueCustomer.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = queueCustomer.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.QueueCustomer.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { queueCustomer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QueueCustomer_Destroy([DataSourceRequest]DataSourceRequest request, QueueCustomer queueCustomer)
        {
            if (ModelState.IsValid)
            {
                var entity = new QueueCustomer
                {
                    QueueCustomerId = queueCustomer.QueueCustomerId,
                    CustomerName = queueCustomer.CustomerName,
                    TinNumber = queueCustomer.TinNumber,
                    Phone = queueCustomer.Phone,
                    InvestmentTypeId = queueCustomer.InvestmentTypeId,
                    InvestmentAmount = queueCustomer.InvestmentAmount,
                    Grade = queueCustomer.Grade,
                    OrgBranchName = queueCustomer.OrgBranchName,
                    DateCreated = queueCustomer.DateCreated,
                    LastModified = queueCustomer.LastModified,
                    CreatedBy = queueCustomer.CreatedBy,
                    ModifiedBy = queueCustomer.ModifiedBy
                };

                db.QueueCustomer.Attach(entity);
                db.QueueCustomer.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { queueCustomer }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        private bool HasActiveTransactions(QueueCustomer customer)
        {
            var qRec = db.Queue.Where(c => c.QueueCustomerId == customer.QueueCustomerId).ToList();
            var qStRec = db.QueueStatus.Where(c => c.Queue.QueueCustomerId == customer.QueueCustomerId).ToList();

            return (qRec.Any() || qStRec.Any());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
