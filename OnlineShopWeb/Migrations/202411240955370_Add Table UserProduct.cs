namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableUserProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProducts",
                c => new
                    {
                        UserProductId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        ViewNum = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserProductId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserProducts", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.UserProducts", new[] { "ProductId" });
            DropIndex("dbo.UserProducts", new[] { "UserId" });
            DropTable("dbo.UserProducts");
        }
    }
}
