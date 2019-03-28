using ShippingApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace ShippingApi.Controllers
{
    public class ByReceiveController : ApiController
    {
        [TokenAuthorize]
        public async Task<IHttpActionResult> Get(string date_from = "", string date_to = "")
        {
            string sql = "EXEC SP_ByReceiveRpt @date_from='" + date_from.Replace("'","") + "',@date_to='" + date_to.Replace("'", "") + "'";

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
