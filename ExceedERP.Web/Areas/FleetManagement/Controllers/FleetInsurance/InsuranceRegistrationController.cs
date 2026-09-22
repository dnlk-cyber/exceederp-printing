using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Global;
using System.IO;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    //[AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class InsuranceRegistrationController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
           
            ViewData["Category"] = CacheData.GetEquipmentIdentityCaches().Select(
      i => new
      {
          Text = i.Name,
          Value = i.Id
      }).AsQueryable();
            ViewData["Type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();

            var setting = db.RequestToInspections.FirstOrDefault();
            ViewBag.setting = setting != null ? setting.Remark : " ";
            ViewData["Name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["InsuranceCover"] = db.InsuranceCoverTypes.Select(p => new
            {
                Id = p.InsuranceCoverTypeId,
                Name = p.Code + "-" + p.Name + " " + p.Description
            }

                ).AsQueryable();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            });
            ViewData["fleet"] = CacheData.GetFleetsSecondCache().Select(x => new { Id = x.FleetsID, Name = x.PlateNumber }).ToList();
            ViewData["InsuranceCompany"] = db.InsuranceCompany.Select(x => new { x.InsuranceCompanyId, x.InsuranceCompanyName });
            return View();
        }

        public JsonResult Seed() 
        {
            var fleetInsurances = db.FleetInsurance.ToList();
            foreach (var item in fleetInsurances)
            {
                var fleet = db.Fleets.Find(item.FleetsID);
                if(fleet != null)
                {
                    item.EquipmentCategoryId = fleet?.EquipmentIdentityID ?? Guid.Empty;
                    item.EquipmentTypeId = fleet?.EquipmentTypeID ?? Guid.Empty;
                    item.EquipmentNameId = fleet?.EquipmentNameID ?? Guid.Empty;
                    db.Entry(item).State = EntityState.Modified;
                }
                
            }
            db.SaveChanges();
            return Json("Succcessfully Assigned Data", JsonRequestBehavior.AllowGet);
        }
        private bool IsExpired(DateTime? expireDate)
        {
            DateTime currentDate = DateTime.Now;
            TimeSpan timeDifference = (DateTime)expireDate - currentDate;
            int daysRemaining = timeDifference.Days;
            if(daysRemaining <= 15)
            {
                return true;
            }
            return false;
        }
        private StatusEnum IsExpire(DateTime? expireDate)
        {
            DateTime currentDate = DateTime.Now;
            TimeSpan timeDifference = (DateTime)expireDate - currentDate;
            int daysRemaining = timeDifference.Days;
            if(daysRemaining <= 15)
            {
               return StatusEnum.Expired;
            }
            return StatusEnum.Active;
        }


        //public ActionResult FleetInsurances_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    var fleetInsurances = db.FleetInsurance;

        //    DataSourceResult result = fleetInsurances.ToDataSourceResult(request, fleetsInsurance => new FleetInsurance
        //    {
        //        FleetInsuranceId = fleetsInsurance.FleetInsuranceId,
        //        FleetsID = fleetsInsurance.FleetsID,
        //        InsuranceCompanyId = fleetsInsurance.InsuranceCompanyId,
        //        PremiumCost = fleetsInsurance.PremiumCost,
        //        DueDate = fleetsInsurance.DueDate,
        //        ExpireDate = fleetsInsurance.ExpireDate,
        //        AddToHistory=fleetsInsurance.AddToHistory,
        //        HasExpired = IsExpired(fleetsInsurance.DueDate),
        //        Attachment = fleetsInsurance.Attachment,
        //        Description = fleetsInsurance.Description,
        //        PolicyNo = fleetsInsurance.PolicyNo,
        //        ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance,
        //        ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance,
        //        InsuranceType = fleetsInsurance.InsuranceType,
        //        CreatedBy = fleetsInsurance.CreatedBy,
        //        DateCreated = fleetsInsurance.DateCreated,
        //        EquipmentCategoryId = fleetsInsurance.EquipmentCategoryId,
        //        EquipmentTypeId = fleetsInsurance.EquipmentTypeId,
        //        EquipmentNameId = fleetsInsurance.EquipmentNameId

        //    });

        //    return Json(result);
        //}

        public ActionResult FleetInsurances_Read([DataSourceRequest] DataSourceRequest request)
        {
            var fleetInsurancesQuery = from FI in db.FleetInsurance
                                       join F in db.Fleets on FI.FleetsID equals F.FleetsID
                                       select new
                                       {
                                           FI.FleetInsuranceId,
                                           FI.FleetsID,
                                           FI.InsuranceCompanyId,
                                           FI.PremiumCost,
                                           FI.DueDate,
                                           FI.InsuranceCoverTypeId,
                                           FI.ExpireDate,
                                           FI.Attachment,
                                           FI.Payment,
                                           FI.Description,
                                           FI.PolicyNo,
                                           FI.ThirdPartyInsurance,
                                           FI.ComprehensiveInsurance,
                                           FI.InsuranceType,
                                           FI.CreatedBy,
                                           FI.DateCreated,
                                           FI.EquipmentCategoryId,
                                           FI.EquipmentTypeId,
                                           FI.EquipmentNameId,
                                           FI.IsOnlineApproved,
                                           FI.IsSendForApproval,
                                           FI.IsPaymentRequestGenerated,
                                           PlateNumber = F.PlateNumber
                                       };

            // Execute query in memory
            var fleetInsurances = fleetInsurancesQuery
                                  .AsEnumerable()   // EF executes SQL here
                                  .Select(f => new
                                  {
                                      f.FleetInsuranceId,
                                      f.FleetsID,
                                      f.InsuranceCompanyId,
                                      f.PremiumCost,
                                      f.DueDate,
                                      f.ExpireDate,
                                      f.InsuranceCoverTypeId,
                                      f.Attachment,
                                      f.Description,
                                      f.PolicyNo,
                                      f.Payment,
                                      f.ThirdPartyInsurance,
                                      f.ComprehensiveInsurance,
                                      f.InsuranceType,
                                      f.CreatedBy,
                                      f.DateCreated,
                                      f.EquipmentCategoryId,
                                      f.EquipmentTypeId,
                                      f.EquipmentNameId,
                                      f.PlateNumber,
                                      f.IsOnlineApproved,
                                      f.IsSendForApproval,
                                      f.IsPaymentRequestGenerated,
                                      HasExpired = IsExpired(f.ExpireDate),  // now safe
                                      StatusEnum=IsExpire(f.ExpireDate),
                              AddToHistory = false // if needed
                          });

            var result = fleetInsurances.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FleetInsurances_Create([DataSourceRequest] DataSourceRequest request, FleetInsurance fleetsInsurance)
        {
            if(fleetsInsurance.DueDate == null || fleetsInsurance.ExpireDate == null)
            {
                ModelState.AddModelError(string.Empty, "Due Date and Expire Date needs to have value");
            }
            //var exists = db.FleetInsurance.Where(x => x.FleetsID == fleetsInsurance.FleetsID).ToList();
            //if (exists.Any())
            //{
            //    ModelState.AddModelError(string.Empty, "This Item is already registred!");
            //}
            if (ModelState.IsValid)
            {
                var entity = new FleetInsurance
                {
                    
                    FleetsID= fleetsInsurance.FleetsID,
                    InsuranceCompanyId= fleetsInsurance.InsuranceCompanyId,
                    PremiumCost= fleetsInsurance.PremiumCost,
                    DueDate= fleetsInsurance.DueDate,
                    ExpireDate= fleetsInsurance.ExpireDate,
                    Payment=fleetsInsurance.Payment,
                    Attachment= fleetsInsurance.Attachment,
                    InsuranceCoverTypeId= fleetsInsurance.InsuranceCoverTypeId,
                    Description = fleetsInsurance.Description,
                    PolicyNo = fleetsInsurance.PolicyNo,
                    ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance,
                    ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance,
                    InsuranceType = fleetsInsurance.InsuranceType,
                    EquipmentCategoryId = fleetsInsurance.EquipmentCategoryId,
                    EquipmentTypeId = fleetsInsurance.EquipmentTypeId,
                    EquipmentNameId = fleetsInsurance.EquipmentNameId
                };


                db.FleetInsurance.Add(entity);
                db.SaveChanges();
                fleetsInsurance.FleetInsuranceId = entity.FleetInsuranceId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance registration", "Success", User.Identity.Name, $"created insurance registration {entity.FleetInsuranceId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { fleetsInsurance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetInsurances_Update([DataSourceRequest] DataSourceRequest request, FleetInsurance fleetsInsurance)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetInsurance
                {
                    FleetInsuranceId = fleetsInsurance.FleetInsuranceId,
                    FleetsID = fleetsInsurance.FleetsID,
                    InsuranceCompanyId = fleetsInsurance.InsuranceCompanyId,
                    PremiumCost = fleetsInsurance.PremiumCost,
                    DueDate = fleetsInsurance.DueDate,
                    ExpireDate = fleetsInsurance.ExpireDate,
                    Attachment = fleetsInsurance.Attachment,
                    Payment = fleetsInsurance.Payment,
                    Description = fleetsInsurance.Description,
                    InsuranceCoverTypeId= fleetsInsurance.InsuranceCoverTypeId,
                    PolicyNo = fleetsInsurance.PolicyNo,
                    ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance,
                    ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance,
                    InsuranceType = fleetsInsurance.InsuranceType,
                    EquipmentCategoryId = fleetsInsurance.EquipmentCategoryId,
                    EquipmentTypeId = fleetsInsurance.EquipmentTypeId,
                    EquipmentNameId = fleetsInsurance.EquipmentNameId
                };

                db.FleetInsurance.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                if(fleetsInsurance.AddToHistory==true)
                {
                    var fleetInsuranceHistory = new FleetInsuranceHistory();
                    fleetInsuranceHistory.FleetInsuranceHistoryId = fleetInsuranceHistory.FleetInsuranceHistoryId;
                    fleetInsuranceHistory.EquipmentCategoryId = fleetsInsurance.EquipmentCategoryId;
                    fleetInsuranceHistory.EquipmentCategoryId = fleetsInsurance.EquipmentNameId;
                    fleetInsuranceHistory.EquipmentTypeId = fleetsInsurance.EquipmentTypeId;
                    fleetInsuranceHistory.ExpireDate = fleetsInsurance.ExpireDate;
                    fleetInsuranceHistory.FleetInsuranceId = fleetsInsurance.FleetInsuranceId;
                    fleetInsuranceHistory.FleetsID = fleetsInsurance.FleetsID;
                    fleetInsuranceHistory.Payment = fleetsInsurance.Payment;
                    fleetInsuranceHistory.InsuranceCoverTypeId = fleetsInsurance.InsuranceCoverTypeId;
                    fleetInsuranceHistory.HazardOfExpiryDate = fleetsInsurance.HazardOfExpiryDate;
                    fleetInsuranceHistory.InsuranceCompanyId= fleetsInsurance.InsuranceCompanyId;
                    fleetInsuranceHistory.InsuranceType = fleetsInsurance.InsuranceType;
                    fleetInsuranceHistory.PolicyNo = fleetsInsurance.PolicyNo;
                    fleetInsuranceHistory.Remark = fleetsInsurance.Remark;
                    fleetInsuranceHistory.ThirdParty = fleetsInsurance.ThirdParty;
                    fleetInsuranceHistory.ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance;
                    fleetInsuranceHistory.ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance;
                    fleetInsuranceHistory.Description = fleetsInsurance.Description;
                    db.FleetInsuranceHistorys.Add(fleetInsuranceHistory);
                    db.SaveChanges();

                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance update", "Success", User.Identity.Name, $"updated insurance registration number {entity.FleetInsuranceId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { fleetsInsurance }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult FleetInsurances_Destroy([DataSourceRequest] DataSourceRequest request, FleetInsurance fleetsInsurance)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetInsurance
                {
                    FleetInsuranceId = fleetsInsurance.FleetInsuranceId,
                    FleetsID = fleetsInsurance.FleetsID,
                    InsuranceCompanyId = fleetsInsurance.InsuranceCompanyId,
                    PremiumCost = fleetsInsurance.PremiumCost,
                    DueDate = fleetsInsurance.DueDate,
                    ExpireDate = fleetsInsurance.ExpireDate,
                    Attachment = fleetsInsurance.Attachment,
                    Description = fleetsInsurance.Description,
                    InsuranceCoverTypeId = fleetsInsurance.InsuranceCoverTypeId,
                    ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance,
                    ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance,
                    InsuranceType = fleetsInsurance.InsuranceType,

                    PolicyNo = fleetsInsurance.PolicyNo,
                    EquipmentCategoryId = fleetsInsurance.EquipmentCategoryId,
                    EquipmentTypeId = fleetsInsurance.EquipmentTypeId,
                    EquipmentNameId = fleetsInsurance.EquipmentNameId
                };

                db.FleetInsurance.Attach(entity);
                db.FleetInsurance.Remove(entity);
                db.SaveChanges();
               
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted insurance registration number {entity.FleetInsuranceId }");
                #endregion
            }

            return Json(new[] { fleetsInsurance }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetInsuranceDoc_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var fleetInsurance = db.FleetInsuranceDocuments.Where(x => x.FleetInsuranceId == id);
            return Json(fleetInsurance.ToDataSourceResult(request));
        }
        public ActionResult FleetInsuranceDoc_Delete([DataSourceRequest]DataSourceRequest request, FleetInsuranceDocument model)
        {
            var fleetInsurance = db.FleetInsuranceDocuments.FirstOrDefault(x => x.FleetInsuranceId == model.FleetInsuranceDocumentId);
            db.FleetInsuranceDocuments.Remove(fleetInsurance);
            db.SaveChanges();
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadCompanyDocument(int CompanyDocumentId)
        {
            var record = db.FleetInsuranceDocuments.Find(CompanyDocumentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        
        #region Docs

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsuranceAttachment_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> attachments, int id)
        {
            if (attachments != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 2 * 1024 * 1024;

                string subPath = "~/App_Data/Insurance/" + "InsuranceReg" + id.ToString();

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
                            string physicalPath = Server.MapPath(subPath + "/" + fileName);
                            //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                            file.SaveAs(physicalPath);
                            db.FleetInsuranceDocuments.Add(new FleetInsuranceDocument()
                            {
                                FileName = Path.GetFileName(file.FileName),
                                FleetInsuranceId = id,
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
        public JsonResult UploadCompanyFile(int FleetInsuranceId, HttpPostedFileBase[] CompanyFile, string Description)
        {
            for (int i = 0; i < CompanyFile.Count(); i++)
            {
                HttpPostedFileBase compFile = CompanyFile[i];
                if (compFile != null && compFile.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Files/Archive/");
                    string shortPath = "/Content/Files/Archive/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string newFileName = Path.GetFileNameWithoutExtension(compFile.FileName);// compFile.FileName;
                    var fileExtension = System.IO.Path.GetExtension(compFile.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = newFileName + name + fileExtension;
                    string filePath = path + newFileName;

                    compFile.SaveAs(filePath);

                    var entity = new FleetInsuranceDocument
                    {
                        FleetInsuranceId = FleetInsuranceId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = Description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };

                    db.FleetInsuranceDocuments.Add(entity);
                    db.SaveChanges();
                }
            }

            return Json("Successfull");
        }
        #endregion
        public JsonResult GetCoverType()
        {

            return Json(db.InsuranceCoverTypes.Where(x => x.IsActive).Select(p => new
            {
                Id = p.InsuranceCoverTypeId,
                Name = p.Code + "-" + p.Name + " " + p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult SendForApproval(int id)
        {
            var record = db.FleetInsurance.FirstOrDefault(x => x.FleetInsuranceId == id);
            record.IsSendForApproval = true;
            record.SendForApprovalBy = User.Identity.Name;
            record.SendForApprovalTime = DateTime.Now;
            db.Entry(record).State = EntityState.Modified;
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Send For Approval Success",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Admin_Read([DataSourceRequest] DataSourceRequest request, int Id)
        {
            var Admin = new List<FleetInsuranceValidation>();
            var Request = db.FleetInsuranceValidation.Where(x => x.FleetInsuranceId == Id).FirstOrDefault();
            if (Request != null)
            {

                Admin =
                    db.FleetInsuranceValidation
                        .Where(x => x.FleetInsuranceId == Id)
                        .ToList();
            }
            return Json(Admin.ToDataSourceResult(request));
        }

        public ActionResult Admin_Create(int id, [DataSourceRequest]DataSourceRequest request, FleetInsuranceValidation Validation)
        {
            var record = db.FleetInsurance.FirstOrDefault(x => x.FleetInsuranceId == id);
            
            var hasApproved = db.FleetInsuranceValidation
                    .Where(x => x.FleetInsuranceId == id)
                .Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
            if (hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");
            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (Validation.Status == ApprovalStatuses.Approve)
                    {
                        record.IsOnlineApproved = true;
                        record.OnlineApprovedBy = User.Identity.Name;
                        record.OnlineApprovedTime= DateTime.Now;

                        //var subCompartments = db.HarvestedSubCompartment.Where(x => x.PlantationHarvestingYearId == inventoryYear.PlantationHarvestingYearId).ToList();

                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();
                        
                    }

                    if (Validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.SendForApprovalTime = DateTime.Now;
                        record.SendForApprovalBy = " ";
                        db.Entry(record).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                    var entity = new FleetInsuranceValidation
                    {
                        FleetInsuranceId = id,
                        Status = Validation.Status,
                        Remark = Validation.Remark,                       
                        DateCreated = DateTime.Now,
                        LastModified = Validation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = Validation.ModifiedBy
                    };

                    db.FleetInsuranceValidation.Add(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { Validation }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}