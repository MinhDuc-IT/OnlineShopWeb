namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "OrderCode");
            DropColumn("dbo.Orders", "OrderNotes");
            DropColumn("dbo.Orders", "PaymentMethod");
            DropColumn("dbo.Orders", "PaymentStatus");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "PaymentStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "PaymentMethod", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "OrderNotes", c => c.String());
            AddColumn("dbo.Orders", "OrderCode", c => c.String(maxLength: 50));
        }
    }
}
