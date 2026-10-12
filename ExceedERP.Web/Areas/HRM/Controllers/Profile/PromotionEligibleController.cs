using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using System.Web;
using System.IO;
using ExceedERP.Core.Domain.HRM.SalaryStructure;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser)]
    public class PromotionEligibleController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            GetRank();
            GetStep();
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            ViewData["RankList"] = CacheData.GetRankCaches().Select(
               i => new
               {
                   RankName = i.Name,
                   RankId = i.Id,
               }).AsQueryable(); ;
            var stepss = db.Steps.AsQueryable();
            ViewData["StepList"] = CacheData.GetStepCaches().Select(
               i => new
               {
                   StepName = i.Name,
                   StepId = i.Id,

               }).AsQueryable();
            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
               i => new
               {
                   FullNameAmharic = i.AmharicName,
                   FullNameaEnglish = i.Name,
                   EmployeeId = i.Id,
                   HeadId = i.Id
               }).AsQueryable();
            ViewData["jobTitle"] = CacheData.GetJobTitlesCaches().Select(
               i => new
               {
                   Description = i.Name,
                   LookupId = i.Id,
               }).AsQueryable();
            ViewData["branches"] = CacheData.GetBranchCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BranchId = i.Id,
               }).AsQueryable(); ;

            ViewData["businessUnits"] = CacheData.GetBusinessUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BusinessUnitId = i.Id,
               }).AsQueryable(); ; ;
            ViewData["workunits"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable();
            ViewData["isEthiopicDate"] = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate;
            ViewData["isManual"] = db.GeneralJobtitleSettings.FirstOrDefault()?.IsManual;

        }


        public string GetFullName(int empId)
        {
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (employee != null)
            {
                return employee.FullNameaEnglish;
            }

            return "";
        }
     
        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
        public ActionResult PromotionEligible_Read([DataSourceRequest]DataSourceRequest request)
        {
            
            var promotionYear = db.PromotionYearSettings.FirstOrDefault();
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            DateTime minimumDate = promotionYear != null? DateTime.Now.AddYears(-promotionYear.NumberOfPromotionYears): DateTime.Now.AddYears(-2);
            var placements = db.Placements.AsNoTracking()
                .Where(x => x.AssignmentDate <= minimumDate)            // Assignment date is over 2 years old
                .Where(x => x.isCurrent)
                .Where(x => myBranches.Contains(x.BranchId))
                .Where(x => db.Placements.Any(y => y.BranchId == x.BranchId 
                                                    &&y.StepId == x.StepId 
                                                    &&y.AssignmentDate == x.AssignmentDate))
                .OrderBy(x => x.StepId).AsQueryable();
            
            DataSourceResult result = placements.ToDataSourceResult(request, x => new
            {
                x.PlacementId,
                x.EmployeeId,
                EmployeeCode = x.Employee.EmployeeCode,
                x.HeadId,
                x.ImmediateSupervisor,
                x.IsHead,
                x.AssignmentDate,
                x.SalarChangeDate,
                x.SalarChangeDateAmharic,
                x.OrgStructureId,
                x.ParentOrgStructureId,
                x.LookupId,
                x.MappingId,
                x.JobTitleName,
                x.ReportsToMappingId,
                x.RankId,
                x.StepId,
                x.BranchId,
                x.Salary,
                x.Department,
                x.ReportsTo,
                x.AssignmentType,
                x.ResponsibilityLevel,
                x.Remark,
                x.DateCreated,
                x.LastModified,
                x.CreatedBy,
                x.ModifiedBy,
                x.isCurrent,
                x.IsEditable,
                x.AssignmentDateAmharic,
                x.EmployementType,
                x.IsPend,
                x.BusinessUnitId,
                x.IsApprove,
                x.Age,
                x.StepName,
                x.HaveFixedAsset,
                x.ApprovedByEmpId,
                x.NextSalaryChangeDateAmharic,
                x.NextSalaryChangeDate,
                HireDate = x.Employee.HiredDateAmharic,
                HireDateGorg = x.Employee.HiredDate,
                Gender = x.Employee.Gender.ToString(),
                Check = x.HeadId == null && x.IsHead,
               
            });

            return Json(result);
        }
       
        private string GetTitleName(int id)
        {

            try
            {
                db = new ExceedDbContext();
                var name = db.Lookups.Where(x => x.LookupId == id).FirstOrDefault().Description;
                return name;
            }
            catch (Exception)
            {

                return "";
            }
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee placment");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee placment");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        public JsonResult GetRank()
        {
            db = new ExceedDbContext();
            var ranks = db.Ranks.AsQueryable();
            ViewData["RankList"] = ranks;
            return Json(ranks.Select(p => new { RankId = p.RankId, RankName = p.RankName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStep()
        {
            db = new ExceedDbContext();
            var stepss = db.Steps.AsQueryable();
            ViewData["StepList"] = stepss;
            return Json(stepss.Select(p => new { StepId = p.StepId, StepName = p.StepName }), JsonRequestBehavior.AllowGet);
        }
       
      
        public ActionResult PromoteUnpromotedEmployee(int id)
        {
            try
            {
                var promotionsetting = db.PromotionYearSettings.FirstOrDefault();
                if (promotionsetting == null)
                    return JsonResponse("You have to set Promotion Year Settings first.", "OK");

                var placement = db.Placements.Find(id);
                if (placement == null)
                    return JsonResponse("This placement is not found.", "FAIL");

                var step = db.Steps.Find(placement.StepId);
                if (step == null)
                    return JsonResponse("The current Step is not found.", "FAIL");

                int newStepSequenceNumber = step.StepSequence + promotionsetting.StepAmountsToPromote;
                var updatedStep = db.Steps.FirstOrDefault(x => x.StepSequence == newStepSequenceNumber);
                if (updatedStep == null)
                    return JsonResponse("You have to set the new step.", "OK");

                var newSalary = db.Salaries
                    .FirstOrDefault(x => x.StepId == updatedStep.StepId && x.RankId == placement.RankId);

                var newPlacement = CreateNewPlacement(placement, updatedStep, newSalary);
                db.Placements.Add(newPlacement);

                // Update existing placement status
                placement.IsApprove = true;
                placement.isCurrent = false;
                placement.ApprovedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                db.Entry(placement).State = EntityState.Modified;
                db.SaveChanges();
                return JsonResponse("You successfully created this placement.", "OK");
            }
            catch (Exception ex)
            {
                // Log exception here if logging is available
                return JsonResponse("An error occurred while processing your request.", "FAIL");
            }
        }

        private Placement CreateNewPlacement(Placement oldPlacement, Step updatedStep, Salary newSalary)
        {
            return new Placement
            {
                PlacementId = oldPlacement.PlacementId,
                EmployeeId = oldPlacement.EmployeeId,
                HeadId = oldPlacement.HeadId,
                ImmediateSupervisor = oldPlacement.ImmediateSupervisor,
                IsHead = oldPlacement.IsHead,
                AssignmentDate = DateTime.Now,
                AssignmentDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                MappingId = oldPlacement.MappingId,
                RankId = oldPlacement.RankId,
                StepId = updatedStep.StepId,
                LookupId = oldPlacement.LookupId,
                OrgStructureId = oldPlacement.OrgStructureId,
                ParentOrgStructureId = oldPlacement.ParentOrgStructureId,
                Salary = newSalary?.SalaryAmount ?? oldPlacement.Salary,
                ReportsTo = oldPlacement.ReportsTo,
                JobTitleName = oldPlacement.JobTitleName,
                Department = oldPlacement.Department,
                ReportsToMappingId = oldPlacement.ReportsToMappingId,
                AssignmentType = oldPlacement.AssignmentType,
                ResponsibilityLevel = oldPlacement.ResponsibilityLevel,
                BranchId = oldPlacement.BranchId,
                BusinessUnitId = oldPlacement.BusinessUnitId,
                RankName = oldPlacement.RankName,
                StepName = updatedStep.StepName,
                Remark = oldPlacement.Remark,
                DateCreated = oldPlacement.DateCreated,
                LastModified = DateTime.Now,
                CreatedBy = oldPlacement.CreatedBy,
                ModifiedBy = User.Identity.Name,
                isCurrent = true,
                IsApprove = true,
                IsPend = oldPlacement.IsPend,
                IsTemporary = false,
                Index = oldPlacement.Index + 1,
                EmployementType = oldPlacement.EmployementType,
                EmployeeCode = oldPlacement.EmployeeCode,
                OrgBranchName = oldPlacement.OrgBranchName,
                ApprovedByEmpId = oldPlacement.PreparedByEmpId,
                HaveFixedAsset = oldPlacement.HaveFixedAsset,
                PreparedByEmpId = oldPlacement.PreparedByEmpId
            };
        }

        private JsonResult JsonResponse(string message, string status)
        {
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult PromoteUnpromotedEmployee(int id)
        //{
        //    var promotionsetting = db.PromotionYearSettings.FirstOrDefault();
        //    if(promotionsetting != null)
        //    {
        //        var placement = db.Placements.Find(id);
        //        if (placement != null)
        //        {
        //            var step = db.Steps.Find(placement.StepId);
        //            if (step != null)
        //            {
        //                var employeeId = placement.EmployeeId;
        //                var branchId = placement.BranchId;
        //                var user = User.Identity.Name;

        //                int stepSqencenumber = step.StepSequence + promotionsetting.StepAmountsToPromote;
        //                var updatedStep = db.Steps.FirstOrDefault(x => x.StepSequence == stepSqencenumber);
        //                if (updatedStep != null)
        //                {
        //                    var slary = db.Salaries.Where(x => x.StepId == updatedStep.StepId && x.RankId == placement.RankId).FirstOrDefault();

        //                    var entity = new Placement
        //                    {
        //                        PlacementId = placement.PlacementId,
        //                        EmployeeId = placement.EmployeeId,
        //                        HeadId = placement.HeadId,
        //                        ImmediateSupervisor = placement.ImmediateSupervisor,
        //                        IsHead = placement.IsHead,
        //                        AssignmentDate = DateTime.Now,
        //                        AssignmentDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
        //                        MappingId = placement.MappingId,
        //                        RankId = placement.RankId,
        //                        StepId = updatedStep.StepId,
        //                        LookupId = placement.LookupId,
        //                        OrgStructureId = placement.OrgStructureId,
        //                        ParentOrgStructureId = placement.ParentOrgStructureId,
        //                        Salary = slary != null ? slary.SalaryAmount : placement.Salary,
        //                        ReportsTo = placement.ReportsTo,
        //                        JobTitleName = placement.JobTitleName,
        //                        Department = placement.ReportsTo,
        //                        ReportsToMappingId = placement.ReportsToMappingId,
        //                        AssignmentType = placement.AssignmentType,
        //                        ResponsibilityLevel = placement.ResponsibilityLevel,
        //                        BranchId = placement.BranchId,
        //                        BusinessUnitId = placement.BusinessUnitId,
        //                        RankName = placement.RankName,
        //                        StepName = placement.StepName,
        //                        Remark = placement.Remark,
        //                        DateCreated = placement.DateCreated,
        //                        LastModified = placement.LastModified,
        //                        CreatedBy = placement.CreatedBy,
        //                        ModifiedBy = placement.ModifiedBy,
        //                        isCurrent = true, // this will be true after approval
        //                        IsApprove = true,
        //                        IsPend = placement.IsPend,
        //                        IsTemporary = false,
        //                        Index = placement.Index + 1,
        //                        EmployementType = placement.EmployementType,
        //                        EmployeeCode = placement.EmployeeCode,
        //                        OrgBranchName = placement.OrgBranchName,
        //                        ApprovedByEmpId = placement.PreparedByEmpId,
        //                        HaveFixedAsset = placement.HaveFixedAsset,
        //                        PreparedByEmpId = placement.PreparedByEmpId

        //                    };
        //                    db.Placements.Add(entity);
        //                    db.SaveChanges();
        //                    placement.IsApprove = true;
        //                    placement.isCurrent = false;
        //                    placement.ApprovedByEmpId = UserInformationHelper.GetEmployeeId(user);
        //                    db.Entry(placement).State = EntityState.Modified;
        //                    db.SaveChanges();
        //                    // Detach placement entity to prevent multiple tracking
        //                    db.SaveChanges();

        //                    var formattedData134 = new
        //                    {
        //                        Message = "You successfully created  this placement.",
        //                        Status = "OK"
        //                    };
        //                    return Json(formattedData134, JsonRequestBehavior.AllowGet);
        //                }
        //                else
        //                {
        //                    var formattedData = new
        //                    {
        //                        Message = "you have to set for the new step  .",
        //                        Status = "OK"
        //                    };
        //                    return Json(formattedData, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else
        //            {
        //                var formattedData13 = new
        //                {
        //                    Message = "the current Step is not found.",
        //                    Status = "FAIL"
        //                };
        //                return Json(formattedData13, JsonRequestBehavior.AllowGet);
        //            }


        //        }

        //        var formattedData1 = new
        //        {
        //            Message = "This placement is not found.",
        //            Status = "FAIL"
        //        };
        //        return Json(formattedData1, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var formattedData = new
        //        {
        //            Message = "you have to set Promotion Year Settings first.",
        //            Status = "OK"
        //        };
        //        return Json(formattedData, JsonRequestBehavior.AllowGet);

        //    }

        //}
        //public  void GeneratePdf(Placement placement)
        //{
           

        //    // Create a PdfWriter object to write to a file
        //    var employee = db.Person_Employee.Find(placement.EmployeeId);
        //    string FileDirectory = "D:/Projects/ExceedERP-HR/ExceedERP.Web/ContentFiles/PDFFIle/";
        //    string fileName = $"Employee_{employee.EmployeeCode}_{employee.EmployeeId}_{employee.FirstNameEnglish}_{employee.LastNameEnglish}_{placement.AssignmentType.ToString()}.pdf";
        //    string filePath = Path.Combine(FileDirectory, fileName);
        //    if (!Directory.Exists(filePath))
        //    {
        //        Directory.CreateDirectory(filePath);
        //    }
        //    using (var writer = new PdfWriter(filePath))
        //    {
        //        // Create a PdfDocument object
        //        using (var pdf = new PdfDocument(writer))
        //        {
        //            // Create a Document object
        //            var document = new Document(pdf);

        //            // Add content to the document
        //            document.Add(new Paragraph($"Employee Data for EmployeeId: {employee.EmployeeId}"));
        //            document.Add(new Paragraph($"Name: {employee.FirstName} {employee.LastName}"));
        //            // Add other employee details as needed
        //        }
        //    }
        //    UploadPdfToEmployeeArchive(employee.EmployeeId, fileName, filePath);
        //    //Console.WriteLine("PDF generated successfully.");
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}