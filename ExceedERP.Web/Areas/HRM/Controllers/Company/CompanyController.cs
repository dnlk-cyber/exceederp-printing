using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class CompanyController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string AmharicName { get; set; }
        string ErrorMessage = "";
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var periodList = db.Periods;
            // var accountList = db.Accounts;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<Period>();
            // ViewData["AccountList"] = accountList != null ? accountList.ToList() : new List<Account>();
            var BusinessTypes = Enum.GetValues(typeof(BusinessType))
                .Cast<BusinessType>()
                .ToList();
            var businessList = BusinessTypes.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["BusinessTypes"] = businessList;
        }
        public ActionResult DownloadFile(String filename)
        {
            //if (HRMInputs.SelectedFile == 0)
            //{
            //    string messages = "Please select the row first!!";
            //    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            //}
            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
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
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.LOGO + "/" + attachment.Description));
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

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult Companies_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MainCompany> companies = db.Companies.Where(c => c.Category == "0" && c.IsActive);
            DataSourceResult result = companies.ToDataSourceResult(request, mainCompany => new
            {
                MainCompanyId = mainCompany.MainCompanyId,
                TradeName = mainCompany.TradeName,
                BrandName = mainCompany.BrandName,
                BusinessType = mainCompany.BusinessType,
                Category = mainCompany.Category,
                IsActive = mainCompany.IsActive,
                AmharicName = mainCompany.AmharicName,
                Abbreviation = mainCompany.Abbreviation,
                DateCreated = mainCompany.DateCreated,
                LastModified = mainCompany.LastModified,
                CreatedBy = mainCompany.CreatedBy,
                ModifiedBy = mainCompany.ModifiedBy
            });

            return Json(result);
        }
        private bool CheckCompanyValidation(int id)
        {
            ErrorMessage = "";

            #region Identification
            var identifications = db.Identifications.Where(x => x.Reference == id.ToString() && x.ReferenceType == "Company").FirstOrDefault();
            if (identifications != null)
            {
                ErrorMessage = "You cannot delete this Comapny. Please Delete Identification information first.";
                return false;
            }
            #endregion
            #region Address
            var Address = db.Addresses.Where(x => x.Reference == id.ToString() && x.ReferenceType == "Company").FirstOrDefault();
            if (Address != null)
            {
                ErrorMessage = "You cannot delete this Comapny. Please Delete Address information first.";
                return false;
            }
            #endregion
            #region IndustryInvolvemnt
            var IndustryInvolvemnt = db.IndustryInvolveds.Where(x => x.Reference == id.ToString() && x.ReferenceType == "Company").FirstOrDefault();
            if (IndustryInvolvemnt != null)
            {
                ErrorMessage = "You cannot delete this Comapny. Please Delete Industry Involvement information first.";
                return false;
            }
            #endregion
            #region Attachment
            var Attachment = db.Attachments.Where(x => x.Reference == id.ToString() && x.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
            if (Attachment != null)
            {
                ErrorMessage = "You cannot delete this Comapny. Please Delete Logo information first.";
                return false;
            }
            #endregion
            return true;
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Companies_Create([DataSourceRequest]DataSourceRequest request, MainCompany mainCompany)
        {
            ModelState.Clear();
            // MainCompanyStructure mainCompanyStructure = new MainCompanyStructure();

            var companes = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (companes != null)
            {
                ModelState.AddModelError("", "The main Company is already maintained.");
            }

            if (ModelState.IsValid)
            {
                    var entity = new MainCompany
                    {
                        TradeName = mainCompany.TradeName,
                        BrandName = mainCompany.BrandName,
                        BusinessType = mainCompany.BusinessType,
                        Category = "0",
                        IsActive = true,
                        AmharicName = mainCompany.AmharicName,
                        Abbreviation = mainCompany.Abbreviation,
                        DateCreated = DateTime.Now,
                        LastModified = mainCompany.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = mainCompany.ModifiedBy
                    };

                    var lookup = new Structure
                    {
                        Name = "Head Office",
                    };

                    db.Structure.Add(lookup);
                    db.SaveChanges();

                    var mainCompanyStructureEntity = new MainCompanyStructure
                    {
                        Name = mainCompany.TradeName + "(" + mainCompany.AmharicName + ")",
                        StructureId = lookup.StructureId
                    };
                    //var structure = db.Structure.FirstOrDefault(x => x.Name == "Head Office");
                    //if (structure != null)
                    //{
                    //  mainCompanyStructureEntity.StructureId = structure.StructureId;
                    //}
                    db.MainCompanyStructures.Add(mainCompanyStructureEntity);
                    db.SaveChanges();

                    db.Companies.Add(entity);
                    db.SaveChanges();
                    mainCompany.MainCompanyId = entity.MainCompanyId;

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                    UserHelper.OperationLog(Request.UserHostAddress, "Maintain Company ", "Success", User.Identity.Name, User.Identity.Name + " Mainitain Company called " + entity.AmharicName, Modules.HRM);

                    #endregion
            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));
        }
        public void GetAmharicName(string car)
        {
            AmharicName = car;
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Companies_Update([DataSourceRequest]DataSourceRequest request, MainCompany mainCompany)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (AmharicName == null) AmharicName = mainCompany.AmharicName;
                var entity = new MainCompany
                {
                    MainCompanyId = mainCompany.MainCompanyId,
                    TradeName = mainCompany.TradeName,
                    BrandName = mainCompany.BrandName,
                    BusinessType = mainCompany.BusinessType,
                    Category = "0",
                    IsActive = mainCompany.IsActive,
                    AmharicName = mainCompany.AmharicName,
                    Abbreviation = mainCompany.Abbreviation,
                    DateCreated = mainCompany.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = mainCompany.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.Companies.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "Maintain Company ", "Success", User.Identity.Name, User.Identity.Name + " Updated Company Info of " + entity.AmharicName, Modules.HRM);


                #endregion
            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Companies_Destroy([DataSourceRequest]DataSourceRequest request, MainCompany mainCompany)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckCompanyValidation(mainCompany.MainCompanyId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new MainCompany
                    {
                        MainCompanyId = mainCompany.MainCompanyId,
                        TradeName = mainCompany.TradeName,
                        BrandName = mainCompany.BrandName,
                        BusinessType = mainCompany.BusinessType,
                        Category = "0",
                        IsActive = mainCompany.IsActive,
                        AmharicName = mainCompany.AmharicName,
                        Abbreviation = mainCompany.Abbreviation,
                        DateCreated = mainCompany.DateCreated,
                        LastModified = mainCompany.LastModified,
                        CreatedBy = mainCompany.CreatedBy,
                        ModifiedBy = mainCompany.ModifiedBy
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Company" + mainCompany.TradeName + " " + mainCompany.BrandName);
                    #endregion
                    db.Companies.Attach(entity);
                    db.Companies.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { mainCompany }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName).Split('.')[1];
                    var path =
                        Path.Combine(
                            Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.LOGO), "Logo" + "." + fileName);
                    var mainPath =
                        Path.Combine(
                            Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.LOGO));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (System.IO.File.Exists(path))
                    {

                        try
                        {
                            System.IO.DirectoryInfo di = new DirectoryInfo(mainPath);

                            foreach (FileInfo filee in di.GetFiles())
                            {
                                filee.Delete();
                            }
                            //System.IO.File.Delete(path);
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);
                            //  return;
                        }
                    }


                    file.SaveAs(path);
                    var checkExistance =
                       db.Attachments.Where(
                           x =>
                               x.Reference == HRMInputs.EmployeeId.ToString() &&

                               x.ReferenceType == AttachmentType.LOGO).ToList();
                    if (checkExistance != null)
                    {
                        db.Attachments.RemoveRange(checkExistance);
                        db.SaveChanges();
                    }
                    db = new ExceedDbContext();
                    byte[] image = System.IO.File.ReadAllBytes(path);
                    var entity = new Attachment
                    {

                        Reference = HRMInputs.EmployeeId.ToString(),
                        Description = "Logo" + "." + fileName,
                        FileType = file.ContentType,
                        ReferenceType = AttachmentType.LOGO,
                        BinaryFile = image,
                        Index = 0,
                        URL = path,
                        Remark = HRMInputs.Owner

                    };
                    db.Attachments.Add(entity);

                    db.SaveChanges();
                    ViewBag.Message = "Upload successful";
                }

                //}
                //else
                //{

                //    ViewBag.Message = "The file is existed";

                //}
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
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
                    Remark = attachment.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + attachment.ReferenceType.ToString() + " " + attachment.Description);
                #endregion
                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
                if (System.IO.File.Exists(attachment.URL))
                {

                    try
                    {
                        System.IO.File.Delete(attachment.URL);
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);
                        //  return;
                    }
                }
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
