using Newtonsoft.Json.Linq;
using ShippingApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ShippingApi.Controllers
{
    public class ShippingController : ApiController
    {
        [TokenAuthorize]
        public async Task<IHttpActionResult> Get(int start = 0, int length = 30, string sort = "", string direction = "", string po_no = "", string io_no = "", string rec_no = "", string contract_no = "", string ht_hf = "", string status = "", string vendor_code = "", string desc_cn = "", string release_no = "", string rec_date = "", string eta = "")
        {
            start = (start < 0) ? 0 : start;
            length = (length <= 0) ? 10 : length;
            sort = (string.IsNullOrEmpty(sort) || sort.Length == 0) ? "primary" : sort;
            direction = (string.IsNullOrEmpty(direction) || direction.Length <= 0) ? "asc" : direction;

            string tab = "A_Shipping";
            string sql = "SELECT * FROM " + tab;
            string whe = " WHERE 1=1";
            if (!string.IsNullOrEmpty(po_no))
                whe += " and po_no like '%" + po_no.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(io_no))
                whe += " and io_no like '%" + io_no.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(rec_no))
                whe += " and rec_no like '%" + rec_no.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(contract_no))
                whe += " and contract_no like '%" + contract_no.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(ht_hf))
                whe += " and ht_hf like '%" + ht_hf.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(vendor_code))
                whe += " and vendor_code like '%" + vendor_code.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(desc_cn))
                whe += " and desc_cn like N'%" + desc_cn.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(release_no))
                whe += " and release_no like '%" + release_no.Replace("'", "''") + "%'";
            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "all")
                    whe += "";
                else if (status.ToLower() == "received")
                    whe += " and eta is not null";
                else
                    whe += " and eta is null";
            }
            if (!string.IsNullOrEmpty(rec_date) && rec_date.Trim().Length > 0)
            {
                DateTime dte;
                if (DateTime.TryParse(rec_date, out dte))
                    whe += " and CONVERT(date,rec_date)='" + dte.Date.ToString("dd-MMM-yyyy") + "'";
            }
            if (!string.IsNullOrEmpty(eta) && eta.Trim().Length > 0)
            {
                DateTime dte;
                if (DateTime.TryParse(eta, out dte))
                    whe += " and CONVERT(date,eta)='" + dte.Date.ToString("dd-MMM-yyyy") + "'";
            }

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
            string sql = "EXEC SP_Shipping " + SqlMapper.Json2Params(json);
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
            string sql = "EXEC SP_Shipping " + SqlMapper.Json2Params(json);
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
            string sql = "EXEC SP_ShippingDel " + SqlMapper.Json2Params(json);
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
