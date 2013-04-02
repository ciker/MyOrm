using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOrm;
using System.Data.SQLite;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (var db = new DbManager("data source = c:\\111\\db.sqlite"))
            using (var db = new DbManager("data source = :memory:"))
            {
                db.CreateTable<Product>();

                for (var i = 0; i < 10; i++)
                {
                    db.Insert(new Product()
                        {
                            Name = "Product" + i,
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now,
                            Price = i,
                            Reminder = i
                        });
                }
                foreach (var product in db.Select<Product>())
                {
                    Console.WriteLine("{0}, {1}, {2}, {3}, {4:d}, {5:d}",
                        product.Id, product.Name, product.Price, product.Reminder, product.DateCreated, product.DateModified);
                }
                Console.WriteLine("".PadLeft(80, '='));
                foreach (var product in db.Query<Product>("SELECT * FROM Products WHERE Reminder>@reminder AND Price>@price",
                    new SQLiteParameter("@reminder", 5), new SQLiteParameter("@price", 8)))
                {
                    Console.WriteLine("{0}, {1}, {2}, {3}, {4:d}, {5:d}",
                        product.Id, product.Name, product.Price, product.Reminder, product.DateCreated, product.DateModified);
                }
                var product1 = db.Get<Product>(1);
                Console.WriteLine("".PadLeft(80, '='));
                Console.WriteLine("{0}, {1}, {2}, {3}, {4:d}, {5:d}",
                    product1.Id, product1.Name, product1.Price, product1.Reminder, product1.DateCreated, product1.DateModified);
                product1.Reminder = 5;
                db.Update(product1);
                product1 = db.Get<Product>(1);
                Console.WriteLine("".PadLeft(80, '='));
                Console.WriteLine("{0}, {1}, {2}, {3}, {4:d}, {5:d}",
                    product1.Id, product1.Name, product1.Price, product1.Reminder, product1.DateCreated, product1.DateModified);
                db.Delete<Product>(1);
                Console.WriteLine("".PadLeft(80, '='));
                foreach (var product in db.Select<Product>())
                {
                    Console.WriteLine("{0}, {1}, {2}, {3}, {4:d}, {5:d}",
                        product.Id, product.Name, product.Price, product.Reminder, product.DateCreated, product.DateModified);
                }
            }

            //    db.CreateTable<Product>();
            //    db.CreateTable<Category>();
            //    Product insert = null;
            //    for (var i = 0; i < 10; i++)
            //    {
            //        insert =
            //            db.Insert(new Product
            //                {
            //                    Name = "Product_" + i,
            //                    DateCreated = DateTime.Now,
            //                    Price = 1260,
            //                    Reminder = 12
            //                });
            //    }
            //    var products = db.Select<Product>().Where(c => c.Price == 1260);
            //    foreach (var product in products)
            //    {
            //        Console.WriteLine("Id={0}, Name={1}, Price={2:N2}, DateCreated={3:d}",
            //            product.Id, product.Name, product.Price, product.DateCreated);
            //    }
            //    if (insert != null)
            //    {
            //        var byId = db.GetById<Product>(insert.Id);
            //        byId.Name = "dslkjnslvndflkvnfd";
            //        byId.Price = 1230;
            //        db.Update(byId);
            //        byId = db.GetById<Product>(1);
            //    }
            //    db.Delete<Product>(2);
            //    Console.WriteLine("".PadLeft(80, '='));
            //    products = db.Select<Product>();
            //    foreach (var product in products)
            //    {
            //        Console.WriteLine("Id={0}, Name={1}, Price={2:N2}, DateCreated={3:d}",
            //            product.Id, product.Name, product.Price, product.DateCreated);
            //    }

            //}
            Console.ReadKey();
        }
    }
}
