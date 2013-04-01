using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyOrm
{
    public class DbManager : IDisposable
    {
        private readonly IDbConnection _connection;

        public DbManager(string connectionString)
        {
            _connection = new SQLiteConnection(connectionString);
            _connection.Open();
        }

        public DbManager(IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();
            _connection = connection;
        }

        public void CreateTable<T>()
        {
            var type = typeof(T);
            var pk = getPrimaryKey(type);
            var first = true;
            var sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE IF NOT EXISTS '{0}' (", type.Name);
            foreach (var pi in type.GetProperties())
            {
                var fa = pi.GetCustomAttributes(typeof(FieldAttribute), false);
                var field = pi.Name;
                if (fa.Any())
                {
                    field = ((FieldAttribute)fa[0]).Name;
                }

                fa = pi.GetCustomAttributes(typeof(LengthAttribute), false);
                var length = 255;
                if (fa.Any())
                {
                    length = ((LengthAttribute)fa[0]).Length;
                }

                var typestr = "";
                switch (pi.PropertyType.ToString())
                {
                    case "System.Int32":
                        typestr = "INTEGER";
                        break;
                    case "System.String":
                        typestr = "NVARCHAR(" + length + ")";
                        break;
                    case "System.Decimal":
                        typestr = "DECIMAL(15,4)";
                        break;
                    case "System.Int64":
                        typestr = "INTEGER";
                        break;
                    case "System.Boolean":
                        typestr = "BOOLEAN";
                        break;
                    case "System.DateTime":
                        typestr = "DATE";
                        break;
                }
                if (pi.Name == pk)
                {
                    typestr += " PRIMARY KEY AUTOINCREMENT";
                }

                if (!first)
                    sb.Append(",");
                first = false;
                sb.AppendFormat("\n'{0}' {1}", field, typestr);
            }
            sb.Append(")");
            Console.WriteLine(sb.ToString());

            //var sql = "CREATE TABLE IF NOT EXISTS '{0}' ({1})";
            //var pk = getPrimaryKey(type);

            //foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(T)))
            //{

            //    var attributes = property.Attributes;
            //    foreach (var attribute in attributes)
            //    {
            //        if (attribute is PrimaryKeyAttribute)
            //        {
            //            Console.WriteLine("Primary key: {0}", property.Name);
            //        }
            //    }
            //foreach (var attribute in property.Attributes.Contains(PrimaryKeyAttribute))
            //{
            //    Console.WriteLine(attribute);
            //}
            //Console.WriteLine(property.Name);
            //}

            //Console.WriteLine(getPrimaryKey(typeof(T)));
            //var type = typeof(T);
            //var pk = getPrimaryKey(type);
            //Console.WriteLine(pk);
            //foreach (var pi in type.GetProperties())
            //{
            //    foreach (var a in pi.GetCustomAttributes(false))
            //    {

            //    }
            //}
        }

        public IEnumerable<T> Select<T>()
        {
            return null;
        }

        public T GetById<T>(object id)
        {
            var type = typeof(T);
            var sql = string.Format("SELECT {0} FROM {1} WHERE {2}=@id", getFieldsString(type), getTableName(type), getPrimaryKey(type));
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add("@id");
                command.Parameters["@id"] = id;
                var reader = command.ExecuteReader();
                //return getEntityObject<T>(reader);
            }
            return default(T);
        }

        private object getTableName(Type type)
        {
            return null;
        }

        private object getFieldsString(Type type)
        {
            return null;
        }

        private T getEntityObject<T>(IDataReader reader) where T : new()
        {
            //var t = new T();
            //foreach (var pi in t.GetType().GetProperties())
            //{
            //    switch (pi.PropertyType.ToString())
            //    {
            //        case "System.Int32":
            //            break;
            //        case "System.String":
            //            break;
            //        case "System.Decimal":
            //            break;
            //        case "System.Boolean":
            //            break;
            //    }

            //    var propertyType = pi.PropertyType;
            //    var value = reader.GetValue(reader.GetOrdinal(pi.Name));
            //    Convert.ChangeType(value, propertyType);
            //    //pi.SetValue();
            //    foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(T)))
            //    {
            //        property.Name
            //    }
            //}
            return default(T);
        }

        //private string getFieldName(PropertyInfo pi)
        //{

        //}

        private string getPrimaryKey(Type type)
        {
            return type.GetProperties().Where(c => c.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any())
                .Select(pi => pi.Name).FirstOrDefault();
        }

        public void Insert<T>(T obj)
        {

        }

        public void Update<T>(T obj)
        {

        }

        public void Delete<T>(T obj)
        {

        }

        public void Delete<T>(object id)
        {

        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Close();
        }
    }
}
