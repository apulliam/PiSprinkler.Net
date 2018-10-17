using Windows.ApplicationModel.Background;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;
using SprinklerDotNet;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace PiSprinkler
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static SprinklerBase _sprinkler;
        private HttpServer _httpServer;
        private BackgroundTaskDeferral _deferral;

        internal static SprinklerBase Sprinkler
        {
            get
            {
                return _sprinkler;
            }
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
           _deferral = taskInstance.GetDeferral();

            _sprinkler = new Sprinkler();
            await _sprinkler.Initialize();
            await _sprinkler.StartScheduler();

            var restRouteHandler = new RestRouteHandler();

            restRouteHandler.RegisterController<SprinklerController>();
            
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
