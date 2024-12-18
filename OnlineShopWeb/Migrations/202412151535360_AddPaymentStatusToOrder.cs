namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPaymentStatusToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "PaymentStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "PaymentStatus");
        }
    }
}
