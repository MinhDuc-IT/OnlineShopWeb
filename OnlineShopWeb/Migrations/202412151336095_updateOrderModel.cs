namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "OrderCode", c => c.String(nullable: false));
            AddColumn("dbo.Orders", "OrderNotes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OrderNotes");
            DropColumn("dbo.Orders", "OrderCode");
        }
    }
}
