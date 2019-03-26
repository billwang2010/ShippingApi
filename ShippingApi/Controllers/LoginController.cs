using Newtonsoft.Json.Linq;
using ShippingApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ShippingApi.Controllers
{
    public class LoginController : ApiController
    {
        public async Task<IHttpActionResult> Get(JObject json)
        {
            string sql = "EXEC SP_Login " + SqlMapper.Json2Params(json);

            var content = Request.Properties["MS_HttpContext"] as HttpContextBase;
            var client = content.Request.Browser.Browser;
            sql += ",@Client='" + client.Replace("'", "") + "'";

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

        public async Task<IHttpActionResult> Post(JObject json)
        {
            string sql = "EXEC SP_Login " + SqlMapper.Json2Params(json);

            var content = Request.Properties["MS_HttpContext"] as HttpContextBase;
            var client = content.Request.Browser.Browser;
            sql += ",@Client='" + client.Replace("'", "") + "'";

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
