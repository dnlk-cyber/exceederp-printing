﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core;
﻿using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class ObjectStateDefinitionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["ListOfGSLType"] =db.GlobalTypeList.AsQueryable();
            return View();
        }

        public ActionResult ObjectStateDefinitions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ObjectStateDefinition> objectstatedefinitions = db.ObjectStateDefinitions;
            DataSourceResult result = objectstatedefinitions.ToDataSourceResult(request, objectStateDefinition => new {
                ObjectStateDefinitionID = objectStateDefinition.ObjectStateDefinitionID,
                GSLType = objectStateDefinition.GSLType,
                Description = objectStateDefinition.Description,
                Color = objectStateDefinition.Color,
                Font = objectStateDefinition.Font,
                CreatedBy = objectStateDefinition.CreatedBy,
                DateCreated = objectStateDefinition.DateCreated,
                LastModified = objectStateDefinition.LastModified,
                ModifiedBy = objectStateDefinition.ModifiedBy,
                OrgBranchName = objectStateDefinition.OrgBranchName,
                Remark = objectStateDefinition.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectStateDefinitions_Create([DataSourceRequest]DataSourceRequest request, ObjectStateDefinition objectStateDefinition)
        {
                List<ObjectStateDefinition> objectStateDefinitions = db.ObjectStateDefinitions.Where(s => s.Description == objectStateDefinition.Description).ToList();
                if (objectStateDefinitions.Any())
                {
                    ModelState.AddModelError("DuplicateEntrie", "objectState Definition Already Exist");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var entity = new ObjectStateDefinition
                        {
                            GSLType = objectStateDefinition.GSLType,
                            Description = objectStateDefinition.Description,
                            Color = objectStateDefinition.Color,
                            Font = objectStateDefinition.Font,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                            Remark = objectStateDefinition.Remark
                        };

                        db.ObjectStateDefinitions.Add(entity);
                        db.SaveChanges();
                        objectStateDefinition.ObjectStateDefinitionID = entity.ObjectStateDefinitionID;
                    }

                }
            return Json(new[] { objectStateDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectStateDefinitions_Update([DataSourceRequest]DataSourceRequest request, ObjectStateDefinition objectStateDefinition)
        {
            List<ObjectStateDefinition> objectStateDefinitions = db.ObjectStateDefinitions.Where(s => s.Description == objectStateDefinition.Description).ToList();
            if (objectStateDefinitions.Any())
            {
                ModelState.AddModelError("DuplicateEntire", "ObjectState Definition Already Exist");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new ObjectStateDefinition
                    {
                        ObjectStateDefinitionID = objectStateDefinition.ObjectStateDefinitionID,
                        GSLType = objectStateDefinition.GSLType,
                        Description = objectStateDefinition.Description,
                        Color = objectStateDefinition.Color,
                        Font = objectStateDefinition.Font,
                        Remark = objectStateDefinition.Remark
                    };

                    db.ObjectStateDefinitions.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new[] { objectStateDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectStateDefinitions_Destroy([DataSourceRequest]DataSourceRequest request, ObjectStateDefinition objectStateDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new ObjectStateDefinition
                {
                    ObjectStateDefinitionID = objectStateDefinition.ObjectStateDefinitionID,
                    GSLType = objectStateDefinition.GSLType,
                    Description = objectStateDefinition.Description,
                    Color = objectStateDefinition.Color,
                    Font = objectStateDefinition.Font,
                    Remark = objectStateDefinition.Remark
                };

                db.ObjectStateDefinitions.Attach(entity);
                db.ObjectStateDefinitions.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Object State Definition number " + objectStateDefinition.ObjectStateDefinitionID);
      
            }

            return Json(new[] { objectStateDefinition }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
