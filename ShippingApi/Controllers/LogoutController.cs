using Newtonsoft.Json.Linq;
using ShippingApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ShippingApi.Controllers
{
    public class LogoutController : ApiController
    {
        public async Task<IHttpActionResult> Get(JObject json)
        {
            string sql = "EXEC SP_Logout " + SqlMapper.Json2Params(json);

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
            string sql = "EXEC SP_Logout " + SqlMapper.Json2Params(json);

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
