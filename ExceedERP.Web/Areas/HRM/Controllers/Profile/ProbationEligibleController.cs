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
    public class ProbationEligibleController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

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
            var employeeIds = db.Placements.Select(x => x.EmployeeId).ToList();
            // ViewData["EmployeeList"] = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).Select(x => new { EmployeeId = x.EmployeeId, EmployeeCode = x.EmployeeCode, FullNameAmharic = x.FirstName + " " + x.MiddleName + " " + x.LastName, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();// inputs.GetEmployeesForPlacment(User.Identity.Name);
            ViewData["EmployeeList"] = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).Select(x => new { EmployeeId = x.EmployeeId, HeadId=x.EmployeeId, EmployeeCode = x.EmployeeCode, FullNameAmharic = x.EmployeeCode + " - " + x.FirstName + " " + x.MiddleName + " " + x.LastName, FullNameaEnglish = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish }).ToList();// inputs.GetEmployeesForPlacment(User.Identity.Name);
            ViewData["jobTitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            //var branches = db.Branch;
            //ViewData["branches"] = branches;
            //var workunits = db.MainCompanyStructures.ToList();
            //ViewData["workunits"] = workunits;
            var branches = db.Branch.Select(x => new { x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
            var workunits = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();
            ViewData["workunits"] = workunits;
            ViewData["Employees"] = db.Person_Employee.ToList();
            ViewData["Org"] = db.MainCompanyStructures.ToList();
            ViewData["Jobtitle"] = db.Lookups.ToList();
            ViewData["Measurments"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROBATION_MEASUREMENT).ToList();
            
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
        private List<int> GetEmployeeIdInProbation(List<Person_Employee> employees)
        {
            List<int> employeeIds = new List<int>();

            // Get holidays once to avoid repeated database calls in the loop
            List<DateTime> holidays = db.Holidays.Select(x => x.Date).ToList();

            // Filter employees hired within the last 90 days
            employees = employees
                .Where(x => (DateTime.Now.Date - x.HiredDate.Date).TotalDays < 90)
                .ToList();

            foreach (var employee in employees)
            {
                int probationDays = 0;
                DateTime date = employee.HiredDate;

                // Calculate probation days, counting only weekdays and non-holiday days
                while (probationDays < 90 && (date - employee.HiredDate).TotalDays < 90)
                {
                    date = date.AddDays(1);

                    if (date.DayOfWeek != DayOfWeek.Sunday && !holidays.Contains(date))
                    {
                        probationDays++;
                    }

                    // Check if probation days fall within 60-89 days
                    if (probationDays >= 60 && probationDays < 90)
                    {
                        employeeIds.Add(employee.EmployeeId);
                        break;
                    }
                }
            }

            return employeeIds;
        }

        [AuthorizeRoles(HrmConstants.HRPlacementAdmin, HrmConstants.HRPlacementUser, HrmConstants.HRAdmin)]
        public ActionResult ProbationEligible_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);

            var person_employees = db.Person_Employee.AsNoTracking().ToList().Where(x => mybraches.Contains(x.BranchId.ToString()) && x.IsActive && x.IsPlaced).Where(x => (DateTime.Now.Date - x.HiredDate.Date).TotalDays < 90).ToList();
            List<int> employeeIds = GetEmployeeIdInProbation(person_employees);
            var placements = db.Placements.Where(x=>employeeIds.Contains(x.EmployeeId)||x.EmployementType==EmployeeStatus.PROBATION)
                                        .Where(x => x.isCurrent).ToList();
            
            DataSourceResult result = placements.ToDataSourceResult(request, placement => new
            {
                PlacementId = placement.PlacementId,
                EmployeeId = placement.EmployeeId,
                HeadId = placement.HeadId,
                ImmediateSupervisor = placement.ImmediateSupervisor,
                IsHead = placement.IsHead,
                AssignmentDate = placement.AssignmentDate,
                OrgStructureId = placement.OrgStructureId,
                ParentOrgStructureId = placement.ParentOrgStructureId,
                LookupId = placement.LookupId,
                MappingId = placement.MappingId,
                JobTitleName = GetTitleName(placement.LookupId),
                ReportsToMappingId = placement.ReportsToMappingId,
                RankId = placement.RankId,
                StepId = placement.StepId,
                BranchId = placement.BranchId,
                Salary = placement.Salary,
                Department = placement.Department,
                ReportsTo = placement.ReportsTo,
                AssignmentType = placement.AssignmentType,
                ResponsibilityLevel = placement.ResponsibilityLevel,
                Remark = placement.Remark,
                DateCreated = placement.DateCreated,
                LastModified = placement.LastModified,
                CreatedBy = placement.CreatedBy,
                ModifiedBy = placement.ModifiedBy,
                isCurrent = placement.isCurrent,
                AssignmentDateAmharic = placement.AssignmentDateAmharic,
                EmployeeCode = placement.EmployeeCode,
                EmployementType = placement.EmployementType,
                IsPend = placement.IsPend,
                BusinessUnitId = placement.BusinessUnitId,
                IsApprove = placement.IsApprove,
                FullName = GetFullName(placement.EmployeeId),
                HireDate = HrHelper.GetHireDate(placement.EmployeeId),
                Age = EmployeeHelper.GetAge(placement.EmployeeId)
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
       
      
        public ActionResult UpdateEmployeeProbationStatus(int id)
        {
            try
            {
                var probationsetting = db.ProbationsSettings.FirstOrDefault();
                if (probationsetting == null)
                    return JsonResponse("You have to set Brobation  Settings first.", "OK");

                var performance = db.ProbationPerformances.Find(id);
                if (performance == null)
                    return JsonResponse("This Probation Evalution is not found.", "FAIL");


                var placement = db.Placements.FirstOrDefault(x=>x.isCurrent
                                     &&x.EmployeeId==performance.EmployeeId);
                if (performance == null)
                    return JsonResponse("This Probation Evalution is not found.", "FAIL");



                if (performance.Result >= probationsetting.MinimumFitValue)
                {
                    placement.EmployementType = EmployeeStatus.PERMANENT;
                    placement.IsApprove = true;
                    placement.isCurrent = false;
                    placement.ApprovedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    db.Entry(placement).State = EntityState.Modified;
                    var person = db.Person_Employee.Find(placement.EmployeeId);
                    if(person != null)
                    {
                        person.IsActive = true;
                        person.IsPlaced = true;
                        person.Status = EmployeeStatus.PERMANENT;
                        db.Entry(person).State = EntityState.Modified;

                    }

                    db.SaveChanges();
                    return JsonResponse("You are fit and become Permanent Employee successfully Fit this placement.", "OK");
                }
                else{
                    placement.IsApprove = false;
                    placement.isCurrent = false;
                    placement.ApprovedByEmpId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    db.Entry(placement).State = EntityState.Modified;
                    var person = db.Person_Employee.Find(placement.EmployeeId);
                    if (person != null)
                    {
                        person.IsActive = false;
                        person.IsPlaced = false;
                        db.Entry(person).State = EntityState.Modified;

                    }
                    var entity = new Core.Domain.HRM.EmployeeDetail.Termination
                    {
                        EmployeeId = placement.EmployeeId,
                        LookupId = placement.LookupId,
                        Reason = "probation unfit",
                        ApplicationDate = DateTime.Now,
                        TerminationDate = DateTime.Now,
                        Remark = "Unfit",
                        TerminationDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day,DateTime.Now.Month,DateTime.Now.Year),
                        ApplicationDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                        DateCreated = DateTime.Now,
                        LastModified = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = User.Identity.Name,
                        IsOnlineApproved = true,
                        IsDirect = true,
                        IsForciblyTerminated = true,
                        IsDepartmentSendForApproval = true,
                        IsApproved = true,
                        EmployeeCode = person !=null?person.EmployeeCode:null,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                        //TerminationLevel = termination.TerminationLevel

                    };
                    db.Terminations.Add(entity);
                    db.SaveChanges();

                    return JsonResponse("You are unfit and you have successfully Terminated Employee .", "OK");
                }


                // Update existing placement status
             
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