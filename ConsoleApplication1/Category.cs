using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyOrm;

namespace ConsoleApplication1
{
    
    public class Category
    {
        [PrimaryKey]
        [Field("CategoryId")]
        public int Id { get; set; }
        [Field]
        public string Name { get; set; }
    }
}
