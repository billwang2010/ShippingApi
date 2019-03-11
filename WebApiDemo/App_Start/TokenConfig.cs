using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebApiDemo
{
    public class TokenAuthorize : System.Web.Http.AuthorizeAttribute
    {
        private string token = ConfigurationManager.ConnectionStrings["Token"].ConnectionString;

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //get token
            var content = actionContext.Request.Properties["MS_HttpContext"] as HttpContextBase;
            var reqToken = content.Request.Headers["Token"];
            if (!string.IsNullOrEmpty(reqToken))
            {
                if (reqToken == token)
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
