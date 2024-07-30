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

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
  
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class EmployeeContractController : BaseController
    {
        // GET: HRM/EmployeeExperience
        private ExceedDbContext db = new ExceedDbContext();
        public static string FromAmharic { get; set; }
        public static string ToAmharic { get; set; }
        public static DateTime From { get; set; }
        public static DateTime To { get; set; }
        public ActionResult Index()
        {

            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromContAm")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToContAm")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
      
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
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic != null && ToAmharic != null)
                {
                    var checkexistance = db.Contract.FirstOrDefault(x => x.EmployeeId == id && x.From == From && x.To == To);
                    if (checkexistance != null)
                    {
                        ModelState.AddModelError("", "This Contract date is already registered!!");
                    }
                    else if (From.Date > DateTime.Now.Date)
                    {

                        ModelState.AddModelError("", "start date should be less or equal to the current date!!");
                    }
                    else if (From.Date > To.Date)
                    {

                        ModelState.AddModelError("", "start date should be less or equal to the end date!!");
                    }
                    else
                    {
                        var entity = new Contract
                        {
                            ContractId = contract.ContractId,
                            From = From,
                            To = To,
                            MonthlySalary = contract.MonthlySalary,
                            EmployeeId = id,
                            Remark = contract.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = contract.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = contract.ModifiedBy,
                            FromContAm = FromAmharic,
                            ToContAm = ToAmharic
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
                }
                else
                {
                    ModelState.AddModelError("", "Both From and To Date values are mandatory!!");
                }



            }

            return Json(new[] { contract }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Contract_Update([DataSourceRequest]DataSourceRequest request, Contract contract)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null)
                {
                    FromAmharic = contract.FromContAm;
                    var splitDate = FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ToAmharic == null)
                {
                    ToAmharic = contract.ToContAm;
                    var splitDate = ToAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (From.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "start date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new Contract
                    {
                        ContractId = contract.ContractId,
                        From = From,
                        To = To,
                        MonthlySalary = contract.MonthlySalary,
                        EmployeeId = contract.EmployeeId,
                        Remark = contract.Remark,
                        DateCreated = contract.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = contract.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        FromContAm = FromAmharic,
                        ToContAm = ToAmharic
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