using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetRegistration
{
    public class FleetBoloRegistrationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/FleetBoloRegistration
        public ActionResult Index()
        {
            ViewData["Category"] = db.EquipmentIdentity.Select(s => new
            {
                Value = s.EquipmentIdentityID,
                Text = s.EquipmentCategoryName
            }).ToList();
            ViewData["Type"] = db.EquipmentType.Select(s => new
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();

            ViewData["Name"] = db.EquipmentName.Select(s => new
            {
                Value = s.EquipmentNameID,
                Text = s.Name
            }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Text = s.PlateNumber,
                Value = s.FleetsID
            }).ToList();
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FleetBoloRegistration> bolo = db.FleetBoloRegistrations;
            DataSourceResult result = bolo.ToDataSourceResult(request, model => new FleetBoloRegistration
            {
                FleetBoloRegistrationId = model.FleetBoloRegistrationId,
                FleetId = model.FleetId,
                BoloExpireDate = model.BoloExpireDate,
                BoloRegistrationDate = model.BoloRegistrationDate,
                Remark = model.Remark,
                DateCreated = model.DateCreated,
                LastModified = model.LastModified,
                CreatedBy = model.CreatedBy,
                ModifiedBy = model.ModifiedBy,
                EquipmentCategoryId = model.EquipmentCategoryId,
                EquipmentNameId = model.EquipmentNameId,
                EquipmentTypeId = model.EquipmentTypeId
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Create([DataSourceRequest]DataSourceRequest request, FleetBoloRegistration model)
        //{
        //    var bolo = db.FleetBoloRegistrations.Where(x => x.FleetId == model.FleetId).ToList();

        //    if (bolo.Any())
        //    {
        //        ModelState.AddModelError("", "This vehicle already exists");
        //    }
        //    if(model.BoloRegistrationDate == model.BoloExpireDate)
        //    {
        //        ModelState.AddModelError("", "Registration Date and Expiration Date can't be equal");
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        model.CreatedBy = User.Identity.Name;
        //        model.DateCreated = DateTime.Today;

        //        db.FleetBoloRegistrations.Add(model);
        //        db.SaveChanges();

        //    }

        //    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        //}
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, FleetBoloRegistration model)
        {
            var bolo = db.FleetBoloRegistrations.Where(x => x.FleetId == model.FleetId).ToList();

            if (bolo.Any())
            {
                ModelState.AddModelError("exists", "This vehicle already exists");
            }
            if (model.BoloRegistrationDate == model.BoloExpireDate)
            {
                ModelState.AddModelError("", "Registration Date and Expiration Date can't be equal");
            }
            ModelState.Remove("EquipmentCategoryId");
            ModelState.Remove("EquipmentTypeID");
            ModelState.Remove("EquipmentNameID");
            var fleetBoloRegistrationUpdate = db.FleetBoloRegistrations.Where(fbr => fbr.EquipmentCategoryId == null).ToList();
            foreach (var FleetBoloRegistrationUpdate in fleetBoloRegistrationUpdate)
            {
                var fetchfromfleetRegistration = db.Fleets.SingleOrDefault(pl => pl.FleetsID == FleetBoloRegistrationUpdate.FleetId);
                if (fetchfromfleetRegistration != null)
                {
                    FleetBoloRegistrationUpdate.EquipmentCategoryId = fetchfromfleetRegistration.EquipmentIdentityID;
                    FleetBoloRegistrationUpdate.EquipmentTypeId = fetchfromfleetRegistration.EquipmentTypeID;
                    FleetBoloRegistrationUpdate.EquipmentNameId = fetchfromfleetRegistration.EquipmentNameID;
                    db.Entry(FleetBoloRegistrationUpdate).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            var FetcheFromEquipmentIdentity = db.Fleets.SingleOrDefault(pl => pl.FleetsID == model.FleetId);
            if (FetcheFromEquipmentIdentity == null)
               {
                
                    ModelState.AddModelError("no entry inFleet", "Plate Should be registered on fleet first");
                
            }

                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Today;
                model.EquipmentCategoryId = FetcheFromEquipmentIdentity.EquipmentIdentityID;
                model.EquipmentTypeId = FetcheFromEquipmentIdentity.EquipmentTypeID;
                model.EquipmentNameId = FetcheFromEquipmentIdentity.EquipmentNameID;

                if (ModelState.IsValid)
            {
               
                    db.FleetBoloRegistrations.Add(model);
                    db.SaveChanges();
                    
            }
                
            

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, FleetBoloRegistration model)
        {
            var bolo = db.FleetBoloRegistrations.Where(x => x.FleetId == model.FleetId).ToList();
            var previousBolo = db.FleetBoloRegistrations.SingleOrDefault(pb => pb.FleetBoloRegistrationId == model.FleetBoloRegistrationId);
            //if (bolo.Any())
            //{
            //    ModelState.AddModelError("", "This vehicle already exists");
            //}
            if (model.BoloRegistrationDate == model.BoloExpireDate)
            {
                ModelState.AddModelError("", "Registration Date and Expiration Date can't be equal");
            }
            ModelState.Remove("EquipmentCategoryId");
            ModelState.Remove("EquipmentTypeID");
            ModelState.Remove("EquipmentNameID");
            if (ModelState.IsValid)
            {
                var FetcheFromEquipmentIdentity = db.Fleets.SingleOrDefault(pl => pl.FleetsID == model.FleetId);
                model.EquipmentCategoryId = FetcheFromEquipmentIdentity.EquipmentIdentityID;
                model.EquipmentTypeId = FetcheFromEquipmentIdentity.EquipmentTypeID;
                model.EquipmentNameId = FetcheFromEquipmentIdentity.EquipmentNameID;
                if (FetcheFromEquipmentIdentity != null)
                {
                    if (model.BoloExpireDate >= DateTime.Now)
                    {
                        var fleetBoloRegistrationHistory = new FleetBoloRegistrationHistory();
                        fleetBoloRegistrationHistory.BoloExpireDate = previousBolo.BoloExpireDate;
                        fleetBoloRegistrationHistory.BoloRegistrationDate = previousBolo.BoloRegistrationDate;
                        fleetBoloRegistrationHistory.CreatedBy = previousBolo.CreatedBy;
                        fleetBoloRegistrationHistory.ModifiedBy = previousBolo.ModifiedBy;
                        fleetBoloRegistrationHistory.FleetBoloRegistrationHistoryId = fleetBoloRegistrationHistory.FleetBoloRegistrationHistoryId;
                        fleetBoloRegistrationHistory.EquipmentCategoryId = previousBolo.EquipmentCategoryId;
                        fleetBoloRegistrationHistory.EquipmentNameId = previousBolo.EquipmentNameId;
                        fleetBoloRegistrationHistory.EquipmentTypeId = previousBolo.EquipmentTypeId;
                        fleetBoloRegistrationHistory.FleetBoloRegistrationId = previousBolo.FleetBoloRegistrationId;
                        fleetBoloRegistrationHistory.FleetId = previousBolo.FleetId;
                        fleetBoloRegistrationHistory.Remark = previousBolo.Remark;
                        db.FleetBoloRegistrationsHistorys.Add(fleetBoloRegistrationHistory);
                        db.SaveChanges();
                    }
                    previousBolo.BoloExpireDate = model.BoloExpireDate;
                    previousBolo.BoloRegistrationDate = model.BoloRegistrationDate;
                    previousBolo.EquipmentCategoryId = model.EquipmentCategoryId;
                    previousBolo.EquipmentNameId = model.EquipmentNameId;
                    previousBolo.EquipmentTypeId = model.EquipmentTypeId;
                    previousBolo.FleetBoloRegistrationId = model.FleetBoloRegistrationId;
                    previousBolo.FleetId = model.FleetId;
                    previousBolo.Remark = model.Remark;
                    previousBolo.ModifiedBy = User.Identity.Name;
                    previousBolo.LastModified = DateTime.Today;
                    db.Entry(previousBolo).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, FleetBoloRegistration model)
        {
            if (ModelState.IsValid)
            {

                var bolo = db.FleetBoloRegistrations.Find(model.FleetBoloRegistrationId);
                if (model.BoloExpireDate >= DateTime.Now)
                {
                    var fleetBoloRegistrationHistory = new FleetBoloRegistrationHistory();
                    fleetBoloRegistrationHistory.BoloExpireDate = bolo.BoloExpireDate;
                    fleetBoloRegistrationHistory.BoloRegistrationDate = bolo.BoloRegistrationDate;
                    fleetBoloRegistrationHistory.CreatedBy = bolo.CreatedBy;
                    fleetBoloRegistrationHistory.ModifiedBy = bolo.ModifiedBy;
                    fleetBoloRegistrationHistory.FleetBoloRegistrationHistoryId = fleetBoloRegistrationHistory.FleetBoloRegistrationHistoryId;
                    fleetBoloRegistrationHistory.EquipmentCategoryId = bolo.EquipmentCategoryId;
                    fleetBoloRegistrationHistory.EquipmentNameId = bolo.EquipmentNameId;
                    fleetBoloRegistrationHistory.EquipmentTypeId = bolo.EquipmentTypeId;
                    fleetBoloRegistrationHistory.FleetBoloRegistrationId = bolo.FleetBoloRegistrationId;
                    fleetBoloRegistrationHistory.FleetId = bolo.FleetId;
                    db.FleetBoloRegistrationsHistorys.Add(fleetBoloRegistrationHistory);
                    db.SaveChanges();
                    db.FleetBoloRegistrations.Remove(bolo);
                    db.SaveChanges();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult BoloRegistrationDoc_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var boloDoc = db.BoloRegistrationDocuments.Where(x => x.FleetBoloRegistrationId == id);
            return Json(boloDoc.ToDataSourceResult(request));
        }
        public ActionResult BoloRegistrationDoc_Delete([DataSourceRequest]DataSourceRequest request, BoloRegistrationDocument model)
        {
            var boloDoc = db.BoloRegistrationDocuments.FirstOrDefault(x => x.FleetBoloRegistrationId == model.FleetBoloRegistrationId);
            db.BoloRegistrationDocuments.Remove(boloDoc);
            db.SaveChanges();
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BoloRegistrationAttachment_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 2 * 1024 * 1024;

                string subPath = "~/App_Data/Bolo/";
                string shortPath = "/App_Data/Bolo/";

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
                            db.BoloRegistrationDocuments.Add(new BoloRegistrationDocument()
                            {
                                FileName = Path.GetFileName(file.FileName),
                                FleetBoloRegistrationId = id,
                                ShortPath = shortPath,
                                FilePath = physicalPath

                            });
                            db.SaveChanges();
                        }
                    }
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "PO attachment", "Success", User.Identity.Name, $"created attachement under po {id} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { attachments }.ToDataSourceResult(request, ModelState));
        }
        //public JsonResult UploadCompanyFile(int FleetBoloRegistrationId, HttpPostedFileBase[] CompanyFile, string Description)
        //{
        //    for (int i = 0; i < CompanyFile.Count(); i++)
        //    {
        //        HttpPostedFileBase compFile = CompanyFile[i];
        //        if (compFile != null && compFile.ContentLength > 0)
        //        {
        //            string path = HttpContext.Server.MapPath("~/Content/Files/Archive/");
        //            string shortPath = "/Content/Files/Archive/";
        //            if (!Directory.Exists(path))
        //            {
        //                Directory.CreateDirectory(path);
        //            }
        //            string newFileName = Path.GetFileNameWithoutExtension(compFile.FileName);// compFile.FileName;
        //            var fileExtension = System.IO.Path.GetExtension(compFile.FileName);
        //            string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
        //            newFileName = newFileName + name + fileExtension;
        //            string filePath = path + newFileName;

        //            compFile.SaveAs(filePath);

        //            var entity = new BoloRegistrationDocument
        //            {
        //                FleetBoloRegistrationId = FleetBoloRegistrationId,
        //                FilePath = filePath,
        //                FileName = newFileName,
        //                Description = Description,
        //                ShortPath = shortPath,
        //                FileExtension = fileExtension
        //            };

        //            db.BoloRegistrationDocuments.Add(entity);
        //            db.SaveChanges();
        //        }
        //    }

        //    return Json("Successfull");
        //}
        public ActionResult DownloadBoloDocument(int CompanyDocumentId)
        {
            var record = db.BoloRegistrationDocuments.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
    }
}