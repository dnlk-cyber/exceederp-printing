using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using System.Net;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.HRM.Payroll.Vm;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.Core.Domain.Insurance;
using System.Data.Entity;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Finance;

namespace ExceedERP.Web.Areas.HRM.Controllers
{
    [Authorize]
    public class HRMDataProviderController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        private HRMInputs inputs = new HRMInputs();
        public JsonResult GetEmployeeFullName(int id)
        {
            // Find the current placement of the employee with the given ID
            var placement = db.Placements.Where(x => x.EmployeeId == id && x.isCurrent).FirstOrDefault();

            // Initialize the employee variable
            Person_Employee employee = null;

            // If a current placement is found and the HeadId is not null, find the head employee
            if (placement != null && placement.HeadId != null)
            {
                employee = db.Person_Employee.Find((int)placement.HeadId);
            }

            // Format the data
            var formattedData = new
            {
                FullName = employee != null ? employee.EmployeeCode + "-" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish : "Approver not found",
                Status = employee != null ? "OK" : "NOT_OK"
            };

            // Return the formatted data as JSON
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNextApprover(string id)
        {
            ViewBag.id = id;
            return View();
        }
        public JsonResult GetNextApproverList(int id)
        {
            // Find the HeadId of the given employee
            var nextApproveId = db.Placements.FirstOrDefault(x => x.EmployeeId == id)?.HeadId;

            // Get a list of current head employees excluding the given employee and the current approver
            var employeeIds = db.Placements
                .Where(x => x.EmployeeId != id 
                             && x.isCurrent 
                             && x.IsHead 
                             && x.EmployeeId != nextApproveId)
                .Select(x => x.EmployeeId);

            // Retrieve the list of employees based on the filtered IDs
            var employees = db.Person_Employee
                .Where(x => employeeIds.Contains(x.EmployeeId))
                .ToList();

            // Format the data
            var result = employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullName = p.EmployeeCode + "- " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish
            });

