using System;

namespace MyOrm
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NotNullAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DefaultAttribute : Attribute
    {
        public object Value { get; private set; }
        public DefaultAttribute(object value)
        {
            Value = value;
        }
    }


    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; private set; }
        public TableAttribute(string name)
        {
            Name = name;
        }
    }



    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAttribute : Attribute
    {
        public string Name { get; private set; }
        public FieldAttribute(string name)
        {
            Name = name;
        }
        public FieldAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class LengthAttribute : Attribute
    {
        public int Length { get; private set; }
        public LengthAttribute(int length)
        {
            Length = length;
        }
    }
}
