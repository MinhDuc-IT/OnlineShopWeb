//namespace OnlineShopWeb.Migrations
//{
//    using System;
//    using System.Data.Entity;
//    using System.Data.Entity.Migrations;
//    using System.Linq;

//    internal sealed class Configuration : DbMigrationsConfiguration<OnlineShopWeb.Data.ApplicationDbContext>
//    {
//        public Configuration()
//        {
//            AutomaticMigrationsEnabled = false;
//        }

//        protected override void Seed(OnlineShopWeb.Data.ApplicationDbContext context)
//        {
//            //  This method will be called after migrating to the latest version.

//            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
//            //  to avoid creating duplicate seed data.
//        }
//    }
//}
namespace OnlineShopWeb.Migrations
{
    using OnlineShopWeb.Data;
    using OnlineShopWeb.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<OnlineShopWeb.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "OnlineShopWeb.Data.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Products.AddOrUpdate(
                p => p.ProductId, // Xác định trường khóa chính để tránh thêm trùng
                new Product
                {
                    ProductId = 1,
                    Name = "Sản phẩm mẫu",
                    BrandId = 1,
                    CategoryId = 1,
                    Price = 100,
                    Stock = 50,
                    Description = "Mô tả sản phẩm mẫu",
                    Image = "/images/sample.jpg",
                    IsDeleted = false
                }
            );

            context.SaveChanges();
        }
    }
}
