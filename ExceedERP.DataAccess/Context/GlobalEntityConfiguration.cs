using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.DataAccess.Context
{
        public  class GlobalEntityConfiguration
    {

        public class UserBusinessUnitMapEntityTypeConfiguration : EntityTypeConfiguration<UserBusinessUnitMap>
        {

            public UserBusinessUnitMapEntityTypeConfiguration()
            {
                this.ToTable("UserBusinessUnitMap", "Global");
                this.HasKey<int>(t => t.UserBusinessUnitMapId);

            }
        }

        public class DocumentApprovalRuleEntityTypeConfiguration : EntityTypeConfiguration<DocumentApprovalRule>
            {

                public DocumentApprovalRuleEntityTypeConfiguration()
                {
                    this.ToTable("DocumentApprovalRule", "Global");
                    this.HasKey<int>(t => t.DocumentApprovalRuleId);

                }
            }

            public class ApprovalGroupEntityTypeConfiguration : EntityTypeConfiguration<ApprovalGroup>
            {

                public ApprovalGroupEntityTypeConfiguration()
                {
                    this.ToTable("ApprovalGroup", "Global");
                    this.HasKey<int>(t => t.ApprovalGroupId);

                }
            }
      
            public class ApprovalGroupRuleEntityTypeConfiguration : EntityTypeConfiguration<ApprovalGroupRule>
        {

            public ApprovalGroupRuleEntityTypeConfiguration()
            {
                this.ToTable("ApprovalGroupRule", "Global");
                this.HasKey<int>(t => t.ApprovalGroupRuleId);

            }
        }


            public class AssignApprovalGroupEntityTypeConfiguration : EntityTypeConfiguration<AssignApprovalGroup>
            {

                public AssignApprovalGroupEntityTypeConfiguration()
                {
                    this.ToTable("AssignApprovalGroup", "Global");
                    this.HasKey<int>(t => t.AssignApprovalGroupId);

                }
            }


            public class AssignApprovalGroupAssignmentEntityTypeConfiguration : EntityTypeConfiguration<AssignApprovalGroupAssignment>
            {

                public AssignApprovalGroupAssignmentEntityTypeConfiguration()
                {
                    this.ToTable("AssignApprovalGroupAssignment", "Global");
                    this.HasKey<int>(t => t.AssignApprovalGroupAssignmentId);

                }
            }

            public class GlobalRuleValueEntityTypeConfiguration : EntityTypeConfiguration<GlobalRuleValue>
            {

                public GlobalRuleValueEntityTypeConfiguration()
                {
                    this.ToTable("GlobalRuleValue", "Global");
                    this.HasKey<int>(t => t.GlobalRuleValueId);

                }
            }
        

          
    }
}
