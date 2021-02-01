﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
 using ExceedERP.Core.Domain.CRM;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles( CRMRoles.MarketingUser)] 
    public class CampaignController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            SetInitials();
            return View();
        }

        public void SetInitials()
        {
          /*  ViewData["EmployeeList"] =
                db.PersonCategories.Select(
                    p => new { PersonID = p.PersonID, Name = p.FirstName + " " + p.MiddleName + " " + p.LastName }).ToList();
            ViewData["TeamList"] = db.MainCompanyStructures.ToList();*/
            ViewData["CartList"] = db.Carts.ToList();
            ViewData["ProductList"] = db.Products.ToList();

        }
        public ActionResult Campaigns_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Campaign> campaigns = db.Campaigns;
            DataSourceResult result = campaigns.ToDataSourceResult(request, campaign => new
            {
                CampaignID = campaign.CampaignID,
                Status = campaign.Status,
                AssignedTo = campaign.AssignedTo,
                TeamId = campaign.TeamId,
                CartID = campaign.CartID,
                CompaignType = campaign.CompaignType,
                Budget = campaign.Budget,
                ExpectedRevenu = campaign.ExpectedRevenu,
                Objective = campaign.Objective,
                CreatedBy = campaign.CreatedBy,
                DateCreated = campaign.DateCreated,
                LastModified = campaign.LastModified,
                ModifiedBy = campaign.ModifiedBy,
                OrgBranchName = campaign.OrgBranchName,
                Description = campaign.Description

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Campaigns_Create([DataSourceRequest]DataSourceRequest request, Campaign campaign)
        {
            if (ModelState.IsValid)
            {
               var entity = new Campaign
                {
                    Status = campaign.Status,
                    AssignedTo = campaign.AssignedTo,
                    TeamId = campaign.TeamId,
                    CartID = campaign.CartID,
                    CompaignType = campaign.CompaignType,
                    Budget = campaign.Budget,
                    ExpectedRevenu = campaign.ExpectedRevenu,
                    Objective = campaign.Objective,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Description = campaign.Description
                };

                db.Campaigns.Add(entity);
                db.SaveChanges();
                campaign.CampaignID = entity.CampaignID;
            }

            return Json(new[] { campaign }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Campaigns_Update([DataSourceRequest]DataSourceRequest request, Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                var entity = new Campaign
                {
                    CampaignID = campaign.CampaignID,
                    Status = campaign.Status,
                    AssignedTo = campaign.AssignedTo,
                    TeamId = campaign.TeamId,
                    CartID = campaign.CartID,
                    CompaignType = campaign.CompaignType,
                    Budget = campaign.Budget,
                    ExpectedRevenu = campaign.ExpectedRevenu,
                    Objective = campaign.Objective,
                    Description = campaign.Description
                };

                db.Campaigns.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { campaign }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Campaigns_Destroy([DataSourceRequest]DataSourceRequest request, Campaign campaign)
        {
            if (ModelState.IsValid)
            {
                var entity = new Campaign
                {
                    CampaignID = campaign.CampaignID,
                    Status = campaign.Status,
                    AssignedTo = campaign.AssignedTo,
                    TeamId = campaign.TeamId,
                    CartID = campaign.CartID,
                    CompaignType = campaign.CompaignType,
                    Budget = campaign.Budget,
                    ExpectedRevenu = campaign.ExpectedRevenu,
                    Objective = campaign.Objective,
                    Description = campaign.Description
                };

                db.Campaigns.Attach(entity);
                db.Campaigns.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Campaign number " + campaign.CampaignID);
		
     

            }

            return Json(new[] { campaign }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
