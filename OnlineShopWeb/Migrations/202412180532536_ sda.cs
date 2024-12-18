namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sda : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderCode", c => c.String(maxLength: 50));
            AddColumn("dbo.Orders", "OrderNotes", c => c.String());
            AddColumn("dbo.Orders", "PaymentMethod", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "PaymentStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PaymentStatus");
            DropColumn("dbo.Orders", "PaymentMethod");
            DropColumn("dbo.Orders", "OrderNotes");
            DropColumn("dbo.Orders", "OrderCode");
        }
    }
}
