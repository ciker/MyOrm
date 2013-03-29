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
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(typeof(T)))
            {
                foreach (var attribute in property.Attributes.Matches(PrimaryKeyAttribute.IsDefined()))
                {
                    Console.WriteLine(attribute);
                }
                Console.WriteLine(property.Name);
            }
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
            var pk = getPrimaryKey(typeof(T));
            var sql = string.Format("SELECT {0} FROM {1} WHERE {2}=@id");
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
