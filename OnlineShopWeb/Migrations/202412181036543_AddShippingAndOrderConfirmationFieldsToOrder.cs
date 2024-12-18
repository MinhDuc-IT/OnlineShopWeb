namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShippingAndOrderConfirmationFieldsToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "DeliveryConfirmationImage", c => c.String());
            AddColumn("dbo.Orders", "CanceledBy", c => c.String());
            AddColumn("dbo.Orders", "CancellationTime", c => c.DateTime());
            AddColumn("dbo.Orders", "DeliveredBy", c => c.String());
            AddColumn("dbo.Orders", "DeliveryConfirmationTime", c => c.DateTime());
            AddColumn("dbo.Orders", "ShippingTime", c => c.DateTime());
            AddColumn("dbo.Orders", "OrderConfirmedBy", c => c.String());
            AddColumn("dbo.Orders", "OrderConfirmationTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "OrderConfirmationTime");
            DropColumn("dbo.Orders", "OrderConfirmedBy");
            DropColumn("dbo.Orders", "ShippingTime");
            DropColumn("dbo.Orders", "DeliveryConfirmationTime");
            DropColumn("dbo.Orders", "DeliveredBy");
            DropColumn("dbo.Orders", "CancellationTime");
            DropColumn("dbo.Orders", "CanceledBy");
            DropColumn("dbo.Orders", "DeliveryConfirmationImage");
        }
    }
}
