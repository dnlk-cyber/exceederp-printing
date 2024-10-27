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
            return _db.GLRecordJournals
                    .OrderBy(x => x.CreatedBy)
                    .Select(x => x.CreatedBy)
                    .Distinct()
                    .ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> PostedByUsers()
        {
            return _db.GLRecordJournals
            .Where(x => x.IsOnlineApproved && x.isPosted && !x.isVoid)
            .OrderBy(x => x.PostedBy)
            .Select(x => x.PostedBy)
            .Distinct()
            .ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> JournalCategory()
        {
            return _db.GLRecordJournals
            .OrderBy(x => x.CategoryName)
            .Select(x => x.CategoryName)
            .Distinct()
            .ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> JournalSources()
        {
            return _db.GLJournalSources
                    .Select(x => x.Sources)
                    .ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccountSegmentVm> GetAccountList()
        {
            return _db.GLChartOfAccountAccounts.Select(x => new AccountSegmentVm
            {
                Values = x.Values,
                Description = x.Description
            }).ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SegmentValueVm> GetSubAccountList()
        {
            return _db.GlChartOfAccountSegments
                .Where(x => x.GlSegmentSetup.SegmentType == DynamicSegmentTypes.SubAccount)
                .OrderBy(x => x.Values)
                .Select(x => new SegmentValueVm
                {
                    Values = x.Values,
                    Description = x.Description,
                    HasParent = x.HasParent,
                    Parent = x.Parent,
                    Enable = x.Enable,
                    AllowBudgeting = x.AllowBudgeting,
                    AllowPosting = x.AllowPosting
                })
                .ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SegmentValueVm> GetCostCenterList()
        {
            return _db.GlChartOfAccountSegments
             .Where(x => x.GlSegmentSetup.SegmentType == DynamicSegmentTypes.CostCenter)
             .Select(x => new SegmentValueVm
             {
                 Values = x.Values,
                 Description = x.Description,
                 HasParent = x.HasParent,
                 Parent = x.Parent,
                 Enable = x.Enable,
                 AllowBudgeting = x.AllowBudgeting,
                 AllowPosting = x.AllowPosting
             })
             .ToList();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SegmentValueVm> GetBizUnitAccountList()
        {
            return _db.GlChartOfAccountSegments
            .Where(x => x.GlSegmentSetup.SegmentType == DynamicSegmentTypes.BusinessUnit)
            .Select(x => new SegmentValueVm
            {
                Values = x.Values,
                Description = x.Description,
                HasParent = x.HasParent,
                Parent = x.Parent,
                Enable = x.Enable,
                AllowBudgeting = x.AllowBudgeting,
                AllowPosting = x.AllowPosting
            })
            .ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NameDescVm> GeSegmentTypeList()
        {
            return _db.GlSegmentSetups
               //.Where(x => x.SegmentType != DynamicSegmentTypes.NaturalAccount)
               .Select(segmentSetup => new NameDescVm
               {
                   Name = segmentSetup.Name,
                   Description = segmentSetup.Description
               })
               .ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SegmentValueVm> GeSegmentValueList(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return new List<SegmentValueVm>(); 
            }

            var segmentType = _db.GlSegmentSetups
                .Where(x => x.Name == type)
                .Select(x => x.GLSegmentSetupId)
                .FirstOrDefault();

            if (segmentType == 0) 
            {
                return new List<SegmentValueVm>(); 
            }

            return _db.GlChartOfAccountSegments
                .Where(x => x.GLSegmentSetupId == segmentType)
                .Select(x => new SegmentValueVm
                {
                    Values = x.Values,
                    Description = x.Description,
                    HasParent = x.HasParent,
                    Parent = x.Parent,
                    Enable = x.Enable,
                    AllowBudgeting = x.AllowBudgeting,
                    AllowPosting = x.AllowPosting
                })
                .ToList(); 
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> BankReconciliationStatus()
        {
            return new List<string> { "All", "Reconciled", "Unreconciled" };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NameDescVm> GetJournalPostStatus()
        {
            return new List<NameDescVm>
            {
                new NameDescVm { Name = "POSTED" },
                new NameDescVm { Name = "NOT POSTED" }
            };
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NameDescVm> GetJournalApprovalStatus()
        {

            return new List<NameDescVm>
            {
                new NameDescVm { Name = "APPROVED" },
                new NameDescVm { Name = "NOT APPROVED" }
            };

        }
    }

    public class AccountSegmentVm
    {
        public string Values { get; set; }
        public string Description { get; set; }
    }
}
