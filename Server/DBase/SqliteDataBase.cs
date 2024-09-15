using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.SQLite;
using System.Data;
using System.Data.SQLite;

namespace OpenWLS.Server.DBase
{
    public class SqliteDataBase
    {
        protected string pathDB;
        SQLiteConnection conn;
        public SqliteDataBase()
        {
            conn = null;
        }
        protected void OpenDbInMemory()
        {
            var connectionStringBuilder = new SQLiteConnectionStringBuilder { DataSource = ":memory:" };
            conn = new SQLiteConnection(connectionStringBuilder.ToString());
            //    conn = new SqliteConnection("FullUri=file::memory:?cache=shared;");
            conn.Open();

        }
        public string OpenDb(string fileName)
        {
            pathDB = fileName;
            //    string pathDB = System.IO.Path.Combine(Environment.CurrentDirectory, fileName);

         //   if (!File.Exists(pathDB))
         //       return "Open dbfile {pathDB} failed";

            string connection_string = $"Data Source={pathDB};Version=3;";
            conn = new SQLiteConnection(connection_string);
            conn.Open();
            return "Open dbfile {pathDB} successfully";
        }

        public DataTable GetDataTable(string sql)
        {

            DataTable dt = new DataTable();
            try
            {
                if (conn == null)
                {
                    // var pathDB = System.IO.Path.Combine(Environment.CurrentDirectory, pathDB);
                    if (!File.Exists(pathDB)) return null;
                    string connection_string = string.Format("Data Source={0};Version=3;", pathDB);

                    conn = new SQLiteConnection(connection_string);
                    conn.Open();
                }
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
                return dt;
            }

            catch (Exception e)
            {
                //throw new Exception(e.Message);
                return null;
            }
        }

        public bool ExecuteNonQuery(string sql)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception e)
            {
                //   DaConsole.WriteLine("Sql error: exec " + sql + ".  " + e.Message);
                return false;
            }
        }

        public SQLiteDataReader GetSQLiteDataReader(string sql)
        {
            try
            {
                //         if (conn == null) OpenDB();
                if (conn == null) return null;
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    return reader;
                }
            }
            catch (Exception e)
            {
                // DaConsole.WriteLine("Sql error: get dt: " + sql + ".  " + e.Message);
                return null;
            }
        }

        public bool ExecuteNonQuery(string sql, SQLiteParameter para)
        {
            try
            {
                using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.Add(para);
                    cmd.ExecuteNonQuery();
                    //   SSysLog.AddCustomerMessageNonQuery(sql, name, cp);
                }
                return true;
            }
            catch (Exception e)
            {
                //     DaConsole.WriteLine("Sql error: exec " + sql + ".  " + e.Message);
                return false;
            }
        }

        public long GetMaxID(string tbl)
        {
            try
            {
                //          if (conn == null) OpenDB();
                if (conn == null) return 0;
                string sql = "select ID from " + tbl + " order by ID desc limit 1";
                SQLiteCommand createCommand = new SQLiteCommand(sql, conn);
                SQLiteDataReader dataReader = createCommand.ExecuteReader();
                if (dataReader.Read())
                    return dataReader.GetInt64(0);
            }
            catch (Exception e)
            {
                // DaConsole.WriteLine("Sql error: get max id of " + tbl + ".  " + e.Message);
            }
            return 0;
        }

        public void Close()
        {
            if (conn != null)
                conn.Close();
            conn = null;
        }
    }
}
