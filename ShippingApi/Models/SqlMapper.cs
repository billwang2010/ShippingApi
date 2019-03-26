using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ShippingApi.Models
{
    public static class SqlMapper
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["MSSQLConnection"].ConnectionString;
       
        private static DataTable GetDataTable(string sql, bool schema = false)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(sql, connection);
                adapter.SelectCommand.CommandTimeout = 300;
                DataTable dt = new DataTable();
                if (schema) adapter.FillSchema(dt, SchemaType.Source);
                adapter.Fill(dt);
                adapter.Dispose();
                return dt;
            }
        }

        public static string Json2Params(JObject json)
        {
            string sql = " ";

            IEnumerable<JProperty> jproperties = json.Properties();
            foreach (JProperty item in jproperties)
            {
                sql += "@" + item.Name + "=N'" + item.Value.ToString().Replace("'", "''") + "',";
            }
            return sql.Remove(sql.Length - 1);
        }

        public static string GetUserName(string token)
        {
            return SqlMapper.GetValue("select top 1 UserName from A_Login where Token='" + token.Replace("'", "''") + "' and LogoutTime IS NULL order by LoginTime desc");
        }

        public static string GetKeys(string sql)
        {
            DataTable dtTable = SqlMapper.GetDataTable(sql, true);
            if (dtTable == null) return string.Empty;

            DataTable dt = SqlMapper.GetDataTable("EXEC sys.sp_pkeys '" + dtTable.TableName + "'");
            string s = " ";
            foreach (DataRow dr in dt.Rows)
            {
                s += dr["COLUMN_NAME"] + ",";
            }
            return s.Remove(s.Length - 1);
        }

        public static string GetValue(string sql)
        {
            DataTable dt = SqlMapper.GetDataTable(sql);
            if (dt == null) return string.Empty;

            if (dt.Rows.Count > 0)
                return dt.Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public static List<dynamic> GetData(string sql)
        {
            DataTable dt = SqlMapper.GetDataTable(sql);
            if (dt == null)
                return null;
            else
                return DataTableToJson(dt);
        }

        private static List<dynamic> DataTableToJson(DataTable dt)
        {
            List<dynamic> ls = new List<dynamic>();
            JObject ret = new JObject();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ret = new JObject();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ret.Add(dt.Columns[j].ColumnName, dt.Rows[i][j].ToString());
                }
                ls.Add(ret);
            }
            return ls;
        }  
    }
}