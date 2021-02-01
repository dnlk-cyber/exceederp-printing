namespace ExceedERP.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class contract4 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "EXCEED.ContractAttachement", newName: "ContractAttachment");
            RenameTable(name: "EXCEED.ContractPaymentAttachement", newName: "ContractPaymentAttachment");
            //DropPrimaryKey("EXCEED.ContractAttachment");
            //DropPrimaryKey("EXCEED.ContractPaymentAttachment");
         
            AddColumn("EXCEED.ContractAttachment", "ContractAttachmentId", c => c.Int(nullable: false, identity: true));
            AddColumn("EXCEED.ContractPaymentAttachment", "ContractPaymentAttachmentId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("EXCEED.ContractAttachment", "ContractAttachmentId");
            AddPrimaryKey("EXCEED.ContractPaymentAttachment", "ContractPaymentAttachmentId");
            //DropColumn("EXCEED.ContractAttachment", "ContractAttachementId");
            //DropColumn("EXCEED.ContractPaymentAttachment", "ContractPaymentAttachementId");

        }
        
        public override void Down()
        {
            AddColumn("EXCEED.ContractPaymentAttachment", "ContractPaymentAttachementId", c => c.Int(nullable: false, identity: true));
            AddColumn("EXCEED.ContractAttachment", "ContractAttachementId", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("EXCEED.ContractPaymentAttachment");
            DropPrimaryKey("EXCEED.ContractAttachment");
            DropColumn("EXCEED.ContractPaymentAttachment", "ContractPaymentAttachmentId");
            DropColumn("EXCEED.ContractAttachment", "ContractAttachmentId");
            AddPrimaryKey("EXCEED.ContractPaymentAttachment", "ContractPaymentAttachementId");
            AddPrimaryKey("EXCEED.ContractAttachment", "ContractAttachementId");
            RenameTable(name: "EXCEED.ContractPaymentAttachment", newName: "ContractPaymentAttachement");
            RenameTable(name: "EXCEED.ContractAttachment", newName: "ContractAttachement");
        }
    }
}
