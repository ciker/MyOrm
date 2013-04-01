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
        public string Name { get; set; }
    }
}
