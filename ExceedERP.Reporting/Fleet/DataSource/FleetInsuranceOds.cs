using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class FleetInsuranceOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetInsuranceViewModel> GetFleetInsurance()
        {
            List<FleetInsuranceViewModel> list = new List<FleetInsuranceViewModel>();
            var fleetInsuranceList = db.FleetInsurance.ToList();
            
            foreach (var fleetInsurance in fleetInsuranceList)
            {
                var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetInsurance.FleetsID);
                var insuranceCompany = db.InsuranceCompany.FirstOrDefault(x => x.InsuranceCompanyId == fleetInsurance.InsuranceCompanyId);
                var fleetInsuranceViewModel = new FleetInsuranceViewModel
                {
                    PlateNo = fleet?.PlateNumber ?? "Not Assigned",
                    InsuranceCompany = insuranceCompany?.InsuranceCompanyName ?? "Not Assigned",
                    Attachment = fleetInsurance.Attachment,
                    Description = fleetInsurance.Description,
                    DueDate = fleetInsurance?.DueDate,
                    ExpireDate = fleetInsurance?.ExpireDate,
                    PremiumCost = fleetInsurance.PremiumCost,
                    ComprehensiveInsurance = fleetInsurance.ComprehensiveInsurance,
                    InsuranceType = fleetInsurance.InsuranceType,
                    PolicyNo = fleetInsurance.PolicyNo,
                    ThirdPartyInsurance = fleetInsurance.ThirdPartyInsurance
                };

                list.Add(fleetInsuranceViewModel);
            }
            return list;
        }
    }

    public class FleetInsuranceViewModel
    {
        public string PlateNo { get; set; }
        public string InsuranceCompany { get; set; }
        public decimal PremiumCost { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Attachment { get; set; }
        public string Description { get; set; }
        public FleetInsuranceTypeEnum InsuranceType { get; set; }
        public decimal ThirdPartyInsurance { get; set; }
        public decimal ComprehensiveInsurance { get; set; }
        public string PolicyNo { get; set; }
        
    }
}
