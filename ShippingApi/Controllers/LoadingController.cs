using ShippingApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace ShippingApi.Controllers
{
    public class LoadingController : ApiController
    {
        [TokenAuthorize]
        public async Task<IHttpActionResult> Get(string date_from = "", string date_to = "", string release_no = "")
        {
            string sql = "EXEC SP_LoadingRpt @date_from='" + date_from.Replace("'", "") + "',@date_to='" + date_to.Replace("'", "") + "',@release_no='" + release_no.Replace("'", "''") + "'";

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
