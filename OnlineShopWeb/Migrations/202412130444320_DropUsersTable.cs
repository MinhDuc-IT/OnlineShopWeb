namespace OnlineShopWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class DropUsersTable : DbMigration
    {
        public override void Up()
        {
            // Xóa các ràng buộc khóa ngoại trước
            DropForeignKey("dbo.UserProducts", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "CustomerId", "dbo.Users");
            DropForeignKey("dbo.Carts", "CustomerId", "dbo.Users");

            // Xóa bảng Users
            DropTable("dbo.Users");
        }

        public override void Down()
        {
            // Khôi phục lại bảng Users
            CreateTable(
                "dbo.Users",
                c => new
                {
                    CustomerId = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 100),
                    Email = c.String(nullable: false),
                    Password = c.String(nullable: false),
                    Phone = c.String(nullable: false),
                    Address = c.String(maxLength: 200),
                    Gender = c.Int(),
                    BirthDay = c.DateTime(),
                    Avatar = c.Binary(),
                    Role = c.String(nullable: false),
                })
                .PrimaryKey(t => t.CustomerId);

            // Khôi phục lại các ràng buộc khóa ngoại
            AddForeignKey("dbo.UserProducts", "UserId", "dbo.Users", "CustomerId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "CustomerId", "dbo.Users", "CustomerId", cascadeDelete: true);
            AddForeignKey("dbo.Carts", "CustomerId", "dbo.Users", "CustomerId", cascadeDelete: true);

        }
    }
}
