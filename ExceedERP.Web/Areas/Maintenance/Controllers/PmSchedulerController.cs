using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class PmSchedulerController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["Services"] = db.RepairType.Select(s => new
            {
                Code = s.RepairTypeID,
                Name = s.Code + "-" + s.Name
            }).ToList();
            ViewData["PmSetting"] = db.PmSettings.Select(s => new
            {
                Code = s.PmSettingId,
                Name = s.PmName
            }).ToList();
            ViewData["PlatesList"] = db.Fleets.Select(s => new
            {
                Code = s.FleetsID,
                Name = s.PlateNumber+" "+s.SerialNo+" "+s.ChasisNumber
            }).ToList();

            return View();
        }
        public ActionResult PmScheduler_Read([DataSourceRequest]DataSourceRequest request)
        {
           
                IQueryable <PmScheduler> pmScheduler = db.PmSchedulers;
            DataSourceResult Result = pmScheduler.ToDataSourceResult(request, x => new {
                Location = x.Location,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DateFrequency = x.DateFrequency,
                KMFrequency = x.KMFrequency,
                DelayReason = x.DelayReason,
                EquipmentNameId = x.EquipmentNameId,
                IsExpired = x.IsExpired,
                LastModified = x.LastModified,
                LastPmDate = x.LastPmDate,
                LastPmKM = x.LastPmKM,
                ModifiedBy = x.ModifiedBy,
                NextPmDate = x.NextPmDate,
                NextPmKM = x.NextPmKM,
                FleetId = x.FleetId,
                PmSchedulerId = x.PmSchedulerId,
                PmSettingId = x.PmSettingId,
                TaskId = x.TaskId,
                WorkDelay = x.WorkDelay,
            }); // Force execution before passing to Kendo

            return Json(Result,JsonRequestBehavior.AllowGet);
        }

        public ActionResult PmScheduler_Create([DataSourceRequest]DataSourceRequest request, PmScheduler pmScheduler)
        {
            ModelState.Remove("EquipmentNameId");
            ModelState.Remove("DateFrequency");
            var FetchNameId = db.Fleets.FirstOrDefault(x => x.FleetsID.ToString() == pmScheduler.FleetId);
            if (FetchNameId == null)
                ModelState.AddModelError("Not Found", "No Fleet Regisered With This PlateNumber");
            pmScheduler.EquipmentNameId = db.EquipmentName.FirstOrDefault(x=>x.EquipmentNameID==FetchNameId.EquipmentNameID).Name;
            var scheduler = db.PmSchedulers.Where(x => x.FleetId == pmScheduler.FleetId&&x.TaskId==pmScheduler.TaskId).ToList();
            var FetchPreviousSchedule = new PmScheduler();
            foreach(var lastObject in scheduler)
            {
                FetchPreviousSchedule = lastObject;
            }
            DateTime xyz = new DateTime(1900, 1, 1);
            if (FetchPreviousSchedule.NextPmDate>DateTime.Now)
            {
                ModelState.AddModelError("Next Schedule is not expired","The Previously scheduled PM Date is Future");
            }
         
            if (FetchPreviousSchedule!=null&&FetchPreviousSchedule.LastPmDate>xyz)
            {
                pmScheduler.LastPmDate = FetchPreviousSchedule.LastPmDate;
                pmScheduler.LastPmKM = FetchPreviousSchedule.LastPmKM;
            }
            //pmScheduler.DateFrequency = (pmScheduler.NextPmDate - pmScheduler.LastPmDate).Days;
            var FetchBranch = db.Branch.FirstOrDefault(x => x.BranchId == FetchNameId.BranchId);
            if(FetchBranch!=null)
            {
                pmScheduler.Location = FetchBranch.Name;
            }
           
           
            if (ModelState.IsValid)
            {
                var entity = (PmScheduler)this.GetMappedScheduler(pmScheduler);
                db.PmSchedulers.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { pmScheduler }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GenerateSchedule()
        {
            var generatedSchedules = db.PmSchedulers
                .GroupBy(x => new
                {
                    x.FleetId,
                    x.PmSettingId,
                    x.TaskId
                })
                .Select(g => g
                    .OrderByDescending(x => x.PmSchedulerId)
                    .FirstOrDefault())
                .ToList();
            foreach(var x in generatedSchedules)
            {
                if (x.NextPmDate >= DateTime.Now)
                    continue;
                var entity = new PmScheduler
                {

                    CreatedBy =User.Identity.Name,
                    DateCreated =DateTime.Now,
                    DateFrequency = x.DateFrequency,
                    KMFrequency = x.KMFrequency,
                    DelayReason = x.DelayReason,
                    Location = x.Location,
                    EquipmentNameId = x.EquipmentNameId,
                    IsExpired = false,
                    LastPmKM = x.LastPmKM,
                    ModifiedBy = x.ModifiedBy,
                    NextPmDate = DateTime.Now.AddYears(1),
                    LastPmDate=x.LastPmDate.AddYears(1),
                    NextPmKM = x.NextPmKM,
                    FleetId = x.FleetId,                    
                    PmSettingId = x.PmSettingId,
                    TaskId = x.TaskId,
                    WorkDelay = x.WorkDelay,
                };
                db.PmSchedulers.Add(entity);
                
            }
            db.SaveChanges();
            var formattedData = new
            {
                message = "Schedule generated schedule",
                Result="ok"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        private object GetMappedScheduler(PmScheduler x)
        {
            var entity = new PmScheduler
            {
                
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                DateFrequency = x.DateFrequency,
                KMFrequency = x.KMFrequency,
                DelayReason = x.DelayReason,
                Location = x.Location,
                EquipmentNameId = x.EquipmentNameId,
                IsExpired = x.IsExpired,
                LastModified = x.LastModified,
                LastPmDate = x.LastPmDate,
                LastPmKM = x.LastPmKM,
                ModifiedBy = x.ModifiedBy,
                NextPmDate = x.NextPmDate,
                NextPmKM = x.NextPmKM,
                FleetId = x.FleetId,
                PmSchedulerId = x.PmSchedulerId,
                PmSettingId = x.PmSettingId,
                TaskId = x.TaskId,
                WorkDelay = x.WorkDelay,
            };
            return entity;
        }
        public ActionResult PmScheduler_Destroy([DataSourceRequest]DataSourceRequest request, PmScheduler pmScheduler)
        {
            var FetchSchedule = db.PmSchedulers.SingleOrDefault(x => x.PmSchedulerId == pmScheduler.PmSchedulerId);
            if(FetchSchedule==null)
            {
                ModelState.AddModelError("No Record Found", "No Record Found");
            }
            if(ModelState.IsValid)
            {
                db.PmSchedulers.Remove(FetchSchedule);
                db.SaveChanges();
            }
            return Json(new[] { pmScheduler }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PmScheduler_Update([DataSourceRequest]DataSourceRequest request, PmScheduler pmScheduler)
        {
            var FetchSchedule = db.PmSchedulers.SingleOrDefault(x => x.PmSchedulerId == pmScheduler.PmSchedulerId);
            if (FetchSchedule == null)
            {
                ModelState.AddModelError("No Record Found", "No Record Found");
            }
            var scheduler = db.PmSchedulers.Where(x => x.FleetId == pmScheduler.FleetId && x.TaskId == pmScheduler.TaskId).ToList();
            var FetchPreviousSchedule = new PmScheduler();
            foreach (var lastObject in scheduler)
            {
                FetchPreviousSchedule = lastObject;
            }
            
            //if (FetchPreviousSchedule != null)
            //{
            //    pmScheduler.LastPmDate = FetchSchedule.LastPmDate;
            //    pmScheduler.LastPmKM = FetchSchedule.LastPmKM;
            //}
            var FetchNameId = db.Fleets.FirstOrDefault(x => x.FleetsID.ToString() == pmScheduler.FleetId);
            if (FetchNameId == null)
                ModelState.AddModelError("Not Found", "No Fleet Regisered With This PlateNumber");
            pmScheduler.EquipmentNameId = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == FetchNameId.EquipmentNameID).Name;
           // pmScheduler.DateFrequency = (pmScheduler.NextPmDate - pmScheduler.LastPmDate).Days;
             pmScheduler.Location =FetchNameId.Location;
           
            if (ModelState.IsValid)
            {
                FetchSchedule.KMFrequency = pmScheduler.KMFrequency;
                FetchSchedule.LastModified =DateTime.Now;
                FetchSchedule.LastPmDate = pmScheduler.LastPmDate;
                FetchSchedule.LastPmKM = pmScheduler.LastPmKM;
                FetchSchedule.ModifiedBy = User.Identity.Name;
                FetchSchedule.NextPmDate = pmScheduler.NextPmDate;
                FetchSchedule.NextPmKM = pmScheduler.NextPmKM;
                FetchSchedule.FleetId = pmScheduler.FleetId;
               // FetchSchedule.PmSchedulerId = pmScheduler.KMFrequency;
                FetchSchedule.PmSettingId = pmScheduler.PmSettingId;
                FetchSchedule.TaskId = pmScheduler.TaskId;
                FetchSchedule.WorkDelay = pmScheduler.WorkDelay;
                FetchSchedule.DateFrequency = pmScheduler.DateFrequency;
                FetchSchedule.DelayReason = pmScheduler.DelayReason;
                FetchSchedule.EquipmentNameId = pmScheduler.EquipmentNameId;
                
                //db.PmSchedulers.Attach(entity);
                db.Entry(FetchSchedule).State = EntityState.Modified;
                db.SaveChanges(); 
            }
            return Json(new[] { pmScheduler }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult SOPDoc_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var boloDoc = db.SOPDocuments.Where(x => x.PmSchedulerId == id);
            return Json(boloDoc.ToDataSourceResult(request));
        }
        public ActionResult SOPDoc_Delete([DataSourceRequest]DataSourceRequest request, SOPDocument model)
        {
            var boloDoc = db.SOPDocuments.FirstOrDefault(x => x.PmSchedulerId == model.PmSchedulerId);
            db.SOPDocuments.Remove(boloDoc);
            db.SaveChanges();
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SOPAttachment_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 2 * 1024 * 1024;

                string subPath = "~/App_Data/SOP/";
                string shortPath = "/App_Data/SOP/";

                bool exists = Directory.Exists(Server.MapPath(subPath));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
                foreach (var file in attachments)
                {
                    if (file != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(file.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                        }
                        if (file.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", $"File size exceeds the {MaxFileSize / (1024 * 1024)} MB limit.");
                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            string fileName = Path.GetFileName(file.FileName);
                            string physicalPath = Server.MapPath(subPath + fileName);
                            //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                            file.SaveAs(physicalPath);
                            db.SOPDocuments.Add(new SOPDocument()
                            {
                                FileName = Path.GetFileName(file.FileName),
                                PmSchedulerId = id,
                                ShortPath = shortPath,
                                FilePath = physicalPath

                            });
                            db.SaveChanges();
                        }
                    }
                }
                #region operation log
              //  UserHelper.OperationLog(Request.UserHostAddress, "PO attachment", "Success", User.Identity.Name, $"created attachement under po {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadSOPDocument(int SOPDocumentIdentification)
        {
            var record = db.SOPDocuments.Find(SOPDocumentIdentification);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        public JsonResult PmSchedulerExpired(int id)
        {
            var FetchSchedule = db.PmSchedulers.FirstOrDefault(x => x.PmSchedulerId == id&&x.IsExpired==false);
            if(FetchSchedule!=null)
            {
                FetchSchedule.IsExpired = true;

            }
            var SuccessMessage = new
            {
                Message = "Updated",
                Status = "OK"
            };
            return Json(SuccessMessage, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}