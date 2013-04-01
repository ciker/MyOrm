using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyOrm
{
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

    public class FieldAttribute : Attribute
    {
        public string Name { get; set; }
        public FieldAttribute(string name)
        {
            Name = name;
        }
    }

    public class LengthAttribute : Attribute
    {
        public int Length { get; set; }
        public LengthAttribute(int length)
        {
            Length = length;
        }
    }
}
