using ShippingApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace ShippingApi.Controllers
{
    public class InvoiceController : ApiController
    {
        [TokenAuthorize]
        public async Task<IHttpActionResult> Get(string ht_hf = "", string contract_no = "", string release_no = "")
        {
            string sql = "EXEC SP_InvoiceRpt @ht_hf='" + ht_hf.Replace("'","''") + "',@contract_no='" + contract_no.Replace("'", "''") + "',@release_no='" + release_no.Replace("'", "''") + "'";

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
