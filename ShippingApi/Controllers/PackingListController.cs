using ShippingApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace ShippingApi.Controllers
{
    public class PackingListController : ApiController
    {
        [TokenAuthorize]
        public async Task<IHttpActionResult> Get(string ht_hf = "", string contract_no = "", string release_no = "")
        {
            string sql = "EXEC SP_PackingListRpt @ht_hf='" + ht_hf.Replace("'","''") + "',@contract_no='" + contract_no.Replace("'", "''") + "',@release_no='" + release_no.Replace("'", "''") + "'";

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
        public async Task<IHttpActionResult> Get(string list_name)
        {
            string sql = string.Empty;
            switch (list_name.ToLower())
            {
                case "contract_no":
                    sql = "SELECT distinct contract_no FROM A_Shipping ";
                    break;
                case "release_no":
                    sql = "SELECT distinct release_no FROM A_Shipping ";
                    break;
                default:
                    sql = "SELECT distinct ht_hf FROM A_Shipping WHERE ht_hf is not null ";
                    break;
            }

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
