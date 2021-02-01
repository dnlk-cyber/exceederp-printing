namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inven6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("EXCEED.Revaluation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropForeignKey("EXCEED.ReEvaluationItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.ReEvaluation", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.ReEvaluationItem", "ReEvaluationId", "EXCEED.ReEvaluation");
            DropForeignKey("EXCEED.ReEvaluation", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.ReEvaluationValidation", "ReEvaluationId", "EXCEED.ReEvaluation");
            DropIndex("EXCEED.Revaluation", new[] { "AssetRegistrationId" });
            DropIndex("EXCEED.ReEvaluationItem", new[] { "ReEvaluationId" });
            DropIndex("EXCEED.ReEvaluationItem", new[] { "ItemCode" });
            DropIndex("EXCEED.ReEvaluation", new[] { "StoreCode" });
            DropIndex("EXCEED.ReEvaluation", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.ReEvaluationValidation", new[] { "ReEvaluationId" });
            //CreateTable(
            //    "EXCEED.FixedAssetRevaluation",
            //    c => new
            //        {
            //            FixedAssetRevaluationID = c.Int(nullable: false, identity: true),
            //            RegistrationID = c.Int(nullable: false),
            //            AddedValue = c.Decimal(nullable: false, precision: 18, scale: 2),
            //            Description = c.String(),
            //            RevaluationDate = c.DateTime(nullable: false),
            //            OrgBranchName = c.String(),
            //            DateCreated = c.DateTime(),
            //            LastModified = c.DateTime(),
            //            CreatedBy = c.String(),
            //            ModifiedBy = c.String(),
            //        })
            //    .PrimaryKey(t => t.FixedAssetRevaluationID)
            //    .ForeignKey("EXCEED.AssetRegistration", t => t.RegistrationID, cascadeDelete: true)
            //    .Index(t => t.RegistrationID);
            
            CreateTable(
                "EXCEED.RevaluationItem",
                c => new
                    {
                        RevaluationItemId = c.Int(nullable: false, identity: true),
                        RevaluationId = c.Int(nullable: false),
                        ItemCode = c.String(maxLength: 128),
                        PreviousPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NewPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Boolean(nullable: false),
                        ItemCategoryCode = c.String(),
                    })
                .PrimaryKey(t => t.RevaluationItemId)
                .ForeignKey("EXCEED.InventoryItem", t => t.ItemCode)
                .ForeignKey("EXCEED.Revaluation", t => t.RevaluationId, cascadeDelete: true)
                .Index(t => t.RevaluationId)
                .Index(t => t.ItemCode);
            
            CreateTable(
                "EXCEED.RevaluationValidation",
                c => new
                    {
                        RevaluationValidationId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        Remark = c.String(),
                        ReEvaluationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                        Revaluation_RevaluationId = c.Int(),
                    })
                .PrimaryKey(t => t.RevaluationValidationId)
                .ForeignKey("EXCEED.Revaluation", t => t.Revaluation_RevaluationId)
                .Index(t => t.Revaluation_RevaluationId);
            
            AddColumn("EXCEED.Revaluation", "StoreCode", c => c.String(maxLength: 128));
            AddColumn("EXCEED.Revaluation", "ItemCategoryCode", c => c.String(maxLength: 128));
            AddColumn("EXCEED.Revaluation", "Status", c => c.String());
            AddColumn("EXCEED.Revaluation", "Remark", c => c.String());
            AddColumn("EXCEED.Revaluation", "IsVoid", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Revaluation", "VoidBy", c => c.String());
            AddColumn("EXCEED.Revaluation", "VoidTime", c => c.DateTime());
            AddColumn("EXCEED.Revaluation", "IsOnlineApproved", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Revaluation", "OnlineApprovedBy", c => c.String());
            AddColumn("EXCEED.Revaluation", "OnlineApprovedTime", c => c.DateTime());
            AddColumn("EXCEED.Revaluation", "IsOnlineTransferred", c => c.Boolean(nullable: false));
            AddColumn("EXCEED.Revaluation", "IsOnlineTransferredBy", c => c.String());
            AddColumn("EXCEED.Revaluation", "OnlineTransferredTime", c => c.DateTime());
            //AddColumn("EXCEED.Revaluation", "IsSendForApproval", c => c.Boolean(nullable: false));
            //AddColumn("EXCEED.Revaluation", "SendForApprovalBy", c => c.String());
            //AddColumn("EXCEED.Revaluation", "SendForApprovalTime", c => c.DateTime());
            CreateIndex("EXCEED.Revaluation", "StoreCode");
            CreateIndex("EXCEED.Revaluation", "ItemCategoryCode");
            AddForeignKey("EXCEED.Revaluation", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode");
            AddForeignKey("EXCEED.Revaluation", "StoreCode", "EXCEED.InventoryStore", "StoreCode");
            //DropColumn("EXCEED.Revaluation", "AssetRegistrationId");
            DropColumn("EXCEED.Revaluation", "AddedValue");
            DropColumn("EXCEED.Revaluation", "Description");
            DropColumn("EXCEED.Revaluation", "RevaluationDate");
            DropTable("EXCEED.ReEvaluationItem");
            DropTable("EXCEED.ReEvaluation");
            DropTable("EXCEED.ReEvaluationValidation");
        }
        
        public override void Down()
        {
            CreateTable(
                "EXCEED.ReEvaluationValidation",
                c => new
                    {
                        ReEvaluationValidationId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        Remark = c.String(),
                        ReEvaluationId = c.Int(nullable: false),
                        OrgBranchName = c.String(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ReEvaluationValidationId);
            
            CreateTable(
                "EXCEED.ReEvaluation",
                c => new
                    {
                        ReEvaluationId = c.Int(nullable: false, identity: true),
                        StoreCode = c.String(maxLength: 128),
                        ItemCategoryCode = c.String(maxLength: 128),
                        Status = c.String(),
                        Remark = c.String(),
                        IsVoid = c.Boolean(nullable: false),
                        VoidBy = c.String(),
                        VoidTime = c.DateTime(),
                        IsOnlineApproved = c.Boolean(nullable: false),
                        OnlineApprovedBy = c.String(),
                        OnlineApprovedTime = c.DateTime(),
                        IsOnlineTransferred = c.Boolean(nullable: false),
                        IsOnlineTransferredBy = c.String(),
                        OnlineTransferredTime = c.DateTime(),
                        IsSendForApproval = c.Boolean(nullable: false),
                        OrgBranchName = c.String(),
                        SendForApprovalBy = c.String(),
                        SendForApprovalTime = c.DateTime(),
                        DateCreated = c.DateTime(),
                        LastModified = c.DateTime(),
                        CreatedBy = c.String(),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.ReEvaluationId);
            
            CreateTable(
                "EXCEED.ReEvaluationItem",
                c => new
                    {
                        ReEvaluationItemId = c.Int(nullable: false, identity: true),
                        ReEvaluationId = c.Int(nullable: false),
                        ItemCode = c.String(maxLength: 128),
                        PreviousPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NewPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Boolean(nullable: false),
                        ItemCategoryCode = c.String(),
                    })
                .PrimaryKey(t => t.ReEvaluationItemId);
            
            AddColumn("EXCEED.Revaluation", "RevaluationDate", c => c.DateTime(nullable: false));
            AddColumn("EXCEED.Revaluation", "Description", c => c.String());
            AddColumn("EXCEED.Revaluation", "AddedValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("EXCEED.Revaluation", "AssetRegistrationId", c => c.Int(nullable: false));
            DropForeignKey("EXCEED.RevaluationValidation", "Revaluation_RevaluationId", "EXCEED.Revaluation");
            DropForeignKey("EXCEED.Revaluation", "StoreCode", "EXCEED.InventoryStore");
            DropForeignKey("EXCEED.RevaluationItem", "RevaluationId", "EXCEED.Revaluation");
            DropForeignKey("EXCEED.Revaluation", "ItemCategoryCode", "EXCEED.ItemCategory");
            DropForeignKey("EXCEED.RevaluationItem", "ItemCode", "EXCEED.InventoryItem");
            DropForeignKey("EXCEED.FixedAssetRevaluation", "AssetRegistrationId", "EXCEED.AssetRegistration");
            DropIndex("EXCEED.RevaluationValidation", new[] { "Revaluation_RevaluationId" });
            DropIndex("EXCEED.Revaluation", new[] { "ItemCategoryCode" });
            DropIndex("EXCEED.Revaluation", new[] { "StoreCode" });
            DropIndex("EXCEED.RevaluationItem", new[] { "ItemCode" });
            DropIndex("EXCEED.RevaluationItem", new[] { "RevaluationId" });
            DropIndex("EXCEED.FixedAssetRevaluation", new[] { "AssetRegistrationId" });
            DropColumn("EXCEED.Revaluation", "SendForApprovalTime");
            DropColumn("EXCEED.Revaluation", "SendForApprovalBy");
            DropColumn("EXCEED.Revaluation", "IsSendForApproval");
            DropColumn("EXCEED.Revaluation", "OnlineTransferredTime");
            DropColumn("EXCEED.Revaluation", "IsOnlineTransferredBy");
            DropColumn("EXCEED.Revaluation", "IsOnlineTransferred");
            DropColumn("EXCEED.Revaluation", "OnlineApprovedTime");
            DropColumn("EXCEED.Revaluation", "OnlineApprovedBy");
            DropColumn("EXCEED.Revaluation", "IsOnlineApproved");
            DropColumn("EXCEED.Revaluation", "VoidTime");
            DropColumn("EXCEED.Revaluation", "VoidBy");
            DropColumn("EXCEED.Revaluation", "IsVoid");
            DropColumn("EXCEED.Revaluation", "Remark");
            DropColumn("EXCEED.Revaluation", "Status");
            DropColumn("EXCEED.Revaluation", "ItemCategoryCode");
            DropColumn("EXCEED.Revaluation", "StoreCode");
            DropTable("EXCEED.RevaluationValidation");
            DropTable("EXCEED.RevaluationItem");
            DropTable("EXCEED.FixedAssetRevaluation");
            CreateIndex("EXCEED.ReEvaluationValidation", "ReEvaluationId");
            CreateIndex("EXCEED.ReEvaluation", "ItemCategoryCode");
            CreateIndex("EXCEED.ReEvaluation", "StoreCode");
            CreateIndex("EXCEED.ReEvaluationItem", "ItemCode");
            CreateIndex("EXCEED.ReEvaluationItem", "ReEvaluationId");
            CreateIndex("EXCEED.Revaluation", "AssetRegistrationId");
            AddForeignKey("EXCEED.ReEvaluationValidation", "ReEvaluationId", "EXCEED.ReEvaluation", "ReEvaluationId", cascadeDelete: true);
            AddForeignKey("EXCEED.ReEvaluation", "StoreCode", "EXCEED.InventoryStore", "StoreCode");
            AddForeignKey("EXCEED.ReEvaluationItem", "ReEvaluationId", "EXCEED.ReEvaluation", "ReEvaluationId", cascadeDelete: true);
            AddForeignKey("EXCEED.ReEvaluation", "ItemCategoryCode", "EXCEED.ItemCategory", "ItemCategoryCode");
            AddForeignKey("EXCEED.ReEvaluationItem", "ItemCode", "EXCEED.InventoryItem", "ItemCode");
            AddForeignKey("EXCEED.Revaluation", "AssetRegistrationId", "EXCEED.AssetRegistration", "AssetRegistrationId", cascadeDelete: true);
        }
    }
}
