﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureJobOrderProductionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();

            ViewData["JobType"] = db.FurnitureStandardJobTypes.Select(s => new
            {
                Value = s.FurnitureStandardJobTypeId,
                Text = s.JobTypeName
            }).ToList();
            ViewData["ItemCode"] = db.InventoryItems.Select(s => new
            {
                Value = s.ItemCode,
                Text = s.Name
            }).ToList();

            ViewData["MaterialCategory"] = db.ManufacturingMaterialCategories.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryId,
                Text = s.ManufacturingCategory
            }).ToList();

            ViewData["MaterialCategoryItem"] = db.ManufacturingMaterialCategoryItems.Select(s => new
            {
                Value = s.ManufacturingMaterialCategoryItemId,
                Text = s.ItemName
            }).ToList();

            ViewData["TaskCategory"] = db.ManifucturingTaskCategories.Select(s => new
            {
                Value = s.ManifucturingTaskCategoryId,
                Text = s.Name
            });

            ViewData["Employee"] = db.Person_Employee.Select(s => new
            {
                Text = s.FirstName + "-" + s.MiddleName + "-" + s.LastName,
                Value = s.EmployeeId
            });

            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var costCenter = db.BranchCostCenter;
            var branches = db.Branch;

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

            ViewData["Branches"] = branches;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();



            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var stores = db.InventoryStores;
            var branches = db.Branch;
            var costCenter = db.BranchCostCenter;
            var selectedcostCenter = costCenter.Select(
              i => new
              {
                  Name = i.MainCompanyStructure.Name,
                  Code = i.OrgStructureId
              }).AsQueryable();

            var inventoryPeriods = db.InventoryPeriods;


            ViewData["costCenters"] = selectedcostCenter;
            ViewData["stores"] = stores;
            ViewData["Branches"] = branches;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();
            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;

            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }
        public ActionResult FurnitureJobOrderProductions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobOrderProduction> furniturejoborderproductions = db.FurnitureJobOrderProductions;
            DataSourceResult result = furniturejoborderproductions.ToDataSourceResult(request, furnitureJobOrderProduction => new FurnitureJobOrderProduction
            {
                FurnitureJobOrderProductionId = furnitureJobOrderProduction.FurnitureJobOrderProductionId,
                CustomerId = furnitureJobOrderProduction.CustomerId,
                JobTypeId = furnitureJobOrderProduction.JobTypeId,
                JobNo = furnitureJobOrderProduction.JobNo,
                Unit = furnitureJobOrderProduction.Unit,
                Quantity = furnitureJobOrderProduction.Quantity,
                DeliveryDate = furnitureJobOrderProduction.DeliveryDate,
                IsSendForApproval = furnitureJobOrderProduction.IsSendForApproval,
                IsOnlineApproved = furnitureJobOrderProduction.IsOnlineApproved
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderProductions_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderProduction furnitureJobOrderProduction)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderProduction
                {
                    CustomerId = furnitureJobOrderProduction.CustomerId,
                    JobTypeId = furnitureJobOrderProduction.JobTypeId,
                    JobNo = furnitureJobOrderProduction.JobNo,
                    Unit = furnitureJobOrderProduction.Unit,
                    Quantity = furnitureJobOrderProduction.Quantity,
                    DeliveryDate = furnitureJobOrderProduction.DeliveryDate
                };

                db.FurnitureJobOrderProductions.Add(entity);
                db.SaveChanges();
                furnitureJobOrderProduction.FurnitureJobOrderProductionId = entity.FurnitureJobOrderProductionId;
            }

            return Json(new[] { furnitureJobOrderProduction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderProductions_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderProduction furnitureJobOrderProduction)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderProduction
                {
                    FurnitureJobOrderProductionId = furnitureJobOrderProduction.FurnitureJobOrderProductionId,
                    CustomerId = furnitureJobOrderProduction.CustomerId,
                    JobTypeId = furnitureJobOrderProduction.JobTypeId,
                    JobNo = furnitureJobOrderProduction.JobNo,
                    Unit = furnitureJobOrderProduction.Unit,
                    Quantity = furnitureJobOrderProduction.Quantity,
                    DeliveryDate = furnitureJobOrderProduction.DeliveryDate
                };

                db.FurnitureJobOrderProductions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobOrderProduction }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureJobOrderProductions_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobOrderProduction furnitureJobOrderProduction)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureJobOrderProduction
                {
                    FurnitureJobOrderProductionId = furnitureJobOrderProduction.FurnitureJobOrderProductionId,
                    CustomerId = furnitureJobOrderProduction.CustomerId,
                    JobTypeId = furnitureJobOrderProduction.JobTypeId,
                    JobNo = furnitureJobOrderProduction.JobNo,
                    Unit = furnitureJobOrderProduction.Unit,
                    Quantity = furnitureJobOrderProduction.Quantity,
                    DeliveryDate = furnitureJobOrderProduction.DeliveryDate
                };

                db.FurnitureJobOrderProductions.Attach(entity);
                db.FurnitureJobOrderProductions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { furnitureJobOrderProduction }.ToDataSourceResult(request, ModelState));
        }


        #region Approval
        public JsonResult CheckProduction(int id)
        {
            object response = null;
            var production = db.FurnitureJobOrderProductions.Find(id);
            
            if (production != null)
            {
                var jobAssignments = db.FurnitureJobAssignments.Where(x => x.FurnitureJobOrderProductionId == production.FurnitureJobOrderProductionId).ToList();
                if (jobAssignments.Any())
                {
                    production.IsSendForApproval = true;
                    production.SendForApprovalBy = User.Identity.Name;
                    production.SendForApprovalTime = DateTime.Today;

                    db.FurnitureJobOrderProductions.Attach(production);
                    db.Entry(production).State = EntityState.Modified;
                    db.SaveChanges();
                    response = new { Success = true, Message = "Furniture Production Checked Successfully!" };
                }
                else
                {
                    response = new { Success = false, Message = "Something is wrong! You haven't assigned a job to any employee!" };
                }
                
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }

        public JsonResult ApproveProduction(int id)
        {
            object response = null;
            var production = db.FurnitureJobOrderProductions.Find(id);
            if (production != null)
            {
                //--Generate Production--//
                GenerateProductionFollowUp(production.FurnitureJobOrderProductionId);

                production.IsOnlineApproved = true;
                production.OnlineApprovedBy = User.Identity.Name;
                production.OnlineApprovedTime = DateTime.Today;


                db.FurnitureJobOrderProductions.Attach(production);
                db.Entry(production).State = EntityState.Modified;
                db.SaveChanges();
                
                response = new { Success = true, Message = "Successfully Approved and Created Daily Production follow-up!" };
            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }

        private void GenerateProductionFollowUp(int furnitureJobOrderProductionId)
        {
            var jobAssignments = db.FurnitureJobAssignments.Where(x => x.FurnitureJobOrderProductionId == furnitureJobOrderProductionId).ToList();
            foreach (var jobAssignment in jobAssignments)
            {
                var assignedWorkDays = (jobAssignment.TaskEndDate.Date - jobAssignment.TaskAssignedDate.Date).Days;
                for (int i = 0; i < assignedWorkDays; i++)
                {
                    var entity = new FurnitureDailyProductionFollowUp
                    {
                        DailyProductionPlan = jobAssignment.DailyProductionPlan,
                        EmployeeId = jobAssignment.EmployeeId,                       
                        WorkShop = jobAssignment.Type,
                        ManifucturingTaskCategoryId = jobAssignment.TaskCategoryId,
                        FurnitureJobOrderProductionId = jobAssignment.FurnitureJobOrderProductionId,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today,
                        
                    };
                    db.FurnitureDailyProductionFollowUps.Add(entity);                 
                }             
            }
            db.SaveChanges();
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
