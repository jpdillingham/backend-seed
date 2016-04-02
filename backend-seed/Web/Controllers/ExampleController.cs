using NLog;
using System.Web.Http;
using System;

namespace backend_seed.Web.Controllers
{
    /// <summary>
    /// Example Web API controller class.
    /// </summary>
    public class ExampleController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Example API method.
        /// </summary>
        /// <returns></returns>
        [Route("api/example")]
        [HttpGet]
        public DateTime GetExample()
        {
            logger.Info("[ExampleController]: API Request: " + Request.RequestUri);
            return DateTime.Now;
        }
    }
}
