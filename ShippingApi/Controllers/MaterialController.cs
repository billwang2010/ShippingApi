using Newtonsoft.Json.Linq;
using ShippingApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ShippingApi.Controllers
{
    public class MaterialController : ApiController
    {
        [TokenAuthorize]
        public async Task<IHttpActionResult> Get(int start = 0, int length = 10, string sort = "primary", string direction = "asc", string rec_id = "", string desc_en = "", string desc_cn = "", string uom = "", string pkg = "", string mat_type = "")
        {
            start = (start < 0) ? 0 : start;
            length = (length <= 0) ? 10 : length;
            sort = (sort.Length == 0) ? "primary" : sort;
            direction = (direction.Length <= 0) ? "asc" : direction;

            string tab = "A_Material";
            string sql = "SELECT * FROM " + tab;
            string whe = " WHERE 1=1";
            if (!string.IsNullOrEmpty(rec_id))
            {
                int rid = 0;
                if (int.TryParse(rec_id, out rid))
                    whe += " and rec_id=" + rec_id;
            }
            if (!string.IsNullOrEmpty(desc_en))
                whe += " and desc_en like '%" + desc_en.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(desc_cn))
                whe += " and desc_cn like N'%" + desc_cn.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(uom))
                whe += " and uom like N'%" + uom.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(pkg))
                whe += " and pkg like N'%" + pkg.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(mat_type))
                whe += " and mat_type like '%" + mat_type.Replace("'", "''") + "%'";

            var result = new List<dynamic>();
            int recordsTotal = 0;

            var task = Task.Run(() => {
                if (sort.ToLower() == "primary") sort = SqlMapper.GetKeys("SELECT top 0 * FROM " + tab);

                string rec = SqlMapper.GetValue("SELECT COUNT(1) FROM " + tab + whe);
                if (rec != null) int.TryParse(rec.ToString(), out recordsTotal);

                sql += whe + " order by " + sort + " " + direction + " offset " + start + " rows fetch next " + length + " rows only";
                result = SqlMapper.GetData(sql);
            });
            await Task.WhenAll(task);

            if (result == null)
                return NotFound();
            else
            {
                JObject ret = new JObject();
                ret.Add("start", start);
                ret.Add("length", length);
                ret.Add("recordsTotal", recordsTotal);
                ret.Add("data", JToken.FromObject(result));
                return Json(ret);
            }
        }

        [TokenAuthorize]
        public async Task<IHttpActionResult> Post(JObject json)
        {
            string sql = "EXEC SP_Material " + SqlMapper.Json2Params(json);
            var content = Request.Properties["MS_HttpContext"] as HttpContextBase;
            sql += ",@Updated_By='" + SqlMapper.GetUserName(content.Request.Headers["Token"]).Replace("'", "''") + "'";

            var result = new List<dynamic>();
            var task = Task.Run(() => {
                result = SqlMapper.GetData(sql);
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
            string sql = "EXEC SP_Material " + SqlMapper.Json2Params(json);
            var content = Request.Properties["MS_HttpContext"] as HttpContextBase;
            sql += ",@Updated_By='" + SqlMapper.GetUserName(content.Request.Headers["Token"]).Replace("'","''") + "'";

            var result = new List<dynamic>();
            var task = Task.Run(() => {
                result = SqlMapper.GetData(sql);
            });
            await Task.WhenAll(task);

            if (result == null)
                return NotFound();
            else
                return Json(result.ToList());
        }

        [TokenAuthorize]
        public async Task<IHttpActionResult> Delete(JObject json)
        {
            string sql = "EXEC SP_MaterialDel " + SqlMapper.Json2Params(json);
            var content = Request.Properties["MS_HttpContext"] as HttpContextBase;
            sql += ",@Updated_By='" + SqlMapper.GetUserName(content.Request.Headers["Token"]).Replace("'", "''") + "'";

            var result = new List<dynamic>();
            var task = Task.Run(() => {
                result = SqlMapper.GetData(sql);
            });
            await Task.WhenAll(task);

            if (result == null)
                return NotFound();
            else
                return Json(result.ToList());
        }
    }
}
