//﻿using System;
//using System.Collections.Generic;
//
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
//using ExceedERP.Core.Domain.FixedAsset.UserCard;
//﻿using ExceedERP.DataAccess.Context;


//namespace ExceedERP.Web.Areas.FixedAsset.Controllers
//{
//    public class UserCardController : Controller
//    {
//        private ExceedDbContext db = new ExceedDbContext();

//        public ActionResult Index()
//        {
//            return View();
//        }

//        public ActionResult UserCards_Read([DataSourceRequest]DataSourceRequest request)
//        {
//            IQueryable<UserCard> usercards = db.UserCards;
//            DataSourceResult result = usercards.ToDataSourceResult(request, userCard => new {
//                UserCardID = userCard.UserCardID,
//                PersonID = userCard.PersonID,
//                EmployeeName = userCard.EmployeeName,
//                PostRefNo = userCard.PostRefNo,
//                BranchName = userCard.BranchName,
//                Process = userCard.Process,
//                EmpJobTitle = userCard.EmpJobTitle,
//                EmpOffice = userCard.EmpOffice,
//                Bulding = userCard.Bulding,
                
//                OrgBranchName = userCard.OrgBranchName,
//                DateCreated = userCard.DateCreated,
//                LastModified = userCard.LastModified,
//                CreatedBy = userCard.CreatedBy,
//                ModifiedBy = userCard.ModifiedBy
//            });

//            return Json(result);
//        }


//        public ActionResult UserCardItems_Read([DataSourceRequest]DataSourceRequest request, int UserCardID)
//        {
//            IQueryable<UserCardItems> usercarditems = db.UserCardItems.Where(x => x.UserCardID == UserCardID);
//            DataSourceResult result = usercarditems.ToDataSourceResult(request, userCardItems => new
//            {
//                UserCardItemsID = userCardItems.UserCardItemsID,
//                ItemDesc = userCardItems.ItemDesc,
//                UOM = userCardItems.UOM,
//                Quanity = userCardItems.Quanity,
//                UnitPrice = userCardItems.UnitPrice,
//                TagNumber = userCardItems.TagNumber,
//                SerialPartNo = userCardItems.SerialPartNo,
//                IssueDate = userCardItems.IssueDate,
//                SIVNo = userCardItems.SIVNo,
//                ReturnDate = userCardItems.ReturnDate,
//                SRNNo = userCardItems.SRNNo,
//                Remark = userCardItems.Remark,

//                OrgBranchName = userCardItems.OrgBranchName,
//                DateCreated = userCardItems.DateCreated,
//                LastModified = userCardItems.LastModified,
//                CreatedBy = userCardItems.CreatedBy,
//                ModifiedBy = userCardItems.ModifiedBy
//            });

//            return Json(result);
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult UserCardItems_Create(int UserCardID, [DataSourceRequest]DataSourceRequest request, UserCardItems userCardItems)
//        {
//            var currentUsername = !string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name) ? System.Web.HttpContext.Current.User.Identity.Name : "Anonymous";
//            if (ModelState.IsValid)
//            {
//                var entity = new UserCardItems
//                {
//                    UserCardID = userCardItems.UserCardID,
//                    ItemDesc = userCardItems.ItemDesc,
//                    UOM = userCardItems.UOM,
//                    Quanity = userCardItems.Quanity,
//                    UnitPrice = userCardItems.UnitPrice,
//                    TagNumber = userCardItems.TagNumber,
//                    SerialPartNo = userCardItems.SerialPartNo,
//                    IssueDate = userCardItems.IssueDate,
//                    SIVNo = userCardItems.SIVNo,
//                    ReturnDate = userCardItems.ReturnDate,
//                    SRNNo = userCardItems.SRNNo,
//                    Remark = userCardItems.Remark,

//                    OrgBranchName = userCardItems.OrgBranchName,
//                    DateCreated = DateTime.Now,
//                    LastModified = userCardItems.LastModified,
//                    CreatedBy = currentUsername,
//                    ModifiedBy = userCardItems.ModifiedBy
//                };

