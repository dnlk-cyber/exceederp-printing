using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Common.Controllers
{
	public class IsoDocumentsController : BaseController
	{
		private readonly ExceedDbContext db = new ExceedDbContext();
		public ActionResult Index()
		{
			return View();
		}
		public ActionResult Data_Read([DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<IsoDocument> data = db.IsoDocuments;
			DataSourceResult result = data.ToDataSourceResult(request, isoDocument => new
			{
				IsoDocumentId = isoDocument.IsoDocumentId,
				DocumentType = isoDocument.DocumentType,
				Module = isoDocument.Module,
				FormNumber = isoDocument.FormNumber,
				URL = isoDocument.URL,
				FormTitle = isoDocument.FormTitle,
				Remark = isoDocument.Remark,
				DateCreated = isoDocument.DateCreated,
				LastModified = isoDocument.LastModified,
				CreatedBy = isoDocument.CreatedBy,
				ModifiedBy = isoDocument.ModifiedBy
			});

			return Json(result);
		}
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Data_Create([DataSourceRequest]DataSourceRequest request, IsoDocument isoDocument, int id, string addressReferenceType)
		{
			if (ModelState.IsValid)
			{

				var entity = new IsoDocument
				{
					IsoDocumentId = isoDocument.IsoDocumentId,
					DocumentType = isoDocument.DocumentType,
					Module = isoDocument.Module,
					FormNumber = isoDocument.FormNumber,
					URL = isoDocument.URL,
					FormTitle = isoDocument.FormTitle,
					Remark = isoDocument.Remark,
					DateCreated = DateTime.Now,
					LastModified = isoDocument.LastModified,
					CreatedBy = User.Identity.Name,
					ModifiedBy = isoDocument.ModifiedBy
				};

				db.IsoDocuments.Add(entity);
				db.SaveChanges();
				isoDocument.IsoDocumentId = entity.IsoDocumentId;

			}

			return Json(new[] { isoDocument }.ToDataSourceResult(request, ModelState));
		}
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Data_Update([DataSourceRequest]DataSourceRequest request, IsoDocument isoDocument)
		{
			if (ModelState.IsValid)
			{
				var entity = new IsoDocument
				{
					IsoDocumentId = isoDocument.IsoDocumentId,
					DocumentType = isoDocument.DocumentType,
					Module = isoDocument.Module,
					FormNumber = isoDocument.FormNumber,
					URL = isoDocument.URL,
					FormTitle = isoDocument.FormTitle,
					Remark = isoDocument.Remark,
					DateCreated = isoDocument.DateCreated,
					LastModified = DateTime.Now,
					CreatedBy = isoDocument.CreatedBy,
					ModifiedBy = User.Identity.Name
				};

				db.IsoDocuments.Attach(entity);
				db.Entry(entity).State = EntityState.Modified;
				db.SaveChanges();
			}

			return Json(new[] { isoDocument }.ToDataSourceResult(request, ModelState));
		}
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Data_Destroy([DataSourceRequest]DataSourceRequest request, IsoDocument isoDocument)
		{
			if (ModelState.IsValid)
			{
				var entity = new IsoDocument
				{
					IsoDocumentId = isoDocument.IsoDocumentId,
					DocumentType = isoDocument.DocumentType,
					Module = isoDocument.Module,
					FormNumber = isoDocument.FormNumber,
					URL = isoDocument.URL,
					FormTitle = isoDocument.FormTitle,
					Remark = isoDocument.Remark,
					DateCreated = isoDocument.DateCreated,
					LastModified = isoDocument.LastModified,
					CreatedBy = isoDocument.CreatedBy,
					ModifiedBy = isoDocument.ModifiedBy
				};

				db.IsoDocuments.Attach(entity);
				db.IsoDocuments.Remove(entity);
				db.SaveChanges();
			}

			return Json(new[] { isoDocument }.ToDataSourceResult(request, ModelState));
		}

		public ActionResult UpdateOrGenerateDocument()
		{
			List<IsoDocument> documents = new List<IsoDocument>();
			
			var consDocs = new List<IsoDocument>
			{
				
				#region HRM reports
				#region EmployeePlacement
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Placement_Report.ToString(),
					URL = "/HRM/EmployeeReport/EmployeePlacement"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Summary_by_Age.ToString(),
					URL = "/HRM/EmployeeReport/Age"
				},
				#endregion
				#region Employee Performance Total Summary
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Performance_Total_Summary_Report.ToString(),
					URL = "/HRM/EmployeePerformance/EmployeePerformanceTotalSummary"
				},
				#endregion
				#region Employee Performance
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employment_Type.ToString(),
					URL = "/HRM/EmployeeReport/EmploymentType",
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Performance_Summary.ToString(),
					URL = "/HRM/EmployeePerformance/PerformanceSummary",
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Performance.ToString(),
					URL = "/HRM/PerformanceReport/EmployeePerformance"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Performance_Department.ToString(),
					URL = "/HRM/PerformanceReport/EmployeePerformanceDepartment"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Yearly_or_Half_yearly_or_Quarterly_Performance.ToString(),
					URL = "/HRM/PerformanceReport/EmployeeYearlyPerformance"
				},

				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Bsc_Performance.ToString(),
					URL = "/HRM/PerformanceReport/EmployeeBscPerformance",
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Probation_Measure.ToString(),
					URL = "/HRM/PerformanceReport/ProbationMeasure"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.My_Performance.ToString(),
					URL = "/HRM/PerformanceReport/MyPerformance"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Education.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeEducation"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Attendance_Report.ToString(),
					URL = "/HRM/EmployeeReport/AttendanceReport"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Salary.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeSalary"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Birth_Date.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeBirthDate"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Long_Term_Education.ToString(),
					URL = "/HRM/EmployeeReport/LongTermEducation"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Summary.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeSummary"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Termination_Summary.ToString(),
					URL = "/HRM/EmployeeReport/TerminationSummary"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Termination_Reason_Summary.ToString(),
					URL = "/HRM/EmployeeReport/TerminationReasonSummary"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Job_Title.ToString(),
					URL = "/HRM/EmployeeReport/JobTitle"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Discipline_Measure_Summary.ToString(),
					URL = "/HRM/EmployeeReport/DisciplineMeasureSummary"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Retirement_Notification.ToString(),
					URL = "/HRM/EmployeeReport/RetirementNotification"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Disciplinary_Measure.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeDisciplinaryMeasure"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Job_Title.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeJobTitle"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Pension.ToString(),
					URL = "/HRM/EmployeeReport/EmployeePension"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Educational_Level.ToString(),
					URL = "/HRM/EmployeeReport/EducationalLevel"
				}, new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Educational_Level.ToString(),
					URL = "/HRM/EmployeeReport/EducationalLevel"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Educational_Field.ToString(),
					URL = "/HRM/EmployeeReport/EducationalField"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Summary_By_Nation.ToString(),
					URL = "/HRM/EmployeeReport/Nation"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Summary_By_Rank.ToString(),
					URL = "/HRM/EmployeeReport/Rank"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Termination.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeTermination"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Training.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeTraining"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Experience.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeExperience"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_By_Branch.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeByBranch"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Vacant_Postion.ToString(),
					URL = "/HRM/EmployeeReport/VacantPostion"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Judge_By_Region.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeJudgeByRegion"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Promotion_PerYear.ToString(),
					URL = "/HRM/EmployeeReport/EmployeePromotionPerYear"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Work_and_Cost_Summary.ToString(),
					URL = "/HRM/EmployeeReport/WorkandCostSummary"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_to_be_Promoted.ToString(),
					URL = "/HRM/EmployeeReport/EmployeetobePromoted"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Health_and_Value_Payment.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeHealthPayment"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Health_and_Value_Payment_selfCare.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeHealthPaymentselfCare"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Education_and_Training_One_By_One.ToString(),
					URL = "/HRM/EmployeeReport/EducationAndTrainingOneByOne"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Training_One_By_One.ToString(),
					URL = "/HRM/EmployeeReport/TrainingOneByOne"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Training_Request_Report.ToString(),
					URL = "/HRM/EmployeeReport/TrainingRequestReport"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Not_Placed_Employees.ToString(),
					URL = "/HRM/EmployeeReport/NotPlacedEmployees"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Company_Structure.ToString(),
					URL = "/HRM/EmployeeReport/CompanyStructure"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Monthly_Salary_Per_Period.ToString(),
					URL = "/HRM/EmployeeReport/MonthlySalaryPerPeriod"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Employee_Profile_Report.ToString(),
					URL = "/HRM/EmployeeReport/EmployeeProfileReport"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Day_Absent_And_Late_Coming_Report.ToString(),
					URL = "/HRM/EmployeeReport/DayAbsentAndLateComingReport"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Attendance_Letter.ToString(),
					URL = "/HRM/PrintableLetter/AttendanceLetter"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Training_Evaluation.ToString(),
					URL = "/HRM/PrintableLetter/TrainingEvaluation"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Change_Of_Payroll.ToString(),
					URL = "/HRM/PrintableLetter/ChangeOfPayroll"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Payable_Leave.ToString(),
					URL = "/HRM/PrintableLetter/PayableLeave"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Leave_Plan.ToString(),
					URL = "/HRM/PrintableLetter/LeavePlan"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Internal_Vacancy_For_SelfCare.ToString(),
					URL = "/HRM/PrintableLetter/InternalVacancyForSelfCare"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Internal_Applicant.ToString(),
					URL = "/HRM/PrintableLetter/InternalApplicant"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.External_Applicant.ToString(),
					URL = "/HRM/PrintableLetter/ExternalApplicant"
				},
				new IsoDocument
				{
					Module = Modules.HRM,
					DocumentType = HRMModuleReportType.Vacancy_For_Media.ToString(),
					URL = "/HRM/PrintableLetter/VacancyForMedia"
				},
				#endregion
				#endregion
				
			  
		
		  #region procurement
 
 
			  new IsoDocument
			  {
						  Module = Modules.Procurement,
						  DocumentType = ProcurementModuleReportType.Service_Requestion.ToString(),
						  URL = "/Procurement/ProcurementPrint/ServiceRequisitionPrint"


			  },
			  new IsoDocument
			  {
						   Module = Modules.Procurement,
						  DocumentType = ProcurementModuleReportType.Purchase_Order.ToString(),
						  URL = "/Procurement/ProcurementPrint/PurchaseOrderPrint"


			  },
			  new IsoDocument
			  {
						   Module = Modules.Procurement,
						  DocumentType = ProcurementModuleReportType.Purchase_Requisition.ToString(),
						  URL = "/Procurement/ProcurementPrint/PurchaseRequistionReviewPrint"


			  },
			   new IsoDocument
			  {
						   Module = Modules.Procurement,
						  DocumentType = ProcurementModuleReportType.Purchase_Payment_Request.ToString(),
						  URL = "/Procurement/ProcurementPrint/PurchasePaymentPrint"


			  },
				new IsoDocument
			  {
						   Module = Modules.Procurement,
						  DocumentType = ProcurementModuleReportType.Proforma_Result_Summary.ToString(),
						  URL = "/Procurement/ProcurementPrint/ProformaResultEn"


			  },
				new IsoDocument
			  {
						   Module = Modules.Procurement,
						  DocumentType = ProcurementModuleReportType.Award.ToString(),
						  URL = "/Procurement/ProcurementPrint/Award"


			  },
			
		   
			
		  #endregion
			};
			documents.AddRange(consDocs);
			

			
			foreach(var document in documents)
			{
				IsoDocumentHelper.AddUpdateDocumentInfo(document);
			}
			return Json(new
			{
				Message = "Operation completed successfully",
				Status = "OK"
			});
		}
		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}
