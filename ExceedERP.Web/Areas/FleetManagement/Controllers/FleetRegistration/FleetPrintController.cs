using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;    

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{

    [Authorize]
    public class FleetPrintController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: /FleetManagement/FleetReport/
        public ActionResult SiteVehicleTransfer(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]
        public ActionResult FuelPrint(string from, string to ,string plateno)
        {
            ViewBag.from = from;
            ViewBag.to = to;
            ViewBag.plateno = plateno;
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]
        public ActionResult EquipmentPrint(string Id)
        {
            ViewBag.Id = Id;
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.ResourceRequestor)]

        public ActionResult EquipmentDemand(string Id)
        {
            ViewBag.Id = Id;
            var equipmentDemandApprover = db.VehicleDemandApproval.Where(x => x.VehicleDemandId.ToString() == Id).FirstOrDefault();
            var employeeId = UserInformationHelper.GetEmployeeId(equipmentDemandApprover.CreatedBy);
            ViewBag.EmployeeId = employeeId;
           
            var equipmentDemandChecker = db.VehicleDemand.Where(x => x.VehicleDemandId.ToString() == Id).FirstOrDefault();
            var userName = UserInformationHelper.GetEmployeeUserId(equipmentDemandChecker.EmployeeId);
            ViewBag.RUName = userName;
            var demandCheckerEmployeeId = UserInformationHelper.GetEmployeeId(equipmentDemandChecker.CreatedBy);
            ViewBag.CheckerEmployeeId = demandCheckerEmployeeId;

            return View();
        }
        public ActionResult RentalPaymentRequest(int Id)
        {
            ViewBag.Id = Id;
            var preparedby = db.RentalPaymentRequests.Where(x => x.RentalPaymentRequestId == Id).FirstOrDefault();
            var preparedById = UserInformationHelper.GetEmployeeId(preparedby.DepartmentSendForApprovalBy);
            ViewBag.PreparedById = preparedById;

            var departmentApprovedBy = db.RentalPaymentRequestDepartmentValidations.Where(x => x.RentalPaymentRequestId == Id).FirstOrDefault();
            var departmentApprovalById = UserInformationHelper.GetEmployeeId(departmentApprovedBy.CreatedBy);
            ViewBag.DepartmentApprovalById = departmentApprovalById;

            var rentalPaymentRequestApprover = db.RentalPaymentRequestBudgetValidations.Where(x => x.RentalPaymentRequestId == Id).FirstOrDefault();
            if (rentalPaymentRequestApprover!=null)
            {
                var rentalPaymentRequesterId = UserInformationHelper.GetEmployeeId(rentalPaymentRequestApprover.CreatedBy);
                ViewBag.EmployeeId = rentalPaymentRequesterId;
            }
            else
            {
                ViewBag.EmployeeId = "";
            }
            

            var rentalPaymentRequester = db.RentalPaymentRequests.Where(x => x.RentalPaymentRequestId== Id).FirstOrDefault();
            var rentalPaymentApproverId = UserInformationHelper.GetEmployeeId(rentalPaymentRequester.OnlineApprovedBy);
            ViewBag.CheckerEmployeeId = rentalPaymentApproverId;

            var renatlPaymentFinanceApprover = db.RentalPaymentRequests.Where(x => x.RentalPaymentRequestId == Id).FirstOrDefault();
            var rentalPaymentFinanceApproverId = UserInformationHelper.GetEmployeeId(renatlPaymentFinanceApprover.FinanceApprovedBy);
            ViewBag.FinanceApproverId = rentalPaymentFinanceApproverId;

            var rentalPaymentSendForFinanceBy = db.RentalPaymentRequests.Where(x => x.RentalPaymentRequestId == Id).FirstOrDefault();
            var rentalPaymentSendForFinanceById = UserInformationHelper.GetEmployeeId(rentalPaymentSendForFinanceBy.FinanceSendForApprovalBy);
            ViewBag.SendForFinanceApproverId = rentalPaymentSendForFinanceById;
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.ResourceDistributor)]
        public ActionResult EquipmentDistribution(string Id)
        {

            ViewBag.Id = Id;
            var equipmentApprover = db.VehicleDistributionApproval.Where(x => x.VehicleDistributionId.ToString() == Id).FirstOrDefault();
            var employeeId = UserInformationHelper.GetEmployeeId(equipmentApprover.CreatedBy);
            ViewBag.EmployeeId = employeeId;
            var equipmentChecker = db.VehicleDistribution.Where(x => x.VehicleDistributionID.ToString() == Id).FirstOrDefault();
            var checkerEmployeeId = UserInformationHelper.GetEmployeeId(equipmentChecker.CreatedBy);
            ViewBag.CheckerEmployeeId = checkerEmployeeId;
            return View();
        }
        public ActionResult RentalInspectionPrint(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}