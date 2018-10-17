using System;
using System.Threading.Tasks;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using SprinklerDotNet;
using SprinklerDotNet.Config;

namespace PiSprinkler
{
    [RestController(InstanceCreationType.PerCall)]
    internal class SprinklerController
    {
        
        [UriFormat("/programs")]
        public async Task<IGetResponse> GetAllPrograms()
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                await StartupTask.Sprinkler.GetPrograms());
        }

        //[UriFormat("/programs")]
        //public async Task<IDeleteResponse> ClearAllPrograms()
        //{
        //    await StartupTask.Sprinkler.ClearAllPrograms();
        //    return new DeleteResponse(DeleteResponse.ResponseStatus.OK);
        //}

        //[UriFormat("/programs/{id}")]
        //public async Task<IGetResponse> GetProgram(string id)
        //{
        //    return new GetResponse(
        //        GetResponse.ResponseStatus.OK,
        //        await StartupTask.Sprinkler.GetProgram(Guid.Parse(id)));
        //}

        //[UriFormat("/cycles")]
        //public async Task<IGetResponse> GetAllWateringCycles()
        //{
        //    return new GetResponse(
        //        GetResponse.ResponseStatus.OK,
        //        await StartupTask.Sprinkler.GetAllWateringCycles());
        //}

        //[UriFormat("/cycles/{zoneNumber}")]
        //public async Task<IGetResponse> GetWateringCyclesByZone(string zoneNumber)
        //{
        //    return new GetResponse(
        //        GetResponse.ResponseStatus.OK,
        //        await StartupTask.Sprinkler.GetWateringCyclesByZone(int.Parse(zoneNumber)));
        //}

        ///// <summary>
        ///// Make sure the number of parameters in your UriFormat match the parameters in your method and
        ///// the names (case sensitive) and order are respected.
        ///// </summary>
        //[UriFormat("/programs/{id}")]
        //public async Task<IDeleteResponse> DeleteProgram(string id)
        //{
        //    var response = (await StartupTask.Sprinkler.DeleteProgram(Guid.Parse(id))) ? DeleteResponse.ResponseStatus.OK : DeleteResponse.ResponseStatus.NotFound;
        //    return new DeleteResponse(response);
        //}

        //[UriFormat("/programs")]
        //public async Task<IPostResponse> AddProgram([FromContent] SprinklerCore.Config.SprinklerConfig sprinklerConfig)
        //{
        //    var programId = await StartupTask.Sprinkler.AddProgram(sprinklerConfig);
        //    return new PostResponse(PostResponse.ResponseStatus.Created, $"{programId}");
        //}

        [UriFormat("/zones")]
        public async Task<IGetResponse> GetZones()
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                await StartupTask.Sprinkler.GetZones());
        }


        //[UriFormat("/zones/{id}")]
        //public async Task<IGetResponse> GetZone(string id)
        //{
        //    return new GetResponse(
        //        GetResponse.ResponseStatus.OK,
        //        await StartupTask.Sprinkler.GetZone(int.Parse(id)));
        //}

        //[UriFormat("/zones/{id}/start")]
        //public async Task<IPutResponse> StartZone(string id)
        //{
        //    await StartupTask.Sprinkler.StartZone(int.Parse(id));
        //    return new PutResponse(PutResponse.ResponseStatus.OK);
        //}

        //[UriFormat("/zones/stop")]
        //public async Task<IPutResponse> StopZone()
        //{
        //    await StartupTask.Sprinkler.StopZone();
        //    return new PutResponse(PutResponse.ResponseStatus.OK);
        //}
    }
}
