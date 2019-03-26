using ShippingApi.Models;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ShippingApi
{
    public class TokenAuthorize : System.Web.Http.AuthorizeAttribute
    {
        private string dbToken = string.Empty;

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //get token
            var content = actionContext.Request.Properties["MS_HttpContext"] as HttpContextBase;
            string reqToken = content.Request.Headers["Token"];
            dbToken = SqlMapper.GetValue("select top 1 Token from A_Login with(nolock) where Token='" + reqToken.Replace("'", "''") + "' and LogoutTime IS NULL order by LoginTime desc");

            if (!string.IsNullOrEmpty(reqToken))
            {
                if (reqToken == dbToken)
                {
                    base.IsAuthorized(actionContext);
                }
                else
                {
                    var response = new HttpResponseMessage();
                    response.Content = new StringContent("Unauthorized token.");
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    throw new HttpResponseException(response);
                }
            }
            else
                HandleUnauthorizedRequest(actionContext);
        }
    }
}