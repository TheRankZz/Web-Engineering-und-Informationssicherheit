namespace ServiceProvider.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        sender = c.String(),
                        cipher = c.String(),
                        iv = c.String(),
                        key_recipient_enc = c.String(),
                        sig_recipient = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        username = c.String(),
                        salt_masterkey = c.String(),
                        privkey_enc = c.String(),
                        pubkey = c.String(),
                        message_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Messages", t => t.message_Id)
                .Index(t => t.message_Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "message_Id" });
            DropForeignKey("dbo.Users", "message_Id", "dbo.Messages");
            DropTable("dbo.Users");
            DropTable("dbo.Messages");
        }
    }
}
