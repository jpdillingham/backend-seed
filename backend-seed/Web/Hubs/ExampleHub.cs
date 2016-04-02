using Microsoft.AspNet.SignalR;
using NLog;
using System.Threading.Tasks;

namespace backend_seed.Web.Hubs
{
    /// <summary>
    /// Example SignalR Hub class.
    /// </summary>
    public class ExampleHub : Hub
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Event called when a new client connects to the hub.
        /// </summary>
        /// <returns>A Task used for asynchronous calls.</returns>
        public override Task OnConnected()
        {
            logger.Info("[ExampleHub]: Client connected.");
            return base.OnConnected();
        }

        /// <summary>
        /// Called when a client disconnects from the hub.
        /// </summary>
        /// <param name="stopCalled">True if the connection was intentionally stopped with Stop(), false otherwise.</param>
        /// <returns>A Task used for asynchronous calls.</returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            logger.Info("[ExampleHub]: Client disconnected.");
            return base.OnDisconnected(stopCalled);
        }

        /// <summary>
        /// Called when a client reconnects to the hub after having previously disconnected.
        /// </summary>
        /// <returns>A Task used for asynchronous calls.</returns>
        public override Task OnReconnected()
        {
            logger.Info("[ExampleHub]: Client reconnected.");
            return base.OnReconnected();
        }

        /// <summary>
        /// Called from the HubManager event proxy; called when a new message is added to the Logger.
        /// </summary>
        /// <param name="sender">The object that raised the original Changed event.</param>
        /// <param name="e">The event arguments.</param>
        public void Read(object value)
        {
            Clients.Group("Example").read(value);
        }

        /// <summary>
        /// Subscribes the calling client to the Hub.
        /// </summary>
        /// <param name="arg">Required parameter; method signature must match the invocation from javascript.</param>
        public void Subscribe(object arg)
        {
            logger.Info("[ExampleHub]: Client '" + Context.ConnectionId + "' is now subscribed.");
            Groups.Add(Context.ConnectionId, "Example");
            Clients.Caller.subscribeSuccess("Example");
        }

        /// <summary>
        /// Unsubscribes the calling client from the Hub.
        /// </summary>
        /// <param name="arg">Required parameter; method signature must match the invocation from javascript.</param>
        public void Unsubscribe(object arg)
        {
            logger.Info("[ExampleHub]: Client '" + Context.ConnectionId + "' unsubscribed.");
            Groups.Remove(Context.ConnectionId, "Example");
            Clients.Caller.unsubscribeSuccess("Example");
        }
    }
}
