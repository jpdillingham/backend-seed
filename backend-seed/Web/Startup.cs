using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using System.Web.Http;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.FileSystems;
using Microsoft.AspNet.SignalR;
using NLog;
using System.Web.Http.Routing;
using System.Web.Http.Controllers;

namespace backend_seed.Web
{
    /// <summary>
    /// The Owin startup class for the application.
    /// </summary>
    public class Startup
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Owin configuration method.
        /// </summary>
        /// <param name="app">The IAppBuilder instance injected by WebApp.Start()</param>
        public void Configuration(IAppBuilder app)
        {
            // fetch the web root and web directory settings from the config file.

            // webroot appends a root folder to the URLs for the web server, for example, if webroot is "backend",
            // the index would reside at http://localhost/backend, all API calls would be based from http://localhost/backend/api, etc.
            // this is useful for placing the application behind a rerverse proxy.
            string webRoot = Utility.GetSetting("WebRoot");

            // webdirectory determines which directory is used as the web root for static files.  generally this doesn't need to be changed, 
            // and if it is changed the folder for static content needs to be moved in the post build event for the project to match.
            // to ensure cross-platform compatibility, the webDirectory setting should use the pipe character (|) instead of forward or
            // backward slashes to indicate a directory separator.  This will be replaced in the code below.
            string webDirectory = Utility.GetSetting("WebDirectory").Replace('|', System.IO.Path.DirectorySeparatorChar);

            logger.Info("Starting the Web subsystem at root '" + webRoot + "'.  Serving static content from '" + webDirectory + "'.");

            // enable CORS to allow access to the web server from other domains (e.g. not localhost)
            app.UseCors(CorsOptions.AllowAll);
            
            // map signalr using webroot
            app.MapSignalR((webRoot.Length > 0 ? "/" : "") + webRoot + "/signalr", new HubConfiguration());

            // configure the web API to use attribute routing with a custom route provider
            // this is required to append webroot to api routes programatically (and automatically)
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes(new APIPrefixProvider(webRoot));
            app.UseWebApi(config);

            // configure the static file server.  serve files from the directory specified in the config
            // modify the requestpath to use webroot.
            app.UseFileServer(new FileServerOptions()
            {
                FileSystem = new PhysicalFileSystem(webDirectory),
                RequestPath = PathString.FromUriComponent((webRoot.Length > 0 ? "/" : "") + webRoot)
            });
        }
    }

    /// <summary>
    /// Custom route provider class to allow the global webroot to be appended to API routes.
    /// </summary>
    public class APIPrefixProvider : DefaultDirectRouteProvider
    {
        private string prefix;

        /// <summary>
        /// Default constructor.  Creates an instance of the class with the supplied prefix string.
        /// </summary>
        /// <param name="prefix">The desired prefix for API routes.</param>
        public APIPrefixProvider(string prefix)
        {
            this.prefix = prefix;
        }

        /// <summary>
        /// Returns the custom route prefix.
        /// </summary>
        /// <param name="controllerDescriptor"></param>
        /// <returns>The custom route prefix.</returns>
        protected override string GetRoutePrefix(HttpControllerDescriptor controllerDescriptor)
        {
            string existingPrefix = base.GetRoutePrefix(controllerDescriptor);
            if (existingPrefix == default(string))
                return prefix;
            else
                return prefix + "/" + existingPrefix;
        }
    }
}