//                db.UserCardItems.Add(entity);
//                db.SaveChanges();
//                userCardItems.UserCardItemsID = entity.UserCardItemsID;
//            }

//            return Json(new[] { userCardItems }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult UserCards_Create([DataSourceRequest]DataSourceRequest request, UserCard userCard)
//        {
//            var currentUsername = !string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name) ? System.Web.HttpContext.Current.User.Identity.Name : "Anonymous";
//            if (ModelState.IsValid)
//            {
//                var entity = new UserCard
//                {
//                    PersonID = userCard.PersonID,
//                    EmployeeName = userCard.EmployeeName,
//                    PostRefNo = userCard.PostRefNo,
//                    BranchName = userCard.BranchName,
//                    Process = userCard.Process,
//                    EmpJobTitle = userCard.EmpJobTitle,
//                    EmpOffice = userCard.EmpOffice,
//                    Bulding = userCard.Bulding,

//                    OrgBranchName = userCard.OrgBranchName,
//                    DateCreated = DateTime.Now,
//                    LastModified = userCard.LastModified,
//                    CreatedBy = currentUsername,
//                    ModifiedBy = userCard.ModifiedBy
//                };

//                db.UserCards.Add(entity);
//                db.SaveChanges();
//                userCard.UserCardID = entity.UserCardID;
//            }

//            return Json(new[] { userCard }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult UserCards_Update([DataSourceRequest]DataSourceRequest request, UserCard userCard)
//        {
//            var currentUsername = !string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name) ? System.Web.HttpContext.Current.User.Identity.Name : "Anonymous";
//            if (ModelState.IsValid)
//            {
//                var entity = new UserCard
//                {
//                    UserCardID = userCard.UserCardID,
//                    PersonID = userCard.PersonID,
//                    EmployeeName = userCard.EmployeeName,
//                    PostRefNo = userCard.PostRefNo,
//                    BranchName = userCard.BranchName,
//                    Process = userCard.Process,
//                    EmpJobTitle = userCard.EmpJobTitle,
//                    EmpOffice = userCard.EmpOffice,
//                    Bulding = userCard.Bulding,

//                    OrgBranchName = userCard.OrgBranchName,
//                    DateCreated = userCard.DateCreated,
//                    LastModified = DateTime.Now.Date,
//                    CreatedBy = userCard.CreatedBy,
//                    ModifiedBy = currentUsername
//                };

//                db.UserCards.Attach(entity);
//                db.Entry(entity).State = EntityState.Modified;
//                db.SaveChanges();
//            }

//            return Json(new[] { userCard }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult UserCards_Destroy([DataSourceRequest]DataSourceRequest request, UserCard userCard)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = new UserCard
//                {
//                    UserCardID = userCard.UserCardID,
//                    PersonID = userCard.PersonID,
//                    EmployeeName = userCard.EmployeeName,
//                    PostRefNo = userCard.PostRefNo,
//                    BranchName = userCard.BranchName,
//                    Process = userCard.Process,
//                    EmpJobTitle = userCard.EmpJobTitle,
//                    EmpOffice = userCard.EmpOffice,
//                    Bulding = userCard.Bulding,
                    
//                    OrgBranchName = userCard.OrgBranchName,
//                    DateCreated = userCard.DateCreated,
//                    LastModified = userCard.LastModified,
//                    CreatedBy = userCard.CreatedBy,
//                    ModifiedBy = userCard.ModifiedBy
//                };

//                db.UserCards.Attach(entity);
//                db.UserCards.Remove(entity);
//                db.SaveChanges();
//            }

//            return Json(new[] { userCard }.ToDataSourceResult(request, ModelState));
//        }

//        [HttpPost]
//        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
//        {
//            var fileContents = Convert.FromBase64String(base64);

//            return File(fileContents, contentType, fileName);
//        }
    
//        [HttpPost]
//        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
//        {
//            var fileContents = Convert.FromBase64String(base64);

//            return File(fileContents, contentType, fileName);
//        }

//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}
