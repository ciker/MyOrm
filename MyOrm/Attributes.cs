using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyOrm
{
    class Attributes
    {
    }

    public class AutoincrementAttribute : Attribute
    {

    }

    public class PrimaryKeyAttribute : Attribute
    {

    }

    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}
