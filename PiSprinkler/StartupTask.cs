using Windows.ApplicationModel.Background;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using SprinklerCore;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace PiSprinkler
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static SprinklerController _sprinklerController;
        private HttpServer _httpServer;
        private BackgroundTaskDeferral _deferral;

        internal static SprinklerController SprinklerController
        {
            get
            {
                return _sprinklerController;
            }
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
           _deferral = taskInstance.GetDeferral();

            _sprinklerController = new SprinklerController();
            _sprinklerController.RunScheduler();

            var restRouteHandler = new RestRouteHandler();

            restRouteHandler.RegisterController<Sprinkler>();
            
            var configuration = new HttpServerConfiguration()
                .ListenOnPort(80)
                .RegisterRoute("api",restRouteHandler);
     
            var httpServer = new HttpServer(configuration);
            _httpServer = httpServer;

            await httpServer.StartServerAsync();

            // Dont release deferral, otherwise app will stop
        }
    }
}
