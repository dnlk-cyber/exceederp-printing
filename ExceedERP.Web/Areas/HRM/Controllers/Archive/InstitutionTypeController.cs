﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
﻿using ExceedERP.Core.Domain.HRM.ArchiveManagement;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
    
    [AuthorizeRoles(HrmConstants.HRArchiveAdmin)]
    public class InstitutionTypeController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InstitutionType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InstitutionType> institutiontype = db.InstitutionType;
            DataSourceResult result = institutiontype.ToDataSourceResult(request, institutionType => new {
                InstitutionTypeId = institutionType.InstitutionTypeId,
                InstitutionName = institutionType.InstitutionName,
                Remark = institutionType.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionType_Create([DataSourceRequest]DataSourceRequest request, InstitutionType institutionType)
        {
            if (ModelState.IsValid)
            {
                var checkExstance = db.InstitutionType.ToList();
                var jobName = institutionType.InstitutionName;
                var mainCompanyStructures = checkExstance.FirstOrDefault(j => j.InstitutionName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                if (mainCompanyStructures != null)
                {
                    ModelState.AddModelError("", "This Institution type already exists in the database!!");
                }
                else
                {
                    var entity = new InstitutionType
                    {
                        InstitutionName = institutionType.InstitutionName,
                        Remark = institutionType.Remark
                    };

                    db.InstitutionType.Add(entity);
                    db.SaveChanges();
                    institutionType.InstitutionTypeId = entity.InstitutionTypeId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Institution Type", "Success", User.Identity.Name, User.Identity.Name + " add new institution Type called " + entity.InstitutionName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                    
                    #endregion
                }
            }

            return Json(new[] { institutionType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionType_Update([DataSourceRequest]DataSourceRequest request, InstitutionType institutionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new InstitutionType
                {
                    InstitutionTypeId = institutionType.InstitutionTypeId,
                    InstitutionName = institutionType.InstitutionName,
                    Remark = institutionType.Remark
                };

                db.InstitutionType.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Institution Type", "Success", User.Identity.Name, User.Identity.Name + " updated institution Type called " + entity.InstitutionName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                
                #endregion
            }

            return Json(new[] { institutionType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionType_Destroy([DataSourceRequest]DataSourceRequest request, InstitutionType institutionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new InstitutionType
                {
                    InstitutionTypeId = institutionType.InstitutionTypeId,
                    InstitutionName = institutionType.InstitutionName,
                    Remark = institutionType.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted institution type of name " + institutionType.InstitutionName);
                #endregion
                db.InstitutionType.Attach(entity);
                db.InstitutionType.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { institutionType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
