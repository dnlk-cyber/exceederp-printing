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
      [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Company_Admin + "," + HR_Training_User + "," + HR_Admin + "," + HR_Company_User)]
    public class IdentificationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Profile_Admin = "HR_Profile_Admin";
        private const string HR_Profile_User = "HR_Profile_User";
        private const string HR_Company_Admin = "HR_Company_Admin";
        private const string HR_Company_User = "HR_Company_User";
        private const string HR_Training_User = "HR_Training_User";
        private const string HR_Admin = "HR_Admin";
        public static string IssueDateAm { get; set; }
        
        public static DateTime IssueDate { get; set; }
        public static DateTime ExpiryDate { get; set; }
        public static string ExpiryDateAm { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "IssueDateIdAm")
            {
                IssueDateAm = value;
                var splitDate = IssueDateAm.Split('/');
                IssueDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ExpiryDateAm")
            {
                ExpiryDateAm = value;
                var splitDate = ExpiryDateAm.Split('/');
                ExpiryDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
      [Authorize(Roles = HR_Profile_Admin + "," + HR_Profile_User + "," + HR_Company_Admin + "," + HR_Training_User + "," + HR_Admin + "," + HR_Company_User)]
        public ActionResult Identifications_Read([DataSourceRequest]DataSourceRequest request, string identificationReferenceType, string id)
        {
            IQueryable<Identification> identifications = db.Identifications.Where(i => i.Reference == id.ToString() && i.ReferenceType == identificationReferenceType);
            DataSourceResult result = identifications.ToDataSourceResult(request, identification => new {
                IdentificationId = identification.IdentificationId,
                Reference = id.ToString(),
                ReferenceType = identification.ReferenceType,
                Description = identification.Description,
                IdNumber = identification.IdNumber,
                Type = identification.Type,
                IssueDate = identification.IssueDate,
                ExpiryDate = identification.ExpiryDate,
                Issuer = identification.Issuer,
                Remark = identification.Remark,
                DateCreated = identification.DateCreated,
                LastModified = identification.LastModified,
                CreatedBy = identification.CreatedBy,
                ModifiedBy = identification.ModifiedBy,
                IssueDateIdAm = identification.IssueDateIdAm,
                ExpiryDateAm = identification.ExpiryDateAm

            });

            return Json(result);
        }
           [Authorize(Roles = HR_Profile_User + "," + HR_Training_User + "," + HR_Company_User)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Identifications_Create([DataSourceRequest]DataSourceRequest request, Identification identification, string identificationReferenceType, string id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.Identifications.Where(x => x.Type == identification.Type && x.Reference == id.ToString() && x.ReferenceType == identificationReferenceType).FirstOrDefault();
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "This Identification type is already registered!!");
                }
                    else if (ExpiryDate.Date <IssueDate.Date )
                {
                    ModelState.AddModelError("", "Please Check the date range!!");
                }
                else if (IssueDateAm==null || ExpiryDateAm==null)
                    {
                        ModelState.AddModelError("", "Date values are mandatory!!");
                    }
                else
                {
                    var entity = new Identification
                    {
                        Reference = id.ToString(),
                        ReferenceType = identificationReferenceType,
                        Description = identification.Description,
                        IdNumber = identification.IdNumber,
                        Type = identification.Type,
                        IssueDate = IssueDate,
                        ExpiryDate = ExpiryDate,
                        Issuer = identification.Issuer,
                        Remark = identification.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = identification.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = identification.ModifiedBy,
                        IssueDateIdAm = IssueDateAm,
                        ExpiryDateAm = ExpiryDateAm
                    };

                    db.Identifications.Add(entity);
                    db.SaveChanges();
                    identification.IdentificationId = entity.IdentificationId;
                }
            }

            return Json(new[] { identification }.ToDataSourceResult(request, ModelState));
        }
          [Authorize(Roles = HR_Profile_User + "," + HR_Training_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Identifications_Update([DataSourceRequest]DataSourceRequest request, Identification identification)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (IssueDateAm == null)
                {
                    IssueDateAm = identification.IssueDateIdAm;
                    var splitDate = IssueDateAm.Split('/');
                    IssueDate  = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ExpiryDateAm == null)
                {
                    ExpiryDateAm = identification.ExpiryDateAm;
                    var splitDate = ExpiryDateAm.Split('/');
                    ExpiryDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ExpiryDate.Date < IssueDate.Date)
                {
                    ModelState.AddModelError("", "Please Check the date range!!");
                }
                else
                {
                    var entity = new Identification
                    {
                        ReferenceType = identification.ReferenceType,
                        IdentificationId = identification.IdentificationId,
                        Reference = identification.Reference,
                        Description = identification.Description,
                        IdNumber = identification.IdNumber,
                        Type = identification.Type,
                        IssueDate = IssueDate,
                        ExpiryDate = ExpiryDate,
                        Issuer = identification.Issuer,
                        Remark = identification.Remark,
                        DateCreated = identification.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = identification.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        IssueDateIdAm = IssueDateAm,
                        ExpiryDateAm = ExpiryDateAm
                    };

                    db.Identifications.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { identification }.ToDataSourceResult(request, ModelState));
        }
                [Authorize(Roles = HR_Profile_Admin + "," +  HR_Company_Admin + ","  + HR_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Identifications_Destroy([DataSourceRequest]DataSourceRequest request, Identification identification)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Identification
                {
                    IdentificationId = identification.IdentificationId,
                    Reference = identification.Reference,
                    Description = identification.Description,
                    IdNumber = identification.IdNumber,
                    Type = identification.Type,
                    IssueDate = identification.IssueDate,
                    ExpiryDate = identification.ExpiryDate,
                    Issuer = identification.Issuer,
                    Remark = identification.Remark,
                    DateCreated = identification.DateCreated,
                    LastModified = identification.LastModified,
                    CreatedBy = identification.CreatedBy,
                    ModifiedBy = identification.ModifiedBy,
                    IssueDateIdAm = identification.IssueDateIdAm,
                    ExpiryDateAm = identification.ExpiryDateAm
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Identification" + identification.IdNumber.ToString() + " " + identification.Description);
                #endregion
                db.Identifications.Attach(entity);
                db.Identifications.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { identification }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
