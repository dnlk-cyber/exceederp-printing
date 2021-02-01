using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Common;
using System.Web.Mvc;
namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
   public class OperatorDataSource
    {

        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetSalaryIncentive()
        {

            var result = EnumExtension.EnumToSelectList(typeof(ExceedERP.Core.Domain.FleetManagement.FleetRegistration.Operator.SalaryIncentiveenum));

            return result;

        }
            public List<SelectListItem> GetSalaryLisenseType()
        {

            var result = EnumExtension.EnumToSelectList(typeof(ExceedERP.Core.Domain.FleetManagement.FleetRegistration.Operator.LicenseTypeenum));

            return result;

        }
            public List<SelectListItem> GetSalaryLisenseLevel()
            {

                var result = EnumExtension.EnumToSelectList(typeof(ExceedERP.Core.Domain.FleetManagement.FleetRegistration.Operator.LicenseLevelenum));

                return result;

            }
            public List<Helpers.Fleet.FleetBindingHelper.HelperObject> GetPostions()
            {

                var result = db.OperatorPosition.Select(s => new Helpers.Fleet.FleetBindingHelper.HelperObject
                    {
                        Name = s.Name,
                        Value = s.OperatorPositionID.ToString()
                    }).ToList();

                return result;

            }

           [DataObjectMethod(DataObjectMethodType.Select)]
        public List<OperatorViewModel> GetOperators ( string Postion , string SalaryIncentive , string LisenseType , string LicenseLevel , DateTime StartDate, DateTime EndDate)
        {

            List<OperatorViewModel> OperatorList = new List<OperatorViewModel>();
            var Operators = db.Operator.ToList();

            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            foreach (var s in Operators)
            {
                string Location = "UnAssigned";
              
                var Fleet = db.Fleets.Where(x => x.OperatorName == s.FullName).FirstOrDefault();
                if(Fleet != null)
                {
                    Location = Fleet.Location;
                }
                  OperatorList.Add(  new OperatorViewModel
                {
                    
                    EducationLevel = s.EducationLevel,
                    Email = s.Email,
                    FullName = s.FullName,
                    Gender = s.Gender,
                    GuaranteeAddress = s.GuaranteeAddress,
                    GuaranteeName = s.GuaranteeName,
                    GuaranteePhoneNumber = s.GuaranteePhoneNumber,
                    HireDate = s.HireDate,
                    LicenseLevel = s.LicenseLevel,
                    LicenseNo = s.LicenseNo,
                    LicenseType = s.LicenseType,
                    MobilePhone = s.MobilePhone,
                    NameOfTraningCenter = s.NameOfTraningCenter,
                    Postion = s.Postion,
                    LicenseCity = s.LicenseCity,
                    SalaryIncentive = s.SalaryIncentive,
                    AmharicName = AmharicName,
                    Name = Name,
                    LogoFileName = LogoFileName,
                    Location = Location

                });
            }
        

            if (!string.IsNullOrEmpty(Postion))
            {
                OperatorList = OperatorList.Where(x => x.Postion == Postion).ToList();
            }
            if (!string.IsNullOrEmpty(SalaryIncentive))
            {
                OperatorList = OperatorList.Where(x => x.SalaryIncentive == SalaryIncentive).ToList();
            }
            if (!string.IsNullOrEmpty(LisenseType))
            {
                OperatorList = OperatorList.Where(x => x.LicenseType == LisenseType).ToList();
            }
            if (!string.IsNullOrEmpty(LicenseLevel))
            {
                OperatorList = OperatorList.Where(x => x.LicenseLevel == LicenseLevel).ToList();
            }

            OperatorList = OperatorList.Where(x => x.HireDate > StartDate && x.HireDate < EndDate).ToList();

            return OperatorList;
        }
    }
}
