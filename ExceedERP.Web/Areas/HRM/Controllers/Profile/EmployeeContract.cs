using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
  
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class EmployeeContractController : BaseController
    {
        // GET: HRM/EmployeeExperience
        private ExceedDbContext db = new ExceedDbContext();
        //public static string FromAmharic { get; set; }
        //public static string ToAmharic { get; set; }
        //public static DateTime From { get; set; }
        //public static DateTime To { get; set; }
        public ActionResult Index()
        {

            return View();
        }
        //public void SetDate(string name, string value)
        //{
        //    if (name == "FromContAm")
        //    {
        //        FromAmharic = value;
        //        var splitDate = FromAmharic.Split('/');
        //        From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }
        //    else if (name == "ToContAm")
        //    {
        //        ToAmharic = value;
        //        var splitDate = ToAmharic.Split('/');
        //        To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
        //    }

        //}
      
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin, HrmConstants.HRProfileUser)]
        public ActionResult Contract_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Contract> contracts = db.Contract.Where(c => c.EmployeeId == id);
            DataSourceResult result = contracts.ToDataSourceResult(request, contract => new
            {
                ContractId = contract.ContractId,
                From = contract.From,
                To = contract.To,
                MonthlySalary = contract.MonthlySalary,
                EmployeeId = contract.EmployeeId,
                Remark = contract.Remark,
                DateCreated = contract.DateCreated,
                LastModified = contract.LastModified,
                CreatedBy = contract.CreatedBy,
                ModifiedBy = contract.ModifiedBy,
                FromContAm = contract.FromContAm,
                ToContAm = contract.ToContAm
            });

            return Json(result);
        }
   
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Contract_Create([DataSourceRequest]DataSourceRequest request, Contract contract, int id)
        {
           
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(contract.FromContAm)
                || string.IsNullOrEmpty(contract.ToContAm))
                {
                    ModelState.AddModelError("", " Date fields are mandatory");
                    return Json(new[] { contract }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    contract.From = HrHelper.ParseEthiopicDate(contract.FromContAm);
                    contract.To = HrHelper.ParseEthiopicDate(contract.ToContAm);
                }

            }
            else
            {

                contract.FromContAm = XAPI.EthiopicDateTime.GetEthiopicDate(contract.From.Day,contract.From.Month,contract.From.Year);
                contract.ToContAm = XAPI.EthiopicDateTime.GetEthiopicDate(contract.To.Day,contract.To.Month,contract.To.Year);

            }


           
            var checkexistance = db.Contract.FirstOrDefault(x => x.EmployeeId == id && x.From == contract.From && x.To == contract.To);
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This Contract date is already registered!!");
            }
            if (contract.From.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "start date should be less or equal to the current date!!");
            }
            if (contract.From.Date > contract.To.Date)
            {

                ModelState.AddModelError("", "start date should be less or equal to the end date!!");
            }
            contract.EmployeeId = id;
            contract.DateCreated = DateTime.Now;
            contract.CreatedBy = User.Identity.Name;
            if (ModelState.IsValid)
            {
               
                   
                var entity = new Contract
                {
                    ContractId = contract.ContractId,
                    From = contract.From,
                    To = contract.To,
                    MonthlySalary = contract.MonthlySalary,
                    EmployeeId = contract.EmployeeId,
                    Remark = contract.Remark,
                    DateCreated = contract.DateCreated,
                    LastModified = contract.LastModified,
                    CreatedBy = contract.CreatedBy,
                    ModifiedBy = contract.ModifiedBy,
                    FromContAm = contract.FromContAm,
                    ToContAm = contract.ToContAm
                };

                db.Contract.Add(entity);
                db.SaveChanges();
                contract.ContractId = entity.ContractId;

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Contract", "Success", User.Identity.Name, "Contract for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                            
                }
                #endregion
                    
                
               



            }

            return Json(new[] { contract }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Contract_Update([DataSourceRequest]DataSourceRequest request, Contract contract)
        {

            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(contract.FromContAm)
                || string.IsNullOrEmpty(contract.ToContAm))
                {
                    ModelState.AddModelError("", " Date fields are mandatory");
                    return Json(new[] { contract }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    contract.From = HrHelper.ParseEthiopicDate(contract.FromContAm);
                    contract.To = HrHelper.ParseEthiopicDate(contract.ToContAm);
                }

            }
            else
            {

                contract.FromContAm = XAPI.EthiopicDateTime.GetEthiopicDate(contract.From.Day, contract.From.Month, contract.From.Year);
                contract.ToContAm = XAPI.EthiopicDateTime.GetEthiopicDate(contract.To.Day, contract.To.Month, contract.To.Year);

            }

            
            var checkexistance = db.Contract.FirstOrDefault(x => x.EmployeeId == contract.EmployeeId && x.From == contract.From && x.To == contract.To);
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This Contract date is already registered!!");
            }
            if (contract.From.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "start date should be less or equal to the current date!!");
            }
            if (contract.From.Date > contract.To.Date)
            {

                ModelState.AddModelError("", "start date should be less or equal to the end date!!");
            }
            contract.LastModified = DateTime.Now;
            contract.ModifiedBy = User.Identity.Name;
            if (ModelState.IsValid)
            {
               
                var entity = new Contract
                {
                    ContractId = contract.ContractId,
                    From = contract.From,
                    To = contract.To,
                    MonthlySalary = contract.MonthlySalary,
                    EmployeeId = contract.EmployeeId,
                    Remark = contract.Remark,
                    DateCreated = contract.DateCreated,
                    LastModified = contract.LastModified,
                    CreatedBy = contract.CreatedBy,
                    ModifiedBy = contract.ModifiedBy,
                    FromContAm = contract.FromContAm,
                    ToContAm = contract.ToContAm
                };

                db.Contract.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Employee Contract", "Success", User.Identity.Name, "Contract of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                        
                }
                #endregion
                
            }

            return Json(new[] { contract }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Contract_Destroy([DataSourceRequest]DataSourceRequest request, Contract contract)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Contract
                {
                    ContractId = contract.ContractId,
                    From = contract.From,
                    To = contract.To,
                    MonthlySalary = contract.MonthlySalary,
                    EmployeeId = contract.EmployeeId,
                    Remark = contract.Remark,
                    DateCreated = contract.DateCreated,
                    LastModified = contract.LastModified,
                    CreatedBy = contract.CreatedBy,
                    ModifiedBy = contract.ModifiedBy,
                    FromContAm = contract.FromContAm,
                    ToContAm = contract.ToContAm
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted contract information of employee Id" + contract.EmployeeId);
                #endregion
                db.Contract.Attach(entity);
                db.Contract.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { contract }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}