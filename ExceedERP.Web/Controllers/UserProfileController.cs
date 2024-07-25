using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.Finance.Controllers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;

namespace ExceedERP.Web.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private ApplicationDbContext userDb = new ApplicationDbContext();
        private ExceedDbContext appDb = new ExceedDbContext();

        // GET: UserProfile
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var branches = appDb.Branch;
            ViewData["Branches"] = branches;

            var businessUnits = appDb.BusinessUnits;
            ViewData["BusinessUnits"] = businessUnits;

            ViewData["CC"] = appDb.MainCompanyStructures.ToList();

            var periodList = appDb.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var list = from p in periodList
                       join g in appDb.GLPeriods
                           on p.Name equals g.Name
                       select (new
                       {
                           g
                       });
            ViewData["PeriodList"] = appDb.GLPeriods.ToList();// list.Select(x => x.g).ToList(); ;
                                                           // var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = appDb.Person_Employee.ToList();

        }

        public ActionResult MyBranches_Read([DataSourceRequest]DataSourceRequest request, string type)
        {
            string userId = User.Identity.GetUserId();

            IQueryable<UserBranchGroup> accountChekerVms =userDb.UserBranches.Where(x=>x.ApplicationUserId== userId).AsQueryable();
            DataSourceResult result = accountChekerVms.ToDataSourceResult(request, accountChekerVm => new
            {
                Id = accountChekerVm.Id,
                UserBranchId = accountChekerVm.UserBranchId,
                IsDefault = accountChekerVm.IsDefault,
                
            });

            return Json(result);
        }

        public ActionResult MyBusinessUnits_Read([DataSourceRequest]DataSourceRequest request, string type)
        {
            string userId = User.Identity.GetUserId();

            IQueryable<UserBusinessUnitMap> accountChekerVms = appDb.UserBusinessUnitMaps.Where(x => x.UserId == userId).AsQueryable();
            DataSourceResult result = accountChekerVms.ToDataSourceResult(request, accountChekerVm => new
            {
                UserBusinessUnitMapId = accountChekerVm.UserBusinessUnitMapId,
                UserId = accountChekerVm.UserId,
                BusinessUnitId = accountChekerVm.BusinessUnitId,
                IsDefault = accountChekerVm.IsDefault,

            });

            return Json(result);
        }
        public JsonResult MakeDefault(int id)
        {
            string userId = User.Identity.GetUserId();
           
            UserBranchGroup mapping = userDb.UserBranches.Find(id);
            if (mapping != null)
            {
                //make all non-default
                var userBranches = userDb.UserBranches.Where(x => x.ApplicationUserId == userId).ToList();
                foreach (var map in userBranches)
                {
                    map.IsDefault = false;
                    userDb.Entry(map).State = EntityState.Modified;
                    userDb.SaveChanges();
                }

                mapping.IsDefault = true;
                userDb.Entry(mapping).State = EntityState.Modified;
                userDb.SaveChanges();

                #region log
                int did = int.Parse(mapping.UserBranchId);
                var HRbranch = appDb.Branch.FirstOrDefault(p => p.BranchId == did);
                UserHelper.AccessLog(Request.UserHostAddress, "User branch", "Success", User.Identity.Name, $"Updated user default branch for user  {User.Identity.Name} to {HRbranch.Name} ");
                #endregion
                var formattedData = new
                {
                    Message = "Default updated successfully.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var formattedData = new
                {
                    Message = "Mapping not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult MakeBizDefault(int id)
        {
            string userId = User.Identity.GetUserId();

            UserBusinessUnitMap mapping = appDb.UserBusinessUnitMaps.Find(id);
            if (mapping != null)
            {
                //make all non-default
                var userBusinessUnits = appDb.UserBusinessUnitMaps.Where(x => x.UserId == userId).ToList();
                foreach (var map in userBusinessUnits)
                {
                    map.IsDefault = false;
                    appDb.Entry(map).State = EntityState.Modified;
                    appDb.SaveChanges();
                }

                mapping.IsDefault = true;
                appDb.Entry(mapping).State = EntityState.Modified;
                appDb.SaveChanges();

                #region log
                var HRBusinessUnit = appDb.BusinessUnits.FirstOrDefault(p => p.BusinessUnitId ==mapping.BusinessUnitId );
                UserHelper.AccessLog(Request.UserHostAddress, "User business unit", "Success", User.Identity.Name, $"Updated user default business unit for user  {User.Identity.Name} to {HRBusinessUnit.Name} ");
                #endregion
                var formattedData = new
                {
                    Message = "Default updated successfully.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var formattedData = new
                {
                    Message = "Mapping not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Signatures_Read([DataSourceRequest]DataSourceRequest request, string Id)
        {
            var userName = User.Identity.Name;
            var user = userDb.Users.Where(q => q.UserName == userName).FirstOrDefault();
            string id = "";
            id = user?.Id;
            if (Id!=null)
            {
                id = Id;
            }
            

                IQueryable<ApplicationUserSignature> signatures = userDb.ApplicationUserSignatures.Where(q => q.UserId == id);
                DataSourceResult result = signatures.ToDataSourceResult(request, signature => new
                {
                    ApplicationUserSignatureId = signature.ApplicationUserSignatureId,
                    UserId = signature.UserId,
                    Sign = signature.Sign,
                    Image64 = signature.Image64
                });

                return Json(result);
           
        }

        public ActionResult Signatures_Create([DataSourceRequest]DataSourceRequest request, ApplicationUserSignature signature)
        {
            if (ModelState.IsValid)
            {

                var userName = User.Identity.Name;
                var user = userDb.Users.Where(q => q.UserName == userName).FirstOrDefault();
                string id = user?.Id;
                var sign = userDb.ApplicationUserSignatures.Where(q => q.UserId == id).FirstOrDefault();
                signature.ApplicationUserSignatureId = Guid.NewGuid().ToString();
                var entity = (ApplicationUserSignature)this.GetObject(signature);
                if (sign == null)
                {
                    userDb.ApplicationUserSignatures.Add(entity);

                }
                else
                {
                    sign.Sign = entity.Sign;
                    userDb.ApplicationUserSignatures.Attach(sign);
                    userDb.Entry(sign).State = EntityState.Modified;
                }
                userDb.SaveChanges();
                signature.ApplicationUserSignatureId = entity.ApplicationUserSignatureId;
            }
            // return RedirectToAction("DisplaySignature");

            return Json(new[] { signature }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Upload_Signature([DataSourceRequest]DataSourceRequest request, HttpPostedFileBase signatureDocument)
        {
            ApplicationUserSignature sign1 = new ApplicationUserSignature();
            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                var user = userDb.Users.Where(q => q.UserName == userName).FirstOrDefault();
                string id = user?.Id;
                int fileSizeInBytes = signatureDocument.ContentLength;
                byte[] firma = null;
                using (var br = new BinaryReader(signatureDocument.InputStream))
                {
                    firma = br.ReadBytes(fileSizeInBytes);
                }
                var sign = userDb.ApplicationUserSignatures.Where(q => q.UserId == id).FirstOrDefault();
                if (sign == null)
                {
                    ApplicationUserSignature signature = new ApplicationUserSignature();
                    signature.UserId = id;
                    signature.Sign = firma;
                    userDb.ApplicationUserSignatures.Add(signature);
                    userDb.SaveChanges();
                    return Json(new[] { signature }.ToDataSourceResult(request, ModelState));


                }
                else
                {
                    sign.Sign = firma;
                    userDb.ApplicationUserSignatures.Attach(sign);
                    userDb.Entry(sign).State = EntityState.Modified;
                    userDb.SaveChanges();
                    return Json(new[] { sign }.ToDataSourceResult(request, ModelState));

                }
            }

            return Json(new[] { sign1 }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Signatures_Update([DataSourceRequest]DataSourceRequest request, ApplicationUserSignature signature)
        {

            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                var user = userDb.Users.Where(q => q.UserName == userName).FirstOrDefault();
                string id = user?.Id;
                var entity = (ApplicationUserSignature)this.GetObject(signature);

                userDb.ApplicationUserSignatures.Attach(entity);
                userDb.Entry(entity).State = EntityState.Modified;
                userDb.SaveChanges();
            }

            return Json(new[] { signature }.ToDataSourceResult(request, ModelState));
        }
        //by admin
        [Authorize(Roles = "Admin")]
        public ActionResult Signatures_CreateAdmin([DataSourceRequest]DataSourceRequest request, ApplicationUserSignature signature,string id)
        {
            if (ModelState.IsValid)
            {
                var sign = userDb.ApplicationUserSignatures.Where(q => q.UserId == id).FirstOrDefault();
                signature.ApplicationUserSignatureId = Guid.NewGuid().ToString();
                var entity = (ApplicationUserSignature)GetObjectAdmin(signature,id);
                if (sign == null)
                {
                    userDb.ApplicationUserSignatures.Add(entity);

                }
                else
                {
                    sign.Sign = entity.Sign;
                    userDb.ApplicationUserSignatures.Attach(sign);
                    userDb.Entry(sign).State = EntityState.Modified;
                }
                userDb.SaveChanges();
                signature.ApplicationUserSignatureId = entity.ApplicationUserSignatureId;
                #region log
                var user = userDb.Users.Find(id);
                UserHelper.AccessLog(Request.UserHostAddress, "User signature", "Success", User.Identity.Name, $"Updated user signature for user {user.UserName} ");
                #endregion
            }
            // return RedirectToAction("DisplaySignature");

            return Json(new[] { signature }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Upload_SignatureAdmin([DataSourceRequest]DataSourceRequest request, HttpPostedFileBase signatureDocument, string id)
        {
            ApplicationUserSignature sign1 = new ApplicationUserSignature();
            if (ModelState.IsValid)
            {
                
                int fileSizeInBytes = signatureDocument.ContentLength;
                byte[] firma = null;
                using (var br = new BinaryReader(signatureDocument.InputStream))
                {
                    firma = br.ReadBytes(fileSizeInBytes);
                }
                var sign = userDb.ApplicationUserSignatures.Where(q => q.UserId == id).FirstOrDefault();
                if (sign == null)
                {
                    ApplicationUserSignature signature = new ApplicationUserSignature();
                    signature.UserId = id;
                    signature.Sign = firma;
                    userDb.ApplicationUserSignatures.Add(signature);
                    userDb.SaveChanges();

                    #region log
                    var user = userDb.Users.Find(id);
                    UserHelper.AccessLog(Request.UserHostAddress, "User signature", "Success", User.Identity.Name, $"Updated user signature for user {user.UserName} ");
                    #endregion
                    return Json(new[] { signature }.ToDataSourceResult(request, ModelState));


                }
                else
                {
                    sign.Sign = firma;
                    userDb.ApplicationUserSignatures.Attach(sign);
                    userDb.Entry(sign).State = EntityState.Modified;
                    userDb.SaveChanges();

                    #region log
                    var user = userDb.Users.Find(id);
                    UserHelper.AccessLog(Request.UserHostAddress, "User signature", "Success", User.Identity.Name, $"Updated user signature for user {user.UserName} ");
                    #endregion
                    return Json(new[] { sign }.ToDataSourceResult(request, ModelState));

                }
            }

            return Json(new[] { sign1 }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles = "Admin")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Signatures_UpdateAdmin([DataSourceRequest]DataSourceRequest request, ApplicationUserSignature signature,string id)
        {

            if (ModelState.IsValid)
            {
                var entity = (ApplicationUserSignature)this.GetObjectAdmin(signature,id);

                userDb.ApplicationUserSignatures.Attach(entity);
                userDb.Entry(entity).State = EntityState.Modified;
                userDb.SaveChanges();

                #region log
                var user = userDb.Users.Find(id);
                UserHelper.AccessLog(Request.UserHostAddress, "User signature", "Success", User.Identity.Name, $"Updated user signature for user {user.UserName} ");
                #endregion
            }

            return Json(new[] { signature }.ToDataSourceResult(request, ModelState));
        }
        [Authorize(Roles = "Admin")]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Signatures_Destroy([DataSourceRequest]DataSourceRequest request, ApplicationUserSignature signature)
        {

            if (ModelState.IsValid)
            {
                var entity = (ApplicationUserSignature)this.GetObject(signature);

                userDb.ApplicationUserSignatures.Attach(entity);
                userDb.ApplicationUserSignatures.Remove(entity);
                userDb.SaveChanges();
                #region log
                var user = userDb.Users.Find(signature.UserId);
                UserHelper.AccessLog(Request.UserHostAddress, "User signature", "Success", User.Identity.Name, $"Deleted user signature for user {user.UserName} ");
                #endregion
            }

            return Json(new[] { signature }.ToDataSourceResult(request, ModelState));
        }

        private object GetObjectAdmin(ApplicationUserSignature signature,string id)
        {          
           
            var entity = new ApplicationUserSignature
            {
                ApplicationUserSignatureId = signature.ApplicationUserSignatureId,
                UserId = id,
                Sign = signature.Sign

            };
            return entity;
        }
        private object GetObject(ApplicationUserSignature signature)
        {
            var userName = User.Identity.Name;
            var user = userDb.Users.Where(q => q.UserName == userName).FirstOrDefault();
            string id = user?.Id;

            var entity = new ApplicationUserSignature
            {
                ApplicationUserSignatureId = signature.ApplicationUserSignatureId,
                UserId = id,
                Sign = signature.Sign

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                userDb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
