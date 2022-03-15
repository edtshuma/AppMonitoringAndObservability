
using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Models;
using PlatformsService.SyncDataService.Http;
using System.Threading.Tasks;
using PlatformService.Dtos;
using PlatformService.AsyncDataServices;
using Microsoft.Extensions.Logging;

namespace PlatformService
{
    //ControllerBase is fo APIs
    [Route("api/[controller]")]
    [ApiController]
   public class PlatformsController : ControllerBase {
        private readonly IMapper _mapper;
        private IPlatformRepo _repository;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;
        private readonly ILogger<PlatformsController> _logger;

        public PlatformsController(IPlatformRepo repository, 
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient,
        ILogger<PlatformsController> logger
       )
       {
           _mapper = mapper;        
           //_logger = logger;
           _repository =repository;
           _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
       public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms() {
           
            _logger.LogInformation("Called GetAllPlatforms API");
            Console.WriteLine("Getting all platforms ...");

        var platformItems= _repository.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
       }



         [HttpGet("{id}", Name="GetPlatformById")]
       public ActionResult<PlatformReadDto> GetPlatformById(int id) {
           
            _logger.LogInformation("Called GetPlatformById API");
            Console.WriteLine("Getting all platforms ...");

        var platformItem = _repository.GetPlatformById(id);

        if (platformItem != null) {

           return Ok(_mapper.Map<PlatformReadDto>(platformItem));
        }

        return NotFound();
       }



       [HttpPost]
       public async Task<ActionResult<PlatformCreateDto>> CreatePlatform(PlatformCreateDto platformCreateDto) {

            _logger.LogInformation("Called CreatePlatform API");

            var model = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(model);           
            _repository.SaveChanges();
           
            var platformReadDto = _mapper.Map<PlatformReadDto>(model);
            //Send Sync message
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }

            catch (Exception ex)
            {

                Console.WriteLine($"Could not send synchronoulsy: { ex.Message}");
                _logger.LogError($"Something went wrong inside CreatePlatform action: {ex.Message}");
            }

            //Send Async message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }

            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong inside CreatePlatform action: {ex.Message}");
                Console.WriteLine($"Could not send asynchronoulsy: { ex.Message}");
            }

            //where can u access the newly created rsc  >> CreatedAtRoute
            //name of the route <<GetPlatformById
            return CreatedAtRoute(nameof(GetPlatformById), new { id = platformReadDto.Id }, platformReadDto);
        }

   }
}