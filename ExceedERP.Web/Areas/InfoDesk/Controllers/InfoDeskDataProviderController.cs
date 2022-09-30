using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using ExceedERP.DataAccess.Context;
using System.Data.Entity.Core.Objects;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.InfoDesk;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
  
    [AllowAnonymous]
    public class InfoDeskDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext UserDb = new ApplicationDbContext();
        public static string EmployeeId
        {
            get;
            set;
        }
        public static decimal CalculatedCost
        {
            get;
            set;
        }
        public static decimal SelectedPrice
        {
            get;
            set;
        }
        public JsonResult GetTicketActivityType()
        {
            var StatusTypes = db.TicketActivityDefinition_HelpDesks.ToList();

            return Json(StatusTypes, JsonRequestBehavior.AllowGet);
        }      
        public JsonResult UserNameDDBind()
        {

            var Parts = UserDb.Users.Select(s => new
            {
                Name = s.FirstName + " " + s.LastName,
                Value = s.UserName,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getGroupMembers(String TicketGroup)
        {
            var Group = db.SupportGroup_HelpDesks.FirstOrDefault(x => x.Name == TicketGroup);
            var Members = db.SupportGroupDetail_HelpDesks.ToList();

            if (Group != null)
            {
                Members = Members.Where(x => x.SupportGroupId == Group.SupportGroup_HelpDeskId).ToList();
            }

            var Parts =Members.Select(s => new
            {
                Name = s.EmployeeId,
                Value = s.SupportGroupId,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetSpecificTicket()
        {
            var tickets = db.TicketType_HelpDesks.AsQueryable();
            return Json(tickets, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAllEmployees()
        {
            var employee1 = new
            {
                EmployeeId = 1,
                FullNameaenglish = "beki"
            };
            var employee2 = new
            {
                EmployeeId = 2,
                FullNameaenglish = "beki2"
            };
            var list = new { employee1, employee2 };
            return Json(null, JsonRequestBehavior.AllowGet);

            //var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            //return Json(employees.Select(p => new
            //{
            //    EmployeeId = p.EmployeeId,
            //    FullNameaEnglish = p.EmployeeCode + "-" + p.FullNameaEnglish,
            //}), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getBranches()
        {
            var customers = db.Branch.AsQueryable();
            return Json(customers, JsonRequestBehavior.AllowGet);

        }      
        public static decimal SelectedTaxRate
        {
            get;
            set;
        }
        // GET: Common/DataProvider
        public ActionResult Index()
        {
            return View();
        }
        //New 
       
        public JsonResult GetMembersByGroup(string Group)
        {
            var SupportGroup = db.SupportGroup_HelpDesks.Where(x => x.Name == Group).FirstOrDefault();
            if (SupportGroup != null)
            {
                var Memebers = db.SupportGroupDetail_HelpDesks.Where(x => x.SupportGroupId == SupportGroup.SupportGroup_HelpDeskId).Select(s => new
                {
                    Name = s.EmployeeId,
                    Value = s.EmployeeId,
                }).ToList();

                return Json(Memebers, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GroupDDBind()
        {

            var Parts = db.SupportGroup_HelpDesks.Select(s => new  
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }

        public JsonResult PriorityDDBind()
        {

            var Parts = db.PriorityType_HelpDesks.Select(s => new
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult StatusDDBind()
        {

            var Parts = db.StatusType_HelpDesks.Select(s => new
            {
                Name = s.Name,
                Value = s.Name,
            }).ToList();
            return Json(Parts, JsonRequestBehavior.AllowGet);

        }
        public JsonResult StatusAwaitingDDBind()
        {

            var StatusAwaiting = db.StatusType_HelpDesks.Where(x => x.Status == InfoDeskFinalizeType.WAITING).ToList();
            return Json(StatusAwaiting, JsonRequestBehavior.AllowGet);

        }     
        public JsonResult GetNationality()
        {
            var countries = db.Countries.AsQueryable();
            return Json(countries.Select(p => new { CountryID = p.CountryId, Nationality = p.Nationality }), JsonRequestBehavior.AllowGet);
        }    
        public JsonResult GetSpecificationCategory()
        {
            var specificationCategoryList = db.Lookups.Where(sc => sc.LookupType == LookUpTypes.ADDITION_TYPE).AsQueryable();

            return Json(specificationCategoryList, JsonRequestBehavior.AllowGet);
        }              
        public JsonResult GetStatusType()
        {
            var StatusTypes = db.StatusType_HelpDesks.ToList();

            return Json(StatusTypes,JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGroup()
        {
            var Groups = db.SupportGroup_HelpDesks.ToList();

            return Json(Groups, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSupportGroup()
        {
            var groups = db.SupportGroup_HelpDesks.ToList();
            return Json(groups.Select(p => new
            {
                Name = p.Name,
                SupportGroup_HelpDeskId = p.SupportGroup_HelpDeskId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTicketDefintion()
        {
            var TicketActivityDefinition = db.TicketActivityDefinition_HelpDesks.ToList();

            return Json(TicketActivityDefinition, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPriorityType()
        {
            var PriorityTypes = db.PriorityType_HelpDesks.ToList();

            return Json(PriorityTypes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBankList()
        {
            var bankList = db.Banks.AsQueryable();

            return Json(bankList.Select(b => new { BankID = b.BankId, TradeName = b.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLanguage()
        {
            var languages = db.Languages.AsQueryable();
            return Json(languages.Select(p => new { LanguageID = p.LanguageId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCountry()
        {
            var countries = db.Countries.AsQueryable();
            return Json(countries.Select(p => new { CountryID = p.CountryId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNation()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.NATION).AsQueryable();
            return Json(lookups.Select(p => new { LookupID = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeList()
          {
              var employeeList = db.Person_Employee.Where(e => e.IsActive == true).AsQueryable();

              return Json(employeeList.Select(p => new { PersonID = p.EmployeeId, Name = p.FirstName + " " + p.MiddleName + " " + p.LastName }), JsonRequestBehavior.AllowGet);
          }

          public JsonResult GetTeamList()
          {
              var organizationStructure = db.MainCompanyStructures;
              return Json(organizationStructure.Select(os => new { OrgStructureID = os.OrgStructureId, Name = os.Name }), JsonRequestBehavior.AllowGet);
          }
   
       
      
        public ActionResult GetReference(string referenceType)
        {
            return View();
        }

        public JsonResult GetActiveEmployees()
        {
            var employees = db.Person_Employee.Where(x=>x.IsActive).ToList();

            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetActiveEmployeesFromGroup(int id)
        {
            var customer = db.CustomerTicket_HelpDesks.FirstOrDefault(x=>x.CustomerTicket_HelpDeskId == id);
            List<int> empIds = new List<int>();
            if(customer != null)
            {
                var group = db.SupportGroup_HelpDesks.FirstOrDefault(x =>x.SupportGroup_HelpDeskId == customer.SupportGroupId);
                if(group != null)
                {
                    var subGroups = db.SupportGroupDetail_HelpDesks.Where(x=>x.SupportGroupId == group.SupportGroup_HelpDeskId).ToList().Select(x=>x.EmployeeId);
                    if(subGroups.Any())
                    {
                        empIds.AddRange(subGroups);
                    }                   
                }
            }
         
            var employees = db.Person_Employee.Where(x => empIds.Contains(x.EmployeeId) && x.IsActive).ToList();

            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetMaterials()
        {
            var materials = db.MaterialItems.ToList();

            return Json(materials.Select(p => new
            {
                MaterialItemsId = p.MaterialItemsId,
                ItemName = p.ItemName
            }), JsonRequestBehavior.AllowGet);

        }


    }


}