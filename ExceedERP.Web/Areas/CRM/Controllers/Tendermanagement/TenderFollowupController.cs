using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.CRM.TenderManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	[AuthorizeRoles(CRMRoles.CRMTenderFollowup)] 
    public class TenderFollowupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["OrganizationCustomers"] = db.OrganizationCustomers.Select(o => new { o.TradeName, o.OrganizationCustomerID }).ToList();
            ViewData["ItemCategories"] = db.ItemCategorys.Select(c => new { c.Name, c.ItemCategoryCode}).ToList();
            ViewData["InventoryItems"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name }).ToList();

            return View();
        }

        public ActionResult Data_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<TenderFollowup> tenderFollowups = db.TenderFollowup.OrderByDescending(d => d.TenderFollowupId).ToList();
            foreach (TenderFollowup tender in tenderFollowups)
            {
                var days = (tender.ClosingDate.Date - DateTime.Now.Date).Days;
                if (days <= 0)
                {
                    tender.CountDownTillSubmission = "Submission Date Passed";
                }
                else if(days == 1)
                {
                    tender.CountDownTillSubmission = $"{days} day";
                }
                else
                {
                    tender.CountDownTillSubmission = $"{days} days";
                }
            }
            DataSourceResult result = tenderFollowups.ToDataSourceResult(request, tenderFollowup => new
            {
                TenderFollowupId = tenderFollowup.TenderFollowupId,
                CustomerId = tenderFollowup.CustomerId,
                BidBondAmount = tenderFollowup.BidBondAmount,
                CountDownTillSubmission = tenderFollowup.CountDownTillSubmission,
                DateAnnounceBidValidatiy = tenderFollowup.DateAnnounceBidValidatiy,
                ServiceCharge = tenderFollowup.ServiceCharge,
                ClosingDate = tenderFollowup.ClosingDate,
                OpeningDate = tenderFollowup.OpeningDate,
                Location = tenderFollowup.Location,
                DocumentPreparationStartDate = tenderFollowup.DocumentPreparationStartDate,
                Status = tenderFollowup.Status,
                TenderResult = tenderFollowup.TenderResult,
                LatestLetter = tenderFollowup.LatestLetter,
                ContactPerson = tenderFollowup.ContactPerson,
                Remark = tenderFollowup.Remark,
                BidAmount = tenderFollowup.BidAmount,
                BidSecurityCharge = tenderFollowup.BidSecurityCharge,
                BidValidty = tenderFollowup.BidValidty,
                ReasonOfNotParticipating = tenderFollowup.ReasonOfNotParticipating,
                ReasonOfLosing = tenderFollowup.ReasonOfLosing,


                CreatedBy = tenderFollowup.CreatedBy,
                DateCreated = tenderFollowup.DateCreated,
                LastModified = tenderFollowup.LastModified,
                ModifiedBy = tenderFollowup.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Create([DataSourceRequest]DataSourceRequest request, TenderFollowup tenderFollowup)
        {

            if (ModelState.IsValid)
            {
                var entity = new TenderFollowup
                {
                    TenderFollowupId = tenderFollowup.TenderFollowupId,
                    CustomerId = tenderFollowup.CustomerId,
                    BidBondAmount = tenderFollowup.BidBondAmount,
                    DateAnnounceBidValidatiy = tenderFollowup.DateAnnounceBidValidatiy,
                    ServiceCharge = tenderFollowup.ServiceCharge,
                    ClosingDate = tenderFollowup.ClosingDate,
                    OpeningDate = tenderFollowup.OpeningDate,
                    Location = tenderFollowup.Location,
                    DocumentPreparationStartDate = tenderFollowup.DocumentPreparationStartDate,
                    Status = tenderFollowup.Status,
                    TenderResult = tenderFollowup.TenderResult,
                    LatestLetter = tenderFollowup.LatestLetter,
                    ContactPerson = tenderFollowup.ContactPerson,
                    Remark = tenderFollowup.Remark,
                    BidAmount = tenderFollowup.BidAmount,
                    BidSecurityCharge = tenderFollowup.BidSecurityCharge,
                    BidValidty = tenderFollowup.BidValidty,
                    ReasonOfNotParticipating = tenderFollowup.ReasonOfNotParticipating,
                    ReasonOfLosing = tenderFollowup.ReasonOfLosing,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                };

                db.TenderFollowup.Add(entity);
                db.SaveChanges();
                tenderFollowup.TenderFollowupId = entity.CustomerId;
            }

            return Json(new[] { tenderFollowup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Update([DataSourceRequest]DataSourceRequest request, TenderFollowup tenderFollowup)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderFollowup
                {
                    TenderFollowupId = tenderFollowup.TenderFollowupId,
                    CustomerId = tenderFollowup.CustomerId,
                    BidBondAmount = tenderFollowup.BidBondAmount,
                    DateAnnounceBidValidatiy = tenderFollowup.DateAnnounceBidValidatiy,
                    ServiceCharge = tenderFollowup.ServiceCharge,
                    ClosingDate = tenderFollowup.ClosingDate,
                    OpeningDate = tenderFollowup.OpeningDate,
                    Location = tenderFollowup.Location,
                    DocumentPreparationStartDate = tenderFollowup.DocumentPreparationStartDate,
                    Status = tenderFollowup.Status,
                    TenderResult = tenderFollowup.TenderResult,
                    LatestLetter = tenderFollowup.LatestLetter,
                    ContactPerson = tenderFollowup.ContactPerson,
                    Remark = tenderFollowup.Remark,
                    BidAmount = tenderFollowup.BidAmount,
                    BidSecurityCharge = tenderFollowup.BidSecurityCharge,
                    BidValidty = tenderFollowup.BidValidty,
                    ReasonOfNotParticipating = tenderFollowup.ReasonOfNotParticipating,
                    ReasonOfLosing = tenderFollowup.ReasonOfLosing,

                    CreatedBy = tenderFollowup.CreatedBy,
                    DateCreated = tenderFollowup.DateCreated,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.TenderFollowup.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { tenderFollowup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Destroy([DataSourceRequest]DataSourceRequest request, TenderFollowup tenderFollowup)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderFollowup
                {
                    TenderFollowupId = tenderFollowup.TenderFollowupId,
                    CustomerId = tenderFollowup.CustomerId,
                    BidBondAmount = tenderFollowup.BidBondAmount,
                    DateAnnounceBidValidatiy = tenderFollowup.DateAnnounceBidValidatiy,
                    ServiceCharge = tenderFollowup.ServiceCharge,
                    ClosingDate = tenderFollowup.ClosingDate,
                    OpeningDate = tenderFollowup.OpeningDate,
                    Location = tenderFollowup.Location,
                    DocumentPreparationStartDate = tenderFollowup.DocumentPreparationStartDate,
                    Status = tenderFollowup.Status,
                    TenderResult = tenderFollowup.TenderResult,
                    LatestLetter = tenderFollowup.LatestLetter,
                    ContactPerson = tenderFollowup.ContactPerson,
                    Remark = tenderFollowup.Remark,
                    BidAmount = tenderFollowup.BidAmount,
                    BidSecurityCharge = tenderFollowup.BidSecurityCharge,
                    BidValidty = tenderFollowup.BidValidty,
                    ReasonOfNotParticipating = tenderFollowup.ReasonOfNotParticipating,
                    ReasonOfLosing = tenderFollowup.ReasonOfLosing,


                    CreatedBy = tenderFollowup.CreatedBy,
                    DateCreated = tenderFollowup.DateCreated,
                    LastModified = tenderFollowup.LastModified,
                    ModifiedBy = tenderFollowup.ModifiedBy,
                };

                db.TenderFollowup.Attach(entity);
                db.TenderFollowup.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Tender followup  " + tenderFollowup.TenderFollowupId);

            }

            return Json(new[] { tenderFollowup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
