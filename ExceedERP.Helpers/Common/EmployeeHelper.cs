using System;
using System.Collections.Generic;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Helpers.Common
{

	public static class EmployeeHelper
	{

        public static int? GetSupervisorEmployeeId(int employeeId, int workId, int branchId, int businessUnitId)
        {

            using (var db = new ExceedDbContext())
            {
                var placement = db.Placements.FirstOrDefault(x => x.OrgStructureId == workId
                                                               && x.BranchId == branchId
                                                               && x.BusinessUnitId == businessUnitId
                                                               && x.EmployeeId != employeeId
                                                               && x.isCurrent
                                                               && x.IsHead);

                // Return the EmployeeId if a match is found, otherwise null
                return placement?.EmployeeId;
            }
        }

        public static int GetHeadId(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				int headId = 0;
				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null && placement.HeadId != null)
				{

					return (int)placement.HeadId;
				}

				return headId;
			}
		}
        //public static int GetDelegatorEmployeeId(int empId)
        //{
        //	using (ExceedDbContext db = new ExceedDbContext())
        //	{
        //		int delegatorEmpId = 0;
        //		DateTime curentDate = DateTime.Today;
        //		var delegation = db.Delegations.Where(x => x.EmployeeId == empId)
        //			.Where(x => x.ToDateTime > curentDate || x.ToDateTime == null)
        //			.Where(x => x.IsOpen).FirstOrDefault();
        //		if (delegation != null)
        //		{
        //			delegatorEmpId = delegation.FromEmployeeId;
        //		}

        //		return delegatorEmpId;
        //	}
        //}
        public static int GetDelegatorEmployeeId(int empId)
        {
            using (var db = new ExceedDbContext())
            {
                var today = DateTime.Today;

                var activeDelegation = db.Delegations
                    .Where(d =>
                        d.EmployeeId == empId &&
                        d.IsOpen &&
                        (!d.ToDateTime.HasValue || d.ToDateTime > today))
                    .FirstOrDefault();

                return activeDelegation?.FromEmployeeId ?? 0;
            }
        }

        public static List<int> GetSubordinatesOfSubortinates_FilteredByHead(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				List<int> employeeList = new List<int>();

				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null && placement.IsHead)
				{
					var empPlacement = db.Placements.Where(x => x.isCurrent && x.HeadId == placement.EmployeeId).ToList();

					foreach (var emp in empPlacement)
					{
						if (emp.IsHead)
						{
							var childs = db.Placements.Where(x => x.HeadId == emp.EmployeeId);
							foreach (var child in childs)
							{
								employeeList.Add(child.EmployeeId);
							}
						}
						employeeList.Add(emp.EmployeeId);
					}
					return employeeList;
				}
				else
				{
					//if (placement != null) employeeList.Add(placement.EmployeeId);

					return employeeList;
				}
			}

		}
		public static List<Person_Employee> GetEmployeesByBranch(int branchId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				var list = new List<Person_Employee>();
				var allEmployee = db.Person_Employee.ToList();
				if (allEmployee.Any())
				{
					var placements = db.Placements.Where(x => x.BranchId == branchId).ToList();
					if (placements.Any())
					{
						foreach (var p in placements)
						{
							var check = allEmployee.FirstOrDefault(x => x.EmployeeId == p.EmployeeId);
							if (check != null)
							{
								list.Add(check);
							}
						}
					}
				}

				return list;
			}

		}
		public static List<Person_Employee> GetEmployeesBySelectedBranch(List<string> branchId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				var list = new List<Person_Employee>();
				var allEmployee = db.Person_Employee.ToList();

				var placements = db.Placements.Where(x => branchId.Contains(x.BranchId.ToString()) && x.isCurrent).ToList();
				if (placements.Any())
				{
					foreach (var p in placements)
					{
						var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
						if (check != null)
						{
							list.Add(check);
						}
					}
				}
				return list;
			}

		}
		public static List<Person_Employee> GetEmployeesByReportsTo(int reportstoId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				var list = new List<Person_Employee>();
				var allEmployee = db.Person_Employee.ToList();
				var placements = db.Placements.Where(x => x.ReportsTo == reportstoId && x.isCurrent).ToList();
				if (placements.Any())
				{
					foreach (var p in placements)
					{
						var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
						if (check != null)
						{
							list.Add(check);
						}
					}
				}
				return list;
			}

		}
		public static List<Person_Employee> GetEmployeesByBranchAndWorkUnit(int branchId, int orgStructureId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				var list = new List<Person_Employee>();
				var allEmployee = db.Person_Employee.ToList();
				var placements = db.Placements.Where(x => x.BranchId == branchId && x.OrgStructureId == orgStructureId && x.isCurrent).ToList();
				if (placements.Any())
				{
					var filterd = from f in placements
								  join e in allEmployee
								  on f.EmployeeId equals e.EmployeeId
								  where f.EmployeeId == e.EmployeeId
								  select (new { e });

					foreach (var p in filterd)
					{

						list.Add(p.e);

					}
				}
				return list;
			}

		}

		public static List<Person_Employee> GetEmployeesBySelectedBranchAndWorkUnit(List<string> branchId, int orgStructureId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				var list = new List<Person_Employee>();
				var allEmployee = db.Person_Employee.ToList();
				var placements = db.Placements.Where(x => branchId.Contains(x.BranchId.ToString()) && x.OrgStructureId == orgStructureId && x.isCurrent).ToList();
				if (placements.Any())
				{
					var filterd = from f in placements
								  join e in allEmployee
								  on f.EmployeeId equals e.EmployeeId
								  where f.EmployeeId == e.EmployeeId
								  select (new { e });

					foreach (var p in filterd)
					{

						list.Add(p.e);

					}
				}
				return list;
			}

		}
		public static List<Person_Employee> GetEmployeesByWorkUnit(int orgStructureId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				var list = new List<Person_Employee>();
				var allEmployee = db.Person_Employee.ToList();
				if (allEmployee.Any())
				{
					var placements = db.Placements.Where(x => x.OrgStructureId == orgStructureId && x.isCurrent).ToList();
					if (placements.Any())
					{
						foreach (var p in placements)
						{
							var check = allEmployee.FirstOrDefault(x => x.EmployeeId == p.EmployeeId);
							if (check != null)
							{
								list.Add(check);
							}
						}
					}

				}
				return list;
			}
		}
		public static string GetWorkUnitName(int orgStructureId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				string Name = "";
				var Structure = db.MainCompanyStructures.Where(x => x.OrgStructureId == orgStructureId).FirstOrDefault();
				if (Structure != null)
				{
					Name = Structure.Name;
				}
				return Name;
			}
		}
		//new
		public static double GetAge(int empId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				var employee = db.Person_Employee.FirstOrDefault(x => x.IsActive && x.EmployeeId == empId);
				if (employee != null)
				{
					double age = DateTime.Now.Date.Subtract(new DateTime(employee.DateOfBirth.Year, employee.DateOfBirth.Month, employee.DateOfBirth.Day)).TotalDays / 365.25;
					return age;
				}
				return 0;
			}
		}
		public static string GetEmployeeJobTitle(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{
				string jobTitle = "";
				var assignment = db.Placements
					.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);
				if (assignment != null) jobTitle = assignment.JobTitleName;
				return jobTitle;
			}
		}
		public static string GetEmployeeEnglishName(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var employee = db.Person_Employee
					.FirstOrDefault(x => x.EmployeeId == empid);
				if (employee != null) return employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
				return "";
			}
		}

		public static int GetSupervisor(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{


				int nextempid = 0;
				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null)
				{
					if (placement.IsHead)
					{
						var currentWorkunit = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
						if (currentWorkunit != null)
						{
							var parentworkunit = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == currentWorkunit.Parent);
							if (parentworkunit != null)
							{
								var parentbranch = db.Branch.Find(placement.BranchId);
								var headPlacement = db.Placements
									.Where(x => x.BranchId == parentbranch.BranchId || x.BranchId == placement.BranchId)
									.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == parentworkunit.OrgStructureId);
								if (headPlacement != null)
								{
									nextempid = headPlacement.EmployeeId;
								}
							}

						}

					}
					else
					{
						var head = db.Placements
							.Where(x => x.BranchId == placement.BranchId)//added
							.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == placement.OrgStructureId);
						if (head != null)
						{
							nextempid = head.EmployeeId;
						}
					}
				}
				return nextempid;
			}
		}

		public static int GetSupervisor_old(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				int nextempid = 0;
				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null && placement.IsHead)
				{
					var currentWorkunit = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
					if (currentWorkunit != null)
					{
						var parentworkunit = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == currentWorkunit.Parent);
						if (parentworkunit != null)
						{
							var headPlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == parentworkunit.OrgStructureId);
							if (headPlacement != null)
							{
								nextempid = headPlacement.EmployeeId;
							}
						}

					}

				}
				else if (placement != null)
				{
					var head = db.Placements.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == placement.OrgStructureId);
					if (head != null)
					{
						nextempid = head.EmployeeId;
					}
				}
				return nextempid;
			}
		}
		//new filtered by branch also
		public static List<int> GetSubordinates(int empid)
		{
			
			List<int> employeeList = new List<int>();
			var directSub = GetSubordinateEmps(empid);
			employeeList.AddRange(directSub);

			//if the current employee is delegated by other
			int delegatetorEmpId = GetDelegatorEmployeeId(empid);
			if(delegatetorEmpId!=0)
			{
				var delegatedSub = GetSubordinateEmps(delegatetorEmpId);
				employeeList.AddRange(delegatedSub);
			}
			
			return employeeList;            
		   
		}
		private static List<int> GetSubordinateEmps(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				List<int> employeeList = new List<int>();
				//if the current employee is delegated by other
				int delegatetorEmpId = GetDelegatorEmployeeId(empid);

				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);


				if (placement != null && placement.IsHead)
				{
					var empPlacement = db.Placements.Where(x => x.isCurrent && x.OrgStructureId == placement.OrgStructureId && x.BranchId == placement.BranchId).ToList();

					foreach (var emp in empPlacement)
					{
						employeeList.Add(emp.EmployeeId);
					}
					var childWorkunits = db.MainCompanyStructures.Where(x => x.Parent == placement.OrgStructureId).ToList();
					foreach (var ch in childWorkunits)
					{
						var headPlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == ch.OrgStructureId && x.BranchId == placement.BranchId);
						if (headPlacement != null)
						{
							employeeList.Add(headPlacement.EmployeeId);
						}

						var branchs = db.Branch.Where(x => x.BranchId != placement.BranchId && x.ParentBranchId == placement.BranchId).ToList();//added &&x.ParentBranchId== placement.BranchId
						foreach (var bra in branchs)
						{
							var otherOneBranch = db.Placements.FirstOrDefault(x => x.BranchId == bra.BranchId && x.isCurrent && x.IsHead && x.OrgStructureId == placement.OrgStructureId);
							// if that department is not there in the branch, list all the child department heads to child of the head office parent 
							if (otherOneBranch == null)
							{
								var otherBranchChilds = db.Placements.Where(x => x.BranchId == bra.BranchId && x.isCurrent && x.IsHead && x.OrgStructureId == ch.OrgStructureId).Select(x => x.EmployeeId).ToList();
								employeeList.AddRange(otherBranchChilds);
							}
						}
					}

					return employeeList;
				}
				else
				{
					if (placement != null) employeeList.Add(placement.EmployeeId);
					return employeeList;
				}
				//child head
			}
		}

		public static List<int> GetSubordinates_old1(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				List<int> employeeList = new List<int>();

				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);


				if (placement != null && placement.IsHead)
				{
					var workunits = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
					if (workunits != null)
					{
						var empPlacement = db.Placements.Where(x => x.isCurrent && x.OrgStructureId == workunits.OrgStructureId).ToList();

						foreach (var emp in empPlacement)
						{
							employeeList.Add(emp.EmployeeId);
						}
						var childWorkunits = db.MainCompanyStructures.Where(x => x.Parent == workunits.OrgStructureId).ToList();
						foreach (var ch in childWorkunits)
						{
							if (childWorkunits.Any())
							{
								var headPlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == ch.OrgStructureId);
								if (headPlacement != null)
								{
									employeeList.Add(headPlacement.EmployeeId);
								}
							}
						}
					}

					return employeeList;
				}

				else
				{
					if (placement != null) employeeList.Add(placement.EmployeeId);
					return employeeList;
				}
				//child head
			}


		}
		//new ..added active branch only
		public static List<int> GetSubordinates_FilteredByBranchAlso(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				List<int> employeeList = new List<int>();

				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null && placement.IsHead)
				{
					var empPlacement = db.Placements.Where(x => x.isCurrent && x.OrgStructureId == placement.OrgStructureId && x.BranchId == placement.BranchId).ToList();

					foreach (var emp in empPlacement)
					{
						employeeList.Add(emp.EmployeeId);
					}
					var childWorkunits = db.MainCompanyStructures.Where(x => x.IsActive).Where(x => x.Parent == placement.OrgStructureId).ToList();
					foreach (var ch in childWorkunits)
					{
						var headPlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == ch.OrgStructureId && x.BranchId == placement.BranchId);
						if (headPlacement != null)
						{
							employeeList.Add(headPlacement.EmployeeId);
						}

						var branchs = db.Branch.Where(x => x.IsActive).Where(x => x.BranchId != placement.BranchId).ToList();
						foreach (var bra in branchs)
						{
							var otherOneBranch = db.Placements.FirstOrDefault(x => x.BranchId == bra.BranchId && x.isCurrent && x.IsHead && x.OrgStructureId == placement.OrgStructureId);
							// if that department is not there in the branch, list all the child department heads to child of the head office parent 
							if (otherOneBranch == null)
							{
								var otherBranchChilds = db.Placements.Where(x => x.BranchId == bra.BranchId && x.isCurrent && x.IsHead && x.OrgStructureId == ch.OrgStructureId).Select(x => x.EmployeeId).ToList();
								employeeList.AddRange(otherBranchChilds);
							}
						}
					}

					return employeeList;
				}
				else
				{
					if (placement != null) employeeList.Add(placement.EmployeeId);
					return employeeList;
				}
				//child head
			}
		}
		public static List<int> GetSubordinates_FilteredByBranchAlso_1(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				List<int> employeeList = new List<int>();

				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null && placement.IsHead)
				{
					var empPlacement = db.Placements.Where(x => x.isCurrent && x.OrgStructureId == placement.OrgStructureId && x.BranchId == placement.BranchId).ToList();

					foreach (var emp in empPlacement)
					{
						employeeList.Add(emp.EmployeeId);
					}
					var childWorkunits = db.MainCompanyStructures.Where(x => x.Parent == placement.OrgStructureId).ToList();
					foreach (var ch in childWorkunits)
					{
						var headPlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == ch.OrgStructureId && x.BranchId == placement.BranchId);
						if (headPlacement != null)
						{
							employeeList.Add(headPlacement.EmployeeId);
						}

						var branchs = db.Branch.Where(x => x.BranchId != placement.BranchId && x.ParentBranchId == placement.BranchId).ToList();//added &&x.ParentBranchId== placement.BranchId
						foreach (var bra in branchs)
						{
							var otherOneBranch = db.Placements.FirstOrDefault(x => x.BranchId == bra.BranchId && x.isCurrent && x.IsHead && x.OrgStructureId == placement.OrgStructureId);
							// if that department is not there in the branch, list all the child department heads to child of the head office parent 
							if (otherOneBranch == null)
							{
								var otherBranchChilds = db.Placements.Where(x => x.BranchId == bra.BranchId && x.isCurrent && x.IsHead && x.OrgStructureId == ch.OrgStructureId).Select(x => x.EmployeeId).ToList();
								employeeList.AddRange(otherBranchChilds);
							}
						}
					}

					return employeeList;
				}
				else
				{
					if (placement != null) employeeList.Add(placement.EmployeeId);
					return employeeList;
				}
				//child head
			}
		}
		//previous method
		public static List<int> GetSubordinates_FilteredByBranchAlso_old1(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				List<int> employeeList = new List<int>();

				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null && placement.IsHead)
				{
					var empPlacement = db.Placements.Where(x => x.isCurrent && x.OrgStructureId == placement.OrgStructureId && x.BranchId == placement.BranchId).ToList();

					foreach (var emp in empPlacement)
					{
						employeeList.Add(emp.EmployeeId);
					}
					var childWorkunits = db.MainCompanyStructures.Where(x => x.Parent == placement.OrgStructureId).ToList();
					foreach (var ch in childWorkunits)
					{
						var headPlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == ch.OrgStructureId && x.BranchId == placement.BranchId);
						if (headPlacement != null)
						{
							employeeList.Add(headPlacement.EmployeeId);
						}

						var branchs = db.Branch.Where(x => x.BranchId != placement.BranchId).ToList();
						foreach (var bra in branchs)
						{
							var otherOneBranch = db.Placements.FirstOrDefault(x => x.BranchId == bra.BranchId && x.isCurrent && x.IsHead && x.OrgStructureId == placement.OrgStructureId);
							// if that department is not there in the branch, list all the child department heads to child of the head office parent 
							if (otherOneBranch == null)
							{
								var otherBranchChilds = db.Placements.Where(x => x.BranchId == bra.BranchId && x.isCurrent && x.IsHead && x.OrgStructureId == ch.OrgStructureId).Select(x => x.EmployeeId).ToList();
								employeeList.AddRange(otherBranchChilds);
							}
						}
					}

					return employeeList;
				}
				else
				{
					if (placement != null) employeeList.Add(placement.EmployeeId);
					return employeeList;
				}
			}
			//child head
		}
        public static List<int> GetSubordinates_FilteredByHead(int empid)
        {
            using (var db = new ExceedDbContext())
            {
                var placement = db.Placements
                    .FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

                int delegatorId = GetDelegatorEmployeeId(empid);

                // If not head and not delegated, return empty
                if ((placement?.IsHead != true) && delegatorId == 0)
                    return new List<int>();

                // If delegated, act as head using delegatorId
                var subordinates = db.Placements
                    .Where(x => x.isCurrent &&
                                (x.HeadId == empid ||
                                 (delegatorId != 0 && x.HeadId == delegatorId)))
                    .Select(x => x.EmployeeId)
                    .Distinct()
                    .ToList();

                return subordinates;
            }
        }

        //new 
        //      public static List<int> GetSubordinates_FilteredByHead(int empid)
        //{
        //	using (ExceedDbContext db = new ExceedDbContext())
        //	{

        //		List<int> employeeList = new List<int>();

        //		var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

        //		if (placement != null && placement.IsHead)
        //		{
        //			var empPlacement = db.Placements.Where(x => x.isCurrent && x.HeadId == placement.EmployeeId).ToList();

        //			foreach (var emp in empPlacement)
        //			{
        //				employeeList.Add(emp.EmployeeId);
        //			}

        //			//if the current employee is delegated by other.will add cehck endless delegation
        //			int delegatetorEmpId = GetDelegatorEmployeeId(empid);
        //			if (delegatetorEmpId != 0)
        //			{
        //				var delegatedSub = GetSubordinateEmps(delegatetorEmpId);
        //				employeeList.AddRange(delegatedSub);
        //			}

        //			return employeeList;
        //		}
        //		else
        //		{
        //			//if the current employee is delegated by other.will add cehck endless delegation
        //			int delegatetorEmpId = GetDelegatorEmployeeId(empid);
        //			if (delegatetorEmpId != 0)
        //			{
        //				var delegatedSub = GetSubordinateEmps(delegatetorEmpId);
        //				employeeList.AddRange(delegatedSub);
        //			}
        //			return employeeList;
        //		}
        //	}

        //}
        public static List<int> GetSubordinates_FilteredByHeadAndBranch(int empid, int branchId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				List<int> employeeList = new List<int>();

				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null && placement.IsHead)
				{
					var empPlacement = db.Placements.Where(x => x.BranchId == branchId).Where(x => x.isCurrent && x.HeadId == placement.EmployeeId).ToList();

					foreach (var emp in empPlacement)
					{
						employeeList.Add(emp.EmployeeId);
					}

					return employeeList;
				}
				else
				{
					//if (placement != null) employeeList.Add(placement.EmployeeId);
					return employeeList;
				}
			}

		}

        public static string GetDaystoWordsEnglish(double totalday)
        {
            //int totalday = (int)days;
            int months = 0;
            int year = 0;
            string words = "";
            if (totalday < 30)
            {
                words = words + totalday + " Days ";
            }
            else if ((totalday >= 30) && (totalday <= 365))
            {
                months = (int)(totalday / 30);
                words = words + months + " Months ";
                double monthremainder = totalday % 30;
                if (monthremainder < 30)
                {
                    words = words + "From " + (int)monthremainder + " Day ";
                }
            }
            else if (totalday >= 365)
            {
                year = (int)(totalday / 365);
                words = words + year + " Year ";
                double yearremainder = totalday % 365;
                if ((yearremainder >= 30) && (yearremainder <= 365))
                {
                    months = (int)(yearremainder / 30);
                    words = words + "From " + months + " Month ";
                    double monthremainder = yearremainder % 30;
                    if (monthremainder < 30)
                    {
                        words = words + "From " + (int)monthremainder + " Day ";
                    }
                }
            }

            return words;
        }

        public static string GetDaystoWords(double totalday)
		{
			//int totalday = (int)days;
			int months = 0;
			int year = 0;
			string words = "";
			if (totalday < 30)
			{
				words = words + totalday + " days ";
			}
			else if ((totalday >= 30) && (totalday <= 365))
			{
				months = (int)(totalday / 30);
				words = words + months + " month ";
				double monthremainder = totalday % 30;
				if (monthremainder < 30)
				{
					words = words + (int)monthremainder + " days ";
				}
			}
			else if (totalday >= 356)
			{
				year = (int)(totalday / 365);
				words = words + year + " year ";
				double yearremainder = totalday % 365;
				if ((yearremainder >= 30) && (yearremainder <= 365))
				{
					months = (int)(yearremainder / 30);
					words = words + months + " month ";
					double monthremainder = yearremainder % 30;
					if (monthremainder < 30)
					{
						words = words + (int)monthremainder + " days ";
					}
				}
			}

			return words;
		}

		public static string GetDaystoWordsAmharic(double totalday)
		{
			//int totalday = (int)days;
			int months = 0;
			int year = 0;
			string words = "";
			if (totalday < 30)
			{
				words = words + totalday + " ቀን ";
			}
			else if ((totalday >= 30) && (totalday <= 365))
			{
				months = (int)(totalday / 30);
				words = words + months + " ወር ";
				double monthremainder = totalday % 30;
				if (monthremainder < 30)
				{
					words = words + "ከ " + (int)monthremainder + " ቀን ";
				}
			}
			else if (totalday >= 356)
			{
				year = (int)(totalday / 365);
				words = words + year + " አመት ";
				double yearremainder = totalday % 365;
				if ((yearremainder >= 30) && (yearremainder <= 365))
				{
					months = (int)(yearremainder / 30);
					words = words + "ከ " + months + " ወር ";
					double monthremainder = yearremainder % 30;
					if (monthremainder < 30)
					{
						words = words + "ከ " + (int)monthremainder + " ቀን ";
					}
				}
			}

			return words;
		}
		public static string ToWordAmharic(this int number)
		{
			if (number == 0)
				return "ዜሮ";

			if (number < 0)
				return "ኔጌቲቭ " + Math.Abs(number).ToWordAmharic();

			string words = "";

			if ((number / 1000000) > 0)
			{
				words += (number / 1000000).ToWordAmharic().TrimEnd() + " ሚሊዮን ";
				number %= 1000000;
			}

			if ((number / 1000) > 0)
			{
				words += (number / 1000).ToWordAmharic().TrimEnd() + " ሺ ";
				number %= 1000;
			}

			if ((number / 100) > 0)
			{
				words += (number / 100).ToWordAmharic().TrimEnd() + " መቶ ";
				number %= 100;
			}

			if (number > 0)
			{
				if (words != "")
					words += " ";

				var unitsMap = new[] { "ዜሮ", "አንድ", "ሁለት", "ሶስት", "አራት", "አምስት", "ስድስት", "ሰባት", "ስምንት", "ዘጠኝ", "አስር", "አስራ አንድ", "አስራ ሁለት", "አስራ ሶስት", "አስራ አራት", "አስራ አምስት", "አስራ ስድስት", "አስራ ሰባት", "አስራ ስምንት", "አስራ ዘጠኝ" };
				var tensMap = new[] { "ዜሮ", "አስር", "ሃያ", "ሰላሳ", "አርባ", "ሃምሳ", "ስልሳ", "ሰባ", "ሰማኒያ", "ዘጠና" };

				if (number < 20)
					words += unitsMap[number];
				else
				{
					words += tensMap[number / 10];
					if ((number % 10) > 0)
						words += " " + unitsMap[number % 10];
				}
			}

			return words;
		}
        public static string ToWordEnglish(this int number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "- " + Math.Abs(number).ToWordAmharic();

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += (number / 1000000).ToWordAmharic().TrimEnd() + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += (number / 1000).ToWordAmharic().TrimEnd() + "Thousand  ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += (number / 100).ToWordAmharic().TrimEnd() + " Hundred";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += " ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "eleven", "Tweleve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "seventeen", "Eighteen", "Twenty" };
                var tensMap = new[] { "Zero", "Ten", "Tweenty", "Thirty", "Fourty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words;
        }

        public static string ToWordAfaanOromo(this int number)
        {
            if (number == 0)
                return "Sifira"; // or "Zero" if you prefer English form

            if (number < 0)
                return "Netee " + Math.Abs(number).ToWordAfaanOromo(); // “Netee” = negative

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += (number / 1000000).ToWordAfaanOromo().TrimEnd() + " Miliyoona ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += (number / 1000).ToWordAfaanOromo().TrimEnd() + " Kuma ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += (number / 100).ToWordAfaanOromo().TrimEnd() + " Dhibba ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += " ";

                var unitsMap = new[]
                {
                    "Sifira", "Tokko", "Lama", "Sadii", "Afur", "Shan", "Jaha",
                    "Torba", "Saddeet", "Sagaltama", "Kudhan",
                    "Kudha Tokko", "Kudha Lama", "Kudha Sadii", "Kudha Afur",
                    "Kudha Shan", "Kudha Jaha", "Kudha Torba", "Kudha Saddeet", "Kudha Sagaltama"

                };

                var tensMap = new[]
                {
                    "Sifira", "Kudhan", "Digdama", "Soddoma", "Afurtama", "Shantama",
                    "Jaatama", "Torbatama", "Saddeettama", "Sagaltama"

                };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }

        public static int GetEmployeePlacementBranch(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				int branchId = 0;
				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null)
				{
					branchId = placement.BranchId;

				}
				return branchId;
			}
		}

		public static int GetEmployeePlacementBusinessUnit(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				int businessUnitId = 0;
				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null)
				{
					businessUnitId = placement.BusinessUnitId;

				}
				return businessUnitId;
			}
		}
		//new 
		public static string GetEmployeePlacementBranchName(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var branch = db.Branch.Find(GetEmployeePlacementBranch(empid));


				if (branch != null)
				{
					return branch.Name;

				}
				return "";
			}
		}
		public static decimal GetEmployeeSalary(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null)
				{
					return placement.Salary;

				}
				return 0;
			}
		}
		public static string GetEmployeeWorkUnit(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null)
				{
					return GetWorkUnitName(placement.OrgStructureId);

				}
				return "";
			}
		}
		public static int GetEmployeeWorkUnitId(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

				if (placement != null)
				{
					return placement.OrgStructureId;

				}
				return 0;
			}
		}

		public static int GetEmployeeDepartmentIdByEmpId(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				int department = 0;

				var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empid && x.isCurrent);
				if (placment != null)
				{

					department = placment.OrgStructureId;
				}

				return department;
			}

		}
		public static string GetEmployeeDepartmentNameByEmpId(int empid)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				string department = "";

				var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empid && x.isCurrent);
				if (placment != null)
				{
					var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placment.OrgStructureId);
					if (mainCompany != null)
					{
						department = mainCompany.Name;
					}
				}

				return department;
			}

		}

		////////////////////////////////////
		public static List<Person_Employee> GetAllEmployeesByBranchAndWorkUnit(int branchId, int orgStructureId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var list = new List<Person_Employee>();
				var allEmployee = db.Person_Employee.ToList();
				var placements = db.Placements.Where(x => x.BranchId == branchId && x.OrgStructureId == orgStructureId).ToList();
				if (placements.Any())
				{
					var filterd = from f in placements
								  join e in allEmployee
								  on f.EmployeeId equals e.EmployeeId
								  where f.EmployeeId == e.EmployeeId
								  select (new { e });

					foreach (var p in filterd)
					{
						list.Add(p.e);
					}
				}
				return list;
			}

		}
		public static List<Person_Employee> GetAllEmployeesByBranch(int branchId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var list = new List<Person_Employee>();
				var allEmployee = db.Person_Employee.ToList();
				var placements = db.Placements.Where(x => x.BranchId == branchId).ToList();
				if (placements.Any())
				{
					foreach (var p in placements)
					{
						var check = allEmployee.FirstOrDefault(x => x.EmployeeId == p.EmployeeId);
						if (check != null)
						{
							list.Add(check);
						}
					}
				}
				return list;
			}
		}
		public static List<Person_Employee> GetAllEmployeesByWorkUnit(int orgStructureId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var list = new List<Person_Employee>();
				var allEmployee = db.Person_Employee.ToList();
				var placements = db.Placements.Where(x => x.OrgStructureId == orgStructureId).ToList();
				if (placements.Any())
				{
					foreach (var p in placements)
					{
						var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
						if (check != null)
						{
							list.Add(check);
						}
					}
				}
				return list;
			}
		}

		public static List<Person_Employee> GetEmployeesByBranchInPlacement(List<Person_Employee> employees, int branchId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var placements = db.Placements.Where(x => x.BranchId == branchId).DistinctByField(x => x.EmployeeId).ToList();
				if (placements.Any())
				{
					List<int> empIds = new List<int>();
					empIds.AddRange(placements.Select(x => x.EmployeeId));

					employees = employees.Where(x => empIds.Contains(x.EmployeeId)).ToList();

					return employees;
				}
				return null;
			}
		}

		public static List<Person_Employee> GetEmployeesByDepartment(List<Person_Employee> employees, int orgStructureId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var placements = db.Placements.Where(x => x.OrgStructureId == orgStructureId).DistinctByField(x => x.EmployeeId).ToList();
				if (placements.Any())
				{
					List<int> empIds = new List<int>();
					empIds.AddRange(placements.Select(x => x.EmployeeId));

					employees = employees.Where(x => empIds.Contains(x.EmployeeId)).ToList();

					return employees;
				}
				return null;
			}
		}


		///////// Data From Placement Table
		public static List<Placement> GetEmployees_ByBranch_FromPlacement(List<Placement> placements, int branchId)
		{
			placements = placements.Where(x => x.BranchId == branchId).ToList();

			return placements;
		}
		public static List<Placement> GetEmployees_ByDepartment_FromPlacement(List<Placement> placements, int orgId)
		{
			placements = placements.Where(x => x.OrgStructureId == orgId).ToList();

			return placements;
		}
		public static List<Placement> GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(List<Placement> placements, int orgId)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
				placements = placements.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();

				return placements;
			}
		}
		public static List<Placement> GetEmployees_ByJobTitle_FromPlacement(List<Placement> placements, int lookupId)
		{
			placements = placements.Where(x => x.LookupId == lookupId).ToList();

			return placements;
		}
		public static List<Placement> Get_Active_EmployeesFrom_Placement(List<Placement> placements)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var empIds = db.Person_Employee.Where(x => x.IsActive).Select(x => x.EmployeeId).ToList();
				placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

				return placements;
			}
		}
		public static List<Placement> Get_InActive_EmployeesFrom_Placement(List<Placement> placements)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var empIds = db.Person_Employee.Where(x => !x.IsActive).Select(x => x.EmployeeId).ToList();
				placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

				return placements;
			}
		}

		public static List<Placement> Get_Male_EmployeesFrom_Placement(List<Placement> placements)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var empIds = db.Person_Employee.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
				placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

				return placements;
			}
		}
		public static List<Placement> Get_Female_EmployeesFrom_Placement(List<Placement> placements)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				var empIds = db.Person_Employee.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();
				placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

				return placements;
			}
		}
		public static List<Placement> Get_Employees_ByGender_From_Placement(List<Placement> placements, string gender)
		{
			using (ExceedDbContext db = new ExceedDbContext())
			{

				if (gender.ToLower() == "male")
				{
					var empIds = db.Person_Employee.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
					placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
				}
				else if (gender.ToLower() == "female")
				{
					var empIds = db.Person_Employee.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();
					placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
				}

				return placements;
			}
		}
	}
}
