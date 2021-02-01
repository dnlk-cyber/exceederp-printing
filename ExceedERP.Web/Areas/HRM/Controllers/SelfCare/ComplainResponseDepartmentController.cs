using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using ExceedERP.Core.Domain.Global;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    public class ComplainResponseDepartmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ComplainResponseDepartment
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }


        private void PopulateViewData()
        {
            var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = listt.Where(x => x.IsActive);
            ViewData["TypeList"] = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.COMPLAIN_TYPE);
            ViewData["Orgs"] = db.MainCompanyStructures.ToList();
            ViewData["CC"] = db.MainCompanyStructures.ToList();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
        public ActionResult ComplainResponseDepartment_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var place = db.Placements.FirstOrDefault(x=>x.EmployeeId == empId && x.isCurrent);
            IQueryable<Complain> complains = db.Complains.Where(x => x.RefersTo == place.OrgStructureId);
            DataSourceResult result = complains.ToDataSourceResult(request, complain => new
            {
                ComplainId = complain.ComplainId,
                EmployeeId = complain.EmployeeId,
                Date = complain.Date,
                DateAmharic = complain.DateAmharic,
                ComplainDescription = complain.ComplainDescription,
                Remark = complain.Remark,
                LookupId = complain.LookupId,
                RefersTo = complain.RefersTo
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ComplainResponseDepartment_Create([DataSourceRequest]DataSourceRequest request, Complain complain)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    ModelState.AddModelError("", "Date value is mandatory!!");
                }
                else
                {
                    var entity = new Complain
                    {
                        Date = Date,
                        DateAmharic = DateAmharic,
                        ComplainDescription = complain.ComplainDescription,
                        LookupId = complain.LookupId,
                        Remark = complain.Remark,
                        EmployeeId = complain.EmployeeId,
                        RefersTo = complain.RefersTo
                    };

                    db.Complains.Add(entity);
                    db.SaveChanges();
                    complain.ComplainId = entity.ComplainId;
                }
            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ComplainResponseDepartment_Update([DataSourceRequest]DataSourceRequest request, Complain complain)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = complain.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                var entity = new Complain
                {
                    ComplainId = complain.ComplainId,
                    EmployeeId = complain.EmployeeId,
                    Date = Date,
                    DateAmharic = DateAmharic,
                    ComplainDescription = complain.ComplainDescription,
                    Remark = complain.Remark,
                    LookupId = complain.LookupId,
                    RefersTo = complain.RefersTo
                };

                db.Complains.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ComplainResponseDepartment_Destroy([DataSourceRequest]DataSourceRequest request, Complain complain)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Complain
                {
                    ComplainId = complain.ComplainId,
                    Date = complain.Date,
                    DateAmharic = complain.DateAmharic,
                    ComplainDescription = complain.ComplainDescription,
                    Remark = complain.Remark,
                    LookupId = complain.LookupId,
                    EmployeeId = complain.EmployeeId,
                    RefersTo = complain.RefersTo
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Complain of employee " + complain.Person_Employee.FirstName + " " + complain.Person_Employee.MiddleName + " " + complain.Person_Employee.LastName);
                #endregion
                db.Complains.Attach(entity);
                db.Complains.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { complain }.ToDataSourceResult(request, ModelState));
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        public ActionResult UploadDocument(HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.DOCUMENT), txtValue);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.DOCUMENT));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {


                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = HRMInputs.EmployeeId.ToString(),
                            Description = file.FileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}