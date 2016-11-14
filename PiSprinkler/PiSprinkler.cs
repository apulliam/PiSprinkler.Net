using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using SprinklerCore;

namespace PiSprinkler
{

    [RestController(InstanceCreationType.PerCall)]
    public sealed class Sprinkler
    {
        [UriFormat("/cycles")]
        public IGetResponse GetAllWateringCycles()
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                StartupTask.SprinklerController.GetAllWateringCycles());
        }

        [UriFormat("/cycles")]
        public IDeleteResponse ClearWateringCycles()
        {
            StartupTask.SprinklerController.ClearWateringCycles();
            return new DeleteResponse(DeleteResponse.ResponseStatus.OK);
        }

        [UriFormat("/cycles/{id}")]
        public IGetResponse GetAllWateringCycle(string id)
        {
            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                StartupTask.SprinklerController.GetWateringCycle(Guid.Parse(id)));
        }


        [UriFormat("/cycles/zones/{zoneNumber}")]
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
        [UriFormat("/cycles/{id}")]
        public IDeleteResponse DeleteWateringCycle(string id)
        {
            var response = StartupTask.SprinklerController.DeleteWateringCycle(Guid.Parse(id)) ? DeleteResponse.ResponseStatus.OK : DeleteResponse.ResponseStatus.NotFound;
            return new DeleteResponse(response);
        }

        [UriFormat("/cycles")]
        public IPostResponse AddWateringCycle([FromContent] CycleConfig cycle)
        {
            var cycleId = StartupTask.SprinklerController.AddWateringCycle(cycle.ToInternal());
            return new PostResponse(PostResponse.ResponseStatus.Created, $"{cycleId}");
        }

        [UriFormat("/zones/{id}/start")]
        public IPutResponse StartZone(string id)
        {
            StartupTask.SprinklerController.StartZone(int.Parse(id));
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }

        [UriFormat("/zones/{id}/stop")]
        public IPutResponse StopZone(string id)
        {
            StartupTask.SprinklerController.StopZone(int.Parse(id));
            return new PutResponse(PutResponse.ResponseStatus.OK);
        }
    }
}
