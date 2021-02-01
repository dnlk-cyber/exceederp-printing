﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.Common.Controllers
{
     [Authorize(Roles = Payroll_Admin)]
    public class BankController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Admin = "Payroll_Admin";
        public ActionResult Index()
        {
            ViewData["PeriodList"] = db.Periods.AsQueryable();
            //ViewData["AccountList"] = db.Accounts.AsQueryable();
            return View();
        }

        public ActionResult Organizations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Bank> organizations = db.Banks;
            DataSourceResult result = organizations.ToDataSourceResult(request, bank => new
            {
                BankID = bank.BankId,
                Name = bank.Name,
                Category = bank.Category,
                IsActive = bank.IsActive,
                ParentID = bank.ParentId,
                Remark = bank.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Organizations_Create([DataSourceRequest]DataSourceRequest request, Bank bank)
        {
            if (ModelState.IsValid)
            {
                var checkExstance = db.Banks.ToList();
                var jobName = bank.Name;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.Name.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This Bank is already exists in the database!!");
                }
                else
                {
                    var entity = new Bank
                    {
                        Name = bank.Name,
                        ParentId = bank.ParentId,
                        Remark = bank.Remark,
                        Category = bank.Category,
                        IsActive = bank.IsActive
                    };

                    db.Banks.Add(entity);
                    db.SaveChanges();
                    bank.BankId = entity.BankId;
                }
            }

            return Json(new[] { bank }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Organizations_Update([DataSourceRequest]DataSourceRequest request, Bank bank)
        {
            if (ModelState.IsValid)
            {
                var entity = new Bank
                {
                    BankId = bank.BankId,
                    Name = bank.Name,
                    Remark = bank.Remark,
                    ParentId = bank.ParentId,
                    Category = bank.Category,
                    IsActive = bank.IsActive
                };

                db.Banks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { bank }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Organizations_Destroy([DataSourceRequest]DataSourceRequest request, Bank bank)
        {
            if (ModelState.IsValid)
            {
                var entity = new Bank
                {
                    BankId = bank.BankId,
                    Name = bank.Name,
                    Category = bank.Category,
                    IsActive = bank.IsActive,
                    ParentId = bank.ParentId,
                    Remark = bank.Remark
                };

                db.Banks.Attach(entity);
                db.Banks.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { bank }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
