using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOrm;

namespace ConsoleApplication1
{
    public class Product
    {
        [PrimaryKey]
        [Field("ProductId")]
        public int Id { get; set; }
        [Field]
        [NotNull]
        public string Name { get; set; }
        [Field]
        public decimal Price { get; set; }
        [Field]
        public decimal Reminder { get; set; }
        [Field]
        public DateTime DateModified { get; set; }
        [Field]
        public DateTime DateCreated { get; set; }
        [Field]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}
