using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Reporting.CRM.DataSource
{
    public class CampaignObjectDataSource
    {

        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetOrgCampaignStatus()
        {
            ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject OrgCampaigns = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject();
            List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> Campaign = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();

            var Campaigns = db.Campaigns.ToList();

            foreach (var c in Campaigns)
            {
                OrgCampaigns = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject();
                OrgCampaigns.Name = c.Status;
                OrgCampaigns.Id = 1;
                Campaign.Add(OrgCampaigns);
            }

            return Campaign.Distinct().ToList();
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetOrgCampaignType()
        {
        
            var Campaigns = EnumExtension.EnumToSelectList(typeof(CampaignType));


            return Campaigns;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CampaignView> GetCampaignPerformance(string campaignType, string status, DateTime startingDate, DateTime endingDate)
        {
            List<CampaignView> Campaign = new List<CampaignView>();
            CampaignView CampaignViewModel;
            List<ExceedERP.Core.Domain.CRM.Campaign> Campaigns;
            var OrgCustomers = db.OrganizationCustomers.ToList();
            var product = db.Products.ToList();
            Campaigns = db.Campaigns.ToList();

            if (!string.IsNullOrEmpty(campaignType))
            {
                Campaigns = Campaigns.Where(so =>  so.CompaignType == campaignType).ToList();
            }
                if (!string.IsNullOrEmpty(status))
                {
                    Campaigns = Campaigns.Where(so => so.Status == status ).ToList();
                }
              
         
                if (!string.IsNullOrEmpty(status))
                {
                    Campaigns = Campaigns.Where(so => so.Status == status).ToList();
                }
               

            List<ExceedERP.Core.Domain.CRM.Campaign> campaignsList = new List<ExceedERP.Core.Domain.CRM.Campaign>();


            campaignsList =
             Campaigns.Where(x => x.DateCreated >= startingDate && x.DateCreated <= endingDate).ToList();


            foreach (ExceedERP.Core.Domain.CRM.Campaign cam in campaignsList)
            {


                CampaignViewModel = new CampaignView();
                CampaignViewModel.Description = cam.Description;
                CampaignViewModel.CompaignType = cam.CompaignType;
                CampaignViewModel.Status = cam.Status;
                CampaignViewModel.Budget = cam.Budget;
                CampaignViewModel.ExpectedRevenu = cam.ExpectedRevenu;
                CampaignViewModel.AssignedTo = cam.AssignedTo;
            
                Campaign.Add(CampaignViewModel);

            }

            return Campaign;
        }


    }
}