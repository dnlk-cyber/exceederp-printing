using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Common;
using System.Web;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
   
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class LegacyEmployeesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            ViewData["branches"] = CacheData.GetBranchCaches().Select(i => new
            {
                Name = i.Name,
                BranchId = i.Id,
            }).AsQueryable();
        }

        // ================= READ =================
        public ActionResult LegacyEmployees_Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.LegacyEmployees.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // ================= CREATE =================
        [HttpPost]
        public ActionResult LegacyEmployees_Create([DataSourceRequest] DataSourceRequest request, LegacyEmployee model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;

                db.LegacyEmployees.Add(model);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // ================= UPDATE =================
        [HttpPost]
        public ActionResult LegacyEmployees_Update([DataSourceRequest] DataSourceRequest request, LegacyEmployee model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Now;

                db.LegacyEmployees.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // ================= DELETE =================
        [HttpPost]
        public ActionResult LegacyEmployees_Destroy([DataSourceRequest] DataSourceRequest request, LegacyEmployee model)
        {
            var entity = db.LegacyEmployees.Find(model.LegacyEmployeeId);

            if (entity != null)
            {
                db.LegacyEmployees.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // ================= PROCESS (IMPORTANT) =================
        [HttpPost]
        public ActionResult ProcessToEmployee(int id)
        {
            var legacy = db.LegacyEmployees.Find(id);

            if (legacy == null)
                return Json(new { Status = "Error", Message = "Record not found" });


            string FirstName =legacy.FullName?.Split(' ').Length > 0 ? legacy.FullName.Split(' ')[0] : "first";
            string MiddleName = legacy.FullName?.Split(' ').Length > 1 ? legacy.FullName.Split(' ')[1] : "second";
            string LastName = legacy.FullName?.Split(' ').Length > 2 ? legacy.FullName.Split(' ')[2] : "last";
            //Create Person_Employee
            DateTime date = legacy?.DateCreated??DateTime.Now;
            int age = legacy?.Age ?? 0;
            DateTime dOB = date.AddYears(-age);
            var employee = new Person_Employee
            {
                EmployeeCode = legacy.EmployeeCode,
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                FirstNameEnglish = FirstName,
                MiddleNameEnglish = MiddleName,
                LastNameEnglish = LastName,
                CategoryID = 0,
                BranchId = 0,
                Region = Region.OTHER,
                Religion = ReligionList.OTHER,
                MartialStatus = MartialStatus.OTHER,
                Status = EmployeeStatus.PERMANENT,
                Gender = legacy.Sex.ToLower() == "male" ? Gender.MALE : Gender.FEMALE,
                HiredDate = legacy.RecruitmentDate,
                HiredDateAmharic = HrHelper.GetECDate(legacy.RecruitmentDate),
                DateOfBirth = date.AddYears(-age),
                DateOfBirthAm = HrHelper.GetECDate(dOB),
                IsActive = false,
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now
            };

            db.Person_Employee.Add(employee);
            db.SaveChanges();

            // 👉 Create Placement
            var placement = new Placement
            {
                EmployeeId = employee.EmployeeId,
                AssignmentDate = legacy.RecruitmentDate,
                AssignmentDateAmharic = HrHelper.GetECDate(legacy.RecruitmentDate),
                Salary = legacy.PresentSalary,
                RankId = 0,
                StepId = 0,
                BranchId = 0,
                OrgStructureId = 0,
                ParentOrgStructureId = 0,
                ReportsTo = 0,
                Department = 0,
                LookupId = 0,
                Age = age,
                ResponsibilityLevel = ResponsibilityLevel.STAFF,
                AssignmentType = AssignmentType.NOTSELECTED,
                EmployementType = EmployeeStatus.PERMANENT,
                isCurrent = false,
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now
            };

            db.Placements.Add(placement);

            //  Mark processed
            legacy.IsProcessed = true;

            db.SaveChanges();

            return Json(new { Status = "OK", Message = "Employee Created Successfully" });
        }
        [HttpPost]
        public ActionResult UploadLegacyEmployees(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return Json(new { Status = "Error", Message = "No file uploaded" });

            var list = new List<LegacyEmployee>();

            using (var package = new OfficeOpenXml.ExcelPackage(file.InputStream))
            {
                var sheet = package.Workbook.Worksheets.First();

                int rowCount = sheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++) // skip header
                {
                    string code = sheet.Cells[row, 1].Text;
                    if (db.LegacyEmployees.Any(x => x.EmployeeCode == code))
                        continue;

                    var item = new LegacyEmployee
                    {
                        EmployeeCode = sheet.Cells[row, 1].Text,
                        Dept = sheet.Cells[row, 2].Text,
                        FullName = sheet.Cells[row, 3].Text,
                        Sex = sheet.Cells[row, 4].Text,
                        Age = int.TryParse(sheet.Cells[row, 5].Text, out int age) ? age : (int?)null,

                        Title = sheet.Cells[row, 6].Text,
                        Grade = sheet.Cells[row, 7].Text,
                        PresentSalary = decimal.TryParse(sheet.Cells[row, 8].Text, out decimal sal) ? sal : 0,

                        Education = sheet.Cells[row, 9].Text,
                        FieldOfStudy = sheet.Cells[row, 10].Text,

                        WithInExperience = decimal.TryParse(sheet.Cells[row, 11].Text, out decimal wexp) ? wexp : 0,
                        OutExperience = decimal.TryParse(sheet.Cells[row, 12].Text, out decimal oexp) ? oexp : 0,
                        TotalExperience = decimal.TryParse(sheet.Cells[row, 13].Text, out decimal texp) ? texp : 0,

                        LeaveBalance = decimal.TryParse(sheet.Cells[row, 14].Text, out decimal leave) ? leave : 0,

                        RecruitmentDate = DateTime.TryParse(sheet.Cells[row, 15].Text, out DateTime rec) ? rec : DateTime.Now,
                        DateofTermination = DateTime.TryParse(sheet.Cells[row, 16].Text, out DateTime term) ? term : (DateTime?)null,

                        ReasonforTermination = sheet.Cells[row, 17].Text,
                        Branch = sheet.Cells[row, 18].Text,
                        SubBranch = sheet.Cells[row, 19].Text,

                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    list.Add(item);
                }
            }

            db.LegacyEmployees.AddRange(list);
            db.SaveChanges();

            return Json(new { Status = "OK", Message = $"{list.Count} records uploaded successfully" });
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Legend Employee Employee ");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}