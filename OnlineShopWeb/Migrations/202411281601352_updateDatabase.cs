namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDatabase : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.Banners",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            title = c.String(nullable: false, maxLength: 500),
            //            description = c.String(nullable: false, maxLength: 500),
            //            Image = c.Binary(nullable: false),
            //            IsDeleted = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);

            //CreateTable(
            //    "dbo.UserProducts",
            //    c => new
            //        {
            //            UserProductId = c.Int(nullable: false, identity: true),
            //            UserId = c.Int(nullable: false),
            //            ProductId = c.Int(nullable: false),
            //            ViewNum = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.UserProductId)
            //    .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
            //    .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
            //    .Index(t => t.UserId)
            //    .Index(t => t.ProductId);

            //Sql("UPDATE dbo.Users SET Avatar = NULL");


            AlterColumn("dbo.Users", "Name", c => c.String(maxLength: 100));
            AlterColumn("dbo.Users", "Gender", c => c.Int());
            AlterColumn("dbo.Users", "BirthDay", c => c.DateTime());
            AlterColumn("dbo.Users", "Avatar", c => c.Binary());
            //DropColumn("dbo.Users", "userName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "userName", c => c.String());
            DropForeignKey("dbo.UserProducts", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.UserProducts", new[] { "ProductId" });
            DropIndex("dbo.UserProducts", new[] { "UserId" });
            AlterColumn("dbo.Users", "Avatar", c => c.String());
            AlterColumn("dbo.Users", "BirthDay", c => c.String());
            AlterColumn("dbo.Users", "Gender", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "Name", c => c.String(nullable: false, maxLength: 100));
            DropTable("dbo.UserProducts");
            DropTable("dbo.Banners");
        }
    }
}
