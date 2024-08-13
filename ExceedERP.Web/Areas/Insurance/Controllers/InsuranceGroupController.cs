﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Company;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Insurance;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
 
    [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]
    public class InsuranceGroupController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
  
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;

            var name = db.InsuranceCategory.ToList();
            ViewData["GroupName"] = name;
        }
        public JsonResult GroupName()
        {
            List<InsuranceCategory> groupList = new List<InsuranceCategory>();
            var insuranceTypes = db.InsuranceType.Where(x=>x.InsuranceTypeGroup == InsuranceGroupType.Employee);
            foreach(var insurance in insuranceTypes)
            {
                var groups = db.InsuranceCategory.Where(x => x.InsuranceTypeId == insurance.InsuranceTypeId).ToList();
                //foreach(var group in groups)
                //{
                //    InsuranceCategory insuranceGroup = new InsuranceCategory();
                    groupList.AddRange(groups);
                //}
            }
           
         
            return Json(groupList.Select(p => new
            {
                InsuranceCategoryId = p.InsuranceCategoryId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult InsuranceGroup_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InsuranceGroup> insurancegroup = db.InsuranceGroup;
            DataSourceResult result = insurancegroup.ToDataSourceResult(request, insuranceGroup => new
            {
                InsuranceGroupId = insuranceGroup.InsuranceGroupId,
                InsuranceCategoryId = insuranceGroup.InsuranceCategoryId,
                GroupName = insuranceGroup.GroupName,
                JobTitleName = GetTitleName(insuranceGroup.LookupId),
                OrgStructureId = insuranceGroup.OrgStructureId,
                LookupId = insuranceGroup.LookupId,
                Remark = insuranceGroup.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceGroup_Create([DataSourceRequest]DataSourceRequest request, InsuranceGroup insuranceGroup)
        {
            var categoryId = db.InsuranceCategory.FirstOrDefault();

            if (ModelState.IsValid)
            {
                var entity = new InsuranceGroup
                {
                    GroupName = insuranceGroup.GroupName,
                    JobTitleName = GetTitleName(insuranceGroup.LookupId),
                    InsuranceCategoryId = insuranceGroup.InsuranceCategoryId,
                    OrgStructureId = insuranceGroup.OrgStructureId,
                    LookupId = insuranceGroup.LookupId,
                    Remark = insuranceGroup.Remark
                };

                db.InsuranceGroup.Add(entity);
                db.SaveChanges();
                insuranceGroup.InsuranceGroupId = entity.InsuranceGroupId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Group", "Success", User.Identity.Name, User.Identity.Name + " add Insurance group at Date " + DateTime.Today + " Group name " + entity.GroupName, Modules.Insurance);

                #endregion
            }

            return Json(new[] { insuranceGroup }.ToDataSourceResult(request, ModelState));
        }
        private string GetTitleName(int id)
        {

            try
            {
                db = new ExceedDbContext();
                var name = db.Lookups.Where(x => x.LookupId == id).FirstOrDefault().Description;
                return name;
            }
            catch (Exception)
            {

                return "";
            }
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceGroup_Update([DataSourceRequest]DataSourceRequest request, InsuranceGroup insuranceGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new InsuranceGroup
                {
                    InsuranceGroupId = insuranceGroup.InsuranceGroupId,
                    InsuranceCategoryId = insuranceGroup.InsuranceCategoryId,
                    JobTitleName = GetTitleName(insuranceGroup.LookupId),
                    OrgStructureId = insuranceGroup.OrgStructureId,
                    LookupId = insuranceGroup.LookupId,
                    GroupName = insuranceGroup.GroupName,

                    Remark = insuranceGroup.Remark
                };

                db.InsuranceGroup.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Group", "Success", User.Identity.Name, User.Identity.Name + " update Insurance group at Date " + DateTime.Today + " Group name " + entity.GroupName, Modules.Insurance);

                #endregion
            }

            return Json(new[] { insuranceGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceGroup_Destroy([DataSourceRequest]DataSourceRequest request, InsuranceGroup insuranceGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new InsuranceGroup
                {
                    InsuranceGroupId = insuranceGroup.InsuranceGroupId,
                    InsuranceCategoryId = insuranceGroup.InsuranceCategoryId,
                    JobTitleName = GetTitleName(insuranceGroup.LookupId),
                    OrgStructureId = insuranceGroup.OrgStructureId,
                    LookupId = insuranceGroup.LookupId,
                    GroupName = insuranceGroup.GroupName,

                    Remark = insuranceGroup.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  insurance group information of group" + insuranceGroup.GroupName);
                #endregion
                db.InsuranceGroup.Attach(entity);
                db.InsuranceGroup.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { insuranceGroup }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetJobTitle(int OrgStructureId)
        {
            db = new ExceedDbContext();
            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == OrgStructureId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    titleList.Add(lookup);

                }
            }
            return Json(titleList.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);


        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
