﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Insurance;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Web.Areas.HRM.Controllers;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
   // [Authorize(Roles = Insurance_Admin + "," + Insurance_User)]
    [AuthorizeRoles(HrmConstants.HRInsuranceAdmin, HrmConstants.HRInsuranceUser)]
    public class CourtInsuranceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Insurance_Admin = "Insurance_Admin";
        private const string Insurance_User = "Insurance_User";
        public static int EmployeeId { get; set; }
        public static DateTime Date { get; set; }
        public static string DateInsuAmhairc { get; set; }
        public static string DateAssocAmharic { get; set; }
        private static double MinAge { get; set; }
        private static double MaxAge { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
            var assocation = db.EmployeeCourtAssociation.AsQueryable();
            ViewData["Association"] = assocation;
            var configartion = db.Configurations.Where(x => x.Category == "Insurance");
            if (configartion.Any())
            {

                foreach (var c in configartion)
                {
                    if (c.Reference == "MINIMUM_AGE")
                    {
                        MinAge = Convert.ToDouble(c.CurrentValue);
                    }
                    if (c.Reference == "MAXIMUM_AGE")
                    {
                        MaxAge = Convert.ToDouble(c.CurrentValue);
                    }

                }
            }
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateInsuAmhairc")
            {
                DateInsuAmhairc = value;
                var splitDate = DateInsuAmhairc.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (name == "DateAssocAmharic")
            {
                DateAssocAmharic = value;
                var splitDate = DateAssocAmharic.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }



        }
        public JsonResult CalculateAge(int id = 0)
        {
            List<EmployeeCourtAssociation> list = new List<EmployeeCourtAssociation>();
            var assocation = db.EmployeeCourtAssociation.Where(x => x.EmployeeId == EmployeeId).AsQueryable();

            foreach (var a in assocation)
            {
                double years = DateTime.Now.Date.Subtract(new DateTime(a.DateOfBirth.Year, a.DateOfBirth.Month, a.DateOfBirth.Day)).TotalDays / 365.25;

                if ((years > MinAge && years < MaxAge) && a.AssociationType == AssociationType.CHILD)
                {
                    list.Add(a);
                }
                else if (a.AssociationType == AssociationType.WIFE)
                {
                    list.Add(a);
                }
            }
            return Json(list.Select(p => new { AssociationId = p.AssociationId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        //[Authorize(Roles = Insurance_Admin + "," + Insurance_User)]
        [AuthorizeRoles(HrmConstants.HRInsuranceAdmin, HrmConstants.HRInsuranceUser)]
        public ActionResult CourtInsurance_Read([DataSourceRequest]DataSourceRequest request, int id, string type)
        {
            // CalculateAge(id);
            EmployeeId = id;
            IQueryable<CourtInsurance> courtinsurance = db.CourtInsurance.Where(x => x.EmployeeId == id);
            if (type == "aso")
            {

                courtinsurance = courtinsurance.Where(x => x.Type == "Assocation");
            }
            else
            {
                courtinsurance = courtinsurance.Where(x => x.Type == "Judge");
            }

            DataSourceResult result = courtinsurance.ToDataSourceResult(request, courtInsurance => new
            {
                CourtInsuranceId = courtInsurance.CourtInsuranceId,
                EmployeeId = courtInsurance.EmployeeId,
                Date = courtInsurance.Date,
                AssociationId = courtInsurance.AssociationId,
                Reason = courtInsurance.Reason,
                InsuranceAmount = courtInsurance.InsuranceAmount,
                Remark = courtInsurance.Remark,
                OrgBranchName = courtInsurance.OrgBranchName,
                DateCreated = courtInsurance.DateCreated,
                LastModified = courtInsurance.LastModified,
                CreatedBy = courtInsurance.CreatedBy,
                Type = courtInsurance.Type,
                ModifiedBy = courtInsurance.ModifiedBy,
                DateInsuAmhairc = courtInsurance.DateInsuAmhairc,
                DateAssocAmharic = courtInsurance.DateAssocAmharic
            });

            return Json(result);
        }
        private decimal GetInsuranceAmount(int AssocationId, string empId)
        {
            decimal amount = 0;
            var assocation = db.EmployeeCourtAssociation.Where(x => x.AssociationId == AssocationId).FirstOrDefault();
            var Configartion = db.Configurations.Where(x => x.Category == "Insurance");
            if (empId == "det")
            {
                var insurance = Configartion.Where(x => x.Reference == "JUDGE").FirstOrDefault();
                if (insurance != null)
                {
                    amount = Convert.ToDecimal(insurance.CurrentValue);
                }
                return amount;
            }
            if (assocation != null)
            {
                switch (assocation.AssociationType)
                {
                    case AssociationType.WIFE:

                        var insurance = Configartion.Where(x => x.Reference == "WIFE_OR_HUSBAND").FirstOrDefault();
                        if (insurance != null)
                        {
                            amount = Convert.ToDecimal(insurance.CurrentValue);
                        }
                        break;
                    case AssociationType.HUSBAND:

                        var insurancee = Configartion.Where(x => x.Reference == "WIFE_OR_HUSBAND").FirstOrDefault();
                        if (insurancee != null)
                        {
                            amount = Convert.ToDecimal(insurancee.CurrentValue);
                        }
                        break;
                    case AssociationType.CHILD:

                        var insurancec = Configartion.Where(x => x.Reference == "CHILDREN").FirstOrDefault();
                        if (insurancec != null)
                        {
                            amount = Convert.ToDecimal(insurancec.CurrentValue);
                        }
                        break;
                }


            }
            return amount;
        }
        private string Validation(int assocationId)
        {
            var assocation = db.EmployeeCourtAssociation.Where(x => x.AssociationId == assocationId).FirstOrDefault();

            if (assocation != null)
            {
                var employee = db.Person_Employee.Where(x => x.EmployeeId == assocation.EmployeeId).FirstOrDefault();
                if (employee != null)
                {
                    switch (assocation.AssociationType)
                    {
                        case AssociationType.WIFE:
                            if (employee.MartialStatus == MartialStatus.DIVORCED)
                            {
                                return "The Child age is greater than 18. So the insurance is not permitted";
                            }

                            break;
                        case AssociationType.HUSBAND:
                            if (employee.MartialStatus == MartialStatus.DIVORCED)
                            {
                                return "The Child age is greater than 18. So the insurance is not permitted.";
                            }

                            break;
                        case AssociationType.CHILD:
                            var datediff = DateTime.Now.Year - assocation.DateOfBirth.Year;
                            if (datediff > 18)
                            {
                                return "The Child age is greater than 18. So the insurance is not permitted.";
                            }
                            break;
                    }
                }
            }
            return null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        //[Authorize(Roles = Insurance_User)]
        [AuthorizeRoles(HrmConstants.HRInsuranceUser)]
        public ActionResult CourtInsurance_Create([DataSourceRequest]DataSourceRequest request, CourtInsurance courtInsurance, int idTwo, string typeTwo)
        {
            ModelState.Clear();
            if (typeTwo == "aso")
            {
                courtInsurance.Type = "Assocation";

            }
            else
            {
                courtInsurance.Type = "Judge";
            }
            if (ModelState.IsValid)
            {
                if (idTwo > 0)
                {

                    var validateResult = Validation(courtInsurance.AssociationId);
                    if (validateResult != null)
                    {
                        return new JavaScriptResult() { Script = "alert('" + validateResult + "');" };
                    }
                }
                if (typeTwo == "aso" && DateAssocAmharic == null)
                {

                    ModelState.AddModelError("", "Date values are mandatory!!");

                }
                else if (typeTwo == "det" && DateInsuAmhairc == null)
                {


                    ModelState.AddModelError("", "Date values are mandatory!!");

                }

                else if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }

                else
                {
                    var entity = new CourtInsurance
                    {
                        Date = Date,
                        EmployeeId = idTwo,
                        AssociationId = courtInsurance.AssociationId,
                        Reason = courtInsurance.Reason,
                        InsuranceAmount = GetInsuranceAmount(courtInsurance.AssociationId, typeTwo),
                        Remark = courtInsurance.Remark,
                        OrgBranchName = courtInsurance.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = courtInsurance.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = courtInsurance.ModifiedBy,
                        Type = courtInsurance.Type,
                        DateInsuAmhairc = DateInsuAmhairc,
                        DateAssocAmharic = DateAssocAmharic
                    };

                    db.CourtInsurance.Add(entity);
                    db.SaveChanges();
                    courtInsurance.CourtInsuranceId = entity.CourtInsuranceId;
                }
            }

            return Json(new[] { courtInsurance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        //[Authorize(Roles = Insurance_User)]
        [AuthorizeRoles(HrmConstants.HRInsuranceUser)]
        public ActionResult CourtInsurance_Update([DataSourceRequest]DataSourceRequest request, CourtInsurance courtInsurance, string typeThree)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (typeThree == "aso" && DateAssocAmharic == null)
                {
                    DateAssocAmharic = courtInsurance.DateAssocAmharic;
                    var splitDate = DateAssocAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (typeThree == "det" && DateInsuAmhairc == null)
                {
                    DateInsuAmhairc = courtInsurance.DateInsuAmhairc;
                    var splitDate = DateInsuAmhairc.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }
                else if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new CourtInsurance
                    {
                        CourtInsuranceId = courtInsurance.CourtInsuranceId,
                        Date = Date,
                        AssociationId = courtInsurance.AssociationId,
                        Reason = courtInsurance.Reason,
                        InsuranceAmount = GetInsuranceAmount(courtInsurance.AssociationId, typeThree),
                        Remark = courtInsurance.Remark,
                        Type = courtInsurance.Type,
                        OrgBranchName = courtInsurance.OrgBranchName,
                        DateCreated = courtInsurance.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = courtInsurance.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        EmployeeId = courtInsurance.EmployeeId,
                        DateInsuAmhairc = DateInsuAmhairc,
                        DateAssocAmharic = DateAssocAmharic
                    };

                    db.CourtInsurance.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { courtInsurance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        //[Authorize(Roles = Insurance_Admin)]
        [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]
        public ActionResult CourtInsurance_Destroy([DataSourceRequest]DataSourceRequest request, CourtInsurance courtInsurance)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new CourtInsurance
                {
                    CourtInsuranceId = courtInsurance.CourtInsuranceId,
                    Date = courtInsurance.Date,
                    AssociationId = courtInsurance.AssociationId,
                    Reason = courtInsurance.Reason,
                    Type = courtInsurance.Type,
                    InsuranceAmount = courtInsurance.InsuranceAmount,
                    Remark = courtInsurance.Remark,
                    OrgBranchName = courtInsurance.OrgBranchName,
                    DateCreated = courtInsurance.DateCreated,
                    LastModified = courtInsurance.LastModified,
                    CreatedBy = courtInsurance.CreatedBy,
                    ModifiedBy = courtInsurance.ModifiedBy,
                    EmployeeId = courtInsurance.EmployeeId,
                    DateInsuAmhairc = courtInsurance.DateInsuAmhairc,
                    DateAssocAmharic = courtInsurance.DateAssocAmharic
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted insurance information of employee id " + courtInsurance.EmployeeId);
                #endregion
                db.CourtInsurance.Attach(entity);
                db.CourtInsurance.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { courtInsurance }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
