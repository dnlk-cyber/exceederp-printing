using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRLeaveUser, HrmConstants.HRAdmin)]
    public class LeaveConfigurationsController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        // GET: HRM/LeaveConfigurations
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["FisicalYear"] = CacheData.GetFiscalYearCaches().Select(
             i => new
             {
                 Name = i.Name,
                 GlFiscalYearId = i.Id,
             }).AsQueryable();
            ViewData["LeaveSetting"] = Enum.GetValues(typeof(LeaveSetting))
                                                 .Cast<LeaveSetting>()
                                                 .Select(v => new { Text = v.ToString(), Value = v.ToString() })
                                                 .ToList();

        }
    }
}