            // Return the formatted data as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNextApproverListCopy(int id)
        {
            var NextApproveId = db.Placements.FirstOrDefault(x => x.EmployeeId == id)?.HeadId;
            var employeeIds = db.Placements.Where(x => x.EmployeeId != id && x.isCurrent&&x.IsHead&&x.EmployeeId != NextApproveId).Select(x=>x.EmployeeId);
            var employees = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
           
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullName = p.EmployeeCode + "- " + p.FirstNameEnglish + " "+p.MiddleNameEnglish+" "+p.LastNameEnglish
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateNextApprover(int currentEmployeeId, int nextApproverEmployeeId)
        {
            // Find employee performances that meet the update criteria
            var employeePerformances = db.EmployeePerformances
                .Where(x => x.EmployeeId == currentEmployeeId && !x.IsSecondLevelApproved && (x.IsRejectedByEmployee || x.IsAcceptedByEmployee))
                .ToList();

            if (employeePerformances.Any())
            {
                // Update the SecondLevelApproverId for each performance
                foreach (var employeePerformance in employeePerformances)
                {
                    employeePerformance.SecondLevelApproverId = nextApproverEmployeeId;
                    db.Entry(employeePerformance).State = EntityState.Modified;
                }

                // Save all changes to the database
                db.SaveChanges();

                var formattedData = new
                {
                    Message = "Update successful",
                    Status = "OK"
                };

                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (db.EmployeePerformances.Any(x => x.EmployeeId == currentEmployeeId && !x.IsSecondLevelApproved))
            {
                var formattedData = new
                {
                    Message = "The employee must accept or reject the evaluation result first",
                    Status = "NOT_OKe"
                };

                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var formattedData = new
                {
                    Message = "No records found",
                    Status = "NOT_OK"
                };

                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetClearanceType()
        {
            var oldJobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.CLEARANCE_TYPE ).ToList();

            return Json(oldJobTitle.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeStatus()
        {
            var employeeStatus = Enum.GetValues(typeof(EmployeeStatus))
                                     .Cast<EmployeeStatus>()
                                     .ToList();
            // Manually add the default value (-1)
            //employeeStatus.Insert(0, (EmployeeStatus)(-1));
            return Json(employeeStatus.Select(p => new
            {
                Value = (int)p,
                Text = p.ToString(),
            }).OrderBy(x => x.Value), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCriteriaCategory()
        {
            return Json(db.CriteriaCategories.Select(x => new { x.CriteriaCategoryId, x.Name }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSelfEvaluationCriterias(string level)
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var lookupId = db.Lookups.Where(x => x.EvalutionType == EvaluationType.SelfEvaluation).Select(x => x.LookupId);
            List<MeasurementDefinition> measurments = db.MeasurementDefinitions.Where(x => lookupId.Contains(x.LookupId)).ToList();
            if (level == "Global" || level == "1")
            {
                measurments = db.MeasurementDefinitions.Where(x => !x.IsDepartmentLevel && lookupId.Contains(x.LookupId)).ToList();
            }
            else if (level == "Department" || level == "2")
            {
                measurments = db.MeasurementDefinitions.Where(x => x.IsDepartmentLevel && lookupId.Contains(x.LookupId)).ToList();
            }
            var placement = db.Placements.Where(x => x.EmployeeId == empId&&x.isCurrent).OrderByDescending(x=>x.PlacementId).FirstOrDefault();
            if (placement != null)
            {
                measurments = measurments
                    .Where(x => x.Role == placement.ResponsibilityLevel)
                    .ToList();

            }
            return Json(measurments.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " + GetSelfEvaluationTypeName(p.LookupId) + " - " + p.Role.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMySelfForPerformanceEvaluation()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employees = db.Person_Employee.Where(x =>x.EmployeeId == id).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + "-" + p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeesFromPeertopeerForPerformanceEvaluation(string fiscalyear, string glperiod)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var peerevaluation = db.PeerToPeerPerformanceEvaluations.Where(x => x.EvaluatorId == id).Select(x => x.EmployeeId).ToList();
            if (!string.IsNullOrEmpty(fiscalyear))
            {
                int.TryParse(fiscalyear, out int fiscalYearId);
                peerevaluation = db.PeerToPeerPerformanceEvaluations
                                        .Where(x => x.EvaluatorId == id
                                        && x.PerformanceRequest.FiscalYearId == fiscalYearId).Select(x => x.EmployeeId).ToList();
            }
            if (!string.IsNullOrEmpty(glperiod))
            {
                int.TryParse(glperiod, out int periodId);
                peerevaluation = db.PeerToPeerPerformanceEvaluations
                                        .Where(x => x.EvaluatorId == id
                                        && x.PerformanceRequest.PeriodId == periodId).Select(x => x.EmployeeId).ToList();
            }
            var employees = db.Person_Employee.Where(x => peerevaluation.Contains(x.EmployeeId) && x.EmployeeId != id).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + "-" + p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeesFromInternalCustomerForPerformanceEvaluation(string fiscalyear, string glperiod)
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            IQueryable<PerformanceRequestPeriod> requestPeriods = db.PerformanceRequestPeriods;
            if (int.TryParse(fiscalyear, out int fiscalYearId))
            {
                requestPeriods = requestPeriods.Where(x => x.FiscalYearId == fiscalYearId);
            }

            if (int.TryParse(glperiod, out int periodId))
            {
                requestPeriods = requestPeriods.Where(x => x.PeriodId == periodId);
            }
            HashSet<int> masterIds = new HashSet<int>(requestPeriods.Select(x => x.PerformanceRequestPeriodId));
            HashSet<int> inturnalCustomerIds = new HashSet<int>(db.InternalCustomers.Where(x => x.EmployeeId == id && masterIds.Contains(x.InternalCustomerEvaluation.PerformanceRequestPeriodId)).Select(x=>x.InternalCustomerId));
            var emplloyeeIds = db.InternalCustomerDetails.Where(x => inturnalCustomerIds.Contains(x.InternalCustomerId)).Select(x => x.TobeEvalutedEmployeeId);
            var employees = db.Person_Employee.Where(x => emplloyeeIds.Contains(x.EmployeeId) && x.EmployeeId != id).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + "-" + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish
            }), JsonRequestBehavior.AllowGet);

            
        }
        public string GetSelfEvaluationTypeName(int lookupId)
        {
            var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == lookupId);
            if (lookup != null)
            {
                return lookup.Description;
            }
            return "";
        }
        public JsonResult GetPeerTopeerEvaluationCriterias(string level,string empId)
        {
            var lookupId = db.Lookups.Where(x => x.EvalutionType == EvaluationType.PeerEvaluation).Select(x => x.LookupId);
            List<MeasurementDefinition> measurments = db.MeasurementDefinitions.Where(x => lookupId.Contains(x.LookupId)).ToList();
            if (level == "Global" || level == "1")
            {
                measurments = db.MeasurementDefinitions.Where(x => !x.IsDepartmentLevel && lookupId.Contains(x.LookupId)).ToList();
            }
            else if (level == "Department" || level == "2")
            {
                measurments = db.MeasurementDefinitions.Where(x => x.IsDepartmentLevel && lookupId.Contains(x.LookupId)).ToList();
            }
            if (!string.IsNullOrEmpty(empId))
            {
                if (int.TryParse(empId, out int employeeId))
                {
                    var role = db.Placements
                        .Where(x => x.EmployeeId == employeeId
                                    && x.isCurrent)
                        .Select(x => (ResponsibilityLevel?)x.ResponsibilityLevel)
                        .FirstOrDefault();

                    if (role.HasValue)
                    {
                        measurments = measurments.Where(x => x.Role == role.Value).ToList();
                    }
                }
            }

            return Json(measurments.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " + GetEvaluationTypeName(p.LookupId) + " - " + p.Role.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCorporateOrSectorEvaluationCriterias(string level,string text)
        {
            var lookupIds = level =="6"?db.Lookups.Where(x => x.EvalutionType == EvaluationType.Corporate_Result).Select(x => x.LookupId)
                                      : level == "7" ? db.Lookups.Where(x =>  x.EvalutionType == EvaluationType.Sector_Result).Select(x => x.LookupId)
                                      : level == "8"? db.Lookups.Where(x => x.EvalutionType == EvaluationType.MainDepartmentLevel).Select(x => x.LookupId)
                                      : db.Lookups.Where(x => x.EvalutionType == EvaluationType.Project_Manager).Select(x => x.LookupId);
            
            List<MeasurementDefinition> measurments = db.MeasurementDefinitions.Where(x => lookupIds.Contains(x.LookupId)).ToList();
            if (!string.IsNullOrEmpty(text) && measurments.Any())
            {
                measurments = measurments.Where(x => !string.IsNullOrEmpty(x.EvaluationCriteria) && x.EvaluationCriteria.ToUpper().Contains(text.ToUpper())).ToList();

            }

            return Json(measurments.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " + GetEvaluationTypeName(p.LookupId) + " - " + p.Role.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult InternalCustomerEvaluationCriterias(string level,string empId)
        {
            var lookupId = db.Lookups.Where(x => x.EvalutionType == EvaluationType.InternalCustomer).Select(x => x.LookupId);
            List<MeasurementDefinition> measurments = db.MeasurementDefinitions.Where(x => lookupId.Contains(x.LookupId) /*x.EvalutionType == EvaluationType.InternalCustomer*/).ToList();
            if (level == "Global" || level == "1")
            {
                measurments = db.MeasurementDefinitions.Where(x => lookupId.Contains(x.LookupId) && !x.IsDepartmentLevel /*&& x.EvalutionType == EvaluationType.InternalCustomer*/).ToList();
            }
            else if (level == "Department" || level == "2")
            {
                measurments = db.MeasurementDefinitions.Where(x => lookupId.Contains(x.LookupId) && x.IsDepartmentLevel /*&& x.EvalutionType == EvaluationType.InternalCustomer*/).ToList();
            }
            if (!string.IsNullOrEmpty(empId))
            {
                if (int.TryParse(empId, out int employeeId))
                {
                    var placement = db.Placements.Where(x => x.EmployeeId == employeeId && x.isCurrent).OrderByDescending(x => x.PlacementId).FirstOrDefault();
                    if (placement != null)
                    {
                        measurments = measurments
                            .Where(x => x.Role == placement.ResponsibilityLevel)
                            .ToList();

                    }
                }
            }
            return Json(measurments.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " + GetEvaluationTypeName(p.LookupId) + " - " + p.Role.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPayrollPeriods()
        {

            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);


            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLookUpdata()
        {
            return Json(db.Lookups.Where(x=>x.LookupType==LookUpTypes.ADDITION_TYPE).Select(p => new
            {
                LookupId=p.LookupId,
                Code=p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeTypes()
        {
            var data = db.EmployeeTypes.ToList();
            return Json(data.Select(p => new
            {
                p.EmployeeTypeId,
                p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEvaluationCriterias(string criteriaCategoryId, string level, string workUnit = null, string empId = null)
        {
            // 1. Exclude specific evaluation types
            var excludedLookupIds = db.Lookups
                .Where(x =>
                    x.EvalutionType == EvaluationType.SelfEvaluation ||
                    x.EvalutionType == EvaluationType.PeerEvaluation ||
                    x.EvalutionType == EvaluationType.InternalCustomer ||
                    x.EvalutionType == EvaluationType.Sector_Result ||
                    x.EvalutionType == EvaluationType.Corporate_Result ||
                    x.EvalutionType == EvaluationType.MainDepartmentLevel ||
                    x.EvalutionType == EvaluationType.Project_Manager)
                .Select(x => x.LookupId);

            // 2. Start with all relevant measurements (as IQueryable)
            var query = db.MeasurementDefinitions
                .Where(x => !excludedLookupIds.Contains(x.LookupId));

            // 3. Filter by criteria category
            if (int.TryParse(criteriaCategoryId, out int categoryId))
            {
                query = query.Where(x => x.CriteriaCategoryId == categoryId);
            }

            // 4. Filter by level (Global vs Department)
            if (level == "Global" || level == "1")
            {
                query = query.Where(x => !x.IsDepartmentLevel);
            }
            else if (level == "Department" || level == "2")
            {
                query = query.Where(x => x.IsDepartmentLevel);

                if (int.TryParse(workUnit, out int orgId))
                {
                    query = query.Where(x => x.OrgStuructureId == orgId);
                }
            }

            // 5. Filter by employee's responsibility level if provided
            if (int.TryParse(empId, out int employeeId))
            {
                var role = db.Placements
                    .Where(x => x.EmployeeId == employeeId
                                &&x.isCurrent)
                    .Select(x => (ResponsibilityLevel?)x.ResponsibilityLevel)
                    .FirstOrDefault();

                if (role.HasValue)
                {
                    query = query.Where(x => x.Role == role.Value);
                }
            }

            // 6. Materialize and format the result
            var result = query.ToList().Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = $"{p.EvaluationCriteria} - {GetEvaluationTypeName(p.LookupId)} - {p.Role}"
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetEvaluationCriterias(string criteriaCategoryId,string level,string workUnit=null,string empId=null)
        //{
        //    var lookupIds = db.Lookups.Where(x => x.EvalutionType == EvaluationType.SelfEvaluation 
        //                                        || x.EvalutionType == EvaluationType.PeerEvaluation 
        //                                        || x.EvalutionType == EvaluationType.InternalCustomer
        //                                        || x.EvalutionType == EvaluationType.Sector_Result
        //                                        || x.EvalutionType == EvaluationType.Corporate_Result
        //                                        || x.EvalutionType == EvaluationType.MainDepartmentLevel
        //                                        || x.EvalutionType == EvaluationType.Project_Manager
        //                                        ).Select(x => x.LookupId).ToList();
        //    List<MeasurementDefinition> measurments = db.MeasurementDefinitions.Where(x=>! lookupIds.Contains(x.LookupId)).ToList();
        //    if (!string.IsNullOrEmpty(criteriaCategoryId) && int.TryParse(criteriaCategoryId, out int categoryId))
        //    {
        //        measurments = measurments.Where(x => x.CriteriaCategoryId == categoryId).ToList();
        //    }
        //    if (level == "Global" || level == "1")
        //    {
        //        measurments = measurments.Where(x => !x.IsDepartmentLevel).ToList();
        //    }
        //    else if (level == "Department" || level == "2")
        //    {
        //        measurments = measurments.Where(x => x.IsDepartmentLevel).ToList();
        //        if(!string.IsNullOrEmpty(workUnit)&&int.TryParse(workUnit,out int orgId))
        //        {
        //            measurments = measurments.Where(x => x.OrgStuructureId == orgId).ToList();
        //        }

        //    }
        //    if (!string.IsNullOrEmpty(empId))
        //    {
        //        if (int.TryParse(empId, out int employeeId))
        //        {
        //            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employeeId);
        //            if (placement != null)
        //            {
        //                measurments = measurments
        //                    .Where(x => x.Role == placement.ResponsibilityLevel)
        //                    .ToList();

        //            }
        //        }
        //    }
        //    return Json(measurments.Select(p => new
        //    {
        //        MeasurementDefinitionId = p.MeasurementDefinitionId,
        //        Name = p.EvaluationCriteria + " - " + GetEvaluationTypeName(p.LookupId) + " - " + p.Role.ToString()
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public string GetEvaluationTypeName(int lookupId)
        {
            var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == lookupId);
            if (lookup != null)
            {
                return lookup.Description;
            }
            return "";
        }
        public JsonResult GetOldandNewJobTitles()
        {
            var oldJobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.OLD_JOB_TITLE || x.LookupType == LookUpTypes.JOB_TITLE).ToList();

            return Json(oldJobTitle.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriod()
        {

            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var list = from p in periodList
                       join g in db.GLPeriods
                           on p.Name equals g.Name
                       select (new
                       {
                           g
                       });

            return Json(list.Select(p => new
            {
                GLPeriodId = p.g.GLPeriodId,
                Name = p.g.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodOther()
        {

            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var listt = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });
            // var fisicalyear = listt.Select(x => x.g).ToList(); ;

            return Json(listt.Select(x => x.g), JsonRequestBehavior.AllowGet);

        }
        //public JsonResult GetAllEmployeesForPlacement(string text)
        //{
        //    List<EmployeeVm> list = new List<EmployeeVm>();

        //    if (!String.IsNullOrWhiteSpace(text))
        //    {
        //        var employees = db.Person_Employee.Where(x=>x.IsActive)
        //            .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
        //                x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
        //                  x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
        //                    x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
        //                     (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
        //                      (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
        //            )
        //            .Select(x => new EmployeeVm
        //            {
        //                EmployeeId = x.EmployeeId,
        //                EmployeeCode = x.EmployeeCode,
        //                FirstName = x.FirstName,
        //                MiddleName = x.MiddleName,
        //                LastName = x.LastName,
        //                FirstNameEnglish = x.FirstNameEnglish,
        //                MiddleNameEnglish = x.MiddleNameEnglish,
        //                LastNameEnglish = x.LastNameEnglish
        //            }).ToList();

        //        list = employees;
        //    }

        //    return Json(list.Select(p => new
        //    {
        //        EmployeeId = p.EmployeeId,
        //        FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
        //        FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetAllEmployeesProfile(string text, int? employeeId)
        {
            var query = db.Person_Employee.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.ToUpper();
                query = query.Where(x =>
                    x.EmployeeCode.ToUpper().Contains(text) ||
                    x.FirstNameEnglish.ToUpper().Contains(text) ||
                    x.MiddleNameEnglish.ToUpper().Contains(text) ||
                    x.LastNameEnglish.ToUpper().Contains(text) ||
                    (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text) ||
                    (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text)
                );
            }
            else if (employeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }
            
            var employees = query.Select(x => new
            {
                EmployeeId = x.EmployeeId,
                FullNameaEnglish = x.EmployeeCode + " - " + x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish
            }).ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllEmployeesForPlacement(string text)
        {
            var query = db.Person_Employee.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.ToUpper();

                query = query.Where(x =>
                    x.EmployeeCode.ToUpper().Contains(text) ||
                    x.FirstNameEnglish.ToUpper().Contains(text) ||
                    x.MiddleNameEnglish.ToUpper().Contains(text) ||
                    x.LastNameEnglish.ToUpper().Contains(text) ||
                    (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text) ||
                    (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text)
                );
            }


            var employees = query.Select(x => new
            {
                EmployeeId = x.EmployeeId,
                FullNameaEnglish = x.EmployeeCode + " - " + x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish,
                FullNameAmharic = x.EmployeeCode + " - " + x.FirstName + " " + x.MiddleName + " " + x.LastName
            }).ToList();

            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllEmployeesForPerformance(string text)
        {
            List<EmployeeVm> list = new List<EmployeeVm>();

            if (!string.IsNullOrWhiteSpace(text))
            {
                var employees = db.Person_Employee
                    .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                        x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                          x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                              (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                    )
                    .Select(x => new EmployeeVm
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        FirstNameEnglish = x.FirstNameEnglish,
                        MiddleNameEnglish = x.MiddleNameEnglish,
                        LastNameEnglish = x.LastNameEnglish
                    }).ToList();

                list = employees;
            }

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllHeadEmployeesForPlacement()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employeeIds = db.Placements.Where(x => x.isCurrent && x.IsHead/*&&x.EmployeeId !=id*/).Select(x => x.EmployeeId);
            var employees = db.Person_Employee.Where(x=>x.IsActive).Where(x => employeeIds.Contains(x.EmployeeId))
                    .Select(x => new
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        FirstNameEnglish = x.FirstNameEnglish,
                        MiddleNameEnglish = x.MiddleNameEnglish,
                        LastNameEnglish = x.LastNameEnglish
                    }).AsQueryable();

          
           

            return Json(employees.Select(p => new
            {
                HeadId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllEmployeesForSelfCare(string text)
        {
            List<EmployeeVm> list = new List<EmployeeVm>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var employees = db.Person_Employee.Where(x=>x.IsActive)
                    .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                        x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                          x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                              (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                    )
                    .Select(x => new EmployeeVm
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        FirstNameEnglish = x.FirstNameEnglish,
                        MiddleNameEnglish = x.MiddleNameEnglish,
                        LastNameEnglish = x.LastNameEnglish
                    }).ToList();

                list = employees;
            }

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFisicalYear()
        {

            var periodList = db.GLFiscalYears.OrderByDescending(x=>x.GlFiscalYearId).ToList();
            //var fisicalYear = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetFisicalYearForRecruitmentAndManPower()
        {
            var periodList = db.GLFiscalYears.Where(x=>x.DateFrom>=DateTime.Now||x.DateTo>= DateTime.Now).OrderByDescending(x=>x.GlFiscalYearId).ToList();
            var fisicalYear = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCurrentPeriods()
        {
            List<GLPeriod> glPeriods = new List<GLPeriod>();
            var year = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Today && x.DateTo >= DateTime.Today);
            if (year != null)
            {
                var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == year.GlFiscalYearId).ToList();
                glPeriods.AddRange(periods);
            }

            return Json(glPeriods.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPeriods(string GlFiscalYearId)
        {
            int.TryParse(GlFiscalYearId, out int yearId);
            List<GLPeriod> periodList = new List<GLPeriod>();
            var periods = db.GLPeriods
                                .Where(x => x.GlFiscalYearId == yearId)
                                .OrderByDescending(x=>x.GLPeriodId)
                                .ToList();

            if (periods.Any())
            {
                periodList.AddRange(periods);
            }
            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetGlPeriod()
        {
            var periodList = db.GLPeriods.OrderByDescending(x => x.DateTo).ToList();

            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetGlPeriodFromOpenBudgetYear()
        {

            var periodList = new List<GLPeriod>();//db.GLPeriods.OrderByDescending(x => x.GLPeriodId).ToList();
            var year = db.GLFiscalYears.Where(x => x.Status == FiscalYearStatus.Open).Select(x => x.GlFiscalYearId);

            if (year.Any())
            {
                var periods = db.GLPeriods.Where(x => year.Contains(x.GlFiscalYearId)).OrderByDescending(x => x.GLPeriodId).ToList();
                periodList.AddRange(periods);
            }

            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCurrentYearPeriods()
        {
            var periodList = db.GLPeriods.ToList();
            var year = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom.Year == DateTime.Now.Year);
            if (year != null)
            {
                periodList = periodList.Where(x => x.GlFiscalYearId == year.GlFiscalYearId).ToList();
            }

            //var fisicalYear = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetSelectedPeriod()
        {
            var periodList = db.GLFiscalYears.AsQueryable();
            var selectedperiod = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(selectedperiod.Select(p => new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTrainingProvider()
        {

            var providerlist = db.Companies.AsQueryable();
            if (providerlist.Any())
            {
                providerlist = providerlist.Where(c => c.Category == "1" && c.IsActive).AsQueryable();
            }

            return Json(providerlist.Select(p => new
            {
                MainCompanyId = p.MainCompanyId,
                TradeName = p.TradeName
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTrainingTypeList()
        {
            var traTypeList = db.Lookups.AsQueryable();
            if (traTypeList.Any())
            {
                traTypeList = traTypeList.Where(l => l.LookupType == LookUpTypes.TRAINING_TYPE).OrderBy(o => o.LookupType);

            }
            return Json(traTypeList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCourseList()
        {
            var courseList = db.Lookups.AsQueryable();

            if (courseList.Any())
            {
                courseList = courseList.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive).AsQueryable();

            }
            var courses = courseList != null ? courseList.ToList() : new List<Lookup>();
            return Json(courses.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetEducationLevel()
        {
            var eduLevel = db.Lookups.Where(x=>x.IsActive).AsQueryable();

            if (eduLevel.Any())
            {
                eduLevel = eduLevel.Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL && x.IsActive).AsQueryable();

            }
            var level = eduLevel != null ? eduLevel.ToList() : new List<Lookup>();
            return Json(level.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetProject()
        {
            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();

            return Json(types.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProbationType()
        {
            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROBATION_STATUS).ToList();

            return Json(types.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRequestStatus()
        {
            var requests = Enum.GetValues(typeof(RequestStatus))
                         .Cast<RequestStatus>()
                         .Select(v => v).Where(item => item != RequestStatus.DEPARTMENT_APPROVAL && item != RequestStatus.PEND)
                         .ToList();
            //foreach (var kk in requests)
            //{
            //    int ii = (int)kk;
            //    string ll = kk.ToString();
            //}
            return Json(requests.Select(p => new
            {
                Value = (int)p,
                Text = p.ToString(),
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartments(int BranchId)
        {
            var jobTitleMappings = db.JobTitleMapping.Where(x => x.BranchId == BranchId).ToList();
            if (jobTitleMappings.Any())
            {
                var orgIds = jobTitleMappings.Select(x => x.OrgStructureId).ToList();
                if (orgIds.Any())
                {
                    var structures = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => orgIds.Contains(x.OrgStructureId))
                        .Where(x=>x.IsActive)
                        .AsQueryable();
                    return Json(structures.Select(p => new
                    {
                        OrgStructureId = p.OrgStructureId,
                        Name = p.Name

                    }), JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }
        public JsonResult GetDepartmentForComplaints()
        {
            var jobTitleMappings = db.JobTitleMapping.Where(x=>x.IsActive).ToList();
            if (jobTitleMappings.Any())
            {
                var orgIds = jobTitleMappings.Select(x => x.OrgStructureId).ToList();
                if (orgIds.Any())
                {
                    var structures = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => orgIds.Contains(x.OrgStructureId))
                        .Where(x=>x.IsActive)
                        .AsQueryable();
                    return Json(structures.Select(p => new
                    {
                        OrgStructureId = p.OrgStructureId,
                        Name = p.Name

                    }), JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }
        public JsonResult GetDepartmentsForPlacement(int OrgStructureId, bool isHead)
        {
            if (isHead)
            {
                var parent = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId==OrgStructureId&& x.IsActive);
                if(parent != null)
                {
                    var orgStructures = db.MainCompanyStructures
                                        .Where(x=>x.IsActive)
                                        .Where(x => x.OrgStructureId == parent.Parent).AsQueryable();
                    return Json(orgStructures.Select(p => new
                    {
                        OrgStructureId = p.Parent,
                        Name = p.Name

                    }), JsonRequestBehavior.AllowGet);
                }
                var WorkUnit = db.MainCompanyStructures
                                                    .Where(x => x.OrgStructureId == OrgStructureId)
                                                    .Where(x => x.IsActive).AsQueryable();
                return Json(WorkUnit.Select(p => new
                {
                    OrgStructureId = p.OrgStructureId,
                    Name = p.Name

                }), JsonRequestBehavior.AllowGet);

            }
            var structures = db.MainCompanyStructures
                .Where(x=>x.OrgStructureId==OrgStructureId)
                .Where(x => x.IsActive).AsQueryable();
            return Json(structures.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDepartmentsToMakeParent()
        {
            var structures = db.MainCompanyStructures.Where(x => x.IsActive).AsQueryable();

            return Json(structures.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentForManinDepartmentSettings()
        {
            var structures = db.MainCompanyStructures.Where(x => x.IsActive).AsQueryable();

            return Json(structures.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllWorkUnitForPerformances(int evaluationCriteriaId, string businessUnitId, string branchId, string text)
        {
            var orgstuctureLists = new List<MainCompanyStructure>();
            var placements = db.Placements.Where(x => x.isCurrent).ToList();
            if (!string.IsNullOrEmpty(branchId)&& placements.Any())
            {
                int.TryParse(branchId, out int braId);
                placements = placements.Where(x => x.BranchId == braId).ToList();
            }
            if (!string.IsNullOrEmpty(businessUnitId) && placements.Any())
            {
                int.TryParse(businessUnitId, out int busisId);
                placements = placements.Where(x => x.BusinessUnitId == busisId).ToList();
            }

            var workUnitIds = placements.Select(x => x.OrgStructureId).Distinct();
            var orgstuctures = db.MainCompanyEvalutionSettings.Where(x => x.MeasurementDefinitionId == evaluationCriteriaId && x.OrgStructureId != null);

            HashSet<int> orgIds = new HashSet<int>(orgstuctures.Where(x=> workUnitIds.Contains((int)x.OrgStructureId)).Select(x => (int)x.OrgStructureId));

            //var structures = orgIds.Any()? db.MainCompanyStructures.Where(x=> orgIds.Contains(x.OrgStructureId)).Where(x => x.IsActive).ToList(): orgstuctureLists;
            var structures = orgIds.Any()? db.MainCompanyStructures.Where(x=> orgIds.Contains(x.OrgStructureId)).Where(x => x.IsActive).ToList():workUnitIds.Any()? db.MainCompanyStructures.Where(x=>workUnitIds.Contains(x.OrgStructureId)).ToList(): orgstuctureLists;

            if (!string.IsNullOrEmpty(text)&& structures.Any())
            {

                structures = structures.Where(x => x.IsActive).ToList().Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(text.ToUpper())).ToList();
                //if (ids.Any())
                //{
                //    structures = structures.Where(x => ids.Contains(x.OrgStructureId)).AsQueryable();
                //    if(branchId != 0)
                //    {
                //       // var workUnitIds = db.JobTitleMapping.Where(x => x.IsActive && x.BranchId == branchId).Select(x => x.OrgStructureId);
                //        if (structures.Any() && workUnitIds.Any())
                //        {
                //            structures = structures.Where(x => workUnitIds.Contains(x.OrgStructureId)).AsQueryable();

                //        }
                //    }
                //}
            }
            if (structures.Any())
            {
                return Json(structures.Select(p => new
                {
                    OrgStructureId = p.OrgStructureId,
                    Name = p.Name
                }), JsonRequestBehavior.AllowGet);
            }
            return Json(orgstuctureLists, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllJobTitleForPerformances(int evaluationCriteriaId,int branchId,string text)
        {
            var maindepartment = db.MainCompanyEvalutionSettings.Where(x => x.MeasurementDefinitionId == evaluationCriteriaId && x.LookUpId != null);
            HashSet<int> lookupIds = new HashSet<int>(maindepartment.Select(x => (int)x.LookUpId));

            var lookups = db.Lookups.Where(x=>lookupIds.Contains(x.LookupId)&&x.LookupType==LookUpTypes.JOB_TITLE)
                                    .Where(x => x.LookupType == LookUpTypes.JOB_TITLE
                                                || x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                                || x.LookupType == LookUpTypes.Jobtitle_External_Experience)
                                                .Where(x => x.IsActive).AsQueryable();
            if (!String.IsNullOrEmpty(text) && lookups.Any())
            {

                lookups = lookups.Where(x => !string.IsNullOrEmpty(x.Description) && x.Description.ToUpper().Contains(text.ToUpper())).AsQueryable();
                if (branchId !=0)
                {
                    var jobTitleIds  = db.JobTitleMapping.Where(x =>x.BranchId==branchId).Where(x=>x.IsActive).Select(x=>x.LookupId).Distinct();
                    if (jobTitleIds.Any())
                    {
                        lookups = lookups.Where(x => jobTitleIds.Contains(x.LookupId)).AsQueryable();
                    }

                }
            }
            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitle(int BranchId, int OrgStructureId)
        {
            var manPowerPlannings = db.JobTitleMapping.Where(x => x.BranchId == BranchId && x.OrgStructureId == OrgStructureId).ToList();

            if (manPowerPlannings.Any())
            {
                var jobIds = manPowerPlannings.Select(x => x.LookupId).ToList();

                var jobTitle = db.Lookups.Where(x => jobIds.Contains(x.LookupId) && x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive).Where(x => x.LookupType == LookUpTypes.JOB_TITLE
                                                || x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                                || x.LookupType == LookUpTypes.Jobtitle_External_Experience).ToList();

                return Json(jobTitle.Select(p => new
                {
                    LookupId = p.LookupId,
                    Description = p.Description,
                }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetJobTitleForManPower(int BranchId)
        {
            var manPowerPlannings = db.JobTitleMapping.Where(x=>x.IsActive).Where(x => x.BranchId == BranchId).ToList();

            if (manPowerPlannings.Any())
            {
                var jobIds = manPowerPlannings.Select(x => x.LookupId).ToList();

                var jobTitle = db.Lookups.Where(x => jobIds.Contains(x.LookupId) && x.IsActive)
                                         .Where(x => x.LookupType == LookUpTypes.JOB_TITLE
                                                   || x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                                   || x.LookupType == LookUpTypes.Jobtitle_External_Experience).ToList();

                return Json(jobTitle.Select(p => new
                {
                    LookupId = p.LookupId,
                    Description = p.Description,
                }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetAllJobTitle()
        {
            var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE 
                                                ||x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                                ||x.LookupType == LookUpTypes.Jobtitle_External_Experience 
                                                && x.IsActive).ToList();

            return Json(jobTitle.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllJobTitleForVaancy()
        {
            var jobTitleIds = db.ExperienceJobTitles.Select(x => x.LookupId);
            var jobTitle = db.Lookups
                .Where(x =>jobTitleIds.Contains(x.LookupId)).ToList();

            return Json(jobTitle.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetOldJobTitles()
        {
            var oldJobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.OLD_JOB_TITLE && x.IsActive).ToList();

            return Json(oldJobTitle.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleForExternalExperience()
        {
            var externalJobtitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.Jobtitle_External_Experience
                                                        ||x.LookupType == LookUpTypes.JOB_TITLE 
                                                        ||x.LookupType == LookUpTypes.OLD_JOB_TITLE 
                                                        && x.IsActive)
                                                        .ToList();

            return Json(externalJobtitle.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobTitleForRecruitment(string OrgStructureId, string searchText)
        {
            List<Jobs> jobList = new List<Jobs>();
            //var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive && x.Description==searchText).ToList();
            //var jobTitle = db.RecruitmentPlans.Where(x => x.WorkUnitId.ToString() == OrgStructureId && x.JobtitleName.ToUpper(XAPI.Common.exceedculture).Contains(searchText.ToUpper(XAPI.Common.exceedculture)));

            //var jobTitle = db.RecruitmentPlans.Where(x => x.OrgStructureId.ToString() == OrgStructureId).ToList();

            int.TryParse(OrgStructureId, out int orgStructId);
            var jobTitles = db.JobTitleMapping.Where(x => x.OrgStructureId == orgStructId).ToList();
            foreach (var jj in jobTitles)
            {
                var job = new Jobs();

                job.LookupId = jj.LookupId;
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == jj.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                if (lookup != null)
                {
                    job.Description = lookup.Description;
                }
                jobList.Add(job);

            }

            //if (!String.IsNullOrEmpty(searchText))
            //{
            //jobTitle = jobTitle.Where(x =>
            //    x.JobtitleName.ToUpper(XAPI.Common.exceedculture)
            //        .Contains(searchText.ToUpper(XAPI.Common.exceedculture))).ToList();

            //    jobTitles = jobTitles.Where(x =>x.id).ToList();
            //}
            return Json(jobList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkUnitFromRecruitmentPlan(string BranchId, string searchText)
        {
            List<MainCompanyStructure> workUnitsList = new List<MainCompanyStructure>();
            var jobTitleMappings = db.JobTitleMapping.Where(x => x.BranchId.ToString() == BranchId).ToList();
            if (!String.IsNullOrEmpty(searchText))
            {
                foreach (var recruit in jobTitleMappings)
                {
                    var mainCompany = db.MainCompanyStructures.Where(x => x.IsActive).FirstOrDefault(x => x.OrgStructureId == recruit.OrgStructureId);
                    if (mainCompany != null)
                    {
                        workUnitsList.Add(mainCompany);
                    }
                }
                var searched = workUnitsList.Where(x => x.Name.ToString().ToUpper(XAPI.Common.exceedculture).Contains(searchText.ToUpper(XAPI.Common.exceedculture))).ToList();
                workUnitsList.Clear();
                workUnitsList.AddRange(searched);
            }
            else
            {
                foreach (var recruit in jobTitleMappings)
                {
                    var mainCompany = db.MainCompanyStructures.Where(x => x.IsActive).FirstOrDefault(x => x.OrgStructureId == recruit.OrgStructureId);
                    if (mainCompany != null)
                    {
                        workUnitsList.Add(mainCompany);
                    }
                }
            }

            //var recruitmentPlan = db.RecruitmentPlans.Where(x => x.BranchId.ToString() == BranchId).ToList();
            //if (recruitmentPlan.Any())
            //{
            //    foreach (var plan in recruitmentPlan)
            //    {
            //        var mainCompany =
            //            db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == plan.OrgStructureId);
            //        if (mainCompany != null)
            //        {
            //            workUnitsList.Add(mainCompany);
            //        }
            //    }
            //}

            return Json(workUnitsList.DistinctByField(x => x.OrgStructureId).Select(p => new
            {
                Name = p.Name,
                OrgStructureId = p.OrgStructureId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleForPromotion(string mediacode)
        {
            db = new ExceedDbContext();
            var plans = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode.ToString() == mediacode).ToList();
            List<ApplicantRegistrationViewModel> titleList = new List<ApplicantRegistrationViewModel>();
            if (plans.Any())
            {
                ApplicantRegistrationViewModel appRegister = new ApplicantRegistrationViewModel();
                foreach (var p in plans)
                {
                    appRegister = new ApplicantRegistrationViewModel();
                    var detail = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == p.VacancyId 
                                                            && (x.RecruSource == RecruitmentSource.INTERNAL
                                                                ||x.RecruSource == RecruitmentSource.INTERNALPROMOTION
                                                                ||x.RecruSource == RecruitmentSource.INTERNALTRANSFER));
                    if (detail != null)
                    {
                        var recruitPlan = db.RecruitmentPlans.Find(detail.RecruitmentPlanId);
                        if (recruitPlan != null)
                        {
                            var lookup = db.Lookups.Where(x => x.LookupId == recruitPlan.LookupId).FirstOrDefault();
                            if (lookup != null)
                            {
                                appRegister.LookUpId = lookup.LookupId;
                                appRegister.VacancyId = detail.VacancyId;
                                appRegister.Description = lookup.Description;
                                titleList.Add(appRegister);
                            }
                        }

                    }


                }
            }
            return Json(titleList.Select(p => new
            {
                LookupId = p.LookUpId + "-" + p.VacancyId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetJobTitlesForPlacement(int orgStructureId, string text)
        {
            var titleList = (from jm in db.JobTitleMapping
                             join l in db.Lookups on jm.LookupId equals l.LookupId
                             where jm.IsActive && jm.OrgStructureId == orgStructureId
                             select l)
                            .Distinct()
                            .ToList();

            if (!string.IsNullOrEmpty(text))
            {
                var upperText = text.ToUpper();
                titleList = titleList
                    .Where(x => x.Description != null && x.Description.ToUpper().Contains(upperText))
                    .ToList();
            }

            return Json(titleList.Select(p => new
            {
                p.LookupId,
                p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobTitles(int OrgStructureId)
        {
            var orgs = db.JobTitleMapping.Where(x=>x.IsActive).Where(x => x.OrgStructureId == OrgStructureId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    if (!titleList.Contains(lookup))
                    {
                        titleList.Add(lookup);
                    }

                }
            }
           
            return Json(titleList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetJobTitlesForTemppraryPlacement()
        {
            var orgs = db.JobTitleMapping.Where(x=>x.IsActive).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    if (!titleList.Contains(lookup))
                    {
                        titleList.Add(lookup);
                    }

                }
            }
            return Json(titleList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetSalary()
        {
            var Salaries = db.Salaries.ToList();

            return Json(Salaries.Select(p => new
            {
                SalaryStructureId = p.SalaryStructureId,
                SalaryAmount = p.SalaryAmount,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleSelected()
        {
            var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE);
            var jobtitleselected = jobTitle.Any() ? jobTitle.ToList() : new List<Lookup>();

            return Json(jobtitleselected.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleMapping(string mediacode)
        {

            db = new ExceedDbContext();
            List<ApplicantRegistrationViewModel> titleList = new List<ApplicantRegistrationViewModel>();
            var plans = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode.ToString() == mediacode).ToList();
            if (plans.Any())
            {
                titleList = new List<ApplicantRegistrationViewModel>();
                ApplicantRegistrationViewModel appRegister = new ApplicantRegistrationViewModel();
                foreach (var p in plans)
                {
                    appRegister = new ApplicantRegistrationViewModel();
                    var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == p.VacancyId && x.RecruSource == RecruitmentSource.EXTERNAL && x.IsActive /*&& !x.IsComplete*/);
                    if (vacancy != null)
                    {

                        var vacPlan = db.RecruitmentPlans.Find(vacancy.RecruitmentPlanId);
                        if (vacPlan != null)
                        {
                            var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == vacPlan.LookupId);
                            if (lookup != null)
                            {
                                appRegister.VacancyId = vacancy.VacancyId;
                                appRegister.LookUpId = lookup.LookupId;
                                appRegister.Description = lookup.Description;
                                titleList.Add(appRegister);
                            }
                        }

                    }


                }
            }
            return Json(titleList.Select(p => new
            {
                LookupId = p.LookUpId + "-" + p.VacancyId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetRelationshipList()
        {
            var relations = db.Lookups.Where(l => l.LookupType == LookUpTypes.RELATIONSHIP);

            var relationship = relations != null ? relations.ToList() : new List<Lookup>();
            return Json(relationship.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetAssetLists()
        //{

        //    var assetList = db.Fleets;
        //    return Json(assetList.Select(p => new
        //    {
        //        FleetsID = p.FleetsID,
        //        Name = p.EquipmentName,
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetEducationFieldList()
        {

            var lookups = db.Lookups.Where(l =>l.IsActive
                                                && l.LookupType == LookUpTypes.EDUCATION_FIELD).AsQueryable();
            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEducationLevelList()
        {

            var lookups = db.Lookups.Where(l =>l.IsActive&& l.LookupType == LookUpTypes.EDUCATION_LEVEL).AsQueryable();
            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCommitmentCosignType()
        {

            var cosignTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COSIGN_TYPE);
            return Json(cosignTypes.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCommitment()
        {

            var commtTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COMMITMENT_TYPE);
            return Json(commtTypes.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllEmployees()
        {

            var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + "-" + p.FullNameaEnglish,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllActiveEmployees()
        {

            var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + "-" + p.FirstNameEnglish+" "+p.MiddleNameEnglish+" "+p.LastNameEnglish,
            }), JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult GetTotalEmployees()
        {

            var employees = db.Person_Employee.Where(x=>x.IsActive).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + "-" + p.FullNameaEnglish,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBankLists()
        {

            var bankList = db.Banks;
            return Json(bankList.Select(p => new
            {
                BankId = p.BankId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTermination()
        {
            var terminationTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.TERMINATIONTYPE);
            return Json(terminationTypes.Select(p => new
            {
                Description = p.Description,
                LookupId = p.LookupId,
            }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetZone()
        //{
        //    var zone = db.Zone.AsQueryable();
        //    return Json(zone.Select(p => new
        //    {
        //        ZoneId = p.ZoneId,
        //        ZoneName = p.ZoneName,
        //    }), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetInstitutionType()
        //{
        //    var institution = db.InstitutionType.AsQueryable();
        //    return Json(institution.Select(p => new
        //    {
        //        InstitutionName = p.InstitutionName,
        //        InstitutionTypeId = p.InstitutionTypeId,
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetInsuranceCategory(string InsuranceTypeId)
        {

            var catagories = db.InsuranceCategory.Where(x => x.InsuranceTypeId.ToString() == InsuranceTypeId);
            return Json(catagories.Select(p => new
            {
                Name = p.Name,
                InsuranceCategoryId = p.InsuranceCategoryId,

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategory()
        {
            List<string> attributes = Enum.GetValues(typeof(PromotionSetting))
                .Cast<PromotionSetting>()
                .Select(v => v.ToString())
                .ToList();

            return Json(attributes.Select(s => new
            {
                Text = s.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories()
        {
            List<string> attributes = Enum.GetValues(typeof(InsuranceAttribute))
                .Cast<InsuranceAttribute>()
                .Select(v => v.ToString())
                .ToList();

            return Json(attributes.Select(s => new { Text = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceType()
        {
            List<InsuranceType> insuranceTypeList = new List<InsuranceType>();
            var types = db.InsuranceType.Where(x => x.InsuranceTypeGroup == InsuranceGroupType.Other).ToList();
            foreach (var type in types)
            {
                InsuranceType insuranceType = new InsuranceType();
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == type.LookupId);
                if (lookup != null)
                {
                    insuranceType.LookupId = lookup.LookupId;
                    insuranceType.Name = lookup.Description;
                    insuranceType.InsuranceTypeId = type.InsuranceTypeId;
                }
                insuranceTypeList.Add(insuranceType);
            }
            return Json(insuranceTypeList.Select(p => new
            {
                Name = p.Name,
                InsuranceTypeId = p.InsuranceTypeId,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMonths(int DetailMeasuresId)
        {
            var months = db.DisciplineMeasuresType.Where(X => X.DetailMeasuresId == DetailMeasuresId).ToList();
            return Json(months.Select(p => new
            {
                ExpireYear = p.ExpireYear
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceTypePPE()
        {
            List<InsuranceType> insuranceTypeList = new List<InsuranceType>();
            var types = db.InsuranceType.Where(x => x.InsuranceTypeGroup == InsuranceGroupType.PPE).ToList();
            foreach (var type in types)
            {
                InsuranceType insuranceType = new InsuranceType();
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == type.LookupId);
                if (lookup != null)
                {
                    insuranceType.LookupId = lookup.LookupId;
                    insuranceType.Name = lookup.Description;
                    insuranceType.InsuranceTypeId = type.InsuranceTypeId;
                }
                insuranceTypeList.Add(insuranceType);
            }
            return Json(insuranceTypeList.Select(p => new
            {
                Name = p.Name,
                InsuranceTypeId = p.InsuranceTypeId,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceGroup(string InsuranceTypeId)
        {
            List<InsuranceGroup> insuranceCategoryList = new List<InsuranceGroup>();
            var insuranceType = db.InsuranceType.FirstOrDefault(x => x.InsuranceTypeId.ToString() == InsuranceTypeId);
            if (insuranceType != null)
            {
                var insuranceCatagories = db.InsuranceCategory.Where(x => x.InsuranceTypeId == insuranceType.InsuranceTypeId).ToList();


                foreach (var insuranceCatagory in insuranceCatagories)
                {
                    InsuranceGroup insuranceGrp = new InsuranceGroup();
                    insuranceGrp.Name = insuranceCatagory.Name;
                    insuranceGrp.InsuranceGroupId = insuranceCatagory.InsuranceCategoryId;


                    insuranceCategoryList.Add(insuranceGrp);
                }
            }


            return Json(insuranceCategoryList.Select(p => new
            {
                Name = p.Name,
                InsuranceGroupId = p.InsuranceGroupId,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBusinessClassList()
        {
            var businessClass = db.Lookups.Where(x => x.LookupType == LookUpTypes.INSURANCE_CLASS_OF_BUSINESS);
            return Json(businessClass.Select(p => new
            {
                Description = p.Description,
                LookupId = p.LookupId,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUsersList()
        {
            var context = new ApplicationDbContext();
            var allUsers = context.Users.ToList();
            return Json(allUsers.Select(p => new
            {
                UserName = p.UserName,
                Email = p.Email,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserEmployeesList()
        {
            var allEmployees = db.Person_Employee.ToList();
            return Json(allEmployees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMeasureDetails(int MeasuresId)
        {
            var measureDetails = db.DisciplineMeasuresDetail.Where(x => x.MeasuresId == MeasuresId);
            return Json(measureDetails.Select(p => new
            {
                DetailMeasuresId = p.DetailMeasuresId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMeasures()
        {
            var measurs = db.DisciplineMeasures;
            return Json(measurs.Select(p => new
            {
                MeasuresId = p.MeasuresId,
                MeasuresType = p.MeasuresType,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEvaluationType()
        {

            var types = db.Lookups.Where(l => l.LookupType == LookUpTypes.EVALUATION_TYPE);
            return Json(types.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranchType()
        {

            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE);
            return Json(types.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranch()
        {

            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            var branches = db.Branch.Where(x=>x.IsActive).Where(x => mybraches.Contains(x.BranchId));
            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranchFromRecruitmentPlan()
        {
            List<Branch> branchList = new List<Branch>();
            //var recruitmentPlan = db.RecruitmentPlans.ToList();
            var branches = db.Branch.Where(x => x.IsActive).ToList();

            branchList.AddRange(branches);

            return Json(branchList.DistinctByField(x => x.BranchId).Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBranchs()
        {
            var branches = db.Branch.Where(x => x.IsActive).ToList();


            return Json(branches.DistinctByField(x => x.BranchId).Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFromBranchs()
        {
           
            var branches = db.Branch.Where(x => x.IsActive).ToList();


            return Json(branches.DistinctByField(x => x.BranchId).Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranches()
        {

            var branches = db.Branch.Where(x => x.IsActive);
            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                AmharicName = p.AmharicName??p.Name,
                Name = p.Name??p.AmharicName,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllWorkUnit()
        {
            List<RecruitmentPlan> planList = new List<RecruitmentPlan>();

            var vacancies = db.VaccancyPlans;
            foreach (var vac in vacancies)
            {
                var plan = db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == vac.RecruitmentPlanId);
                if (plan != null)
                {
                    planList.Add(plan);
                }
            }

            return Json(vacancies.Select(p => new
            {
                WorkUnitId = p.VacancyId,
                RecruitmentPlan = planList.FirstOrDefault(x => x.RecruitmentPlanId == p.RecruitmentPlanId)
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOverTimeEmployees()
        {
            List<Person_Employee> personList = new List<Person_Employee>();
            var overRequests = db.OverTimeRequestEmployee.ToList();
            foreach (var overRequest in overRequests)
            {
                var requestStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == overRequest.OverTimeRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "OverTime");
                if (requestStatus != null)
                {
                    var requests = db.OverTimeRequestEmployee.Where(x => x.OverTimeRequestId == requestStatus.RequestId);
                    foreach (var request in requests)
                    {
                        var person = db.Person_Employee.Where(x=>x.IsActive).FirstOrDefault(x => x.EmployeeId == request.EmployeeId);
                        if (person != null)
                        {
                            personList.Add(person);
                        }
                    }

                }

            }
            return Json(personList.DistinctByField(x => x.EmployeeId).Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBranch()
        {
            var branches = db.Branch.Where(x => x.IsActive).ToList();
            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.AmharicName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFiscalYearForPerformance()
        {
            HashSet<int> fiscalYearIds = new HashSet<int>(db.QuarterLengths
                                                                 .Where(x=>!x.IsClosed)
                                                                 .Select(x => x.GlFiscalYearId));
            var fiscalYear = db.GLFiscalYears
                                    .Where(x => fiscalYearIds.Contains(x.GlFiscalYearId))
                                    .OrderByDescending(x => x.GlFiscalYearId)
                                    .ToList();
            //var fiscalYear = db.GLFiscalYears.OrderByDescending(x=>x.GlFiscalYearId).ToList();

            return Json(fiscalYear.Select(p =>
            new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodOrQuarterList(int GlFiscalYearId)
        {
            List<Quarter> quarterList = new List<Quarter>();
            var quarterLength = db.QuarterLengths
                                        .FirstOrDefault(x => x.GlFiscalYearId == GlFiscalYearId
                                                        && !x.IsClosed);
            if (quarterLength != null)
            {
                var quarter = db.Quarters
                    .Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId
                             && !x.IsClosed).ToList();
                quarterList.AddRange(quarter);
            }

            return Json(quarterList.Select(p => new
            {
                QuarterId = p.QuarterId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllBranchByBusinessUnit(int businessUnitId, string searchText)
      {
            // Load all branches and filter by active status and search text
            var branches = db.Branch.ToList()
                .ToList();
            if (!string.IsNullOrEmpty(searchText))
            {
                branches = branches.Where(x => x.IsActive).ToList().Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(searchText.ToUpper())).ToList();
            }
            // If a specific business unit is selected
            if (businessUnitId != 0)
            {
                var branchIds = db.JobTitleMapping
                    .Where(x => x.IsActive && x.BusinessUnitId == businessUnitId)
                    .Select(x => x.BranchId)
                    .Distinct()
                    .ToList();

                if (branchIds.Any())
                {
                    branches = branches.Where(x => branchIds.Contains(x.BranchId)).ToList();
                }
                else
                {
                    branches = new List<Branch>(); // No branches match the business unit
                }
            }

            // Prepare the result
            var result = branches.Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.AmharicName
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetAllBranchByBusinessUnit(int businessUnitId, string searchText)
        //{
        //    var branches = db.Branch.ToList();
        //     branches = branches.Where(x =>!string.IsNullOrEmpty(x.Name) && x.Name.ToUpper().Contains(searchText.ToUpper())).Where(x => x.IsActive).ToList();
        //    if (businessUnitId != 0)
        //    {
        //        var branchId = db.JobTitleMapping.Where(x => x.IsActive).ToList().Where(x => x.BusinessUnitId == businessUnitId).Select(x=>x.BranchId).Distinct().ToList();
        //        if (branchId.Any())
        //        {
        //            branches = branches.Where(x => x.Name.ToUpper().Contains(searchText.ToUpper())).ToList().Where(x => branchId.Contains(x.BranchId)).ToList();

        //            return Json(branches.Select(p => new
        //            {
        //                BranchId = p.BranchId,
        //                Name = p.AmharicName,
        //            }), JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            branches = new List<Branch>();
        //        }

        //         return Json(branches.Select(p => new
        //        {
        //            BranchId = p.BranchId,
        //            Name = p.AmharicName,
        //        }), JsonRequestBehavior.AllowGet);

        //    }

        //    return Json(branches.Select(p => new
        //    {
        //        BranchId = p.BranchId,
        //        Name = p.AmharicName,
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetSelectedBranchs()
        {

            var branches = db.Branch.Where(x=>x.IsActive).Where(x => x.BranchTypes == BranchTypes.PROJECT);
            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                AmharicName = p.AmharicName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancy()
        {
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.EXTERNAL).ToList();
            return Json(vacancies.Select(p => new
            {
                VaccancyCode = p.VacancyId,
                Name = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancyId()
        {
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.EXTERNAL).ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMediaVacancyCode()
        {
            List<RecruitmentVacancy> recruit = new List<RecruitmentVacancy>();
            var vacancies = db.RecruitmentVacancy.ToList();
            if (vacancies.Any())
            {
                foreach (var recruitmentVacancy in vacancies)
                {
                    var vacany = db.VaccancyPlans.FirstOrDefault(x =>
                        x.VacancyId == recruitmentVacancy.VacancyId && x.IsActive /*&& !x.IsComplete*/ && x.RecruSource == RecruitmentSource.EXTERNAL);
                    if (vacany != null)
                    {
                        recruit.Add(recruitmentVacancy);
                    }
                }
            }
            return Json(recruit.DistinctByField(x => x.MediaVacancyCode).Select(p => new
            {
                VaccancyCode = p.MediaVacancyCode,
                MediaVacancyCode = p.MediaVacancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMediaVacancyCodeForInternal()
        {
            List<RecruitmentVacancy> recruit = new List<RecruitmentVacancy>();
            var vacancies = db.RecruitmentVacancy.ToList();
            if (vacancies.Any())
            {
                foreach (var recruitmentVacancy in vacancies)
                {
                    var vacany = db.VaccancyPlans.FirstOrDefault(x =>
                        x.VacancyId == recruitmentVacancy.VacancyId && x.IsActive /*&& !x.IsComplete*/
                        && (x.RecruSource == RecruitmentSource.INTERNAL
                              || x.RecruSource == RecruitmentSource.INTERNALPROMOTION
                              || x.RecruSource == RecruitmentSource.INTERNALTRANSFER));
                    if (vacany != null)
                    {
                        recruit.Add(recruitmentVacancy);
                    }
                }
            }
            return Json(recruit.DistinctByField(x => x.MediaVacancyCode).Select(p => new
            {
                VaccancyCode = p.MediaVacancyCode,
                MediaVacancyCode = p.MediaVacancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancyAllId()
        {
            var vacancies = db.VaccancyPlans;
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMedia()
        {
            var media = db.Lookups.Where(x => x.LookupType == LookUpTypes.MEDIA);
            return Json(media.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRecruitmentPlan()
        {
            var recruit = db.RecruitmentPlans.DistinctByField(x => x.JobtitleName).Where(x => x.To >= DateTime.Now);
            return Json(recruit.Select(p => new
            {
                RecruitmentPlanId = p.RecruitmentPlanId,
                JobtitleName = p.JobtitleName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleFromRecruitmentPlan(string OrgStructureId)
        {
            //List<Lookup> lookupList=new List<Lookup>();
            int branchId = Convert.ToInt32(OrgStructureId.Split('-')[0]);
            int orgId = Convert.ToInt32(OrgStructureId.Split('-')[1]);
            var recruit = db.RecruitmentPlans.Where(x => x.BranchId == branchId && x.OrgStructureId == orgId).DistinctByField(x => x.LookupId).ToList();//.Where(x => x.To >= DateTime.Now);
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            //if (recruit.Any())
            //{
            //    foreach (var recruitmentPlan in recruit)
            //    {
            //        var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == recruitmentPlan.LookupId && x.LookupType==LookUpTypes.JOB_TITLE);
            //        if (lookup!=null)
            //        {
            //            lookupList.Add(lookup);
            //        }
            //    }

            //}

            var lookupList = from r in recruit
                             join l in lookups
                                 on r.LookupId equals l.LookupId
                             select (new
                             {
                                 r.LookupId,
                                 l.Description

                             });

            return Json(lookupList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleFromPlanToVac(string OrgStructureId)
        {
            List<RecruitmentPlanJob> lookupL = new List<RecruitmentPlanJob>();
            if (OrgStructureId != "undefined-undefined")
            {
                int branchId = Convert.ToInt32(OrgStructureId.Split('-')[0]);
                int orgId = Convert.ToInt32(OrgStructureId.Split('-')[1]);

                var recruit = db.RecruitmentPlans
                             .Where(x => x.BranchId == branchId && x.OrgStructureId == orgId && x.To >= DateTime.Now &&
                              x.NoOfVacantPost > 0).DistinctByField(x => x.LookupId);


                List<RecruitmentPlanJob> planLists = new List<RecruitmentPlanJob>();

                foreach (var plan in recruit)
                {
                    var pl = new RecruitmentPlanJob();

                    pl.RecruitmentPlanId = plan.RecruitmentPlanId;

                    var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == plan.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                    if (lookup != null)
                    {
                        pl.JobTitle = lookup.Description;
                    }

                    planLists.Add(pl);
                }

                return Json(planLists.Select(p => new
                {
                    RecruitmentPlanId = p.RecruitmentPlanId,
                    JobTitle = p.JobTitle,
                }), JsonRequestBehavior.AllowGet);
            }

            return Json(lookupL);
        }

        public JsonResult GetJobTitleFromManPowerToVacancy(string OrgStructureId)
        {
            List<ManPowerPlanningJob> manPowerList = new List<ManPowerPlanningJob>();

            if (OrgStructureId != "undefined-undefined")
            {
                int branchId = Convert.ToInt32(OrgStructureId.Split('-')[0]);
                int orgId = Convert.ToInt32(OrgStructureId.Split('-')[1]);

                var manPowerPlannings = db.JobTitleMapping.Where(x=>x.IsActive).Where(x => x.BranchId == branchId && x.OrgStructureId == orgId).ToList();

                foreach (var plan in manPowerPlannings)
                {
                    var manPower = new ManPowerPlanningJob();

                    var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == plan.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                    if (lookup != null)
                    {
                        manPower.JobTitleName = lookup.Description;
                        manPower.LookupId = lookup.LookupId;
                    }

                    manPowerList.Add(manPower);
                }
                return Json(manPowerList.Select(p => new
                {
                    LookupId = p.LookupId,
                    JobTitleName = p.JobTitleName,
                }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetEmployeAllowance()
        {
            var alloanceList = db.AllowanceDefinations.AsQueryable();

            return Json(alloanceList.Select(p => new
            {
                AllowanceDefinationId = p.AllowanceDefinationId,
                AllowanceName = p.AllowanceName,
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployees([DataSourceRequest] DataSourceRequest request)
        {
            HRMInputs inputs = new HRMInputs();
            db = new ExceedDbContext();
            var employees = inputs.GetEmployees(User.Identity.Name);

            return Json(employees.OrderBy(x => x.EmployeeCode).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployee()
        {
            HRMInputs inputs = new HRMInputs();

            var list = inputs.GetEmployees(User.Identity.Name);

            return Json(list.Select(p => new
            {
                PersonId = p.PersonId,
                FullNameWithOutCode = p.FullNameWithOutCode
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetActiveEmployees(string text)
        {
            var list = db.Person_Employee
                .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                        x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                          x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstName + " " + x.MiddleName + " " + x.LastName).Contains(text) ||
                              (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                    )
                .Where(x => x.IsActive).ToList();

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " " + p.FirstNameEnglish+" "+p.MiddleNameEnglish+" "+p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " " + p.FullNameAmharic
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetActiveEmployeeForClearances()
        {
            var employeeIds = db.Placements.Where(x => x.isCurrent&&x.IsHead).Select(x => x.EmployeeId).Distinct();
            var list = db.Person_Employee.Where(x=>employeeIds.Contains(x.EmployeeId)).Where(x => x.IsActive).ToList();

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " " + p.FirstNameEnglish+" "+p.MiddleNameEnglish+" "+p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " " + p.FullNameAmharic
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetActiveEmployeesForDelegation()
        {
            var list = db.Person_Employee
               
                .Where(x => x.IsActive).ToList();

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameEnglish = p.EmployeeCode + " " + p.FullNameaEnglish,
                FullNameAmharic = p.EmployeeCode + " " + p.FullNameAmharic
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetActiveAndInActiveEmployeesForDelegation()
        {
            var list = db.Person_Employee.ToList();

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameEnglish = p.EmployeeCode + " " + p.FullNameaEnglish,
                FullNameAmharic = p.EmployeeCode + " " + p.FullNameAmharic
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetActiveEmployeesForDepartmentDelegation()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(id);
            subordinates.Add(id);
            var list = db.Person_Employee
                                .Where(x=>subordinates.Contains(x.EmployeeId))
                                .Where(x => x.IsActive).ToList();
            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameEnglish = p.EmployeeCode + " " + p.FullNameaEnglish,
                FullNameAmharic = p.EmployeeCode + " " + p.FullNameAmharic
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeesForPlacement([DataSourceRequest] DataSourceRequest request)
        {
            HRMInputs inputs = new HRMInputs();
            db = new ExceedDbContext();
            var employees = inputs.GetEmployeesForPlacment(User.Identity.Name);

            return Json(employees.OrderBy(x => x.EmployeeCode).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployeesSupervisor([DataSourceRequest] DataSourceRequest request, string OrgStructureId)
        {


            HRMInputs inputs = new HRMInputs();
            db = new ExceedDbContext();
            Placement placement;
            List<Person_Employee> employeesList = new List<Person_Employee>();
            Person_Employee employeeName;
            List<Placement> placementList = new List<Placement>();
            List<MainCompanyStructure> workunitList = new List<MainCompanyStructure>();

            var employees = inputs.GetEmployeesForPlacment(User.Identity.Name);
            //  get all parent work units
            var workunit = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.StructureId.ToString() == OrgStructureId).FirstOrDefault();

            if (workunit != null)
            {

                workunitList.Add(workunit);
                int parent = workunit.Parent;

                while (parent != 0)
                {
                    var immediateParenet = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.OrgStructureId == parent).FirstOrDefault();
                    //add to list
                    if (immediateParenet != null)
                    {

                        workunitList.Add(immediateParenet);
                        parent = immediateParenet.Parent;
                    }

                }
            }

            //get current placemen=true employes in the above work units
            foreach (var place in workunitList)
            {
                employeeName = new Person_Employee();
                placement = new Placement();
                placement = db.Placements.Where(x => x.OrgStructureId == place.OrgStructureId && x.isCurrent).FirstOrDefault();

                if (placement != null)
                {
                    employeeName = db.Person_Employee.Where(x => x.EmployeeId == placement.EmployeeId).FirstOrDefault();
                    if (employeeName != null)
                    {

                        //employeeName.FirstName = employeeName.FirstName + " " + employeeName.MiddleName;
                        employeesList.Add(employeeName);
                    }

                }
                //placementList.Add(placement);
            }

            return Json(employeesList.Select(x => new
            {
                FirstName = x.EmployeeCode + "-" + x.FirstName + " " + x.MiddleName + " " + x.LastName,
                EmployeeId = x.EmployeeId,
                OrgStructureId = OrgStructureId
            }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetEmployeesByReportTo([DataSourceRequest] DataSourceRequest request)
        {
            var employees = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);

            return Json(employees.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployeesAskForOther()
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            // var employees = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            List<Person_Employee> personEmployees = new List<Person_Employee>();
            //var employeesId = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(employeeId).ToList();
            var employeesId = EmployeeHelper.GetSubordinates_FilteredByHead(employeeId).ToList();
            employeesId.Remove(employeeId);
            foreach (var id in employeesId)
            {

                Person_Employee person = new Person_Employee();

                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id && x.IsActive);
                if (emp != null)
                {

                    personEmployees.Add(emp);
                }
            }

            return Json(personEmployees.DistinctByField(x => x.FirstName + " " + x.MiddleName + " " + x.LastName).Select(p => new
            {
                PersonId = p.EmployeeId,
                EmployeeId = p.EmployeeId,
                FullName = p.EmployeeCode+"-"+p.FullNameAmharic //p.FirstName + " " + p.MiddleName + " " + p.LastName
            }), JsonRequestBehavior.AllowGet);



        }
        public JsonResult GetSelectedEmployee()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);

            var selectlist = list.Where(x => x.IsActive);

            return Json(selectlist.Select(p => new
            {
                FullName = p.EmployeeCode + "-" + p.FullName,
                PersonId = p.PersonId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllowance()
        {
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE
                                                ||x.LookupType==LookUpTypes.OLD_JOB_TITLE
                                                ||x.LookupType==LookUpTypes.Jobtitle_External_Experience);
            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBenefit()
        {
            var benefits = db.Lookups.Where(l => l.LookupType == LookUpTypes.OTHER_BENEFIT).ToList();
            return Json(benefits.Select(p => new
            {
                Description = p.Description,
                LookupId = p.LookupId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPayrollPlacmentEmployees()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            return Json(list.Select(p => new
            {
                FullName = p.EmployeeCode + "-" + p.FullName,
                PersonId = p.PersonId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApprovedEmpForOverTime()
        {
            List<Person_Employee> empLists = new List<Person_Employee>();
            var emps = db.OverTimeRequestEmployee.ToList();
            foreach (var emp in emps)
            {
                var employee = db.Person_Employee.Where(x=>x.IsActive).FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                Person_Employee emplist = new Person_Employee();

                emplist.EmployeeCode = employee.EmployeeCode;
                emplist.EmployeeId = employee.EmployeeId;
                emplist.FirstName = employee.FullNameaEnglish;

                empLists.Add(emplist);
            }
            return Json(empLists.Select(p => new
            {
                FullNameaEnglish = p.EmployeeCode + "-" + p.FullNameaEnglish,
                EmployeeId = p.EmployeeId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUOM()
        {
            var uoms = db.Lookups.Where(l => l.LookupType == LookUpTypes.UOM).ToList();
            return Json(uoms.Select(p => new
            {
                Description = p.Description,
                LookupId = p.LookupId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAdditionLookUp()
        {
            var addtions = db.Lookups.Where(l => l.LookupType == LookUpTypes.ADDITION_TYPE).ToList();
            return Json(addtions.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);



        }
        public JsonResult GetComplaintType()
        {
            var lookups = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.COMPLAIN_TYPE);

            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Name = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTrainingPlan()
        {
           var assessmentSummaries = db.NeedAssessmentSummary
                                                    .Where(x => x.IsThirdSendForApproval 
                                                    && x.IsApproved || x.IsRejected).AsQueryable();

            //var structures = db.TrainingPlans.AsQueryable();
            return Json(assessmentSummaries.DistinctByField(x => x.LookupId).Select(p => new
            {
                TrainingPlanId = p.NeedAssessmentSummaryId,
                Name = p.GlFiscalYear.Name + " " + p.Lookup.Description

            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetSelectedDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.IsActive).FirstOrDefault(x => x.Parent == 0);
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                var departmentLevelThree = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.Parent == b.OrgStructureId).ToList();
                                if (departmentLevelThree.Any())
                                {
                                    foreach (var c in departmentLevelThree)
                                    {
                                        var deparmentLeveFour = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.Parent == c.OrgStructureId).AsEnumerable().DistinctByField(x => x.OrgStructureId).ToList();
                                        if (deparmentLeveFour.Any())
                                        {
                                            foreach (var d in deparmentLeveFour)
                                            {
                                                allDepartment.Add(d);
                                            }
                                        }
                                        allDepartment.Add(c);
                                    }

                                }
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            var department = allDepartment;
            return Json(department.DistinctByField(x => x.OrgStructureId).Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDepartmentOfHeads()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();

            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var employeePlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeId && x.IsHead);
            if (employeePlacement != null)
            {
                var workunits = db.MainCompanyStructures.Where(x => x.IsActive).FirstOrDefault(x => x.OrgStructureId == employeePlacement.OrgStructureId);
                if (workunits != null)
                {
                    // var companyStructure = db.MainCompanyStructures.Find(workunits.OrgStructureId);
                    allDepartment.Add(workunits);
                    var heads = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.Parent == workunits.OrgStructureId).AsEnumerable().DistinctByField(x => x.OrgStructureId).ToList();
                    if (heads.Any())
                    {
                        foreach (var lowh in heads)
                        {
                            var companyStructure = new MainCompanyStructure();

                            companyStructure = db.MainCompanyStructures.Where(x => x.IsActive&&x.OrgStructureId == lowh.OrgStructureId).FirstOrDefault();

                            allDepartment.Add(companyStructure);
                        }
                    }

                }

            }

            return Json(allDepartment.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDepartmentOfHeadsFromRecPlan()
        {

            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();

            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var employeePlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeId && x.IsHead);
            if (employeePlacement != null)
            {
                var recruitmentPlan =
                    db.RecruitmentPlans.Where(x => x.OrgStructureId == employeePlacement.OrgStructureId);
                var workunits = db.MainCompanyStructures.Where(x => x.IsActive).FirstOrDefault(x => x.OrgStructureId == employeePlacement.OrgStructureId);
                if (workunits != null)
                {

                    var companyStructure = db.MainCompanyStructures.Where(x => x.IsActive&&x.OrgStructureId==workunits.OrgStructureId).FirstOrDefault();
                    allDepartment.Add(companyStructure);
                    var heads = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.Parent == workunits.OrgStructureId).AsEnumerable().DistinctByField(x => x.OrgStructureId).ToList();
                    if (heads.Any())
                    {
                        foreach (var lowh in heads)
                        {
                            companyStructure = new MainCompanyStructure();
                            companyStructure = db.MainCompanyStructures.Where(x => x.IsActive&&x.OrgStructureId==lowh.OrgStructureId).FirstOrDefault();
                            allDepartment.Add(companyStructure);
                        }
                    }

                }

            }

            return Json(allDepartment.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDepartmentHead()
        {
            List<OrgChartViewModel> structureList = new List<OrgChartViewModel>();
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employeePlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeId && x.IsHead);
            if (employeePlacement != null)
            {
                var workunits = db.MainCompanyStructures.Where(x => x.IsActive).ToList();
                if (workunits.Any())
                {
                    foreach (var workunit in workunits)
                    {
                        var structure = new OrgChartViewModel();
                        structure.relationship = workunit.OrgStructureId;
                        structure.name = workunit.Name;
                        structure.parent = workunit.Parent;

                        structureList.Add(structure);
                    }

                }

                var root = structureList.FirstOrDefault(x => x.parent == employeePlacement.OrgStructureId);

                setChildren(root, structureList);
                // return Json(root, JsonRequestBehavior.AllowGet);
                //structureList.Add(Chil);

            }

            return Json(structureList, JsonRequestBehavior.AllowGet);


        }
        public void setChildren(OrgChartViewModel orgModel, List<OrgChartViewModel> workunitList)
        {
            var childs = workunitList.Where(x => x.parent == orgModel.relationship).ToList();
            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    setChildren(child, workunitList);
                    orgModel.children.Add(child);
                }
            }

        }
        public JsonResult GetCommittee()
        {
            var committees = db.Committees.AsQueryable();
            return Json(committees.Select(p => new
            {
                CommitteeId = p.CommitteeId,
                Name = p.Description
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCostCenter(int BranchId, string text)
        {

            GlCostCenterTypes type;
            List<SelectorVm> costcenterList = new List<SelectorVm>();
            var branch = db.Branch.Where(x => x.IsActive).FirstOrDefault(x => x.BranchId == BranchId);
            if (branch != null)
            {
                if (branch.BranchTypes == BranchTypes.HEAD_OFFICE)
                {
                    type = GlCostCenterTypes.HeadOffice;
                }
                else
                {
                    type = GlCostCenterTypes.Project;
                }
                // AccountHelper accountHelper = new AccountHelper();
                var glCostCenterSeg = db.GlSegmentSetups.Where(x => x.SegmentType == DynamicSegmentTypes.CostCenter).FirstOrDefault();
                if (glCostCenterSeg != null)
                {
                    var glcostcenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == glCostCenterSeg.GLSegmentSetupId).Where(x => x.Enable && !x.Parent).ToList();

                    if (!string.IsNullOrEmpty(text))
                    {
                        glcostcenters = glcostcenters.Where(x => x.Values.Contains(text) || x.Description.Contains(text)).ToList();
                    }

                    foreach (var item in glcostcenters)
                    {
                        var record = new SelectorVm
                        {
                            Code = item.Values,
                            Name = item.Values + "-" + item.Description
                        };
                        costcenterList.Add(record);
                    }
                }

            }

            return Json(costcenterList.Select(p => new
            {
                Values = p.Code,
                Description = p.Name
            }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetVacancyPlan()
        {
            var list = db.VaccancyPlans.ToList();
            return Json(list.Select(p => new
            {
                VaccancyCode = p.VacancyId,
                Name = p.VaccancyCode
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetVacancyGeneratedCode()
        {
            var list = db.RecruitmentVacancy.DistinctByField(x => x.MediaVacancyCode).ToList();
            return Json(list.Select(p => new
            {
                VacancyId = p.VacancyId,
                MediaVacancyCode = p.MediaVacancyCode
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCommitteType()
        {
            var committeTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.COMMITTEE_TYPE);
            var committeType = committeTypes;
            return Json(committeType.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStructures()
        {
            var structures = db.Structure;

            return Json(structures.Select(s => new
            {
                StructureId = s.StructureId,
                Name = s.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCountry()
        {
            var countries = db.Countries.AsQueryable();
            return Json(countries.Select(p => new
            {
                CountryId = p.CountryId,
                Nationality = p.Nationality
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNation()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.NATION && l.IsActive).AsQueryable();

            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPersonTitle()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.PERSON_TITLE && l.IsActive).AsQueryable();

            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetLaboratoryCatagories()
        //{
        //    var catagories = db.LaboratoryCatagories.AsQueryable();
        //    return Json(catagories.Select(p => new
        //    {
        //        LaboratoryCatagoryId = p.LaboratoryCatagoryId,
        //        Catagory = p.Catagory
        //    }), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetLaboratoryTests()
        //{
        //    var tests = db.LaboratoryTests.AsQueryable();
        //    return Json(tests.Select(p => new
        //    {
        //        LaboratoryTestsId = p.LaboratoryTestsId,
        //        Test = p.Test
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetRank()
        {
            var ranks = db.Ranks.AsQueryable();
            return Json(ranks.Select(p => new
            {
                RankId = p.RankId,
                RankName = p.RankName
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRankForAllowance()
        {
            var ranksQuery = db.Ranks.Select(x => new
            {
                RankId = x.RankId,
                RankName = x.RankName
            }).AsQueryable();

           
          
            return Json(ranksQuery.Select(r => new
            {
                RankId = r.RankId,
                RankName = r.RankName
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStep()
        {

            var stepss = db.Steps.AsQueryable();
            ViewData["StepList"] = stepss;
            return Json(stepss.Select(p => new
            {
                StepId = p.StepId,
                StepName = p.StepName
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWorkUnit()
        {
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            var branchCostCenter = db.BranchCostCenter.ToList();

            foreach (var b in branchCostCenter)
            {

                var oneWorkUnit = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.OrgStructureId == b.OrgStructureId).FirstOrDefault();
                if (oneWorkUnit != null)
                {
                    workUnits.Add(oneWorkUnit);
                }

            }

            return Json(workUnits.Select(p => new
            {
                Name = p.Name,
                OrgStructureId = p.OrgStructureId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWorkUnits()
        {
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            var mainCompany = db.MainCompanyStructures.Where(x => x.IsActive).ToList();
            workUnits.AddRange(mainCompany);


            return Json(workUnits.Select(p => new
            {
                Name = p.Name,
                OrgStructureId = p.OrgStructureId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodForOvertime()
        {
            //var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            //var list = from p in periodList
            //           join g in db.GLPeriods
            //               on p.Name equals g.Name
            //           select (new
            //           {
            //               g
            //           });
            //ViewData["PeriodList"] = periodList.Select(x => x.Name).ToList(); //!= null ? list.ToList() : new List<PayrollPeriod>();
            var periodList = db.GLPeriods.OrderByDescending(x=>x.DateTo);
            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWorkUnitFromVacancy(string mediacode)
        {
            List<MainCompanyStructure> workUnitsList = new List<MainCompanyStructure>();

            var vacancyIds = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode == mediacode).Select(x => x.VacancyId).ToList();

            var departments = db.VaccancyPlans.Where(x => vacancyIds.Contains(x.VacancyId) && x.IsActive).Select(x => x.Department).ToList();

            if (departments.Any())
            {
                workUnitsList = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => departments.Contains(x.OrgStructureId)).ToList();
            }

            return Json(workUnitsList.DistinctByField(x => x.OrgStructureId).Select(p => new
            {
                Name = p.Name,
                OrgStructureId = p.OrgStructureId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleFromVacancy(int OrgStructureId, string MediaVacancyCode)
        {
            List<ApplicantRegistrationViewModel> titleList = new List<ApplicantRegistrationViewModel>();
            ApplicantRegistrationViewModel appRegister = new ApplicantRegistrationViewModel();

            var vacancyIds = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode == MediaVacancyCode).Select(x => x.VacancyId).ToList();

            var vacancyJobtitleIds = db.VaccancyPlans.Where(x => vacancyIds.Contains(x.VacancyId) && x.Department == OrgStructureId).Select(x => x.JobTitle).ToList();

            var lookups = db.Lookups.Where(x => vacancyJobtitleIds.Contains(x.LookupId)).ToList();

            //var vacancy = db.VaccancyPlans.Where(x => x.OrgStructureId.ToString() == OrgStructureId && x.IsActive && !x.IsComplete).ToList();
            //foreach (var vacancy1 in vacancy)
            //{
            //    var recruitmentPlan =
            //        db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == vacancy1.RecruitmentPlanId);
            //    if (recruitmentPlan != null)
            //    {
            //        var lookup = db.Lookups.FirstOrDefault(x =>
            //            x.LookupId == recruitmentPlan.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
            //        if (lookup != null)
            //        {
            //            appRegister.LookUpId = lookup.LookupId;
            //            appRegister.VacancyId = vacancy1.VacancyId;
            //            appRegister.Description = lookup.Description;
            //            titleList.Add(appRegister);
            //        }
            //    }
            //}

            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCompany()
        {
            var hospitals = db.Companies.Where(x => x.Category == "2" && x.IsActive);
            return Json(hospitals.Select(p => new
            {
                TradeName = p.TradeName,
                MainCompanyId = p.MainCompanyId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHospitals()
        {
            var hospitals = db.Companies.Where(x => x.Category == "2" && x.IsActive);
            return Json(hospitals.Select(p => new
            {
                TradeName = p.TradeName,
                MainCompanyId = p.MainCompanyId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHealthCompany()
        {
            var hospitals = db.Companies.Where(x => x.Category == "3" && x.IsActive);
            return Json(hospitals.Select(p => new
            {
                TradeName = p.TradeName,
                MainCompanyId = p.MainCompanyId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeLists()
        {
            var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            var employeeList = list.Where(x => x.IsActive);
            return Json(employeeList.Select(p => new
            {
                PersonId = p.PersonId,
                FullName = p.FullName
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Employee_ValueMapper(string value)
        {
            HRMInputs inputs = new HRMInputs();

            var dataItemIndex = -1;
            var data = inputs.GetEmployees(User.Identity.Name);
            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.PersonId.ToString() == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTargetGroup()
        {
            var targetGroups = db.Lookups.Where(x => x.LookupType == LookUpTypes.TARGET_GROUP);
            return Json(targetGroups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProbationMeasurments()
        {
            var targetGroups = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROBATION_MEASUREMENT);
            return Json(targetGroups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeesByUser()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(id);
            //var subordinates1 = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(id);
            var employees = db.Person_Employee.Where(x=>x.IsActive)
                                              .Where(x => subordinates.Contains(x.EmployeeId)||x.EmployeeId==id)
                                              .ToList();

            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + "-" + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeesByUserForPerformanceEvaluation()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(id);
            var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == PerformanceSelfEvaluation.Enable_Self_Evaluation.ToString());
            if (config != null)
            {
                subordinates.Remove(id);
            }
            var employees = db.Person_Employee.Where(x => x.IsActive).Where(x => subordinates.Contains(x.EmployeeId) && x.EmployeeId != id).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + "-" + p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeSForPerformanceEvaluationByBranch(string branchId, string text)
        {
            // Get the employee ID of the current user
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            // Get the list of subordinates for the current user
            var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(id);

            // If a branch ID is provided, filter subordinates by branch
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int branchIdParsed))
            {
                subordinates = EmployeeHelper.GetSubordinates_FilteredByHeadAndBranch(id, branchIdParsed);
            }

            // Check if self-evaluation is enabled in the configuration
            var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == PerformanceSelfEvaluation.Enable_Self_Evaluation.ToString());

            // Remove the current user from the list of subordinates if self-evaluation is enabled
            if (config != null)
            {
                subordinates.Remove(id);
            }

            // Retrieve the list of active employees who are subordinates and not the current user
           
            if (!string.IsNullOrEmpty(text))
            {

               var  employees = db.Person_Employee.Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                       x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                         x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                           x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                   ).Where(x => x.IsActive && subordinates.Contains(x.EmployeeId) && x.EmployeeId != id)
               .Select(p => new
               {
                   EmployeeId = p.EmployeeId,
                   FullNameEnglish = p.EmployeeCode + "-" + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish
               })
               .ToList();
                return Json(employees, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var employees = db.Person_Employee.Where(x => x.IsActive && subordinates.Contains(x.EmployeeId))
               .Select(p => new
               {
                   EmployeeId = p.EmployeeId,
                   FullNameEnglish = p.EmployeeCode + "-" + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish
               })
               .ToList();
                return Json(employees, JsonRequestBehavior.AllowGet);
            }
            // Return the list of employees as JSON
           
        }
        public JsonResult GetOneDepartmentEmployee()
        {
            // Get the employee ID of the current user
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            // Get the list of subordinates for the current user
            var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(id);
            var employees = db.Person_Employee.Where(x => x.IsActive && subordinates.Contains(x.EmployeeId) /*&& x.EmployeeId != id*/)
               .Select(p => new
               {
                   EmployeeId = p.EmployeeId,
                   FullNameEnglish = p.EmployeeCode + "-" + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish
               })
               .ToList();
                return Json(employees, JsonRequestBehavior.AllowGet);
           
            // Return the list of employees as JSON
           
        }

        public JsonResult GetSubjectType()
        {
            IQueryable<SubjectType> subjectTypes = db.SubjectTypes;
            return Json(subjectTypes.Select(p => new
            {
                SubjectTypeId = p.SubjectTypeId,
                SubjectTypeName = p.SubjectTypeName
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPayrollEmployeeList([DataSourceRequest] DataSourceRequest request)
        {
            List<EmployeeSelectorVm> list = new List<EmployeeSelectorVm>();
            db = new ExceedDbContext();
            var employees = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);

            foreach (var emp in employees)
            {

                var record = new EmployeeSelectorVm
                {
                    PersonId = emp.PersonId,
                    FullName = emp.EmployeeCode + "-" + emp.FullName
                };
                list.Add(record);

            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }

        public ActionResult PayrollEmployee_ValueMapper(string value)
        {

            var dataItemIndex = -1;
            var data = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.PersonId.ToString() == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadFile(int filename, string type)
        {
            string referenceType = "";

            db = new ExceedDbContext();
            try
            {
                var attachment = db.Attachments.FirstOrDefault(x => x.AttachmentId == filename);
                if (attachment != null)
                {
                    switch (type)
                    {
                        case "Photo":
                            referenceType = AttachmentType.PHOTO.ToString();
                            break;
                        case "Logo":
                            referenceType = AttachmentType.LOGO.ToString();
                            break;
                        case "Doc":
                            referenceType = AttachmentType.DOCUMENT.ToString();
                            break;
                        case "Cosign":
                            referenceType = AttachmentType.COSIGN.ToString();
                            break;
                        case "Clearance":
                            referenceType = AttachmentType.CLEARANCE.ToString();
                            break;
                        case "Minute":
                            referenceType = AttachmentType.MINUTE.ToString();
                            break;
                        case "Incoming":
                            referenceType = AttachmentType.INCOMING_LETTER.ToString();
                            break;
                        case "Outgoing":
                            referenceType = AttachmentType.OUTGOING_LETTER.ToString();
                            break;
                        case "Letter":
                            referenceType = AttachmentType.LETTER.ToString();
                            break;

                    }
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.Description + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + attachment.Reference + "-" + referenceType + "/" + attachment.Description));

                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(attachment.Reference, attachment.FileType);
                }
            }
            catch (Exception e)
            {


            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public ActionResult DownloadFileFromAttachmentArchive(int filename, string type)
        {
            //string referenceType = "";




            //db = new ExceedDbContext();
            //try
            //{
            //    var attachment = db.AttachmentArchives.FirstOrDefault(x => x.AttachmentArchiveId == filename);
            //    if (attachment != null)
            //    {
            //        switch (type)
            //        {
            //            case "Photo":
            //                referenceType = AttachmentType.PHOTO.ToString();
            //                break;
            //            case "Logo":
            //                referenceType = AttachmentType.LOGO.ToString();
            //                break;
            //            case "Doc":
            //                referenceType = AttachmentType.DOCUMENT.ToString();
            //                break;
            //            case "Cosign":
            //                referenceType = AttachmentType.COSIGN.ToString();
            //                break;
            //            case "Clearance":
            //                referenceType = AttachmentType.CLEARANCE.ToString();
            //                break;
            //            case "Minute":
            //                referenceType = AttachmentType.MINUTE.ToString();
            //                break;
            //            case "Incoming":
            //                referenceType = AttachmentType.INCOMING_LETTER.ToString();
            //                break;
            //            case "Outgoing":
            //                referenceType = AttachmentType.OUTGOING_LETTER.ToString();
            //                break;
            //            case "Letter":
            //                referenceType = AttachmentType.LETTER.ToString();
            //                break;

            //        }
            //        WebClient req = new WebClient();
            //        HttpResponse response = System.Web.HttpContext.Current.Response;
            //        response.Clear();
            //        response.ClearContent();
            //        response.ClearHeaders();
            //        response.Buffer = true;
            //        response.AddHeader("Content-Disposition",
            //            "attachment;filename=\"" + attachment.Description + "\"");
            //        byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + attachment.Reference + "-" + referenceType + "/" + attachment.Description));

            //        response.BinaryWrite(datas);
            //        response.End();
            //        HRMInputs.SelectedFile = 0;
            //        return File(attachment.Reference, attachment.FileType);
            //    }
            //}
            //catch (Exception e)
            //{


            //}
            //HRMInputs.SelectedFile = 0;
            return null;
        }

        public JsonResult GetAllBusinessUnits()
        {
            List<SelectorVm> list = new List<SelectorVm>();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = db.BusinessUnits;
                foreach (var bu in businessUnits)
                {
                    var record = new SelectorVm();
                    record.Name = bu.Name;
                    record.Code = bu.BusinessUnitId.ToString();
                    list.Add(record);
                }
            }
            else
            {
                var record = new SelectorVm();
                record.Name = "No Business Unit";
                record.Code = "0";
                list.Add(record);
            }
            return Json(list.Select(x => new
            {
                Name = x.Name,
                Code = x.Code,
                BusinessUnitId = x.Code,
            }), JsonRequestBehavior.AllowGet);
        }

        //////// Recruitment New ////////////////////
        public JsonResult GetAllVacacy()
        {
            var vacancies = db.VaccancyPlans.ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetVacancyLists()
        {
            var vacancies = db.VaccancyPlans.ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEducationLevelLists()
        {
            var educationLevels = db.Lookups.Where(x =>x.IsActive&& x.LookupType == LookUpTypes.EDUCATION_LEVEL).ToList();
            return Json(educationLevels.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEducationFieldLists()
        {
            var educationFields = db.Lookups.Where(x =>x.IsActive&& x.LookupType == LookUpTypes.EDUCATION_FIELD).ToList();
            return Json(educationFields.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLookupJobtitle()
        {
            var experienceJobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE
                                                || x.LookupType == LookUpTypes.OLD_JOB_TITLE
                                                || x.LookupType == LookUpTypes.Jobtitle_External_Experience).ToList();
            return Json(experienceJobtitles.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        //
        public ActionResult GetLookupJobtitleForInternal()
        {
            // this "Jobtitle External Experience" is for internal applicants that they have external experiences registered in profile
            var experienceJobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE || x.LookupType == LookUpTypes.OLD_JOB_TITLE).ToList();
            return Json(experienceJobtitles.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEducationLevels()
        {
            var educationLevels = db.Lookups.Where(x => x.IsActive&&x.LookupType == LookUpTypes.EDUCATION_LEVEL).ToList();

            return Json(educationLevels.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEducationFields()
        {
            var educationFields = db.Lookups.Where(x =>x.IsActive&& x.LookupType == LookUpTypes.EDUCATION_FIELD).ToList();

            return Json(educationFields.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancyExternal()
        {
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.EXTERNAL && x.IsActive).ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancyInternal()
        {
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.INTERNAL
                                                || x.RecruSource == RecruitmentSource.INTERNALPROMOTION
                                                || x.RecruSource == RecruitmentSource.INTERNALTRANSFER
                                                && x.IsActive).ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVacancyCodeForInternalApplicant()
        {
            var vacancies = db.VaccancyPlans
                  .Where(x => x.RecruSource == RecruitmentSource.INTERNAL
                  ||x.RecruSource == RecruitmentSource.INTERNALPROMOTION
                  ||x.RecruSource == RecruitmentSource.INTERNALTRANSFER
                  && x.IsActive).ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentsByVacancy(int VacancyId)
        {
            List<MainCompanyStructure> mainCompanys = new List<MainCompanyStructure>();

            var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == VacancyId);
            if (vacancy != null)
            {
                var mainCompanyStructure = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.OrgStructureId == vacancy.Department).ToList();

                mainCompanys.AddRange(mainCompanyStructure);
            }
            return Json(mainCompanys.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentsByBranchFromManPowerPlanning(int branchId, int businessUnitId, string searchText)
        {
            var structuresQuery = db.JobTitleMapping
                .Where(x => x.IsActive && x.BranchId == branchId);

            if (businessUnitId != 0)
            {
                structuresQuery = structuresQuery.Where(x => x.BusinessUnitId == businessUnitId);
            }

            var structureList = structuresQuery
                .Select(x => x.OrgStructureId)
                .Distinct()
                .ToList();

            var departmentQuery = db.MainCompanyStructures
                .Where(x => x.IsActive && structureList.Contains(x.OrgStructureId));

            if (!string.IsNullOrEmpty(searchText))
            {
                var searchTextUpper = searchText.ToUpper();
                departmentQuery = departmentQuery.Where(x => x.Name != null 
                                                      && x.Name.ToUpper().Contains(searchTextUpper));
            }

            // Remove duplicates (in case multiple job mappings point to same OrgStructureId)
            var departments = departmentQuery
                .DistinctByField(x => x.OrgStructureId)
                .Select(x => new
                {
                    OrgStructureId = x.OrgStructureId,
                    Name = x.Name
                })
                .ToList();

            return Json(departments, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDepartmentsByBranch(int branchId,  string searchText )
        {
            List<MainCompanyStructure> departments = new List<MainCompanyStructure>();

            var structures = db.JobTitleMapping
                .Where(x => x.IsActive)
                .Where(x => x.BranchId == branchId).ToList();
            if (!string.IsNullOrEmpty(searchText))
            {
                var ids = db.MainCompanyStructures
                    .Where(x => x.IsActive).ToList()
                    .Where(x => !string.IsNullOrEmpty(x.Name) 
                                && x.Name.ToUpper().Contains(searchText.ToUpper()))
                                .Select(x => x.OrgStructureId).ToList();

                structures = structures.Where(x => ids.Contains(x.OrgStructureId)).ToList();

                foreach (var str in structures)
                {
                    var structure = new MainCompanyStructure();
                    var department = db.MainCompanyStructures.Where(x => x.IsActive).FirstOrDefault(x => x.OrgStructureId == str.OrgStructureId);
                    if (department != null)
                    {
                        structure.Name = department.Name;
                        structure.OrgStructureId = department.OrgStructureId;

                        departments.Add(structure);
                    }
                }

            }
            else
            {
                foreach (var str in structures)
                {
                    var structure = new MainCompanyStructure();
                    var department = db.MainCompanyStructures
                        .Where(x => x.IsActive)
                        .FirstOrDefault(x => x.OrgStructureId == str.OrgStructureId);
                    if (department != null)
                    {
                        structure.Name = department.Name;
                        structure.OrgStructureId = department.OrgStructureId;

                        departments.Add(structure);
                    }
                }
            }


            return Json(departments.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentsByBranchForHeads(int branchId, string searchText)
        {
            List<MainCompanyStructure> departments = new List<MainCompanyStructure>();

            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            // Current head placement
            var employeePlacement = db.Placements
                .FirstOrDefault(x => x.isCurrent
                                  && x.EmployeeId == employeeId
                                  && x.IsHead);

            // Get all allowed department ids for the head
            List<int> allowedDepartmentIds = new List<int>();

            if (employeePlacement != null)
            {
                var workunit = db.MainCompanyStructures
                    .FirstOrDefault(x => x.IsActive
                                      && x.OrgStructureId == employeePlacement.OrgStructureId);

                if (workunit != null)
                {
                    // Add current department
                    allowedDepartmentIds.Add(workunit.OrgStructureId);

                    // Add child departments
                    var childDepartments = db.MainCompanyStructures
                        .Where(x => x.IsActive && x.Parent == workunit.OrgStructureId)
                        .Select(x => x.OrgStructureId)
                        .Distinct()
                        .ToList();

                    allowedDepartmentIds.AddRange(childDepartments);
                }
            }

            // Branch structures
            var structures = db.JobTitleMapping
                .Where(x => x.IsActive && x.BranchId == branchId)
                .ToList();

            // Filter only departments accessible by the head
            structures = structures
                .Where(x => allowedDepartmentIds.Contains(x.OrgStructureId))
                .ToList();

            // Search filter
            if (!string.IsNullOrEmpty(searchText))
            {
                var ids = db.MainCompanyStructures
                    .Where(x => x.IsActive
                             && !string.IsNullOrEmpty(x.Name)
                             && x.Name.ToUpper().Contains(searchText.ToUpper()))
                    .Select(x => x.OrgStructureId)
                    .ToList();

                structures = structures
                    .Where(x => ids.Contains(x.OrgStructureId))
                    .ToList();
            }

            // Build department list
            foreach (var str in structures)
            {
                var department = db.MainCompanyStructures
                    .FirstOrDefault(x => x.IsActive
                                      && x.OrgStructureId == str.OrgStructureId);

                if (department != null)
                {
                    departments.Add(new MainCompanyStructure
                    {
                        Name = department.Name,
                        OrgStructureId = department.OrgStructureId
                    });
                }
            }

            return Json(departments
                .DistinctByField(x => x.OrgStructureId)
                .Select(p => new
                {
                    OrgStructureId = p.OrgStructureId,
                    Name = p.Name
                }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentsByBussinessUnitFromManPowerPlanning(int businessUnitId,  string searchText )
        {
            List<MainCompanyStructure> departments = new List<MainCompanyStructure>();

            var structures = db.JobTitleMapping
                                .Where(x => x.IsActive)
                                .DistinctByField(x => x.OrgStructureId)
                                .ToList();
            if(businessUnitId != 0)
            {
                structures = structures
                    .Where(x => x.BusinessUnitId == businessUnitId)
                    .ToList();
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                var ids = db.MainCompanyStructures
                                .Where(x => x.IsActive)
                                .ToList()
                                .Where(x => !string.IsNullOrEmpty(x.Name) 
                                        && x.Name.ToUpper().Contains(searchText.ToUpper()))
                                        .Select(x => x.OrgStructureId).ToList();

                structures = structures.Where(x => ids.Contains(x.OrgStructureId)).ToList();

                foreach (var str in structures)
                {
                    var structure = new MainCompanyStructure();
                    var department = db.MainCompanyStructures.Where(x => x.IsActive).FirstOrDefault(x => x.OrgStructureId == str.OrgStructureId);
                    if (department != null)
                    {
                        structure.Name = department.Name;
                        structure.OrgStructureId = department.OrgStructureId;

                        departments.Add(structure);
                    }
                }

            }
            else
            {
                foreach (var str in structures)
                {
                    var structure = new MainCompanyStructure();
                    var department = db.MainCompanyStructures.Where(x => x.IsActive).FirstOrDefault(x => x.OrgStructureId == str.OrgStructureId);
                    if (department != null)
                    {
                        structure.Name = department.Name;
                        structure.OrgStructureId = department.OrgStructureId;

                        departments.Add(structure);
                    }
                }
            }


            return Json(departments.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobtitlesByVacancy(int VacancyId)
        {
            List<Lookup> jobTitles = new List<Lookup>();

            var vacancy = db.VaccancyPlans
                .FirstOrDefault(x => x.VacancyId == VacancyId);
            if (vacancy != null)
            {
                var jobTitle = db.Lookups.Where(x => x.LookupId == vacancy.JobTitle&&x.IsActive).ToList();

                jobTitles.AddRange(jobTitle);
            }
            return Json(jobTitles.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllDepartments()
        {
            var departments = db.MainCompanyStructures.Where(x => x.IsActive).ToList();

            return Json(departments.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllDepartmentsForEvalutionCriteria()
        {
            var orgIds = db.MeasurementDefinitions.Where(x => x.OrgStuructureId != null || x.OrgStuructureId != 0).Select(x => x.OrgStuructureId);
            var departments = db.MainCompanyStructures
                                            .Where(x=>orgIds.Contains(x.OrgStructureId))
                                            .Where(x => x.IsActive).ToList();

            return Json(departments.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTrainingCriterias()
        {
            var criterias = db.TrainingEvaluationCriterias.ToList();

            return Json(criterias.Select(p => new
            {
                TrainingEvaluationCriteriaId = p.TrainingEvaluationCriteriaId,
                Criteria = p.Criteria
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmployeeByDepartmentId(int? OrgStructureId = null)
        {
            var query = db.Person_Employee.Where(e => e.IsActive);

            // If a department is selected, filter by current placement in that department
            if (OrgStructureId.HasValue && OrgStructureId.Value > 0)
            {
                query = from e in query
                        join p in db.Placements on e.EmployeeId equals p.EmployeeId
                        where p.OrgStructureId == OrgStructureId.Value && p.isCurrent
                        select e;
            }

            var employees = query
                .Select(e => new
                {
                    e.EmployeeId,
                    e.EmployeeCode,
                    e.FirstName,
                    e.MiddleName,
                    e.LastName,
                    e.FirstNameEnglish,
                    e.MiddleNameEnglish,
                    e.LastNameEnglish
                })
                .Distinct()
                .ToList();

            var result = employees
                .OrderBy(p => p.EmployeeCode)
                .Select(p =>
                {
                    string amharicName = string.Join(" ", new[] { p.FirstName, p.MiddleName, p.LastName }
                        .Where(s => !string.IsNullOrWhiteSpace(s)));

                    string englishName = string.Join(" ", new[] { p.FirstNameEnglish, p.MiddleNameEnglish, p.LastNameEnglish }
                        .Where(s => !string.IsNullOrWhiteSpace(s)));

                    string displayName = !string.IsNullOrWhiteSpace(amharicName) ? amharicName : englishName;

                    return new
                    {
                        p.EmployeeId,
                        FullNameAmharic = $"{p.EmployeeCode} - {displayName}"
                    };
                });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeesInSelectedDepartment(int OrgStructureId)
        {
            //employe.GetSubordinates_FilteredByBranchAlso
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var EemployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var subordinates = EmployeeHelper.GetSubordinates_FilteredByHead(EemployeeId);
            var empIds = db.Placements.Where(x => x.OrgStructureId == OrgStructureId && x.isCurrent&& mybraches.Contains(x.BranchId)).Select(x => x.EmployeeId).ToList();

            var employees = db.Person_Employee.Where(x=>x.IsActive).Where(x => empIds.Contains(x.EmployeeId)||subordinates.Contains(x.EmployeeId)).DistinctByField(x=>x.EmployeeId).ToList();

            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameAmharic =p.EmployeeCode+"-"+ p.FullNameAmharic
            }), JsonRequestBehavior.AllowGet);
        }
        //////
        public JsonResult GetEnumWithOutApproveOption()
        {

            var statuses = Enum.GetValues(typeof(RequestStatus))
                           .Cast<RequestStatus>()
                           .Where(x => x != RequestStatus.APPROVE)
                           .Select(v => v)
                           .ToList();

            return Json(statuses.Select(p => new
            {
                Value = Convert.ToInt32(p),
                Display = p.ToString()
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEnumWithOutWaitingForAprovalandApproveOption()
        {

            var statuses = Enum.GetValues(typeof(RequestStatus))
                           .Cast<RequestStatus>()
                           .Where(x => x != RequestStatus.APPROVE && x !=RequestStatus.WAITING_FOR_APPROVAL)
                           .Select(v => v)
                           .ToList();

            return Json(statuses.Select(p => new
            {
                Value = Convert.ToInt32(p),
                Display = p.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetEnumWithOutWaitingForApprovalandDepartmentApproveOption()
        {

            var statuses = Enum.GetValues(typeof(RequestStatus))
                           .Cast<RequestStatus>()
                           .Where(x => x != RequestStatus.WAITING_FOR_APPROVAL && x != RequestStatus.DEPARTMENT_APPROVAL)
                           .Select(v => v)
                           .ToList();

            return Json(statuses.Select(p => new
            {
                Value = Convert.ToInt32(p),
                Display = p.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFiscalYearFromQuarterLength()
        {
            HashSet<int> fiscalYearIds = new HashSet<int>(db.QuarterLengths.Select(x => x.GlFiscalYearId));
            var fiscalYear = db.GLFiscalYears
                                    .Where(x=> fiscalYearIds.Contains(x.GlFiscalYearId))
                                    .OrderByDescending(x => x.GlFiscalYearId)
                                    .ToList();
            
            return Json(fiscalYear.Select(p =>
            new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranchForHRDashBoard()
        {
            var branches = db.Branch.Where(x=> x.IsActive)
                                    .ToList();
            
            return Json(branches.Select(p =>
            new
            {
                Value = p.BranchId,
                Text = p.Name

            }), JsonRequestBehavior.AllowGet);
        }
    }
    public class Jobs
    {
        public string Description { get; set; }
        public int LookupId { get; set; }
    }
    public class RecruitmentPlanJob
    {
        public int RecruitmentPlanId { get; set; }
        public string JobTitle { get; set; }
    }

    public class ManPowerPlanningJob
    {
        public string JobTitleName { get; set; }
        public int LookupId { get; set; }
    }
}