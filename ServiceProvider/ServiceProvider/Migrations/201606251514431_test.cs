namespace ServiceProvider.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "message_Id", "dbo.Messages");
            DropIndex("dbo.Users", new[] { "message_Id" });
            AddColumn("dbo.Messages", "User_Id", c => c.Int());
            AddForeignKey("dbo.Messages", "User_Id", "dbo.Users", "Id");
            CreateIndex("dbo.Messages", "User_Id");
            DropColumn("dbo.Users", "message_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "message_Id", c => c.Int());
            DropIndex("dbo.Messages", new[] { "User_Id" });
            DropForeignKey("dbo.Messages", "User_Id", "dbo.Users");
            DropColumn("dbo.Messages", "User_Id");
            CreateIndex("dbo.Users", "message_Id");
            AddForeignKey("dbo.Users", "message_Id", "dbo.Messages", "Id");
        }
    }
}
