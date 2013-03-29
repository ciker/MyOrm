using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
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
    }
}
