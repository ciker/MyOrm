using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MyOrm
{
    public static class DbExtensions
    {
        public static IDbConnection DbConnect(this string connectionString)
        {
            var connection = new SQLiteConnection(connectionString);
            return connection.OpenAndReturn();
        }

        public static void Execute(this IDbConnection connection, string sql, params IDbDataParameter[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
                command.ExecuteNonQuery();
            }
        }

        public static IDataReader ExecuteReader(this IDbConnection connection, string sql, params IDbDataParameter[] parameters)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
                return command.ExecuteReader();
            }
        }

        public static IEnumerable<T> Select<T>(this IDbConnection connection)
        {
            var sql = string.Format("SELECT * FROM {0}", GetTableName(typeof(T)).Quoted());
            var reader = connection.ExecuteReader(sql);
            while (reader.Read())
            {
                yield return GetObjectMap<T>(reader);
            }
        }

        public static T GetObjectMap<T>(IDataRecord record)
        {
            var obj = Activator.CreateInstance<T>();
            foreach (var pi in obj.GetType().GetProperties().Where(c => c.GetCustomAttributes(typeof(FieldAttribute), false).Any()))
            {
                if (!object.Equals(record[GetFieldName(pi)], DBNull.Value))
                {
                    var r = record[GetFieldName(pi)];
                    var value = Convert.ChangeType(r, pi.PropertyType);
                    pi.SetValue(obj, value, null);
                }
            }
            return obj;
        }

        private static string GetFieldName(PropertyInfo pi)
        {
            var field = pi.Name;
            var fa = pi.GetCustomAttributes(typeof(FieldAttribute), false);
            if (fa.Any())
            {
                var name = ((FieldAttribute)fa[0]).Name;
                if (!string.IsNullOrWhiteSpace(name))
                    field = name;
            }
            return field;
        }


        public static string GetTableName(Type type)
        {
            var name = type.Name;

            if (name.EndsWith("s")) { name += "e"; }
            else if (name.EndsWith("y")) { name = name.Substring(0, name.Length - 1) + "ie"; }
            name += "s";

            var attr = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (attr.Any())
            {
                name = ((TableAttribute)attr[0]).Name;
            }

            return name;
        }

        public static bool IsPrimaryKey(this PropertyInfo pi)
        {
            return pi.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any();
        }

        public static bool IsNotNull(this PropertyInfo pi)
        {
            return pi.GetCustomAttributes(typeof(NotNullAttribute), false).Any();
        }

        public static string Quoted(this string str)
        {
            return string.Format("\"{0}\"", str);
        }

    }
}
