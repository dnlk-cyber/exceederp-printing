using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource
{
	[DataObject]
	public class CommonOds
	{
		readonly ExceedDbContext _db = new ExceedDbContext();
		readonly AccountHelper _accountHelper = new AccountHelper();

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<PaymentMethod> PaymentMethods()
		{

			return _db.PaymentMethods.ToList();
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<string> JournalCreateByUsers()
		{

			List<string> list = new List<string>();
			var users = _db.GLRecordJournals
			   // .Where(x => x.IsOnlineApproved && x.isPosted && !x.isVoid)
				.OrderBy(x => x.CreatedBy).Select(x => x.CreatedBy).Distinct().ToList();
			list.AddRange(users);
			return list;
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<string> PostedByUsers()
		{

			List<string> list = new List<string>();
			var users = _db.GLRecordJournals
				.Where(x => x.IsOnlineApproved && x.isPosted && !x.isVoid).OrderBy(x => x.PostedBy).Select(x => x.PostedBy).Distinct().ToList();
			list.AddRange(users);
			return list;
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<string> JournalCategory()
		{

			List<string> list = new List<string>();
			//list.AddRange(_db.GLJournalCategoriers.Select(x => x.Category).ToList());
			var categories = _db.GLRecordJournals.OrderBy(x => x.CategoryName).Select(x => x.CategoryName).Distinct().ToList();
			list.AddRange(categories);
			return list;
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<string> JournalSources()
		{

			List<string> list = new List<string>();
			list.AddRange(_db.GLJournalSources.Select(x => x.Sources).ToList());
			return list;
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<AccountSegmentVm> GetAccountList()
		{
			return _db.GLChartOfAccountAccounts.Select(x=> new AccountSegmentVm
			{
				Values=x.Values,
				Description=x.Description
			}).ToList();
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<SegmentValueVm> GetSubAccountList()
		{
			List<SegmentValueVm> list = new List<SegmentValueVm>();

			if (_accountHelper.IsDynamicCoA())
			{
				var dynamicsub = _db.GlChartOfAccountSegments
					.Where(x=>x.GlSegmentSetup.SegmentType==DynamicSegmentTypes.SubAccount)
					.Select(x => new {x.Values, x.Description, x.AllowBudgeting,x.HasParent, x.Parent, x.AllowPosting,x.Enable }).OrderBy(x => x.Values).ToList();
				foreach (var value in dynamicsub)
				{
					var v = new SegmentValueVm
					{
						Values = value.Values,
						Description = value.Description,
						HasParent = value.HasParent,
						Parent = value.Parent,
						Enable = value.Enable,
						AllowBudgeting = value.AllowBudgeting,
						AllowPosting = value.AllowPosting
					};

					list.Add(v);
				}
			}
			else
			{
				//var staticsub = _db.GLChartOfAccountSubAccounts
				//    .Select(x => new { x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.Parent, x.AllowPosting, x.Enable }).OrderBy(x=>x.Values).ToList();
				//foreach (var value in staticsub)
				//{
				//    var v = new SegmentValueVm
				//    {
				//        Values = value.Values,
				//        Description = value.Description,
				//        HasParent = value.HasParent,
				//        Parent = value.Parent,
				//        Enable = value.Enable,
				//        AllowBudgeting = value.AllowBudgeting,
				//        AllowPosting = value.AllowPosting
				//    };

				//    list.Add(v);
				//}
			}
		   
			return list;
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<SegmentValueVm> GetCostCenterList()
		{
			List<SegmentValueVm> list = new List<SegmentValueVm>();

			if (_accountHelper.IsDynamicCoA())
			{
				var dynamicsub = _db.GlChartOfAccountSegments
					.Where(x => x.GlSegmentSetup.SegmentType == DynamicSegmentTypes.CostCenter)
					.Select(x => new { x.Values, x.Description, x.AllowBudgeting, x.HasParent,x.Parent, x.AllowPosting,x.Enable }).ToList();
				foreach (var value in dynamicsub)
				{
					var v = new SegmentValueVm
					{
						Values = value.Values,
						Description = value.Description,
						HasParent = value.HasParent,
						Parent = value.Parent,
						Enable = value.Enable,
						AllowBudgeting = value.AllowBudgeting,
						AllowPosting = value.AllowPosting
					};

					list.Add(v);
				}
			}
			else
			{
				//var staticsub = _db.GLChartOfAccountCostCenters.ToList();
				//foreach (var value in staticsub)
				//{
				//    var v = new SegmentValueVm
				//    {
				//        Values = value.Values,
				//        Description = value.Description,
				//        HasParent = value.HasParent,
				//        Parent = value.Parent,
				//        Enable = value.Enable,
				//        AllowBudgeting = value.AllowBudgeting,
				//        AllowPosting = value.AllowPosting
				//    };

				//    list.Add(v);
				//}
			}

			return list;
		}
		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<SegmentValueVm> GetBizUnitAccountList()
		{
			List<SegmentValueVm> list = new List<SegmentValueVm>();

			if (_accountHelper.IsDynamicCoA())
			{
				var dynamicsub = _db.GlChartOfAccountSegments
					.Where(x => x.GlSegmentSetup.SegmentType == DynamicSegmentTypes.BusinessUnit)
					.Select(x => new { x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.Parent, x.AllowPosting, x.Enable }).ToList();
				foreach (var value in dynamicsub)
				{
					var v = new SegmentValueVm
					{
						Values = value.Values,
						Description = value.Description,
						HasParent = value.HasParent,
						Parent = value.Parent,
						Enable = value.Enable,
						AllowBudgeting = value.AllowBudgeting,
						AllowPosting = value.AllowPosting
					};

					list.Add(v);
				}
			}
			//else
			//{
			//    var staticsub = _db.GLChartOfAccountSubAccounts.ToList();
			//    foreach (var value in staticsub)
			//    {
			//        var v = new SegmentValueVm
			//        {
			//            Values = value.Values,
			//            Description = value.Description,
			//            HasParent = value.HasParent,
			//            Parent = value.Parent,
			//            Enable = value.Enable,
			//            AllowBudgeting = value.AllowBudgeting,
			//            AllowPosting = value.AllowPosting
			//        };

			//        list.Add(v);
			//    }
			//}

			return list;
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<SegmentTypeVm> GeSegmentTypeList()
		{
			List<SegmentTypeVm> list = new List<SegmentTypeVm>();
			if (_accountHelper.IsDynamicCoA())
			{
				var segment = _db.GlSegmentSetups
				//.Where(x=>x.SegmentType!= DynamicSegmentTypes.NaturalAccount)
				.ToList();
				foreach (var segmentSetup in segment)
				{
					var type = new SegmentTypeVm
					{
						Name = segmentSetup.Name,
						Description = segmentSetup.Description
					};
					list.Add(type);
				}

			}
			else
			{
				//var type = new SegmentTypeVm
				//{
				//    Name = "Branch"
				//};
				//list.Add(type);
				// type = new SegmentTypeVm
				//{
				//    Name = "Cost Center"
				//};
				//list.Add(type);
				// type = new SegmentTypeVm
				//{
				//    Name = "Sub Account"
				//};
				//list.Add(type);
			}

			return list;
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<SegmentValueVm> GeSegmentValueList(string type)
		{
			List<SegmentValueVm> list = new List<SegmentValueVm>();
			if (_accountHelper.IsDynamicCoA())
			{
				if (!String.IsNullOrWhiteSpace(type))
				{
					var segmentType = _db.GlSegmentSetups.FirstOrDefault(x => x.Name == type);
					var values = _db.GlChartOfAccountSegments
						.Where(x => x.GLSegmentSetupId == segmentType.GLSegmentSetupId)
						.Select(x => new { x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.Parent, x.AllowPosting, x.Enable })
						.ToList();

					foreach (var value in values)
					{
						var v = new SegmentValueVm
						{
							Values = value.Values,
							Description = value.Description,
							HasParent = value.HasParent,
							Parent = value.Parent,
							Enable = value.Enable,
							AllowBudgeting = value.AllowBudgeting,
							AllowPosting = value.AllowPosting
						};

						list.Add(v);
					}
				}
			}

			else
			{
				//switch (type)
				//{
				//    case "Branch":
				//        var accountLocations = _db.GLChartOfAccountLocations
				//            .Select(x => new { x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.Parent, x.AllowPosting, x.Enable }).ToList();
				//        foreach (var value in accountLocations)
				//        {
				//            var v = new SegmentValueVm
				//            {
				//                Values = value.Values,
				//                Description = value.Description,
				//                HasParent = value.HasParent,
				//                Parent = value.Parent,
				//                Enable = value.Enable,
				//                AllowBudgeting = value.AllowBudgeting,
				//                AllowPosting = value.AllowPosting
				//            };

				//            list.Add(v);
				//        }

				//        break;

				//    case "Cost Center":
				//        var accountCostCenters = _db.GLChartOfAccountCostCenters
				//            .Select(x => new { x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.Parent, x.AllowPosting, x.Enable }).ToList();
				//        foreach (var value in accountCostCenters)
				//        {
				//            var v = new SegmentValueVm
				//            {
				//                Values = value.Values,
				//                Description = value.Description,
				//                HasParent = value.HasParent,
				//                Parent = value.Parent,
				//                Enable = value.Enable,
				//                AllowBudgeting = value.AllowBudgeting,
				//                AllowPosting = value.AllowPosting
				//            };

				//            list.Add(v);
				//        }

				//        break;

				//    case "Sub Account":
				//        var accountSubAccounts = _db.GLChartOfAccountSubAccounts.Select(x => new { x.Values, x.Description, x.AllowBudgeting, x.HasParent, x.Parent, x.AllowPosting, x.Enable }).ToList();
				//        foreach (var value in accountSubAccounts)
				//        {
				//            var v = new SegmentValueVm
				//            {
				//                Values = value.Values,
				//                Description = value.Description,
				//                HasParent = value.HasParent,
				//                Parent = value.Parent,
				//                Enable = value.Enable,
				//                AllowBudgeting = value.AllowBudgeting,
				//                AllowPosting = value.AllowPosting
				//            };

				//            list.Add(v);
				//        }

				//        break;
				//}



			}

			return list;
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<string> BankReconciliationStatus()
		{

			List<string> list = new List<string> {"All", "Reconciled", "Unreconciled"};
			return list;
		}

		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<SegmentTypeVm> GetJournalPostStatus()
		{
			List<SegmentTypeVm> list = new List<SegmentTypeVm>();

			var type = new SegmentTypeVm
			{
				Name = "POSTED"
			};
			list.Add(type);
			type = new SegmentTypeVm
			{
				Name = "NOT POSTED"
			};
			list.Add(type);
		   
			list.Add(type);

			return list;
		}
		[DataObjectMethod(DataObjectMethodType.Select)]
		public List<SegmentTypeVm> GetJournalApprovalStatus()
		{
			List<SegmentTypeVm> list = new List<SegmentTypeVm>();

			var type = new SegmentTypeVm
			{
				Name = "APPROVED"
			};
			list.Add(type);
			type = new SegmentTypeVm
			{
				Name = "NOT APPROVED"
			};
			list.Add(type);

			list.Add(type);

			return list;
		}
	}

	public class AccountSegmentVm
	{
		public string Values { get; set; }
		public string Description { get; set; }
	}
}
