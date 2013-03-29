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
                db.Insert(new Product() { Name = "Product1" });
                var products = db.Select<Product>();
                if (products != null)
                    foreach (var product in products)
                    {
                        Console.WriteLine("Id={0}, Name={1}", product.Id, product.Name);
                    }
            }
            Console.ReadKey();
        }
    }
}
