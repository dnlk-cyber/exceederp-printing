
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
    public class HolidayGiftEmployeesController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            ViewData["Employees"] = db.Person_Employee
                .Select(e => new { e.EmployeeId, FullNameaEnglish=e.FirstNameEnglish+" "+e.MiddleNameEnglish+" "+e.LastNameEnglish })
                .ToList();

            ViewData["JobTitles"] = db.Lookups
                .Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE)
                .Select(x => new { x.LookupId, x.Description })
                .ToList();
        }

        // ========================= READ =========================
        public ActionResult HolidayGiftEmployees_Read([DataSourceRequest] DataSourceRequest request)
        {
            var data = db.HolidayGiftEmployees
                .Include(x => x.Employee)
                .ToList();

            var result = data.ToDataSourceResult(request, x => new
            {
                x.HolidayGiftEmployeeId,
                x.EmployeeId,
                x.PaymentRequestId,
                x.EmployeeCode,
                x.BasicSalary,
                x.EthiopianHoliday,
                x.HolidayGiftAmount,
                x.PreparedBy,
                x.CheckedBy,
                x.IsActive,
                x.DepartmentOnlineApprovedBy,
                x.Status,
                x.DepartmentSendForApprovalBy,
                x.DepartmentSendForApprovalTime,
                x.IsDepartmentSendForApproval,
                x.DepartmentOnlineApprovedTime,
                x.IsDepartmentOnlineApproved,
                x.OnlineApprovedBy,
                x.OnlineApprovedTime,
                x.IsOnlineApproved,
                x.IsSendForApproval,
                x.SendForApprovalBy,
                x.SendForApprovalTime,
                x.IsVoid,
                x.VoidBy,
                x.CreatedBy,
                x.LastModified,
                x.DateCreated,
                x.VoidTime
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // ========================= CREATE =========================
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HolidayGiftEmployees_Create([DataSourceRequest] DataSourceRequest request, HolidayGiftEmployee model)
        {
            // Fetch current placement for the employee
            var placement = db.Placements.FirstOrDefault(p => p.EmployeeId == model.EmployeeId && p.isCurrent);

            if (placement == null)
            {
                ModelState.AddModelError("", "Employee does not have an active placement.");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            // Prevent duplicates for active gifts
            bool exists = db.HolidayGiftEmployees.Any(x => x.EmployeeId == model.EmployeeId && x.EthiopianHoliday==model.EthiopianHoliday&& x.IsActive);
            if (exists)
            {
                ModelState.AddModelError("", "Holiday gift already exists for this employee.");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            // Fetch employee
            var employee = db.Person_Employee.FirstOrDefault(e => e.EmployeeId == model.EmployeeId);
            if (employee == null)
            {
                ModelState.AddModelError("", "Employee not found.");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            // === Religion validation based on holiday ===
            switch (model.EthiopianHoliday)
            {
                case EthiopianHoliday.Christmas:
                case EthiopianHoliday.Easter:
                case EthiopianHoliday.Meskel:
                case EthiopianHoliday.Epiphany:
                case EthiopianHoliday.GoodFriday:
                    if (employee.Religion == ReligionList.MUSLIM)
                    {
                        ModelState.AddModelError("", "Muslim employees cannot receive gifts for this holiday.");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }
                    break;

                case EthiopianHoliday.EidAlFitr:
                case EthiopianHoliday.EidAlAdha:
                case EthiopianHoliday.Mawlid:
                    if (employee.Religion != ReligionList.MUSLIM)
                    {
                        ModelState.AddModelError("", "Only Muslim employees can receive gifts for this holiday.");
                        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                    }
                    break;

                default:
                    // No restriction for other holidays
                    break;
            }

            // Assign branch/org/business info from placement
            model.BranchId = placement.BranchId;
            model.OrgStructureId = placement.OrgStructureId;
            model.BusinessUnitId = placement.BusinessUnitId;
            model.LookupId = placement.LookupId;
            model.BasicSalary = placement.Salary;

            // Assign basic salary from placement
            model.BasicSalary = placement.Salary;

            // Assign HolidayGiftRate based on salary
            var giftRate = db.HolidayGiftRateSettings
                .FirstOrDefault(r => model.BasicSalary >= r.MinGrossSalary && model.BasicSalary <= r.MaxGrossSalary);
            if (giftRate != null)
            {
                model.HolidayGiftAmount = giftRate.GiftAmount;
                model.HolidayGiftRateSettingId = giftRate.HolidayGiftRateSettingId;
            }
            else
            {
                model.HolidayGiftAmount = 0;
                model.HolidayGiftRateSettingId = 0;
            }

            if (ModelState.IsValid)
            {
               
                

                // Set auditing fields
                model.PreparedBy = User.Identity.Name;
                model.PreparedTime = DateTime.Now;
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.IsActive = true;

                db.HolidayGiftEmployees.Add(model);
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "Holiday Gift", "Success",
                    User.Identity.Name, $"Created Holiday Gift for Employee {model.EmployeeCode}", Modules.HRM);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // ========================= UPDATE =========================
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HolidayGiftEmployees_Update([DataSourceRequest] DataSourceRequest request, HolidayGiftEmployee model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.HolidayGiftEmployees.Find(model.HolidayGiftEmployeeId);

                if (entity != null)
                {
                    entity.EmployeeId = model.EmployeeId;
                    entity.BasicSalary = model.BasicSalary;
                    entity.HolidayGiftAmount = model.HolidayGiftAmount;

                    entity.CheckedBy = User.Identity.Name;
                    entity.CheckedTime = DateTime.Now;

                    db.SaveChanges();

                    UserHelper.OperationLog(Request.UserHostAddress, "Holiday Gift", "Success",
                        User.Identity.Name, "Updated Holiday Gift", Modules.HRM);
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // ========================= DELETE =========================
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HolidayGiftEmployees_Destroy([DataSourceRequest] DataSourceRequest request, HolidayGiftEmployee model)
        {
            var entity = db.HolidayGiftEmployees.Find(model.HolidayGiftEmployeeId);

            if (entity != null)
            {
                db.HolidayGiftEmployees.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success",
                    User.Identity.Name, "Deleted Holiday Gift " + model.HolidayGiftEmployeeId);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // ========================= GENERATE FROM PLACEMENT =========================
        [HttpPost]
        public ActionResult GenerateHolidayGift(string empId, string branchId, string Holiday)
        {
            var query = db.Placements.Where(x => x.isCurrent);

            if (int.TryParse(branchId, out int branId))
                query = query.Where(x => x.BranchId == branId);

            if (int.TryParse(empId, out int employeeId))
                query = query.Where(x => x.EmployeeId == employeeId);

            // Base employee query
            var employees = db.Person_Employee.Where(e => e.IsActive);

            // Filter by Religion based on Holiday
            if (!string.IsNullOrEmpty(Holiday))
            {
                switch (Holiday)
                {
                    case "Christmas":
                    case "Easter":
                    case "Meskel":
                    case "Timket":
                    case "Epiphany":
                    case "GoodFriday":
                        employees = employees.Where(e => e.Religion != ReligionList.MUSLIM);
                        break;

                    case "EidAlFitr":
                    case "EidAlAdha":
                    case "Mawlid":
                        employees = employees.Where(e => e.Religion == ReligionList.MUSLIM);
                        break;

                    default:
                        // No filter or include all
                        break;
                }
            }
            //gifts
            var holidayGiftRates = db.HolidayGiftRateSettings.ToList();
            // Convert to dictionary for performance 🚀
            var employeeDict = employees.ToDictionary(e => e.EmployeeId);

            EthiopianHoliday holidayEnum;

            Enum.TryParse(Holiday, out holidayEnum);
           
            foreach (var item in query.ToList())
            {
                if (!employeeDict.TryGetValue(item.EmployeeId, out var employee))
                    continue;

                var gift = holidayGiftRates.FirstOrDefault(r =>
                  item.Salary >= r.MinGrossSalary &&
                  item.Salary <= r.MaxGrossSalary);

                if (gift == null) continue;

                // avoid duplicates
                bool exists = db.HolidayGiftEmployees.Any(x =>
                    x.EmployeeId == employee.EmployeeId && x.IsActive);

                if (exists) continue;

                var entity = new HolidayGiftEmployee
                {
                    EmployeeId = employee.EmployeeId,
                    EmployeeCode = employee.EmployeeCode,
                    BranchId = item.BranchId,
                    OrgStructureId = item.OrgStructureId,
                    BusinessUnitId = item.BusinessUnitId,
                    LookupId = item.LookupId,
                    EthiopianHoliday = holidayEnum,
                    BasicSalary = item.Salary,
                    HolidayGiftAmount = gift.GiftAmount, // TODO: calculate from rate
                    HolidayGiftRateSettingId = gift.HolidayGiftRateSettingId ,
                    CreatedBy = User.Identity.Name,
                    PreparedBy = User.Identity.Name,
                    PreparedTime = DateTime.Now,
                    DateCreated = DateTime.Now,
                    IsActive = true
                };

                db.HolidayGiftEmployees.Add(entity);
            }

            db.SaveChanges();

            return Json(new { success = true, message = "Holiday Gift Generated Successfully" });
        }


        [HttpPost]
        public ActionResult CheckHolidayGifts(int id)
        {
            try
            {
                var record = db.HolidayGiftEmployees.FirstOrDefault(x => x.HolidayGiftEmployeeId == id);

                if (record == null)
                {
                    return Json(new { Status = false, Message = "Record not found." });
                }

                // Prevent double check
                if (!string.IsNullOrEmpty(record.CheckedBy))
                {
                    return Json(new { Status = false, Message = "Already checked." });
                }

                record.CheckedById = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                record.CheckedBy = User.Identity.Name;
                record.CheckedTime = DateTime.Now;

                // Optional: status tracking
                record.Status = "Checked";

                db.SaveChanges();

                return Json(new { Status = true, Message = "Successfully checked." });
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ApproveAndsendToPaymentRequest(int id)
        {
            try
            {
                var record = db.HolidayGiftEmployees
                               .FirstOrDefault(x => x.HolidayGiftEmployeeId == id);

                if (record == null)
                    return Json(new { Status = false, Message = "Record not found." });

                //  Must be checked first
                if (string.IsNullOrEmpty(record.CheckedBy))
                    return Json(new { Status = false, Message = "Please check the record first." });

                //  Prevent duplicate approval
                if (!string.IsNullOrEmpty(record.OnlineApprovedBy)|| record.IsOnlineApproved)
                    return Json(new { Status = false, Message = "Already approved." });

                // ===CREATE PAYMENT REQUEST ===
                var requestEntity = new PaymentRequest
                {
                    SupplierId = "0",
                    IsNewSupplier = false,
                    Supplier = " ",
                    Purchaser = 0,
                    ReasonOfPayment = $"Holiday Gift - {record.EthiopianHoliday}",
                    RequestDate = DateTime.Now.Date,
                    RequestDateAm = HrHelper.GetECDate(DateTime.Now),
                    InvoiceNo = "0",
                    PurchaseRequestNo = "0",
                    PONo = "0",
                    GRN = "0",
                    Source = PaymentRequestSource.HolidayGift.ToString(),
                    SourceId = record.HolidayGiftEmployeeId.ToString(),
                    BranchId = record.BranchId??0,
                    BusinessUnitId = record?.BusinessUnitId??0,
                    OrgStuructureId = record?.OrgStructureId??0,
                    EmployeeId = record.EmployeeId,
                    Paid = false,
                    FromBranchId = record.BranchId??0,
                    FromBusinessUnitId = record.BusinessUnitId??0,
                    CreatedAtBranchId = record.BranchId??0,
                    CreatedAtBusinessUnitId = record.BusinessUnitId??0,
                    CreatedAtOrgStructureId = record.OrgStructureId??0,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    IsDepartmentOnlineApproved = true,
                    DepartmentOnlineApprovedBy = User.Identity.Name,
                    DepartmentOnlineApprovedTime = DateTime.Now
                };

                db.PaymentRequests.Add(requestEntity);
                db.SaveChanges();

                // ================= PAYMENT REQUEST LINE =================
                //var glTaxRate = db.ExpenseAndTaxSettings.FirstOrDefault();
                //var glTax = db.GLTaxRates.FirstOrDefault();

                var requestLine = new PaymentRequestLine
                {
                    PaymentRequestId = requestEntity.PaymentRequestId,
                    Amount = record.HolidayGiftAmount,
                    TaxAmount = 0,
                    TotalAmount = record.HolidayGiftAmount,

                    ItemName = "Leave Payable",
                    GLTaxRateID = null,

                    Description = $"Holiday Gift - {record.EthiopianHoliday}",
                    Remark = "Holiday Gift Payment",

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now
                };

                db.PaymentRequestLines.Add(requestLine);
                db.SaveChanges();

                // === UPDATE HOLIDAY GIFT ===
                record.IsOnlineApproved = true;
                record.OnlineApprovedBy = User.Identity.Name;
                record.OnlineApprovedTime = DateTime.Now;

                // (Optional but VERY IMPORTANT)
                // Add this property in your model:
                // public int? PaymentRequestId { get; set; }
                record.PaymentRequestId = requestEntity.PaymentRequestId;

                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new
                {
                    Status = true,
                    Message = "Approved and sent to payment request successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = false,
                    Message = ex.Message
                });
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
