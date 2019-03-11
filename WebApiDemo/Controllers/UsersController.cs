using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApiDemo.Models;

namespace WebApiDemo.Controllers
{
    public class UsersController : ApiController
    {
        // GET api/users
        [TokenAuthorize]
        public async Task<IHttpActionResult> Get()
        {
            string sql = "SELECT * FROM G_User";
            var result = DynamicMapper.DynamicSqlQuery(sql);
            if (result == null)
                return NotFound();
            else
                return await Task.FromResult(Json(result.ToList()));
        }

        // GET api/users/5
        [TokenAuthorize]
        public async Task<IHttpActionResult> Get(string id)
        {
            string sql = "SELECT top 1 * FROM G_User where UserID='" + id + "'";
            var result = DynamicMapper.DynamicSqlQuery(sql);
            if (result == null)
                return NotFound();
            else
                return await Task.FromResult(Json(result.ToList()));
        }

        [TokenAuthorize]
        public async Task<IHttpActionResult> Post(JObject json)
        {
            string sql = "EXEC SP_UserData ";
            IEnumerable<JProperty> jproperties = json.Properties();
            foreach (JProperty item in jproperties)
            {
                sql += "@" + item.Name + "=N'" + item.Value.ToString().Replace("'", "''") + "',";
            }
            sql = sql.Remove(sql.Length - 1);

            var result = new List<dynamic>();
            var task = Task.Run(() => {
                result = DynamicMapper.DynamicSqlQuery(sql);
            });
            await Task.WhenAll(task);

            if (result == null)
                return NotFound();
            else
                return Json(result.ToList());
        }

        [TokenAuthorize]
        public async Task<IHttpActionResult> Put(JObject json)
        {
            string sql = "EXEC SP_UserData ";
            IEnumerable<JProperty> jproperties = json.Properties();
            foreach (JProperty item in jproperties)
            {
                sql += "@" + item.Name + "=N'" + item.Value.ToString().Replace("'", "''") + "',";
            }
            sql = sql.Remove(sql.Length - 1);

            var result = new List<dynamic>();
            var task = Task.Run(() => {
                result = DynamicMapper.DynamicSqlQuery(sql);
            });
            await Task.WhenAll(task);

            if (result == null)
                return NotFound();
            else
                return Json(result.ToList());
        }

        [TokenAuthorize]
        public async Task<IHttpActionResult> Delete(string id)
        {
            string sql = "delete G_User where UserID='" + id.Replace("'", "''") + "';select result='Deleted " + id.Replace("'", "''") + "'";
            var result = new List<dynamic>();
            var task = Task.Run(() => {
                result = DynamicMapper.DynamicSqlQuery(sql);
            });
            await Task.WhenAll(task);

            if (result == null)
                return NotFound();
            else
                return Json(result.ToList());
        }
    }
}
