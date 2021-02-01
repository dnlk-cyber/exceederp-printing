namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class procurement20 : DbMigration
    {
        public override void Up()
        {
            AddColumn("EXCEED.AnnualNeedAssesmentItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.BidAwardedSupplierItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.TenderEvaluationItemResult", "ModelNo", c => c.String());
            AddColumn("EXCEED.LotItems", "ModelNo", c => c.String());
            AddColumn("EXCEED.ParticipantsItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.ContractManagementItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseFollowUpItems", "Name", c => c.String());
            AddColumn("EXCEED.ForeignPurchaseFollowUpItems", "ModelNo", c => c.String());
            AddColumn("EXCEED.ParticipatingSupplierItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.ProformaEvaluationResult", "ModelNo", c => c.String());
            AddColumn("EXCEED.ProformaItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.WinningSupplierItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.PurchaseOrderItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.ProcurementPlanItem", "ModelNo", c => c.String());
            AddColumn("EXCEED.ProformPassFail", "ModelNo", c => c.String());
            AddColumn("EXCEED.ProformTechnical", "ModelNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("EXCEED.ProformTechnical", "ModelNo");
            DropColumn("EXCEED.ProformPassFail", "ModelNo");
            DropColumn("EXCEED.ProcurementPlanItem", "ModelNo");
            DropColumn("EXCEED.PurchaseOrderItem", "ModelNo");
            DropColumn("EXCEED.WinningSupplierItem", "ModelNo");
            DropColumn("EXCEED.ProformaItem", "ModelNo");
            DropColumn("EXCEED.ProformaEvaluationResult", "ModelNo");
            DropColumn("EXCEED.ParticipatingSupplierItem", "ModelNo");
            DropColumn("EXCEED.ForeignPurchaseFollowUpItems", "ModelNo");
            DropColumn("EXCEED.ForeignPurchaseFollowUpItems", "Name");
            DropColumn("EXCEED.ContractManagementItem", "ModelNo");
            DropColumn("EXCEED.ParticipantsItem", "ModelNo");
            DropColumn("EXCEED.LotItems", "ModelNo");
            DropColumn("EXCEED.TenderEvaluationItemResult", "ModelNo");
            DropColumn("EXCEED.BidAwardedSupplierItem", "ModelNo");
            DropColumn("EXCEED.AnnualNeedAssesmentItem", "ModelNo");
        }
    }
}
