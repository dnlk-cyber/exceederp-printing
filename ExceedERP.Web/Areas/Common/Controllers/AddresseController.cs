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
﻿using Microsoft.Ajax.Utilities;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Company_Admin + "," + HR_Training_User + "," + HR_Admin + "," + HR_Company_User + "," + HR_Recruitment_User + "," + HR_Recruitment_Admin)]
    public class AddresseController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Profile_Admin = "HR_Profile_Admin";
        private const string HR_Profile_User = "HR_Profile_User";
        private const string HR_Company_Admin = "HR_Company_Admin";
        private const string HR_Training_User = "HR_Training_User";
        private const string HR_Admin = "HR_Admin";
        private const string HR_Company_User = "HR_Company_User";
        private const string HR_Recruitment_User = "HR_Recruitment_User";
        private const string HR_Recruitment_Admin = "HR_Recruitment_Admin";
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Company_Admin + "," + HR_Training_User + "," + HR_Admin + "," + HR_Company_User + "," + HR_Recruitment_User + "," + HR_Recruitment_Admin)]
        public ActionResult Addresses_Read([DataSourceRequest]DataSourceRequest request, int id, string addressReferenceType)
        {
            IQueryable<Address> addresses = db.Addresses.Where(ad => ad.Reference == id.ToString() && ad.ReferenceType == addressReferenceType);
            DataSourceResult result = addresses.ToDataSourceResult(request, address => new {
                AddressID = address.AddressId,
                ReferenceType=address.ReferenceType,
                Reference = address.Reference,
                Category = address.Category,
                Value = address.Value,
                Remark = address.Remark,
                DateCreated = address.DateCreated,
                LastModified = address.LastModified,
                CreatedBy = address.CreatedBy,
                ModifiedBy = address.ModifiedBy
            });

            return Json(result);
        }
          [Authorize(Roles = HR_Profile_User + "," + HR_Training_User + "," + HR_Company_User + "," + HR_Recruitment_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Addresses_Create([DataSourceRequest]DataSourceRequest request, Address address, int id, string addressReferenceType)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.Addresses.Where(x => x.Category == address.Category && x.Reference == id.ToString () && x.ReferenceType == addressReferenceType).FirstOrDefault();
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "This Address Type is already registered!!");
                }
                else
                {
                    var entity = new Address
                    {
                        Reference = id.ToString(),
                        Category = address.Category,
                        Value = address.Value,
                        ReferenceType = addressReferenceType,
                        Remark = address.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = address.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = address.ModifiedBy
                    };

                    db.Addresses.Add(entity);
                    db.SaveChanges();
                    address.AddressId = entity.AddressId;
                }
            }

            return Json(new[] { address }.ToDataSourceResult(request, ModelState));
        }
          [Authorize(Roles = HR_Profile_User + "," + HR_Training_User + "," + HR_Company_User + "," + HR_Recruitment_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Addresses_Update([DataSourceRequest]DataSourceRequest request, Address address)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Address
                {
                    ReferenceType = address.ReferenceType,
                    AddressId = address.AddressId,
                    Reference = address.Reference,
                    Category = address.Category,
                    Value = address.Value,
                    Remark = address.Remark,
                    DateCreated = address.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = address.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.Addresses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { address }.ToDataSourceResult(request, ModelState));
        }
          [Authorize(Roles = HR_Profile_Admin + "," + HR_Company_Admin + "," + HR_Admin + "," + HR_Recruitment_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Addresses_Destroy([DataSourceRequest]DataSourceRequest request, Address address)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Address
                {
                    AddressId = address.AddressId,
                    Reference = address.Reference,
                    Category = address.Category,
                    Value = address.Value,
                    Remark = address.Remark,
                    DateCreated = address.DateCreated,
                    LastModified = address.LastModified,
                    CreatedBy = address.CreatedBy,
                    ModifiedBy = address.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Address" + address.ReferenceType + " " + address.Reference);
                #endregion
                db.Addresses.Attach(entity);
                db.Addresses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { address }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
