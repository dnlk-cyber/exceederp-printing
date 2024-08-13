using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Insurance;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
   
    [AuthorizeRoles(HrmConstants.HRInsuranceAdmin, HrmConstants.HRInsuranceUser)]
    public class InsuranceDataProviderController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public JsonResult GetEmployee()
        {


            return Json(db.Person_Employee.Where(x=>x.IsActive).Select(p => new
            {
                PersonId = p.EmployeeId,
                FullNameWithOutCode = p.FirstNameEnglish+" "+p.MiddleNameEnglish+" "+p.LastNameEnglish
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBusinessClass()
        {
            List<InsuranceType> insuranceTypeList = new List<InsuranceType>();
            var insuranceTypes = db.InsuranceType.Where(x => x.InsuranceTypeGroup == InsuranceGroupType.Employee).ToList();
            foreach (var insuranceType in insuranceTypes)
            {
                InsuranceType insurance = new InsuranceType();
                var businessClass = db.Lookups.FirstOrDefault(x => x.LookupId == insuranceType.LookupId && x.LookupType == LookUpTypes.INSURANCE_CLASS_OF_BUSINESS);

                insurance.LookupId = businessClass.LookupId;
                insurance.Name = businessClass.Description;

                insuranceTypeList.Add(insurance);
                ViewData["classList"] = insuranceTypeList;
            }


            return Json(insuranceTypeList.Select(s => new
            {
                LookupId = s.LookupId,
                Name = s.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceType()
        {

            List<InsuranceType> insuranceTypeList = new List<InsuranceType>();
            var insuranceTypes = db.InsuranceType.Where(x => x.InsuranceTypeGroup == InsuranceGroupType.Employee).ToList();

            insuranceTypeList.AddRange(insuranceTypes);


            return Json(insuranceTypeList.Select(s => new
            {

                InsuranceTypeId = s.InsuranceTypeId,
                Name = s.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceGroup()
        {

            var insurace = db.InsuranceGroup.ToList();

            ViewData["insuraceGroup"] = insurace;
            return Json(insurace.Select(s => new { InsuranceGroupId = s.InsuranceGroupId, GroupName = s.GroupName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceCategory(int InsuranceTypeId)
        {

            var insurace = db.InsuranceCategory.Where(x=>x.InsuranceTypeId == InsuranceTypeId).ToList();

            ViewData["insuraceGroup"] = insurace;
            return Json(insurace.Select(s => new { InsuranceCategoryId = s.InsuranceCategoryId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
