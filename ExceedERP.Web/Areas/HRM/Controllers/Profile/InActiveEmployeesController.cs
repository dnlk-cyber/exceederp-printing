﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
    public class InActiveEmployeesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Branch"] = db.Branch.ToList();
            var commtTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COMMITMENT_TYPE);
            ViewData["commitment"] = commtTypes;
            var cosignTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COSIGN_TYPE);
            ViewData["CosignType"] = cosignTypes;

            var relations = db.Lookups.Where(l => l.LookupType == LookUpTypes.RELATIONSHIP);
            ViewData["Relationship"] = relations != null ? relations.ToList() : new List<Lookup>();

            GetNation();
            GetNationality();

            return View();
        }
        public JsonResult GetNation()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.NATION && l.IsActive).AsQueryable();
            ViewData["NationList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNationality()
        {
            var countries = db.Countries.AsQueryable();
            ViewData["Nationalites"] = countries;
            return Json(countries.Select(p => new { CountryId = p.CountryId, Nationality = p.Nationality }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult InActiveEmployees_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Person_Employee> person_employee = db.Person_Employee.Where(x=>!x.IsActive);
            DataSourceResult result = person_employee.ToDataSourceResult(request, person_Employee => new {
                EmployeeId = person_Employee.EmployeeId,
                EmployeeCode = person_Employee.EmployeeCode,
                Title = person_Employee.Title,
                FirstName = person_Employee.FirstName,
                MiddleName = person_Employee.MiddleName,
                LastName = person_Employee.LastName,
                Nationality = person_Employee.Nationality,
                DateOfBirth = person_Employee.DateOfBirth,
                DateOfBirthAm = person_Employee.DateOfBirthAm,
                Gender = person_Employee.Gender,
                CategoryID = person_Employee.CategoryID,
                IsActive = person_Employee.IsActive,
                MothersFullName = person_Employee.MothersFullName,
                BirthPlace = person_Employee.BirthPlace,
                Nation = person_Employee.Nation,
                Region = person_Employee.Region,
                Religion = person_Employee.Religion,
                MartialStatus = person_Employee.MartialStatus,
                Status = person_Employee.Status,
                HiredDate = person_Employee.HiredDate,
                HiredDateAmharic = person_Employee.HiredDateAmharic,
                Remark = person_Employee.Remark,
                FirstNameEnglish = person_Employee.FirstNameEnglish,
                MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                LastNameEnglish = person_Employee.LastNameEnglish,
                IsInsured = person_Employee.IsInsured,
                BranchId = person_Employee.BranchId,
                OrgBranchName = person_Employee.OrgBranchName,
                DateCreated = person_Employee.DateCreated,
                LastModified = person_Employee.LastModified,
                CreatedBy = person_Employee.CreatedBy,
                ModifiedBy = person_Employee.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult NotPlaced_Employees_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Person_Employee> person_employee = db.Person_Employee.Where(x => x.IsActive && !x.IsPlaced);
            DataSourceResult result = person_employee.ToDataSourceResult(request, person_Employee => new {
                EmployeeId = person_Employee.EmployeeId,
                EmployeeCode = person_Employee.EmployeeCode,
                Title = person_Employee.Title,
                FirstName = person_Employee.FirstName,
                MiddleName = person_Employee.MiddleName,
                LastName = person_Employee.LastName,
                Nationality = person_Employee.Nationality,
                DateOfBirth = person_Employee.DateOfBirth,
                DateOfBirthAm = person_Employee.DateOfBirthAm,
                Gender = person_Employee.Gender,
                CategoryID = person_Employee.CategoryID,
                IsActive = person_Employee.IsActive,
                MothersFullName = person_Employee.MothersFullName,
                BirthPlace = person_Employee.BirthPlace,
                Nation = person_Employee.Nation,
                Region = person_Employee.Region,
                Religion = person_Employee.Religion,
                MartialStatus = person_Employee.MartialStatus,
                Status = person_Employee.Status,
                HiredDate = person_Employee.HiredDate,
                HiredDateAmharic = person_Employee.HiredDateAmharic,
                Remark = person_Employee.Remark,
                FirstNameEnglish = person_Employee.FirstNameEnglish,
                MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                LastNameEnglish = person_Employee.LastNameEnglish,
                IsInsured = person_Employee.IsInsured,
                BranchId = person_Employee.BranchId,
                OrgBranchName = person_Employee.OrgBranchName,
                DateCreated = person_Employee.DateCreated,
                LastModified = person_Employee.LastModified,
                CreatedBy = person_Employee.CreatedBy,
                ModifiedBy = person_Employee.ModifiedBy
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InActiveEmployees_Create([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            if (ModelState.IsValid)
            {
                var entity = new Person_Employee
                {
                    EmployeeCode = person_Employee.EmployeeCode,
                    Title = person_Employee.Title,
                    FirstName = person_Employee.FirstName,
                    MiddleName = person_Employee.MiddleName,
                    LastName = person_Employee.LastName,
                    Nationality = person_Employee.Nationality,
                    DateOfBirth = person_Employee.DateOfBirth,
                    DateOfBirthAm = person_Employee.DateOfBirthAm,
                    Gender = person_Employee.Gender,
                    CategoryID = person_Employee.CategoryID,
                    IsActive = person_Employee.IsActive,
                    MothersFullName = person_Employee.MothersFullName,
                    BirthPlace = person_Employee.BirthPlace,
                    Nation = person_Employee.Nation,
                    Region = person_Employee.Region,
                    Religion = person_Employee.Religion,
                    MartialStatus = person_Employee.MartialStatus,
                    Status = person_Employee.Status,
                    HiredDate = person_Employee.HiredDate,
                    HiredDateAmharic = person_Employee.HiredDateAmharic,
                    Remark = person_Employee.Remark,
                    FirstNameEnglish = person_Employee.FirstNameEnglish,
                    MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                    LastNameEnglish = person_Employee.LastNameEnglish,
                    IsInsured = person_Employee.IsInsured,
                    BranchId = person_Employee.BranchId,
                    OrgBranchName = person_Employee.OrgBranchName,
                    DateCreated = person_Employee.DateCreated,
                    LastModified = person_Employee.LastModified,
                    CreatedBy = person_Employee.CreatedBy,
                    ModifiedBy = person_Employee.ModifiedBy
                };

                db.Person_Employee.Add(entity);
                db.SaveChanges();
                person_Employee.EmployeeId = entity.EmployeeId;
            }

            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InActiveEmployees_Update([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            if (ModelState.IsValid)
            {
                var entity = new Person_Employee
                {
                    EmployeeId = person_Employee.EmployeeId,
                    EmployeeCode = person_Employee.EmployeeCode,
                    Title = person_Employee.Title,
                    FirstName = person_Employee.FirstName,
                    MiddleName = person_Employee.MiddleName,
                    LastName = person_Employee.LastName,
                    Nationality = person_Employee.Nationality,
                    DateOfBirth = person_Employee.DateOfBirth,
                    DateOfBirthAm = person_Employee.DateOfBirthAm,
                    Gender = person_Employee.Gender,
                    CategoryID = person_Employee.CategoryID,
                    IsActive = person_Employee.IsActive,
                    MothersFullName = person_Employee.MothersFullName,
                    BirthPlace = person_Employee.BirthPlace,
                    Nation = person_Employee.Nation,
                    Region = person_Employee.Region,
                    Religion = person_Employee.Religion,
                    MartialStatus = person_Employee.MartialStatus,
                    Status = person_Employee.Status,
                    HiredDate = person_Employee.HiredDate,
                    HiredDateAmharic = person_Employee.HiredDateAmharic,
                    Remark = person_Employee.Remark,
                    FirstNameEnglish = person_Employee.FirstNameEnglish,
                    MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                    LastNameEnglish = person_Employee.LastNameEnglish,
                    IsInsured = person_Employee.IsInsured,
                    BranchId = person_Employee.BranchId,
                    OrgBranchName = person_Employee.OrgBranchName,
                    DateCreated = person_Employee.DateCreated,
                    LastModified = person_Employee.LastModified,
                    CreatedBy = person_Employee.CreatedBy,
                    ModifiedBy = person_Employee.ModifiedBy
                };

                db.Person_Employee.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InActiveEmployees_Destroy([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            if (ModelState.IsValid)
            {
                var entity = new Person_Employee
                {
                    EmployeeId = person_Employee.EmployeeId,
                    EmployeeCode = person_Employee.EmployeeCode,
                    Title = person_Employee.Title,
                    FirstName = person_Employee.FirstName,
                    MiddleName = person_Employee.MiddleName,
                    LastName = person_Employee.LastName,
                    Nationality = person_Employee.Nationality,
                    DateOfBirth = person_Employee.DateOfBirth,
                    DateOfBirthAm = person_Employee.DateOfBirthAm,
                    Gender = person_Employee.Gender,
                    CategoryID = person_Employee.CategoryID,
                    IsActive = person_Employee.IsActive,
                    MothersFullName = person_Employee.MothersFullName,
                    BirthPlace = person_Employee.BirthPlace,
                    Nation = person_Employee.Nation,
                    Region = person_Employee.Region,
                    Religion = person_Employee.Religion,
                    MartialStatus = person_Employee.MartialStatus,
                    Status = person_Employee.Status,
                    HiredDate = person_Employee.HiredDate,
                    HiredDateAmharic = person_Employee.HiredDateAmharic,
                    Remark = person_Employee.Remark,
                    FirstNameEnglish = person_Employee.FirstNameEnglish,
                    MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                    LastNameEnglish = person_Employee.LastNameEnglish,
                    IsInsured = person_Employee.IsInsured,
                    BranchId = person_Employee.BranchId,
                    OrgBranchName = person_Employee.OrgBranchName,
                    DateCreated = person_Employee.DateCreated,
                    LastModified = person_Employee.LastModified,
                    CreatedBy = person_Employee.CreatedBy,
                    ModifiedBy = person_Employee.ModifiedBy
                };

                db.Person_Employee.Attach(entity);
                db.Person_Employee.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
