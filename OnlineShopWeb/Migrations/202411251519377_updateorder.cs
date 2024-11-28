namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateorder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "RecipientName", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Orders", "RecipientPhoneNumber", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "RecipientAddress", c => c.String(nullable: false, maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "RecipientAddress");
            DropColumn("dbo.Orders", "RecipientPhoneNumber");
            DropColumn("dbo.Orders", "RecipientName");
        }
    }
}
