using System.Threading.Tasks;
using PlatformService.Dtos;

namespace PlatformsService.SyncDataService.Http

{
    public interface ICommandDataClient
    {
        Task SendPlatformToCommand(PlatformReadDto plat);
    }
}