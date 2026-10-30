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
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using System.Data.Entity.Validation;
using ExceedERP.Core.Domain.Common.HRM;
using System.Web;
using System.IO;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin,HrmConstants.HRCompanyUser,
                    HrmConstants.HRAdmin,HrmConstants.HRDepartmentHead,
                    HrmConstants.HREmployeeSelfService)]
    public class CommitteeController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {


            PopulateViewData();
            GetCommitteType();
            return View();
        }

        private void PopulateViewData()
        {
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
            ViewData["DisciplinaryCases"] = db.DisciplinaryCases.Select(x=>new
            {
                x.DisciplinaryCaseId,
                x.NatureofTransgression
            }).ToList();

            ViewData["committeType"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.COMMITTEE_TYPE).ToList();

        }

        public void GetCommitteType()
        {
            var committeTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.COMMITTEE_TYPE);
            ViewData["committeType"] = committeTypes;
        }
     
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult Committees_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Committee> committees = db.Committees;//.Where(x => employees.Contains(x.EmployeeId.ToString())); ;
            DataSourceResult result = committees.ToDataSourceResult(request, committee => new
            {

                CommitteeId = committee.CommitteeId,
                LookupId = committee.LookupId,
                IsActive = committee.IsActive,
                Remark = committee.Remark,
                DateCreated = committee.DateCreated,
                LastModified = committee.LastModified,
                CreatedBy = committee.CreatedBy,
                ModifiedBy = committee.ModifiedBy,
                VaccancyCode = committee.VaccancyCode,
                Description = committee.Description
            });

            return Json(result);
        }
  
        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Committees_Create([DataSourceRequest]DataSourceRequest request, Committee committee)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new Committee
                {

                    LookupId = committee.LookupId,
                    IsActive = committee.IsActive,
                    Remark = committee.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = committee.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = committee.ModifiedBy,
                    VaccancyCode = committee.VaccancyCode,
                    Description = committee.Description
                };


                try
                {
                    db.Committees.Add(entity);
                    db.SaveChanges();
                    committee.CommitteeId = entity.CommitteeId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                   
                        UserHelper.OperationLog(Request.UserHostAddress, "Committee ", "Success", User.Identity.Name, User.Identity.Name + " Create Committee for " + entity.Description,Modules.HRM);
                    

                    #endregion
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }
                //}
            }

            return Json(new[] { committee }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles( HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Committees_Update([DataSourceRequest]DataSourceRequest request, Committee committee)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Committee
                {
                    CommitteeId = committee.CommitteeId,
                    // EmployeeId = committee.EmployeeId,
                    //Role = committee.Role,
                    LookupId = committee.LookupId,
                    IsActive = committee.IsActive,
                    Remark = committee.Remark,
                    DateCreated = committee.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = committee.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    VaccancyCode = committee.VaccancyCode,
                    Description = committee.Description
                };

                db.Committees.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Committee ", "Success", User.Identity.Name, User.Identity.Name + " Updated Committee Info of " ,Modules.HRM);

                
                #endregion
            }

            return Json(new[] { committee }.ToDataSourceResult(request, ModelState));
        }
       
        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Committees_Destroy([DataSourceRequest]DataSourceRequest request, Committee committee)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Committee
                {
                    CommitteeId = committee.CommitteeId,
                    // EmployeeId = committee.EmployeeId,
                    // Role = committee.Role,
                    LookupId = committee.LookupId,
                    IsActive = committee.IsActive,
                    Remark = committee.Remark,
                    DateCreated = committee.DateCreated,
                    LastModified = committee.LastModified,
                    CreatedBy = committee.CreatedBy,
                    ModifiedBy = committee.ModifiedBy,
                    VaccancyCode = committee.VaccancyCode,
                    Description = committee.Description
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Commitee of  ");
                #endregion
                db.Committees.Attach(entity);
                db.Committees.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { committee }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult GetCommittee()
        {


            var list = db.VaccancyPlans.ToList();


            return Json(list.Select(p => new { VaccancyCode = p.VaccancyCode }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(
                            Server.MapPath("~/Content/Images/" + "0" + "-" +
                                           AttachmentType.MINUTE), fileName);
                    var mainPath =
                        Path.Combine(
                            Server.MapPath("~/Content/Images/" + "0" + "-" +
                                           AttachmentType.MINUTE));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {


                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = HRMInputs.Owner,
                            Description = file.FileName,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.MINUTE,
                            Index = 0,
                            URL = path,
                            Remark = HRMInputs.Owner

                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public FileResult Download(string data, int id)
        {
            return File("~/Content/Images/" + data, System.Net.Mime.MediaTypeNames.Application.Octet);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
