using System;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using SprinklerCore;

namespace PiSprinkler
{
    [RestController(InstanceCreationType.PerCall)]
    internal class Sprinkler
    {
        [UriFormat("/programs")]
        public IGetResponse GetAllPrograms()
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                StartupTask.SprinklerController.GetAllPrograms());
        }

        [UriFormat("/programs")]
        public IDeleteResponse ClearAllPrograms()
        {
            StartupTask.SprinklerController.ClearAllPrograms();
            return new DeleteResponse(DeleteResponse.ResponseStatus.OK);
        }

        [UriFormat("/programs/{id}")]
        public IGetResponse GetProgram(string id)
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                StartupTask.SprinklerController.GetProgram(Guid.Parse(id)));
        }

        [UriFormat("/programs/zones/{zoneNumber}")]
        public IGetResponse GetWateringCyclesByZone(string zoneNumber)
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                StartupTask.SprinklerController.GetWateringCyclesByZone(int.Parse(zoneNumber)));
        }

        /// <summary>
        /// Make sure the number of parameters in your UriFormat match the parameters in your method and
        /// the names (case sensitive) and order are respected.
        /// </summary>
        [UriFormat("/programs/{id}")]
        public IDeleteResponse DeleteProgram(string id)
        {
            var response = StartupTask.SprinklerController.DeleteProgram(Guid.Parse(id)) ? DeleteResponse.ResponseStatus.OK : DeleteResponse.ResponseStatus.NotFound;
            return new DeleteResponse(response);
        }

        [UriFormat("/programs/{name}")]
        public IPostResponse AddProgram([FromContent] ProgramConfig programConfig)
        {
            var programId = StartupTask.SprinklerController.AddProgram(programConfig);
            return new PostResponse(PostResponse.ResponseStatus.Created, $"{programId}");
        }

        [UriFormat("/zones")]
        public IGetResponse GetZones()
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                StartupTask.SprinklerController.GetAllZones());
        }


        [UriFormat("/zones/{id}")]
        public IGetResponse GetZone(string id)
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                StartupTask.SprinklerController.GetZone(int.Parse(id)));
        }

        [UriFormat("/zones/{id}/start")]
        public IPutResponse StartZone(string id)
        {
            StartupTask.SprinklerController.StartZone(int.Parse(id));
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }

        [UriFormat("/zones/stop")]
        public IPutResponse StopZone()
        {
            StartupTask.SprinklerController.StopZone();
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }
    }
}
