//using ExceedERP.Core.Domain.Common.HRM;
//using ExceedERP.DataAccess.Context;
//using ExceedERP.Web.Helpers;
//using ExceedERP.Web.Models;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetResources
//{
//    public class NextApproverDataProviderController : Controller
//    {
//        // GET: FleetManagement/NextApproverDataProvider
//        ExceedDbContext db = new ExceedDbContext();
//        public ActionResult Index()
//        {
//            return View();
//        }

//        public JsonResult GetEmployeeFullName(string id)
//        {
//            // Find the current placement of the employee with the given ID
//            string userId = UserInformationHelper.GetUserCreatedBy(id);
//            var placement = db.Placements.Where(x => x.EmployeeId.ToString() == userId && x.isCurrent).FirstOrDefault();
//            // Initialize the employee variable
//            Person_Employee employee = null;
//            // If a current placement is found and the HeadId is not null, find the head employee
//            if (placement != null && placement.HeadId != null)
//            {
//                employee = db.Person_Employee.Find((int)placement.HeadId);
//            }
//            // Format the data
//            var formattedData = new
//            {
//                FullName = employee != null ? employee.EmployeeCode + "-" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish : "Approver not found",
//                Status = employee != null ? "OK" : "NOT_OK"
//            };
//            // Return the formatted data as JSON
//            return Json(formattedData, JsonRequestBehavior.AllowGet);
//        }

      
//        public JsonResult GetNextApproverList(string id)
//        {
//            // Find the HeadId of the given employee
//            string userId = UserInformationHelper.GetUserCreatedBy(id);
//            var nextApproveId = db.Placements.FirstOrDefault(x => x.EmployeeId.ToString() == userId)?.HeadId;
           
//            //var placement = db.Placements.Where(x => x.EmployeeId.ToString() == userId && x.isCurrent).FirstOrDefault();
//            // Get a list of current head employees excluding the given employee and the current approver
//            var employeeIds = db.Placements
//                .Where(x => x.EmployeeId.ToString() != userId
//                             && x.isCurrent
//                             && x.IsHead
//                             && x.EmployeeId != nextApproveId)
//                .Select(x => x.EmployeeId);
//            // Retrieve the list of employees based on the filtered IDs
//            var employees = db.Person_Employee
//                .Where(x => employeeIds.Contains(x.EmployeeId))
//                .ToList();
//            // Format the data
//            var result = employees.Select(p => new
//            {
//                EmployeeId = p.EmployeeId,
//                FullName = p.EmployeeCode + "- " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish
//            });
//            // Return the formatted data as JSON
//            return Json(result, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult UpdateNextApprover(Guid currentEmployeeId, string nextApproverEmployeeId)
//        {
//            // var getuserId = db.RequestedEquipments.Find(nextApproverEmployeeId);      
//            var userdetail = new ApplicationDbContext();
//            var userempId = userdetail.Users.FirstOrDefault(x => x.EmployeeId == nextApproverEmployeeId);
//            string UserId = userempId.EmployeeId;
//            var requestedequips = db.RequestedEquipments
//                .SingleOrDefault(x => x.RequestedEquipmentId == currentEmployeeId && !x.IsAssigned)
//                ;
//            if (requestedequips!=null)
//            {
//                // Update the SecondLevelApproverId for each performance

//                requestedequips.ForwardedTo = nextApproverEmployeeId;
//                    db.Entry(requestedequips).State = EntityState.Modified;
                
//                // Save all changes to the database
//                db.SaveChanges();
//                var formattedData = new
//                {
//                    Message = "Update successful",
//                    Status = "OK"
//                };
//                return Json(formattedData, JsonRequestBehavior.AllowGet);
//            }
           
//            else
//            {
//                var formattedData = new
//                {
//                    Message = "No records found",
//                    Status = "NOT_OK"
//                };
//                return Json(formattedData, JsonRequestBehavior.AllowGet);
//            }
//        }
//    }
//}