using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Construction;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Common;
using System.IO;
using System.Web;
using System.Net;

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    [AuthorizeRoles(ConstructionConstants.ContractManagementUser)]
    public class ProjectSubContractController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var orderTypeList = db.VariationOrderType.ToList();
            ViewData["types"] = orderTypeList;

            var claimType = db.ContractClaimType.ToList();
            ViewData["claim_types"] = claimType;

            this.UpdateSiteHandOverTime();

            this.UpdateTimeElapsedToDate();
            return View();
        }

        public ActionResult SubContrat_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProjectSubContract> subcontrat = db.SubContrat.Where( x=> x.ProjectInformationId == id);
            DataSourceResult result = subcontrat.ToDataSourceResult(request, projectSubContract => new {
                ProjectContractId = projectSubContract.ProjectContractId,
                WorkType = projectSubContract.WorkType,
                SubContractor = projectSubContract.SubContractor,

                ContractSigningDate = projectSubContract.ContractSigningDate,
                CommencementDate = projectSubContract.CommencementDate,
                // project amount without any rebate, variation and supplemetary
                ContractAmount = projectSubContract.ContractAmount,
                // completion date without any time extensions
                ContractTime = projectSubContract.ContractTime,
                ProvisionalSum = projectSubContract.ProvisionalSum,

                SiteHandoverTime = projectSubContract.SiteHandoverTime,
                ContractPeriod = projectSubContract.ContractPeriod,
                ApprovedTimeExtension = projectSubContract.ApprovedTimeExtension,
                TotalProjectPeriod = projectSubContract.TotalProjectPeriod,
                RevisedCompletionDate = projectSubContract.RevisedCompletionDate,
                TimeElapsedToDate = projectSubContract.TimeElapsedToDate,
                Rebate = projectSubContract.Rebate,
                Variation = projectSubContract.Variation,
                Supplemetary = projectSubContract.Supplemetary,
                TotalProjectAmount = projectSubContract.TotalProjectAmount,
                NetGrand = projectSubContract.NetGrand,
                ExecutedToDate = projectSubContract.ExecutedToDate,

                ProjectInformationId = projectSubContract.ProjectInformationId,

                OrgBranchName = projectSubContract.OrgBranchName,
                DateCreated = projectSubContract.DateCreated,
                LastModified = projectSubContract.LastModified,
                CreatedBy = projectSubContract.CreatedBy,
                ModifiedBy = projectSubContract.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContrat_Create([DataSourceRequest]DataSourceRequest request, ProjectSubContract projectSubContract, int idTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectSubContract
                {
                    WorkType = projectSubContract.WorkType,
                    SubContractor = projectSubContract.SubContractor,

                    ContractSigningDate = projectSubContract.ContractSigningDate,
                    CommencementDate = projectSubContract.CommencementDate,
                    // project amount without any rebate, variation and supplemetary
                    ContractAmount = projectSubContract.ContractAmount,
                    // completion date without any time extensions
                    ContractTime = projectSubContract.ContractTime,
                    ProvisionalSum = projectSubContract.ProvisionalSum,

                    SiteHandoverTime = projectSubContract.SiteHandoverTime,
                    ContractPeriod = projectSubContract.ContractPeriod,
                    ApprovedTimeExtension = projectSubContract.ApprovedTimeExtension,
                    TotalProjectPeriod = projectSubContract.TotalProjectPeriod,
                    RevisedCompletionDate = projectSubContract.RevisedCompletionDate,
                    TimeElapsedToDate = projectSubContract.TimeElapsedToDate,
                    Rebate = projectSubContract.Rebate,
                    Variation = projectSubContract.Variation,
                    Supplemetary = projectSubContract.Supplemetary,
                    TotalProjectAmount = projectSubContract.TotalProjectAmount,
                    NetGrand = projectSubContract.NetGrand,
                    ExecutedToDate = projectSubContract.ExecutedToDate,

                    ProjectInformationId = idTwo ,
                    OrgBranchName = projectSubContract.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectSubContract.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = projectSubContract.ModifiedBy
                };
                entity.TotalProjectPeriod = entity.ApprovedTimeExtension == null ? entity.ContractPeriod : entity.ContractPeriod + entity.ApprovedTimeExtension;
                entity.ContractTime = entity.ContractSigningDate.AddDays((double)entity.ContractPeriod);
                entity.RevisedCompletionDate = entity.ContractSigningDate.AddDays(((double)entity.TotalProjectPeriod));
                if (entity.Variation == null && entity.Supplemetary == null)
                {
                    entity.TotalProjectAmount = entity.ContractAmount;
                }
                else if (entity.Variation != null && entity.Supplemetary == null)
                {
                    entity.TotalProjectAmount = entity.ContractAmount + entity.Variation;
                }
                else if (entity.Supplemetary != null && entity.Variation == null)
                {
                    entity.TotalProjectAmount = entity.ContractAmount + entity.Supplemetary;
                }
                else
                {
                    entity.TotalProjectAmount = entity.ContractAmount + entity.Variation + entity.Supplemetary;
                }
                entity.NetGrand = entity.TotalProjectAmount - entity.ContractAmount * (entity.Rebate / 100);


                db.SubContrat.Add(entity);
                db.SaveChanges();
                projectSubContract.ProjectContractId = entity.ProjectContractId;
            }

            return Json(new[] { projectSubContract }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContrat_Update([DataSourceRequest]DataSourceRequest request, ProjectSubContract projectSubContract)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectSubContract
                {
                    ProjectContractId = projectSubContract.ProjectContractId,
                    WorkType = projectSubContract.WorkType,
                    SubContractor = projectSubContract.SubContractor,

                    ContractSigningDate = projectSubContract.ContractSigningDate,
                    CommencementDate = projectSubContract.CommencementDate,
                    // project amount without any rebate, variation and supplemetary
                    ContractAmount = projectSubContract.ContractAmount,
                    // completion date without any time extensions
                    ContractTime = projectSubContract.ContractTime,
                    ProvisionalSum = projectSubContract.ProvisionalSum,

                    SiteHandoverTime = projectSubContract.SiteHandoverTime,
                    ContractPeriod = projectSubContract.ContractPeriod,
                    ApprovedTimeExtension = projectSubContract.ApprovedTimeExtension,
                    TotalProjectPeriod = projectSubContract.TotalProjectPeriod,
                    RevisedCompletionDate = projectSubContract.RevisedCompletionDate,
                    TimeElapsedToDate = projectSubContract.TimeElapsedToDate,
                    Rebate = projectSubContract.Rebate,
                    Variation = projectSubContract.Variation,
                    Supplemetary = projectSubContract.Supplemetary,
                    TotalProjectAmount = projectSubContract.TotalProjectAmount,
                    NetGrand = projectSubContract.NetGrand,
                    ExecutedToDate = projectSubContract.ExecutedToDate,

                    ProjectInformationId = projectSubContract.ProjectInformationId,
                    OrgBranchName = projectSubContract.OrgBranchName,
                    DateCreated = projectSubContract.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectSubContract.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };
                entity.TotalProjectPeriod = entity.ApprovedTimeExtension == null ? entity.ContractPeriod : entity.ContractPeriod + entity.ApprovedTimeExtension;
                entity.ContractTime = entity.ContractSigningDate.AddDays((double)entity.ContractPeriod);
                entity.RevisedCompletionDate = entity.ContractSigningDate.AddDays(((double)entity.TotalProjectPeriod));
                if (entity.Variation == null && entity.Supplemetary == null)
                {
                    entity.TotalProjectAmount = entity.ContractAmount;
                }
                else if (entity.Variation != null && entity.Supplemetary == null)
                {
                    entity.TotalProjectAmount = entity.ContractAmount + entity.Variation;
                }
                else if (entity.Supplemetary != null && entity.Variation == null)
                {
                    entity.TotalProjectAmount = entity.ContractAmount + entity.Supplemetary;
                }
                else
                {
                    entity.TotalProjectAmount = entity.ContractAmount + entity.Variation + entity.Supplemetary;
                }
                entity.NetGrand = entity.TotalProjectAmount - entity.ContractAmount * (entity.Rebate / 100);

                db.SubContrat.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ContractMgmtHelper.UpdateContract(entity.ProjectContractId);

            }

            return Json(new[] { projectSubContract }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContrat_Destroy([DataSourceRequest]DataSourceRequest request, ProjectSubContract projectSubContract)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectSubContract
                {
                    ProjectContractId = projectSubContract.ProjectContractId,
                    WorkType = projectSubContract.WorkType,
                    SubContractor = projectSubContract.SubContractor,

                    ContractSigningDate = projectSubContract.ContractSigningDate,
                    CommencementDate = projectSubContract.CommencementDate,
                    // project amount without any rebate, variation and supplemetary
                    ContractAmount = projectSubContract.ContractAmount,
                    // completion date without any time extensions
                    ContractTime = projectSubContract.ContractTime,
                    ProvisionalSum = projectSubContract.ProvisionalSum,

                    SiteHandoverTime = projectSubContract.SiteHandoverTime,
                    ContractPeriod = projectSubContract.ContractPeriod,
                    ApprovedTimeExtension = projectSubContract.ApprovedTimeExtension,
                    TotalProjectPeriod = projectSubContract.TotalProjectPeriod,
                    RevisedCompletionDate = projectSubContract.RevisedCompletionDate,
                    TimeElapsedToDate = projectSubContract.TimeElapsedToDate,
                    Rebate = projectSubContract.Rebate,
                    Variation = projectSubContract.Variation,
                    Supplemetary = projectSubContract.Supplemetary,
                    TotalProjectAmount = projectSubContract.TotalProjectAmount,
                    NetGrand = projectSubContract.NetGrand,
                    ExecutedToDate = projectSubContract.ExecutedToDate,

                    ProjectInformationId = projectSubContract.ProjectInformationId,
                    OrgBranchName = projectSubContract.OrgBranchName,
                    DateCreated = projectSubContract.DateCreated,
                    LastModified = projectSubContract.LastModified,
                    CreatedBy = projectSubContract.CreatedBy,
                    ModifiedBy = projectSubContract.ModifiedBy
                };

                db.SubContrat.Attach(entity);
                db.SubContrat.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectSubContract }.ToDataSourceResult(request, ModelState));
        }
        private void UpdateSiteHandOverTime()
        {
            var contracts = db.ProjectContract.ToList();
            if (contracts.Any())
            {
                foreach (ProjectContract contract in contracts)
                {
                    if (contract.SiteHandoverTime < contract.ContractSigningDate)
                    {
                        contract.SiteHandoverTime = contract.ContractSigningDate;

                        db.ProjectContract.Attach(contract);
                        db.Entry(contract).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        public void Download(int id)
        {
            DataProviderController.SelectedFile = id;

        }
        public ActionResult DownloadDoument(string filename)
        {

            if (DataProviderController.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.PROJECT_DOCUMENT && x.Remark == DataProviderController.Owner && x.AttachmentId == DataProviderController.SelectedFile)
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
                        "attachment;filename=\"" + attachment.FileName + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + DataProviderController.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    DataProviderController.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
            DataProviderController.SelectedFile = 0;
            return null;
        }
        public ActionResult UploadDocument(HttpPostedFileBase file, string url, string txtValue, string txtDecsion, string id)
        {
            try
            {

                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + DataProviderController.EmployeeId.ToString() + "-" +
                                           AttachmentType.PROJECT_DOCUMENT), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + DataProviderController.EmployeeId.ToString() + "-" +
                                           AttachmentType.PROJECT_DOCUMENT));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {


                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = DataProviderController.EmployeeId.ToString(),
                            FileName = file.FileName,
                            Description = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.PROJECT_DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = DataProviderController.EmployeeId.ToString()
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "This file exists";

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
                    Remark = attachment.Remark
                };

                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        private void UpdateTimeElapsedToDate()
        {
            var subcontracts = db.SubContrat.ToList();
            //var allContracts = db.ProjectContract;
            //var ids = new HashSet<int>(sub.Select(s => s.ProjectContractId));
            //var mainContrats = allContracts.Where(main => !ids.Contains(main.ProjectContractId)).ToList();

            if (subcontracts.Any())
            {
                foreach (ProjectContract contract in subcontracts)
                {
                    var difference = DateTime.Now - contract.ContractSigningDate;
                    var days = (decimal)difference.Days + 1;
                    if (days > contract.TotalProjectPeriod)
                    {
                        contract.TimeElapsedToDate = contract.TotalProjectPeriod;
                    }
                    else if (days < 0)
                    {
                        contract.TimeElapsedToDate = 0;
                    }
                    else
                    {
                        contract.TimeElapsedToDate = days;
                    }

                    db.ProjectContract.Attach(contract);
                    db.Entry(contract).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
