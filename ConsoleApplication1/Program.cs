using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOrm;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new DbManager("data source = c:\\111\\db.sqlite"))
            {
                db.CreateTable<Product>();
                db.CreateTable<Category>();
                db.UpdateTable<Category>();
                var insert = db.Insert(new Product() { Name = "Product1", DateCreated = DateTime.Now, Price = 1260, Reminder = 12 });
                db.Insert(new Category() {Name = "Товары"});
                var products = db.Select<Product>().Where(c => c.Price == 1260);
                foreach (var product in products)
                {
                    Console.WriteLine("Id={0}, Name={1}, Price={2:N2}, DateCreated={3:d}",
                        product.Id, product.Name, product.Price, product.DateCreated);
                }
                var byId = db.GetById<Product>(insert.Id);
                byId.Name = "dslkjnslvndflkvnfd";
                byId.Price = 1230;
                db.Update(byId);
                byId = db.GetById<Product>(1);
                db.Delete<Product>(2);
                Console.WriteLine("".PadLeft(80, '='));
                products = db.Select<Product>();
                foreach (var product in products)
                {
                    Console.WriteLine("Id={0}, Name={1}, Price={2:N2}, DateCreated={3:d}",
                        product.Id, product.Name, product.Price, product.DateCreated);
                }

            }
            Console.ReadKey();
        }
    }
}
