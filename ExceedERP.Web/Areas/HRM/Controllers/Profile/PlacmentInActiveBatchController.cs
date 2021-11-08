using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using Kendo.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    public class PlacmentInActiveBatchController : BaseController
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
            ViewData["Branch"] = db.Branch.ToList();
          //  ViewData["EmployeeList"] = db.Person_Employee.ToList();// inputs.GetEmployeesForPlacment(User.Identity.Name);
            ViewData["EmployeeListEnglish"] = db.Person_Employee.ToList();//inputs.GetEmployeesForPlacment(User.Identity.Name);
            ViewData["JobTitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            ViewData["Department"] = db.MainCompanyStructures.ToList();
        }
     
        public JsonResult DeActivateEmployeesByBranch(int branchId)
        {

            var placements = db.Placements.Where(x => x.isCurrent && x.BranchId == branchId).ToList();

            int countPlacements = placements.Count();

            foreach (var place in placements)
            {
                place.isCurrent = false;

                db.Entry(place).State = EntityState.Modified;
                db.SaveChanges();

                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                if(employee != null)
                {
                    employee.IsActive = false;

                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }           

            var checkPlacementsCount = db.Placements.Where(x => x.isCurrent && x.BranchId == branchId).ToList().Count();

            if (checkPlacementsCount == 0)
            {
                var formattedData1 = new
                {
                    Message = "Successfuly DeActivated",
                    Status = "OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }
            else if (checkPlacementsCount > 0 && checkPlacementsCount < countPlacements)
            {
                var formattedData2 = new
                {
                    Message = "Some Employees Are Not DeActivated",
                    Status = "FAIL"
                };
                return Json(formattedData2, JsonRequestBehavior.AllowGet);
            }
            else if (checkPlacementsCount == countPlacements)
            {
                var formattedData3 = new
                {
                    Message = "Not Successfull",
                    Status = "FAIL"
                };
                return Json(formattedData3, JsonRequestBehavior.AllowGet);
            }
            else if (countPlacements == 0)
            {
                var formattedData4 = new
                {
                    Message = "No Placement under this branch",
                    Status = "FAIL"
                };
                return Json(formattedData4, JsonRequestBehavior.AllowGet);
            }
            var formattedData5 = new
            {
                Message = "",
                Status = "FAIL"
            };
            return Json(formattedData5, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PlacmentInActiveBatch_Read([DataSourceRequest]DataSourceRequest request)
        {
            var placements = db.Placements.Where(x => x.isCurrent).ToList();                    

            DataSourceResult result = placements.ToDataSourceResult(request, placement => new
            {
                PlacementId = placement.PlacementId,
                EmployeeId = placement.EmployeeId,
                ImmediateSupervisor = placement.ImmediateSupervisor,
                IsHead = placement.IsHead,
                AssignmentDate = placement.AssignmentDate,
                OrgStructureId = placement.OrgStructureId,
                LookupId = placement.LookupId,
                JobTitleId = placement.LookupId,
                MappingId = placement.MappingId,
                JobTitleName = placement.JobTitleName,
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
                IsPend = placement.IsPend,
                AssignmentDateAmharic = placement.AssignmentDateAmharic,
                EmployeeCode = placement.EmployeeCode,
                HaveFixedAsset = placement.HaveFixedAsset,
                EmployementType = placement.EmployementType,
                BusinessUnitId = placement.BusinessUnitId,
               // Check = true,//placement.Check
            });

            return Json(result);


        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlacmentInActiveBatch_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PlacementInActiveBatchVM> placementInActiveBatchVMs)
        {
            ModelState.Clear();

            if (placementInActiveBatchVMs.Any() && ModelState.IsValid)
            {
                foreach (var place in placementInActiveBatchVMs)
                {
                    if (place.Check)
                    {
                        var placement = db.Placements.FirstOrDefault(x=>x.PlacementId == place.PlacementId && x.isCurrent);

                        if(placement != null)
                        {
                            placement.isCurrent = false;

                            db.Entry(placement).State = EntityState.Modified;
                            db.SaveChanges();
                        }                                         

                    }

                }

            }

            return Json(placementInActiveBatchVMs.ToDataSourceResult(request, ModelState));
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
     }
}