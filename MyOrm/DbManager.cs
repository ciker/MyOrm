using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MyOrm
{
    public class DbManager : IDisposable
    {
        struct FieldInfo
        {
            public int Cid;
            public string Name;
            public string Type;
            public int NotNull;
            public object DefaultValue;
            public int Pk;
        }

        private readonly IDbConnection _connection;

        private static readonly Dictionary<string, string> TableDictionary = new Dictionary<string, string>();
        private static readonly Dictionary<string, Dictionary<string, FieldInfo>> FieldDictionary = new Dictionary<string, Dictionary<string, FieldInfo>>();

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

            var first = true;
            var sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0} (", GetTableName(type).Quoted());

            foreach (var fi in GetFields(type))
            {
                if (!first) { sb.Append(","); }
                first = false;
                sb.AppendFormat("\n{0} {1}{2}{3}",
                                fi.Name.Quoted(),
                                fi.Type,
                                fi.NotNull == 1 ? " NOT NULL" : "",
                                fi.Pk == 1 ? " PRIMARY KEY AUTOINCREMENT" : "");
            }
            sb.Append("\n);");

            Debug.WriteLine(sb.ToString());

            _connection.Execute(sb.ToString());
        }

        private static IEnumerable<FieldInfo> GetFields(Type type)
        {
            var nrow = 0;
            var list = new List<FieldInfo>();
            foreach (PropertyInfo c in type.GetProperties())
            {
                if (c.GetCustomAttributes(typeof(FieldAttribute), false).Any() ||
                    c.GetCustomAttributes(typeof(ReferenceAttribute), false).Any())
                {
                    string fieldName = GetFieldName(c);
                    string fieldType = GetFieldType(c);
                    list.Add(new FieldInfo()
                        {
                            Cid = nrow++,
                            Name = fieldName,
                            Type = fieldType,
                            NotNull = c.IsNotNull() ? 1 : 0,
                            Pk = c.IsPrimaryKey() ? 1 : 0
                        });
                }
            }
            return list;
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
            fa = pi.GetCustomAttributes(typeof(ReferenceAttribute), false);
            if (fa.Any())
            {
                GetPrimaryKeyField(pi.PropertyType)
                fa[0]
            }
            return field;
        }

        private static string GetFieldType(PropertyInfo pi)
        {
            var fieldType = string.Empty;
            switch (pi.PropertyType.ToString())
            {
                case "System.Int32":
                    fieldType = "INTEGER";
                    break;
                case "System.String":
                    var length = GetLength(pi);
                    fieldType = "NVARCHAR(" + length + ")";
                    break;
                case "System.Decimal":
                    fieldType = "DECIMAL(15,4)";
                    break;
                case "System.Int64":
                    fieldType = "INTEGER";
                    break;
                case "System.Boolean":
                    fieldType = "BOOLEAN";
                    break;
                case "System.DateTime":
                    fieldType = "DATE";
                    break;
            }
            return fieldType;
        }

        private List<FieldInfo> GetScheme(Type type)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = "PRAGMA table_info(" + GetTableName(type) + ")";
                var reader = command.ExecuteReader();
                var schema = new List<FieldInfo>();
                while (reader.Read())
                {
                    var info = new FieldInfo
                        {
                            Cid = reader.GetInt32(reader.GetOrdinal("cid")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            Type = reader.GetString(reader.GetOrdinal("type")),
                            NotNull = reader.GetInt32(reader.GetOrdinal("notnull")),
                            DefaultValue = reader.GetValue(reader.GetOrdinal("dflt_value")),
                            Pk = reader.GetInt32(reader.GetOrdinal("pk"))
                        };
                    schema.Add(info);
                }
                return schema;
            }
        }

        private static int GetLength(PropertyInfo pi)
        {
            var fa = pi.GetCustomAttributes(typeof(LengthAttribute), false);
            var length = 255;
            if (fa.Any())
            {
                length = ((LengthAttribute)fa[0]).Length;
            }
            return length;
        }

        public IEnumerable<T> Select<T>() where T : class
        {
            var sql = string.Format("SELECT * FROM {0}", GetTableName(typeof(T)));
            Debug.WriteLine(sql);
            var reader = _connection.ExecuteReader(sql);
            while (reader.Read())
            {
                yield return GetObjectMap<T>(reader);
            }
        }

        public IEnumerable<T> Query<T>(string sql, params IDbDataParameter[] parameters) where T : class
        {
            Debug.WriteLine(sql);
            var reader = _connection.ExecuteReader(sql, parameters);
            while (reader.Read())
            {
                yield return GetObjectMap<T>(reader);
            }
        }

        public T Get<T>(object id)
        {
            var type = typeof(T);
            using (var command = _connection.CreateCommand())
            {
                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = id;
                command.CommandText = string.Format("SELECT * FROM {0} WHERE {1}=@id", GetTableName(type), GetPrimaryKeyField(type));
                command.Parameters.Add(param);
                var reader = command.ExecuteReader();
                return GetObjectMap<T>(reader);
            }
        }

        //public IEnumerable<T> Query<T>() where T : class
        //{
        //    var type = typeof(T);
        //    using (var command = _connection.CreateCommand())
        //    {
        //        var sbWhereList = new StringBuilder();
        //        //var param = command.CreateParameter();
        //        //param.ParameterName = "@id";
        //        //param.Value = id;
        //        command.CommandText = string.Format("SELECT * FROM {0} WHERE {1}", GetTableName(type), sbWhereList);
        //        //command.Parameters.Add(param);
        //        var reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            var map = GetObjectMap<T>(reader);
        //            yield return map;
        //        }
        //    }
        //}

        private static string GetTableName(Type type)
        {
            string name;
            if (!TableDictionary.TryGetValue(type.Name, out name))
            {
                name = type.Name;
                if (name.EndsWith("y"))
                    name = name.Substring(0, name.Length - 1) + "ie";
                name = name + "s";
                var attributes = type.GetCustomAttributes(typeof(TableAttribute), false);
                if (attributes.Any())
                    name = ((TableAttribute)attributes[0]).Name;
                TableDictionary.Add(type.Name, name);
            }
            return name;
        }


        private static string GetPrimaryKeyField(Type type)
        {
            var pi = type.GetProperties().FirstOrDefault(c => c.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any());
            if (pi != null)
                return GetFieldName(pi);
            throw new Exception("Отсутствует Id");
        }

        private static T GetObjectMap<T>(IDataRecord reader)
        {
            var obj = Activator.CreateInstance<T>();
            foreach (var pi in obj.GetType().GetProperties().Where(c => c.GetCustomAttributes(typeof(FieldAttribute), false).Any()))
            {
                if (!object.Equals(reader[GetFieldName(pi)], DBNull.Value))
                {
                    var r = reader[GetFieldName(pi)];
                    var value = Convert.ChangeType(r, pi.PropertyType);
                    pi.SetValue(obj, value, null);
                }
            }
            return obj;
        }

        public T Insert<T>(T obj) where T : class
        {
            var type = typeof(T);

            using (var transaction = _connection.BeginTransaction())
            using (var command = _connection.CreateCommand())
            {
                command.Transaction = transaction;

                var first = true;
                var sbColumnList = new StringBuilder();
                var sbParameterList = new StringBuilder();

                foreach (var pi in type.GetProperties().Where(c => c.GetCustomAttributes(typeof(FieldAttribute), false).Any()))
                {
                    var param = command.CreateParameter();
                    param.ParameterName = string.Format("@{0}", GetFieldName(pi));
                    param.Value = pi.IsPrimaryKey() ? null : pi.GetValue(obj, null);
                    command.Parameters.Add(param);

                    if (!first)
                    {
                        sbColumnList.Append(", ");
                        sbParameterList.Append(", ");
                    }
                    first = false;

                    sbColumnList.Append(GetFieldName(pi));
                    sbParameterList.AppendFormat("@{0}", GetFieldName(pi));
                }

                command.CommandText = string.Format("INSERT INTO {0}({1}) VALUES({2})", GetTableName(type),
                    sbColumnList, sbParameterList);
                Debug.WriteLine(command.CommandText);
                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();

                    command.CommandText = "SELECT last_insert_rowid();";
                    var id = command.ExecuteScalar();
                    var propid =
                            type.GetProperties().FirstOrDefault(c => c.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Any());
                    if (propid != null)
                    {
                        id = Convert.ChangeType(id, propid.PropertyType);
                        propid.SetValue(obj, id, null);
                    }
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    throw new Exception(exception.Message);
                }
            }
            return obj;
        }

        public void Update<T>(T obj) where T : class
        {
            var type = typeof(T);
            using (var transaction = _connection.BeginTransaction())
            using (var command = _connection.CreateCommand())
            {
                command.Transaction = transaction;

                var sbParameterList = new StringBuilder();
                var first = true;

                foreach (var pi in type.GetProperties().Where(c => c.GetCustomAttributes(typeof(FieldAttribute), false).Any()))
                {
                    var param = command.CreateParameter();
                    param.ParameterName = string.Format("@{0}", GetFieldName(pi));
                    param.Value = pi.GetValue(obj, null);
                    command.Parameters.Add(param);

                    if (!first)
                        sbParameterList.Append(", ");
                    first = false;
                    sbParameterList.AppendFormat("{0}=@{0}", GetFieldName(pi));
                }

                var sql = string.Format("UPDATE {0} SET {1} WHERE {2}=@{2}", GetTableName(type), sbParameterList,
                        GetPrimaryKeyField(type));

                command.CommandText = sql;
                Debug.WriteLine(sql);

                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    throw new Exception(exception.Message);
                }
            }
        }

        public void Delete<T>(object id)
        {
            var type = typeof(T);
            using (var transaction = _connection.BeginTransaction())
            using (var command = _connection.CreateCommand())
            {
                command.Transaction = transaction;

                var sql = string.Format("DELETE FROM {0} WHERE {1}=@{1}", GetTableName(type), GetPrimaryKeyField(type));
                var param = command.CreateParameter();
                param.ParameterName = string.Format("@{0}", GetPrimaryKeyField(type));
                param.Value = id;
                command.Parameters.Add(param);

                command.CommandText = sql;
                Debug.WriteLine(sql);

                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception exception)
                {
                    transaction.Rollback();
                    throw new Exception(exception.Message);
                }
            }
        }

        public void Dispose()
        {
            if (_connection != null)
                _connection.Close();
        }
    }
}
