using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System.Web;
using System.IO;
using System.Net;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{
     [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
    public class DisciplinaryCaseCourtController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            GetMeasureDetail();
            GetMeasure();
            var periodList = db.GLPeriods.ToList();
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GLPeriod>();
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLPeriods.ToList();

            ViewData["PeriodList"] = periodList; //!= null ? list.ToList() : new List<PayrollPeriod>();

            return Json(periodList.Select(p => new { GLPeriodId = p.GLPeriodId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
      
        public void SetDate(string name, string value)
        {
            if (name == "TransgressionDateAm")
            {
                var TransgressionDateAm = value;
                var splitDate = TransgressionDateAm.Split('/');
                var TransgressionDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            
        }
        public void GetMeasureDetail()
        {
            var measureDetails = db.DisciplineMeasuresDetail;
            ViewData["MeasuresDetails"] = measureDetails;

        }

        public void GetMeasure()
        {

            var measurs = db.DisciplineMeasures;
            ViewData["Measures"] = measurs;
        }
       
        public ActionResult DisciplinaryCases_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<DisciplinaryCase> disciplinarycases = db.DisciplinaryCases.Where(x => employees.Contains(x.EmployeeId));
            DataSourceResult result = disciplinarycases.ToDataSourceResult(request, disciplinaryCase => new {
                DisciplinaryCaseId = disciplinaryCase.DisciplinaryCaseId,
                EmployeeId = disciplinaryCase.EmployeeId,
                TransgressionDate = disciplinaryCase.TransgressionDate,
                NatureofTransgression = disciplinaryCase.NatureofTransgression,
                Evidence = disciplinaryCase.Evidence,
                TransgressionDateAm = disciplinaryCase.TransgressionDateAm,
                EmployeeCode = disciplinaryCase.EmployeeCode,
                Remark = disciplinaryCase.Remark,
                DateCreated = disciplinaryCase.DateCreated,
                LastModified = disciplinaryCase.LastModified,
                CreatedBy = disciplinaryCase.CreatedBy,
                ModifiedBy = disciplinaryCase.ModifiedBy,
                OrgBranchName = disciplinaryCase.OrgBranchName,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplinaryCases_Create([DataSourceRequest]DataSourceRequest request, DisciplinaryCase disciplinaryCase)
        {
            ModelState.Clear();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(disciplinaryCase.TransgressionDateAm))
                {
                    ModelState.AddModelError("", " Date fields is  mandatory");
                    return Json(new[] { disciplinaryCase }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    disciplinaryCase.TransgressionDate = HrHelper.ParseEthiopicDate(disciplinaryCase.TransgressionDateAm);
                }

            }
            else
            {

                disciplinaryCase.TransgressionDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(disciplinaryCase.TransgressionDate.Day, disciplinaryCase.TransgressionDate.Month, disciplinaryCase.TransgressionDate.Year);

            }
            if (ModelState.IsValid)
            {
                
                var entity = new DisciplinaryCase
                {
                    EmployeeId = disciplinaryCase.EmployeeId ,
                    TransgressionDate = disciplinaryCase.TransgressionDate,
                    NatureofTransgression = disciplinaryCase.NatureofTransgression,
                    Evidence = disciplinaryCase.Evidence,
                    Remark = disciplinaryCase.Remark,
                    EmployeeCode =inputs. GetEmployees(User.Identity.Name ).Where (x=> x.PersonId== disciplinaryCase .EmployeeId ).FirstOrDefault ().EmployeeCode ,
                    TransgressionDateAm = disciplinaryCase.TransgressionDateAm,
                    DateCreated = DateTime .Now ,
                    LastModified = disciplinaryCase.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = disciplinaryCase.ModifiedBy,
                    OrgBranchName = UserInformationHelper .GetUserDefaultBranch (User .Identity .Name ),
                };

                db.DisciplinaryCases.Add(entity);
                db.SaveChanges();
                disciplinaryCase.DisciplinaryCaseId = entity.DisciplinaryCaseId;
            
            }

            return Json(new[] { disciplinaryCase }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplinaryCases_Update([DataSourceRequest]DataSourceRequest request, DisciplinaryCase disciplinaryCase)
        {
            ModelState.Clear();
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                if (string.IsNullOrEmpty(disciplinaryCase.TransgressionDateAm))
                {
                    ModelState.AddModelError("", " Date fields is  mandatory");
                    return Json(new[] { disciplinaryCase }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    disciplinaryCase.TransgressionDate = HrHelper.ParseEthiopicDate(disciplinaryCase.TransgressionDateAm);
                }

            }
            else
            {

                disciplinaryCase.TransgressionDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(disciplinaryCase.TransgressionDate.Day, disciplinaryCase.TransgressionDate.Month, disciplinaryCase.TransgressionDate.Year);

            }
            if (ModelState.IsValid)
            {
               
                    var entity = new DisciplinaryCase
                    {
                        DisciplinaryCaseId = disciplinaryCase.DisciplinaryCaseId,
                        EmployeeId = disciplinaryCase.EmployeeId,
                        TransgressionDate = disciplinaryCase.TransgressionDate,
                        NatureofTransgression = disciplinaryCase.NatureofTransgression,
                        Evidence = disciplinaryCase.Evidence,
                        Remark = disciplinaryCase.Remark,
                        TransgressionDateAm = disciplinaryCase.TransgressionDateAm,
                        EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == disciplinaryCase.EmployeeId).FirstOrDefault().EmployeeCode,
                        DateCreated = disciplinaryCase.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = disciplinaryCase.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        OrgBranchName = disciplinaryCase.OrgBranchName,
                    };

                    db.DisciplinaryCases.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                
            }

            return Json(new[] { disciplinaryCase }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplinaryCases_Destroy([DataSourceRequest]DataSourceRequest request, DisciplinaryCase disciplinaryCase)
        {
            ModelState.Clear();
            var DisciplineMeasures = db.EmpDisciplinaryHearing.Where(x => x.DisciplinaryCaseId == disciplinaryCase.DisciplinaryCaseId).FirstOrDefault();
            if (DisciplineMeasures != null)
            {

                ModelState.AddModelError("", "You cannot delete this Discipline Case.Please delete  Disciplinary Hearing information first.");
                return Json(new[] { disciplinaryCase }.ToDataSourceResult(request, ModelState));


            }
            var DisciplineMeasuresType = db.EmpDisciplineMeasure.Where(x => x.DisciplinaryCaseId == disciplinaryCase.DisciplinaryCaseId).FirstOrDefault();
            if (DisciplineMeasuresType != null)
            {

                ModelState.AddModelError("", "You cannot delete this Discipline Case.Please delete Discipline Measure information first");
                return Json(new[] { disciplinaryCase }.ToDataSourceResult(request, ModelState));


            }

            if (ModelState.IsValid)
            {
                
                    var entity = new DisciplinaryCase
                    {
                        DisciplinaryCaseId = disciplinaryCase.DisciplinaryCaseId,
                        EmployeeId = disciplinaryCase.EmployeeId,
                        TransgressionDate = disciplinaryCase.TransgressionDate,
                        NatureofTransgression = disciplinaryCase.NatureofTransgression,
                        Evidence = disciplinaryCase.Evidence,
                        Remark = disciplinaryCase.Remark,
                        TransgressionDateAm = disciplinaryCase.TransgressionDateAm,
                        EmployeeCode = disciplinaryCase.EmployeeCode,
                        DateCreated = disciplinaryCase.DateCreated,
                        LastModified = disciplinaryCase.LastModified,
                        CreatedBy = disciplinaryCase.CreatedBy,
                        ModifiedBy = disciplinaryCase.ModifiedBy,
                        OrgBranchName = disciplinaryCase.OrgBranchName,
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted disciplinary Case information of employee" + disciplinaryCase.EmployeeCode );
                    #endregion
                    db.DisciplinaryCases.Attach(entity);
                    db.DisciplinaryCases.Remove(entity);
                    db.SaveChanges();
                
            }

            return Json(new[] { disciplinaryCase }.ToDataSourceResult(request, ModelState));
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        public ActionResult DownloadFile(string filename)
        {
            if (HRMInputs.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.MINUTE && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
                            .FirstOrDefault();
                if (attachment != null)
                {
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.Description + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.MINUTE + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public ActionResult Upload(HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.MINUTE), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
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

                            Reference = HRMInputs.EmployeeId.ToString(),
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

       
        public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment
                {
                    AttachmentId = attachment.AttachmentId,
                    ReferenceType = attachment.ReferenceType,
                    Reference = attachment.Reference,
                    Description = attachment.Description,
                    URL = attachment.URL,
                    BinaryFile = attachment.BinaryFile,
                    FileType = attachment.FileType,
                    Index = attachment.Index,
                    Remark = attachment.Remark,
                    FileName = attachment.FileName
                };

                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
   

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
